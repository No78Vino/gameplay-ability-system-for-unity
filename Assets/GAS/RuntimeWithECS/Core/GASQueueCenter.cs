using System.Collections.Generic;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Core
{
    public static class GasQueueCenter
    {
        #region 应用GE队列
        
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
        
        #endregion
        
        #region BaseValue 更新队列
        
        private static readonly Queue<AttrBaseValueUpdateInfo> _baseValueUpdateInfos = new();

        public static Queue<AttrBaseValueUpdateInfo> BaseValueUpdateInfos()
        {
            return _baseValueUpdateInfos;
        }
        public static void AddBaseValueUpdateInfo(Entity asc, int attrSetCodee, int attrCode, float value)
        {
            _baseValueUpdateInfos.Enqueue(new AttrBaseValueUpdateInfo(asc, attrSetCodee, attrCode, value));
        }

        public static void ClearBaseValueUpdateInfos()
        {
            _baseValueUpdateInfos.Clear();
        }
        
        #endregion
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
    
    public struct AttrBaseValueUpdateInfo
    {
        public readonly Entity ASC;
        public readonly int AttrSetCodee;
        public readonly int AttrCode;
        public readonly float Value;
        
        public AttrBaseValueUpdateInfo(Entity asc, int attrSetCodee, int attrCode, float value)
        {
            ASC = asc;
            AttrSetCodee = attrSetCodee;
            AttrCode = attrCode;
            Value = value;
        }
    }
}