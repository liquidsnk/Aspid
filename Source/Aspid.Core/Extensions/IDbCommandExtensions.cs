#region License
#endregion

using System.Data;
using System.Collections.Generic;
using System;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extensions for IDbCommand
    /// </summary>
    public static class IDbCommandExtensions
    {
        /// <summary>
        /// Adds a parameter to this command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IDbDataParameter AddParameter<T>(this IDbCommand command, 
                                                       string name, 
                                                       DbType type, 
                                                       T value)
        {
            var parameter = command.AddParameter(name, value);
            parameter.DbType = type;
            return parameter;
        }

        /// <summary>
        /// Adds a parameter to this command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IDbDataParameter AddParameter<T>(this IDbCommand command, string name, T value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);

            return parameter;
        }

        /// <summary>
        /// Adds the given parameter list to the command.
        /// The parameter placeholder names used are formed by the given name plus the index position on the list.
        /// e.g.: ":name0, :name1, :name2.. etc"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="list">The list.</param>
        public static void AddParameterList<T>(this IDbCommand command, string name, IEnumerable<T> list)
        {
            if (list == null) return;

            int index = 0;
            foreach (var item in list)
            {
                string parameterName = String.Format("{0}{1}", name, index);
                AddParameter(command, parameterName, item);
                index++;
            }
        }
    }
}
