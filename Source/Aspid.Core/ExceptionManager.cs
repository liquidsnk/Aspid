#region License
#endregion

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Aspid.Core.Extensions;
using Aspid.Core.Utils;

namespace Aspid.Core
{
    public class CrashInformation
    {
        public DateTime DateTime { get; set; }

        public string DateTimeString { get; set; }

        public string MachineName { get; set; }

        public string OperatingSystem { get; set; }

        public Version CLRVersion { get; set; }

        public string LoggedUser { get; set; }

        public string UserDomain { get; set; }

        public string CodeBase { get; set; }

        public string CurrentDirectory { get; set; }

        public string Drives { get; set; }

        public int NumberOfProcessors { get; set; }

        public string ApplicationAssemblyName { get; set; }

        public string ApplicationAssemblyVersion { get; set; }

        public string InstalledFrameworkVersions { get; set; }

        public string CurrentStackTrace { get; set; }

        public string ExceptionMessages { get; set; }

        public string StackTrace { get; set; }
    }
    
    public class ExceptionCaughtEventArgs : EventArgs
    {
        public Exception Exception { get; set; }

        public CrashInformation CrashInformation { get; set; }

        public ExceptionCaughtEventArgs(Exception ex, CrashInformation crashInformation)
        {
            ex.ThrowIfNull("ex");
            crashInformation.ThrowIfNull("crashInformation");

            Exception = ex;
            CrashInformation = crashInformation;
        }
    }

    public class ExceptionManager
    {
        static ILogger logger = Logging.GetLogger("ExceptionManager");

        public static ExceptionManager Instance = new ExceptionManager();

        public void StartListeningForUnhandledExceptions()
        {
            logger.LogDebug("Started listening for unhandled exceptions");

            //Attachment for Winforms applications
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += ThreadExceptionManager;

            //Attachment for Console applications
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionManager;
        }

        void ThreadExceptionManager(object sender, ThreadExceptionEventArgs e)
        {
            UnhandledExceptionManager(e.Exception);
        }

        void UnhandledExceptionManager(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledExceptionManager(e.ExceptionObject as Exception);
        }

        void UnhandledExceptionManager(Exception ex)
        {
            logger.LogError("Unhandled exception caught");
            logger.LogException(ex);

            OnExceptionCaught(ex, new ExceptionCaughtEventArgs(ex, GetCrashReportingInformation(ex)));
        }

        public delegate void ExcepionCaughtEventHandler(object sender, ExceptionCaughtEventArgs e);
        public event ExcepionCaughtEventHandler ExceptionCaught;
        
        protected virtual void OnExceptionCaught(Exception ex, ExceptionCaughtEventArgs e)
        {
            var exceptionCaught = ExceptionCaught;
            if (exceptionCaught != null)
            {
                exceptionCaught(ex, e);
            }
        }

        public static CrashInformation GetCrashReportingInformation()
        {
            return GetCrashReportingInformation(null);
        }

        public static CrashInformation GetCrashReportingInformation(Exception ex)
        {
            var now = DateTime.Now;

            //gets entry assembly information
            var entryAssemblyInfo = Assembly.GetEntryAssembly();
            var entryAssemblyName = string.Empty;
            var entryAssemblyVersion = string.Empty;

            if (entryAssemblyInfo != null)
            {
                entryAssemblyName = entryAssemblyInfo.FullName;
                entryAssemblyVersion = entryAssemblyInfo.GetName().Version.ToString();
            }

            //gets exception messages and stack trace
            Exception currentException = ex;
            var exceptionMessage = string.Empty;
            var exceptionStackTrace = string.Empty;
            while (currentException != null)
            {
                string exceptionTypeName = currentException.GetType().Name;
                exceptionMessage += "{1}: {2}{0}".InvariantFormat("\n", exceptionTypeName, currentException.Message);
                exceptionStackTrace += "{1}: {2}{0}".InvariantFormat("\n", exceptionTypeName, currentException.StackTrace);
                currentException = currentException.InnerException;
            }

            return new CrashInformation
            {
                DateTime = now,
                DateTimeString = GetCurrentFormattedDate(now),
                MachineName = Environment.MachineName,
                OperatingSystem = Environment.OSVersion.VersionString,
                CLRVersion = Environment.Version,
                LoggedUser = Environment.UserName,
                UserDomain = Environment.UserDomainName,
                CodeBase = entryAssemblyInfo != null ? entryAssemblyInfo.CodeBase : string.Empty,
                CurrentDirectory = Environment.CurrentDirectory,
                Drives = ",".Join(Environment.GetLogicalDrives()),
                NumberOfProcessors = Environment.ProcessorCount,
                ApplicationAssemblyName = entryAssemblyName,
                ApplicationAssemblyVersion = entryAssemblyVersion,
                InstalledFrameworkVersions = ";".Join(FrameworkUtils.GetInstalledFrameworkVersions().Select(x => x.ToString())),
                CurrentStackTrace = Environment.StackTrace,
                ExceptionMessages = exceptionMessage,
                StackTrace = exceptionStackTrace
            };
        }

        static string GetCurrentFormattedDate(DateTime dateTime)
        {
            return "{0} - GMT: {1}".InvariantFormat(dateTime.ToString("yyyy/MM/dd hh:mm:ss"), dateTime.ToString("zzz"));
        }
    }
}
