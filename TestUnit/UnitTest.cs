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
                Subject = "下午会议室开会",
                IsBodyHtml = false,
                Body = "请各位准时参加下午15：30的会议。",
                Attachments = new List<string> { @"D:\Test\注意事项.txt", @"D:\Test\会议记录模板.xlsx" }
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
                    $"开始执行 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff")}   NO: {id} HashCode: {id.GetHashCode()}");
            }
        }
    }
}
