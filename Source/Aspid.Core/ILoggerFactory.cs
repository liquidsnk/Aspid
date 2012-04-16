#region License
#endregion

namespace Aspid.Core
{
    /// <summary>
    /// Factory used to create Loggers
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Creates and returns a logger.
        /// </summary>
        /// <returns></returns>
        ILogger GetLogger();

        /// <summary>
        ///Creates and returns a named logger.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        ILogger GetLogger(string name);
    }
}
