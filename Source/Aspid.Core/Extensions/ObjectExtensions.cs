#region License
/*
 * tl;dr: NetBSD type of license (two-clause BSD OSI compliant),
 * use it for whatever you like but reproducing this copyright notice.
 * 
 * Copyright (c) 2008 Fredy H. Treboux.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Object class extension methods.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns the string representation of the object as provided by ToString() or string.Empty if the object is null.
        /// </summary>
        /// <param name="self">The object</param>
        /// <returns>The string representation of the current object, or the empty string if it's null</returns>
        public static string ToStringOrEmpty(this object self)
        {
            if (self == null) return string.Empty;
            return self.ToString();
        }

        /// <summary>
        /// Throws <typeparamref name="ArgumentNullException"/> if the object is null, otherwise it does nothing.
        /// </summary>
        /// <param name="self">The obejct</param>
        /// <param name="parameterName">The parameter name</param>
        public static void ThrowIfNull(this object self, string parameterName)
        {
            if (self == null) throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Throws <typeparamref name="ArgumentNullException"/> if the object is null, otherwise it does nothing.
        /// </summary>
        /// <param name="self">The obejct</param>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="message">The message passed to the <typeparamref name="ArgumentNullException"/></param>
        public static void ThrowIfNull(this object self, string parameterName, string message)
        {
            if (self == null) throw new ArgumentNullException(parameterName, message);
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
        /// <param name="self">The current object</param>
        /// <param name="pathExpression">
        /// The lambda expression that represents the path traversal.
        /// It must start at the given lambda paramenter and may go through fields, properties and methods with constant parameters.
        /// </param>
        /// <returns>The value returned by the path traversal, or the default value for the K type if any intermediate result is null</returns>
        public static K SafelyNavigate<T, K>(this T self, Expression<Func<T, K>> pathExpression)
            where T : class
        {
            if (!IsValidMemberAccess(pathExpression))
            {
                const string exceptionMessage = "In order to safely navigate an expression for an object, the expression must be a member access path to the given parameter";
                throw new InvalidOperationException(exceptionMessage);
            };

            var navigationResult = GetSafelyNavigationResultByWalkingPath(pathExpression, self);

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
