#region License
#endregion

using System;
using System.Diagnostics;

namespace Aspid.Core
{
    /// <summary>
    /// Used to log messages
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the name assigned to the logger upon creation.
        /// </summary>
        /// <value>The logger name.</value>
        string Name { get; }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        void Log(string message, TraceEventType severity);

        /// <summary>
        /// Logs the specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogDebug(string message);

        /// <summary>
        /// Logs the specified information message.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogInformation(string message);

        /// <summary>
        /// Logs the specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogError(string message);

        /// <summary>
        /// Logs the specified critical message.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogCritical(string message);

        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void LogException(Exception exception);
    }
}
