namespace MAuth.Web.Commons.Helpers
{
    /// <summary>
    /// 枚举类型帮助类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 解析字符串为指定的枚举类型
        /// </summary>
        /// <typeparam name="TEnum">指定的枚举类型</typeparam>
        /// <param name="value">要解析的字符串</param>
        /// <returns>解析结果</returns>
        public static TEnum? ParseEnum<TEnum>(string? value) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (Enum.TryParse<TEnum>(value, true, out var result))
            {
                return result;
            }

            return null;
        }
    }
}
