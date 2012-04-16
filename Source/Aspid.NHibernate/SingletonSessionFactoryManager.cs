#region License
#endregion

using System.Collections.Generic;
using Aspid.Core.Extensions;
using NHibernate;

namespace Aspid.NHibernate
{
    public class SingletonSessionFactoryManager : ISessionFactoryManager
    {
        public readonly static SingletonSessionFactoryManager Instance = new SingletonSessionFactoryManager(new SessionFactoryManager());
        readonly ISessionFactoryManager SessionFactoryManager;

        private SingletonSessionFactoryManager(ISessionFactoryManager sessionFactoryManager)
	    {
            sessionFactoryManager.ThrowIfNull("sessionFactoryManager");

            SessionFactoryManager = sessionFactoryManager;
	    }

        public ISessionFactory GetFactory(string factoryName)
        {
            return SessionFactoryManager.GetFactory(factoryName);
        }

        public ISessionFactory GetFactory()
        {
            return SessionFactoryManager.GetFactory();
        }

        public IEnumerable<ISessionFactory> Factories
        {
            get { return SessionFactoryManager.Factories; }
        }

        public void AddFactory(ISessionFactory factory)
        {
            SessionFactoryManager.AddFactory(factory);
        }

        public void RemoveFactory(string factoryName)
        {
            SessionFactoryManager.RemoveFactory(factoryName);
        }

        public bool TestConnection()
        {
            return SessionFactoryManager.TestConnection();
        }

        public bool TestConnection(string factoryName)
        {
            return SessionFactoryManager.TestConnection(factoryName);
        }

        public IEnumerable<string> GetFailingConnections()
        {
            return SessionFactoryManager.GetFailingConnections();
        }
    }
}
