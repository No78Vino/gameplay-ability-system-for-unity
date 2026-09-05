using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXProceduralMachine.Editor
{
    /// <summary>工具箱条目分类（三层体系：工具 / 预制菜 / 玩具）</summary>
    public enum ToolboxCategory
    {
        /// <summary>基础工具：组件系统，点一下创建空物体并挂载</summary>
        Tool,
        /// <summary>预制菜：可直接实例化的模板预制体（带默认参数 + 可选预设）</summary>
        Kit,
        /// <summary>小玩具：与 gameplay 挂钩的小交互系统（预留分类）</summary>
        Toy
    }

    /// <summary>家族标签（对齐调参家族，便于按"需求"筛选）</summary>
    public enum ToolboxFamily
    {
        /// <summary>跟随/惯性（次级运动 Position、无人机、相机）</summary>
        Follow,
        /// <summary>摆动/弹性（尾巴、披风、挂件）</summary>
        Sway,
        /// <summary>回弹/冲击（后坐、震屏、姿态回弹）</summary>
        Bounce,
        /// <summary>步态/移动（蜘蛛、四足、IK）</summary>
        Locomotion,
        /// <summary>呼吸/节奏（待机动画、周期驱动）</summary>
        Breath,
        /// <summary>调试/可视化辅助</summary>
        Debug
    }

    /// <summary>参数覆写：实例化后按"组件路径+类型+字段"覆写一个参数</summary>
    [Serializable]
    public class ToolboxParamOverride
    {
        /// <summary>相对根节点的 Transform 路径，空字符串=根节点（如 "Body"、"Body/Rotor_FL"）</summary>
        public string componentPath = "";
        /// <summary>组件类型名（短名或全名均可，如 "DroneFlyDriver"）</summary>
        public string componentType = "";
        /// <summary>要覆写的字段名</summary>
        public string field = "";
        /// <summary>字段值（字符串，按字段类型解析：float/int/bool/Vector3/enum/string）</summary>
        public string value = "";
    }

    /// <summary>预设：一组参数覆写的命名集合（可一键应用到实例）</summary>
    [Serializable]
    public class ToolboxPreset
    {
        public string id = "";
        public string name = "";
        public string description = "";
        public List<ToolboxParamOverride> overrides = new List<ToolboxParamOverride>();
    }

    /// <summary>工具箱条目（Unity 侧内部模型）</summary>
    [Serializable]
    public class ToolboxItem
    {
        public string id = "";
        public string name = "";
        public ToolboxCategory category = ToolboxCategory.Kit;
        public ToolboxFamily family = ToolboxFamily.Follow;
        public string description = "";
        /// <summary>Kit/Toy：预制体路径（相对 Assets）</summary>
        public string prefabPath = "";
        /// <summary>Tool：组件类型全名（如 EXProceduralMachine.SecondOrderDynamicsComponent）</summary>
        public string componentType = "";
        public List<ToolboxPreset> presets = new List<ToolboxPreset>();

        public GameObject LoadPrefab()
        {
            if (string.IsNullOrEmpty(prefabPath)) return null;
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
#else
            return null;
#endif
        }
    }

    /// <summary>工具箱清单（Unity 侧内部模型，可导出/导入 JSON）</summary>
    [Serializable]
    public class ToolboxManifest
    {
        public int version = 1;
        public string module = "EXProceduralMachine";
        public List<ToolboxItem> items = new List<ToolboxItem>();

        public ToolboxItem FindById(string id)
        {
            return items.Find(i => i.id == id);
        }
    }

    // ==================== 导出 DTO（网页端友好：分类/家族为字符串，零解析成本） ====================

    [Serializable]
    public class ToolboxItemDto
    {
        public string id;
        public string name;
        public string category;
        public string family;
        public string description;
        public string prefabPath;
        public string componentType;
        public List<ToolboxPresetDto> presets;
    }

    [Serializable]
    public class ToolboxPresetDto
    {
        public string id;
        public string name;
        public string description;
        public List<ToolboxOverrideDto> overrides;
    }

    [Serializable]
    public class ToolboxOverrideDto
    {
        public string componentPath;
        public string componentType;
        public string field;
        public string value;
    }

    [Serializable]
    public class ToolboxManifestDto
    {
        public int version;
        public string module;
        public List<ToolboxItemDto> items;
    }
}