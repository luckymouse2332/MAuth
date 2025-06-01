namespace MAuth.Web.Services.Identity
{
    /// <summary>
    /// 实现了此接口的类将用于存储和管理身份验证令牌。
    /// </summary>
    public interface ITokenStore
    {
        /// <summary>
        /// 保存令牌到存储中。
        /// </summary>
        /// <param name="token">token的内容</param>
        /// <param name="key">token的键</param>
        /// <returns></returns>
        Task SaveTokenAsync(string key, string token, TimeSpan expiration);

        /// <summary>
        /// 从存储中获取令牌。
        /// </summary>
        /// <param name="key">token的键</param>
        /// <param name="expiration">过期时间</param>
        /// <returns></returns>
        Task<string?> GetTokenAsync(string key);
    }
}
