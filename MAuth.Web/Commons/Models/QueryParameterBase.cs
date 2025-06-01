using System.ComponentModel.DataAnnotations;

namespace MAuth.Web.Commons.Models;

public abstract class QueryParameterBase
{
    private const int MaxPageSize = 20;

    [Display(Name = "页数")]
    public int Page { get; set; } = 1;

    private int _pageSize;

    [Display(Name = "分页大小")]
    public int PageSize
    {
        get => _pageSize == 0 ? MaxPageSize : _pageSize;
        set => _pageSize = value > MaxPageSize || value <= 0 ? MaxPageSize : value;
    }

    // orderBy=property1 desc, property2 asc ...
    [Display(Name = "排序配置")]
    public string? OrderBy { get; set; }

    [Display(Name = "搜索内容")]
    public string? SearchTerm { get; set; }
}
