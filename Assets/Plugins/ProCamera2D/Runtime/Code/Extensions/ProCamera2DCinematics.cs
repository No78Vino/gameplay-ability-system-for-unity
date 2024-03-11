using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[Serializable]
	public class CinematicTarget
	{
		public Transform TargetTransform;
		public float EaseInDuration = 1f;
		public float HoldDuration = 1f;
		public float Zoom = 1f;
		public EaseType EaseType = EaseType.EaseOut;
		public string SendMessageName;
		public string SendMessageParam;
	}

	[Serializable]
	public class CinematicEvent : UnityEvent<int>
	{
	}

	[HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-cinematics/")]
	public class ProCamera2DCinematics : BasePC2D, IPositionOverrider, ISizeOverrider
	{
		public static string ExtensionName = "Cinematics";

		public UnityEvent OnCinematicStarted = new UnityEvent();
		public CinematicEvent OnCinematicTargetReached = new CinematicEvent();
		public UnityEvent OnCinematicFinished = new UnityEvent();

		bool _isPlaying;

		public bool IsPlaying { get { return _isPlaying; } }

		public List<CinematicTarget> CinematicTargets = new List<CinematicTarget>();
		public float EndDuration = 1f;
		public EaseType EndEaseType = EaseType.EaseOut;

		public bool UseNumericBoundaries;

		public bool UseLetterbox = true;

		[Range(0f, .5f)]
		public float LetterboxAmount = .1f;

		public float LetterboxAnimDuration = 1f;

		public Color LetterboxColor = Color.black;

		float _initialCameraSize;

		ProCamera2DNumericBoundaries _numericBoundaries;
		bool _numericBoundariesPreviousState;

		ProCamera2DLetterbox _letterbox;

		Coroutine _startCinematicRoutine;
		Coroutine _goToCinematicRoutine;
		Coroutine _endCinematicRoutine;

		bool _skipTarget;

		Vector3 _newPos;
		Vector3 _originalPos;
		Vector3 _startPos;

		float _newSize;
		bool _paused;

		override protected void Awake()
		{
			base.Awake();

			if (UseLetterbox)
				SetupLetterbox();

			ProCamera2D.AddPositionOverrider(this);
			ProCamera2D.AddSizeOverrider(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			if(ProCamera2D == null) return;

			ProCamera2D.RemovePositionOverrider(this);
			ProCamera2D.RemoveSizeOverrider(this);
		}

		#region IPositionOverrider implementation

		public Vector3 OverridePosition(float deltaTime, Vector3 originalPosition)
		{
			if (!enabled)
				return originalPosition;

			_originalPos = originalPosition;

			if (_isPlaying)
				return _newPos;
			else
				return originalPosition;
		}

		public int POOrder { get { return _poOrder; } set { _poOrder = value; } }

		int _poOrder = 0;

		#endregion

		#region ISizeOverrider implementation

		public float OverrideSize(float deltaTime, float originalSize)
		{
			if (!enabled)
				return originalSize;

			if (_isPlaying)
				return _newSize;
			else
				return originalSize;
		}

		public int SOOrder { get { return _soOrder; } set { _soOrder = value; } }

		int _soOrder = 3000;

		#endregion

		/// <summary>Play the cinematic.</summary>
		public void Play()
		{
			if (_isPlaying)
				return;

			_paused = false;

			if (CinematicTargets.Count == 0)
			{
				Debug.LogWarning("No cinematic targets added to the list");
				return;
			}

			_initialCameraSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;

			if (_numericBoundaries == null)
				_numericBoundaries = ProCamera2D.GetComponentInChildren<ProCamera2DNumericBoundaries>();

			if (_numericBoundaries == null)
				_numericBoundaries = FindObjectOfType<ProCamera2DNumericBoundaries>();

			if (_numericBoundaries == null)
				UseNumericBoundaries = false;
			else
			{
				_numericBoundariesPreviousState = _numericBoundaries.enabled;
				_numericBoundaries.enabled = false;
			}

			_isPlaying = true;
			if (_endCinematicRoutine != null)
			{
				StopCoroutine(_endCinematicRoutine);
				_endCinematicRoutine = null;
			}

			if (_startCinematicRoutine == null)
				_startCinematicRoutine = StartCoroutine(StartCinematicRoutine());
		}

		/// <summary>Stop the cinematic.</summary>
		public void Stop()
		{
			if (!_isPlaying)
				return;

			if (_startCinematicRoutine != null)
			{
				StopCoroutine(_startCinematicRoutine);
				_startCinematicRoutine = null;
			}

			if (_goToCinematicRoutine != null)
			{
				StopCoroutine(_goToCinematicRoutine);
				_goToCinematicRoutine = null;
			}

			if (_endCinematicRoutine == null)
				_endCinematicRoutine = StartCoroutine(EndCinematicRoutine());
		}

		/// <summary>If the cinematic is stopped, it plays it. If it's playing, it stops it.</summary>
		public void Toggle()
		{
			if (_isPlaying)
				Stop();
			else
				Play();
		}

		/// <summary>Goes to the next cinematic target</summary>
		public void GoToNextTarget()
		{
			_skipTarget = true;
		}

		/// <summary>Pauses the cinematic</summary>
		public void Pause()
		{
			_paused = true;
		}

		/// <summary>Unpauses the cinematic</summary>
		public void Unpause()
		{
			_paused = false;
		}

		/// <summary>Goes to the next cinematic target</summary>
		/// <param name="targetTransform">The Transform component of the target</param>
		/// <param name="easeInDuration">The time it takes for the camera to reach the target</param>
		/// <param name="holdDuration">The time the camera follows the target. If below 0, you’ll have to manually move to the next target by using the GoToNextTarget method</param>
		/// <param name="zoom">The zoom the camera should make while following the target. Use 1 for no zoom</param>
		/// <param name="easeType">The animation type of the camera movement to reach the target</param>
		/// <param name="sendMessageName">The method name that will be called when the target is reached</param>
		/// <param name="sendMessageParam">The parameter that will be sent when the above method is called</param>
		/// <param name="index">The position in the targets list. Use -1 to put in last and 0 for first</param>
		public void AddCinematicTarget(Transform targetTransform, float easeInDuration = 1f, float holdDuration = 1f, float zoom = 1f, EaseType easeType = EaseType.EaseOut, string sendMessageName = "", string sendMessageParam = "", int index = -1)
		{
			var newCinematicTarget = new CinematicTarget()
			{
				TargetTransform = targetTransform,
				EaseInDuration = easeInDuration,
				HoldDuration = holdDuration,
				Zoom = zoom,
				EaseType = easeType,
				SendMessageName = sendMessageName,
				SendMessageParam = sendMessageParam
			};

			if (index == -1 || index > CinematicTargets.Count)
				CinematicTargets.Add(newCinematicTarget);
			else
				CinematicTargets.Insert(index, newCinematicTarget);
		}

		// <summary>Remove a cinematic target</summary>
		/// <param name="targetTransform">The Transform component of the target</param>
		public void RemoveCinematicTarget(Transform targetTransform)
		{
			for (int i = 0; i < CinematicTargets.Count; i++)
			{
				if (CinematicTargets[i].TargetTransform.GetInstanceID() == targetTransform.GetInstanceID())
				{
					CinematicTargets.Remove(CinematicTargets[i]);
				}
			}
		}

		IEnumerator StartCinematicRoutine()
		{
			if (OnCinematicStarted != null)
				OnCinematicStarted.Invoke();

			_startPos = ProCamera2D.LocalPosition;
			_newPos = ProCamera2D.LocalPosition;
			_newSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;

			if (UseLetterbox)
			{
				if (_letterbox == null)
					SetupLetterbox();

				_letterbox.Color = LetterboxColor;
				_letterbox.TweenTo(LetterboxAmount, LetterboxAnimDuration);
			}

			var count = -1;
			while (count < CinematicTargets.Count - 1)
			{
				count++;
				_skipTarget = false;
				_goToCinematicRoutine = StartCoroutine(GoToCinematicTargetRoutine(CinematicTargets[count], count));
				yield return _goToCinematicRoutine;
			}

			Stop();
		}

		IEnumerator GoToCinematicTargetRoutine(CinematicTarget cinematicTarget, int targetIndex)
		{
			if (cinematicTarget.TargetTransform == null)
				yield break;

			var initialPosH = Vector3H(ProCamera2D.LocalPosition);
			var initialPosV = Vector3V(ProCamera2D.LocalPosition);

			var currentCameraSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;

			// Ease in
			var t = 0f;
			if (cinematicTarget.EaseInDuration > 0)
			{
				while (t <= 1.0f)
				{
					if (!_paused)
					{
						t += ProCamera2D.DeltaTime / cinematicTarget.EaseInDuration;

						var newPosH = Utils.EaseFromTo(initialPosH, Vector3H(cinematicTarget.TargetTransform.position) - Vector3H(ProCamera2D.ParentPosition), t, cinematicTarget.EaseType);
						var newPosV = Utils.EaseFromTo(initialPosV, Vector3V(cinematicTarget.TargetTransform.position) - Vector3V(ProCamera2D.ParentPosition), t, cinematicTarget.EaseType);

						if (UseNumericBoundaries)
							LimitToNumericBoundaries(ref newPosH, ref newPosV);

						_newPos = VectorHVD(newPosH, newPosV, 0);

						_newSize = Utils.EaseFromTo(currentCameraSize, _initialCameraSize / cinematicTarget.Zoom, t, cinematicTarget.EaseType);

						if (_skipTarget)
							yield break;
					}

					yield return ProCamera2D.GetYield();
				}
			}
			else
			{
				var newPosH = Vector3H(cinematicTarget.TargetTransform.position) - Vector3H(ProCamera2D.ParentPosition);
				var newPosV = Vector3V(cinematicTarget.TargetTransform.position) - Vector3V(ProCamera2D.ParentPosition);
				_newPos = VectorHVD(newPosH, newPosV, 0);

				_newSize = _initialCameraSize / cinematicTarget.Zoom;
			}

			// Dispatch target reached event
			if (OnCinematicTargetReached != null)
				OnCinematicTargetReached.Invoke(targetIndex);

			// Send target reached message
			if (!string.IsNullOrEmpty(cinematicTarget.SendMessageName))
			{
				cinematicTarget.TargetTransform.SendMessage(cinematicTarget.SendMessageName, cinematicTarget.SendMessageParam, SendMessageOptions.DontRequireReceiver);
			}

			// Hold
			t = 0f;
			while (cinematicTarget.HoldDuration < 0 || t <= cinematicTarget.HoldDuration)
			{
				if (!_paused)
				{
					t += ProCamera2D.DeltaTime;

					var newPosH = Vector3H(cinematicTarget.TargetTransform.position) - Vector3H(ProCamera2D.ParentPosition);
					var newPosV = Vector3V(cinematicTarget.TargetTransform.position) - Vector3V(ProCamera2D.ParentPosition);

					if (UseNumericBoundaries)
						LimitToNumericBoundaries(ref newPosH, ref newPosV);

					_newPos = VectorHVD(newPosH, newPosV, 0);

					if (_skipTarget)
						yield break;
				}

				yield return ProCamera2D.GetYield();
			}
		}

		IEnumerator EndCinematicRoutine()
		{
			if (_letterbox != null && LetterboxAmount > 0)
				_letterbox.TweenTo(0f, LetterboxAnimDuration);

			var initialPosH = Vector3H(_newPos);
			var initialPosV = Vector3V(_newPos);

			var currentCameraSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;

			// Ease out
			var t = 0f;
			while (t <= 1.0f)
			{
				if (!_paused)
				{
					t += ProCamera2D.DeltaTime / EndDuration;

					var originalPosH = Vector3H(_originalPos);
					var originalPosV = Vector3V(_originalPos);

					float newPosH = 0f;
					float newPosV = 0f;
					if (ProCamera2D.CameraTargets.Count > 0)
					{
						newPosH = Utils.EaseFromTo(initialPosH, originalPosH, t, EndEaseType);
						newPosV = Utils.EaseFromTo(initialPosV, originalPosV, t, EndEaseType);
					}
					else
					{
						newPosH = Utils.EaseFromTo(initialPosH, Vector3H(_startPos), t, EndEaseType);
						newPosV = Utils.EaseFromTo(initialPosV, Vector3V(_startPos), t, EndEaseType);
					}
					
					if (_numericBoundariesPreviousState)
						LimitToNumericBoundaries(ref newPosH, ref newPosV);

					_newPos = VectorHVD(newPosH, newPosV, 0);

					_newSize = Utils.EaseFromTo(currentCameraSize, _initialCameraSize, t, EndEaseType);
				}

				yield return ProCamera2D.GetYield();
			}

			_isPlaying = false;

			if (_numericBoundaries != null)
				_numericBoundaries.enabled = _numericBoundariesPreviousState;

			if (OnCinematicFinished != null)
				OnCinematicFinished.Invoke();

			// Ugly hack... but no way around it at the moment
			if (ProCamera2D.CameraTargets.Count == 0)
				ProCamera2D.Reset(true);
		}

		void SetupLetterbox()
		{
			var letterbox = ProCamera2D.gameObject.GetComponentInChildren<ProCamera2DLetterbox>();

			if (letterbox == null)
			{
				var cameras = ProCamera2D.gameObject.GetComponentsInChildren<Camera>();
				cameras = cameras.OrderByDescending(c => c.depth).ToArray();
				cameras[0].gameObject.AddComponent<ProCamera2DLetterbox>();
			}

			_letterbox = letterbox;
		}

		void LimitToNumericBoundaries(ref float horizontalPos, ref float verticalPos)
		{
			if (_numericBoundaries.UseLeftBoundary && horizontalPos - ProCamera2D.ScreenSizeInWorldCoordinates.x / 2 < _numericBoundaries.LeftBoundary)
				horizontalPos = _numericBoundaries.LeftBoundary + ProCamera2D.ScreenSizeInWorldCoordinates.x / 2;
			else if (_numericBoundaries.UseRightBoundary && horizontalPos + ProCamera2D.ScreenSizeInWorldCoordinates.x / 2 > _numericBoundaries.RightBoundary)
				horizontalPos = _numericBoundaries.RightBoundary - ProCamera2D.ScreenSizeInWorldCoordinates.x / 2;

			if (_numericBoundaries.UseBottomBoundary && verticalPos - ProCamera2D.ScreenSizeInWorldCoordinates.y / 2 < _numericBoundaries.BottomBoundary)
				verticalPos = _numericBoundaries.BottomBoundary + ProCamera2D.ScreenSizeInWorldCoordinates.y / 2;
			else if (_numericBoundaries.UseTopBoundary && verticalPos + ProCamera2D.ScreenSizeInWorldCoordinates.y / 2 > _numericBoundaries.TopBoundary)
				verticalPos = _numericBoundaries.TopBoundary - ProCamera2D.ScreenSizeInWorldCoordinates.y / 2;
		}

#if UNITY_EDITOR
		override protected void DrawGizmos()
		{
			base.DrawGizmos();

			float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

			// Draw cinematic targets
			for (int i = 0; i < CinematicTargets.Count; i++)
			{
				if (CinematicTargets[i].TargetTransform != null)
				{
					var targetPos = VectorHVD(Vector3H(CinematicTargets[i].TargetTransform.position), Vector3V(CinematicTargets[i].TargetTransform.position), cameraDepthOffset);
					Gizmos.DrawIcon(targetPos, "ProCamera2D/gizmo_icon_exclusive_free.png", false);
				}
			}
		}

		void OnDrawGizmosSelected()
		{
			if (ProCamera2D == null)
				return;

			var gameCamera = ProCamera2D.GetComponent<Camera>();
			float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);
			var cameraDimensions = Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)));

			// Draw cinematic path
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.green;
			for (int i = 0; i < CinematicTargets.Count; i++)
			{
				var targetPos = VectorHVD(Vector3H(CinematicTargets[i].TargetTransform.position), Vector3V(CinematicTargets[i].TargetTransform.position), cameraDepthOffset);

				if (i > 0)
				{
					UnityEditor.Handles.DrawLine(targetPos, VectorHVD(Vector3H(CinematicTargets[i - 1].TargetTransform.position), Vector3V(CinematicTargets[i - 1].TargetTransform.position), cameraDepthOffset));
				}

				UnityEditor.Handles.color = Color.blue;
				if (i < CinematicTargets.Count - 1)
				{
					var nextTargetPos = VectorHVD(Vector3H(CinematicTargets[i + 1].TargetTransform.position), Vector3V(CinematicTargets[i + 1].TargetTransform.position), cameraDepthOffset);
					var arrowSize = cameraDimensions.x * .1f;
					if ((nextTargetPos - targetPos).magnitude > arrowSize)
					{
						UnityEditor.Handles.ArrowHandleCap(
							i,
							targetPos,
							Quaternion.LookRotation(nextTargetPos - targetPos),
							cameraDimensions.x * .1f,
							EventType.Repaint);
					}
				}
			}
		}
#endif
	}
}