using System;
using System.Collections.Generic;

using NHibernate.Cfg;
using NUnit.Framework;

using Aspid.NHibernate.PersistentConversation;

namespace Aspid.NHibernate.Tests
{
    [TestFixture]
    public class NHibernateConversationManagerTests
    {
        Configuration configuration;
        ISessionFactoryManager invalidSessionFactoryManager;
        ISessionFactoryManager validsessionFactoryManager;

        [SetUp]
        public void SetUp()
        {
            var configurationProperties = new Dictionary<string, string>
                                 {
                                     { "connection.driver_class", "NHibernate.Driver.SQLite20Driver" },
                                     { "dialect", "NHibernate.Dialect.SQLiteDialect" },
                                     { "connection.connection_string", "Data Source=:memory:;Version=3;New=True;" },
                                     { "connection.provider", "NHibernate.Connection.DriverConnectionProvider" },
                                     { "session_factory_name", "TestFactory" },
                                     { "proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle" },
                                     { "current_session_context_class", "thread_static" },
                                 };
            configuration = new Configuration { Properties = configurationProperties };
            invalidSessionFactoryManager = new SessionFactoryManager(configuration);

            configuration.Properties["current_session_context_class"] = "Aspid.NHibernate.Context.ConversationSessionContext, Aspid.NHibernate";
            validsessionFactoryManager = new SessionFactoryManager(configuration);
        }

        [TearDown]
        public void TearDown()
        {
            Aspid.NHibernate.Context.ConversationSessionContext.CurrentConversation = null;
        }

        [Test]
        public void NHibernateConversationManager_CreatedWithNullFactoryManager_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new NHibernateConversationManager(null));
        }

        [Test]
        public void NHibernateConversationManager_CreatedWithFactoryManagerWhoseFactoriesAreNotConversational_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => new NHibernateConversationManager(invalidSessionFactoryManager));
        }

        [Test]
        public void NHibernateConversationManager_CreatedWithValidFactoryManager_IsCreated()
        {
            var conversationManager = new NHibernateConversationManager(validsessionFactoryManager);
            Assert.IsNotNull(conversationManager);
        }

        [Test]
        public void BeginConversation_IsAbleToBeginAConversation()
        {
            var conversationManager = new NHibernateConversationManager(validsessionFactoryManager);
            var conversation = conversationManager.BeginConversation();
            Assert.IsNotNull(conversation);
        }

        [Test]
        public void BeginConversation_WhenThereAreMultipleFactories_ThrowsInvalidOperationException()
        {
            configuration.Properties["session_factory_name"] = "TestFactory2";
            var factory = configuration.BuildSessionFactory();
            validsessionFactoryManager.AddFactory(factory);
            var sut = new NHibernateConversationManager(validsessionFactoryManager);
            Assert.Throws<InvalidOperationException>(() => sut.BeginConversation());
        }

        [Test]
        public void BeginConversation_GivenSessionFactoryName_BeginsAConversationForTheSessionFactory()
        {
            var conversationManager = new NHibernateConversationManager(validsessionFactoryManager);
            var conversation = conversationManager.BeginConversation("TestFactory");
            Assert.IsNotNull(conversation);
        }

        [Test]
        public void BeginConversation_GivenANonExistentSessionFactoryName_ThrowsInvalidOperationException()
        {
            var conversationManager = new NHibernateConversationManager(validsessionFactoryManager);
            Assert.Throws<InvalidOperationException>(() => conversationManager.BeginConversation("UNEXISTENT"));
        }

        [Test]
        public void BeginConversation_SetsLastCreatedConversationAsActive()
        {
            var conversationManager = new NHibernateConversationManager(validsessionFactoryManager);
            var conversation1 = conversationManager.BeginConversation();
            Assert.IsTrue(conversation1.IsActive);

            var conversation2 = conversationManager.BeginConversation();
            Assert.IsTrue(conversation2.IsActive);
            Assert.IsFalse(conversation1.IsActive);
        }

        [Test]
        public void GetActiveConversation_ReturnsTheCurrentConversation()
        {
            var conversationManager = new NHibernateConversationManager(validsessionFactoryManager);
            var conversation = conversationManager.BeginConversation();
            Assert.IsTrue(conversation.IsActive);
            Assert.AreSame(conversation, conversationManager.GetActiveConversation());
        }

        [Test]
        public void GetActiveConversation_WhenTheresNoActiveConversation_ReturnsNull()
        {
            var conversationManager = new NHibernateConversationManager(validsessionFactoryManager);
            Assert.IsNull(conversationManager.GetActiveConversation());
        }

        [Test]
        public void SetAsActiveConversation_GivenAConversation_SetsItAsActive()
        {
            var conversationManager = new NHibernateConversationManager(validsessionFactoryManager);
            var conversation1 = conversationManager.BeginConversation();
            var conversation2 = conversationManager.BeginConversation();
            conversationManager.SetAsActiveConversation(conversation1);

            Assert.IsTrue(conversation1.IsActive);
            Assert.IsFalse(conversation2.IsActive);
        }

        [Test]
        public void SetAsActiveConversation_GivenANullConversation_ThrowsArgumentNullException()
        {
            var conversationManager = new NHibernateConversationManager(validsessionFactoryManager);
            Assert.Throws<ArgumentNullException>(() =>conversationManager.SetAsActiveConversation(null));
        }

        [Test]
        public void DeactivateConversation_GivenAConversation_SetsItAsNotActive()
        {
            var conversationManager = new NHibernateConversationManager(validsessionFactoryManager);
            var conversation1 = conversationManager.BeginConversation();
            conversationManager.DeactivateConversation(conversation1);

            Assert.IsFalse(conversation1.IsActive);
        }

        [Test]
        public void DeactivateConversation_GivenANullConversation_ThrowsArgumentNullException()
        {
            var sut = new NHibernateConversationManager(validsessionFactoryManager);
            Assert.Throws<ArgumentNullException>(() => sut.DeactivateConversation(null));
        }
    }
}
