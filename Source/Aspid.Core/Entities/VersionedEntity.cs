#region License
#endregion

using System;

namespace Aspid.Core.Entities
{
    /// <summary>
    /// Abstract Base Class for entities with version
    /// </summary>
    public abstract class VersionedEntity : BaseEntity<long>
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version (for Optimistic concurrency management).</value>
        public virtual int Version { get; protected set; }
    }
}
