using UnityEngine;

namespace EXProceduralMachine
{
    public class FootPlacement
    {
        private BaseMultiLeggedLocomotion _locomotion;
        private FootMotionGroup _group;
        private Vector3 _offset;
        
        // 移动状态变量
        private bool _isMoving;          // 是否正在移动
        private float _currentTime;      // 当前已移动时间
        private float _totalDuration;    // 本次移动总时长
        private Vector3 _startPos;       // 本次移动的起始位置
        private Vector3 _targetPos;      // 本次移动的目标位置
        
        /// <summary>
        /// IK追踪目标点
        /// </summary>
        private Transform _ikTrackPoint;
        
        /// <summary>
        /// 脚的待机状态参考目标，（运动相对距离参考点）
        /// </summary>
        private Transform _fixedIdlePoint;
        
        /// <summary>
        /// 当前足目标点（地面投射起点）
        /// </summary>
        private Transform _currentStepPoint;

        public Transform IkTrackPoint => _ikTrackPoint;
        public Transform StepPoint => _currentStepPoint;
        
        public FootPlacement(FootMotionGroup group,FootConfig cfg,Transform stepPoint)
        {
            _group = group;
            _locomotion = _group.Locomotion;
            _fixedIdlePoint = cfg.idlePoint;
            _currentStepPoint = stepPoint;
            _ikTrackPoint = cfg.ikTrack;
        }

        public void Tick()
        {
            Move();
        }

        /// <summary>
        /// 根据需求移动 _ikTrackPoint
        /// </summary>
        private void Move()
        {
            if (!_isMoving) return;

            // 累加移动时间
            _currentTime += Time.deltaTime;
            // 归一化时间（0~1），超过1则设为1（到达终点）
            float timeNormalized = Mathf.Clamp01(_currentTime / _totalDuration);

            // 计算当前位置
            Vector3 currentPos = _locomotion.CalculateFootPlacementMovingPoint(_startPos,_targetPos,timeNormalized);
            _ikTrackPoint.position = currentPos + _offset;

            // 检查是否到达终点
            if (timeNormalized >= 1f)
            {
                _isMoving = false;
                // 确保最终位置精准匹配目标点（避免浮点误差）
                _ikTrackPoint.position = _targetPos + _offset;
            }
        }

        public bool IsMoving() => _isMoving;
        
        public float DistanceInSpeedDirection()
        {
            var a = _currentStepPoint.position;
            var b = _fixedIdlePoint.position;
            return EXMachHelper.CalculateProjectionDistance(a,b, _locomotion.Direction);
        }
        
        public void RefreshStepPoint()
        {
            _currentStepPoint.position = _fixedIdlePoint.position;
            
            
            // 处理极限参数（避免除零/无效高度）
            var duration = _locomotion.T * _group.PhaseDifference;
            if (duration <= 0.01f)
            {
                _ikTrackPoint.position = _currentStepPoint.position + _offset;
                _isMoving = false;
                return;
            }

            // 重置移动状态（打断原有移动，顺滑衔接）
            _startPos = _ikTrackPoint.position - _offset;    // 起始点设为当前位置
            _targetPos = _currentStepPoint.position;
            _totalDuration = duration;
            _currentTime = 0f;
            _isMoving = true;
        }
    }
}