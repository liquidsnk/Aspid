#region License
#endregion

using System;

namespace Aspid.Core.Entities
{
    /// <summary>
    /// Abstract Base Class for entities
    /// </summary>
    /// <typeparam name="T">Type of the Id</typeparam>
    [Serializable]
    public abstract class BaseEntity<TId> : IEquatable<BaseEntity<TId>>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public virtual TId Id { get; protected set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public virtual bool Equals(BaseEntity<TId> other)
        {
            if (other == null) return false;
           
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (!(obj is BaseEntity<TId>)) return base.Equals(obj);
            return Equals(obj as BaseEntity<TId>);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return IsTransient ? base.GetHashCode() : Id.GetHashCode();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is transient.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is transient; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool IsTransient
        {
            get { return Id.Equals(default(TId)); }
        }
    }
}
