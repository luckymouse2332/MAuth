namespace MAuth.WebAPI.Commons.Helpers;

/// <summary>
/// 玩家认证帮助类
/// </summary>
public static class PlayerLoginHelper
{
    /// <summary>
    /// 将玩家IP和uuid转化为缓存键
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public static string GetPlayerCacheKey(string ipAddress, string uuid)
    {
        var keyStr = $"{uuid}|{ipAddress}";
        var hash = HashHelper.ComputeSha256Hash(keyStr);
        return hash;
    }
}