// Assets/EXUI/Runtime/Attribute/XUIBindAttributes.cs  
using System;  
  
namespace EXUI  
{  
    public enum BindingMode { OneWay, TwoWay, OneTime }  
  
    /// <summary>  
    /// 单向绑定：VM.Property → UI组件.Property  
    /// 用于 Text、Image.fillAmount 等只需读取 VM 值的场景  
    /// </summary>  
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]  
    public class BindOneWayAttribute : Attribute  
    {  
        public string NodePath { get; }        // GetComponentByNode 的节点路径  
        public string VMPropertyName { get; }  // VM 上 ObservableVariable 属性名  
        public string UIPropertyName { get; }  // UI组件上的属性名（默认 "text"）  
  
        public BindOneWayAttribute(string nodePath, string vmPropertyName, string uiProperty)  
        {  
            NodePath = nodePath;  
            VMPropertyName = vmPropertyName;  
            UIPropertyName = uiProperty;  
        }  
    }  
  
    /// <summary>  
    /// 双向绑定：VM ↔ UI组件（适用于 InputField 等）  
    /// </summary>  
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]  
    public class BindTwoWayAttribute : Attribute  
    {  
        public string NodePath { get; }  
        public string VMPropertyName { get; }  
        public string UIPropertyName { get; }  
  
        public BindTwoWayAttribute(string nodePath, string vmPropertyName, string uiProperty = "text")  
        {  
            NodePath = nodePath;  
            VMPropertyName = vmPropertyName;  
            UIPropertyName = uiProperty;  
        }  
    }  
}