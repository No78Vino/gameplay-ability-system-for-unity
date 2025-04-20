using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Aspect
{
    public readonly partial struct AspModifyBaseValue : IAspect
    {
        public readonly Entity self;
        private readonly RefRO<CEffectInUsage> _inUsage;
        private readonly RefRO<CEffectApplied> _comValidEffect;
        private readonly RefRO<CInApplicationProgress> _inApplicationProgress;
        //private readonly MCModifiers _modifiers;

        public Entity ASC => _inUsage.ValueRO.Target;
        
        // public bool ModifyBaseValue()
        // {
        //     // 排除掉Durational的GE类型
        //     var isDurational = GASManager.EntityManager.HasComponent<CDuration>(self);
        //     if (isDurational) return false;
        //
        //     var asc = _inUsage.ValueRO.Target;
        //     bool changed = false;
        //     var attrSets = GASManager.EntityManager.GetBuffer<BEAttributeSet>(asc);
        //     foreach (var mod in _modifiers.Modifiers)
        //     {
        //         var attrSetIndex = attrSets.IndexOfAttrSetCode(mod.AttrSetCode);
        //         if (attrSetIndex == -1) continue;
        //
        //         var attrSet = attrSets[attrSetIndex];
        //         var attributes = attrSet.Attributes;
        //
        //         var attrIndex = attributes.IndexOfAttrCode(mod.AttrCode);
        //         if (attrIndex == -1) continue;
        //
        //         var data = attributes[attrIndex];
        //         var oldValue = data.BaseValue;
        //         var newValue = MmcHelper.Calculate(self, mod, data.BaseValue);
        //
        //         // OnChangeBefore
        //         // BaseValue 不做钳制，因为Max，Min是只针对Current Value
        //         newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(asc, mod.AttrSetCode, mod.AttrCode, newValue);
        //
        //         data.BaseValue = newValue;
        //
        //         // OnChangeAfter
        //         if (newValue != oldValue)
        //         {
        //             // BaseValue 改变，需要标记Dirty
        //             data.Dirty = true;
        //             changed = true;
        //             GASEventCenter.InvokeOnBaseValueChangeAfter(asc, mod.AttrSetCode, mod.AttrCode, oldValue, newValue);
        //         }
        //
        //         attrSet.Attributes[attrIndex] = data;
        //         attrSets[attrSetIndex] = attrSet;
        //     }
        //
        //     return changed;
        // }
    }
}