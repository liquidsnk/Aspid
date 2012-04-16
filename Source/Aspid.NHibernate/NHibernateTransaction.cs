#region License
#endregion

using System;

using NHibernate;

using Aspid.Core;
using Aspid.Core.Extensions;
using Aspid.Core.Exceptions;

namespace Aspid.NHibernate
{
    /// <summary>
    /// NHibernate transaction, wrapped to implement IGenericTransaction
    /// </summary>
    internal class NHibernateTransaction : IGenericTransaction
    {
        private static class ErrorMessages
        {
            public const string InactiveTransaction = "The transaction is Inactive";
        }

        public event EventHandler TransactionRolledBack;
        public event EventHandler TransactionCommitted;
        public event EventHandler TransactionDisposed;

        protected void OnTransactionRollback() 
        {
            var handler = TransactionRolledBack;
            if (handler != null) { handler(this, new EventArgs()); }
        }

        protected void OnTransactionCommit()
        {
            var handler = TransactionCommitted;
            if (handler != null) { handler(this, new EventArgs()); }
        }

        protected void OnTransactionDispose()
        {
            var handler = TransactionDisposed;
            if (handler != null) { handler(this, new EventArgs()); }
        }

        private ITransaction Transaction { get; set; }

        /// <summary>
        /// Gets a value indicating whether the transaction is active.
        /// </summary>
        /// <value><c>true</c> still an active transaction, <c>false</c>.</value>
        public bool IsActive { get; private set; }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateTransaction"/> class.
        /// </summary>
        /// <param name="transaction">The real transaction.</param>
        public NHibernateTransaction(ITransaction transaction)
        {
            transaction.ThrowIfNull("transaction");
            
            IsActive = transaction.IsActive;
            Transaction = transaction;
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        /// <exception cref="Aspid.Core.StaleObjectException">Thrown when there's an operation that causes an NHibernte StaleObjectException</exception>
        public void Commit()
        {
            if (!IsActive) throw new InvalidOperationException(ErrorMessages.InactiveTransaction);

            try
            {
                Transaction.Commit();
            }
            catch (StaleObjectStateException ex)
            {
                throw new StaleObjectException(ex.Message, ex);
            }

            IsActive = false;
            OnTransactionCommit();
        }

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        public void Rollback()
        {
            if (!IsActive) throw new InvalidOperationException(ErrorMessages.InactiveTransaction);
            Transaction.Rollback();
            IsActive = false;

            OnTransactionRollback();
        }

        /// <summary>
        /// Ends the transaction.
        /// </summary>
        /// <param name="commit">
        /// -true: Commits.
        /// -false: Rollbacks.</param>
        public void EndTransaction(bool commit)
        {
            if (commit) { Commit(); }
            else { Rollback(); }
        }

        /// <summary>
        /// Disposes the underlying transaction
        /// </summary>
        public void Dispose()
        {
            if (IsActive)
            {
                IsActive = false;
                OnTransactionRollback();
            }

            Transaction.Dispose();
            Transaction = null;

            GC.SuppressFinalize(this);
        }
    }
}
