using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
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
        public int AbilityLogicCode;
        private AbilityLogicBase Logic => AbilityHelper.TryCreateAbilityLogic(AbilityLogicCode);
        
        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new MCAbilityLogic(Logic));
        }
    }
}