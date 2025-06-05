namespace MAuth.Web.Services.Identity;

/// <summary>
/// 授权策略列表
/// </summary>
public static class AuthorizationPolicyNames
{
    /// <summary>
    /// 仅允许管理员
    /// </summary>
    public const string AdminOnly = "AdminOnly";
    
    /// <summary>
    /// 仅允许当前用户
    /// </summary>
    public const string CurrentUserOnly = "CurrentUserOnly";
}