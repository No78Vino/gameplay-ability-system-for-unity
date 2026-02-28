using System;  
using Unity.Entities;  
using UnityEngine;  
  
namespace GAS.Runtime  
{  
    /// <summary>  
    /// 基于属性值的 MMC。公式：AttributeValue * K + B  
    /// - SnapShot：首次计算时缓存属性值，后续复用（快照存在 MMC 实例内）  
    /// - Track：每次实时读取；依赖属性 BaseValue 变化时自动触发目标属性重计算（推导属性）  
    /// </summary>  
    public sealed class MMCAttributeBased : ModMagnitudeCalculationBase<AttributeBasedMmcParam>  
    {  
        // SnapShot 缓存  
        private bool _snapshotCaptured;  
        private float _snapshotValue;  
  
        // Track 模式：目标属性（被修改的属性）所在的 ASC  
        private AbilitySystemCell _targetAsc;  
        private int _targetAttrSetCode;  
        private int _targetAttrCode;  
  
        // Track 模式：依赖属性（被监听的属性）所在的 ASC  
        private AbilitySystemCell _sourceAsc;  
        private Action<float, float> _trackCallback;  
  
        public override float CalculateMagnitude(MmcContext mmcContext, float magnitude)  
        {  
            var resolver = AttributeBasedMmcParam.GetResolver();  
            if (resolver == null)  
            {  
                Debug.LogError("[AttributeBasedMmc] IAttributeValueResolver 未注册！请在启动时调用 AttributeBasedMmcParam.RegisterResolver()。");  
                return magnitude;  
            }  
  
            float attrValue;  
            if (Parameter.CaptureType == AttributeCaptureType.SnapShot)  
            {  
                if (!_snapshotCaptured)  
                {  
                    _snapshotValue = ResolveAttributeValue(mmcContext.EffectSpec.Entity, resolver);  
                    _snapshotCaptured = true;  
                }  
                attrValue = _snapshotValue;  
            }  
            else // Track：每次实时读取  
            {  
                attrValue = ResolveAttributeValue(mmcContext.EffectSpec.Entity, resolver);  
            }  
  
            return attrValue * Parameter.K + Parameter.B;  
        }  
  
        /// <summary>  
        /// GE 被激活应用到目标 ASC 时调用。  
        /// Track 模式下注册依赖属性的 BaseValue 变化监听，实现推导属性自动重计算。  
        /// </summary>  
        protected override void OnAdded(MmcContext mmcContext, int targetAttrSetCode, int targetAttrCode)  
        {  
            if (Parameter.CaptureType != AttributeCaptureType.Track) return;  
            
            _targetAsc = mmcContext.Target;  
            if (_targetAsc == null) return;  
  
            _targetAttrSetCode = targetAttrSetCode;  
            _targetAttrCode = targetAttrCode;  
  
            // 被依赖属性所在的 ASC：  
            // Target 模式 → 依赖属性在目标自身上  
            // Source 模式 → 依赖属性在 GE 的施放者上（需要从 GE 上下文获取，此处由框架在 OnApplied 前设置好）  
            _sourceAsc = Parameter.FromType == AttributeFromType.Target  
                ? _targetAsc  
                : mmcContext.Source; // 框架侧辅助方法，返回 AbilitySystemCell  
  
            if (_sourceAsc == null) return;  
  
            // 注册监听：当被依赖属性的 BaseValue 变化后，触发目标属性重计算  
            _trackCallback = (_, _) =>  
                AttributeHelper.RecalculateCurrentValue(_targetAsc.Entity, _targetAttrSetCode, _targetAttrCode);  
  
            // GASEventCenter 提供了接受 AbilitySystemCell 的重载，无需暴露 Entity  
            GASEventCenter.RegisterOnBaseValueChangeAfter(  
                _sourceAsc, Parameter.AttrSetCode, Parameter.AttrCode, _trackCallback);  
        }  
  
        /// <summary>  
        /// GE 被移除时调用，注销 Track 监听，防止内存泄漏。  
        /// </summary>  
        protected override void OnRemoved()  
        {  
            if (_trackCallback == null) return;  
            GASEventCenter.UnRegisterOnBaseValueChangeAfter(  
                _sourceAsc, Parameter.AttrSetCode, Parameter.AttrCode, _trackCallback);  
            _trackCallback = null;  
        }  
  
        private float ResolveAttributeValue(Entity geEntity, IAttributeValueResolver resolver)  
        {  
            // 通过 GASManager 将 GE 上下文中的 Source/Target Entity 转换为 AbilitySystemCell  
            var ascEntity = Parameter.FromType == AttributeFromType.Source  
                ? ASCHelper.GetSourceAscEntity(geEntity)  
                : ASCHelper.GetTargetAscEntity(geEntity);  
  
            var asc = GASManager.GetAscFromEntity(ascEntity);
            if (asc != null) return resolver.Resolve(asc, Parameter.AttrSetCode, Parameter.AttrCode);
            Debug.LogWarning($"[AttributeBasedMmc] 无法找到 {Parameter.FromType} ASC。");  
            return 0f;
        }  
    }  
}