using Newtonsoft.Json;
using Serilog;
using System.Security.Cryptography;

namespace MAuth.Web.Helpers
{
    public static class JwtKeyHelper
    {
        /// <summary>
        /// 生成用于JWT非对称加密的公钥和私钥
        /// </summary>
        /// <param name="env"></param>
        public static void GenerateKey(IHostEnvironment env)
        {
            RSAParameters privateKey, publicKey;

            // >= 2048 否则长度太短不安全
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    privateKey = rsa.ExportParameters(true);
                    publicKey = rsa.ExportParameters(false);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            var dir = Path.Combine(env.ContentRootPath, "Rsa");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Log.Information("Created a new directory to save keys: {0}", dir);
            }

            File.WriteAllText(Path.Combine(dir, "key.private.json"), JsonConvert.SerializeObject(privateKey));
            File.WriteAllText(Path.Combine(dir, "key.public.json"), JsonConvert.SerializeObject(publicKey));
            
            Log.Information("Keys are created successfully!");
        }
    }
}
