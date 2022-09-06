using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace DirTemplateExtension
{
    internal class DirectoryUtils
    {
        public static bool IsSpecialFolder(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            return Enum.GetValues(typeof(Environment.SpecialFolder)).Cast<Environment.SpecialFolder>()
                .Any(currentDir => directoryInfo.FullName == Environment.GetFolderPath(currentDir));
        }

        public static bool IsEqualOrChildOf(DirectoryInfo targetDir, DirectoryInfo parentDir)
        {
            var isChildOrEqual = parentDir.FullName.Equals(targetDir.FullName);
            try
            {
                while (targetDir.Parent != null && !isChildOrEqual)
                {
                    if (targetDir.Parent.FullName.Equals(parentDir.FullName))
                    {
                        isChildOrEqual = true;
                        break;
                    }

                    targetDir = targetDir.Parent;
                }
            }
            catch
            {
                // ignored
            }

            return isChildOrEqual;
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, BackgroundWorker backgroundWorker = null)
        {
            foreach (var file in source.GetFiles())
            {
                if (file.Name.Equals(Configuration.ProjectImageFile, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Logger.Debug($"Copying file '{file.Name}' to '{target.FullName}'");
                if (backgroundWorker != null && backgroundWorker.CancellationPending) break;

                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (var dir in source.GetDirectories())
            {
                Logger.Debug($"Creating directory '{dir.Name}' in '{target.FullName}'");
                if (backgroundWorker != null && backgroundWorker.CancellationPending) break;

                var nextTargetSubDir = target.CreateSubdirectory(dir.Name);
                CopyAll(dir, nextTargetSubDir);
            }
        }

        public static bool IsDirectoryEmpty(DirectoryInfo dirInfo)
        {
            return !Directory.EnumerateFileSystemEntries(dirInfo.FullName).Any();
        }
    }
}