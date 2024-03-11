using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-speed-based-zoom/")]
    public class ProCamera2DSpeedBasedZoom : BasePC2D, ISizeDeltaChanger
    {
        public static string ExtensionName = "Speed Based Zoom";

        [Tooltip("The speed at which the camera will reach it's max zoom out.")]
        public float CamVelocityForZoomOut = 5f;
        [Tooltip("Below this speed the camera zooms in. Above this speed the camera will start zooming out.")]
        public float CamVelocityForZoomIn = 2f;

        [Tooltip("Represents how smooth the zoom in of the camera should be. The lower the number the quickest the zoom is.")]
        [Range(0f, 3f)]
        public float ZoomInSmoothness = 1f;
        [Tooltip("Represents how smooth the zoom out of the camera should be. The lower the number the quickest the zoom is.")]
        [Range(0f, 3f)]
        public float ZoomOutSmoothness = 1f;

        [Tooltip("Represents the maximum amount the camera should zoom in when the camera speed is below SpeedForZoomIn")]
        public float MaxZoomInAmount = 2f;
        [Tooltip("Represents the maximum amount the camera should zoom out when the camera speed is equal to SpeedForZoomOut")]
        public float MaxZoomOutAmount = 2f;

        float _zoomVelocity;

        float _initialCamSize;
        float _previousCamSize;

        Vector3 _previousCameraPosition;

        [HideInInspector]
        public float CurrentVelocity;

        override protected void Awake()
        {
            base.Awake();

            if (ProCamera2D == null)
                return;

            _initialCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;
            _previousCamSize = _initialCamSize;

            _previousCameraPosition = VectorHV(Vector3H(ProCamera2D.LocalPosition), Vector3V(ProCamera2D.LocalPosition));

            ProCamera2D.AddSizeDeltaChanger(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if(ProCamera2D)
                ProCamera2D.RemoveSizeDeltaChanger(this);
        }

        #region ISizeDeltaChanger implementation

        public float AdjustSize(float deltaTime, float originalDelta)
        {
            if (!enabled)
                return originalDelta;

            // If the camera is bounded, reset the easing
            if (_previousCamSize == ProCamera2D.ScreenSizeInWorldCoordinates.y)
            {
                _zoomVelocity = 0f;
            }

            // Get camera velocity
            CurrentVelocity = (_previousCameraPosition - VectorHV(Vector3H(ProCamera2D.LocalPosition), Vector3V(ProCamera2D.LocalPosition))).magnitude / deltaTime;
            _previousCameraPosition = VectorHV(Vector3H(ProCamera2D.LocalPosition), Vector3V(ProCamera2D.LocalPosition));

            var currentSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * 0.5f;
            var targetSize = currentSize;

            // Zoom out
            if (CurrentVelocity > CamVelocityForZoomIn)
            {
                var speedPercentage = (CurrentVelocity - CamVelocityForZoomIn) / (CamVelocityForZoomOut - CamVelocityForZoomIn);
                var newSize = _initialCamSize * (1 + MaxZoomOutAmount - 1) * Mathf.Clamp01(speedPercentage);

                if (newSize > currentSize)
                    targetSize = newSize;
            }
            // Zoom in
            else
            {
                var speedPercentage = (1 - (CurrentVelocity / CamVelocityForZoomIn)).Remap(0.0f, 1.0f, 0.5f, 1.0f);
                var newSize = _initialCamSize / (MaxZoomInAmount * speedPercentage);

                if (newSize < currentSize)
                    targetSize = newSize;
            }

            if (Mathf.Abs(currentSize - targetSize) > .0001f)
            {
                float smoothness = (targetSize < currentSize) ? ZoomInSmoothness : ZoomOutSmoothness;
                targetSize = Mathf.SmoothDamp(currentSize, targetSize, ref _zoomVelocity, smoothness, Mathf.Infinity, deltaTime);
            }

            var zoomAmount = targetSize - (ProCamera2D.ScreenSizeInWorldCoordinates.y / 2);

            // Detect if the camera size is bounded
            _previousCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y;

            // Return the zoom delta - delta already factored in by SmoothDamp
            return originalDelta + zoomAmount;
        }

        public int SDCOrder { get { return _sdcOrder; } set { _sdcOrder = value; } }

        int _sdcOrder = 1000;

        #endregion

        override public void OnReset()
        {
            _previousCamSize = _initialCamSize;
            _previousCameraPosition = VectorHV(Vector3H(ProCamera2D.LocalPosition), Vector3V(ProCamera2D.LocalPosition));
            _zoomVelocity = 0;
        }
    }
}