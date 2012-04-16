#region License
#endregion

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Aspid.Core.Utils
{
    public static class ObjectUtils
    {
        /// <summary>
        /// Throws an ArgumentNullException if the object is null.
        /// </summary>
        /// <param name="paramName">Name of the parameter that's being checked.</param>
        public static void ThrowIfNull(object value, string parameterName)
        {
            if (value == null) { throw new ArgumentNullException(parameterName); }
        }

        /// <summary>
        /// Throws an ArgumentNullException if the object is null.
        /// </summary>
        /// <param name="paramName">Name of the parameter that's being checked.</param>
        /// <param name="message">Custom message for the exception.</param>
        public static void ThrowIfNull(object value, string parameterName, string message)
        {
            if (value == null) { throw new ArgumentNullException(parameterName, message); }
        }

        /// <summary>
        /// Performs a ToString on the object.
        /// If the object is null, returns the empty string.
        /// It also ensures that the string returned by ToString is empty if null.
        /// </summary>
        public static string ToStringOrEmpty(object value)
        {
            if (value == null) { return string.Empty; }
            return value.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Navigates a path through the current object, represented by a lambda.
        /// While navigating the path if it encounters a null value, the end result will be the default()
        /// value for the return type of the lambda.
        /// Basically: It provides a way to avoid null checks on path traversals.
        /// It may not work for a complex pathExpression, so beware.
        /// </summary>
        /// <typeparam name="T">The type of the current object</typeparam>
        /// <typeparam name="K">The type of the result of the pathEcpression lambda</typeparam>
        /// <param name="entity">The object to navigate</param>
        /// <param name="pathExpression">
        /// The lambda expression that represents the path traversal.
        /// It must start at the given lambda paramenter and may go through fields, properties and methods with constant parameters.
        /// </param>
        /// <returns>The value returned by the path traversal, or the default value for the K type if any intermediate result is null</returns>
        public static K SafelyNavigate<T, K>(T entity, Expression<Func<T, K>> pathExpression)
            where T : class
        {
            if (!IsValidMemberAccess(pathExpression))
            {
                const string exceptionMessage = "In order to safely navigate an expression for an object, the expression must be a member access path to the given parameter";
                throw new InvalidOperationException(exceptionMessage);
            };

            var navigationResult = GetSafelyNavigationResultByWalkingPath(pathExpression, entity);

            if (navigationResult == null) return default(K);
            return (K)navigationResult;
        }

        private static object GetSafelyNavigationResultByWalkingPath(Expression pathExpression, object parameter)
        {
            if (parameter == null) return null;

            if (pathExpression.NodeType == ExpressionType.Parameter)
            {
                return parameter;
            }
            else if (pathExpression.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)pathExpression;
                var navigationResult = GetSafelyNavigationResultByWalkingPath(lambda.Body, parameter);
                return navigationResult;
            }
            else if (pathExpression.NodeType == ExpressionType.Call)
            {
                var methodCall = (MethodCallExpression)pathExpression;
                var navigationResult = GetSafelyNavigationResultByWalkingPath(methodCall.Object, parameter);
                if (navigationResult == null) return null;

                //Methods with non-constant parameters inside the expression are not supported
                return GetMemberValue(methodCall.Method, navigationResult,
                                      methodCall.Arguments.Select(x => ((ConstantExpression)x).Value).ToArray());
            }
            else if (pathExpression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = (MemberExpression)pathExpression;
                var navigationResult = GetSafelyNavigationResultByWalkingPath(memberExpression.Expression, parameter);
                if (navigationResult == null) return null;

                return GetMemberValue(memberExpression.Member, navigationResult, null);
            }

            return null;
        }

        private static object GetMemberValue(MemberInfo member, object navigationResult, object[] arguments)
        {
            try
            {
                if (member is PropertyInfo)
                {
                    var property = (PropertyInfo)member;
                    return property.GetValue(navigationResult, arguments);
                }
                else if (member is FieldInfo)
                {
                    var field = (FieldInfo)member;
                    return field.GetValue(navigationResult);
                }
                else if (member is MethodInfo)
                {
                    var method = (MethodInfo)member;
                    return method.Invoke(navigationResult, arguments);
                }
                else
                {
                    throw new NotSupportedException("There's only support for safely navigating fields, properties and methods");
                }
            }
            catch (TargetInvocationException ex)
            {
                var innerException = ex.InnerException ?? ex;
                throw new InvalidOperationException("Accessing the navigation chain has caused and exception to be thrown, see the inner exception for details", innerException);
            }
        }

        private static bool IsValidMemberAccess(Expression pathExpression)
        {
            if (pathExpression.NodeType == ExpressionType.Lambda) 
            {
                var lambda = (LambdaExpression)pathExpression;
                return IsValidMemberAccess(lambda.Body);
            }
            else if (pathExpression.NodeType == ExpressionType.MemberAccess)
            {
                var member = (MemberExpression)pathExpression;
                return IsValidMemberAccess(member.Expression);
            }
            else if (pathExpression.NodeType == ExpressionType.Call)
            {
                var methodCall = (MethodCallExpression)pathExpression;

                //Methods with non-constant parameters inside the expression are not supported
                if (methodCall.Arguments.Any(x => x.NodeType != ExpressionType.Constant))
                {
                    throw new NotSupportedException("Methods with non-constant parameters are not supported");
                }

                return IsValidMemberAccess(methodCall.Object);
            }
            else if (pathExpression.NodeType == ExpressionType.Parameter)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
