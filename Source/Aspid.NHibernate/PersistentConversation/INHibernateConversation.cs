#region License
#endregion

using NHibernate;

using Aspid.Core;
using Aspid.Core.PersistentConversation;

namespace Aspid.NHibernate.PersistentConversation
{
    /// <summary>
    /// Represents a conversation from NHibernate's POV
    /// </summary>
    public interface INHibernateConversation : IConversation, IHideObjectMembers
    {
        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>The session.</value>
        ISession Session { get; }
    }
}
