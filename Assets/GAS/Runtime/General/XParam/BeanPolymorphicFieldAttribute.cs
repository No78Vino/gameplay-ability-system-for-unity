using System;
using System.Runtime.CompilerServices;

namespace GAS.Runtime  
{  
    /// <summary>  
    /// 标记一个字段为"多态 Bean 容器"的类型判别符。  
    /// 当 Luban 中用单一多态 Bean 字段（如 CueLogic、TargetCatcherBase）表示的数据,  
    /// 在运行时需要拆解为 (TypeName + Param) 两个字段时使用。  
    /// 代码生成器会自动处理多态 Bean → 运行时字段的拆解。  
    /// </summary>  
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]  
    public class BeanPolymorphicFieldAttribute : Attribute  
    {  
        /// <summary>  
        /// 写入 __beans__.xlsx 的字段名（如 "CueLogic"、"TargetCatcher"）  
        /// </summary>  
        public string BeanFieldName { get; }  
  
        /// <summary>  
        /// Luban 多态抽象 Bean 类型名（如 "CueLogic"、"TargetCatcherBase"）  
        /// </summary>  
        public string LubanPolymorphicType { get; }  
  
        /// <summary>  
        /// 类型判别符的 Setter 方法名（如 "SetCueType"、"SetCatcherType"）  
        /// </summary>  
        public string TypeSetter { get; }  
  
        /// <summary>  
        /// 关联的 Param 字段的 Setter 方法名（如 "SetParam"）  
        /// </summary>  
        public string ParamSetter { get; }  
  
        /// <summary>  
        /// 运行时获取 Param 类型的静态方法全路径  
        /// 如 "CueHelper.GetCueLogicParamType"  
        /// 或 "TargetCatcherHelper.GetCatcherParamType"  
        /// </summary>  
        public string ParamTypeResolver { get; set; }  
  
        /// <summary>  
        /// Editor 侧获取所有子类的 Helper 类别标识  
        /// 用于代码生成器定位对应的 EditorHelper  
        /// 可选值: "Cue", "TargetCatcher" 等  
        /// </summary>  
        public string HelperCategory { get; set; }  
  
        
        /// <summary>
        ///   
        /// </summary>
        public int Order { get; set; }  
        
        public BeanPolymorphicFieldAttribute(  
            string beanFieldName,  
            string lubanPolymorphicType,  
            string typeSetter,  
            string paramSetter,
            [CallerLineNumber] int order = 0)  
        {  
            BeanFieldName = beanFieldName;  
            LubanPolymorphicType = lubanPolymorphicType;  
            TypeSetter = typeSetter;  
            ParamSetter = paramSetter;  
            Order = order;
        }  
    }  
}