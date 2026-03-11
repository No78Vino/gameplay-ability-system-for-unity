using UnityEngine;  
  
namespace GAS.Runtime  
{  
    public class TaskDodgeMove : AbilityTaskBase<XParamDodgeMove>  
    {  
        private Vector3 _dodgeDirection;  
        private CharacterController _controller;  
        private bool _directionLocked;  
        private int _windUpFrames;  
  
        public TaskDodgeMove(AbilityLogicBase logic) : base(logic)  
        {  
        }  
  
        public override void InitParameters(XParam parameter)  
        {  
            base.InitParameters(parameter);  
            _windUpFrames = Parameter.WindUpFrames;  
        }  
  
        protected override void OnBegin(int startFrame)  
        {  
            // 获取Owner的GameObject上的CharacterController  
            var go = Owner.GameObject;  
            _controller = go.GetComponent<CharacterController>();  
            _directionLocked = false;  
        }  
  
        protected override void OnTick(int frameIndex)  
        {  
            if (!_directionLocked)  
            {  
                // 前摇期间：持续读取最新输入方向  
                _dodgeDirection = GetCurrentInputDirection();  
  
                // 到达触发帧时锁定方向  
                if (frameIndex >= _startTime + _windUpFrames)  
                {  
                    _directionLocked = true;  
                    if (_dodgeDirection == Vector3.zero)  
                        _dodgeDirection = Owner.GameObject.transform.forward; // fallback：角色朝向  
                }  
            }  
            else  
            {  
                // 位移阶段：沿锁定方向移动  
                var speed = Parameter.DodgeSpeed;  
                _controller.Move(_dodgeDirection.normalized * speed * Time.fixedDeltaTime);  
            }  
        }  
  
        protected override void OnFinish(int endFrame)  
        {  
            // cleanup  
        }  
  
        /// <summary>  
        /// 从外部读取当前输入方向  
        /// 方式1: 从EasyInputController单例读取 (Demo适用)  
        /// 方式2: 从Owner MonoBehaviour上的某个缓存字段读取  
        /// </summary>  
        private Vector3 GetCurrentInputDirection()  
        {  
            // Demo中直接从InputController获取movement vector  
            var inputController = DemoForESC._Script.Controller  
                .EasyInputController.Inst();  
            if (inputController != null)  
                return inputController.GetMovementVector();  
            return Owner.GameObject.transform.forward;  
        }  
    }  
}