using Ace.Utility;

using System;

using Xunit;

namespace TestUnit
{
    public class SnowflakeTest
    {
        [Fact]
        public void Test()
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
