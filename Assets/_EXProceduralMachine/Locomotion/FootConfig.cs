using System;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     单足配置：骨骼链引用 + 落点参数。
    ///     hip / knee / foot 由自研 IK（TwoBoneIK）直接驱动，无需任何第三方 IK 插件。
    /// </summary>
    [Serializable]
    public struct FootConfig
    {
        [Tooltip("髋部骨骼（大腿根部）")]
        public Transform hip;

        [Tooltip("膝部骨骼（小腿根部）")]
        public Transform knee;

        [Tooltip("踝部骨骼（脚掌末端）")]
        public Transform foot;

        [Tooltip("膝盖弯曲方向参考点（可选；不填时保持初始弯曲方向）")]
        public Transform pole;

        [Tooltip("足部待机锚点：相对身体的静止参考位置（用于步幅测量与复位）")]
        public Transform idlePoint;

        [Tooltip("落点相对地面投射点的偏移")]
        public Vector3 offset;

        [Tooltip("脚掌对齐地面法线（贴地）")]
        public bool alignFootToGround;

        [Tooltip("脚掌本地上方向轴；zero 时默认按 -up 对齐")]
        public Vector3 footUpAxis;
    }
}