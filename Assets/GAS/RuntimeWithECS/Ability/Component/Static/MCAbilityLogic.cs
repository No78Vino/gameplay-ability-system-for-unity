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
        /// <summary>
        ///  能力逻辑代码
        /// 【注意！】该代码和CAbilityBaseInfo的Code保持一致，自定义生成能力（或者Runtime生成）时需要特别注意
        /// </summary>
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