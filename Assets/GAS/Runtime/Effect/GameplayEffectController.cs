using Unity.Entities;

namespace GAS.Runtime
{
    /// <summary>
    /// GE 控制器
    /// 所有ASC对GE的操作逻辑，都整合到这个类内。
    /// 他不是Container概念，因为ECS中GE已经是buffer element被存放在Entity上了，所以这个类不是容器。
    /// </summary>
    public class GameplayEffectController
    {
        private Entity _asc;
        private static EntityManager GasEntityManager => GASManager.EntityManager;

        public GameplayEffectController(Entity asc)
        {
            _asc = asc;
            GasEntityManager.AddBuffer<BGameplayEffect>(_asc);
        }
        
        public DynamicBuffer<BGameplayEffect> CurrentGameplayEffects =>
            GasEntityManager.GetBuffer<BGameplayEffect>(_asc);
        
        private void AddGameplayEffectEntityTo(Entity gameplayEffect, Entity target)
        {
            GameplayEffectHelper.ApplyGameplayEffectTo(gameplayEffect,target,_asc);
        }
        
        public GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffectSpec gameplayEffect, AbilitySystemCell target)
        {
            AddGameplayEffectEntityTo(gameplayEffect.Entity, target.Entity);
            return gameplayEffect;
        }
        
        public void RemoveGameplayEffect(Entity gameplayEffect)
        {
            GameplayEffectHelper.RemoveGameplayEffect(gameplayEffect);
        }

        public void ClearGameplayEffects()
        {
            foreach (var beEffect in CurrentGameplayEffects)
                RemoveGameplayEffect(beEffect.GameplayEffect);
        }
    }
}