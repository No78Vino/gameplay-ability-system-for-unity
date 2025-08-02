using GAS.RuntimeWithECS;
using GAS.Runtime;
using Unity.Entities;
using UnityEngine;

namespace DemoForESC._Script.Gas.Ability
{
    public class AbilityParamMove : IAbilityParam
    {
        private Vector3 _moveDirection;
        private Vector3 _viewPointForward;
        private float _rotationOffset = 0.1f; // 转身缓冲
        
        public float RotationOffset => _rotationOffset;
        public Vector3 ViewPointForward => _viewPointForward;
        public Vector3 MoveDirection => _moveDirection;
        
        public AbilityParamMove()
        {
            _moveDirection = Vector3.zero;
            _viewPointForward = Vector3.forward;
            _rotationOffset = 0.1f;
        }
        
        public void SetValue(Vector3 moveDirection,Vector3 viewPointForward, float rotationOffset)
        {
            _moveDirection = moveDirection;
            _viewPointForward = viewPointForward;
            _rotationOffset = rotationOffset;
        }
    }
    
    public class ALMove : AbilityLogicBase<AbilityParamMove>
    {
        private BaseUnit _unit;
        private CharacterController _controller;
        
        public ALMove(Entity ability) : base(ability)
        {
        }

        public void SetUnit(BaseUnit unit)
        {
            _unit = unit;
            _controller = unit.GetComponent<CharacterController>();
        }

        public override void ActivateAbility(GlobalTimer timer)
        {
            Debug.Log($"Entity:{_abilityEntity}  ActivateAbility ALMove");
        }

        public override void CancelAbility(GlobalTimer timer)
        {
            Debug.Log($"Entity:{_abilityEntity}  CancelAbility ALMove");
        }

        public override void EndAbility(GlobalTimer timer)
        {
            Debug.Log($"Entity:{_abilityEntity}  EndAbility ALMove");
        }

        public override void AbilityTick(GlobalTimer timer)
        {
            Debug.Log($"Entity:{_abilityEntity} AbilityTick ALMove");
            
            // 保持角色面向相机方向（带缓冲）
            var targetRotation = Quaternion.LookRotation(_param.ViewPointForward);
            _unit.transform.rotation = Quaternion.Slerp(
                _unit.transform.rotation,
                targetRotation,
                _param.RotationOffset * Time.deltaTime * 100
            );
            
            // 移动执行
            var speed = _unit.GetSpeed();
            var motion = _param.MoveDirection * speed * Time.fixedDeltaTime;
            _controller.Move(motion);
        }
    }
}