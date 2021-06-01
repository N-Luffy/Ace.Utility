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
                        rabbitMqService.Publish("test-exchange", "test-queue", "test-exchange", "{\"Data\":\"��ʵ��ʵ���մ�ʵ��ʵ������մ�\"}");
                        System.Threading.Thread.Sleep(1000);
                    }
                });

            Task.Factory.StartNew(() =>
            {
                rabbitMqService.Subscribe<object>("test-queue", false, t =>
                {
                    var msg = JsonConvert.SerializeObject(t);
                    Debug.WriteLine($"ʱ�䣺{DateTime.Now},��ǰ��Ϣ���ݣ�{msg}");
                    System.Threading.Thread.Sleep(1000);
                }, false);
            });
            while (true)
            {

            }
        }
    }
}
