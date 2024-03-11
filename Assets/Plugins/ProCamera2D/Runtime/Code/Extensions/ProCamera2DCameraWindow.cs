using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-camera-window/")]
    public class ProCamera2DCameraWindow : BasePC2D, IPositionDeltaChanger
    {
        public static string ExtensionName = "Camera Window";

        public Rect CameraWindowRect = new Rect(0f, 0f, .3f, .3f);
        Rect _cameraWindowRectInWorldCoords;

        public bool IsRelativeSizeAndPosition = true;

        protected override void Awake()
        {
            base.Awake();

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
            
            // Calculate the window rect
            _cameraWindowRectInWorldCoords = GetRectAroundTransf(CameraWindowRect, ProCamera2D.ScreenSizeInWorldCoordinates, _transform, IsRelativeSizeAndPosition);

            // If camera final horizontal position outside camera window rect
            var horizontalDeltaMovement = 0f;
            if (ProCamera2D.CameraTargetPositionSmoothed.x >= _cameraWindowRectInWorldCoords.x + _cameraWindowRectInWorldCoords.width)
            {
                horizontalDeltaMovement = ProCamera2D.CameraTargetPositionSmoothed.x - (Vector3H(_transform.localPosition) + _cameraWindowRectInWorldCoords.width / 2 + CameraWindowRect.x * (IsRelativeSizeAndPosition ? ProCamera2D.ScreenSizeInWorldCoordinates.x : 1f));
            }
            else if (ProCamera2D.CameraTargetPositionSmoothed.x <= _cameraWindowRectInWorldCoords.x)
            {
                horizontalDeltaMovement = ProCamera2D.CameraTargetPositionSmoothed.x - (Vector3H(_transform.localPosition) - _cameraWindowRectInWorldCoords.width / 2 + CameraWindowRect.x * (IsRelativeSizeAndPosition ? ProCamera2D.ScreenSizeInWorldCoordinates.x : 1f));
            }

            // If camera final vertical position outside camera window rect
            var verticalDeltaMovement = 0f;
            if (ProCamera2D.CameraTargetPositionSmoothed.y >= _cameraWindowRectInWorldCoords.y + _cameraWindowRectInWorldCoords.height)
            {
                verticalDeltaMovement = ProCamera2D.CameraTargetPositionSmoothed.y - (Vector3V(_transform.localPosition) + _cameraWindowRectInWorldCoords.height / 2 + CameraWindowRect.y * (IsRelativeSizeAndPosition ? ProCamera2D.ScreenSizeInWorldCoordinates.y : 1f));
            }
            else if (ProCamera2D.CameraTargetPositionSmoothed.y <= _cameraWindowRectInWorldCoords.y)
            {
                verticalDeltaMovement = ProCamera2D.CameraTargetPositionSmoothed.y - (Vector3V(_transform.localPosition) - _cameraWindowRectInWorldCoords.height / 2 + CameraWindowRect.y * (IsRelativeSizeAndPosition ? ProCamera2D.ScreenSizeInWorldCoordinates.y : 1f));
            }

            return VectorHV(horizontalDeltaMovement, verticalDeltaMovement);
        }

        public int PDCOrder { get { return _pdcOrder; } set { _pdcOrder = value; } }
        int _pdcOrder = 0;

        #endregion

        Rect GetRectAroundTransf(Rect rectNormalized, Vector2 rectSize, Transform transf, bool isRelative)
        {
            var finalRectSize = Vector2.Scale(new Vector2(rectNormalized.width, rectNormalized.height), isRelative ? rectSize : Vector2.one);

            var rectPositionX = Vector3H(transf.localPosition) - finalRectSize.x / 2 + rectNormalized.x * (isRelative ? rectSize.x : 1f);
            var rectPositionY = Vector3V(transf.localPosition) - finalRectSize.y / 2 + rectNormalized.y * (isRelative ? rectSize.y : 1f);

            return new Rect(rectPositionX, rectPositionY, finalRectSize.x, finalRectSize.y);
        }

        #if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            base.DrawGizmos();

            var gameCamera = ProCamera2D.GetComponent<Camera>();
            var cameraDimensions = gameCamera.orthographic ? Utils.GetScreenSizeInWorldCoords(gameCamera) : Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(transform.localPosition)));
            float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

            // Draw camera window
            Gizmos.color = EditorPrefsX.GetColor(PrefsData.CameraWindowColorKey, PrefsData.CameraWindowColorValue);
            var cameraRect = GetRectAroundTransf(CameraWindowRect, cameraDimensions, transform, IsRelativeSizeAndPosition);
            Gizmos.DrawWireCube(VectorHVD(cameraRect.x + cameraRect.width / 2, cameraRect.y + cameraRect.height / 2, cameraDepthOffset), VectorHV(cameraRect.width, cameraRect.height));
        }
        #endif
    }
}