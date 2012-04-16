using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Aspid.Core
{
    public class ReflectionHelper
    {
        readonly static Dictionary<Tuple<Type, string>, Delegate> _compiledPropertyGetAccessors = new Dictionary<Tuple<Type, string>, Delegate>();

        public static Delegate GetPropertyPathGetAccessor(Type type, string path)
        {
            Type currentType = type;
            var propertyTuple = Tuple.FromItems(currentType, path);

            if (!_compiledPropertyGetAccessors.ContainsKey(propertyTuple))
            {
                _compiledPropertyGetAccessors[propertyTuple] = BuildPropertyPathGetAccessor(currentType, path);
            }
                        
            return _compiledPropertyGetAccessors[propertyTuple];
        }
        
        private static Delegate BuildPropertyPathGetAccessor(Type currentType, string path)
        {
            var parameter = Expression.Parameter(currentType, "x");
            
            Expression expression = parameter;
            foreach (string propertyName in path.Split('.'))
            {
                var property = currentType.GetProperty(propertyName);
                expression = Expression.MakeMemberAccess(expression, property);
                currentType = property.PropertyType;
            }

            expression = Expression.Convert(expression, typeof(object));
            return Expression.Lambda(expression, new ParameterExpression[] { parameter }).Compile();
        }

        public static PropertyInfo GetPropertyByPath(Type currentType, string path)
        {
            PropertyInfo property = null;
            foreach (string propertyName in path.Split('.'))
            {
                property = currentType.GetProperty(propertyName);
                currentType = property.PropertyType;
            }

            return property;
        }

        public static object GetPropertyPathValue(object obj, string propertyPath)
        {
            return ReflectionHelper.GetPropertyPathGetAccessor(obj.GetType(), propertyPath).DynamicInvoke(obj);
        }

        #region This code doesn't work to set a property path as it was intended (doesn't handle value types correctly or smthng like that), but may be useful in the future
        //static Dictionary<Tuple<Type, string>, Delegate> _compiledPropertySetAccessors = new Dictionary<Tuple<Type, string>, Delegate>();
        //public static Delegate GetPropertyPathSetAccessor(Type type, string path)
        //{
        //    Type currentType = type;
        //    var propertyTuple = Tuple.FromItems(currentType, path);

        //    if (!_compiledPropertySetAccessors.ContainsKey(propertyTuple))
        //    {
        //        _compiledPropertySetAccessors[propertyTuple] = BuildPropertyPathSetAccessor(currentType, path);
        //    }

        //    return _compiledPropertySetAccessors[propertyTuple];
        //}

        //private static Delegate BuildPropertyPathSetAccessor(Type currentType, string path)
        //{
        //    //Build expression
        //    var destinationParameter = Expression.Parameter(currentType, "x");

        //    Expression expression = destinationParameter;
        //    foreach (string propertyName in path.Split('.'))
        //    {
        //        var property = currentType.GetProperty(propertyName);
        //        expression = Expression.MakeMemberAccess(expression, property);
        //        currentType = property.DeclaringType;
        //    }

        //    expression = Expression.Convert(expression, typeof(object));
        //    return ConvertToAssignment(Expression.Lambda(expression, new ParameterExpression[] { destinationParameter })).Compile();
        //}

        //private static LambdaExpression ConvertToAssignment(LambdaExpression getter)
        //{
        //    var parms = new[] 
        //    {
        //        getter.Parameters[0],
        //        Expression.Parameter(typeof(object), "sourceParameter")
        //    };

        //    Expression body = Expression.Call(SetValueHelperMethodInfo, new[] { getter.Body, parms[1] });
        //    return Expression.Lambda(body, parms);
        //}
        #endregion

        internal static readonly MethodInfo SetValueHelperMethodInfo = typeof(ReflectionHelper).GetMethod("SetValueHelper", BindingFlags.NonPublic | BindingFlags.Static);
        internal static void SetValueHelper(ref object target, object value)
        {
            target = value;
        }
        
        public static void SetPropertyPathValue(object obj, string propertyPath, object value)
        {
            Type currentType = obj.GetType();
            PropertyInfo currentProperty = null;

            foreach (string propertyName in propertyPath.Split('.'))
            {
                if (currentProperty != null)
                {
                    obj = currentProperty.GetValue(obj, null);
                }

                currentProperty = currentType.GetProperty(propertyName);
                currentType = currentProperty.PropertyType;
            }

            //TODO: try to convert the value to one accepted by the property (this probably shouldn't be done here, but since this class only serves as support for BindingHelper for now it's ok..)
            if (value != null && !currentType.IsAssignableFrom(value.GetType()) && (value is IConvertible))
            {
                value = Convert.ChangeType(value, currentType);
            }

            currentProperty.SetValue(obj, value, null);
        }
    }
}
