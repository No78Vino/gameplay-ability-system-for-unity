using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     模块级单例管理器：持有跨场景的运行时根节点，
    ///     供程序化动画运行时生成的辅助对象（足部锚点等）挂载。
    /// </summary>
    public class EXProceduralMachineManager
    {
        private static EXProceduralMachineManager _instance;

        public static EXProceduralMachineManager Instance => _instance ??= new EXProceduralMachineManager();

        /// <summary>运行时根节点（DontDestroyOnLoad）</summary>
        public Transform ManagerRoot { get; }

        private EXProceduralMachineManager()
        {
            var go = new GameObject("EXProceduralMachineManager");
            Object.DontDestroyOnLoad(go);
            ManagerRoot = go.transform;
        }

        /// <summary>销毁运行时根节点（下次访问 Instance 时会自动重建）</summary>
        public void Dispose()
        {
            if (ManagerRoot != null)
                Object.Destroy(ManagerRoot.gameObject);
        }
    }
}