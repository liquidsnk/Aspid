#region License
#endregion

using System;

namespace Aspid.Core
{
    /// <summary>
    /// Represents a generic transaction
    /// </summary>
    public interface IGenericTransaction : IDisposable, IHideObjectMembers
    {
        /// <summary>
        /// Gets a value indicating whether the transaction is active.
        /// </summary>
        /// <value><c>true</c> still an active transaction, <c>false</c>.</value>
        bool IsActive { get; }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Ends the transaction.
        /// </summary>
        /// <param name="commit">
        /// -true: Commits.
        /// -false: Rollbacks.
        /// </param>
        void EndTransaction(bool commit);
    }
}
