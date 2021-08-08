using System;
using System.Collections.Generic;

using Ace.Utility;

using ConsoleApp1;

using Xunit;

namespace TestUnit
{
    public class UnitTest
    {
        [Fact]
        public void TestEmail()
        {
            SendServerConfiguration sendServer = new SendServerConfiguration
            {
                IsSsl = false,
                SmtpHost = "smtp.163.com",
                SmtpPort = 25,
                SenderAccount = "test@163.com",
                SenderPassword = "123456"
            };
            EmailBodyModel model = new EmailBodyModel
            {
                FromAddress = "test@163.com",
                ToAddress = "123456789@qq.com",
                Subject = "��������ҿ���",
                IsBodyHtml = false,
                Body = "���λ׼ʱ�μ�����15��30�Ļ��顣",
                Attachments = new List<string> { @"D:\Test\ע������.txt", @"D:\Test\�����¼ģ��.xlsx" }
            };
            EmailUtils.SendEmail(model, sendServer);
        }

        [Fact]
        public void TestSnowflake()
        {
            for (int i = 0; i < 1000; i++)
            {
                var id = Snowflake.Instance().GetId();
                Console.WriteLine(
                    $"��ʼִ�� {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff")}   NO: {id} HashCode: {id.GetHashCode()}");
            }
        }

        [Fact]
        public void AESTest()
        {
            var str = "Here is some data to encrypt!";

            var key = "1234567890987654";

            var encryptStr = Ace.Utility.AES.AesCrypto.Encrypt(str, key);
            System.Diagnostics.Debug.WriteLine("AesCrypto �����ַ���" + encryptStr);

            var decryptStr = Ace.Utility.AES.AesCrypto.Decrypt(encryptStr, key);
            System.Diagnostics.Debug.WriteLine("AesCrypto �����ַ���" + decryptStr);
        }
    }
}
