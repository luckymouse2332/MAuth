namespace MAuth.WebAPI.Services.Identity;

/// <summary>
/// 授权策略列表
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// 仅允许管理员
    /// </summary>
    public const string AdminOnly = "AdminOnly";
    
    /// <summary>
    /// 仅允许当前用户
    /// </summary>
    public const string CurrentUserOnly = "CurrentUserOnly";
    
    /// <summary>
    /// 需要API Key
    /// </summary>
    public const string RequireApiKey = "RequireApiKey";
}