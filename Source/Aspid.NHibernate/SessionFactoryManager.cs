#region License
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Aspid.Core;
using Aspid.Core.Extensions;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;

namespace Aspid.NHibernate
{
    /// <summary>
    /// Default implementation of INHibernateSessionFactoryManager.
    /// Manages single or multiple factories.
    /// </summary>
    public class SessionFactoryManager : ISessionFactoryManager
    {
        static ILogger logger = Logging.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        private static class ErrorMessages
        {
            public const string UnnamedFactory = "Factories managed with this manager must have a name";
            public const string MultipleFactories = "You can use this method only when there's exactly one factory managed";
            public const string UnexistentFactory = "There's no managed session factory with that name: {0}";
            public const string DuplicatedFactory = "There's already a managed session factory with that name: {0}";
            public const string NullOrEmptyFactoryName = "Factory name can't be null nor empty";
            public const string NoValidConnectionStringFound = "No valid connection string found. Configure a connection string using your Machine Name or {0} as name.";
        }

        //factoryName -> factory
        IDictionary<string, ISessionFactory> FactoriesDict;
        public IEnumerable<ISessionFactory> Factories { get { return FactoriesDict.Values; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateSessionFactoryManager"/> class.
        /// </summary>
        public SessionFactoryManager()
        {
            FactoriesDict = new Dictionary<string, ISessionFactory>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateSessionFactoryManager"/> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        public SessionFactoryManager(params Configuration[] configurations)
        {
            configurations.ThrowIfNull("configurations");
            
            FactoriesDict = new Dictionary<string, ISessionFactory>();
            foreach (var configuration in configurations)
            {
                var factory = configuration.BuildSessionFactory();
                AddFactory(factory);
            }           
        }

        /// <summary>
        /// Sets the connection string from AppConfig.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public static void SetConnectionString(Configuration configuration)
        {
            configuration.ThrowIfNull("configuration");
            string connectionString = ConnectionStringManager.GetConnectionStringForMachine();

            //no connection string found?
            if (connectionString.IsNullOrEmpty())
            {
                throw new ApplicationException(ErrorMessages.NoValidConnectionStringFound
                                               .InvariantFormat(ConnectionStringManager.CommonConnectionStringIdentifierName));
            }

            //set the connection string
            configuration.SetProperty("connection.connection_string", connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionFactoryManager"/> class.
        /// </summary>
        /// <param name="sessionFactory">The session factory.</param>
        public SessionFactoryManager(ISessionFactory sessionFactory)
        {
            sessionFactory.ThrowIfNull("sessionFactory");
            
            FactoriesDict = new Dictionary<string, ISessionFactory>();
            AddFactory(sessionFactory);
        }

        /// <summary>
        /// Gets the session factory.
        /// </summary>
        /// <returns>The session factory</returns>
        public ISessionFactory GetFactory()
        {
            if (FactoriesDict.Count != 1) throw new InvalidOperationException(ErrorMessages.MultipleFactories);
            return FactoriesDict.Values.First();
        }

        /// <summary>
        /// Gets a session factory by its name.
        /// </summary>
        /// <param name="factoryName">Name of the session factory.</param>
        /// <returns></returns>
        public ISessionFactory GetFactory(string factoryName)
        {
            ISessionFactory factory;
            if (!FactoriesDict.TryGetValue(factoryName, out factory))
            {
                throw new InvalidOperationException(ErrorMessages.UnexistentFactory.InvariantFormat(factoryName));
            }

            return factory;
        }

        /// <summary>
        /// Adds the given factory to the manager.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public void AddFactory(ISessionFactory factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            var factoryImplementor = (ISessionFactoryImplementor)factory;

            string factoryName = factoryImplementor.Settings.SessionFactoryName;
            
            //TODO: Should we allow unnamed factory if theres only one?
            if (String.IsNullOrEmpty(factoryName))
            {
                throw new InvalidOperationException(ErrorMessages.UnnamedFactory);
            }

            if (FactoriesDict.ContainsKey(factoryName))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, ErrorMessages.DuplicatedFactory, factoryName));
            }

            FactoriesDict[factoryName] = factory;
        }

        /// <summary>
        /// Removes the given factory from the manager.
        /// </summary>
        /// <param name="factoryName">The factory name.</param>
        public void RemoveFactory(string factoryName)
        {
            if (String.IsNullOrEmpty(factoryName)) throw new ArgumentException(ErrorMessages.NullOrEmptyFactoryName);

            //factory exists?
            if (!FactoriesDict.ContainsKey(factoryName))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, ErrorMessages.UnexistentFactory, factoryName));

            FactoriesDict.Remove(factoryName);
        }

        /// <summary>
        /// Tests the connection/s.
        /// </summary>
        /// <returns>
        /// true if all connections work, false otherwise
        /// </returns>
        public bool TestConnection()
        {
            foreach (var factory in Factories)
            {
                if (!TestConnection(factory)) return false;
            }

            return (true && FactoriesDict.Count > 0);
        }

        /// <summary>
        /// Tests the connection on a particular session factory.
        /// </summary>
        /// <param name="factoryName">Name of the factory.</param>
        /// <returns>true if connection work, false otherwise</returns>
        public bool TestConnection(string factoryName)
        {
            return TestConnection(GetFactory(factoryName));
        }

        /// <summary>
        /// Tests the connection/s.
        /// </summary>
        /// <returns>
        /// A list of factory names for which the connection has failed
        /// </returns>
        public IEnumerable<string> GetFailingConnections()
        {
            foreach (ISessionFactoryImplementor factory in Factories)
            {
                if (!TestConnection(factory)) yield return factory.Settings.SessionFactoryName;
            }
        }

        /// <summary>
        /// Tests that the connection can be stablished.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>true if connection work, false otherwise</returns>
        private static bool TestConnection(ISessionFactory factory)
        {
            try
            {
                using (var session = factory.OpenSession())
                using (session.BeginTransaction()) { }
                return true;
            }
            catch (ADOException ex)
            {
                logger.LogException(ex);
            }

            return false;
        }
    }
}
