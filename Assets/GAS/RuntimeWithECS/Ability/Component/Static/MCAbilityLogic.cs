using GAS.RuntimeWithECS.ComponentConfig;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Static
{
    public class MCAbilityLogic : IComponentData
    {
        public AbilityLogicBase Logic;
        
        public MCAbilityLogic(AbilityLogicBase logic)
        {
            Logic = logic;
        }
        
        public MCAbilityLogic()
        {
        }
    }
    
    public sealed class MCConfAbilityLogic:GameplayAbilityComponentConfig
    {
        public string AbilityLogicType;
        public IAbilityParam abilityParam;
        private AbilityLogicBase GetAbilityLogic(Entity ability) => AbilityHelper.TryCreateAbilityLogic(AbilityLogicType,ability);
        
        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            var logic = GetAbilityLogic(ability);
            logic.SetParam(abilityParam);
            _entityManager.AddComponentData(ability, new MCAbilityLogic(logic));
        }
    }
}