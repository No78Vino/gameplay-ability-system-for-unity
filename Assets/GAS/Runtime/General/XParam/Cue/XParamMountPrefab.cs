using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// CueMountPrefab 的参数类
    /// 用于配置 Prefab 挂载的各项参数
    /// </summary>
    public class XParamMountPrefab : XParam
    {
        #region 资源配置

        [ShowInInspector]
        [LabelText("Prefab资源路径")]
        [InfoBox("相对于Resources目录的路径，不含扩展名。如需使用自定义加载器，路径格式由加载器决定。")]
        [BeanField(nameof(SetPrefabPath), Comment = "Prefab资源路径")]
        public string PrefabPath;

        #endregion

        #region 挂载配置

        [ShowInInspector]
        [LabelText("挂载点路径")]
        [InfoBox("为空则挂载到宿主根节点，否则挂载到指定子节点路径")]
        [BeanField(nameof(SetMountPointPath), Comment = "挂载点路径")]
        public string MountPointPath;

        [ShowInInspector]
        [LabelText("是否跟随宿主")]
        [Tooltip("true: Prefab成为挂载点的子物体，跟随宿主移动\nfalse: Prefab挂载到场景根节点，不跟随宿主")]
        [BeanField(nameof(SetFollowHost), Comment = "是否跟随宿主")]
        public bool FollowHost = true;

        #endregion

        #region 变换配置

        [ShowInInspector]
        [LabelText("位置偏移")]
        [Tooltip("相对于挂载点的位置偏移")]
        [BeanField(nameof(SetLocalPosition), Comment = "位置偏移")]
        public Vector3 LocalPosition = Vector3.zero;

        [ShowInInspector]
        [LabelText("旋转偏移")]
        [Tooltip("相对于挂载点的旋转偏移（欧拉角）")]
        [BeanField(nameof(SetLocalRotation), Comment = "旋转偏移")]
        public Vector3 LocalRotation = Vector3.zero;

        [ShowInInspector]
        [LabelText("缩放")]
        [Tooltip("Prefab的缩放值")]
        [BeanField(nameof(SetLocalScale), Comment = "缩放")]
        public Vector3 LocalScale = Vector3.one;

        [ShowInInspector]
        [LabelText("使用世界坐标")]
        [Tooltip("true: 位置和旋转使用世界坐标系\nfalse: 位置和旋转使用相对于挂载点的本地坐标系")]
        [BeanField(nameof(SetUseWorldSpace), Comment = "使用世界坐标")]
        public bool UseWorldSpace = false;

        #endregion

        #region 渲染配置

        [ShowInInspector]
        [LabelText("渲染层级")]
        [Tooltip("设置实例化对象的Layer，-1表示不修改（保持Prefab原始设置）")]
        [BeanField(nameof(SetLayer), Comment = "渲染层级")]
        public int Layer = -1;

        [ShowInInspector]
        [LabelText("排序层")]
        [Tooltip("用于2D渲染的SortingOrder，0表示不修改")]
        [BeanField(nameof(SetSortingOrder), Comment = "排序层")]
        public int SortingOrder = 0;

        [ShowInInspector]
        [LabelText("排序层名称")]
        [Tooltip("用于2D渲染的SortingLayer名称，空表示不修改")]
        [BeanField(nameof(SetSortingLayerName), Comment = "排序层名称")]
        public string SortingLayerName = "";

        [ShowInInspector]
        [LabelText("层级传递")]
        [Tooltip("是否将Layer设置传递给所有子物体")]
        [BeanField(nameof(SetRecursiveLayer), Comment = "层级传递")]
        public bool RecursiveLayer = false;

        #endregion

        #region 生命周期配置

        [ShowInInspector]
        [LabelText("宿主销毁时销毁")]
        [Tooltip("当宿主GameObject销毁时，是否自动销毁挂载的Prefab")]
        [BeanField(nameof(SetDestroyWithHost), Comment = "宿主销毁时销毁")]
        public bool DestroyWithHost = true;

        [ShowInInspector]
        [LabelText("停止时销毁")]
        [Tooltip("当Cue停止（Deactivate）时是否立即销毁Prefab")]
        [BeanField(nameof(SetDestroyOnStop), Comment = "停止时销毁")]
        public bool DestroyOnStop = false;

        [ShowInInspector]
        [LabelText("延迟销毁时间")]
        [Tooltip("Prefab销毁前的延迟时间（秒），0表示立即销毁")]
        [PropertyRange(0f, 60f)]
        [BeanField(nameof(SetDestroyDelay), Comment = "延迟销毁时间")]
        public float DestroyDelay = 0f;

        #endregion

        #region 特效配置

        [ShowInInspector]
        [LabelText("自动播放粒子系统")]
        [Tooltip("如果Prefab包含ParticleSystem，是否自动播放")]
        [BeanField(nameof(SetAutoPlayParticle), Comment = "自动播放粒子系统")]
        public bool AutoPlayParticle = true;

        [ShowInInspector]
        [LabelText("停止时停止粒子")]
        [Tooltip("当Cue停止时是否停止粒子系统播放")]
        [BeanField(nameof(SetStopParticleOnDeactivate), Comment = "停止时停止粒子")]
        public bool StopParticleOnDeactivate = true;

        [ShowInInspector]
        [LabelText("粒子停止模式")]
        [Tooltip("粒子系统停止时的行为")]
        [BeanField(nameof(SetParticleStopAction),LubanType = "int", Comment = "粒子停止模式")]
        public ParticleSystemStopAction ParticleStopAction = ParticleSystemStopAction.None;

        #endregion

        #region 构造函数

        public XParamMountPrefab()
        {
            PrefabPath = string.Empty;
            MountPointPath = string.Empty;
            FollowHost = true;
            LocalPosition = Vector3.zero;
            LocalRotation = Vector3.zero;
            LocalScale = Vector3.one;
            UseWorldSpace = false;
            Layer = -1;
            SortingOrder = 0;
            SortingLayerName = string.Empty;
            RecursiveLayer = false;
            DestroyWithHost = true;
            DestroyOnStop = false;
            DestroyDelay = 0f;
            AutoPlayParticle = true;
            StopParticleOnDeactivate = true;
            ParticleStopAction = ParticleSystemStopAction.None;
        }

        #endregion

        #region Setter方法

        public void SetPrefabPath(string path) => PrefabPath = path;
        public void SetMountPointPath(string path) => MountPointPath = path;
        public void SetFollowHost(bool follow) => FollowHost = follow;
        public void SetLocalPosition(Vector3 pos) => LocalPosition = pos;
        public void SetLocalRotation(Vector3 rot) => LocalRotation = rot;
        public void SetLocalScale(Vector3 scale) => LocalScale = scale;
        public void SetUseWorldSpace(bool useWorld) => UseWorldSpace = useWorld;
        public void SetLayer(int layer) => Layer = layer;
        public void SetSortingOrder(int order) => SortingOrder = order;
        public void SetSortingLayerName(string layerName) => SortingLayerName = layerName;
        public void SetRecursiveLayer(bool recursive) => RecursiveLayer = recursive;
        public void SetDestroyWithHost(bool destroy) => DestroyWithHost = destroy;
        public void SetDestroyOnStop(bool destroy) => DestroyOnStop = destroy;
        public void SetDestroyDelay(float delay) => DestroyDelay = delay;
        public void SetAutoPlayParticle(bool autoPlay) => AutoPlayParticle = autoPlay;
        public void SetStopParticleOnDeactivate(bool stop) => StopParticleOnDeactivate = stop;
        public void SetParticleStopAction(int action) => ParticleStopAction = (ParticleSystemStopAction)action;

        #endregion

        #region Excel序列化

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                SetDefaults();
                return;
            }

            // 0: PrefabPath
            if (paramData.Count > 0 && paramData[0] is string s0)
                PrefabPath = s0 == XParamDefault.DefaultString ? string.Empty : s0;
            else PrefabPath = string.Empty;

            // 1: MountPointPath
            if (paramData.Count > 1 && paramData[1] is string s1)
                MountPointPath = s1 == XParamDefault.DefaultString ? string.Empty : s1;
            else MountPointPath = string.Empty;

            // 2: FollowHost
            if (paramData.Count > 2)
                bool.TryParse(paramData[2]?.ToString(), out FollowHost);
            else FollowHost = true;

            // 3: LocalPosition (格式: x;y;z)
            if (paramData.Count > 3 && paramData[3] is string s3)
                LocalPosition = ParseVector3(s3);
            else LocalPosition = Vector3.zero;

            // 4: LocalRotation (格式: x;y;z)
            if (paramData.Count > 4 && paramData[4] is string s4)
                LocalRotation = ParseVector3(s4);
            else LocalRotation = Vector3.zero;

            // 5: LocalScale (格式: x;y;z)
            if (paramData.Count > 5 && paramData[5] is string s5)
                LocalScale = ParseVector3(s5, Vector3.one);
            else LocalScale = Vector3.one;

            // 6: UseWorldSpace
            if (paramData.Count > 6)
                bool.TryParse(paramData[6]?.ToString(), out UseWorldSpace);
            else UseWorldSpace = false;

            // 7: Layer
            if (paramData.Count > 7)
                int.TryParse(paramData[7]?.ToString(), out Layer);
            else Layer = -1;

            // 8: SortingOrder
            if (paramData.Count > 8)
                int.TryParse(paramData[8]?.ToString(), out SortingOrder);
            else SortingOrder = 0;

            // 9: SortingLayerName
            if (paramData.Count > 9 && paramData[9] is string s9)
                SortingLayerName = s9 == XParamDefault.DefaultString ? string.Empty : s9;
            else SortingLayerName = string.Empty;

            // 10: RecursiveLayer
            if (paramData.Count > 10)
                bool.TryParse(paramData[10]?.ToString(), out RecursiveLayer);
            else RecursiveLayer = false;

            // 11: DestroyWithHost
            if (paramData.Count > 11)
                bool.TryParse(paramData[11]?.ToString(), out DestroyWithHost);
            else DestroyWithHost = true;

            // 12: DestroyOnStop
            if (paramData.Count > 12)
                bool.TryParse(paramData[12]?.ToString(), out DestroyOnStop);
            else DestroyOnStop = false;

            // 13: DestroyDelay
            if (paramData.Count > 13)
                float.TryParse(paramData[13]?.ToString(), out DestroyDelay);
            else DestroyDelay = 0f;

            // 14: AutoPlayParticle
            if (paramData.Count > 14)
                bool.TryParse(paramData[14]?.ToString(), out AutoPlayParticle);
            else AutoPlayParticle = true;

            // 15: StopParticleOnDeactivate
            if (paramData.Count > 15)
                bool.TryParse(paramData[15]?.ToString(), out StopParticleOnDeactivate);
            else StopParticleOnDeactivate = true;

            // 16: ParticleStopAction
            if (paramData.Count > 16)
            {
                if (int.TryParse(paramData[16]?.ToString(), out int actionIndex) && actionIndex >= 0 && actionIndex <= 2)
                    ParticleStopAction = (ParticleSystemStopAction)actionIndex;
            }
            else ParticleStopAction = ParticleSystemStopAction.None;
        }

        public List<object> EncodeExcelData()
        {
            return new List<object>
            {
                string.IsNullOrEmpty(PrefabPath) ? XParamDefault.DefaultString : PrefabPath,
                string.IsNullOrEmpty(MountPointPath) ? XParamDefault.DefaultString : MountPointPath,
                FollowHost,
                $"{LocalPosition.x};{LocalPosition.y};{LocalPosition.z}",
                $"{LocalRotation.x};{LocalRotation.y};{LocalRotation.z}",
                $"{LocalScale.x};{LocalScale.y};{LocalScale.z}",
                UseWorldSpace,
                Layer,
                SortingOrder,
                string.IsNullOrEmpty(SortingLayerName) ? XParamDefault.DefaultString : SortingLayerName,
                RecursiveLayer,
                DestroyWithHost,
                DestroyOnStop,
                DestroyDelay,
                AutoPlayParticle,
                StopParticleOnDeactivate,
                (int)ParticleStopAction
            };
        }

        private void SetDefaults()
        {
            PrefabPath = string.Empty;
            MountPointPath = string.Empty;
            FollowHost = true;
            LocalPosition = Vector3.zero;
            LocalRotation = Vector3.zero;
            LocalScale = Vector3.one;
            UseWorldSpace = false;
            Layer = -1;
            SortingOrder = 0;
            SortingLayerName = string.Empty;
            RecursiveLayer = false;
            DestroyWithHost = true;
            DestroyOnStop = false;
            DestroyDelay = 0f;
            AutoPlayParticle = true;
            StopParticleOnDeactivate = true;
            ParticleStopAction = ParticleSystemStopAction.None;
        }

        private static Vector3 ParseVector3(string data, Vector3 defaultVal = default)
        {
            if (string.IsNullOrEmpty(data) || data == XParamDefault.DefaultString)
                return defaultVal;

            var parts = data.Split(';');
            if (parts.Length != 3 ||
                !float.TryParse(parts[0], out var x) ||
                !float.TryParse(parts[1], out var y) ||
                !float.TryParse(parts[2], out var z))
                return defaultVal;

            return new Vector3(x, y, z);
        }
#endif

        #endregion
    }
}