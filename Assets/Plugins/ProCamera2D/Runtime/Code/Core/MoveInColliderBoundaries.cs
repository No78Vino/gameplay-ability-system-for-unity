using System;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public struct RaycastOrigins
    {
        public Vector3 TopRight;
        public Vector3 TopLeft;
        public Vector3 BottomRight;
        public Vector3 BottomLeft;
    }

    public struct CameraCollisionState
    {
        public bool VTopLeft;
        public bool HTopLeft;
        public bool VTopRight;
        public bool HTopRight;
        public bool VBottomLeft;
        public bool HBottomLeft;
        public bool VBottomRight;
        public bool HBottomRight;
    }

    public class MoveInColliderBoundaries
    {
        Func<Vector3, float> Vector3H;
        Func<Vector3, float> Vector3V;
        Func<float, float, Vector3> VectorHV;

        const float Offset = .2f;
        const float RaySizeCompensation = .2f;

        public Transform CameraTransform;
        public Vector2 CameraSize;
        public LayerMask CameraCollisionMask;

        public int TotalHorizontalRays = 3;
        public int TotalVerticalRays = 3;

        public RaycastOrigins RaycastOrigins { get { return _raycastOrigins; } }
        RaycastOrigins _raycastOrigins;

        public CameraCollisionState CameraCollisionState { get { return _cameraCollisionState; } }
        CameraCollisionState _cameraCollisionState;

        RaycastHit _raycastHit;

        float _verticalDistanceBetweenRays;
        float _horizontalDistanceBetweenRays;

        ProCamera2D _proCamera2D;

        public MoveInColliderBoundaries(ProCamera2D proCamera2D)
        {
            _proCamera2D = proCamera2D;

            switch (_proCamera2D.Axis)
            {
                case MovementAxis.XY:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.y;
                    VectorHV = (h, v) => new Vector3(h, v, 0);
                    break;
                case MovementAxis.XZ:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.z;
                    VectorHV = (h, v) => new Vector3(h, 0, v);
                    break;
                case MovementAxis.YZ:
                    Vector3H = vector => vector.z;
                    Vector3V = vector => vector.y;
                    VectorHV = (h, v) => new Vector3(0, v, h);
                    break;
            }
        }

        public Vector3 Move(Vector3 deltaMovement)
        {
            // Update raycasts origins
            UpdateRaycastOrigins();

            // Shoot corner rays to calculate offset and force movement if needed
            GetOffsetAndForceMovement(_raycastOrigins.BottomLeft, ref deltaMovement, ref _cameraCollisionState.HBottomLeft, ref _cameraCollisionState.VBottomLeft, -1f, -1f);
            GetOffsetAndForceMovement(_raycastOrigins.BottomRight, ref deltaMovement, ref _cameraCollisionState.HBottomRight, ref _cameraCollisionState.VBottomRight, 1f, -1f);
            GetOffsetAndForceMovement(_raycastOrigins.TopLeft, ref deltaMovement, ref _cameraCollisionState.HTopLeft, ref _cameraCollisionState.VTopLeft, -1f, 1f);
            GetOffsetAndForceMovement(_raycastOrigins.TopRight, ref deltaMovement, ref _cameraCollisionState.HTopRight, ref _cameraCollisionState.VTopRight, 1f, 1f);

            // Check movement in the horizontal dir
            float h = 0f;
            if (Vector3H(deltaMovement) != 0)
                h = MoveInAxis(Vector3H(deltaMovement), true);

            // Check movement in the vertical dir
            float v = 0f;
            if (Vector3V(deltaMovement) != 0)
                v = MoveInAxis(Vector3V(deltaMovement), false);

            // Return updated movement
            return VectorHV(h, v);
        }

        void UpdateRaycastOrigins()
        {
            _raycastOrigins.BottomRight = VectorHV(Vector3H(CameraTransform.localPosition) + (CameraSize.x / 2), Vector3V(CameraTransform.localPosition) - (CameraSize.y / 2));

            _raycastOrigins.BottomLeft = VectorHV(Vector3H(CameraTransform.localPosition) - (CameraSize.x / 2), Vector3V(CameraTransform.localPosition) - (CameraSize.y / 2));

            _raycastOrigins.TopRight = VectorHV(Vector3H(CameraTransform.localPosition) + (CameraSize.x / 2), Vector3V(CameraTransform.localPosition) + (CameraSize.y / 2));

            _raycastOrigins.TopLeft = VectorHV(Vector3H(CameraTransform.localPosition) - (CameraSize.x / 2), Vector3V(CameraTransform.localPosition) + (CameraSize.y / 2));

            _horizontalDistanceBetweenRays = CameraSize.x / (TotalVerticalRays - 1);
            _verticalDistanceBetweenRays = CameraSize.y / (TotalHorizontalRays - 1);
        }

        void GetOffsetAndForceMovement(Vector3 rayTargetPos, ref Vector3 deltaMovement, ref bool horizontalCheck, ref bool verticalCheck, float hSign, float vSign)
        {
            Vector3 rayOrigin = VectorHV(Vector3H(CameraTransform.localPosition), Vector3V(CameraTransform.localPosition));
            Vector3 rayDirection = (rayTargetPos - rayOrigin).normalized;
            float raySize = (rayTargetPos - rayOrigin).magnitude + .01f + .5f;
            
            DrawRay(rayOrigin, rayDirection * raySize, Color.yellow);

            if (Physics.Raycast(rayOrigin, rayDirection, out _raycastHit, raySize, CameraCollisionMask))
            {
                if (Mathf.Abs(Vector3H(_raycastHit.normal)) > Mathf.Abs(Vector3V(_raycastHit.normal)))
                {
                    horizontalCheck = !verticalCheck;
                    if (Vector3H(deltaMovement) == 0)
                    {
                        var deltaMovH = .1f * hSign;
                        deltaMovement = VectorHV(deltaMovH, Vector3V(deltaMovement));
                        var h = MoveInAxis(Vector3H(deltaMovement), true);
                        deltaMovement = VectorHV(h, Vector3V(deltaMovement));
                    }
                }
                else
                {
                    verticalCheck = !horizontalCheck;
                    if (Vector3V(deltaMovement) == 0)
                    {
                        var deltaMovV = .1f * vSign;
                        deltaMovement = VectorHV(Vector3H(deltaMovement), deltaMovV);
                        var v = MoveInAxis(Vector3V(deltaMovement), false);
                        deltaMovement = VectorHV(Vector3H(deltaMovement), v);
                    }
                }
            }
            else
            {
                horizontalCheck = false;
                verticalCheck = false;
            }
        }

        float MoveInAxis(float deltaMovement, bool isHorizontal)
        {
            bool isPositiveDirection = deltaMovement > 0;
            float rayDistance = Mathf.Abs(deltaMovement) + RaySizeCompensation;

            Vector3 rayDirection;
            Vector3 initialRayOrigin;
            if (isHorizontal)
            {
                rayDirection = isPositiveDirection ? CameraTransform.right : -CameraTransform.right;
                initialRayOrigin = isPositiveDirection ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;
            }
            else
            {
                rayDirection = isPositiveDirection ? CameraTransform.up : -CameraTransform.up;
                initialRayOrigin = isPositiveDirection ? _raycastOrigins.TopLeft : _raycastOrigins.BottomLeft;
            }
            
            float raycastHitPos = Mathf.NegativeInfinity;
            bool hasRayHit = false;
            var totalRays = isHorizontal ? TotalHorizontalRays : TotalVerticalRays;
            for (int i = 0; i < totalRays; i++)
            {
                float rayH = isHorizontal ? Vector3H(initialRayOrigin) : Vector3H(initialRayOrigin) + i * _horizontalDistanceBetweenRays;
                float rayV = isHorizontal ? Vector3V(initialRayOrigin) + i * _verticalDistanceBetweenRays : Vector3V(initialRayOrigin);

                // Add a small offset to avoid collisions at "zero"
                if (isHorizontal)
                {
                    if ((isPositiveDirection && _cameraCollisionState.VBottomRight && i == 0) ||
                        (!isPositiveDirection && _cameraCollisionState.VBottomLeft && i == 0))
                        rayV += Offset;

                    if ((isPositiveDirection && _cameraCollisionState.VTopRight && i == totalRays - 1) ||
                        (!isPositiveDirection && _cameraCollisionState.VTopLeft && i == totalRays - 1))
                        rayV -= Offset;    
                }
                else
                {
                    if ((isPositiveDirection && _cameraCollisionState.HTopLeft && i == 0) ||
                        (!isPositiveDirection && _cameraCollisionState.HBottomLeft && i == 0))
                        rayH += Offset;

                    if ((isPositiveDirection && _cameraCollisionState.HTopRight && i == totalRays - 1) ||
                        (!isPositiveDirection && _cameraCollisionState.HBottomRight && i == totalRays - 1))
                        rayH -= Offset;    
                }
                
                Vector3 ray = VectorHV(rayH, rayV);

                // Raycast
                if (Physics.Raycast(ray, rayDirection, out _raycastHit, rayDistance, CameraCollisionMask))
                {
                    DrawRay(ray, rayDirection * rayDistance, Color.red);

                    if (isHorizontal &&
                        hasRayHit &&
                        (isPositiveDirection && raycastHitPos <= Vector3H(_raycastHit.point)) ||
                        (!isPositiveDirection && raycastHitPos >= Vector3H(_raycastHit.point)))
                        continue;
                    else if (hasRayHit &&
                             (isPositiveDirection && raycastHitPos <= Vector3V(_raycastHit.point)) ||
                             (!isPositiveDirection && raycastHitPos >= Vector3V(_raycastHit.point)))
                        continue;

                    hasRayHit = true;

                    deltaMovement = isHorizontal ? Vector3H(_raycastHit.point) - Vector3H(ray) + (isPositiveDirection ? -RaySizeCompensation : RaySizeCompensation) : 
                                                    Vector3V(_raycastHit.point) - Vector3V(ray) + (isPositiveDirection ? -RaySizeCompensation : RaySizeCompensation);

                    raycastHitPos = isHorizontal ? Vector3H(_raycastHit.point) : Vector3V(_raycastHit.point);
                }
                else
                {
                    DrawRay(ray, rayDirection * rayDistance, Color.cyan);
                }
            }

            return deltaMovement;
        }

        private void DrawRay(Vector3 start, Vector3 dir, Color color, float duration = 0)
        {
            if (duration != 0)
                Debug.DrawRay(start, dir, color, duration);
            else
                Debug.DrawRay(start, dir, color);
        }
    }
}