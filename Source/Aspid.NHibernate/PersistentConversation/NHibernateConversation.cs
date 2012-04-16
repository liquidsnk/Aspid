#region License
#endregion

using System;

using NHibernate;

using Aspid.Core;
using Aspid.Core.PersistentConversation;

namespace Aspid.NHibernate.PersistentConversation
{
    /// <summary>
    /// Implementation of an NHibernate conversation
    /// </summary>
    internal class NHibernateConversation : INHibernateConversation
    {
        private static class ErrorMessages
        {
            public const string TransactionAlreadyStarted = "Transaction already started";
            public const string TransactionNotStarted = "Transaction not started";
        }

        ISession session;
        ISessionFactory sessionFactory;
        IConversationManager manager;
        IGenericTransaction currentTransaction;

        private bool TheresAnActiveTransaction()
        {
            return ((currentTransaction != null) && (currentTransaction.IsActive));
        }


        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>The session.</value>
        public ISession Session 
        { 
            get 
            {
                if (session == null) { BeginSession(); }
                return session;
            }
            private set { session = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateConversation"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="sessionProvider">The session provider.</param>
        /// Implementors should take notice:
        internal NHibernateConversation(IConversationManager manager, ISessionFactory sessionProvider)
        {
            this.manager = manager;
            sessionFactory = sessionProvider;
            BeginSession();
        }

        /// <summary>
        /// Begins a new storage-transaction
        /// </summary>
        public IGenericTransaction BeginTransaction()
        {
            if (TheresAnActiveTransaction()) { throw new InvalidOperationException(ErrorMessages.TransactionAlreadyStarted); };

            var transaction = new NHibernateTransaction(Session.BeginTransaction());
            transaction.TransactionRolledBack += transaction_TransactionRolledBack;
            transaction.TransactionCommitted += transaction_TransactionCommitted;

            currentTransaction = transaction;

            MakeActive();
            return currentTransaction;
        }

        void transaction_TransactionCommitted(object sender, EventArgs e)
        {
            currentTransaction = null;
        }

        void transaction_TransactionRolledBack(object sender, EventArgs e)
        {
            EndSession();
        }

        /// <summary>
        /// Ends the current active storage-transaction committing the changes
        /// </summary>
        public void CommitTransaction()
        {
            if (!TheresAnActiveTransaction()) { throw new InvalidOperationException(ErrorMessages.TransactionNotStarted); };

            currentTransaction.Commit();
            currentTransaction = null;
        }

        /// <summary>
        /// Ends the current active storage-transaction discarding the changes
        /// </summary>
        public void AbortTransaction()
        {
            if (!TheresAnActiveTransaction()) { throw new InvalidOperationException(ErrorMessages.TransactionNotStarted); };

            currentTransaction.Rollback();
            EndSession();
        }

        /// <summary>
        /// Begins the NHibernate session.
        /// </summary>
        protected virtual void BeginSession()
        {
            Session = sessionFactory.OpenSession();
            Session.FlushMode = FlushMode.Commit;
        }

        /// <summary>
        /// Ends the NHibernate session.
        /// </summary>
        protected virtual void EndSession()
        {
            if (TheresAnActiveTransaction()) { currentTransaction.Dispose(); }
            if (Session != null) { Session.Dispose(); }
            currentTransaction = null;
            Session = null;
        }

        /// <summary>
        /// Disposes the conversation, ending the underlying session.
        /// </summary>
        public void Dispose()
        {
            manager.DeactivateConversation(this);
            EndSession();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sets this conversation as the currently active one
        /// </summary>
        public void MakeActive()
        {
            manager.SetAsActiveConversation(this);
        }

        /// <summary>
        /// Gets a value indicating whether this conversation is on an active transaction.
        /// </summary>
        /// <value><c>true</c> if in transaction; otherwise, <c>false</c>.</value>
        public bool InTransaction
        {
            get { return (TheresAnActiveTransaction()); }
        }

        /// <summary>
        /// Gets a value indicating whether this conversation is active.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this conversation is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get { return (this == manager.GetActiveConversation()); }
        }
    }
}
