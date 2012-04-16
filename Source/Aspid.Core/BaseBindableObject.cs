using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

using Aspid.Core.Extensions;

namespace Aspid.Core
{
    /// <summary>
    /// Implements the INotifyPropertyChanged interface and exposes a RaisePropertyChanged method for derived classes to raise the PropertyChange event.
    /// The event arguments created by this class are cached to prevent managed heap fragmentation.
    /// </summary>
    [Serializable]
    public abstract class BaseBindableObject : INotifyPropertyChanged
    {
        private static readonly IDictionary<string, PropertyChangedEventArgs> _eventArgs = new Dictionary<string, PropertyChangedEventArgs>();
        
        /// <summary>
        /// Raised when a public property of this object is set to a different value.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns an instance of PropertyChangedEventArgs for 
        /// the specified property name.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to create event args for.
        /// </param>		
        protected static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            propertyName.ThrowIfNullOrEmpty(propertyName);
           
            //Get the event args from the cache, creating them and adding to the cache if necessary.
            lock (typeof(BaseBindableObject))
            {
                if (!_eventArgs.ContainsKey(propertyName))
                {
                    _eventArgs.Add(propertyName, new PropertyChangedEventArgs(propertyName));
                }

                return _eventArgs[propertyName];
            }
        }

        /// <summary>
        /// Derived classes can override this method to
        /// execute logic after a property is set. The 
        /// base implementation does nothing.
        /// </summary>
        /// <param name="propertyName">
        /// The property which was changed.
        /// </param>
        protected virtual void AfterPropertyChanged(string propertyName)
        {
        }
        
        protected void ChangeProperty<T>(Expression<Func<T, object>> property, ref T oldValue, ref T newValue)
        {
            ChangeProperty(Reflect<T>.PropertyName(property), ref oldValue, ref newValue);
        }

        protected void ChangeProperty<T>(string propertyName, ref T oldValue, ref T newValue)
        {
            if (object.Equals(oldValue, newValue)) return;
            oldValue = newValue;

            OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T, object>> property)
        {
            OnPropertyChanged(Reflect<T>.PropertyName(property));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(GetPropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs ea)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, ea);
            }

            AfterPropertyChanged(ea.PropertyName);
        }
    }
}