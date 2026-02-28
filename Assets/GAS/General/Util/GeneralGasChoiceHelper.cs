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
            ReflectionHelper.InvokeStaticMethod("GAS.Runtime.XLuban", "LoadTablesForEditor");
            ReflectionHelper.InvokeStaticMethod("GAS.Runtime.XAbility", "LoadAbilityCode");
            _tags?.Clear();
            _cues?.Clear();
            _mmcs?.Clear();
            _effects?.Clear();
            _abilities?.Clear();
            _ascs?.Clear();
            _attrSets?.Clear();
            _attrs?.Clear();
            _timelineAbilityIDs?.Clear();
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
        private static List<ValueDropdownItem> _timelineAbilityIDs;

        public static List<ValueDropdownItem> Tags()
        {
            if (_tags is { Count: > 0 }) return _tags;
            
            _tags = new List<ValueDropdownItem>();
            var tagIDs = GasChoiceRawAccessor.GetGameplayTagsKeysToList();
            foreach (var tagID in tagIDs)
            {
                string tagName = GasChoiceRawAccessor.GetGameplayTagName(tagID);
                _tags.Add(new ValueDropdownItem(tagName,tagID));
            }

            return _tags;
        }

        public static List<ValueDropdownItem> TimelineAbilityIDs()
        {
            if (_timelineAbilityIDs is { Count: > 0 }) return _timelineAbilityIDs;
            
            _timelineAbilityIDs = new List<ValueDropdownItem>();
            var ids = GasChoiceRawAccessor.GetTimelineAbilityIDs();
            foreach (var id in ids)
            {
                string name = GasChoiceRawAccessor.GetTimelineAbilityName(id);
                _timelineAbilityIDs.Add(new ValueDropdownItem($"[{id}]{name}",id));
            }

            return _timelineAbilityIDs;
        }

        public static List<ValueDropdownItem> GameplayEffects()
        {
            if (_effects is { Count: > 0 }) return _effects;
            
            _effects = new List<ValueDropdownItem>();
            
            var ids = GasChoiceRawAccessor.GetGameplayEffectIDs();
            foreach (var id in ids)
            {
                string name = GasChoiceRawAccessor.GetGameplayEffectName(id);
                _effects.Add(new ValueDropdownItem($"[{id}]{name}",id));
            }

            return _effects;
        }

        public static List<ValueDropdownItem> GameplayCues()
        {
            if (_cues is { Count: > 0 }) return _cues;
            
            _cues = new List<ValueDropdownItem>();
            
            var ids = GasChoiceRawAccessor.GetGameplayCueIDs();
            foreach (var id in ids)
            {
                string name = GasChoiceRawAccessor.GetGameplayCueName(id);
                _cues.Add(new ValueDropdownItem($"[{id}]{name}",id));
            }
            
            return _cues;
        }
     
        public static List<ValueDropdownItem> AttrSets()  
        {  
            if (_attrSets is { Count: > 0 }) return _attrSets;  
  
            _attrSets = new List<ValueDropdownItem>();  
  
            var ids = GasChoiceRawAccessor.GetAttributeSetIDs();  
            if (ids == null) return _attrSets;  
            foreach (var id in ids)  
            {  
                string name = GasChoiceRawAccessor.GetAttrSetNameByCode(id);  
                _attrSets.Add(new ValueDropdownItem($"[{id}]{name}", id));  
            }  
  
            return _attrSets;  
        }  
  
        public static List<ValueDropdownItem> Attrs(int attrSetCode)  
        {  
            _attrs ??= new Dictionary<int, List<ValueDropdownItem>>();  
            if (_attrs.TryGetValue(attrSetCode, out var cached) && cached.Count > 0) return cached;  
  
            var list = new List<ValueDropdownItem>();  
            _attrs[attrSetCode] = list;  
  
            // 1. 获取 TbattributeSet 行对象（整行，不取某个字段）  
            object tablesObj = ReflectionHelper.GetStaticFieldOrProperty("GAS.Runtime.XLuban", "Tables");  
            if (tablesObj == null) return list;  
  
            var tablesType = tablesObj.GetType();  
            var tableProp = tablesType.GetProperty("TbattributeSet",  
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);  
            if (tableProp == null) return list;  
  
            var tableObj = tableProp.GetValue(tablesObj);  
            if (tableObj == null) return list;  
  
            // 2. 调用 Get(attrSetCode) 拿到行对象  
            object rowObj = ReflectionHelper.InvokeInstanceMethod(tableObj, "Get", attrSetCode);  
            if (rowObj == null) return list;  
  
            // 3. 取 Attribute 字段（复合对象数组，字段名是 "Attribute" 单数）  
            var attrArrayObj = ReflectionHelper.GetProperty<object>(rowObj, "Attribute");  
            if (attrArrayObj == null) return list;  
  
            // 4. 遍历数组，逐元素取 ID 和 Name  
            var attrArray = attrArrayObj as System.Array;  
            if (attrArray == null) return list;  
  
            foreach (var attrObj in attrArray)  
            {  
                var id = ReflectionHelper.GetProperty<int>(attrObj, "ID");  
                string name = GasChoiceRawAccessor.GetAttributeNameByCode(id);  
                list.Add(new ValueDropdownItem($"[{id}]{name}", id));  
            }  
  
            return list;  
        }
    }
}