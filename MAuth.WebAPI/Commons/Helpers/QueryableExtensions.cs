using System.Linq.Expressions;
using MAuth.WebAPI.Commons.Models;
using Microsoft.EntityFrameworkCore;

namespace MAuth.WebAPI.Commons.Extensions;

/// <summary>
/// 查询常用方法
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// 排序
    /// </summary>
    /// <param name="query">要添加排序的query对象</param>
    /// <param name="sortBy">按哪个字段排序</param>
    /// <param name="descending">是否为降序</param>
    /// <remarks>如果sortBy为空白字符串则返回原来的query对象</remarks>
    /// <returns>返回添加排序后的query对象</returns>
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, sortBy);
        var orderByExp = Expression.Lambda(property, parameter);
        
        var method = descending ? "OrderByDescending" : "OrderBy";
        var result = Expression.Call(typeof(Queryable), method,
            [typeof(T), property.Type], query.Expression, Expression.Quote(orderByExp));

        return query.Provider.CreateQuery<T>(result);
    }

    /// <summary>
    /// 添加分页
    /// </summary>
    /// <param name="query">要添加分页的query对象</param>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>分页后的query对象</returns>
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
    {
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, int page, int pageSize, CancellationToken ct = default)
    {
        var total = await query.CountAsync(ct);
        var items = await query.ApplyPaging(page, pageSize).ToListAsync(ct);
        return new PagedList<T>(items, total, page, pageSize);
    }
}