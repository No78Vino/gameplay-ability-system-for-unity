using System.Collections.Generic;

namespace GAS.Runtime
{
    /// <summary>
    /// EX-GAS泛用型参数接口
    /// 泛用型参数都必须实现Luban的excel表参数读写函数
    /// 
    /// 因为要兼容所有的泛型参数的陪标读写，所以EX-GAS项目的luban配置采用流式配置。
    /// 但是相应的流式配置不支持空占位的配置。
    /// 所以要注意：【必须在excel的[读/写函数]以及各个[自定义逻辑]里处理好默认参数】
    /// </summary>
    public interface XParam
    {
#if UNITY_EDITOR
        
        public void DecodeExcelData(List<object> paramData);
        
        /// <summary>
        /// 请一定要用默认占位数据代替空数据处理！！！
        /// </summary>
        /// <returns></returns>
        public List<object> EncodeExcelData();
#endif
    }
    
    public static class XParamDefault
    {
        public const int DefaultInt = 0;
        public const string DefaultString = "\"\"";
        public const float DefaultFloat = 0f;
        public const bool DefaultBool = false;
    }
}