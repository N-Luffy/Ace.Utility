using System.Collections;
using System.Reflection;
using System.Text;

namespace Ace.Utility
{
    public static class FieldAttributeExtensions
    {
        /// <summary>
        /// 反射类读取DescriptionAttribute的字段
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFieldDescriptions<T>(this T model)
        {
            Dictionary<string, string> descriptions = new Dictionary<string, string>();
            if (model == null) return descriptions;
            Type type = model.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                var attribute = property.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
                if (attribute != null)
                {
                    descriptions.Add(property.Name, attribute.Description);
                }
            }
            return descriptions;
        }

        /// <summary>
        /// 反射类读取DescriptionAttribute的值，拼接成字符串 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string ExtractToString<T>(this T model)
        {
            var trees = Execute(model);
            var builder = new StringBuilder();
            Next(trees, builder);
            return builder.ToString().Trim(',');
        }

        /// <summary>
        /// 拼接字符串
        /// </summary>
        /// <param name="linkTrees"></param>
        /// <param name="builder"></param>
        static void Next(List<LinkTree> linkTrees, StringBuilder builder)
        {
            foreach (var item in linkTrees)
            {
                var isNode = item.LinkTrees != null && item.LinkTrees.Any();
                if (isNode)
                {
                    builder.Append(item.Text + ":{");
                    Next(item.LinkTrees, builder);
                    builder = builder.Remove(builder.Length - 1, 1);
                    builder.Append("}");
                }
                else
                {
                    builder.Append($"{item.Text}:{item.Value},");
                }
            }
        }

        /// <summary>
        /// 反射类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        static List<LinkTree> Execute<T>(T model)
        {
            List<LinkTree> linkTrees = new List<LinkTree>();
            if (model == null) return linkTrees;
            var modelType = model.GetType();

            foreach (var property in modelType.GetProperties())
            {
                var descriptionAttribute = property.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
                if (descriptionAttribute == null) continue;

                var (value, isBasisType) = GetPropertyValue(property, model);
                if (value == null || string.IsNullOrEmpty(value.ToString())) continue;

                var link = new LinkTree { Text = descriptionAttribute.Description, LinkTrees = new List<LinkTree>() };
                if (property.PropertyType.IsArray || (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string)))
                {
                    var itemType = property.PropertyType.IsArray
                        ? property.PropertyType.GetElementType() : property.PropertyType.GetGenericArguments().FirstOrDefault();

                    var listVal = TypeConvertEnumerable(itemType, value);
                    if (listVal == null) continue;
                    if (isBasisType) link.Value = $"[{string.Join(",", listVal.Cast<object>())}]";
                    else foreach (var it in listVal) link.LinkTrees.AddRange(Execute(it));
                }
                else if (property.PropertyType.IsClass)
                {
                    if (isBasisType) link.Value = value.ToString() ?? "";
                    else link.LinkTrees.AddRange(Execute(value));
                }
                else link.Value = value.ToString() ?? "";

                linkTrees.Add(link);
            }
            return linkTrees;
        }

        /// <summary>
        /// 获取字段属性的值
        /// </summary>
        /// <param name="property"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        static (object?, bool) GetPropertyValue(PropertyInfo property, object model)
        {
            var value = property.GetValue(model);

            if (IsBasisTypeMatch(property.PropertyType)) return (value, true);   //其他类型
            else if (property.PropertyType.IsArray) //数组类型
            {
                var elementType = property.PropertyType.GetElementType();
                if (IsBasisTypeMatch(elementType)) return (value, true);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) //集合类型
            {
                var itemType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                if (itemType != null && IsBasisTypeMatch(itemType)) return (value, true);
            }
            return (value, false);
        }

        /// <summary>
        /// 是否基础类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static bool IsBasisTypeMatch(Type type)
        {
            List<Type> types = new List<Type>
            {
                typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort), typeof(byte), typeof(sbyte), typeof(float), typeof(double), typeof(decimal),
                typeof(char), typeof(bool), typeof(string), typeof(DateTime), typeof(Guid)
            };
            return types.Any(elementType => elementType == type);
        }

        /// <summary>
        /// 类型转换集合
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        static IEnumerable? TypeConvertEnumerable(Type type, object val)
        {
            if (type == typeof(int)) return val as IEnumerable<int>;
            if (type == typeof(uint)) return val as IEnumerable<uint>;
            if (type == typeof(long)) return val as IEnumerable<long>;
            if (type == typeof(ulong)) return val as IEnumerable<ulong>;
            if (type == typeof(short)) return val as IEnumerable<short>;
            if (type == typeof(ushort)) return val as IEnumerable<ushort>;
            if (type == typeof(byte)) return val as IEnumerable<byte>;
            if (type == typeof(sbyte)) return val as IEnumerable<sbyte>;
            if (type == typeof(float)) return val as IEnumerable<float>;
            if (type == typeof(double)) return val as IEnumerable<double>;
            if (type == typeof(decimal)) return val as IEnumerable<decimal>;
            if (type == typeof(char)) return val as IEnumerable<char>;
            if (type == typeof(bool)) return val as IEnumerable<bool>;
            if (type == typeof(string)) return val as IEnumerable<string>;
            if (type == typeof(DateTime)) return val as IEnumerable<DateTime>;
            if (type == typeof(Guid)) return val as IEnumerable<Guid>;
            return val as IEnumerable<object>;
        }

        protected class LinkTree
        {
            public string Text;
            public string Value;
            public List<LinkTree> LinkTrees;
        }
    }
}
