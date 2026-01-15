using System.Collections.Generic;
using GAS.Runtime;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace DemoForESC._Script.Gas.Ability
{
    public class ExParameterBaseMove : IExParameterBase
    {
        private Vector3 _moveDirection;
        private Vector3 _viewPointForward;

        [ShowInInspector]
        public float RotationOffset { get; private set; } = 0.1f;

        public Vector3 ViewPointForward => _viewPointForward;
        public Vector3 MoveDirection => _moveDirection;
        
        public ExParameterBaseMove()
        {
            _moveDirection = Vector3.zero;
            _viewPointForward = Vector3.forward;
            RotationOffset = 0.1f;
        }
        
        public void SetValue(Vector3 moveDirection,Vector3 viewPointForward)
        {
            _moveDirection = moveDirection;
            _viewPointForward = viewPointForward;
        }
        
        public void SetRotationOffset(float offset)
        {
            RotationOffset = offset;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                RotationOffset = 0.1f;
                return;
            }

            if (float.TryParse(paramData[0].ToString(), out var val))
                RotationOffset = val;
            else
                RotationOffset = 0.1f;
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object> { RotationOffset };
            return result;
        }
#endif
    }
    
    public class ALMove : AbilityLogicBase<ExParameterBaseMove>
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
            var motion = _param.MoveDirection * speed * Time.fixedDeltaTime / 10;
            _controller.Move(motion);
        }
        
        
    }
}