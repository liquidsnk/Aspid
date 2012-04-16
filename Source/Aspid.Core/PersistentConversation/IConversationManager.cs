#region License
#endregion

namespace Aspid.Core.PersistentConversation
{
    /// <summary>
    /// Manages persistent conversations for an application
    /// </summary>
    public interface IConversationManager : IHideObjectMembers
    {
        /// <summary>
        /// Begins a conversation.
        /// </summary>
        /// <returns>The new persistent conversation</returns>
        IConversation BeginConversation();
        
        /// <summary>
        /// Begins a conversation on a given context.
        /// </summary>
        /// <param name="contextName">Name of the context.</param>
        /// <returns>The new persistent conversation for the given context</returns>
        IConversation BeginConversation(string contextName);
        
        /// <summary>
        /// Sets the given conversation as the currently active one.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        void SetAsActiveConversation(IConversation conversation);
        
        /// <summary>
        /// Gets the active conversation.
        /// </summary>
        /// <returns>The currently active conversation or null if there's none.</returns>
        IConversation GetActiveConversation();

        /// <summary>
        /// Deactivates the conversation.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        void DeactivateConversation(IConversation conversation);
    }
}
