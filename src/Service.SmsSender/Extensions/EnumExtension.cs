using System;

namespace Service.SmsSender.Extensions
{
    public static class EnumExtension
    {
        public static TEnum ToEnum<TEnum>(this string stringValue) where TEnum : struct
        {
            if (string.IsNullOrEmpty(stringValue) || !Enum.TryParse(stringValue, true, out TEnum enumValue))
            {
                return default;
            }

            return enumValue;
        }
    }
}
