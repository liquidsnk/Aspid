#region License
#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Forms;
using Aspid.Core.Extensions;
using Aspid.Core.Utils;

namespace Aspid.Core.WinForms
{
    public class BindingHelper<TObject> where TObject : ICloneable
    {
        public class ControlBinding : IEquatable<ControlBinding>
        {
            public ControlBinding(Control control, string sourcePropertyPath, string destinationProperty)
            {
                control.ThrowIfNull("control");
                sourcePropertyPath.ThrowIfNull("sourcePropertyPath");
                destinationProperty.ThrowIfNull("destinationProperty");

                Control = control;
                SourcePropertyPath = sourcePropertyPath;
                DestinationProperty = destinationProperty;
            }

            public Control Control { get; set; }

            public string SourcePropertyPath { get; set; }

            public string DestinationProperty { get; set; }
            
            public override bool Equals(object obj)
            {
                return Equals(obj as ControlBinding);
            }

            public override int GetHashCode()
            {
                return Control.GetHashCode();
            }

            public bool Equals(ControlBinding other)
            {
                if (other == null) return false;

                return Control.ObjectEquals(other.Control) &&
                       SourcePropertyPath.ObjectEquals(other.SourcePropertyPath) &&
                       DestinationProperty.ObjectEquals(other.DestinationProperty);
            }
        }

        HashSet<Control> _boundControls = new HashSet<Control>();
        Dictionary<Control, HashSet<ControlBinding>> _bindings = new Dictionary<Control, HashSet<ControlBinding>>();

        public BindingHelper()
        {
        }

        public ControlBinding AddBinding<TControl>(TControl control,
                                                   Expression<Func<TControl, object>> boundControlProperty,
                                                   Expression<Func<TObject, object>> boundItemProperty) where TControl : Control
        {
            var binding = new ControlBinding(control,
                                             Reflect<TObject>.PropertyPath(boundItemProperty),
                                             Reflect<TControl>.PropertyPath(boundControlProperty));

            if (!_bindings.ContainsKey(control))
            {
                _bindings[control] = new HashSet<ControlBinding>();
            }

            _bindings[control].Add(binding);

            _boundControls.Add(control);
            return binding;
        }

        public bool HasBinding<TControl>(TControl control) where TControl : Control
        {
            return _boundControls.Contains(control);
        }

        object _originalObject;
        object _boundObject;
        public void BindObject(TObject obj)
        {
            //If we're given a null value, clear all control bindings to it's default values
            if (object.Equals(obj, default(TObject)))
            {
                ClearControlsToDefault();
                return;
            }

            _originalObject = obj.Clone();
            _boundObject = obj;

            ClearChanges();
        }

        private void ClearControlsToDefault()
        {
            foreach (var controlBinding in _bindings)
            {
                foreach (var binding in controlBinding.Value)
                {
                    var control = binding.Control;
                    var boundPropertyType = ReflectionHelper.GetPropertyByPath(control.GetType(), binding.DestinationProperty).PropertyType;
                    SetProperty(control, binding.DestinationProperty, TypeUtils.GetDefaultValue(boundPropertyType));
                }
            }
        }

        public object GetDefaultValue(Type t)
        {
            //Move to a "more general" place since this could be
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }
            else
            {
                return null;
            }
        }

        public bool IsObjectBound { get { return _boundObject != null; } }
        
        private void CheckObjectIsBound()
        {
            if (_boundObject == null) throw new InvalidOperationException("You must bind an objet by calling BindObject first.");
        }

        public bool HasChanges(Control control)
        {
            CheckObjectIsBound();

            //Always commit changes to bound object automatically when checking if the control have any change
            CommitChanges(control); 

            foreach (var binding in _bindings[control])
            {
                var originalPropertyValue = ConvertValue(ResolvePropertyPathValue(_originalObject, binding.SourcePropertyPath));
                var currentPropertyValue = ConvertValue(ResolvePropertyPathValue(_boundObject, binding.SourcePropertyPath));

                if (!originalPropertyValue.ObjectEquals(currentPropertyValue)) return true;
            }

            return false;
        }

        public bool HasChanges()
        {
            CheckObjectIsBound();

            foreach (Control control in _boundControls)
            {
                if (HasChanges(control)) return true;
            }

            return false;
        }

        public void CommitChanges(Control control)
        {
            CheckObjectIsBound();

            foreach (var binding in _bindings[control])
            {
                SetProperty(_boundObject, binding.SourcePropertyPath, ResolvePropertyPathValue(control, binding.DestinationProperty));
            }
        }

        private static void SetProperty(object obj, string propertyPath, object value)
        {
            if (obj == null) return;

            var propertyToSet = ReflectionHelper.GetPropertyByPath(obj.GetType(), propertyPath);
            if (propertyToSet == null || !propertyToSet.CanWrite) return;

            value =  ConvertValue(value);
            var propertyToSetType = propertyToSet.PropertyType;
            if (value != null && !propertyToSetType.IsAssignableFrom(value.GetType()) && (value is IConvertible))
            {
                value = Convert.ChangeType(value, propertyToSetType);
            }

            ReflectionHelper.SetPropertyPathValue(obj, propertyPath, value);
        }

        public void CommitChanges()
        {
            CheckObjectIsBound();

            foreach (Control control in _boundControls)
            {
                CommitChanges(control);
            }
        }

        public void ClearChanges(Control control)
        {
            CheckObjectIsBound();
            
            foreach (var binding in _bindings[control])
            {
                SetProperty(control, binding.DestinationProperty, ResolvePropertyPathValue(_originalObject, binding.SourcePropertyPath));
            }
        }

        public void ClearChanges()
        {
            CheckObjectIsBound();

            foreach (Control control in _boundControls)
            {
                ClearChanges(control);
            }
        }

        private static object ConvertValue(object objectValue)
        {
            if (objectValue is string)
            {
                return ((string)objectValue).EmptyToNull();
            }
            else if (objectValue == DBNull.Value)
            {
                return null;
            }
            
            return objectValue;
        }

        public static object ResolvePropertyPathValue(object obj, string path)
        {
            return ReflectionHelper.GetPropertyPathValue(obj, path);
        }
    }
}
