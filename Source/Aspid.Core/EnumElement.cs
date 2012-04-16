#region License
#endregion

using System;

using Aspid.Core.Utils;

namespace Aspid.Core
{
    /// <summary>
    /// Represents an enumeration element plus a description.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration</typeparam>
    public class EnumElement<T> : IEquatable<EnumElement<T>> where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumElement&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="description">The description.</param>
        public EnumElement(T element, string description)
        {
            Value = element;
            Description = description;
        }

        /// <summary>
        /// Gets or sets the enum value.
        /// </summary>
        /// <value>The value.</value>
        public T Value { get; private set; }

        /// <summary>
        /// Gets the description of the enum element.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; private set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T"/> to <see cref="Aspid.Core.EnumElement&lt;T&gt;"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator EnumElement<T>(T element)
        {
            return new EnumElement<T>(element, EnumUtils.GetElementDescription(element));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Aspid.Core.EnumElement&lt;T&gt;"/> to <see cref="T"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator T(EnumElement<T> element)
        {
            return element.Value;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(EnumElement<T> other)
        {
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return base.Equals(obj);
            }

            return Equals((EnumElement<T>)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
