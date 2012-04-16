#region License
#endregion

using System;

using Aspid.Core.Utils;

namespace Aspid.Core
{
    public class EnumElementFormatter<T> : IFormatProvider, ICustomFormatter where T : struct
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var enumElementNullable = arg as T?;

            if (!enumElementNullable.HasValue)
                return string.Empty;

            var enumElement = (T)enumElementNullable.Value;

            return EnumUtils.GetElementDescription<T>(enumElement);
        }

        public object GetFormat(Type formatType)
        {
            return this;
        }
    }
}
