using System.Security.Cryptography;
using System.Text;

namespace MAuth.Web.Commons.Helpers
{
    public static class HashHelper
    {
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
