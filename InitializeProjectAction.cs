using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DirTemplateExtension
{
    internal class InitializeProjectAction
    {
        private readonly DirectoryInfo _templateDir;
        private readonly List<DirectoryInfo> _targetDirs;
        private BackgroundWorker _worker;
        private FormProgress _formProgress;

        protected InitializeProjectAction(DirectoryInfo templateDir, List<DirectoryInfo> targetDirs)
        {
            _templateDir = templateDir;
            _targetDirs = targetDirs;
        }

        public void Start()
        {
            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = false,
                WorkerSupportsCancellation = true
            };
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            _formProgress = new FormProgress(_templateDir.Name, _worker);
            _formProgress.Show();
            _worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _formProgress.Invoke(new MethodInvoker(() => _formProgress.Close()));

            if (e.Error != null)
            {
                MessageBox.Show(Resources.InitializeProjectAction_Worker_ErrorOccurred,
                    Resources.InitializeProjectAction_Worker_ErrorOccurredTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (e.Cancelled)
            {
                MessageBox.Show(Resources.InitializeProjectAction_Worker_Canceled,
                    Resources.InitializeProjectAction_Worker_CanceledTitle, MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Logger.Debug("Starting worker");
                InitializeDirectories(_templateDir, _targetDirs);
            }
            catch (WorkCancelledException)
            {
                Logger.Debug("Caught work cancelled signal");
                e.Cancel = true;
                _worker.CancelAsync();
            }
        }

        private void InitializeDirectories(DirectoryInfo templateDir, IEnumerable<DirectoryInfo> targetDirs)
        {
            foreach (var targetDir in targetDirs.TakeWhile(targetDir => !_worker.CancellationPending))
            {
                Logger.Debug($"Processing target dir '{targetDir}'");

                if (!DirectoryUtils.IsDirectoryEmpty(targetDir))
                {
                    var dlgResult = MessageBox.Show(
                        string.Format(Resources.InitializeProjectAction_NotEmptyDirWarning, targetDir.FullName),
                        Resources.InitializeProjectAction_NotEmptyDirWarningTitle, MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning);
                    switch (dlgResult)
                    {
                        case DialogResult.No:
                            continue;
                        case DialogResult.Cancel:
                            throw new WorkCancelledException();
                    }
                }

                try
                {
                    DirectoryUtils.CopyAll(templateDir, targetDir, _worker);
                    Logger.WriteLog($"Initialized project \"{targetDir}\" from template \"{templateDir}\"");
                }
                catch (Exception exception) when (!(exception is WorkCancelledException))
                {
                    Logger.WriteLog(
                        $"An error occurred while initializing project \"{targetDir}\" from template \"{templateDir}\".\nException:\n{exception}",
                        EventLogEntryType.Error);
                    throw;
                }
            }

            if (_worker.CancellationPending)
            {
                throw new WorkCancelledException();
            }
        }

        public static InitializeProjectAction Build(string templateDir, List<string> targetDir)
        {
            return new InitializeProjectAction(new DirectoryInfo(templateDir),
                targetDir.ConvertAll(dirPath => new DirectoryInfo(dirPath)));
        }

        protected class WorkCancelledException : Exception
        {
        }
    }
}