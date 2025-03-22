using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public class MCAbilityLogic : IComponentData
    {
        public int AbilityLogicCode;
        public AbilityLogicBase Logic;
        
        public MCAbilityLogic(int abilityLogicCode,AbilityLogicBase logic)
        {
            AbilityLogicCode = abilityLogicCode;
            Logic = logic;
        }
        
        public MCAbilityLogic()
        {
        }
    }
    
    public sealed class MCConfAbilityLogic:GameplayAbilityComponentConfig
    {
        public int AbilityLogicCode;
        public AbilityParamBase abilityParam;
        private AbilityLogicBase GetAbilityLogic(Entity ability) => AbilityHelper.TryCreateAbilityLogic(AbilityLogicCode,ability);
        
        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            var logic = GetAbilityLogic(ability);
            logic.SetParam(abilityParam);
            _entityManager.AddComponentData(ability, new MCAbilityLogic(AbilityLogicCode,logic));
        }
    }
}