using System.Collections;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/trigger-zoom/")]
    public class ProCamera2DTriggerZoom : BaseTrigger
    {
        public static string TriggerName = "Zoom Trigger";

        public bool SetSizeAsMultiplier = true;

        public float TargetZoom = 1.5f;

        public float ZoomSmoothness = 1f;

        [RangeAttribute(0, 1)]
        public float ExclusiveInfluencePercentage = .25f;

        public bool ResetSizeOnExit = false;
        public float ResetSizeSmoothness = 1f;

        float _startCamSize;
        float _initialCamSize;
        float _targetCamSize;
        float _targetCamSizeSmoothed;
        float _previousCamSize;
        float _zoomVelocity;

        float _initialCamDepth;

        void Start()
        {
            if (ProCamera2D == null)
                return;
            
            _startCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;
            _initialCamSize = _startCamSize;
            _targetCamSize = _startCamSize;
            _targetCamSizeSmoothed = _startCamSize;

            _initialCamDepth = Vector3D(ProCamera2D.LocalPosition);
        }

        protected override void EnteredTrigger()
        {
            base.EnteredTrigger();

            ProCamera2D.CurrentZoomTriggerID = _instanceID;

            if (ResetSizeOnExit)
            {
                _initialCamSize = _startCamSize;
                _targetCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;
                _targetCamSizeSmoothed = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;
            }
            else
            {
                _initialCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;
                _targetCamSize = _initialCamSize;
                _targetCamSizeSmoothed = _initialCamSize;
            }

            StartCoroutine(InsideTriggerRoutine());
        }

        protected override void ExitedTrigger()
        {
            base.ExitedTrigger();

            if (ResetSizeOnExit)
            {
                _targetCamSize = _startCamSize;

                StartCoroutine(OutsideTriggerRoutine());
            }
        }

        IEnumerator InsideTriggerRoutine()
        {
            while (_insideTrigger &&
                   _instanceID == ProCamera2D.CurrentZoomTriggerID)
            {
                _exclusiveInfluencePercentage = ExclusiveInfluencePercentage;

                // Find percentage distance to center
                var targetPos = new Vector2(Vector3H(UseTargetsMidPoint ? ProCamera2D.TargetsMidPoint : TriggerTarget.position), Vector3V(UseTargetsMidPoint ? ProCamera2D.TargetsMidPoint : TriggerTarget.position));
                var distancePercentage = GetDistanceToCenterPercentage(targetPos);

                // Calculate the target size
                float finalTargetSize;
                if (SetSizeAsMultiplier)
                    finalTargetSize = _startCamSize / TargetZoom;
                else if (ProCamera2D.GameCamera.orthographic)
                    finalTargetSize = TargetZoom;
                else
                    finalTargetSize = Mathf.Abs(_initialCamDepth) * Mathf.Tan(TargetZoom * 0.5f * Mathf.Deg2Rad);

                var newTargetOrtographicSize = (_initialCamSize * distancePercentage) + (finalTargetSize * (1 - distancePercentage));

                if ((finalTargetSize > ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f
                    && newTargetOrtographicSize > _targetCamSize) ||
                    (finalTargetSize < ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f
                    && newTargetOrtographicSize < _targetCamSize) ||
                    ResetSizeOnExit)
                {
                    _targetCamSize = newTargetOrtographicSize;
                }

                // Detect if the camera size is bounded
                _previousCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y;

                // Yield
                yield return ProCamera2D.GetYield();

                // Update camera size if needed
                if (Mathf.Abs(ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f - _targetCamSize) > .0001f)
                    UpdateScreenSize(ResetSizeOnExit ? ResetSizeSmoothness : ZoomSmoothness);

                // If the camera is bounded, reset the easing
                if (_previousCamSize == ProCamera2D.ScreenSizeInWorldCoordinates.y)
                {
                    _targetCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;
                    _targetCamSizeSmoothed = _targetCamSize;
                    _zoomVelocity = 0f;
                }
            }
        }

        IEnumerator OutsideTriggerRoutine()
        {
            while (!_insideTrigger &&
                   _instanceID == ProCamera2D.CurrentZoomTriggerID &&
                    Mathf.Abs(ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f - _targetCamSize) > .0001f)
            {
                UpdateScreenSize(ResetSizeOnExit ? ResetSizeSmoothness : ZoomSmoothness);
                yield return ProCamera2D.GetYield();
            }
            _zoomVelocity = 0f;
        }

        protected void UpdateScreenSize(float smoothness)
        {
            _targetCamSizeSmoothed = Mathf.SmoothDamp(_targetCamSizeSmoothed, _targetCamSize, ref _zoomVelocity, smoothness, float.MaxValue, ProCamera2D.DeltaTime);

            ProCamera2D.UpdateScreenSize(_targetCamSizeSmoothed);
        }

        #if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            _exclusiveInfluencePercentage = ExclusiveInfluencePercentage;

            base.DrawGizmos();

            var gameCamera = ProCamera2D.GetComponent<Camera>();
            var cameraDimensions = Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)));
            float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

            var startCamSize = Application.isPlaying ? _startCamSize : cameraDimensions.y / 2;

            float finalTargetSize;
            if (SetSizeAsMultiplier)
                finalTargetSize = startCamSize / TargetZoom;
            else if (ProCamera2D.GameCamera.orthographic)
                finalTargetSize = TargetZoom;
            else
                finalTargetSize = Mathf.Abs(Application.isPlaying ? _initialCamDepth : Vector3D(ProCamera2D.transform.localPosition)) * Mathf.Tan(TargetZoom * 0.5f * Mathf.Deg2Rad);
            
            Gizmos.DrawIcon(
                VectorHVD(
                    Vector3H(transform.position), 
                    Vector3V(transform.position), 
                    cameraDepthOffset), 
                startCamSize > finalTargetSize ? "ProCamera2D/gizmo_icon_zoom_in.png" : "ProCamera2D/gizmo_icon_zoom_out.png", 
                false);

            if (ResetSizeOnExit)
                Gizmos.DrawIcon(
                    VectorHVD(
                        Vector3H(transform.position), 
                        Vector3V(transform.position), 
                        cameraDepthOffset), 
                    "ProCamera2D/gizmo_icon_reset.png", 
                    false);
        }

        void OnDrawGizmosSelected()
        {
            if (ProCamera2D == null)
                return;
            
            var gameCamera = ProCamera2D.GetComponent<Camera>();
            var cameraDimensions = Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)));
            float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);
            var cameraCenter = VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset);

            var startCamSize = Application.isPlaying ? _startCamSize : cameraDimensions.y / 2;

            float finalTargetSize;
            if (SetSizeAsMultiplier)
                finalTargetSize = startCamSize / TargetZoom;
            else if (ProCamera2D.GameCamera.orthographic)
                finalTargetSize = TargetZoom;
            else
                finalTargetSize = Mathf.Abs(Application.isPlaying ? _initialCamDepth : Vector3D(ProCamera2D.transform.localPosition)) * Mathf.Tan(TargetZoom * 0.5f * Mathf.Deg2Rad);

            Gizmos.color = EditorPrefsX.GetColor(PrefsData.ZoomTriggerColorKey, PrefsData.ZoomTriggerColorValue);
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 1f);
            Gizmos.DrawWireCube(cameraCenter, VectorHV(finalTargetSize * 2 * ProCamera2D.GameCamera.aspect, finalTargetSize * 2));
        }
        #endif
    }
}