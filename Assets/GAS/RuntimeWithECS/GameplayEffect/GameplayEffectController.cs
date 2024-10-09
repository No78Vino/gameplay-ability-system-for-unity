﻿using GAS.RuntimeWithECS.AbilitySystemCell;
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
        private EntityManager EntityManager => GASManager.EntityManager;

        private void AddGameplayEffectEntityTo(Entity gameplayEffect, Entity target)
        {
            var geBuffers = EntityManager.GetBuffer<GameplayEffectBufferElement>(target);
            geBuffers.Add(new GameplayEffectBufferElement { GameplayEffect = gameplayEffect });
        }
        
        public NewGameplayEffectSpec ApplyGameplayEffectTo(NewGameplayEffectSpec gameplayEffect, AbilitySystemCellBase target)
        {
            AddGameplayEffectEntityTo(gameplayEffect.Entity, target.Entity);
            return gameplayEffect;
        }
    }
}