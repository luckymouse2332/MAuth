namespace MAuth.WebAPI.Commons.Helpers;

public static class ParameterHelper
{
    /// <summary>
    /// 将格式为 param1 sorting, param2 sorting……格式的排序参数转为能被query对象接受的参数
    /// </summary>
    /// <param name="sortParam">排序参数</param>
    /// <returns></returns>
    public static Dictionary<string, string> GetSorting(string sortParam)
    {
        if (string.IsNullOrWhiteSpace(sortParam)) return [];
        
        var sorts = sortParam.Split(',');

        return sorts.Select(sort => sort.Split(' '))
            .ToDictionary(kv => kv[0], kv => kv[1]);
    }
}