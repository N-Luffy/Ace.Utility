using Ace.Utility.Mapper;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Xunit;

namespace TestUnit
{
    public class MapperExpressionTest
    {
        [Fact]
        public void Test()
        {
            var person = new Person
            {
                Id = 1,
                Name = "zhangsan",
                Addr = "地球",
                IdentityNo = "111111",
                Sex = EnumSex.FeMale,
                Birth = DateTime.Now.AddYears(-10),
                Description = "哈哈",
                Age = 20,
                Nick = "aaa",
                QQ = "123",
                Email = "2341@qq.com",
                Phone = "13845614789",
                WeiXin = "14563298785",
                Picture = "http://asdasd.jpg",
                Intro = "bb",
                Score = 21.00,
                Books = new List<string> { "斗罗大陆" },
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
            var personDto = MapperExtension.GetMapper<Person, PersonDto>(person);
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(personDto));
        }

        [Fact]
        public void BatchTest()
        {
            var person = new Person
            {
                Id = 1,
                Name = "zhangsan",
                Addr = "地球",
                IdentityNo = "111111",
                Sex = EnumSex.FeMale,
                Description = "哈哈",
                Age = 20,
                Nick = "aaa",
                Email = "2341@qq.com",
                Picture = "http://asdasd.jpg",
                Intro = "bb",
                Score = 21.00,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
            PersonDto dto = null;
            var func = MapperExtension.GetMapperFunc<Person, PersonDto>();
            var count = 50;
            var st = new Stopwatch();
            st.Start();
            for (var i = 0; i < count; i++)
            {
                var st1 = new Stopwatch();
                st1.Start();
                for (var j = 0; j < 10000; j++)
                {
                    dto = func(person);
                }
                st1.Stop();
                System.Diagnostics.Debug.WriteLine($"第{(i + 1)}次：{st1.ElapsedMilliseconds} 毫秒");
            }
            st.Stop();
            System.Diagnostics.Debug.WriteLine($"表达式树 总耗时：{st.ElapsedMilliseconds} 毫秒 平均：{st.ElapsedMilliseconds / count} 毫秒");
        }
    }

    enum EnumSex { Male, FeMale }
    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Addr { get; set; }
        public string IdentityNo { get; set; }
        public EnumSex Sex { get; set; }
        public DateTime Birth { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }
        public string Nick { get; set; }
        public string QQ { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WeiXin { get; set; }
        public string Picture { get; set; }
        public string Intro { get; set; }
        public double Score { get; set; }
        public List<string> Books { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Addr { get; set; }
        public string IdentityNo { get; set; }
        public EnumSex Sex { get; set; }
        public DateTime Birth { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }
        public string Nick { get; set; }
        public string QQ { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WeiXin { get; set; }
        public string Picture { get; set; }
        public string Intro { get; set; }
        public double Score { get; set; }
        public List<string> Books { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
