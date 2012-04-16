#region License
#endregion

using System;
using System.Collections.Generic;

using NHibernate;

using Aspid.Core;

namespace Aspid.NHibernate
{
    /// <summary>
    /// Manages NHibernate session factories.
    /// </summary>
    public interface ISessionFactoryManager : IHideObjectMembers
    {
        /// <summary>
        /// Gets a session factory by its name.
        /// </summary>
        /// <param name="factoryName">Name of the session factory.</param>
        /// <returns></returns>
        ISessionFactory GetFactory(string factoryName);

        /// <summary>
        /// Gets the session factory.
        /// </summary>
        /// <returns>The session factory</returns>
        ISessionFactory GetFactory();

        /// <summary>
        /// Gets the factories.
        /// </summary>
        /// <value>The factories.</value>
        IEnumerable<ISessionFactory> Factories { get; }

        /// <summary>
        /// Adds the given factory to the manager.
        /// </summary>
        /// <param name="factory">The factory.</param>
        void AddFactory(ISessionFactory factory);

        /// <summary>
        /// Removes the given factory from the manager.
        /// </summary>
        /// <param name="factoryName">The factory name.</param>
        void RemoveFactory(string factoryName);

        /// <summary>
        /// Tests the connection/s.
        /// </summary>
        /// <returns>true if all connections work, false otherwise</returns>
        bool TestConnection();

        /// <summary>
        /// Tests the connection on a particular session factory.
        /// </summary>
        /// <param name="factoryName">Name of the factory.</param>
        /// <returns>true if connection work, false otherwise</returns>
        bool TestConnection(string factoryName);

        /// <summary>
        /// Tests the connection/s.
        /// </summary>
        /// <returns>A list of factory names for which the connection has failed</returns>
        IEnumerable<string> GetFailingConnections();
    }
}
