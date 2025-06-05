namespace MAuth.Web.Commons.Options;

/// <summary>
/// 应用程序Swagger配置选项
/// </summary>
public class ApplicationSwaggerOptions
{
    /// <summary>
    /// 该配置项在配置文件中的Json键名
    /// </summary>
    public const string Name = "Swagger";

    /// <summary>
    /// 使用默认配置
    /// </summary>
    public ApplicationSwaggerOptions()
    {
        BasicOptions = new SwaggerInfoOptions();
        SecurityOptions = new SwaggerSecurityOptions();
    }

    /// <summary>
    /// 基本配置信息
    /// </summary>
    public SwaggerInfoOptions BasicOptions { get; set; }

    /// <summary>
    /// 身份认证配置信息
    /// </summary>
    public SwaggerSecurityOptions SecurityOptions { get; set; }
}
