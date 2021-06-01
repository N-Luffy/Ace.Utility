using System;
using System.Collections.Generic;
using System.Text;

namespace Ace.Utility.RabbitMq
{
    /// <summary>
    /// MQ配置信息
    /// </summary>
    public class RabbitMqConfig
    {
        /// <summary>
        /// 主机名
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 心跳时间
        /// </summary>
        public TimeSpan? HeartBeat { get; set; }

        /// <summary>
        /// 自动重连
        /// </summary>
        public bool? AutomaticRecoveryEnabled { get; set; }

        /// <summary>
        /// 重连时间
        /// </summary>
        public TimeSpan? NetworkRecoveryInterval { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// 死信队列实体
    /// </summary>
    [RabbitMq("dead-letter-{Queue}", ExchangeName = "dead-letter-{exchange}")]
    public class DeadLetterQueue
    {
        public string Body { get; set; }

        public string Exchange { get; set; }

        public string Queue { get; set; }

        public string RoutingKey { get; set; }

        public int RetryCount { get; set; }

        public string ExceptionMsg { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
