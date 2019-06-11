using System;
using System.Collections.Generic;
using System.Text;

namespace Ace.Utility
{
    /// <summary>
    /// 动态生产有规律的ID
    /// </summary>
    public class Snowflake
    {
        private static long _machineId; //机器ID
        private static long _datacenterId = 0L; //数据ID
        private static long _sequence = 0L; //计数从零开始

        private static long twepoch = 687888001020L; //唯一时间随机量

        private static long _machineIdBits = 5L; //机器码字节数
        private static long _datacenterIdBits = 5L; //数据字节数
        public static long MaxMachineId = -1L ^ -1L << (int)_machineIdBits; //最大机器ID
        private static long maxDatacenterId = -1L ^ (-1L << (int)_datacenterIdBits); //最大数据ID

        private static long _sequenceBits = 12L; //计数器字节数，12个字节用来保存计数码        
        private static long _machineIdShift = _sequenceBits; //机器码数据左移位数，就是后面计数器占用的位数
        private static long _datacenterIdShift = _sequenceBits + _machineIdBits;

        private static long _timestampLeftShift = _sequenceBits + _machineIdBits + _datacenterIdBits; //时间戳左移动位数就是机器码+计数器总字节数+数据字节数

        public static long SequenceMask = -1L ^ -1L << (int)_sequenceBits; //一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
        private static long _lastTimestamp = -1L; //最后时间戳

        private static object _syncRoot = new object(); //加锁对象
        static Snowflake _snowflake;

        public static Snowflake Instance()
        {
            return _snowflake ?? (_snowflake = new Snowflake());
        }

        public Snowflake()
        {
            Snowflakes(0L, -1);
        }

        public Snowflake(long machineId)
        {
            Snowflakes(machineId, -1);
        }

        public Snowflake(long machineId, long datacenterId)
        {
            Snowflakes(machineId, datacenterId);
        }

        private void Snowflakes(long machineId, long datacenterId)
        {
            if (machineId >= 0)
            {
                if (machineId > MaxMachineId)
                {
                    throw new Exception("机器码ID非法");
                }

                Snowflake._machineId = machineId;
            }

            if (datacenterId >= 0)
            {
                if (datacenterId > maxDatacenterId)
                {
                    throw new Exception("数据中心ID非法");
                }

                Snowflake._datacenterId = datacenterId;
            }
        }

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns>毫秒</returns>
        private static long GetTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        /// <summary>
        /// 获取下一微秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private static long GetNextTimestamp(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            if (timestamp <= lastTimestamp)
            {
                timestamp = GetTimestamp();
            }

            return timestamp;
        }

        /// <summary>
        /// 获取长整形的ID
        /// </summary>
        /// <returns></returns>
        public long GetId()
        {
            lock (_syncRoot)
            {
                long timestamp = GetTimestamp();
                if (Snowflake._lastTimestamp == timestamp)
                {
                    //同一微秒中生成ID
                    _sequence = (_sequence + 1) & SequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                    if (_sequence == 0)
                    {
                        //一微秒内产生的ID计数已达上限，等待下一微秒
                        timestamp = GetNextTimestamp(Snowflake._lastTimestamp);
                    }
                }
                else
                {
                    //不同微秒生成ID
                    _sequence = 0L;
                }

                if (timestamp < _lastTimestamp)
                {
                    throw new Exception("时间戳比上一次生成ID时时间戳还小，故异常");
                }

                Snowflake._lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳
                long id = ((timestamp - twepoch) << (int)_timestampLeftShift)
                          | (_datacenterId << (int)_datacenterIdShift)
                          | (_machineId << (int)_machineIdShift)
                          | _sequence;
                return id;
            }
        }
    }
}
