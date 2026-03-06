// Assets/GAS/Runtime/General/XParam/BeanFieldAttribute.cs  
using System;  
  
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
  
        public BeanFieldAttribute(string setter)  
        {  
            Setter = setter;  
        }  
    }  
}