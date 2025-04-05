using AutoMapper;
using System.Linq.Expressions;

namespace MAuth.Web.Helpers.Mapper
{
    /// <summary>
    /// 扩展了AutoMapper，用于添加一些方便快捷的转化
    /// </summary>
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// 将枚举类型的字符串值映射为可为null的枚举属性
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="mappingExpression"></param>
        /// <param name="sourceSelector"></param>
        /// <param name="destinationSelector"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> ForEnumMember<TSource, TDestination, TEnum>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TSource, string?>> sourceSelector,
            Expression<Func<TDestination, TEnum>> destinationSelector)
            where TEnum : struct, Enum
        {
            return mappingExpression.ForMember(destinationSelector, opt =>
                opt.MapFrom(src => ParseEnum<TEnum>(sourceSelector.Compile()(src)))
            );
        }

        /// <summary>
        /// 从字符串解析枚举值
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        private static TEnum ParseEnum<TEnum>(string? enumValue)
            where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(enumValue))
            {
                return default;
            }

            return Enum.TryParse<TEnum>(enumValue, out var result) ? result : default;
        }
    }
}
