using System;
using System.Collections.Generic;

using NHibernate.Cfg;
using NUnit.Framework;

using Aspid.NHibernate.PersistentConversation;
using Aspid.NHibernate.Context;

namespace Aspid.NHibernate.Tests
{
    [TestFixture]
    public class NHibernateConversationTests
    {
        NHibernateConversationManager conversationManager;

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
                                     { "current_session_context_class", "Aspid.NHibernate.Context.ConversationSessionContext, Aspid.NHibernate" },
                                 };
            var configuration = new Configuration { Properties = configurationProperties };
            var sessionFactoryManager = new SessionFactoryManager(configuration);
            conversationManager = new NHibernateConversationManager(sessionFactoryManager);
        }

        [Test]
        public void Session_FromANewConversation_IsNotNull()
        {
            var conversation = conversationManager.BeginConversation() as NHibernateConversation;
            Assert.IsNotNull(conversation.Session);
        }

        [Test]
        public void Conversation_WhenBegun_IsActive()
        {
            var conversation = conversationManager.BeginConversation() as NHibernateConversation;
            Assert.IsTrue(conversation.IsActive);
        }

        [Test]
        public void Conversation_WhenBeginningATransaction_IsActive()
        {
            var conversation1 = conversationManager.BeginConversation() as NHibernateConversation;
            var conversation2 = conversationManager.BeginConversation() as NHibernateConversation;
            
            Assert.IsTrue(conversation2.IsActive);
            conversation1.BeginTransaction();
            Assert.IsTrue(conversation1.IsActive);
        }

        [Test]
        public void Conversation_WhenActive_IsTheCurrentlyActiveConversationOnSessionContext()
        {
            var conversation = conversationManager.BeginConversation() as NHibernateConversation;
            Assert.IsTrue(conversation.IsActive);
            Assert.AreSame(ConversationSessionContext.CurrentConversation, conversation);
        }

        [Test]
        public void Conversation_WhenNotActive_IsNotTheCurrentlyActiveConversationOnSessionContext()
        {
            var conversation = conversationManager.BeginConversation() as NHibernateConversation;
            conversationManager.DeactivateConversation(conversation);
            Assert.AreNotSame(ConversationSessionContext.CurrentConversation, conversation);
        }

        [Test]
        public void Conversation_WhenDisposed_IsNotTheCurrentlyActiveConversationOnSessionContext()
        {
            var conversation = conversationManager.BeginConversation() as NHibernateConversation;
            conversation.Dispose();
            Assert.AreNotSame(ConversationSessionContext.CurrentConversation, conversation);
        }

        [Test]
        public void Conversation_SyncsIsActiveProperty()
        {
            var conversation1 = conversationManager.BeginConversation() as NHibernateConversation;
            Assert.IsTrue(conversation1.IsActive);
            conversation1.Dispose();
            Assert.IsFalse(conversation1.IsActive);

            var conversation2 = conversationManager.BeginConversation() as NHibernateConversation;
            Assert.IsTrue(conversation2.IsActive);
            var conversation3 = conversationManager.BeginConversation() as NHibernateConversation;
            Assert.IsFalse(conversation2.IsActive);
            Assert.IsTrue(conversation3.IsActive);

            conversationManager.DeactivateConversation(conversation3);
            Assert.IsFalse(conversation3.IsActive);
        }

        [Test]
        public void Conversation_IsAbleToBeginTransaction()
        {
            var conversation = conversationManager.BeginConversation() as NHibernateConversation;
            var tx = conversation.BeginTransaction();
            Assert.IsNotNull(tx);
            Assert.IsTrue(conversation.InTransaction);
        }

        [Test]
        public void Conversation_TryingToBeginTwoTransactions_ThrowsInvalidOperationException()
        {
            var conversation = conversationManager.BeginConversation() as NHibernateConversation;
            conversation.BeginTransaction();

            Assert.Throws<InvalidOperationException>(() => conversation.BeginTransaction());
        }

        [Test]
        public void Conversation_TryingToAbortWithoutTransaction_ThrowsInvalidOperationException()
        {
            var conversation = conversationManager.BeginConversation() as NHibernateConversation;
            Assert.Throws<InvalidOperationException>(() => conversation.AbortTransaction());
        }

        [Test]
        public void Conversation_TryingToCommitWithoutTransaction_ThrowsInvalidOperationException()
        {
            var conversation = conversationManager.BeginConversation() as NHibernateConversation;
            Assert.Throws<InvalidOperationException>(() => conversation.CommitTransaction());
        }

        [Test]
        public void Conversation_IsAbleToCommitAndAbortTransactions()
        {
            var conversation = conversationManager.BeginConversation() as NHibernateConversation;
            conversation.BeginTransaction();
            Assert.IsTrue(conversation.InTransaction);
            conversation.CommitTransaction();
            Assert.IsFalse(conversation.InTransaction);
            conversation.BeginTransaction();
            Assert.IsTrue(conversation.InTransaction);
            conversation.AbortTransaction();
            Assert.IsFalse(conversation.InTransaction);
        }
    }
}
