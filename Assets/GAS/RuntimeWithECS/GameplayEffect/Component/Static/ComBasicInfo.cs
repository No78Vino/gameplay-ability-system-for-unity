using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComBasicInfo : IComponentData
    {
        /// <summary>
        /// 仅调试显示用，不建议作为运算逻辑的依据
        /// </summary>
        public FixedString32Bytes name; 
        
        // -------------------------------------以下是RUNTIME数据，不需要初始化---------------------------------------//

    }
    
    public sealed class ConfBasicInfo:GameplayEffectComponentConfig
    {
        public string Name;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            _entityManager.SetName(ge, $"GE_{Name}_V{ge.Version}_{ge.Index}");
            _entityManager.AddComponentData(ge, new ComBasicInfo
            {
                name = Name
            });
        }
    }
}