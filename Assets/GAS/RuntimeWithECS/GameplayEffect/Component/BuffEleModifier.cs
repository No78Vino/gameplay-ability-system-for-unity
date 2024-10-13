using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.Modifier;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct BuffEleModifier : IBufferElementData
    {
        public int AttrSetCode;
        public int AttrCode;
        public GEOperation Operation;
        public float Magnitude;
        public MMCSetting MMC;
    }
    
    public sealed class ConfModifiers:GameplayEffectComponentConfig
    {
        public ModifierSetting[] modifierSettings;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            if(!_entityManager.HasBuffer<BuffEleModifier>(ge))
                _entityManager.AddBuffer<BuffEleModifier>(ge);

            var buffer = _entityManager.GetBuffer<BuffEleModifier>(ge);
            foreach (var modifierSetting in modifierSettings)
            {
                var stringParams = modifierSetting.MMC.stringParams == null
                    ? Array.Empty<FixedString32Bytes>()
                    : new FixedString32Bytes[modifierSetting.MMC.stringParams.Length];
                
                if (modifierSetting.MMC.stringParams != null)
                    for (int i = 0; i < modifierSetting.MMC.stringParams.Length; i++)
                        stringParams[i] = modifierSetting.MMC.stringParams[i];

                var floatParams = modifierSetting.MMC.floatParams ?? Array.Empty<float>();
                var intParams = modifierSetting.MMC.intParams ?? Array.Empty<int>();
                buffer.Add(new BuffEleModifier
                {
                    AttrSetCode = modifierSetting.AttrSetCode,
                    AttrCode = modifierSetting.AttrCode,
                    Operation = modifierSetting.Operation,
                    Magnitude = modifierSetting.Magnitude,
                    MMC = new MMCSetting
                    {
                        TypeCode = modifierSetting.MMC.TypeCode,
                        floatParams = new NativeArray<float>(floatParams,Allocator.Persistent),
                        intParams = new NativeArray<int>(intParams,Allocator.Persistent),
                        stringParams = new NativeArray<FixedString32Bytes>(stringParams,Allocator.Persistent)
                    }
                });
            }
        }
    }

    [Serializable]
    public struct ModifierSetting
    {
        public int AttrSetCode;
        public int AttrCode;
        public GEOperation Operation;
        public float Magnitude;
        public MMCSettingConfig MMC;
    }
}