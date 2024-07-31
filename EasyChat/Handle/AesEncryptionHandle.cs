using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

namespace EasyChat.Handle
{
    public class AesEncryptionHandle
    {
        // 硬编码密钥和 IV
        private static readonly string AesKey = "WoYongYuanXiHuanSikadi_WeiSikadi"; // 32 字符
        private static readonly string AesIV = "XianShangXinZang";   // 16 字符

        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(AesKey);
                aes.IV = Encoding.UTF8.GetBytes(AesIV);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(AesKey);
                aes.IV = Encoding.UTF8.GetBytes(AesIV);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
