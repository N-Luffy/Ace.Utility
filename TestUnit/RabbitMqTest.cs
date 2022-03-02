using Ace.Utility.RabbitMq;

using Newtonsoft.Json;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Xunit;

namespace TestUnit
{
    public class RabbitMqTest
    {
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
