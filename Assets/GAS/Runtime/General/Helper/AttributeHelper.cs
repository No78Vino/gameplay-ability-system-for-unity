using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GAS.Runtime
{
    public static class AttributeHelper
    {
        private static EntityManager _entityManager => GASManager.EntityManager;
        
        [BurstCompile]
        public static int IndexOfAttrCode(this NativeArray<CAttributeData> attrs, int attrCode)
        {
            for (var i = 0; i < attrs.Length; i++)
                if (attrs[i].Code == attrCode)
                    return i;
            return -1;
        }
        
        public static float RecalculateCurrentValue(Entity asc,int attrSetCode,int attrCode)
        {
            // 获取属性集
            var attrSets = _entityManager.GetBuffer<BEAttrSet>(asc);
            var attrSetIndex = attrSets.IndexOfAttrSetCode(attrSetCode);
            if (attrSetIndex == -1) return 0;
            var attrSet = attrSets[attrSetIndex];
            
            // 获取属性
            var attributes = attrSet.Attributes;
            var attrIndex = attributes.IndexOfAttrCode(attrCode);
            if (attrIndex == -1) return 0;
            var attr = attributes[attrIndex];
            
            var oldValue = attr.CurrentValue;
            attr.CurrentValue = attr.BaseValue;
            
            // 获取GE
            var gameplayEffects = _entityManager.GetBuffer<BGameplayEffect>(asc);
            foreach (var buffer in gameplayEffects)
            {
                var ge = buffer.GameplayEffect;
                // 未激活的GE不计算
                var cDuration = _entityManager.GetComponentData<CDuration>(ge);
                if(!cDuration.active) continue;
                // 获取GE的属性修改器
                bool hasMods = _entityManager.HasComponent<MCModifiers>(ge);
                if (!hasMods) continue;
                var mods = _entityManager.GetComponentData<MCModifiers>(ge);
                foreach (var mod in mods.Modifiers)
                {
                    if (mod.AttrSetCode != attrSetCode || mod.AttrCode != attrCode) continue;
                    attr.CurrentValue = MmcHelper.Calculate(ge, mod, attr.CurrentValue);
                    // 钳制计算处理
                    if (attr.IsClampMin) attr.CurrentValue = math.max(attr.CurrentValue, attr.MinValue);
                    if (attr.IsClampMax) attr.CurrentValue = math.min(attr.CurrentValue, attr.MaxValue);
                }
            }

            // 结算钳制计算处理
            if (attr.IsClampMin) attr.CurrentValue = math.max(attr.CurrentValue, attr.MinValue);
            if (attr.IsClampMax) attr.CurrentValue = math.min(attr.CurrentValue, attr.MaxValue);
            
            attr.Dirty = false;
            var newCurrentValue = attr.CurrentValue;
            attrSet.Attributes[attrIndex] = attr;
            attrSets[attrSetIndex] = attrSet;
            
            GASEventCenter.InvokeOnCurrentValueChangeAfter(
                asc, attrSet.Code, attr.Code, oldValue, newCurrentValue);
            return newCurrentValue;
        }

        [BurstCompile]
        public static void MakeCurrentValueDirtyForRecalculate(Entity asc, int attrSetCode, int attrCode)
        {
            
        }
    }
}