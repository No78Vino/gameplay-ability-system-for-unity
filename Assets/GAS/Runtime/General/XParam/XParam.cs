using System.Collections.Generic;

namespace GAS.Runtime
{
    /// <summary>
    /// EX-GAS泛用型参数接口
    /// 泛用型参数都必须实现Luban的excel表参数读写函数
    /// </summary>
    public interface XParam
    {
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData);
        public List<object> EncodeExcelData();
#endif
    }
}