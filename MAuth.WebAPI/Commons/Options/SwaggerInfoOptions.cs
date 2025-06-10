namespace MAuth.WebAPI.Commons.Options;

/// <summary>
/// Swagger基本信息
/// </summary>
public class SwaggerInfoOptions
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = "MAuth API";

    /// <summary>
    /// 版本
    /// </summary>
    public string Version { get; set; } = "N/A";

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = "No description provided.";
}
