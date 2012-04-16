#region License
#endregion

using System;

using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;

using Aspid.Core.Extensions;
using Aspid.NHibernate.PersistentConversation;

namespace Aspid.NHibernate.Context
{
    /// <summary>
    /// This session context provides the current active conversation's session.
    /// There's one current active conversation per thread.
    /// </summary>
    public class ConversationSessionContext : ICurrentSessionContext
    {
        private static class ErrorMessages
        {
            public const string NoActiveConversation = "No active conversation";
            public const string TransactionHandlingAttempt = "Transactions should be managed at the conversation level";

        }

        /// <summary>
        /// This thread's active conversation.
        /// </summary>
        [ThreadStatic]
        internal static INHibernateConversation CurrentConversation;

        /// <summary>
        /// The managed NHibernate Session Factory.
        /// </summary>
        public ISessionFactoryImplementor Factory { get; set; }

        /// <summary>
        /// Constructor,
        /// Instances of this class are meant to be created automatically by nhibernate when
        /// a Session Factory using this class as ContextSessionProvider is built.
        /// </summary>
        /// <param name="factory"></param>
        public ConversationSessionContext(ISessionFactoryImplementor factory)
        {
            factory.ThrowIfNull("factory");

            Factory = factory;
        }

        /// <summary>
        /// Wraps the current active conversation's session and returns it.
        /// </summary>
        /// <returns></returns>
        public ISession CurrentSession()
        {
            if (CurrentConversation == null) throw new InvalidOperationException(ErrorMessages.NoActiveConversation);

            return CurrentConversation.Session;
        }
    }
}
