#region License
#endregion

using System;

namespace Aspid.Core.Utils
{
    /// <summary>
    /// Utillity methods to work with Type
    /// </summary>
    public static class TypeUtils
    {
        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }
    }
}
