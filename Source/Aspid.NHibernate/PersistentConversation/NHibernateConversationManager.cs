#region License
#endregion

using System;
using System.Globalization;

using NHibernate.Engine;

using Aspid.Core.PersistentConversation;
using Aspid.NHibernate.Context;

namespace Aspid.NHibernate.PersistentConversation
{
    public class NHibernateConversationManager : IConversationManager
    {
        private static class ErrorMessages
        {
            public const string InvalidSessionFactoryManager = "Invalid session factory manager";
            public const string InvalidFactory = "Provided session factory does not use ConversationSessionContext: {0}";
        }

        ISessionFactoryManager sessionFactoryManager;

        public NHibernateConversationManager()
            :this(new SessionFactoryManager())
        {
        }

        public NHibernateConversationManager(ISessionFactoryManager sessionFactoryManager)
        {
            if (sessionFactoryManager == null) throw new ArgumentNullException("sessionFactoryManager");

            //Check that all the factories managed by this manager are valid
            //(That means, can be used in persistent conversations)
            foreach (var factory in sessionFactoryManager.Factories)
            {
                var factoryImpl = (factory as ISessionFactoryImplementor);
                if (factoryImpl == null) throw new InvalidOperationException(ErrorMessages.InvalidSessionFactoryManager);

                //Should have conversationSessionContext as current_session_context_class
                var context = (factoryImpl.CurrentSessionContext as ConversationSessionContext);
                if (context == null)
                {
                    string factoryName = factoryImpl.Settings != null ? factoryImpl.Settings.SessionFactoryName : "";
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, ErrorMessages.InvalidFactory, factoryName ?? ""));
                }
            }

            this.sessionFactoryManager = sessionFactoryManager;
        }

        /// <summary>
        /// Begins a conversation.
        /// </summary>
        /// <returns>The new persistent conversation</returns>
        public IConversation BeginConversation()
        {
            //TODO: Should create a factory for NHibernateConversations?.
            var conversation = new NHibernateConversation(this, sessionFactoryManager.GetFactory());
            conversation.MakeActive();
            return conversation;
        }

        /// <summary>
        /// Begins a conversation on a given context.
        /// </summary>
        /// <param name="contextName">Name of the context.</param>
        /// <returns>
        /// The new persistent conversation for the given context
        /// </returns>
        public IConversation BeginConversation(string contextName)
        {
            //TODO: Should create a factory for NHibernateConversations?.
            var conversation = new NHibernateConversation(this, sessionFactoryManager.GetFactory(contextName));
            conversation.MakeActive();
            return conversation;
        }

        /// <summary>
        /// Sets the given conversation as the currently active one.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        public void SetAsActiveConversation(IConversation conversation)
        {
            if (conversation == null) throw new ArgumentNullException("conversation");
            ConversationSessionContext.CurrentConversation = conversation as INHibernateConversation;
        }

        /// <summary>
        /// Gets the active conversation.
        /// </summary>
        /// <returns>
        /// The currently active conversation or null if there's none.
        /// </returns>
        public IConversation GetActiveConversation()
        {
            return ConversationSessionContext.CurrentConversation;
        }

        /// <summary>
        /// Deactivates the conversation.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        public void DeactivateConversation(IConversation conversation)
        {
            if (conversation == null) throw new ArgumentNullException("conversation");
            if (conversation.IsActive) 
            {
                ConversationSessionContext.CurrentConversation = null; 
            }
        }
    }
}
