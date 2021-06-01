using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Ace.Utility;
using Ace.Utility.RabbitMq;
using ConsoleApp1;
using Newtonsoft.Json;
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

        [Fact]
        public void TestRabbitMq()
        {
            RabbitMqConfig rabbitMqConfig = new RabbitMqConfig
            {
                Host = "10.10.10.110",
                UserName = "guest",
                Password = "guest",
            };
            RabbitMqService rabbitMqService = new RabbitMqService(rabbitMqConfig);

            Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        rabbitMqService.Publish("test-exchange", "test-queue", "test-exchange", "{\"Data\":\"啊实打实大苏打实打实打算大苏打\"}");
                        System.Threading.Thread.Sleep(1000);
                    }
                });

            Task.Factory.StartNew(() =>
            {
                rabbitMqService.Subscribe<object>("test-queue", false, t =>
                {
                    var msg = JsonConvert.SerializeObject(t);
                    Debug.WriteLine($"时间：{DateTime.Now},当前消息内容：{msg}");
                    System.Threading.Thread.Sleep(1000);
                }, false);
            });
            while (true)
            {

            }
        }
    }
}
