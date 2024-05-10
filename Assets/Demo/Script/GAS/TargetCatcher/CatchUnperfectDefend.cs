﻿using System.Collections.Generic;
#if UNITY_EDITOR
using GAS.Editor;
#endif
using GAS.General;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Demo.Script.GAS.TargetCatcher
{
    public class CatchUnperfectDefend : CatchUndefending
    {
        /// <summary>
        /// 成功防御，但是没有完美防御
        /// </summary>
        /// <param name="mainTarget"></param>
        /// <returns></returns>
        protected override void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget,List<AbilitySystemComponent> results)
        {
            int count = centerType switch
            {
                EffectCenterType.SelfOffset => Owner.OverlapBox2DNonAlloc(offset, size, rotation, Collider2Ds,
                    checkLayer),
                EffectCenterType.WorldSpace => Physics2D.OverlapBoxNonAlloc(offset, size, rotation, Collider2Ds,
                    checkLayer),
                EffectCenterType.TargetOffset => mainTarget.OverlapBox2DNonAlloc(offset, size, rotation, Collider2Ds,
                    checkLayer),
                _ => 0
            };

            for (var i = 0; i < count; ++i)
            {
                var targetUnit = Collider2Ds[i].GetComponent<AbilitySystemComponent>();
                if (targetUnit != null)
                {
                    if (IsDefendSuccess(targetUnit) && !targetUnit.HasTag(GTagLib.Event_PerfectDefending))
                        results.Add(targetUnit);
                }
            }
        }
#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject obj)
        {
            // 使用Debug 绘制box预览
            float showTime = 1;
            var color = Color.green;
            var relativeTransform = AbilityTimelineEditorWindow.Instance.PreviewObject.transform;
            var center = offset;
            var angle = rotation + relativeTransform.eulerAngles.z;
            switch (centerType)
            {
                case EffectCenterType.SelfOffset:
                    center = relativeTransform.position;
                    center.y += relativeTransform.lossyScale.y > 0 ? offset.y : -offset.y;
                    center.x += relativeTransform.lossyScale.x > 0 ? offset.x : -offset.x;
                    break;
                case EffectCenterType.WorldSpace:
                    center = offset;
                    break;
                case EffectCenterType.TargetOffset:
                    //center = _spec.Target.transform.position + (Vector3)_task.Offset;
                    break;
            }

            DebugExtension.DebugBox(center, size, angle, color, showTime);
        }
#endif
    }
#if UNITY_EDITOR
    public class CatchUnperfectDefendInspector : TargetCatcherInspector<CatchUnperfectDefend>
    {
        [BoxGroup] [Delayed] [OnValueChanged("OnCatcherChanged")]
        public Vector2 Offset;
        
        [BoxGroup] [Delayed] [OnValueChanged("OnCatcherChanged")]
        public Vector2 Size;
        
        [BoxGroup] [Delayed] [LabelText("Rotation")] [OnValueChanged("OnCatcherChanged")]
        public float Rotation;
        
        [BoxGroup] [Delayed] [LabelText("Detect Layer")] [OnValueChanged("OnCatcherChanged")]
        public LayerMask Layer;
        
        [BoxGroup] [Delayed] [LabelText("Center Type")] [OnValueChanged("OnCatcherChanged")]
        public EffectCenterType CenterType;
        
        public CatchUnperfectDefendInspector(CatchUnperfectDefend targetCatcherBase) : base(targetCatcherBase)
        {
            Offset = targetCatcherBase.offset;
            Size = targetCatcherBase.size;
            Rotation = targetCatcherBase.rotation;
            Layer = targetCatcherBase.checkLayer;
            CenterType = targetCatcherBase.centerType;
        }
        
        public void OnCatcherChanged()
        {
            _targetCatcher.offset = Offset;
            _targetCatcher.size = Size;
            _targetCatcher.rotation = Rotation;
            _targetCatcher.checkLayer = Layer;
            _targetCatcher.centerType = CenterType;
            Save();
        }
    }
#endif
}