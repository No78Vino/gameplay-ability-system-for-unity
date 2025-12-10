using Unity.Mathematics;
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
        private float _startMoveTime;      // 开始移动时间
        private float _endMoveTime;    // 结束移动时间
        private Vector3 _startPos;       // 本次移动的起始位置
        private Vector3 _targetPos;      // 本次移动的目标位置

        public Vector3 StartPos => _startPos;
        public Vector3 TargetPos => _targetPos;
        
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
        public Transform IdlePoint => _fixedIdlePoint;

        private Vector3 _castPosition;
        public Vector3 CastPosition => _castPosition;

        public FootPlacement(FootMotionGroup group,FootConfig cfg,Transform stepPoint)
        {
            _group = group;
            _locomotion = _group.Locomotion;
            _fixedIdlePoint = cfg.idlePoint;
            _currentStepPoint = stepPoint;
            _ikTrackPoint = cfg.ikTrack;
            _offset = cfg.offset;
        }

        public void Tick()
        {
            _castPosition = EXMachHelper.GetGroundPoint(
                _currentStepPoint.position,
                _locomotion.castMaxDistance,
                _locomotion.groundLayer,
                Vector3.down) + _offset;
            
            Move();
        }

        /// <summary>
        /// 根据需求移动 _ikTrackPoint
        /// </summary>
        private void Move()
        {
            if (!_isMoving) return;
            
            // 归一化时间（0~1），超过1则设为1（到达终点）
            var timeNormalized = Mathf.Clamp01((Time.time-_startMoveTime) / (_endMoveTime-_startMoveTime));
            Debug.Log("timeNormalized = " + timeNormalized);
            // 计算当前位置
            var currentPos = _locomotion.CalculateFootPlacementMovingPoint(_startPos,_targetPos,timeNormalized);
            _ikTrackPoint.position = currentPos;

            // 检查是否到达终点
            if (!(timeNormalized >= 1f)) return;
            _isMoving = false;
            // 确保最终位置精准匹配目标点（避免浮点误差）
            _ikTrackPoint.position = _targetPos;
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
            // if (!((_currentStepPoint.position - _fixedIdlePoint.position).magnitude >=
            //       _group.PhaseDifference * _locomotion.L)) 
            //     return;
            //
            _currentStepPoint.position = _fixedIdlePoint.position;
        }

        public void SetMoving()
        {
            // 处理极限参数（避免除零/无效高度）
            var duration = math.min(_locomotion.T * _group.PhaseDifference,_locomotion.stepTime);
            _targetPos = _castPosition; // 目标点设为新的地面投射点
            
            if (duration <= 0.02f)
            {
                _ikTrackPoint.position = _targetPos;
                _isMoving = false;
                return;
            }

            if (_isMoving) return;
            // 重置移动状态（打断原有移动，顺滑衔接）
            _startPos = _ikTrackPoint.position; // 起始点设为当前位置
            _startMoveTime = Time.time;
            _endMoveTime = _startMoveTime + duration;
            _isMoving = true;
        }
    }
}