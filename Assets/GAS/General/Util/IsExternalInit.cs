using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// 让Unity支持init setter
    /// <see href="https://docs.unity.cn/cn/2022.3/Manual/CSharpCompiler.html">unity文档</see>。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class IsExternalInit
    {
    }
}