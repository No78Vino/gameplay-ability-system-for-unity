using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
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
            GasEntityManager.AddBuffer<GameplayEffectBufferElement>(_asc);
        }
        
        public DynamicBuffer<GameplayEffectBufferElement> CurrentGameplayEffects =>
            GasEntityManager.GetBuffer<GameplayEffectBufferElement>(_asc);
        
        private void AddGameplayEffectEntityTo(Entity gameplayEffect, Entity target)
        {
            GasEntityManager.AddComponent<ComNeedInit>(gameplayEffect);
            GasEntityManager.AddComponent<ComInUsage>(gameplayEffect);
            GasEntityManager.AddComponent<NeedCheckEffects>(target);
            var comInUsage = GasEntityManager.GetComponentData<ComInUsage>(gameplayEffect);
            comInUsage.Source = _asc;
            comInUsage.Target = target;
            GasEntityManager.SetComponentEnabled<ComInUsage>(gameplayEffect,true);
            GasEntityManager.SetComponentEnabled<NeedCheckEffects>(gameplayEffect,true);
            GasEntityManager.SetComponentData(gameplayEffect,comInUsage);
            
            var geBuffers = GameplayEffectUtils.GameplayEffectsOf(target);
            geBuffers.Add(new GameplayEffectBufferElement { GameplayEffect = gameplayEffect });
        }
        
        public NewGameplayEffectSpec ApplyGameplayEffectTo(NewGameplayEffectSpec gameplayEffect, AbilitySystemCell.AbilitySystemCell target)
        {
            AddGameplayEffectEntityTo(gameplayEffect.Entity, target.Entity);
            return gameplayEffect;
        }
    }
}