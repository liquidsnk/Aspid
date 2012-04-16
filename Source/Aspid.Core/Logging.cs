#region License
#endregion

using System;
using System.Diagnostics;

using Aspid.Core.Extensions;

namespace Aspid.Core
{
    /// <summary>
    /// Provides a static single point of access used to create loggers.
    /// </summary>
    public static class Logging
    {
        #region Null-object Logger
        /// <summary>
        /// Null-object Logger
        /// </summary>
        class NonLoggingLogger : ILogger
	    {
            public void  Log(string message, TraceEventType severity)
            {
 	            return;
            }

            public void  LogDebug(string message)
            {
 	            return;
            }

            public void  LogInformation(string message)
            {
 	            return;
            }

            public void  LogError(string message)
            {
 	            return;
            }

            public void  LogCritical(string message)
            {
 	            return;
            }

            public void  LogException(Exception exception)
            {
 	            return;
            }

            public string Name
            {
                get { return null; }
            }
        }
        #endregion

        #region ProxyLoggerFactory, used to be able to Configure LoggerFactory after logger instantiation
        class LoggerFactoryProxy
        {
            readonly NonLoggingLogger nullLogger = new NonLoggingLogger();
            ILoggerFactory factory;
            
            public bool IsDirty { get; set; }

            public ILogger CreateLogger()
            {
                if (factory == null) return nullLogger;
                return factory.GetLogger();
            }

            public ILogger CreateLogger(string name)
            {
                if (factory == null) return nullLogger;
                return factory.GetLogger(name);
            }

            public void ResetLoggerFactory()
            {
                factory = null;
                IsDirty = true;
            }

            public void SetLoggerFactory(ILoggerFactory loggerFactory)
            {
                loggerFactory.ThrowIfNull("loggerFactory");
                factory = loggerFactory;
                IsDirty = true;
            }
        }
        #endregion

        #region ProxyLogger, used to be able to Configure LoggerFactory after logger instantiation
        class LoggerProxy : ILogger
        {
            LoggerFactoryProxy factoryProxy;
            string name = string.Empty;

            internal LoggerProxy(LoggerFactoryProxy factoryProxy)
                : this(factoryProxy, string.Empty)
            {
            }

            public LoggerProxy(LoggerFactoryProxy factoryProxy, Type typeForLogger)
            {
                factoryProxy.ThrowIfNull("factoryProxy");
                typeForLogger.ThrowIfNull("typeForLogger");

                this.factoryProxy = factoryProxy;
                this.name = typeForLogger.AssemblyQualifiedName;
            }

            internal LoggerProxy(LoggerFactoryProxy factoryProxy, string name)
            {
                factoryProxy.ThrowIfNull("factoryProxy");
                
                this.factoryProxy = factoryProxy;
                this.name = name ?? string.Empty;
            }

            ILogger realLogger;
            ILogger RealLogger 
            { 
                get
                {
                     if (factoryProxy.IsDirty) return realLogger = CreateRealLogger();
                     return realLogger ?? (realLogger = CreateRealLogger());
                }
            }

            ILogger CreateRealLogger()
            {
                return name.IsNullOrEmpty() ? 
                       factoryProxy.CreateLogger() : 
                       factoryProxy.CreateLogger(name);
            }

            public void Log(string message, TraceEventType severity)
            {
                RealLogger.Log(message, severity);
            }

            public void LogDebug(string message)
            {
                RealLogger.LogDebug(message);
            }

            public void LogInformation(string message)
            {
                RealLogger.LogInformation(message);
            }

            public void LogError(string message)
            {
                RealLogger.LogError(message);
            }

            public void LogCritical(string message)
            {
                RealLogger.LogCritical(message);
            }

            public void LogException(Exception exception)
            {
                RealLogger.LogException(exception);
            }

            public string Name
            {
                get { return name; }
            }
        }
        #endregion

        static readonly LoggerFactoryProxy factoryProxy = new LoggerFactoryProxy();

        /// <summary>
        /// Configures the LoggerFactory to use the specified ILoggerFactory to instantiate/configure loggers.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public static void Configure(ILoggerFactory loggerFactory)
        {
            loggerFactory.ThrowIfNull("loggerFactory");
            factoryProxy.SetLoggerFactory(loggerFactory);
        }

        /// <summary>
        /// Resets the logger configuration to the default.
        /// (Loggers basically become null-objects)
        /// </summary>
        public static void ResetConfiguration()
        {
            factoryProxy.ResetLoggerFactory();
        }

        /// <summary>
        /// Creates and returns a logger.
        /// </summary>
        /// <returns></returns>
        public static ILogger GetLogger()
        {
            return new LoggerProxy(factoryProxy);
        }

        /// <summary>
        /// Creates and returns a named logger.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static ILogger GetLogger(string name)
        {
            return new LoggerProxy(factoryProxy, name);
        }

        /// <summary>
        /// Creates and returns a logger for a given type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static ILogger GetLogger(Type typeForLogger)
        {
            return new LoggerProxy(factoryProxy, typeForLogger);
        }
    }
}
