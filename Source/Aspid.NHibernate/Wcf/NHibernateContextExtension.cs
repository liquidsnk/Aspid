#region License
#endregion

using System.ServiceModel;
using System;

using NHibernate;

using Aspid.Core.Extensions;
using Aspid.Core.Utils;

namespace Aspid.NHibernate.Wcf
{
    public class NHibernateContextExtension : IExtension<InstanceContext>
    {
        public TransactionHandlingMode TransactionHandlingMode { get; set; }

        bool AutomaticallyCommitOnSuccess
        {
            get
            {
                return EnumUtils.ContainsElement<TransactionHandlingMode>(TransactionHandlingMode, TransactionHandlingMode.AutomaticallyCommitOnSuccess);
            }
        }

        public ISessionFactory SessionFactory { get; private set; }
        public ISession Session { get { return SessionFactory.GetCurrentSession(); } }

        private ITransaction transaction;

        public NHibernateContextExtension(ISessionFactory sessionFactory, TransactionHandlingMode transactionHandlingMode)
        {
            sessionFactory.ThrowIfNull("sessionFactory");

            SessionFactory = sessionFactory;
            TransactionHandlingMode = transactionHandlingMode;
        }

        public void Rollback()
        {
            if (TransactionWasManipulated()) return;

            try
            {
                transaction.Rollback();
                transaction.Dispose();
            }
            catch (Exception)
            {
                transaction = null;
            }
        }

        /// <summary>
        /// Enables an extension object to find out when it has been aggregated. Called when the extension is added to the <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Attach(InstanceContext owner)
        {
            transaction = Session.BeginTransaction();
        }

        /// <summary>
        /// Enables an object to find out when it is no longer aggregated. Called when an extension is removed from the <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Detach(InstanceContext owner)
        {
            if (!TransactionWasManipulated() && AutomaticallyCommitOnSuccess)
            {
                transaction.Commit();
            }

            if (transaction != null)
            {
                transaction.Dispose();
            }

            Session.Dispose();
        }

        /// <summary>
        /// True if the transaction was manipulated.
        /// </summary>
        /// <returns></returns>
        private bool TransactionWasManipulated()
        {
            return transaction == null ||
                   !transaction.IsActive ||
                   transaction.WasRolledBack ||
                   transaction.WasCommitted;
        }
    }
}