using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComBasicInfo : IComponentData
    {
        public FixedString32Bytes name; // 仅调试显示用，不建议作为运算逻辑的依据
    }
    
    public sealed class ConfBasicInfo:GameplayEffectComponentConfig
    {
        public string Name;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            _entityManager.SetName(ge, "GE_" + Name);
            _entityManager.AddComponentData(ge, new ComBasicInfo
            {
                name = Name
            });
        }
    }
}