using UnityEngine;

namespace EXProceduralMachine
{
    public class FootPlacement
    {
        private BaseMultiLeggedLocomotion _locomotion;
        
        /// <summary>
        /// IK追踪目标点
        /// </summary>
        private Transform _ikTrackPoint;
        
        /// <summary>
        /// 脚的待机状态参考目标，（运动相对距离参考点）
        /// </summary>
        private Transform _fixedIdlePoint;
        
        public FootPlacement(BaseMultiLeggedLocomotion locomotion,Transform fixedIdlePoint)
        {
            _locomotion = locomotion;
            _fixedIdlePoint = fixedIdlePoint;
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
            
        }
    }
}