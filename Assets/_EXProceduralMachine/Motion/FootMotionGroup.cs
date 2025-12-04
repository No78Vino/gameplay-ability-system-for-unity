using System;
using Unity.Entities.UniversalDelegates;
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

        public float SplitStepLength => PhaseDifference * Locomotion.L;
        
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
            Vector3 fixedIdleCenter = IdleCenter();
            Vector3 currentCenter = CurrentCenter();
            return (fixedIdleCenter - currentCenter).magnitude;
        }
        
        public float DistanceRate()
        {
            var dist = DistanceInDirection();
            var splitL = Locomotion.L * PhaseDifference;
            return dist / splitL;
        }

        public Vector3 IdleCenter()
        {
            if (_footPlacements.Length == 0) return Locomotion.Body.transform.position;
            
            var fixedIdleCenter = Vector3.zero;
            foreach (var foot in _footPlacements)
                fixedIdleCenter += foot.IdlePoint.position;
            
            return fixedIdleCenter / _footPlacements.Length;
        }
        
        public Vector3 CurrentCenter()
        {
            if (_footPlacements.Length == 0) return Locomotion.Body.transform.position;
            
            var fixedIdleCenter = Vector3.zero;
            foreach (var foot in _footPlacements)
                fixedIdleCenter += foot.StepPoint.position;
            
            return fixedIdleCenter / _footPlacements.Length;
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
                foot.RefreshStepPoint();
        }
    }
}