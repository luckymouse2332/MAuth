namespace MAuth.Web.Commons.Options;

/// <summary>
/// Api文档身份认证信息配置选项
/// </summary>
public class SwaggerSecurityOptions
{
    /// <summary>
    /// 规则名称
    /// </summary>
    public string SchemeName { get; set; } = "Bearer";

    /// <summary>
    /// 规则描述
    /// </summary>
    public string SchemeDescription { get; set; } = "Default authentication scheme";
}
