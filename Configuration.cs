using System;
using System.IO;

namespace DirTemplateExtension
{
    internal class Configuration
    {
        private const string TemplateDirName = "DirTemplates";

        public static DirectoryInfo GetTemplateDirectoryInfo()
        {
            var dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                TemplateDirName);
            return new DirectoryInfo(dirPath);
        }

        public const string EventLogSource = "DirTemplateExtension";
        public const string ProjectImageFile = "project.png";
    }
}