using System.Security.Cryptography;
using System.Text;

namespace MAuth.WebAPI.Commons.Helpers
{
    /// <summary>
    /// 哈希计算帮助类
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// 计算 SHA256 哈希值
        /// </summary>
        /// <param name="text">要计算哈希值的字符串</param>
        /// <returns>哈希值的Base64格式，如果 text 为空则返回空字符串</returns>
        public static string ComputeSha256Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
            var hashedPassword = Convert.ToBase64String(hashedBytes);
            return hashedPassword;
        }
    }
}
