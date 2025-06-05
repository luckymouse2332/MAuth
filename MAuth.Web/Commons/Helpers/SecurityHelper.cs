using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography;
using System.Text.Json;

namespace MAuth.Web.Commons.Helpers;

/// <summary>
/// 安全相关的帮助类
/// </summary>
public static class SecurityHelper
{
    private static readonly string KeyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rsa");
    private static readonly string PrivateKeyPath = Path.Combine(KeyDir, "key.private.json");
    private static readonly string PublicKeyPath = Path.Combine(KeyDir, "key.public.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        IncludeFields = true
    };

    /// <summary>
    /// 获取RSA密钥对，如果不存在则自动生成
    /// </summary>
    public static (RsaSecurityKey PrivateKey, RsaSecurityKey PublicKey) GetPrivateKeyAndPublicKey()
    {
        EnsureKeysExist();

        var privateKey = LoadKey(PrivateKeyPath);
        var publicKey = LoadKey(PublicKeyPath);

        return (new RsaSecurityKey(privateKey), new RsaSecurityKey(publicKey));
    }

    /// <summary>
    /// 确保密钥文件存在，不存在则生成
    /// </summary>
    private static void EnsureKeysExist()
    {
        if (!File.Exists(PrivateKeyPath) || !File.Exists(PublicKeyPath))
        {
            GenerateKeys();
        }
    }

    /// <summary>
    /// 加载指定路径的RSA参数
    /// </summary>
    private static RSAParameters LoadKey(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<RSAParameters>(json, JsonOptions)!;
    }

    /// <summary>
    /// 生成并写入密钥对
    /// </summary>
    private static void GenerateKeys()
    {
        using var rsa = new RSACryptoServiceProvider(2048);

        var privateKey = rsa.ExportParameters(true);
        var publicKey = rsa.ExportParameters(false);

        Directory.CreateDirectory(KeyDir);

        File.WriteAllText(PrivateKeyPath, JsonSerializer.Serialize(privateKey, JsonOptions));
        File.WriteAllText(PublicKeyPath, JsonSerializer.Serialize(publicKey, JsonOptions));

        rsa.PersistKeyInCsp = false;

        Log.Information("Generated new RSA key pair and saved to {Dir}", KeyDir);
    }
}
