using System.Collections.Generic;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Core
{
    public static class GasQueueCenter
    {
        private static readonly Queue<EffectWaitingForApplication> _effectsWaitingForApplication = new();

        public static Queue<EffectWaitingForApplication> EffectsWaitingForApplication()
        {
            return _effectsWaitingForApplication;
        }
        public static void AddEffectWaitingForApplication(Entity gameplayEffect, Entity sourceAsc, Entity targetAsc)
        {
            _effectsWaitingForApplication.Enqueue(new EffectWaitingForApplication(gameplayEffect, sourceAsc, targetAsc));
        }

        public static void ClearEffectsWaitingForApplication()
        {
            _effectsWaitingForApplication.Clear();
        }
    }

    public struct EffectWaitingForApplication
    {
        public readonly Entity GameplayEffect;
        public readonly Entity SourceAsc;
        public readonly Entity TargetAsc;

        public EffectWaitingForApplication(Entity gameplayEffect, Entity sourceASC, Entity targetASC)
        {
            GameplayEffect = gameplayEffect;
            SourceAsc = sourceASC;
            TargetAsc = targetASC;
        }
    }
}