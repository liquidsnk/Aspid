#region License
#endregion

using System;
using System.Diagnostics;

using NUnit.Framework;

namespace Aspid.Core.Tests
{
    [TestFixture]
    public class LoggingTests
    {
        static string lastOutput;

        #region NullLoggerFactory - Logger factory that returns null when asked to create loggers
        class NullLoggerFactory : ILoggerFactory
        {
            public ILogger GetLogger()
            {
                return null;
            }

            public ILogger GetLogger(string name)
            {
                return null;
            }
        }
        #endregion

        #region DummyLoggerFactory - Logger factory that returns dummy loggers that write to the lastOutput field of the LoggingTests class
        class DummyLoggerFactory : ILoggerFactory
        {
            class DummyLogger : ILogger
            {
                public void Log(string message, TraceEventType severity)
                {
                    LoggingTests.lastOutput = message;
                }

                public void LogDebug(string message)
                {
                    LoggingTests.lastOutput = message;
                }

                public void LogInformation(string message)
                {
                    LoggingTests.lastOutput = message;
                }

                public void LogError(string message)
                {
                    LoggingTests.lastOutput = message;
                }

                public void LogCritical(string message)
                {
                    LoggingTests.lastOutput = message;
                }

                public void LogException(Exception exception)
                {
                    LoggingTests.lastOutput = exception.Message;
                }

                public string Name
                {
                    get { return string.Empty; }
                }
            }

            public ILogger GetLogger()
            {
                return new DummyLogger();
            }

            public ILogger GetLogger(string name)
            {
                return new DummyLogger();
            }
        }
        #endregion

        [SetUp]
        public static void Setup()
        {
            //Clear the logger configuration
            Logging.ResetConfiguration();

            //Clear the string used by the DummyLogger to hold the last output.
            lastOutput = string.Empty;
        }

        [Test]
        public void GetLogger_WhenNotConfigured_ReturnsLoggerNullObject()
        {
            ILogger logger = Logging.GetLogger();
            
            Assert.IsNotNull(logger);
        }

        [Test]
        public void GetLogger_WhenNotConfigured_ReturnsUsableLoggerNullObject()
        {
            ILogger logger = Logging.GetLogger();

            Assert.DoesNotThrow(() => logger.LogDebug("test"));
            Assert.IsEmpty(lastOutput);
        }

        [Test]
        public void GetLogger_WhenConfiguredWithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Logging.Configure(null));
        }

        [Test]
        public void GetLogger_WhenConfiguredWithFactoryThatReturnsNull_ReturnsNotNullLogger()
        {
            Logging.Configure(new NullLoggerFactory());
            ILogger logger = Logging.GetLogger();

            Assert.IsNotNull(logger);
        }

        [Test]
        public void GetLogger_WhenConfigured_ReturnsNotNullLogger()
        {
            Logging.Configure(new DummyLoggerFactory());
            ILogger logger = Logging.GetLogger();

            Assert.IsNotNull(logger);
        }

        [Test]
        public void GetLogger_WhenConfigured_ReturnsUsableLogger()
        {
            Logging.Configure(new DummyLoggerFactory());
            ILogger logger = Logging.GetLogger();

            const string testString = "test";
            logger.LogDebug(testString);
            Assert.AreEqual(testString, lastOutput);
        }

        [Test]
        public void GetLogger_WhenStillNotConfigured_ReturnsLoggerThatBecomesUsefulAfterConfigured()
        {
            const string testString = "test";
            ILogger logger = Logging.GetLogger();
            
            //Logger is Null-Object Proxy, it doesn't actually logs anything
            logger.LogDebug(testString);
            Assert.IsEmpty(lastOutput);

            //We configure the static factory, logger should become usable
            Logging.Configure(new DummyLoggerFactory());

            logger.LogDebug(testString);
            Assert.AreEqual(testString, lastOutput);
        }

        [Test]
        public void GetLogger_WhenAlreadyConfigured_ReturnsLoggerThatBecomesNullObjectAfterResettingConfiguration()
        {
            const string testString = "test";
            Logging.Configure(new DummyLoggerFactory());
            ILogger logger = Logging.GetLogger();

            Logging.ResetConfiguration();

            logger.LogDebug(testString);
            Assert.IsEmpty(lastOutput);
        }

        [Test]
        public void GetLogger_WithNameWhenNotConfigured_ReturnsLoggerWithName()
        {
            const string logerName = "logger1";
            ILogger logger = Logging.GetLogger(logerName);

            Assert.AreEqual(logerName, logger.Name);
        }
        
        [Test]
        public void GetLogger_WithoutName_ReturnsLoggerThatUsesEmptyName()
        {
            ILogger logger = Logging.GetLogger();
            Assert.IsEmpty(logger.Name);
        }

        [Test]
        public void GetLogger_WithNullName_ReturnsLoggerThatUsesEmptyName()
        {
            ILogger logger = Logging.GetLogger((string)null);
            Assert.IsEmpty(logger.Name);
        }

        [Test]
        public void GetLogger_WithNameWhenConfigured_ReturnsLoggerWithName()
        {
            const string logerName = "logger1";
            Logging.Configure(new DummyLoggerFactory());
            ILogger logger = Logging.GetLogger(logerName);

            Assert.AreEqual(logerName, logger.Name);
        }

        [Test]
        public void GetLogger_WithType_ReturnsLoggerThatUsesAssemblyQualifiedNameAsName()
        {
            Type typeForLogger = typeof(DateTime);
            Logging.Configure(new DummyLoggerFactory());
            ILogger logger = Logging.GetLogger(typeForLogger);

            Assert.AreEqual(typeForLogger.AssemblyQualifiedName, logger.Name);
        }
        
        [Test]
        public void GetLogger_WithNullType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Logging.GetLogger((Type)null));
        }
    }
}
