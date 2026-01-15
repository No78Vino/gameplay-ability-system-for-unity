using Unity.Entities;

namespace GAS.Runtime
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
    
    public sealed class MCConfAbilityLogic:AbilityComponentConfig
    {
        public string AbilityLogicType;
        public IExParameterBase ExParameterBase;
        private AbilityLogicBase GetAbilityLogic(Entity ability) => AbilityHelper.TryCreateAbilityLogic(AbilityLogicType,ability);
        
        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            var logic = GetAbilityLogic(ability);
            logic.SetParam(ExParameterBase);
            _entityManager.AddComponentData(ability, new MCAbilityLogic(logic));
        }
    }
}