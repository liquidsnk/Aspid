#region License
#endregion

using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace Aspid.Core.WinForms
{
    public class Splash : IDisposable
    {
        private static readonly ILogger logger = Logging.GetLogger(typeof(Splash));

        Form SplashForm { get; set; }
        Stopwatch Time { get; set; }
        int MinimumTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Splash"/> class.
        /// </summary>
        /// <param name="splashForm">The splash form.</param>
        /// <remarks>The form will remain visible from construction of the Splash object, until Close (or Dispose) is called</remarks>
        public Splash(Form splashForm)
            : this(splashForm, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Splash"/> class.
        /// </summary>
        /// <param name="splashForm">The splash form.</param>
        /// <param name="minimumTime">The minimum time to display the splash form.</param>
        public Splash(Form splashForm, int minimumTime)
        {
            if (splashForm == null) throw new ArgumentNullException("splashForm", "splashForm is null.");

            SplashForm = splashForm;
            MinimumTime = minimumTime;
            SplashForm = splashForm;
            MinimumTime = minimumTime;

            ThreadPool.QueueUserWorkItem(x =>
            {
                Application.Run(SplashForm);
            });

            Time = new Stopwatch();
            Time.Start();
        }

        delegate void EmtpyDelegate();
        /// <summary>
        /// Closes the splash form if the MinimumTime has passed already, or put the Thread to sleep until it does.
        /// </summary>
        public void Close()
        {
            Time.Stop();
            if (Time.ElapsedMilliseconds < MinimumTime)
            {
                int remainingTime = (int)(MinimumTime - Time.ElapsedMilliseconds);
                Thread.Sleep(remainingTime);
            }

            try
            {
                SplashForm.Invoke(new EmtpyDelegate(SplashForm.Close));
            }
            catch (InvalidOperationException ex)
            {
                logger.LogException(ex);
                logger.LogError("The Splash Close was executed before the splash form window Handle was created.");
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Close();
        }
    }
}
