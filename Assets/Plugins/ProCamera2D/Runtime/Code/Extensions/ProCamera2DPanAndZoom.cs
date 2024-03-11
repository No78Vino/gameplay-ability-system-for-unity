using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-pan-and-zoom/")]
	public class ProCamera2DPanAndZoom : BasePC2D, IPreMover
	{
		public enum MouseButton
		{
			Left = 0,
			Right = 1,
			Middle = 2
		}

		public static string ExtensionName = "Pan And Zoom";

		// Events
		public Action OnPanStarted;
		public Action OnPanFinished;
		
		// Input
		public bool AutomaticInputDetection = true;
		public bool UseMouseInput;
		public bool UseTouchInput;

		public bool DisableOverUGUI = true;

		// Zoom
		public bool AllowZoom = true;

		public float MouseZoomSpeed = 10f;
		public float PinchZoomSpeed = 50f;

		[Range(0, 2f)]
		public float ZoomSmoothness = .2f;

		public float MaxZoomInAmount = 2f;
		public float MaxZoomOutAmount = 2f;

		public bool ZoomToInputCenter = true;

		[HideInInspector]
		public bool IsZooming;

		float _zoomAmount;

		float _initialCamSize;

		bool _zoomStarted;
		float _origFollowSmoothnessX;
		float _origFollowSmoothnessY;

		float _prevZoomAmount;
		float _zoomVelocity;

		Vector3 _zoomPoint;

		float _touchZoomTime;

		// Pan
		public bool AllowPan = true;

		public bool UsePanByDrag = true;

		[Range(0f, 1f)]
		public float StopSpeedOnDragStart = .95f;

		public Rect DraggableAreaRect = new Rect(0f, 0f, 1f, 1f);

		public Vector2 DragPanSpeedMultiplier = new Vector2(1f, 1f);

		public bool UsePanByMoveToEdges = false;

		public Vector2 EdgesPanSpeed = new Vector2(2f, 2f);

		[Range(0, .99f)]
		public float TopPanEdge = .9f;

		[Range(0, .99f)]
		public float BottomPanEdge = .9f;

		[Range(0, .99f)]
		public float LeftPanEdge = .9f;

		[Range(0, .99f)]
		public float RightPanEdge = .9f;

		public MouseButton PanMouseButton;
		
		public float MinPanAmount = .05f;

		[HideInInspector]
		public bool ResetPrevPanPoint;

		[HideInInspector]
		public bool IsPanning;

		Vector2 _panDelta;

		Transform _panTarget;

		Vector3 _prevMousePosition;
		Vector3 _prevTouchPosition;
		int _prevTouchId;

		bool _onMaxZoom;
		bool _onMinZoom;
		EventSystem _eventSystem;
		bool _skip;

		Vector3 _startPanWorldPos;

		protected override void Awake()
		{
			base.Awake();

			if (AutomaticInputDetection)
			{
				UseMouseInput = !Input.touchSupported;
				UseTouchInput = Input.touchSupported;
			}

			UpdateCurrentFollowSmoothness();

			_eventSystem = EventSystem.current;

			_panTarget = new GameObject("PC2DPanTarget").transform;

			ProCamera2D.AddPreMover(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if(ProCamera2D)
				ProCamera2D.RemovePreMover(this);
		}

		IEnumerator Start()
		{
			_initialCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;

			yield return null;
			
			if(gameObject.scene.buildIndex == -1)
				DontDestroyOnLoad(_panTarget.gameObject);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			_initialCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;

			ProCamera2D.AddCameraTarget(_panTarget);

			CenterPanTargetOnCamera();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			ResetPrevPanPoint = true;
			_onMaxZoom = false;
			_onMinZoom = false;

			ProCamera2D.RemoveCameraTarget(_panTarget);
		}

		#region IPreMover implementation

		public void PreMove(float deltaTime)
		{
			// Detect if the user is pointing over an UI element
			if (UseTouchInput)
			{
				_skip = DisableOverUGUI && _eventSystem;
				if (_skip)
				{
					_skip = false;
					for (int i = 0; i < Input.touchCount; i++)
					{
						if (_eventSystem.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
						{
							_skip = true;
							break;
						}
					}
				}
				if (_skip)
				{
					_prevTouchPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Mathf.Abs(Vector3D(ProCamera2D.LocalPosition)));
					CancelZoom();
				}
			}
			
			if (UseMouseInput)
			{
				_skip = DisableOverUGUI && _eventSystem && _eventSystem.IsPointerOverGameObject();
				if (_skip)
				{
					_prevMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Vector3D(ProCamera2D.LocalPosition)));
					CancelZoom();
				}
			}

			IsZooming = false;

			if (enabled && AllowPan && !_skip)
				Pan(deltaTime);

			if (enabled && AllowZoom && !_skip)
				Zoom(deltaTime);
		}

		public int PrMOrder { get { return _prmOrder; } set { _prmOrder = value; } }

		int _prmOrder = 0;

		#endregion

		void Pan(float deltaTime)
		{
			_panDelta = Vector2.zero;

			if (UseTouchInput)
			{
				// Time since zoom
				if (Time.time - _touchZoomTime < .1f)
				{
					if (Input.touchCount > 0)
						_prevTouchPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Mathf.Abs(Vector3D(ProCamera2D.LocalPosition)));

					return;
				}

				// Touch delta
				if (AllowZoom && Input.touchCount == 1 ||
					!AllowZoom && Input.touchCount > 0)
				{
					var touch = Input.GetTouch(Input.touchCount - 1);

					if (touch.phase == TouchPhase.Began)
					{
						_prevTouchId = touch.fingerId;
						_prevTouchPosition = new Vector3(touch.position.x, touch.position.y, Mathf.Abs(Vector3D(ProCamera2D.LocalPosition)));

						_startPanWorldPos = ProCamera2D.GameCamera.ScreenToWorldPoint(_prevTouchPosition);
					}

					// Ignore if using different finger or touch not moving
					if (touch.fingerId != _prevTouchId || touch.phase != TouchPhase.Moved)
						return;

					var touchPos = new Vector3(touch.position.x, touch.position.y, Mathf.Abs(Vector3D(ProCamera2D.LocalPosition)));
					var normalizedTouchPos = new Vector2(touch.position.x / ProCamera2D.GameCamera.pixelWidth, touch.position.y / ProCamera2D.GameCamera.pixelHeight);

					if (ProCamera2D.GameCamera.pixelRect.Contains(touchPos) && InsideDraggableArea(normalizedTouchPos))
					{
						var prevTouchPositionWorldCoord = ProCamera2D.GameCamera.ScreenToWorldPoint(_prevTouchPosition);
						var currentTouchPositionWorldCoord = ProCamera2D.GameCamera.ScreenToWorldPoint(touchPos);
						
						if (IsPanning)
						{
							if (ResetPrevPanPoint)
							{
								prevTouchPositionWorldCoord = ProCamera2D.GameCamera.ScreenToWorldPoint(touchPos);
								ResetPrevPanPoint = false;
							}

							var panDelta = prevTouchPositionWorldCoord - currentTouchPositionWorldCoord;
							_panDelta = new Vector2(Vector3H(panDelta), Vector3V(panDelta));	
						}
						else
						{
							// Detect pan start
							var screenSize = (ProCamera2D.ScreenSizeInWorldCoordinates.x + ProCamera2D.ScreenSizeInWorldCoordinates.y) / 2;
							var dragDistancePerc = Vector3.Distance(currentTouchPositionWorldCoord, _startPanWorldPos) / screenSize;
							if (dragDistancePerc > MinPanAmount)
							{
								CenterPanTargetOnCamera(StopSpeedOnDragStart);
								StartPanning();
							}
						}
					}

					_prevTouchPosition = touchPos;
				}

				if (IsPanning && Input.touchCount == 0)
					StopPanning();
			}

			var panSpeed = DragPanSpeedMultiplier;

			if (UseMouseInput)
			{
				var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y,
					Mathf.Abs(Vector3D(ProCamera2D.LocalPosition)));

				if (Input.GetMouseButtonDown((int) PanMouseButton))
				{
					_startPanWorldPos = ProCamera2D.GameCamera.ScreenToWorldPoint(mousePos);
				}

				// Detect pan start
				if (UsePanByDrag && Input.GetMouseButton((int)PanMouseButton) && !IsPanning)
				{
					var mousePosWorldCoord = ProCamera2D.GameCamera.ScreenToWorldPoint(mousePos);
					var screenSize = (ProCamera2D.ScreenSizeInWorldCoordinates.x + ProCamera2D.ScreenSizeInWorldCoordinates.y) / 2;
					var dragDistancePerc = Vector3.Distance(mousePosWorldCoord, _startPanWorldPos) / screenSize;
					if (dragDistancePerc > MinPanAmount)
					{
						CenterPanTargetOnCamera(StopSpeedOnDragStart);
						StartPanning();
					}
				}

				// Mouse drag delta
				if (IsPanning && UsePanByDrag && Input.GetMouseButton((int)PanMouseButton))
				{
					var normalizedMousePos = new Vector2(Input.mousePosition.x / ProCamera2D.GameCamera.pixelWidth, Input.mousePosition.y / ProCamera2D.GameCamera.pixelHeight);
					if (ProCamera2D.GameCamera.pixelRect.Contains(mousePos) && InsideDraggableArea(normalizedMousePos))
					{
						var prevMousePositionWorldCoord = ProCamera2D.GameCamera.ScreenToWorldPoint(_prevMousePosition);

						if (ResetPrevPanPoint)
						{
							prevMousePositionWorldCoord = ProCamera2D.GameCamera.ScreenToWorldPoint(mousePos);
							ResetPrevPanPoint = false;
						}

						var panDelta = prevMousePositionWorldCoord - ProCamera2D.GameCamera.ScreenToWorldPoint(mousePos);
						_panDelta = new Vector2(Vector3H(panDelta), Vector3V(panDelta));
					}
				}
				// Move to edges delta
				else if (UsePanByMoveToEdges && !Input.GetMouseButton((int)PanMouseButton))
				{
					var normalizedMousePosX = (-Screen.width * .5f + Input.mousePosition.x) / Screen.width;
					var normalizedMousePosY = (-Screen.height * .5f + Input.mousePosition.y) / Screen.height;

					if (normalizedMousePosX < 0)
						normalizedMousePosX = normalizedMousePosX.Remap(-.5f, -LeftPanEdge * .5f, -.5f, 0f);
					else if (normalizedMousePosX > 0)
						normalizedMousePosX = normalizedMousePosX.Remap(RightPanEdge * .5f, .5f, 0f, .5f);

					if (normalizedMousePosY < 0)
						normalizedMousePosY = normalizedMousePosY.Remap(-.5f, -BottomPanEdge * .5f, -.5f, 0f);
					else if (normalizedMousePosY > 0)
						normalizedMousePosY = normalizedMousePosY.Remap(TopPanEdge * .5f, .5f, 0f, .5f);

					_panDelta = new Vector2(normalizedMousePosX, normalizedMousePosY) * deltaTime;

					if (_panDelta != Vector2.zero)
						panSpeed = EdgesPanSpeed;
				}
				
				if (IsPanning && UsePanByDrag && !Input.GetMouseButton((int)PanMouseButton))
					StopPanning();

				// Prevent unintentional pans when outside of the GameView on the editor
