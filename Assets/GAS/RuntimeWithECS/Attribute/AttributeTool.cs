using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Attribute
{
    public static class AttributeTool
    {
        private static EntityManager _entityManager => GASManager.EntityManager;
        public static float RecalculateCurrentValue(Entity asc,int attrSetCode,int attrCode)
        {
            // 获取属性集
            var attrSets = _entityManager.GetBuffer<BEAttributeSet>(asc);
            var attrSetIndex = attrSets.IndexOfAttrSetCode(attrSetCode);
            if (attrSetIndex == -1) return 0;
            var attrSet = attrSets[attrSetIndex];
            
            // 获取属性
            var attributes = attrSet.Attributes;
            var attrIndex = attributes.IndexOfAttrCode(attrCode);
            if (attrIndex == -1) return 0;
            var attr = attributes[attrIndex];
            

            attr.CurrentValue = attr.BaseValue;
            // 获取GE
            var gameplayEffects = _entityManager.GetBuffer<BEGameplayEffect>(asc);
            if(gameplayEffects.Length == 0) return attr.CurrentValue;
            
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
                }
            }

            var newCurrentValue = attr.CurrentValue;
            attrSet.Attributes[attrIndex] = attr;
            attrSets[attrSetIndex] = attrSet;
            return newCurrentValue;
        }
        
        
    }
}