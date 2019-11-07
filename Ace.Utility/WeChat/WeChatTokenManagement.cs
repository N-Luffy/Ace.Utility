using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static System.String;

namespace Ace.Utility.WeChat
{
    /// <summary>
    /// 微信Token管理
    /// </summary>
    public class WeChatTokenManagement
    {
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="corpId">用户所属企业的corpId</param>
        /// <param name="corpSecret">密钥</param>
        /// <returns></returns>
        public string GetToken(string corpId, string corpSecret)
        {
            string token = GetCacheToken(corpId);
            if (!IsNullOrWhiteSpace(token)) return token;

            while (IsNullOrWhiteSpace(token))
            {
                if (0 == Interlocked.CompareExchange(ref WeChatGlobalConfig.CurrentState, 1, 0))
                {
                    token = GetCacheToken(corpId);
                    if (!IsNullOrWhiteSpace(token))
                    {
                        Interlocked.CompareExchange(ref WeChatGlobalConfig.CurrentState, 0, 1);
                        break;
                    }

                    WeChatTokenConfig weChatTokenConfig = AddTokenConfig(corpId);

                    if (weChatTokenConfig.CurrentRequestCount >= weChatTokenConfig.MaxTokenCount && weChatTokenConfig.ResetRequestCountDate > DateTime.Now)
                    {
                        Interlocked.CompareExchange(ref WeChatGlobalConfig.CurrentState, 0, 1);
                        break;
                    }

                    if (weChatTokenConfig.ResetRequestCountDate <= DateTime.Now)
                    {
                        weChatTokenConfig.CurrentRequestCount = 0;
                        weChatTokenConfig.ResetRequestCountDate = DateTime.Now.AddDays(1).Date;
                        weChatTokenConfig.StopResponseDate = DateTime.MinValue;
                    }

                    if (weChatTokenConfig.StopResponseDate >= DateTime.Now)
                    {
                        Interlocked.CompareExchange(ref WeChatGlobalConfig.CurrentState, 0, 1);
                        break;
                    }

                    try
                    {
                        token = GetWeChatToken(corpId, corpSecret);
                        weChatTokenConfig.CurrentRequestCount += 1;
                        if (token.Contains("error"))
                        {
                            weChatTokenConfig.StopResponseDate = DateTime.Now.AddSeconds(30);
                        }
                        else
                        {
                            weChatTokenConfig.Token = token;
                            weChatTokenConfig.ExpiresDate = DateTime.Now.AddSeconds(7200 - 300);
                        }
                    }
                    catch (Exception e)
                    {
                        weChatTokenConfig.StopResponseDate = DateTime.Now.AddSeconds(30);
                        Interlocked.CompareExchange(ref WeChatGlobalConfig.CurrentState, 0, 1);
                        break;
                    }
                    Interlocked.CompareExchange(ref WeChatGlobalConfig.CurrentState, 0, 1);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(1000);
                        token = GetCacheToken(corpId);
                        if (!IsNullOrWhiteSpace(token)) break;
                    }
                }
            }

            return token;
        }

        /// <summary>
        /// 从缓存中拿到token
        /// <param name="corpId">用户所属企业的corpId</param>
        /// </summary>
        /// <returns></returns>
        public string GetCacheToken(string corpId)
        {
            bool v = WeChatGlobalConfig.StopResponseCorpIds.ContainsKey(corpId);
            return v ? WeChatGlobalConfig.StopResponseCorpIds[corpId].ExpiresDate >= DateTime.Now ? WeChatGlobalConfig.StopResponseCorpIds[corpId].Token : "" : "";
        }

        /// <summary>
        /// 新增微信token配置
        /// </summary>
        /// <param name="corpId">用户所属企业的corpId</param>
        /// <returns></returns>
        private WeChatTokenConfig AddTokenConfig(string corpId)
        {
            if (!WeChatGlobalConfig.StopResponseCorpIds.ContainsKey(corpId))
            {
                WeChatGlobalConfig.StopResponseCorpIds.Add(corpId, new WeChatTokenConfig
                {
                    ResetRequestCountDate = DateTime.Now.AddDays(1).Date
                });
            }

            return WeChatGlobalConfig.StopResponseCorpIds[corpId];
        }

        /// <summary>
        /// 获取微信token
        /// </summary>
        /// <param name="corpId">用户所属企业的corpId</param>
        /// <param name="corpSecret">密钥</param>
        /// <returns></returns>
        public string GetWeChatToken(string corpId, string corpSecret)
        {
            //模拟请求 线程停止2秒
            Thread.Sleep(2000);
            return "test-test";
        }
    }
}