#if UNITY_EDITOR
				Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
				if (screenRect.Contains(Input.mousePosition) == false ||
					screenRect.Contains(_prevMousePosition) == false)
				{
					_panDelta = Vector2.zero;
				}
#endif
				_prevMousePosition = mousePos;
			}

			// Move
			if(_panDelta != Vector2.zero)
			{
				var delta = VectorHV(_panDelta.x * panSpeed.x, _panDelta.y * panSpeed.y);
				_panTarget.Translate(delta);
			}

			// Check if target is outside of bounds
			if ((ProCamera2D.IsCameraPositionLeftBounded && Vector3H(_panTarget.position) < Vector3H(ProCamera2D.LocalPosition)) ||
				(ProCamera2D.IsCameraPositionRightBounded && Vector3H(_panTarget.position) > Vector3H(ProCamera2D.LocalPosition)))
				_panTarget.position = VectorHVD(
					Vector3H(ProCamera2D.LocalPosition) - ProCamera2D.GetOffsetX() * 0.9999f, // The multiplier avoids floating-point comparison errors
					Vector3V(_panTarget.position),
					Vector3D(_panTarget.position));
				

			if ((ProCamera2D.IsCameraPositionBottomBounded && Vector3V(_panTarget.position) < Vector3V(ProCamera2D.LocalPosition)) ||
				(ProCamera2D.IsCameraPositionTopBounded && Vector3V(_panTarget.position) > Vector3V(ProCamera2D.LocalPosition)))
				_panTarget.position = VectorHVD(
					Vector3H(_panTarget.position), 
					Vector3V(ProCamera2D.LocalPosition) - ProCamera2D.GetOffsetY() * 0.9999f, // The multiplier avoids floating-point comparison errors
					Vector3D(_panTarget.position));
		}

		void StartPanning()
		{
			IsPanning = true;
			
			RestoreFollowSmoothness();

			if (OnPanStarted != null)
				OnPanStarted();
		}

		void StopPanning()
		{
			IsPanning = false;

			if (OnPanFinished != null)
				OnPanFinished();
		}

		void Zoom(float deltaTime)
		{
			var zoomInput = 0f;

			if (UseTouchInput)
			{
				if (Input.touchCount == 2)
				{
					var touchZero = Input.GetTouch(0);
					var touchOne = Input.GetTouch(1);

					var touchZeroPrevPos = touchZero.position - new Vector2(touchZero.deltaPosition.x / Screen.width, touchZero.deltaPosition.y / Screen.height);
					var touchOnePrevPos = touchOne.position - new Vector2(touchOne.deltaPosition.x / Screen.width, touchOne.deltaPosition.y / Screen.height);

					var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
					var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

					// Zoom input
					zoomInput = prevTouchDeltaMag - touchDeltaMag;

					// Zoom point
					var midTouch = Vector2.Lerp(touchZero.position, touchOne.position, .5f);
					_zoomPoint = new Vector3(midTouch.x, midTouch.y, Mathf.Abs(Vector3D(ProCamera2D.LocalPosition)));

					// Smoothness to 0
					if (!_zoomStarted)
					{
						_zoomStarted = true;
						_panTarget.position = ProCamera2D.LocalPosition - ProCamera2D.InfluencesSum;
						UpdateCurrentFollowSmoothness();
						RemoveFollowSmoothness();
					}

					// Save time
					_touchZoomTime = Time.time;
				}
				else
				{
					// Reset smoothness
					if (_zoomStarted && Mathf.Abs(_zoomAmount) < .001f)
					{
						RestoreFollowSmoothness();
						_zoomStarted = false;
					}
				}
			}
			
			if(UseMouseInput)
			{
				// Zoom input
				zoomInput = Input.GetAxis("Mouse ScrollWheel");
				
				// Zoom point
				_zoomPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Vector3D(ProCamera2D.LocalPosition)));
			}

			// Stop zoom if zoomPoint not on this camera
			if (!ProCamera2D.GameCamera.pixelRect.Contains(_zoomPoint))
				return;

			// Different zoom speed according to the platform
			var zoomSpeed = UseTouchInput ? PinchZoomSpeed * 10f : MouseZoomSpeed;

			// Cancel zoom if max/min reached
			if ((_onMaxZoom && zoomInput * zoomSpeed < 0) || (_onMinZoom && zoomInput * zoomSpeed > 0))
			{
				CancelZoom();
				return;
			}

			// Zoom amount
			_zoomAmount = Mathf.SmoothDamp(_prevZoomAmount, zoomInput * zoomSpeed, ref _zoomVelocity, ZoomSmoothness, float.MaxValue, deltaTime);

			if (UseMouseInput)
			{
				// Reset smoothness once zoom stops
				if (Mathf.Abs(_zoomAmount) <= .0001f)
				{
					if (_zoomStarted)
						RestoreFollowSmoothness();

					_zoomStarted = false;
					_prevZoomAmount = 0;
					return;
				}

				// Smoothness to 0
				if (!_zoomStarted)
				{
					_zoomStarted = true;
					_panTarget.position = ProCamera2D.LocalPosition - ProCamera2D.InfluencesSum;
					UpdateCurrentFollowSmoothness();
					RemoveFollowSmoothness();
				}
			}

			// Clamp zoom amount
			var targetSize = (ProCamera2D.ScreenSizeInWorldCoordinates.y / 2) + _zoomAmount;
			var minScreenSize = _initialCamSize / MaxZoomInAmount;
			var maxScreenSize = MaxZoomOutAmount * _initialCamSize;
			_onMaxZoom = false;
			_onMinZoom = false;
			if (targetSize < minScreenSize)
			{
				_zoomAmount -= targetSize - minScreenSize;
				_onMaxZoom = true;
			}
			else if (targetSize > maxScreenSize)
			{
				_zoomAmount -= targetSize - maxScreenSize;
				_onMinZoom = true;
			}

			// Save previous zoom amount
			_prevZoomAmount = _zoomAmount;

			// Move camera towards zoom point
			if (ZoomToInputCenter && _zoomAmount != 0)
			{
				var multiplier = _zoomAmount / (ProCamera2D.ScreenSizeInWorldCoordinates.y / 2);
				_panTarget.position += ((_panTarget.position - ProCamera2D.GameCamera.ScreenToWorldPoint(_zoomPoint)) * multiplier);
			}

			// Zoom
			ProCamera2D.Zoom(_zoomAmount);

			IsZooming = true;
		}

		/// <summary>
		/// Call this method after manually updating the camera follow smoothness
		/// </summary>
		public void UpdateCurrentFollowSmoothness()
		{
			_origFollowSmoothnessX = ProCamera2D.HorizontalFollowSmoothness;
			_origFollowSmoothnessY = ProCamera2D.VerticalFollowSmoothness;
		}

		/// <summary>
		/// Use this method to center the pan target on the current camera position.
		/// Useful for situations where you controlled the camera using other methods (like cinematics)
		/// </summary>
		public void CenterPanTargetOnCamera(float interpolant = 1f)
		{
			if (_panTarget != null)
				_panTarget.position = Vector3.Lerp(
					_panTarget.position, 
					VectorHV(Vector3H(ProCamera2D.LocalPosition) - ProCamera2D.GetOffsetX(), Vector3V(ProCamera2D.LocalPosition) - ProCamera2D.GetOffsetY()), 
					interpolant);
		}

		void CancelZoom()
		{
			_zoomAmount = 0f;
			_prevZoomAmount = 0f;
			_zoomVelocity = 0f;
		}

		void RestoreFollowSmoothness()
		{
			ProCamera2D.HorizontalFollowSmoothness = _origFollowSmoothnessX;
			ProCamera2D.VerticalFollowSmoothness = _origFollowSmoothnessY;
		}

		void RemoveFollowSmoothness()
		{
			ProCamera2D.HorizontalFollowSmoothness = 0;
			ProCamera2D.VerticalFollowSmoothness = 0;
		}

		bool InsideDraggableArea(Vector2 normalizedInput)
		{
			if (DraggableAreaRect.x == 0 &&
				DraggableAreaRect.y == 0 &&
				DraggableAreaRect.width == 1 &&
				DraggableAreaRect.height == 1)
				return true;

			if (normalizedInput.x > DraggableAreaRect.x + (1 - DraggableAreaRect.width) / 2 &&
				normalizedInput.x < DraggableAreaRect.x + DraggableAreaRect.width + (1 - DraggableAreaRect.width) / 2 &&
				normalizedInput.y > DraggableAreaRect.y + (1 - DraggableAreaRect.height) / 2 &&
				normalizedInput.y < DraggableAreaRect.y + DraggableAreaRect.height + (1 - DraggableAreaRect.height) / 2)
				return true;

			return false;
		}

