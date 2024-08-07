using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EasyChat.Utilities;

public class EncryptUtilities
{
    // 硬编码密钥和 IV
    private const string AesKey = "WoYongYuanXiHuanSikadi_WeiSikadi"; // 32 字符
    private const string AesIv = "XianShangXinZang"; // 16 字符

    /// <summary>
    ///     AES 加密
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string Encrypt(string content)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(AesKey);
        aes.IV = Encoding.UTF8.GetBytes(AesIv);
        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using MemoryStream msEncrypt = new();
        using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(content);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    /// <summary>
    ///     解密
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string Decrypt(string content)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(AesKey);
        aes.IV = Encoding.UTF8.GetBytes(AesIv);

        var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);

        using var msDecrypt = new MemoryStream(Convert.FromBase64String(content));
        using var csDecrypt = new CryptoStream(msDecrypt, decrypt, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}