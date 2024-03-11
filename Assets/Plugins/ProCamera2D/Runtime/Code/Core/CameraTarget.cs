using System;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [Serializable]
    public class CameraTarget
    {
        public Transform TargetTransform;

        public float TargetInfluence
        {
            set 
            { 
                TargetInfluenceH = value;
                TargetInfluenceV = value;
            }
        }

        [RangeAttribute(0f, 1f)]
        public float TargetInfluenceH = 1f;

        [RangeAttribute(0f, 1f)]
        public float TargetInfluenceV = 1f;

        public Vector2 TargetOffset;

        public Vector3 TargetPosition
        {
            get
            {
                if (TargetTransform != null)
                    return _targetPosition = TargetTransform.position;
                else
                    return _targetPosition;
            }
        }
        Vector3 _targetPosition;
    }
}