#if UNITY_EDITOR
		protected override void DrawGizmos()
		{
			base.DrawGizmos();

			Gizmos.color = EditorPrefsX.GetColor(PrefsData.PanEdgesColorKey, PrefsData.PanEdgesColorValue);
			var gameCamera = ProCamera2D.GetComponent<Camera>();
			var cameraDimensions = gameCamera.orthographic ? Utils.GetScreenSizeInWorldCoords(gameCamera) : Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(transform.localPosition)));
			float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

			if (UsePanByMoveToEdges)
			{
				Gizmos.DrawWireCube(
					VectorHVD(
						Vector3H(ProCamera2D.transform.localPosition) - (LeftPanEdge * cameraDimensions.x / 4f) + (RightPanEdge * cameraDimensions.x / 4f),
						Vector3V(ProCamera2D.transform.localPosition) - (BottomPanEdge * cameraDimensions.y / 4f) + (TopPanEdge * cameraDimensions.y / 4f),
						cameraDepthOffset),
					VectorHV(
						cameraDimensions.x * ((LeftPanEdge + RightPanEdge) / 2f),
						cameraDimensions.y * ((TopPanEdge + BottomPanEdge) / 2f)));
			}

			if (UsePanByDrag)
			{
				if (DraggableAreaRect.x != 0 ||
					DraggableAreaRect.y != 0 ||
					DraggableAreaRect.width != 1 ||
					DraggableAreaRect.height != 1)
					Gizmos.DrawWireCube(
						VectorHVD(ProCamera2D.transform.localPosition.x + DraggableAreaRect.x * cameraDimensions.x, ProCamera2D.transform.localPosition.y + DraggableAreaRect.y * cameraDimensions.y, cameraDepthOffset),
						VectorHV(DraggableAreaRect.width * cameraDimensions.x, DraggableAreaRect.height * cameraDimensions.y));
			}
		}
#endif
	}
}