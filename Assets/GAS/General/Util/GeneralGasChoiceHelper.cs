using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using System;

namespace GAS.General
{
    /// <summary>
    /// 该类只是用于编辑环境下的使用，所以相关函数的实现逻辑只在UNITY_EDITOR下生效
    /// 
    /// 兼容Editor和Runtime两边程序集的配置，Type，选项集合
    /// 不同于Editor的直接读取Excel配置，General的Choices依赖于已经生成好的luban配置文件
    /// 【注意】General下的choices更新都是在【自动化生成脚本】之后才更新
    /// </summary>
    public static class GeneralGasChoiceHelper
    {
        /// <summary>
        /// 加载当前最新的choices相关缓存
        /// </summary>
        public static void LoadCache()
        {
#if UNITY_EDITOR
            ReflectionHelper.InvokeStaticMethod("GAS.Runtime.XLauncher", "InitCache");
#endif
        }

        private static List<ValueDropdownItem> _tags;
        private static List<ValueDropdownItem> _cues;
        private static List<ValueDropdownItem> _effects;
        private static List<ValueDropdownItem> _mmcs;
        private static List<ValueDropdownItem> _abilities;
        private static List<ValueDropdownItem> _ascs;
        private static List<ValueDropdownItem> _attrSets;
        private static Dictionary<int,List<ValueDropdownItem>> _attrs;

        public static List<ValueDropdownItem> Tags()
        {
            if (_tags != null) return _tags;
            
            _tags = new List<ValueDropdownItem>();
            var tagIDs = GasChoiceRawAccessor.GetGameplayTagsKeysToList();
            foreach (var tagID in tagIDs)
            {
                string tagName = GasChoiceRawAccessor.GetGameplayTagName(tagID);
                _tags.Add(new ValueDropdownItem(tagName,tagID));
            }

            return _tags;
        }
    }
}