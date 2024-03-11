using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    /// <summary>
    /// Core class of the plugin. Everything starts and happens through here.
    /// All extensions and triggers have a reference to an instance of this class.
    /// </summary>
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/")]
    [RequireComponent(typeof(Camera))]
    public class ProCamera2D : MonoBehaviour, ISerializationCallbackReceiver
    {
		public const string Title = "Pro Camera 2D";
        public static readonly Version Version = new Version("2.9.5");

        #region Inspector Variables

        public List<CameraTarget> CameraTargets = new List<CameraTarget>();

        public bool CenterTargetOnStart;

        public MovementAxis Axis;

        public UpdateType UpdateType;

        public bool FollowHorizontal = true;
        public float HorizontalFollowSmoothness = 0.15f;

        public bool FollowVertical = true;
        public float VerticalFollowSmoothness = 0.15f;

        [Range(-1f, 1f)]
        public float OffsetX;

        [Range(-1f, 1f)]
        public float OffsetY;

        public bool IsRelativeOffset = true;

        public bool ZoomWithFOV;

        public bool IgnoreTimeScale;

        #endregion


        #region Properties

        /// <summary>Get ProCamera2D's static instance</summary>
        public static ProCamera2D Instance
        {
            get
            {
                if (Equals(_instance, null))
                {
                    _instance = FindObjectOfType<ProCamera2D>();

                    if (Equals(_instance, null))
                        throw new UnityException("ProCamera2D does not exist.");
                }

                return _instance;
            }
        }

        static ProCamera2D _instance;

        /// <summary>Property to know if there's a ProCamera2D present</summary>
        public static bool Exists { get { return _instance != null; } }

		/// <summary>Is the camera moving?</summary>
		public bool IsMoving 
		{ 
			get 
			{ 
				return 
					Vector3H(_transform.localPosition) != Vector3H(_previousCameraPosition) || 
					Vector3V(_transform.localPosition) != Vector3V(_previousCameraPosition);
			} 
		}

        /// <summary>Update ProCamera2D's camera rect</summary>
        public Rect Rect
        {
            get
            {
                return GameCamera.rect;
            }

            set
            {
                GameCamera.rect = value;
                ProCamera2DParallax parallax = GetComponentInChildren<ProCamera2DParallax>();
                if (parallax != null)
                {
                    for (int i = 0; i < parallax.ParallaxLayers.Count; i++)
                    {
                        parallax.ParallaxLayers[i].ParallaxCamera.rect = value;
                    }
                }
            }
        }

        public Vector2 CameraTargetPositionSmoothed
        { 
            get
            { 
                return new Vector2(_cameraTargetHorizontalPositionSmoothed, _cameraTargetVerticalPositionSmoothed); 
            }

            set
            { 
                _cameraTargetHorizontalPositionSmoothed = value.x;
                _cameraTargetVerticalPositionSmoothed = value.y;
            }
        }

        float _cameraTargetHorizontalPositionSmoothed;
        float _cameraTargetVerticalPositionSmoothed;

        public Vector3 LocalPosition { get { return _transform.localPosition; } set { _transform.localPosition = value; } }

        public Vector2 StartScreenSizeInWorldCoordinates { get; private set; }

		public Vector2 ScreenSizeInWorldCoordinates { get; private set; }

        public Vector3 PreviousTargetsMidPoint { get; private set; }

        public Vector3 TargetsMidPoint { get; private set; }

        public Vector3 CameraTargetPosition { get; private set; }

        public float DeltaTime { get; private set; }

        public Vector3 ParentPosition { get; private set; }

        public Vector3 InfluencesSum { get { return _influencesSum; } }
		Vector3 _influencesSum = Vector3.zero;

        #endregion


        #region Public Variables

        public Action<float> PreMoveUpdate;
        public Action<float> PostMoveUpdate;
        public Action<Vector2> OnCameraResize;
        public Action<float> OnUpdateScreenSizeFinished;
        public Action<float> OnDollyZoomFinished;
        
        public Action OnReset;

        public Vector3? ExclusiveTargetPosition;

        public int CurrentZoomTriggerID;

        public bool IsCameraPositionLeftBounded;
        public bool IsCameraPositionRightBounded;
        public bool IsCameraPositionTopBounded;
        public bool IsCameraPositionBottomBounded;

        public Camera GameCamera;
        
        #endregion


        #region Private Variables

        Func<Vector3, float> Vector3H;
        Func<Vector3, float> Vector3V;
        Func<Vector3, float> Vector3D;
        Func<float, float, Vector3> VectorHV;
        Func<float, float, float, Vector3> VectorHVD;

        Coroutine _updateScreenSizeCoroutine;
        Coroutine _dollyZoomRoutine;

        List<Vector3> _influences = new List<Vector3>();
        
        float _originalCameraDepthSign;

        float _previousCameraTargetHorizontalPositionSmoothed;
        float _previousCameraTargetVerticalPositionSmoothed;
        int _previousScreenWidth;
        int _previousScreenHeight;
		Vector3 _previousCameraPosition;

		WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        Transform _transform;

        List<IPreMover> _preMovers = new List<IPreMover>();
        List<IPositionDeltaChanger> _positionDeltaChangers = new List<IPositionDeltaChanger>();
        List<IPositionOverrider> _positionOverriders = new List<IPositionOverrider>();
        List<ISizeDeltaChanger> _sizeDeltaChangers = new List<ISizeDeltaChanger>();
        List<ISizeOverrider> _sizeOverriders = new List<ISizeOverrider>();
        List<IPostMover> _postMovers = new List<IPostMover>();

        #endregion

        #region MonoBehaviour

        void Awake()
        {
            _instance = this;
            _transform = transform;

            // Get parent position
            if (_transform.parent != null)
                ParentPosition = _transform.parent.position;

            if (GameCamera == null)
                GameCamera = GetComponent<Camera>();
            if (GameCamera == null)
                Debug.LogError("Unity Camera not set and not found on the GameObject: " + gameObject.name);
            
            // Reset the axis functions
            ResetAxisFunctions();

            // Remove empty targets
            for (int i = 0; i < CameraTargets.Count; i++)
            {
                if (CameraTargets[i].TargetTransform == null)
                {
                    CameraTargets.RemoveAt(i);
                }
            }

            // Calculates current screen size
            CalculateScreenSize();
            ResetStartSize();

            // We save this so we know the direction of the camera when moving it on the depth axis
            _originalCameraDepthSign = Mathf.Sign(Vector3D(_transform.localPosition));
        }

        void Start()
        {
            SortPreMovers();
            SortPositionDeltaChangers();
            SortPositionOverriders();
            SortSizeDeltaChangers();
            SortSizeOverriders();
            SortPostMovers();

            // Set some values ahead of the update loop so that other extensions can use them on Awake/Start
            TargetsMidPoint = GetTargetsWeightedMidPoint(ref CameraTargets);
            _cameraTargetHorizontalPositionSmoothed = Vector3H(TargetsMidPoint);
            _cameraTargetVerticalPositionSmoothed = Vector3V(TargetsMidPoint);
            DeltaTime = IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

            // Center on target
            if (CenterTargetOnStart && CameraTargets.Count > 0)
            {
                var targetsMidPoint = GetTargetsWeightedMidPoint(ref CameraTargets);
                var cameraTargetPositionX = FollowHorizontal ? Vector3H(targetsMidPoint) : Vector3H(_transform.localPosition);
                var cameraTargetPositionY = FollowVertical ? Vector3V(targetsMidPoint) : Vector3V(_transform.localPosition);
                var finalPos = new Vector2(cameraTargetPositionX, cameraTargetPositionY);
                finalPos += new Vector2(GetOffsetX() - Vector3H(ParentPosition), GetOffsetY() - Vector3V(ParentPosition));
                MoveCameraInstantlyToPosition(finalPos);
            }
            else
            {
                CameraTargetPosition = _transform.position - ParentPosition;
                _cameraTargetHorizontalPositionSmoothed = Vector3H(CameraTargetPosition);
                _previousCameraTargetHorizontalPositionSmoothed = _cameraTargetHorizontalPositionSmoothed;
                _cameraTargetVerticalPositionSmoothed = Vector3V(CameraTargetPosition);
                _previousCameraTargetVerticalPositionSmoothed = _cameraTargetVerticalPositionSmoothed;
            }
        }

        void LateUpdate()
        {
            if (UpdateType == UpdateType.LateUpdate)
                Move(IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        void FixedUpdate()
        {
            if (UpdateType == UpdateType.FixedUpdate)
                Move(IgnoreTimeScale ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime);
        }

        void OnApplicationQuit()
        {
            _instance = null;
        }

		#endregion


		#region Public Methods

		/// <summary>Returns the current global offset on the horizontal axis</summary>
		public float GetOffsetX()
		{
			return IsRelativeOffset ? OffsetX * ScreenSizeInWorldCoordinates.x * .5f : OffsetX;
		}

		/// <summary>Returns the current global offset on the vertical axis</summary>
		public float GetOffsetY()
		{
			return IsRelativeOffset ? OffsetY * ScreenSizeInWorldCoordinates.y * .5f : OffsetY;
		}
		
        /// <summary>Apply the given influence to the camera during this frame.</summary>
        /// <param name="influence">The vector representing the influence to be applied</param>
        public void ApplyInfluence(Vector2 influence)
        {
            if (!isActiveAndEnabled || Time.deltaTime < .0001f || float.IsNaN(influence.x) || float.IsNaN(influence.y))
                return;

            _influences.Add(VectorHV(influence.x, influence.y));
        }

        /// <summary>Apply the given influences to the camera during the corresponding durations.</summary>
        /// <param name="influences">An array of the vectors representing the influences to be applied</param>
        /// <param name="durations">An array with the durations of the influences to be applied</param>
        public Coroutine ApplyInfluencesTimed(Vector2[] influences, float[] durations)
        {
            return StartCoroutine(ApplyInfluencesTimedRoutine(influences, durations));
        }

        /// <summary>Add a target for the camera to follow.</summary>
        /// <param name="targetTransform">The Transform of the target</param>
        /// <param name="targetInfluenceH">The influence this target horizontal position should have when calculating the average position of all the targets</param>
        /// <param name="targetInfluenceV">The influence this target vertical position should have when calculating the average position of all the targets</param>
        /// <param name="duration">The time it takes for this target to reach it's influence. Use for a more progressive transition.</param>
        /// <param name="targetOffset">A vector that offsets the target position that the camera will follow</param>
        public CameraTarget AddCameraTarget(Transform targetTransform, float targetInfluenceH = 1f, float targetInfluenceV = 1f, float duration = 0f, Vector2 targetOffset = default(Vector2))
        {
            var newCameraTarget = new CameraTarget
            {
                TargetTransform = targetTransform,
                TargetInfluenceH = targetInfluenceH,
                TargetInfluenceV = targetInfluenceV,
                TargetOffset = targetOffset
            };

            CameraTargets.Add(newCameraTarget);

            if (duration > 0f)
            {
                newCameraTarget.TargetInfluence = 0f;
                StartCoroutine(AdjustTargetInfluenceRoutine(newCameraTarget, targetInfluenceH, targetInfluenceV, duration));
            }

            return newCameraTarget;
        }

        /// <summary>Add multiple targets for the camera to follow.</summary>
        /// <param name="targetsTransforms">An array or list with the new targets</param>
        /// <param name="targetsInfluenceH">The influence the targets horizontal position should have when calculating the average position of all the targets</param>
        /// <param name="targetsInfluenceV">The influence the targets vertical position should have when calculating the average position of all the targets</param>
        /// <param name="duration">The time it takes for the targets to reach their influence. Use for a more progressive transition.</param>
        /// <param name="targetOffset">A vector that offsets the target position that the camera will follow</param>
        public void AddCameraTargets(IList<Transform> targetsTransforms, float targetsInfluenceH = 1f, float targetsInfluenceV = 1f, float duration = 0f, Vector2 targetOffset = default(Vector2))
        {
            for (int i = 0; i < targetsTransforms.Count; i++)
            {
                AddCameraTarget(targetsTransforms[i], targetsInfluenceH, targetsInfluenceV, duration, targetOffset);
            }
        }

		/// <summary>Add multiple targets for the camera to follow.</summary>
		/// <param name="cameraTargets">An array or list with the new targets</param>
		public void AddCameraTargets(IList<CameraTarget> cameraTargets)
		{
			CameraTargets.AddRange(cameraTargets);
		}

        /// <summary>Gets the corresponding CameraTarget from an object's transform.</summary>
        /// <param name="targetTransform">The Transform of the target</param>
        public CameraTarget GetCameraTarget(Transform targetTransform)
        {
            for (int i = 0; i < CameraTargets.Count; i++)
            {
                if (CameraTargets[i].TargetTransform.GetInstanceID() == targetTransform.GetInstanceID())
                {
                    return CameraTargets[i];
                }
            }
            return null;
        }

        /// <summary>Remove a target from the camera.</summary>
        /// <param name="targetTransform">The Transform of the target</param>
        /// <param name="duration">The time it takes for this target to reach a zero influence. Use for a more progressive transition.</param>
        public void RemoveCameraTarget(Transform targetTransform, float duration = 0f)
        {
            for (int i = 0; i < CameraTargets.Count; i++)
            {
                if (CameraTargets[i].TargetTransform.GetInstanceID() == targetTransform.GetInstanceID())
                {
                    if (duration > 0)
                    {
                        StartCoroutine(AdjustTargetInfluenceRoutine(CameraTargets[i], 0, 0, duration, true));
                    }
                    else
                        CameraTargets.Remove(CameraTargets[i]);
                }
            }
        }

        /// <summary>Removes all targets from the camera.</summary>
        /// <param name="duration">The time it takes for all targets to reach a zero influence. Use for a more progressive transition.</param>
        public void RemoveAllCameraTargets(float duration = 0f)
        {
            if (duration == 0)
            {
                CameraTargets.Clear();
            }
            else
            {
                for (int i = 0; i < CameraTargets.Count; i++)
                {
                    StartCoroutine(AdjustTargetInfluenceRoutine(CameraTargets[i], 0, 0, duration, true));
                }
            }
        }

        /// <summary>Adjusts a target influence</summary>
        /// <param name="cameraTarget">The CameraTarget of the target</param>
        /// <param name="targetInfluenceH">The influence this target horizontal position should have when calculating the average position of all the targets</param>
        /// <param name="targetInfluenceV">The influence this target vertical position should have when calculating the average position of all the targets</param>
        /// <param name="duration">The time it takes for this target to reach it's influence. Don't use a duration if calling every frame.</param>
        public Coroutine AdjustCameraTargetInfluence(CameraTarget cameraTarget, float targetInfluenceH, float targetInfluenceV, float duration = 0)
        {
            if (duration > 0)
                return StartCoroutine(AdjustTargetInfluenceRoutine(cameraTarget, targetInfluenceH, targetInfluenceV, duration));
            else
            {
                cameraTarget.TargetInfluenceH = targetInfluenceH;
                cameraTarget.TargetInfluenceV = targetInfluenceV;

                return null;
            }
        }

        /// <summary>Adjusts a target influence, finding it first by its transform.</summary>
        /// <param name="cameraTargetTransf">The Transform of the target</param>
        /// <param name="targetInfluenceH">The influence this target horizontal position should have when calculating the average position of all the targets</param>
        /// <param name="targetInfluenceV">The influence this target vertical position should have when calculating the average position of all the targets</param>
        /// <param name="duration">The time it takes for this target to reach it's influence. Don't use a duration if calling every frame.</param>
        public Coroutine AdjustCameraTargetInfluence(Transform cameraTargetTransf, float targetInfluenceH, float targetInfluenceV, float duration = 0)
        {
            var cameraTarget = GetCameraTarget(cameraTargetTransf);

            if (cameraTarget == null)
                return null;

            return AdjustCameraTargetInfluence(cameraTarget, targetInfluenceH, targetInfluenceV, duration);
        }
        
        /// <summary>Translates the camera by the specified amount while maintaining internal values</summary>
        /// <param name="translateAmount">The camera will be offset by this amount</param>
        public void TranslateCamera(Vector2 translateAmount)
        {
            var currentPos = _transform.localPosition;
            var newPos = currentPos + VectorHVD(translateAmount.x, translateAmount.y, Vector3D(currentPos));

            _transform.localPosition = newPos;
            CameraTargetPosition = newPos;
            TargetsMidPoint = newPos;
            PreviousTargetsMidPoint = newPos;
 
            _cameraTargetHorizontalPositionSmoothed += translateAmount.x;
            _cameraTargetVerticalPositionSmoothed += translateAmount.y;
 
            _previousCameraTargetHorizontalPositionSmoothed += translateAmount.x;
            _previousCameraTargetVerticalPositionSmoothed += translateAmount.y;
        }

        /// <summary>Moves the camera instantly to the supplied position</summary>
        /// <param name="cameraPos">The final position of the camera</param>
        public void MoveCameraInstantlyToPosition(Vector2 cameraPos)
        {
            _transform.localPosition = VectorHVD(cameraPos.x, cameraPos.y, Vector3D(_transform.localPosition));

            ResetMovement();
        }

        /// <summary>Resets the camera movement and size and also all of its extensions to their start values.
        /// This could be useful if, for example, your player dies and respawns somewhere else on the level</summary>
        /// <param name="centerOnTargets">If true, the camera will move to the "final" targets position</param>
        /// <param name="resetSize">If true, resets the camera size to the start value</param>
        /// <param name="resetExtensions">If true, resets all active extensions to their start values</param>
        public void Reset(bool centerOnTargets = true, bool resetSize = true, bool resetExtensions = true)
        {
            if (centerOnTargets)
                CenterOnTargets();
            else
                ResetMovement();

            if(resetSize)
                ResetSize();

            if (resetExtensions)
                ResetExtensions();
        }

        /// <summary>
        /// Cancels any existing camera movement easing. 
        /// Also check CenterOnTargets and MoveCameraInstantlyToPosition.
        /// </summary>
        public void ResetMovement()
        {
            CameraTargetPosition = _transform.localPosition;

            _cameraTargetHorizontalPositionSmoothed = Vector3H(CameraTargetPosition);
            _cameraTargetVerticalPositionSmoothed = Vector3V(CameraTargetPosition);

            _previousCameraTargetHorizontalPositionSmoothed = _cameraTargetHorizontalPositionSmoothed;
            _previousCameraTargetVerticalPositionSmoothed = _cameraTargetVerticalPositionSmoothed;

            TargetsMidPoint = CameraTargetPosition;
            PreviousTargetsMidPoint = TargetsMidPoint;
        }

        /// <summary>
        /// Resets the camera size to the start value.
        /// </summary>
        public void ResetSize()
        {
            SetScreenSize(StartScreenSizeInWorldCoordinates.y / 2);
        }

		/// <summary>
		/// Resets the start camera size that is used for some calculations.
		/// </summary>
		public void ResetStartSize(Vector2 newSize = default(Vector2))
		{
			if(newSize != default(Vector2))
				StartScreenSizeInWorldCoordinates = newSize;
			else
				StartScreenSizeInWorldCoordinates = Utils.GetScreenSizeInWorldCoords(GameCamera, Mathf.Abs(Vector3D(_transform.localPosition)));
		}

        /// <summary>
        /// Resets all active extensions to their start values.
        /// Notice you can manually reset each extension using the "OnReset" method.
        /// </summary>
        public void ResetExtensions()
        {
            if (OnReset != null)
                OnReset();
        }

        /// <summary>Instantly moves the camera to the targets' position.</summary>
        public void CenterOnTargets()
        {
            var targetsMidPoint = GetTargetsWeightedMidPoint(ref CameraTargets);
            var finalPos = new Vector2(Vector3H(targetsMidPoint), Vector3V(targetsMidPoint));
            finalPos += new Vector2(GetOffsetX(), GetOffsetY());
            MoveCameraInstantlyToPosition(finalPos);
        }

        /// <summary>Resize the camera to the supplied size</summary>
        /// <param name="newSize">Half of the wanted size in world units</param>
        /// <param name="duration">How long it should take to reach the provided size. Use 0 if instant or calling repeatedly</param>
        /// <param name="easeType">The easing method to apply. Only used when the duration is bigger than 0</param>
        public void UpdateScreenSize(float newSize, float duration = 0f, EaseType easeType = EaseType.EaseInOut)
        {
            if (!enabled)
                return;

            if (_updateScreenSizeCoroutine != null)
                StopCoroutine(_updateScreenSizeCoroutine);

            if (duration > 0)
                _updateScreenSizeCoroutine = StartCoroutine(UpdateScreenSizeRoutine(newSize, duration, easeType));
            else
                SetScreenSize(newSize);
        }

        /// <summary>Stops the UpdateScreenSize coroutine.</summary>
        public void StopUpdateScreenSizeCoroutine()
        {
            if (_updateScreenSizeCoroutine != null)
                StopCoroutine(_updateScreenSizeCoroutine);
        }

		/// <summary>Recalculates the camera size in world coordinates. Call only if you change the camera size manually outside of ProCamera2D.</summary>
        public void CalculateScreenSize()
        {
            GameCamera.ResetAspect();
            ScreenSizeInWorldCoordinates = Utils.GetScreenSizeInWorldCoords(GameCamera, Mathf.Abs(Vector3D(_transform.localPosition)));
            _previousScreenWidth = Screen.width;
            _previousScreenHeight = Screen.height;
        }

        /// <summary>Zoom in or out the camera by the supplied amount</summary>
        /// <param name="zoomAmount">The amount to zoom in world units</param>
        /// <param name="duration">How long it should take to reach the new zoom. Use 0 if instant or calling repeatedly</param>
        /// <param name="easeType">The easing method to apply. Only used when the duration is bigger than 0</param>
        public void Zoom(float zoomAmount, float duration = 0f, EaseType easeType = EaseType.EaseInOut)
        {
            UpdateScreenSize(ScreenSizeInWorldCoordinates.y * .5f + zoomAmount, duration, easeType);
        }


        /// <summary>Creates a dolly zoom effect, also known as the Hitchcock effect</summary>
        /// <param name="targetFOV">The final field of view</param>
        /// <param name="duration">The duration of the effect</param>
        /// <param name="easeType">The ease type of the transition</param>
        public void DollyZoom(float targetFOV, float duration = 1f, EaseType easeType = EaseType.EaseInOut)
        {
            if (!enabled)
                return;
            
            if (GameCamera.orthographic)
            {
                Debug.LogWarning("Dolly zooming is only supported on perspective cameras");
                return;
            }

            if (_dollyZoomRoutine != null)
                StopCoroutine(_dollyZoomRoutine);

            targetFOV = Mathf.Clamp(targetFOV, 0.1f, 179.9f);

            if (duration <= 0)
            {
                GameCamera.fieldOfView = targetFOV;

                _transform.localPosition = VectorHVD(
                    Vector3H(_transform.localPosition), 
                    Vector3V(_transform.localPosition), 
                    GetCameraDistanceForFOV(GameCamera.fieldOfView, ScreenSizeInWorldCoordinates.y) * _originalCameraDepthSign);
            }
            else
            {
                StartCoroutine(DollyZoomRoutine(targetFOV, duration, easeType));
            }
        }

        /// <summary>
        /// Move the camera to the average position of all the targets.
        /// This method is automatically called when using LateUpdate or FixedUpdate.
        /// If using ManualUpdate, you have to call it yourself.
        /// </summary>
        /// <param name="deltaTime">The time in seconds it took to complete the last frame</param>
        public void Move(float deltaTime)
        {
			// Save previous camera position
			_previousCameraPosition = _transform.localPosition;

            //Detect resolution changes
            if (Screen.width != _previousScreenWidth || Screen.height != _previousScreenHeight)
                CalculateScreenSize();

            // Delta time
            DeltaTime = deltaTime;
            if (DeltaTime < .0001f)
                return;
                
            // Pre-Move update
            if (PreMoveUpdate != null)
                PreMoveUpdate(DeltaTime);

            // Cycle through the pre movers
            for (int i = 0; i < _preMovers.Count; i++)
            {
                _preMovers[i].PreMove(deltaTime);
            }

            // Calculate targets mid point
            PreviousTargetsMidPoint = TargetsMidPoint;
            TargetsMidPoint = GetTargetsWeightedMidPoint(ref CameraTargets);
            CameraTargetPosition = TargetsMidPoint;

            // Calculate influences
            _influencesSum = Utils.GetVectorsSum(_influences);
            CameraTargetPosition += _influencesSum;
            _influences.Clear();

            // Follow only on selected axis
            var cameraTargetPositionX = FollowHorizontal ? Vector3H(CameraTargetPosition) : Vector3H(_transform.localPosition);
            var cameraTargetPositionY = FollowVertical ? Vector3V(CameraTargetPosition) : Vector3V(_transform.localPosition);
            CameraTargetPosition = VectorHV(cameraTargetPositionX - Vector3H(ParentPosition), cameraTargetPositionY - Vector3V(ParentPosition));

            // Ignore targets and influences if exclusive position is set
            if (ExclusiveTargetPosition.HasValue)
            {
                CameraTargetPosition = VectorHV(Vector3H(ExclusiveTargetPosition.Value) - Vector3H(ParentPosition), Vector3V(ExclusiveTargetPosition.Value) - Vector3V(ParentPosition));
                ExclusiveTargetPosition = null;
            }

            // Add offset
            CameraTargetPosition += VectorHV(FollowHorizontal ? GetOffsetX() : 0, FollowVertical ? GetOffsetY() : 0);

            // Tween camera final position
            _cameraTargetHorizontalPositionSmoothed = Utils.SmoothApproach(_cameraTargetHorizontalPositionSmoothed, _previousCameraTargetHorizontalPositionSmoothed, Vector3H(CameraTargetPosition), 1f / HorizontalFollowSmoothness, DeltaTime);
            _previousCameraTargetHorizontalPositionSmoothed = _cameraTargetHorizontalPositionSmoothed;

            _cameraTargetVerticalPositionSmoothed = Utils.SmoothApproach(_cameraTargetVerticalPositionSmoothed, _previousCameraTargetVerticalPositionSmoothed, Vector3V(CameraTargetPosition), 1f / VerticalFollowSmoothness, DeltaTime);
            _previousCameraTargetVerticalPositionSmoothed = _cameraTargetVerticalPositionSmoothed;

            // Movement this step
            var horizontalDeltaMovement = _cameraTargetHorizontalPositionSmoothed - Vector3H(_transform.localPosition);
            var verticalDeltaMovement = _cameraTargetVerticalPositionSmoothed - Vector3V(_transform.localPosition);

            // Calculate the base delta movement
            var deltaMovement = VectorHV(horizontalDeltaMovement, verticalDeltaMovement);



            // Cycle through the size delta changers
            var deltaSize = 0f;
            for (int i = 0; i < _sizeDeltaChangers.Count; i++)
            {
                deltaSize = _sizeDeltaChangers[i].AdjustSize(deltaTime, deltaSize);
            }

            // Calculate the new size
            var newSize = ScreenSizeInWorldCoordinates.y * .5f + deltaSize;

            // Cycle through the size overriders
            for (int i = 0; i < _sizeOverriders.Count; i++)
            {
                newSize = _sizeOverriders[i].OverrideSize(deltaTime, newSize);
            }

            // Apply the new size
            if (newSize != ScreenSizeInWorldCoordinates.y * .5f)
                SetScreenSize(newSize);



            // Cycle through the position delta changers
            for (int i = 0; i < _positionDeltaChangers.Count; i++)
            {
                deltaMovement = _positionDeltaChangers[i].AdjustDelta(deltaTime, deltaMovement);
            }

            // Calculate the new position
            var newPos = LocalPosition + deltaMovement;

            // Cycle through the position overriders
            for (int i = 0; i < _positionOverriders.Count; i++)
            {
                newPos = _positionOverriders[i].OverridePosition(deltaTime, newPos);
            }

            // Apply the new position
            _transform.localPosition = VectorHVD(Vector3H(newPos), Vector3V(newPos), Vector3D(_transform.localPosition));




            // Cycle through the post movers
            for (int i = 0; i < _postMovers.Count; i++)
            {
                _postMovers[i].PostMove(deltaTime);
            }
            
            // Post-Move update
            if (PostMoveUpdate != null)
                PostMoveUpdate(DeltaTime);
        }

        /// <summary>For internal use</summary>
        internal YieldInstruction GetYield()
        {
            switch (UpdateType)
            {
                case UpdateType.FixedUpdate:
                    return IgnoreTimeScale ? null : _waitForFixedUpdate;

                default:
                    return null;
            }
        }

        #endregion


        #region Private Methods

        void ResetAxisFunctions()
        {
            switch (Axis)
            {
                case MovementAxis.XY:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.y;
                    Vector3D = vector => vector.z;
                    VectorHV = (h, v) => new Vector3(h, v, 0);
                    VectorHVD = (h, v, d) => new Vector3(h, v, d);
                    break;
                case MovementAxis.XZ:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.z;
                    Vector3D = vector => vector.y;
                    VectorHV = (h, v) => new Vector3(h, 0, v);
                    VectorHVD = (h, v, d) => new Vector3(h, d, v);
                    break;
                case MovementAxis.YZ:
                    Vector3H = vector => vector.z;
                    Vector3V = vector => vector.y;
                    Vector3D = vector => vector.x;
                    VectorHV = (h, v) => new Vector3(0, v, h);
                    VectorHVD = (h, v, d) => new Vector3(d, v, h);
                    break;
            } 
        }

        Vector3 GetTargetsWeightedMidPoint(ref List<CameraTarget> targets)
        {
            var midPointH = 0f;
            var midPointV = 0f;

            if (targets.Count == 0)
                return transform.localPosition;

            var totalInfluencesH = 0f;
            var totalInfluencesV = 0f;
            var totalAccountableTargetsH = 0;
            var totalAccountableTargetsV = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null || targets[i].TargetTransform == null)
                {
                    targets.RemoveAt(i);
                    continue;
                }

                midPointH += (Vector3H(targets[i].TargetPosition) + targets[i].TargetOffset.x) * targets[i].TargetInfluenceH;
                midPointV += (Vector3V(targets[i].TargetPosition) + targets[i].TargetOffset.y) * targets[i].TargetInfluenceV;

                totalInfluencesH += targets[i].TargetInfluenceH;
                totalInfluencesV += targets[i].TargetInfluenceV;

                if (targets[i].TargetInfluenceH > 0)
                    totalAccountableTargetsH++;

                if (targets[i].TargetInfluenceV > 0)
                    totalAccountableTargetsV++;
            }

            if (totalInfluencesH < 1 && totalAccountableTargetsH == 1)
                totalInfluencesH += (1 - totalInfluencesH);

            if (totalInfluencesV < 1 && totalAccountableTargetsV == 1)
                totalInfluencesV += (1 - totalInfluencesV);

            if (totalInfluencesH > .0001f)
                midPointH /= totalInfluencesH;

            if (totalInfluencesV > .0001f)
                midPointV /= totalInfluencesV;

            return VectorHV(midPointH, midPointV);
        }

        IEnumerator ApplyInfluencesTimedRoutine(IList<Vector2> influences, float[] durations)
        {
            var count = -1;
            while (count < durations.Length - 1)
            {
                count++;
                var duration = durations[count];

                yield return StartCoroutine(ApplyInfluenceTimedRoutine(influences[count], duration));
            }
        }

        IEnumerator ApplyInfluenceTimedRoutine(Vector2 influence, float duration)
        {
            while (duration > 0)
            {
                duration -= DeltaTime;

                ApplyInfluence(influence);

                yield return GetYield();
            }
        }

        IEnumerator AdjustTargetInfluenceRoutine(CameraTarget cameraTarget, float influenceH, float influenceV, float duration, bool removeIfZeroInfluence = false)
        {
            var startInfluenceH = cameraTarget.TargetInfluenceH;
            var startInfluenceV = cameraTarget.TargetInfluenceV;

            var t = 0f;
            while (t <= 1.0f)
            {
                t += DeltaTime / duration;
                cameraTarget.TargetInfluenceH = Utils.EaseFromTo(startInfluenceH, influenceH, t, EaseType.Linear);
                cameraTarget.TargetInfluenceV = Utils.EaseFromTo(startInfluenceV, influenceV, t, EaseType.Linear);

                yield return GetYield();
            }

            if (removeIfZeroInfluence && cameraTarget.TargetInfluenceH <= 0 && cameraTarget.TargetInfluenceV <= 0)
                CameraTargets.Remove(cameraTarget);
        }

        IEnumerator UpdateScreenSizeRoutine(float finalSize, float duration, EaseType easeType)
        {
            var startSize = ScreenSizeInWorldCoordinates.y * .5f;
            var newSize = startSize;

            var t = 0f;
            while (t <= 1.0f)
            {
                t += DeltaTime / duration;
                newSize = Utils.EaseFromTo(startSize, finalSize, t, easeType);

                SetScreenSize(newSize);

                yield return GetYield();
            }

            _updateScreenSizeCoroutine = null;

            if (OnUpdateScreenSizeFinished != null)
                OnUpdateScreenSizeFinished(newSize);
        }

        IEnumerator DollyZoomRoutine(float finalFOV, float duration, EaseType easeType)
        {
            var startFOV = GameCamera.fieldOfView;
            var newFOV = startFOV;

            var t = 0f;
            while (t <= 1.0f)
            {
                t += DeltaTime / duration;

                newFOV = Utils.EaseFromTo(startFOV, finalFOV, t, easeType);
                GameCamera.fieldOfView = newFOV;

                _transform.localPosition = VectorHVD(
                    Vector3H(_transform.localPosition), 
                    Vector3V(_transform.localPosition), 
                    GetCameraDistanceForFOV(newFOV, ScreenSizeInWorldCoordinates.y) * _originalCameraDepthSign);

                yield return GetYield();
            }

            _dollyZoomRoutine = null;

            if (OnDollyZoomFinished != null)
                OnDollyZoomFinished(newFOV);
            
            if (OnUpdateScreenSizeFinished != null)
                OnUpdateScreenSizeFinished(ScreenSizeInWorldCoordinates.y * .5f);
        }

        void SetScreenSize(float newSize)
        {
            #if UNITY_EDITOR
            if (_transform == null)
                _transform = transform;
            #endif

            if (GameCamera.orthographic)
            {
                newSize = Mathf.Max(newSize, .1f);

                GameCamera.orthographicSize = newSize;
            }
            else
            {
                if (ZoomWithFOV)
                {
                    var newFieldOfView = 2f * Mathf.Atan(newSize / Mathf.Abs(Vector3D(_transform.localPosition))) * Mathf.Rad2Deg;
                    GameCamera.fieldOfView = Mathf.Clamp(newFieldOfView, .1f, 179.9f);
                }
                else
                {
                    _transform.localPosition = VectorHVD(
                        Vector3H(_transform.localPosition), 
                        Vector3V(_transform.localPosition), 
                        (newSize / Mathf.Tan(GameCamera.fieldOfView * 0.5f * Mathf.Deg2Rad)) * _originalCameraDepthSign);
                }
            }

            ScreenSizeInWorldCoordinates = new Vector2(newSize * 2f * GameCamera.aspect, newSize * 2f);

            OnCameraResize?.Invoke(ScreenSizeInWorldCoordinates);
        }

        float GetCameraDistanceForFOV(float fov, float cameraHeight)
        {
            return cameraHeight / (2f * Mathf.Tan(0.5f * fov * Mathf.Deg2Rad));
        }

        #endregion

        #region Extension Interfaces

        public void AddPreMover(IPreMover mover)
        {
            _preMovers.Add(mover);
        }

        public void RemovePreMover(IPreMover mover)
        {
            _preMovers.Remove(mover);
        }

        public void SortPreMovers()
        {
            _preMovers = _preMovers.OrderBy(a => a.PrMOrder).ToList();
        }

        public void AddPositionDeltaChanger(IPositionDeltaChanger changer)
        {
            _positionDeltaChangers.Add(changer);
        }

        public void RemovePositionDeltaChanger(IPositionDeltaChanger changer)
        {
            _positionDeltaChangers.Remove(changer);
        }

        public void SortPositionDeltaChangers()
        {
            _positionDeltaChangers = _positionDeltaChangers.OrderBy(a => a.PDCOrder).ToList();
        }

        public void AddPositionOverrider(IPositionOverrider overrider)
        {
            _positionOverriders.Add(overrider);
        }

        public void RemovePositionOverrider(IPositionOverrider overrider)
        {
            _positionOverriders.Remove(overrider);
        }

        public void SortPositionOverriders()
        {
            _positionOverriders = _positionOverriders.OrderBy(a => a.POOrder).ToList();
        }

        public void AddSizeDeltaChanger(ISizeDeltaChanger changer)
        {
            _sizeDeltaChangers.Add(changer);
        }

        public void RemoveSizeDeltaChanger(ISizeDeltaChanger changer)
        {
            _sizeDeltaChangers.Remove(changer);
        }

        public void SortSizeDeltaChangers()
        {
            _sizeDeltaChangers = _sizeDeltaChangers.OrderBy(a => a.SDCOrder).ToList();
        }

        public void AddSizeOverrider(ISizeOverrider overrider)
        {
            _sizeOverriders.Add(overrider);
        }

        public void RemoveSizeOverrider(ISizeOverrider overrider)
        {
            _sizeOverriders.Remove(overrider);
        }

        public void SortSizeOverriders()
        {
            _sizeOverriders = _sizeOverriders.OrderBy(a => a.SOOrder).ToList();
        }

        public void AddPostMover(IPostMover mover)
        {
            _postMovers.Add(mover);
        }

        public void RemovePostMover(IPostMover mover)
        {
            _postMovers.Remove(mover);
        }

        public void SortPostMovers()
        {
            _postMovers = _postMovers.OrderBy(a => a.PMOrder).ToList();
        }

        #endregion


        #region ISerializationCallbackReceiver implementation

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            ResetAxisFunctions();
        }

        #endregion

        #if UNITY_EDITOR
        int _drawGizmosCounter;

        void OnDrawGizmos()
        {
            // HACK to prevent Unity bug on startup: http://forum.unity3d.com/threads/screen-position-out-of-view-frustum.9918/
            _drawGizmosCounter++;
            if (_drawGizmosCounter < 5 && UnityEditor.EditorApplication.timeSinceStartup < 60f)
                return;

            if (Vector3H == null)
                ResetAxisFunctions();

            var gameCamera = GetComponent<Camera>();

            // Don't draw gizmos on other cameras
            if (Camera.current != gameCamera &&
                ((UnityEditor.SceneView.lastActiveSceneView != null && Camera.current != UnityEditor.SceneView.lastActiveSceneView.camera) ||
                (UnityEditor.SceneView.lastActiveSceneView == null)))
                return;

            var cameraDimensions = Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(transform.position)));
            float cameraDepthOffset = Vector3D(transform.position) + Mathf.Abs(Vector3D(transform.position)) * Vector3D(transform.forward);

            // Targets mid point
            Gizmos.color = EditorPrefsX.GetColor(PrefsData.TargetsMidPointColorKey, PrefsData.TargetsMidPointColorValue);
            var targetsMidPoint = GetTargetsWeightedMidPoint(ref CameraTargets);
            targetsMidPoint = VectorHVD(Vector3H(targetsMidPoint), Vector3V(targetsMidPoint), cameraDepthOffset);
            Gizmos.DrawWireSphere(targetsMidPoint, .01f * cameraDimensions.y);

            // Influences sum
            Gizmos.color = EditorPrefsX.GetColor(PrefsData.InfluencesColorKey, PrefsData.InfluencesColorValue);
            if (_influencesSum != Vector3.zero)
                Utils.DrawArrowForGizmo(targetsMidPoint, _influencesSum, .04f * cameraDimensions.y);

            // Overall offset line
            Gizmos.color = EditorPrefsX.GetColor(PrefsData.OverallOffsetColorKey, PrefsData.OverallOffsetColorValue);
            if (OffsetX != 0 || OffsetY != 0)
            {
                if(IsRelativeOffset)
                    Utils.DrawArrowForGizmo(targetsMidPoint, VectorHV((OffsetX * cameraDimensions.x * .5f), (OffsetY * cameraDimensions.y * .5f)), .04f * cameraDimensions.y);
                else
                    Utils.DrawArrowForGizmo(targetsMidPoint, VectorHV(OffsetX, OffsetY), .04f * cameraDimensions.y);
            }

            // Camera target position
            Gizmos.color = EditorPrefsX.GetColor(PrefsData.CamTargetPositionColorKey, PrefsData.CamTargetPositionColorValue);
            var cameraTargetPosition = targetsMidPoint + _influencesSum + VectorHV((OffsetX * cameraDimensions.x * .5f), (OffsetY * cameraDimensions.y * .5f));
            
            if(!IsRelativeOffset)
                cameraTargetPosition = targetsMidPoint + _influencesSum + VectorHV(OffsetX, OffsetY);
    
            var cameraTargetPos = VectorHVD(Vector3H(cameraTargetPosition), Vector3V(cameraTargetPosition), cameraDepthOffset);
            Gizmos.DrawWireSphere(cameraTargetPos, .015f * cameraDimensions.y);

            // Camera target position smoothed
            if (Application.isPlaying)
            {
                Gizmos.color = EditorPrefsX.GetColor(PrefsData.CamTargetPositionSmoothedColorKey, PrefsData.CamTargetPositionSmoothedColorValue);
                var cameraTargetPosSmoothed = VectorHVD(_cameraTargetHorizontalPositionSmoothed + Vector3H(ParentPosition), _cameraTargetVerticalPositionSmoothed + Vector3V(ParentPosition), cameraDepthOffset);
                Gizmos.DrawWireSphere(cameraTargetPosSmoothed, .02f * cameraDimensions.y);
                Gizmos.DrawLine(cameraTargetPos, cameraTargetPosSmoothed);
            }

            // Current camera position
            Gizmos.color = EditorPrefsX.GetColor(PrefsData.CurrentCameraPositionColorKey, PrefsData.CurrentCameraPositionColorValue);
            var currentCameraPos = VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset);
            Gizmos.DrawWireSphere(currentCameraPos, .025f * cameraDimensions.y);
        }
        #endif
    }
}