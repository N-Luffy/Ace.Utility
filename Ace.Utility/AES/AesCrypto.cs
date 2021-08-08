using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ace.Utility.AES
{
    /// <summary>
    /// AES加密解密
    /// </summary>
    public static class AesCrypto
    {
        //对称加密和分组加密中的四种模式(ECB、CBC、CFB、OFB),这三种的区别，主要来自于密钥的长度，16位密钥=128位，24位密钥=192位，32位密钥=256位。
        //更多参考：http://www.cnblogs.com/happyhippy/archive/2006/12/23/601353.html

        /// <summary>
        /// 检验密钥是否有效长度【16|24|32】
        /// </summary>
        /// <param name="key">密钥</param>
        /// <returns>bool</returns>
        private static bool CheckKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            if (16.Equals(key.Length) || 24.Equals(key.Length) || 32.Equals(key.Length))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 检验向量是否有效长度【16】
        /// </summary>
        /// <param name="iv">向量</param>
        /// <returns>bool</returns>
        private static bool CheckIv(string iv)
        {
            if (string.IsNullOrWhiteSpace(iv))
                return true;
            if (16.Equals(iv.Length))
                return true;
            else
                return false;
        }

        #region 参数是string类型的
        /// <summary>
        ///  加密 参数：string
        /// </summary>
        /// <param name="palinData">明文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>string：密文</returns>
        public static string Encrypt(string palinData, string key, string iv = "")
        {
            if (string.IsNullOrWhiteSpace(palinData)) return null;
            if (!(CheckKey(key) && CheckIv(iv))) return palinData;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(palinData);
            var rm = new RijndaelManaged
            {
                IV = !string.IsNullOrWhiteSpace(iv) ? Encoding.UTF8.GetBytes(iv) : Encoding.UTF8.GetBytes(key.Substring(0, 16)),
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///  解密 参数：string
        /// </summary>
        /// <param name="encryptedData">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>string：明文</returns>
        public static string Decrypt(string encryptedData, string key, string iv = "")
        {
            if (string.IsNullOrWhiteSpace(encryptedData)) return null;
            if (!(CheckKey(key) && CheckIv(iv))) return encryptedData;
            byte[] toEncryptArray = Convert.FromBase64String(encryptedData);
            var rm = new RijndaelManaged
            {
                IV = !string.IsNullOrWhiteSpace(iv) ? Encoding.UTF8.GetBytes(iv) : Encoding.UTF8.GetBytes(key.Substring(0, 16)),
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }
        #endregion

        #region 参数是byte[]类型的
        /// <summary>  
        /// 加密 参数：byte[] 
        /// </summary>  
        /// <param name="palinData">明文</param>  
        /// <param name="key">密钥</param>  
        /// <param name="iv">向量</param>  
        /// <returns>byte[]：密文</returns>  
        public static byte[] Encrypt(byte[] palinData, string key, string iv = "")
        {
            if (palinData == null) return null;
            if (!(CheckKey(key) && CheckIv(iv))) return palinData;
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            byte[] bVector = new byte[16];
            if (!string.IsNullOrWhiteSpace(iv))
                Array.Copy(Encoding.UTF8.GetBytes(iv.PadRight(bVector.Length)), bVector, bVector.Length);
            else
                Array.Copy(Encoding.UTF8.GetBytes(key.Substring(0, 16).PadRight(bVector.Length)), bVector, bVector.Length);
            byte[] cryptograph = null; // 加密后的密文  
            Rijndael Aes = Rijndael.Create();
            // 开辟一块内存流  
            using (MemoryStream Memory = new MemoryStream())
            {
                // 把内存流对象包装成加密流对象  
                using (CryptoStream Encryptor = new CryptoStream(Memory, Aes.CreateEncryptor(bKey, bVector), CryptoStreamMode.Write))
                {
                    // 明文数据写入加密流  
                    Encryptor.Write(palinData, 0, palinData.Length);
                    Encryptor.FlushFinalBlock();
                    cryptograph = Memory.ToArray();
                }
            }
            return cryptograph;
        }

        /// <summary>  
        /// 解密  参数：byte[] 
        /// </summary>  
        /// <param name="encryptedData">被解密的密文</param>  
        /// <param name="key">密钥</param>  
        /// <param name="iv">向量</param>  
        /// <returns>byte[]：明文</returns>  
        public static byte[] Decrypt(byte[] encryptedData, string key, string iv = "")
        {
            if (encryptedData == null) return null;
            if (!(CheckKey(key) && CheckIv(iv))) return encryptedData;
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            byte[] bVector = new byte[16];
            if (!string.IsNullOrWhiteSpace(iv))
                Array.Copy(Encoding.UTF8.GetBytes(iv.PadRight(bVector.Length)), bVector, bVector.Length);
            else
                Array.Copy(Encoding.UTF8.GetBytes(key.Substring(0, 16).PadRight(bVector.Length)), bVector, bVector.Length);
            byte[] original = null; // 解密后的明文  
            Rijndael Aes = Rijndael.Create();
            // 开辟一块内存流，存储密文  
            using (MemoryStream Memory = new MemoryStream(encryptedData))
            {
                // 把内存流对象包装成加密流对象  
                using (CryptoStream Decryptor = new CryptoStream(Memory, Aes.CreateDecryptor(bKey, bVector), CryptoStreamMode.Read))
                {
                    // 明文存储区  
                    using (MemoryStream originalMemory = new MemoryStream())
                    {
                        byte[] Buffer = new byte[1024];
                        int readBytes = 0;
                        while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                        {
                            originalMemory.Write(Buffer, 0, readBytes);
                        }
                        original = originalMemory.ToArray();
                    }
                }
            }
            return original;
        }
        #endregion
    }
}
