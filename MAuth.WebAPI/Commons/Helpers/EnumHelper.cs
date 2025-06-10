using System.ComponentModel;
using System.Reflection;

namespace MAuth.WebAPI.Commons.Helpers;

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

    /// <summary>
    /// 获取枚举值的描述
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <typeparam name="TEnum">枚举值的类型</typeparam>
    /// <returns>当前枚举值的描述</returns>
    /// <remarks>如果当前枚举值为空、不存在或者不存在描述，则返回空值</remarks>
    public static string? GetDescription<TEnum>(TEnum value)
    {
        if (value is null)
        {
            return null;
        }

        var enumType = typeof(TEnum);
        var enumName = Enum.GetName(enumType, value);

        if (enumName is null)
        {
            return null;
        }

        var attribute = enumType.GetField(enumName)
            !.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description;
    }
}