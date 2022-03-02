using Ace.Utility;

using System.Collections.Generic;

using Xunit;

namespace TestUnit
{
    public class EmailTest
    {
        [Fact]
        public void Test()
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
    }
}
