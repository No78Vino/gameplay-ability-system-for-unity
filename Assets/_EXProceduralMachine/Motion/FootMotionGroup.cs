using System;
using UnityEngine;

namespace EXProceduralMachine
{
    [Serializable]
    public class FootMotionGroup
    {
        public FootConfig[] feet;

        private FootPlacement[] _footPlacements;
        public FootPlacement[] FootPlacements => _footPlacements;

        public BaseMultiLeggedLocomotion Locomotion { get;private set; }

        private Transform _tfGroup;
        public Transform GroupTransform => _tfGroup;
        
        public float PhaseDifference { get; private set; }
        
        public void Initialize(BaseMultiLeggedLocomotion locomotion,float phaseDifference)
        {
            PhaseDifference = phaseDifference;
            Locomotion = locomotion;
            
            if (_tfGroup == null)
            {
                _tfGroup = new GameObject("FootMotionGroup").transform;
                _tfGroup.SetParent(locomotion.MotionTransform);
            }

            if (_footPlacements == null)
            {
                _footPlacements = new FootPlacement[feet.Length];
                for (var i = 0; i < feet.Length; i++)
                {
                    var foot = new GameObject("Foot" + i).transform;
                    foot.SetParent(_tfGroup);
                    foot.position = feet[i].idlePoint.position;
                    _footPlacements[i] = new FootPlacement(this, feet[i],foot);
                }
            }
        }
        
        public bool IsMoving()
        {
            return _footPlacements.Length > 0 && _footPlacements[0].IsMoving();
        }

        public float DistanceInDirection()
        {
            if (_footPlacements.Length == 0) return 0;
            var foot = _footPlacements[0];
            return foot.DistanceInSpeedDirection();
        }

        public void Tick()
        {
            foreach (var foot in _footPlacements)
                foot.Tick();
        }
        
        /// <summary>
        /// 更新足对位置，足对开始移动
        /// </summary>
        public void UpdateFootPlacements()
        {
            foreach (var foot in _footPlacements)
            {
                foot.RefreshStepPoint();
            }
        }
    }
}