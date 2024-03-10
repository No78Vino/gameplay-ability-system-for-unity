using System;
using System.Linq;
using GAS.Editor.Ability;
using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.General.Util;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.TimelineAbility;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;
using UnityEngine;
using UnityEngine.UIElements;

namespace Demo.Script.GAS.AbilityTask
{
    [Serializable]
    public class DefendableAttack:InstantAbilityTask
    {
        public EffectCenterType CenterType;
        public Vector2 Size;
        public Vector2 Offset;
        public float Angle;
        
        public GameplayEffectAsset DefendedDamageEffect;
        public GameplayEffectAsset PerfectDefendEffect;
        public GameplayEffectAsset DirectDamageEffect;
        
        bool Defended(FightUnit target,Collider2D[] defendAreas)
        {
            if(!target.ASC.HasTag(GameplayTagSumCollection.Event_Defending)) return false;
            
            return defendAreas.Any(defendArea => target.DefendArea == defendArea);
        }

        bool IsPerfectDefended(FightUnit target)
        {
            return target.ASC.HasTag(GameplayTagSumCollection.Event_PerfectDefending);
        }
        
        public override void OnExecute()
        {
            var relativeTransform = _spec.Owner.GetComponent<FightUnit>().Renderer.transform;
            var targets = ((TimelineAbilitySpec)_spec).TimelineAbilityOverlapBox2D(Offset, Size, Angle, LayerMask.GetMask("FightUnit"), CenterType,relativeTransform);
            var defendAreas = ((TimelineAbilitySpec)_spec).TimelineAbilityOverlapBox2D(Offset, Size, Angle, LayerMask.GetMask("DefendArea"), CenterType,relativeTransform);

            var owner = _spec.Owner;
            foreach (var target in targets)
            {
                var targetUnit = target.GetComponent<FightUnit>();
                if (targetUnit)
                {
                    var defended = Defended(targetUnit, defendAreas);
                    if (defended)
                    {
                        bool isPerfectDefended = IsPerfectDefended(targetUnit);
                        if (isPerfectDefended)
                        {
                            var effect = new GameplayEffect(PerfectDefendEffect);
                            targetUnit.ASC.ApplyGameplayEffectTo(effect, owner);
                        }
                        else
                        {
                            var effect = new GameplayEffect(DefendedDamageEffect);
                            owner.ApplyGameplayEffectTo(effect, targetUnit.ASC);
                        }
            
                    }
                    else
                    {
                        var effect = new GameplayEffect(DirectDamageEffect);
                        owner.ApplyGameplayEffectTo(effect, targetUnit.ASC);
                    }
                }
            }
        }
    }

    public class DefendableAttackInspector : InstantAbilityTaskInspector<DefendableAttack>
    {
        public DefendableAttackInspector(AbilityTaskBase taskBase) : base(taskBase)
        {
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateSonInspector(false);
            
            var label = TrackInspectorUtil.CreateLabel("Defendable Attack");
            inspector.Add(label);
            
            var centerType = TrackInspectorUtil.CreateEnumField("CenterType", _task.CenterType, (evt) =>
            {
                _task.CenterType = (EffectCenterType)(evt.newValue);
                Save();
            });
            inspector.Add(centerType);

            var size = TrackInspectorUtil.CreateVector2Field("Size", _task.Size,
                (oldValue,newValue) =>
                {
                    _task.Size = newValue;
                    Save();
                });
            inspector.Add(size);

            var offset = TrackInspectorUtil.CreateVector2Field("Offset", _task.Offset,
                (oldValue,newValue) =>
                {
                    _task.Offset = newValue;
                    Save();
                });
            inspector.Add(offset);
            
            var angle = TrackInspectorUtil.CreateFloatField("Angle", _task.Angle, (evt) =>
            {
                _task.Angle = evt.newValue;
                Save();
            });
            inspector.Add(angle);
            
            var defendedDamageEffect = TrackInspectorUtil.CreateObjectField("DefendedDamageEffect",typeof(GameplayEffectAsset), _task.DefendedDamageEffect, (evt) =>
            {
                _task.DefendedDamageEffect = evt.newValue as GameplayEffectAsset;
                Save();
            });
            inspector.Add(defendedDamageEffect);
            
            var perfectDefendEffect = TrackInspectorUtil.CreateObjectField("PerfectDefendEffect",typeof(GameplayEffectAsset), _task.PerfectDefendEffect, (evt) =>
            {
                _task.PerfectDefendEffect = evt.newValue as GameplayEffectAsset;
                Save();
            });
            inspector.Add(perfectDefendEffect);
            
            var directDamageEffect = TrackInspectorUtil.CreateObjectField("DirectDamageEffect",typeof(GameplayEffectAsset), _task.DirectDamageEffect, (evt) =>
            {
                _task.DirectDamageEffect = evt.newValue as GameplayEffectAsset;
                Save();
            });
            inspector.Add(directDamageEffect);
            
            return inspector;
        }

        public override void OnEditorPreview()
        {
            // 使用Debug 绘制box预览
            float showTime = 1;
            Color color = Color.green;
            var relativeTransform = AbilityTimelineEditorWindow.Instance.PreviewObject.GetComponent<FightUnit>().Renderer.transform;
            var center = _task.Offset;
            var size = _task.Size;
            var angle = _task.Angle + relativeTransform.eulerAngles.z;
            switch (_task.CenterType)
            {
                case EffectCenterType.SelfOffset:
                    center = relativeTransform.position;
                    center.y += relativeTransform.lossyScale.y > 0 ? _task.Offset.y : -_task.Offset.y;
                    center.x += relativeTransform.lossyScale.x > 0 ? _task.Offset.x : -_task.Offset.x;
                    break;
                case EffectCenterType.WorldSpace:
                    center = (Vector2)_task.Offset;
                    break;
                case EffectCenterType.TargetOffset:
                    //center = _spec.Target.transform.position + (Vector3)_task.Offset;
                    break;
            }
            DebugExtension.DebugBox(center, size, angle, color, showTime);
        }
    }
}