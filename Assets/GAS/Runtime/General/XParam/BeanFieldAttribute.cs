using System;  
using System.Runtime.CompilerServices;  
  
namespace GAS.Runtime  
{  
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]  
    public class BeanFieldAttribute : Attribute  
    {  
        /// <summary>  
        /// 绑定的 Set 方法名（必填，推荐用 nameof(SetXxx) 传入）  
        /// </summary>  
        public string Setter { get; }  
  
        /// <summary>  
        /// 覆盖 Bean 字段名（默认取成员名）  
        /// </summary>  
        public string Name { get; set; }  
  
        /// <summary>  
        /// 覆盖 Luban 类型（默认自动映射 C# 类型）  
        /// </summary>  
        public string LubanType { get; set; }  
  
        /// <summary>  
        /// Bean 字段注释  
        /// </summary>  
        public string Comment { get; set; }  
  
        /// <summary>  
        /// 字段在 Bean 中的排序权重。  
        /// 默认值由 [CallerLineNumber] 自动填入源码行号，保证声明顺序。  
        /// 也可显式指定: [BeanField("SetXxx", Order = 100)]  
        /// </summary>  
        public int Order { get; set; }  
  
        public BeanFieldAttribute(string setter, [CallerLineNumber] int order = 0)  
        {  
            Setter = setter;  
            Order = order;  
        }  
    }  
}