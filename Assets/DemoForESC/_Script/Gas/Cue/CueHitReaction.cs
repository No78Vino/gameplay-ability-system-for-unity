using GAS.Runtime;  
using Unity.Entities;  
using UnityEngine;  
  
namespace DemoForESC._Script.Gas.Cue  
{  
    /// <summary>  
    /// 受击反馈 CueLogic  
    ///   
    /// 效果：根据攻击来源和受击目标在XZ平面上的方向向量，让受击角色往后倒一下再恢复。  
    /// 参数：XParamFloat.Value = 受击反馈的持续时间（秒），如 0.3  
    ///   
    /// 使用场景：  
    ///   - 作为扣血 GE 的 CueOnApply（Instant GE）触发瞬时受击反馈  
    ///   - 也可在 Timeline 的 TaskPlayCue 中使用  
    ///   
    /// 实现原理：  
    ///   1. OnAdd 时缓存受击目标的 Transform  
    ///   2. OnActivate 时计算攻击方向（XZ平面），记录起始时间  
    ///   3. OnTick 中每帧根据时间插值驱动"后仰-恢复"的旋转动画  
    ///   4. 到时间后 RemoveSelf + KillSelf 自我清理  
    /// </summary>  
    public class CueHitReaction : GameplayCueBase<XParamFloat>  
    {  
        // 受击反馈最大后仰角度（度）  
        private const float MaxTiltAngle = 15f;  
  
        private Transform _targetTransform;  
        private Quaternion _originalRotation;  
        private Vector3 _hitDirXZ;  // 攻击方向（XZ平面归一化）  
        private float _startTime;  
        private float _duration;  
        private bool _isPlaying;  
  
        public override void OnAdd(float time)  
        {  
            base.OnAdd(time);  
  
            // 缓存受击目标的 Transform  
            var targetGo = _abilitySystemCell?.GameObject;  
            if (targetGo != null)  
                _targetTransform = targetGo.transform;  
        }  
  
        public override void OnActivate(float time)  
        {  
            base.OnActivate(time);  
  
            if (_targetTransform == null)  
            {  
                RemoveSelf();  
                KillSelf();  
                return;  
            }  
  
            _duration = Parameter != null ? Parameter.Value : 0.3f;  
            if (_duration <= 0f) _duration = 0.3f;  
  
            _startTime = time;  
            _originalRotation = _targetTransform.rotation;  
            _isPlaying = true;  
  
            // 获取攻击来源的位置，计算XZ平面方向向量  
            _hitDirXZ = ComputeHitDirectionXZ();  
        }  
  
        public override void OnTick(float time)  
        {  
            base.OnTick(time);  
  
            if (!_isPlaying || _targetTransform == null)  
            {  
                RemoveSelf();  
                KillSelf();  
                return;  
            }  
  
            float elapsed = time - _startTime;  
            float t = Mathf.Clamp01(elapsed / _duration);  
  
            // 使用正弦曲线：0 → 峰值(后仰) → 0(恢复)  
            // sin(π * t): t=0 → 0, t=0.5 → 1(最大后仰), t=1 → 0(完全恢复)  
            float tiltFactor = Mathf.Sin(Mathf.PI * t);  
            float tiltAngle = MaxTiltAngle * tiltFactor;  
  
            // 绕受击方向的垂直轴（XZ平面的right向量）旋转  
            // hitDirXZ 是从受击者指向攻击者的方向, 后仰需要绕其叉积轴旋转  
            Vector3 tiltAxis = Vector3.Cross(Vector3.up, _hitDirXZ).normalized;  
            if (tiltAxis.sqrMagnitude < 0.001f)  
                tiltAxis = _targetTransform.right;  
  
            _targetTransform.rotation = _originalRotation * Quaternion.AngleAxis(tiltAngle,   
                _originalRotation * Vector3.Normalize(Quaternion.Inverse(_originalRotation) * tiltAxis));  
  
            // 简化写法：直接用世界空间旋转  
            _targetTransform.rotation = Quaternion.AngleAxis(tiltAngle, tiltAxis) * _originalRotation;  
  
            if (t >= 1f)  
            {  
                // 恢复原始旋转  
                _targetTransform.rotation = _originalRotation;  
                _isPlaying = false;  
                RemoveSelf();  
                KillSelf();  
            }  
        }  
  
        public override void OnDeactivate(float time)  
        {  
            base.OnDeactivate(time);  
            ResetRotation();  
        }  
  
        public override void OnRemove(float time)  
        {  
            base.OnRemove(time);  
            ResetRotation();  
            _targetTransform = null;  
        }  
  
        public override void Reset()  
        {  
            _isPlaying = false;  
            _hitDirXZ = Vector3.zero;  
        }  
  
        /// <summary>  
        /// 计算攻击来源到受击目标的XZ平面方向向量  
        /// </summary>  
        private Vector3 ComputeHitDirectionXZ()  
        {  
            Vector3 attackerPos = Vector3.zero;  
            bool hasAttackerPos = false;  
  
            // Cue来源是 GameplayEffect 时，从 GE 的 CEffectInUsage.Source 获取攻击者  
            if (_sourceType == CueSourceType.GameplayEffect && _sourceEntity != Entity.Null)  
            {  
                if (EntityManager.Exists(_sourceEntity) && EntityManager.HasComponent<CEffectInUsage>(_sourceEntity))  
                {  
                    var inUsage = EntityManager.GetComponentData<CEffectInUsage>(_sourceEntity);  
                    var sourceAsc = GASManager.GetAscFromEntity(inUsage.Source);  
                    if (sourceAsc?.GameObject != null)  
                    {  
                        attackerPos = sourceAsc.GameObject.transform.position;  
                        hasAttackerPos = true;  
                    }  
                }  
            }  
  
            if (!hasAttackerPos || _targetTransform == null)  
            {  
                // 没有攻击来源信息，默认用目标自身朝前方向作为受击方向  
                return _targetTransform != null ? -_targetTransform.forward : Vector3.back;  
            }  
  
            // 计算XZ平面方向: 从攻击者指向受击者  
            Vector3 targetPos = _targetTransform.position;  
            Vector3 dir = new Vector3(  
                targetPos.x - attackerPos.x,  
                0f,  
                targetPos.z - attackerPos.z  
            );  
  
            return dir.sqrMagnitude > 0.001f ? dir.normalized : -_targetTransform.forward;  
        }  
  
        /// <summary>  
        /// 恢复原始旋转  
        /// </summary>  
        private void ResetRotation()  
        {  
            if (_isPlaying && _targetTransform != null)  
            {  
                _targetTransform.rotation = _originalRotation;  
                _isPlaying = false;  
            }  
        }  
    }  
}