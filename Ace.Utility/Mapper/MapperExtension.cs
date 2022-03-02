using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace Ace.Utility.Mapper
{
    /// <summary>
    /// 利用Expression表达式树完成AutoMapper类似的功能
    /// </summary>
    public class MapperExtension
    {
        public static TDestination GetMapper<TSource, TDestination>(TSource source)
        {
            return GetMapperFunc<TSource, TDestination>()(source);
        }

        public static Func<TSource, TDestination> GetMapperFunc<TSource, TDestination>()
        {
            var para = Expression.Parameter(typeof(TSource), "src");

            //var res=new TDestination()
            var variable = Expression.Variable(typeof(TDestination), "res");
            var newExp = Expression.New(typeof(TDestination).GetConstructor(new Type[0]));
            var assign = Expression.Assign(variable, newExp);

            //属性赋值
            var assignProps = new List<BinaryExpression>();
            var props = typeof(TSource).GetProperties();
            var destProps = typeof(TDestination).GetProperties();
            foreach (var prop in destProps)
            {
                if (prop.CanWrite)
                {
                    var srcProp = props.FirstOrDefault(t => t.Name == prop.Name);
                    if (srcProp != null)
                    {
                        //属性
                        var assignProp = Expression.Assign(Expression.Property(variable, prop.Name), Expression.MakeMemberAccess(para, srcProp));
                        assignProps.Add(assignProp);
                    }
                }
            }

            //return res
            var labelTarget = Expression.Label(typeof(TDestination));
            var labelExpression = Expression.Label(labelTarget, variable);
            var gotoExpression = Expression.Return(labelTarget, variable, typeof(TDestination));

            var expressions = new List<Expression>();
            expressions.Add(assign);
            expressions.AddRange(assignProps);
            expressions.Add(gotoExpression);
            expressions.Add(labelExpression);

            //组装表达式块，编译生成委托
            var block = Expression.Block(typeof(TDestination), new ParameterExpression[] { variable }, expressions);
            var mapperFunc = Expression.Lambda<Func<TSource, TDestination>>(block, para).Compile();
            return mapperFunc;
        }
    }
}
