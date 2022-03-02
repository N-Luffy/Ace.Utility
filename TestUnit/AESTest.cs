using Xunit;

namespace TestUnit
{
    public class AESTest
    {
        [Fact]
        public void Test()
        {
            var str = "Here is some data to encrypt!";

            var key = "1234567890987654";

            var encryptStr = Ace.Utility.AES.AesCrypto.Encrypt(str, key);
            System.Diagnostics.Debug.WriteLine("AesCrypto 加密字符：" + encryptStr);

            var decryptStr = Ace.Utility.AES.AesCrypto.Decrypt(encryptStr, key);
            System.Diagnostics.Debug.WriteLine("AesCrypto 解密字符：" + decryptStr);
        }
    }
}
