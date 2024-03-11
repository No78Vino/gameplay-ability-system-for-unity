using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-geometry-boundaries/")]
    public class ProCamera2DGeometryBoundaries : BasePC2D, IPositionDeltaChanger
    {
        public static string ExtensionName = "Geometry Boundaries";

        [Tooltip("The layer that contains the (3d) colliders that define the boundaries for the camera")]
        public LayerMask BoundariesLayerMask;

        public MoveInColliderBoundaries MoveInColliderBoundaries;

        override protected void Awake()
        {
            base.Awake();

            MoveInColliderBoundaries = new MoveInColliderBoundaries(ProCamera2D);
            MoveInColliderBoundaries.CameraTransform = ProCamera2D.transform;
            MoveInColliderBoundaries.CameraCollisionMask = BoundariesLayerMask;

            ProCamera2D.AddPositionDeltaChanger(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(ProCamera2D)
                ProCamera2D.RemovePositionDeltaChanger(this);
        }

        #region IPositionDeltaChanger implementation

        public Vector3 AdjustDelta(float deltaTime, Vector3 originalDelta)
        {
            if (!enabled)
                return originalDelta;
            
            MoveInColliderBoundaries.CameraSize = ProCamera2D.ScreenSizeInWorldCoordinates;

            // Apply movement considering the collider boundaries
            return MoveInColliderBoundaries.Move(originalDelta);
        }

        public int PDCOrder { get { return _pdcOrder; } set { _pdcOrder = value; } }
        int _pdcOrder = 3000;

        #endregion
    }
}