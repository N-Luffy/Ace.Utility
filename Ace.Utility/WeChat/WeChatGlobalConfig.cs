using System;
using System.Collections.Generic;
using System.Text;

namespace Ace.Utility.WeChat
{
    /// <summary>
    /// 微信全局配置
    /// </summary>
    public static class WeChatGlobalConfig
    {
        /// <summary>
        /// 延迟响应微信集合
        /// </summary>
        // key: 微信CorpId
        // Value: 微信Token配置
        public static Dictionary<string, WeChatTokenConfig> StopResponseCorpIds { get; set; } = new Dictionary<string, WeChatTokenConfig>();

        /// <summary>
        /// 当前是否正在访问微信  0未获取  1获取
        /// </summary>
        public static int CurrentState = 0;
    }

    /// <summary>
    /// 微信Token配置
    /// </summary>
    public class WeChatTokenConfig
    {
        /// <summary>
        /// 最大访问微信次数
        /// </summary>
        public readonly int MaxTokenCount = 10;

        /// <summary>
        /// 当前请求次数
        /// </summary>
        public int CurrentRequestCount { get; set; } = 0;

        /// <summary>
        /// 重置请求次数时间
        /// </summary>
        public DateTime ResetRequestCountDate { get; set; }

        /// <summary>
        /// 延迟响应时间
        /// </summary>
        // 由网络错误、密钥错误或其他错误导致token未获取，将停止一段时间响应，避免不停获取次数过多导致微信限制访问
        public DateTime StopResponseDate { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// token过期时间
        /// </summary>
        public DateTime ExpiresDate { get; set; }
    }
}
