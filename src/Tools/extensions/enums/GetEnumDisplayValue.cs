using System;

namespace Tools.extensions.enums
{
    public static class GetEnumDisplayValue
    {
        public static string GetStringValue(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(EnumDisplayAttribute), false) as EnumDisplayAttribute[];

            return attributes != null && attributes.Length > 0 ? attributes[0].String : string.Empty;
        }
    }
}