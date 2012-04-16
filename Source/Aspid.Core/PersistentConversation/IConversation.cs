#region License
#endregion

using System;

namespace Aspid.Core.PersistentConversation
{
    /// <summary>
    /// Represents a conversation with the persistent storage
    /// </summary>
    public interface IConversation : IHideObjectMembers, IDisposable
    {
        /// <summary>
        /// Begins a new storage-transaction
        /// </summary>
        IGenericTransaction BeginTransaction();

        /// <summary>
        /// Ends the current active storage-transaction committing the changes
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Ends the current active storage-transaction discarding the changes
        /// </summary>
        void AbortTransaction();

        /// <summary>
        /// Gets a value indicating whether this conversation is on an active transaction.
        /// </summary>
        /// <value><c>true</c> if in transaction; otherwise, <c>false</c>.</value>
        bool InTransaction { get; }

        /// <summary>
        /// Gets a value indicating whether this conversation is active.
        /// </summary>
        /// <value><c>true</c> if this conversation is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; }

        /// <summary>
        /// Sets this conversation as the currently active one
        /// </summary>
        void MakeActive();
    }
}
