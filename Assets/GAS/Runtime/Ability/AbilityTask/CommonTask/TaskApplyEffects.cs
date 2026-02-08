using Unity.Entities;

namespace GAS.Runtime
{
    public class TaskApplyEffects: AbilityTaskBase<XParamApplyEffects>
    {
        public TaskApplyEffects(AbilityLogicBase logic) : base(logic)
        {
        }

        protected override void OnBegin(int startFrame)
        {
            base.OnBegin(startFrame);

            var target = Owner;
            var owner = Owner;
            foreach (var effectCode in Parameter.IDs)
            {
                var effectCfg = GameplayEffectHelper.GetConfigByID(effectCode);
                var geEntity = CreateGameplayEffectEntity(effectCfg);
                ApplyGameplayEffectTo(geEntity,target , owner);
            }
        }

        private Entity CreateGameplayEffectEntity(GameplayEffectConfig config)
        {
            return EffectUtil.CreateGameplayEffectEntity(config.ComponentConfigs);
        } 
        
        private void ApplyGameplayEffectTo(Entity gameplayEffect, AbilitySystemCell target, AbilitySystemCell source)
        {
            EffectUtil.ApplyGameplayEffectTo(gameplayEffect, target,source);
            EntityHelper.AddComponent<CCreatedByAbility>(gameplayEffect);
            EntityHelper.SetComponent(gameplayEffect,new CCreatedByAbility()
            {
                sourceAbility = Spec.AbilityEntity
            });
        }
    }
}