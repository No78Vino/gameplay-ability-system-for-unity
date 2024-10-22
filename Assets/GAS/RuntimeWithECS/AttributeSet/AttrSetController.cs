using System.Collections.Generic;
using GAS.RuntimeWithECS.Attribute;
using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AttributeSet
{
    public class AttrSetController
    {
        // 属性集code缓存，便于快速查找
        private readonly Dictionary<int, int> _attrSetCodeIndexMap = new();
        private readonly List<int> _attrSetCodeList = new();

        public AttrSetController(Entity entity)
        {
            Entity = entity;
            EntityManager.AddBuffer<AttributeSetBufferElement>(Entity);
        }

        public Entity Entity { get; }

        public EntityManager EntityManager => GASManager.EntityManager;

        private AttributeData GetAttributeData(int attrSetCode, int attrCode)
        {
            // 若不存在，则直接返回NULL
            if (!_attrSetCodeList.Contains(attrSetCode)) return AttributeData.NULL;
            var attrSetIndex = _attrSetCodeIndexMap[attrSetCode];
            var attrBuffer = EntityManager.GetBuffer<AttributeSetBufferElement>(Entity);
            var attrSetCom = attrBuffer[attrSetIndex];

            var attrIndex = attrSetCom.GetAttrIndexByCode(attrCode);
            return attrIndex >= 0 ? attrSetCom.Attributes[attrIndex] : AttributeData.NULL;
        }

        public bool AddAttrSet(NewAttributeSetConfig config)
        {
            var attrSetCode = config.Code;
            if (_attrSetCodeList.Contains(attrSetCode)) return false;
            _attrSetCodeList.Add(attrSetCode);
            _attrSetCodeIndexMap.Add(attrSetCode, _attrSetCodeList.Count - 1);
            // 添加属性集数据
            var attrBuffer = EntityManager.GetBuffer<AttributeSetBufferElement>(Entity);
            var newAttrs = new AttributeData[config.Settings.Length];
            for (var i = 0; i < config.Settings.Length; i++)
            {
                var setting = config.Settings[i];
                newAttrs[i] = new AttributeData
                {
                    Code = setting.Code,
                    BaseValue = setting.InitValue,
                    CurrentValue = setting.InitValue,
                    MinValue = setting.Min,
                    MaxValue = setting.Max
                };
            }

            attrBuffer.Add(new AttributeSetBufferElement
            {
                Code = attrSetCode,
                Attributes = new NativeArray<AttributeData>(newAttrs, Allocator.Persistent)
            });
            return true;
        }

        public float GetBaseValue(int attrSetCode, int attrCode)
        {
            var com = GetAttributeData(attrSetCode, attrCode);
            return com.BaseValue;
        }

        public float GetCurrentValue(int attrSetCode, int attrCode)
        {
            var com = GetAttributeData(attrSetCode, attrCode);
            return com.CurrentValue;
        }

        private void ChangeBaseValue(int attrSetCode, int attrCode, float value, bool triggerEvent)
        {
            if (!_attrSetCodeList.Contains(attrSetCode)) return;
            var attrSetIndex = _attrSetCodeIndexMap[attrSetCode];
            
            var attrBuffer = EntityManager.GetBuffer<AttributeSetBufferElement>(Entity);
            var attrSet = attrBuffer[attrSetIndex];
            var attrIndex = attrSet.GetAttrIndexByCode(attrCode);
            
            var data = attrSet.Attributes[attrIndex];
            data.BaseValue = value;
            data.TriggerCueEvent = triggerEvent;
            // TODO: 重新计算current value
            // 注意：这里只是添加了TagAttributeDirty，设置了Attribute的dirty为true，真正的重计算完成是在RecalculateCurrentValueSystem中。
            // 【RecalculateCurrentValueSystem会有重计算完成的广播，广播会告知哪些实例的哪些属性重计算的新值。ECS本质是一帧内立即响应，而不是延迟一帧执行。】
            // 如果是需要初始化BaseValue之后，在接下来的逻辑中立即使用CurrentValue，你还需要额外调用RecalculateCurrentValueImmediately()
            data.Dirty = true;
            EntityManager.AddComponent<ComAttributeDirty>(Entity);
            
            attrSet.Attributes[attrIndex] = data;
            attrBuffer[attrSetIndex] = attrSet;
        }

        /// <summary>
        ///     初始化基础值
        ///     【不会触发任何事件，除了重计算current value】
        /// </summary>
        /// <param name="attrSetCode"></param>
        /// <param name="attrCode"></param>
        /// <param name="value"></param>
        public void InitBaseValue(int attrSetCode, int attrCode, float value)
        {
            ChangeBaseValue(attrSetCode, attrCode, value, false);
        }

        /// <summary>
        ///     设置基础值
        ///     【会触发值变化的对应各种事件】
        /// </summary>
        /// <param name="attrSetCode"></param>
        /// <param name="attrCode"></param>
        /// <param name="value"></param>
        public void SetBaseValue(int attrSetCode, int attrCode, float value)
        {
            ChangeBaseValue(attrSetCode, attrCode, value, true);
        }
    }
}