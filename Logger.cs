using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DirTemplateExtension
{
    public class Logger
    {
        private static readonly string LogName;

        static Logger()
        {
            LogName = Assembly.GetExecutingAssembly().FullName;
        }

        public static void WriteLog(string message, EventLogEntryType eventType = EventLogEntryType.Information)
        {
            try
            {
                using (var eventLog = new EventLog())
                {
                    eventLog.Source = Configuration.EventLogSource;
                    eventLog.WriteEntry($"{message} [{LogName}]", eventType);
                }
            }
            catch (Exception exception)
            {
                if (eventType != EventLogEntryType.Information && eventType != EventLogEntryType.SuccessAudit)
                {
                    MessageBox.Show(string.Format(Resources.Logger_Exception, exception.Message, message),
                        Resources.Logger_WriteLog_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                Debug(message);
            }
        }

        public static void Debug(string message)
        {
            var templateDirPath = Configuration.GetTemplateDirectoryInfo().FullName;
            if (!File.Exists(Path.Combine(templateDirPath, ".debug")))
            {
                return;
            }

            var logFile = Path.Combine(templateDirPath, "debug.log");

            try
            {
                var append = File.Exists(logFile) && new FileInfo(logFile).Length < 10 * 1024 * 1024;
                using (var logWriter = new StreamWriter(logFile, append))
                {
                    logWriter.WriteLine(DateTime.Now + ">  " + message);
                    logWriter.Flush();
                }
            }
            catch (Exception exception)
            {
                // ignored
                MessageBox.Show(exception.ToString());
            }
        }
    }
}