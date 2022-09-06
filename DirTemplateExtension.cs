using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace DirTemplateExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    [COMServerAssociation(AssociationType.DirectoryBackground)]
    [COMServerAssociation(AssociationType.Folder)]
    public class DirTemplateExtension : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            Logger.Debug(
                $"CanShowMenu result for FolderPath: ({IsSupportedDirectory(FolderPath)})['{string.Join("; ", FolderPath)}']; result for SelectedItemPaths: ({SelectedItemPaths.All(IsSupportedDirectory)}) ['{string.Join("; ", SelectedItemPaths)}']");
            var selectedDirCount = SelectedItemPaths.Count();

            return IsSupportedDirectory(FolderPath) || (selectedDirCount > 0 &&
                                                        SelectedItemPaths.Count(IsSupportedDirectory) ==
                                                        selectedDirCount);
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip
            {
                RenderMode = ToolStripRenderMode.System
            };

            var directory = Configuration.GetTemplateDirectoryInfo();
            var itemRootMenu = CreateItemMenu(Resources.ContextMenu_CreateProject,
                Resources.ContextMenu_CreateProjectTooltip, Resources.InitProject);
            menu.Items.Add(itemRootMenu);

            var dirIndex = 1;
            foreach (var directoryInfo in directory.EnumerateDirectories())
            {
                var itemImage = LoadProjectIcon(directoryInfo);
                var itemDir = CreateItemMenu(directoryInfo.Name,
                    string.Format(Resources.ContextMenu_ItemTooltip, directoryInfo.FullName), itemImage);
                itemDir.Tag = directoryInfo.FullName;

                // Define shortcut keys for first F1-12 keys.
                RegisterItemShortcutKey(dirIndex, itemDir);

                itemDir.Click += (sender, args) => InitializeDirFromTemplate((string)((ToolStripMenuItem)sender).Tag);

                itemRootMenu.DropDownItems.Add(itemDir);
                dirIndex++;
            }

            if (itemRootMenu.DropDownItems.Count > 0)
            {
                var separatorItem = new ToolStripSeparator();
                itemRootMenu.DropDownItems.Add(separatorItem);
            }

            var templateDirMenuItem = CreateItemMenu(Resources.ContextMenu_OpenTemplateDir);
            templateDirMenuItem.Click += (sender, args) => OpenTemplatesDirectory();
            itemRootMenu.DropDownItems.Add(templateDirMenuItem);

            return menu;
        }

        private static void RegisterItemShortcutKey(int keyIndex, ToolStripMenuItem itemDir)
        {
            if (keyIndex >= 13 || !Enum.TryParse($"F{keyIndex}", out Keys parsedKey))
            {
                return;
            }

            try
            {
                itemDir.ShowShortcutKeys = true;
                itemDir.ShortcutKeys = Keys.LWin | Keys.Control | Keys.LShiftKey | parsedKey;
                itemDir.ShortcutKeyDisplayString = $"Ctrl + LWin + LShift + F{keyIndex}";
            }
            catch
            {
                Logger.WriteLog("Unable to register shortcut key: F" + keyIndex);
            }
        }

        private static Image LoadProjectIcon(DirectoryInfo directoryInfo)
        {
            var projectImagePath = Path.Combine(directoryInfo.FullName, Configuration.ProjectImageFile);

            if (!File.Exists(projectImagePath))
            {
                return null;
            }

            Image itemImage = null;

            try
            {
                using (var image = Image.FromFile(projectImagePath))
                {
                    itemImage = ImageUtils.ResizeImage(image, 16, 16);
                }
            }
            catch (Exception exception)
            {
                Logger.WriteLog(
                    $"Unable to read project \"{directoryInfo.FullName}\" image.\nException: {exception}",
                    EventLogEntryType.Error);
            }

            return itemImage;
        }

        private bool IsSupportedDirectory(string path)
        {
            return !string.IsNullOrEmpty(path)
                   && Directory.Exists(path)
                   && !DirectoryUtils.IsSpecialFolder(path)
                   && !DirectoryUtils.IsEqualOrChildOf(new DirectoryInfo(path),
                       Configuration.GetTemplateDirectoryInfo());
        }

        private static ToolStripMenuItem CreateItemMenu(string text, string tooltip = "", Image image = null)
        {
            return new ToolStripMenuItem
            {
                Text = text,
                ToolTipText = tooltip,
                Image = image,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageScaling = ToolStripItemImageScaling.None,
                Margin = new Padding(15, 0, 0, 0),
                TextAlign = ContentAlignment.TopLeft,
                ImageAlign = ContentAlignment.MiddleLeft
            };
        }

        private void OpenTemplatesDirectory()
        {
            var templateDirInfo = Configuration.GetTemplateDirectoryInfo();

            try
            {
                if (!templateDirInfo.Exists)
                {
                    throw new DirectoryNotFoundException("Unable to open template dir");
                }

                Process.Start(new ProcessStartInfo()
                {
                    FileName = templateDirInfo.FullName,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception exception)
            {
                var message = string.Format(Resources.TemplateDir_OpenError, templateDirInfo.FullName);
                Logger.WriteLog(message + $"\nException: {exception}", EventLogEntryType.Error);
                MessageBox.Show(message, Resources.TemplateDir_OpenErrorBoxTitle, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void InitializeDirFromTemplate(string templateDir)
        {
            Logger.Debug(
                $"InitializeDirFromTemplate for SelectedItemPaths['{string.Join(", ", SelectedItemPaths)}'] and FolderPath ({FolderPath})");
            InitializeProjectAction.Build(templateDir, SelectedItemPaths.ToList())
                .Start();
        }
    }
}