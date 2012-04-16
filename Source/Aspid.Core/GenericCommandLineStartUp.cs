#region License
#endregion

using System;
using System.Diagnostics.CodeAnalysis;

namespace Aspid.Core
{
    /// <summary>
    /// ABC for command-line setup->run type of applications
    /// </summary>
    ///<remarks>
    /// Virtual method are there to help extensibility, but derived classes are only supposed to implement abstracts.
    /// Override virtuals at your own peril.
    ///</remarks>
    public abstract class GenericCommandLineStartup
    {
        static ILogger logger = Logging.GetLogger(typeof(GenericCommandLineStartup));

        /// <summary>
        /// Provides a way to determinate an exit code for an uncaught exception on application setup.
        /// Default delegate/implementation returns 1 for any exception.
        /// </summary>
        /// <remarks>Implementors sohuld take notice that there's not a null-check for the delegate on execution.</remarks>
        private Func<Exception, int> getReturnCodeForSetupOnUncaughtException = (x => 1);
        protected Func<Exception, int> GetReturnCodeForSetupOnUncaughtException
        {
            get { return getReturnCodeForSetupOnUncaughtException; }
            set { getReturnCodeForSetupOnUncaughtException = value; }
        }

        /// <summary>
        /// Provides a way to determinate an exit code for an uncaught exception on application run.
        /// Default delegate/implementation returns 1 for any exception.
        /// </summary>
        /// <remarks>Implementors sohuld take notice that there's not a null-check for the delegate on execution.</remarks>
        private Func<Exception, int> getReturnCodeForRunOnUncaughtException = (x => 1);
        protected Func<Exception, int> GetReturnCodeForRunOnUncaughtException
        {
            get { return getReturnCodeForRunOnUncaughtException; }
            set { getReturnCodeForRunOnUncaughtException = value; }
        }

        /// <summary>
        /// Executes initialization logic and check pre-conditions for running the application.
        /// </summary>
        /// <returns>0 if setup was successful, implementation dependant error code otherwise</returns>
        /// Specifically made to use a return code on exception:
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public virtual int Setup(string[] args)
        {
            try
            {
                return ApplicationSetup(args ?? new string[0]);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return GetReturnCodeForSetupOnUncaughtException(ex);
            }
        }

        /// <summary>
        /// Runs the application process.
        /// </summary>
        /// <returns>0 if run was successful, implementation dependant error code otherwise</returns>
        /// Specifically made to use a return code on exception:
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public virtual int Run()
        {
            try
            {
                return ApplicationRun();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return GetReturnCodeForRunOnUncaughtException(ex);
            }
        }

        /// <summary>
        /// Tries to setup the application and run it.
        /// </summary>
        /// <returns>
        /// 0 if succesful, implementation dependant error code otherwise
        /// </returns>
        public virtual int SetupAndRun()
        {
            return SetupAndRun(null);
        }

        /// <summary>
        /// Tries to setup the application and run it.
        /// </summary>
        /// <param name="args">The command-line args.</param>
        /// <returns>
        /// 0 if succesful, implementation dependant error code otherwise
        /// </returns>
        public virtual int SetupAndRun(string[] args)
        {
            var setUpResult = Setup(args);
            return (setUpResult == 0) ? Run() : setUpResult;
        }

        /// <summary>
        /// When implemented on a derived class should execute initialization logic and check pre-conditions for running the application.
        /// </summary>
        /// <param name="args">The command-line args, it won't be a null array but may still be empty/zero-sized.</param>
        /// <returns>
        /// 0 if setup was successful, implementation dependant error code otherwise
        /// </returns>
        protected abstract int ApplicationSetup(string[] args);

        /// <summary>
        /// When implemented on a derived class sohuld run the application process.
        /// </summary>
        /// <returns>0 if run was successful, implementation dependant error code otherwise</returns>
        protected abstract int ApplicationRun();
    }
}
