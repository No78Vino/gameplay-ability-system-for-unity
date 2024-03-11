using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/trigger-boundaries/")]
    public class ProCamera2DTriggerBoundaries : BaseTrigger, IPositionOverrider
    {
        public static string TriggerName = "Boundaries Trigger";

        public ProCamera2DNumericBoundaries NumericBoundaries;

        public bool AreBoundariesRelative = true;
        
        public bool UseTopBoundary = true;
        public float TopBoundary = 10;
        public bool UseBottomBoundary = true;
        public float BottomBoundary = -10;
        public bool UseLeftBoundary = true;
        public float LeftBoundary = -10;
        public bool UseRightBoundary = true;
        public float RightBoundary = 10;

        public float TransitionDuration = 1f;
        public EaseType TransitionEaseType;

        public bool ChangeZoom;
        public float TargetZoom = 1.5f;
        public float ZoomSmoothness = 1f;

        public bool IsCurrentTrigger { get { return NumericBoundaries.CurrentBoundariesTrigger._instanceID == _instanceID; } }

        public bool SetAsStartingBoundaries
        {
            set
            {
                if (value && !_setAsStartingBoundaries)
                {
                    var allBoundariesTriggers = FindObjectsOfType(typeof(ProCamera2DTriggerBoundaries));
                    foreach (ProCamera2DTriggerBoundaries trigger in allBoundariesTriggers)
                    {
                        trigger.SetAsStartingBoundaries = false;
                    }
                }

                _setAsStartingBoundaries = value;
            }

            get
            {
                return _setAsStartingBoundaries;
            }
        }

        /// <summary>Internal use only. Use the property SetAsStartingBoundaries</summary>
        public bool _setAsStartingBoundaries;

        float _initialCamSize;

        BoundariesAnimator _boundsAnim;

        float _targetTopBoundary;
        float _targetBottomBoundary;
        float _targetLeftBoundary;
        float _targetRightBoundary;

        bool _transitioning;
        Vector3 _newPos;

        protected override void Awake()
        {
            base.Awake();

            ProCamera2D.AddPositionOverrider(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(ProCamera2D)
                ProCamera2D.RemovePositionOverrider(this);
        }

        void Start()
        {
            if (ProCamera2D == null)
                return;
            
            if (NumericBoundaries == null)
            {
                var numericBoundaries = FindObjectOfType<ProCamera2DNumericBoundaries>();
                NumericBoundaries = numericBoundaries == null ? ProCamera2D.gameObject.AddComponent<ProCamera2DNumericBoundaries>() : numericBoundaries;
            }

            _boundsAnim = new BoundariesAnimator(ProCamera2D, NumericBoundaries);
            _boundsAnim.OnTransitionStarted += () =>
            {
                if (NumericBoundaries.OnBoundariesTransitionStarted != null)
                    NumericBoundaries.OnBoundariesTransitionStarted();
            };

            _boundsAnim.OnTransitionFinished += () =>
            {
                if (NumericBoundaries.OnBoundariesTransitionFinished != null)
                    NumericBoundaries.OnBoundariesTransitionFinished();
            };

            GetTargetBoundaries();

            if (SetAsStartingBoundaries)
                SetBoundaries();

            _initialCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;
        }

        #region IPositionOverrider implementation

        public Vector3 OverridePosition(float deltaTime, Vector3 originalPosition)
        {
            if (!enabled)
                return originalPosition;

            if (_transitioning)
                return _newPos;
            else
                return originalPosition;
        }

        public int POOrder { get { return _poOrder; } set { _poOrder = value; } }
        int _poOrder = 1000;

        #endregion

        protected override void EnteredTrigger()
        {
            base.EnteredTrigger();

            if (NumericBoundaries.CurrentBoundariesTrigger != null)
                StartCoroutine(TurnOffPreviousTrigger(NumericBoundaries.CurrentBoundariesTrigger));

            if ((NumericBoundaries.CurrentBoundariesTrigger != null &&
                 NumericBoundaries.CurrentBoundariesTrigger._instanceID != _instanceID) || 
                NumericBoundaries.CurrentBoundariesTrigger == null) 
            {
                NumericBoundaries.CurrentBoundariesTrigger = this;

                StartCoroutine (Transition ());
            }
        }

        IEnumerator TurnOffPreviousTrigger(ProCamera2DTriggerBoundaries trigger)
        {
            yield return new WaitForEndOfFrame();

            trigger._transitioning = false;
        }

        /// <summary>
        /// Sets the Numeric Boundaries extension to the values of this trigger
        /// </summary>
        public void SetBoundaries()
        {
            if (NumericBoundaries != null)
            {
                NumericBoundaries.CurrentBoundariesTrigger = this;

                NumericBoundaries.UseLeftBoundary = UseLeftBoundary;
                if (UseLeftBoundary)
                    NumericBoundaries.LeftBoundary = NumericBoundaries.TargetLeftBoundary = _targetLeftBoundary;

                NumericBoundaries.UseRightBoundary = UseRightBoundary;
                if (UseRightBoundary)
                    NumericBoundaries.RightBoundary = NumericBoundaries.TargetRightBoundary = _targetRightBoundary;

                NumericBoundaries.UseTopBoundary = UseTopBoundary;
                if (UseTopBoundary)
                    NumericBoundaries.TopBoundary = NumericBoundaries.TargetTopBoundary = _targetTopBoundary;

                NumericBoundaries.UseBottomBoundary = UseBottomBoundary;
                if (UseBottomBoundary)
                    NumericBoundaries.BottomBoundary = NumericBoundaries.TargetBottomBoundary = _targetBottomBoundary;
            }
        }

        void GetTargetBoundaries()
        {
            if (AreBoundariesRelative)
            {
                _targetTopBoundary = Vector3V(transform.position) + TopBoundary;
                _targetBottomBoundary = Vector3V(transform.position) + BottomBoundary;
                _targetLeftBoundary = Vector3H(transform.position) + LeftBoundary;
                _targetRightBoundary = Vector3H(transform.position) + RightBoundary;
            }
            else
            {
                _targetTopBoundary = TopBoundary;
                _targetBottomBoundary = BottomBoundary;
                _targetLeftBoundary = LeftBoundary;
                _targetRightBoundary = RightBoundary;
            }
        }

        IEnumerator Transition()
        {
            if (!UseTopBoundary && !UseBottomBoundary && !UseLeftBoundary && !UseRightBoundary)
            {
                NumericBoundaries.UseTopBoundary = false;
                NumericBoundaries.UseBottomBoundary = false;
                NumericBoundaries.UseLeftBoundary = false;
                NumericBoundaries.UseRightBoundary = false;
                yield break;
            }
            
            var position = transform.position;
            var topBoundary = AreBoundariesRelative ? position.y + TopBoundary : TopBoundary;
            var bottomBoundary = AreBoundariesRelative ? position.y + BottomBoundary : BottomBoundary;
            var leftBoundary = AreBoundariesRelative ? position.x + LeftBoundary : LeftBoundary;
            var rightBoundary = AreBoundariesRelative ? position.x + RightBoundary : RightBoundary;
            const float epsilon = 0.01f;

            // Avoid unnecessary transitions
            var skip = true;
            if (UseTopBoundary && (Mathf.Abs(NumericBoundaries.TopBoundary - topBoundary) > epsilon || !NumericBoundaries.UseTopBoundary))
                skip = false;
            if (skip && UseBottomBoundary && (Mathf.Abs(NumericBoundaries.BottomBoundary - bottomBoundary) > epsilon || !NumericBoundaries.UseBottomBoundary))
                skip = false;
            if (skip && UseLeftBoundary && (Mathf.Abs(NumericBoundaries.LeftBoundary - leftBoundary) > epsilon || !NumericBoundaries.UseLeftBoundary))
                skip = false;
            if (skip && UseRightBoundary && (Mathf.Abs(NumericBoundaries.RightBoundary - rightBoundary) > epsilon || !NumericBoundaries.UseRightBoundary))
                skip = false;
            if (skip)
                yield break;

            GetTargetBoundaries();

            _boundsAnim.UseTopBoundary = UseTopBoundary;
            _boundsAnim.TopBoundary = _targetTopBoundary;
            _boundsAnim.UseBottomBoundary = UseBottomBoundary;
            _boundsAnim.BottomBoundary = _targetBottomBoundary;
            _boundsAnim.UseLeftBoundary = UseLeftBoundary;
            _boundsAnim.LeftBoundary = _targetLeftBoundary;
            _boundsAnim.UseRightBoundary = UseRightBoundary;
            _boundsAnim.RightBoundary = _targetRightBoundary;

            _boundsAnim.TransitionDuration = TransitionDuration;
            _boundsAnim.TransitionEaseType = TransitionEaseType;

            // Zoom
            if (ChangeZoom && _initialCamSize / TargetZoom != ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f)
                ProCamera2D.UpdateScreenSize(_initialCamSize / TargetZoom, ZoomSmoothness, TransitionEaseType);
            

            // Move camera "manually"
            if (_boundsAnim.GetAnimsCount() > 1)
            {
                if (NumericBoundaries.MoveCameraToTargetRoutine != null)
                    NumericBoundaries.StopCoroutine(NumericBoundaries.MoveCameraToTargetRoutine);

                NumericBoundaries.MoveCameraToTargetRoutine = NumericBoundaries.StartCoroutine(MoveCameraToTarget());
            }

            // Start bounds animation
            yield return new WaitForEndOfFrame();
            _boundsAnim.Transition();
        }

        IEnumerator MoveCameraToTarget()
        {
            var initialCamPosH = Vector3H(ProCamera2D.LocalPosition);
            var initialCamPosV = Vector3V(ProCamera2D.LocalPosition);

            _newPos = VectorHVD(initialCamPosH, initialCamPosV, 0);
            _transitioning = true;

            var t = 0f;
            while (t <= 1.0f)
            {
                t += ProCamera2D.DeltaTime / TransitionDuration;

                var newPosH = Utils.EaseFromTo(initialCamPosH, ProCamera2D.CameraTargetPositionSmoothed.x, t, TransitionEaseType);
                var newPosV = Utils.EaseFromTo(initialCamPosV, ProCamera2D.CameraTargetPositionSmoothed.y, t, TransitionEaseType);

                LimitToNumericBoundaries(ref newPosH, ref newPosV);

                _newPos = VectorHVD(newPosH, newPosV, 0);

                yield return ProCamera2D.GetYield();
            }

            NumericBoundaries.MoveCameraToTargetRoutine = null;

            _transitioning = false;
        }

        void LimitToNumericBoundaries(ref float horizontalPos, ref float verticalPos)
        {
            if (NumericBoundaries.UseLeftBoundary && horizontalPos - ProCamera2D.ScreenSizeInWorldCoordinates.x / 2 < NumericBoundaries.LeftBoundary)
                horizontalPos = NumericBoundaries.LeftBoundary + ProCamera2D.ScreenSizeInWorldCoordinates.x / 2;
            else if (NumericBoundaries.UseRightBoundary && horizontalPos + ProCamera2D.ScreenSizeInWorldCoordinates.x / 2 > NumericBoundaries.RightBoundary)
                horizontalPos = NumericBoundaries.RightBoundary - ProCamera2D.ScreenSizeInWorldCoordinates.x / 2;

            if (NumericBoundaries.UseBottomBoundary && verticalPos - ProCamera2D.ScreenSizeInWorldCoordinates.y / 2 < NumericBoundaries.BottomBoundary)
                verticalPos = NumericBoundaries.BottomBoundary + ProCamera2D.ScreenSizeInWorldCoordinates.y / 2;
            else if (NumericBoundaries.UseTopBoundary && verticalPos + ProCamera2D.ScreenSizeInWorldCoordinates.y / 2 > NumericBoundaries.TopBoundary)
                verticalPos = NumericBoundaries.TopBoundary - ProCamera2D.ScreenSizeInWorldCoordinates.y / 2;
        }

        #if UNITY_EDITOR
        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

            Gizmos.DrawIcon(VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset - .01f * Mathf.Sign(Vector3D(ProCamera2D.transform.position))), "ProCamera2D/gizmo_icon_bg.png", false);

            if (UseTopBoundary)
            {
                Gizmos.DrawIcon(VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset), "ProCamera2D/gizmo_icon_bound_top.png", false);
            }

            if (UseBottomBoundary)
            {
                Gizmos.DrawIcon(VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset), "ProCamera2D/gizmo_icon_bound_bottom.png", false);
            }

            if (UseRightBoundary)
            {
                Gizmos.DrawIcon(VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset), "ProCamera2D/gizmo_icon_bound_right.png", false);
            }

            if (UseLeftBoundary)
            {
                Gizmos.DrawIcon(VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset), "ProCamera2D/gizmo_icon_bound_left.png", false);
            }

            if (SetAsStartingBoundaries)
                Gizmos.DrawIcon(VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset), "ProCamera2D/gizmo_icon_start.png", false);
        }

        void OnDrawGizmosSelected()
        {
            if (ProCamera2D == null)
                return;
            
            float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);
            var cameraCenter = VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset);
            var cameraDimensions = Utils.GetScreenSizeInWorldCoords(ProCamera2D.GetComponent<Camera>(), Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)));

            GetTargetBoundaries();

            Gizmos.color = EditorPrefsX.GetColor(PrefsData.BoundariesTriggerColorKey, PrefsData.BoundariesTriggerColorValue);
            if (UseTopBoundary)
            {
                Gizmos.DrawRay(VectorHVD(Vector3H(transform.position) - cameraDimensions.x / 2, _targetTopBoundary, cameraDepthOffset), ProCamera2D.transform.right * cameraDimensions.x);
                Utils.DrawArrowForGizmo(cameraCenter, VectorHV(0, _targetTopBoundary - Vector3V(transform.position)));
            }

            if (UseBottomBoundary)
            {
                Gizmos.DrawRay(VectorHVD(Vector3H(transform.position) - cameraDimensions.x / 2, _targetBottomBoundary, cameraDepthOffset), ProCamera2D.transform.right * cameraDimensions.x);
                Utils.DrawArrowForGizmo(cameraCenter, VectorHV(0, _targetBottomBoundary - Vector3V(transform.position)));
            }

            if (UseRightBoundary)
            {
                Gizmos.DrawRay(VectorHVD(_targetRightBoundary, Vector3V(transform.position) - cameraDimensions.y / 2, cameraDepthOffset), ProCamera2D.transform.up * cameraDimensions.y);
                Utils.DrawArrowForGizmo(cameraCenter, VectorHV(_targetRightBoundary - Vector3H(transform.position), 0));
            }

            if (UseLeftBoundary)
            {
                Gizmos.DrawRay(VectorHVD(_targetLeftBoundary, Vector3V(transform.position) - cameraDimensions.y / 2, cameraDepthOffset), ProCamera2D.transform.up * cameraDimensions.y);
                Utils.DrawArrowForGizmo(cameraCenter, VectorHV(_targetLeftBoundary - Vector3H(transform.position), 0));
            }
        }
        #endif
    }
}