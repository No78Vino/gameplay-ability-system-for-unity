using UnityEngine;
using System;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	public struct NumericBoundariesSettings
	{
		public bool UseNumericBoundaries;
		public bool UseTopBoundary;
		public float TopBoundary;
		public bool UseBottomBoundary;
		public float BottomBoundary;
		public bool UseLeftBoundary;
		public float LeftBoundary;
		public bool UseRightBoundary;
		public float RightBoundary;
	}

	[HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-numeric-boundaries/")]
	public class ProCamera2DNumericBoundaries : BasePC2D, IPositionDeltaChanger, ISizeOverrider
	{
		public static string ExtensionName = "Numeric Boundaries";

		public NumericBoundariesSettings Settings
		{
			get
			{
				return new NumericBoundariesSettings()
				{
					UseNumericBoundaries = UseNumericBoundaries,
					UseTopBoundary = UseTopBoundary,
					TopBoundary = TopBoundary,
					UseBottomBoundary = UseBottomBoundary,
					BottomBoundary = BottomBoundary,
					UseLeftBoundary = UseLeftBoundary,
					LeftBoundary = LeftBoundary,
					UseRightBoundary = UseRightBoundary,
					RightBoundary = RightBoundary
				};
			}
			set
			{
				UseNumericBoundaries = value.UseNumericBoundaries;
				UseTopBoundary = value.UseTopBoundary;
				TopBoundary = value.TopBoundary;
				UseBottomBoundary = value.UseBottomBoundary;
				BottomBoundary = value.BottomBoundary;
				UseLeftBoundary = value.UseLeftBoundary;
				LeftBoundary = value.LeftBoundary;
				UseRightBoundary = value.UseRightBoundary;
				RightBoundary = value.RightBoundary;
			}
		}

		public Action OnBoundariesTransitionStarted;
		public Action OnBoundariesTransitionFinished;

		public bool UseNumericBoundaries = true;
		public bool UseTopBoundary;
		public float TopBoundary = 10f;
		public float TargetTopBoundary;

		public bool UseBottomBoundary = true;
		public float BottomBoundary = -10f;
		public float TargetBottomBoundary;

		public bool UseLeftBoundary;
		public float LeftBoundary = -10f;
		public float TargetLeftBoundary;

		public bool UseRightBoundary;
		public float RightBoundary = 10f;
		public float TargetRightBoundary;

		public bool IsCameraPositionHorizontallyBounded;
		public bool IsCameraPositionVerticallyBounded;

		public Coroutine TopBoundaryAnimRoutine;
		public Coroutine BottomBoundaryAnimRoutine;
		public Coroutine LeftBoundaryAnimRoutine;
		public Coroutine RightBoundaryAnimRoutine;
		public ProCamera2DTriggerBoundaries CurrentBoundariesTrigger;

		public Coroutine MoveCameraToTargetRoutine;

		public bool HasFiredTransitionStarted;
		public bool HasFiredTransitionFinished;

		#region Soft Boundaries
		public bool UseSoftBoundaries = true;

		[Range(0f, 4f)]
		public float Softness = .5f;

		[Range(0f, .5f)]
		public float SoftAreaSize = .1f;

		float _smoothnessVelX = 0f;
		float _smoothnessVelY = 0f;
		
		public bool UseBoundaryDelayOnEnterScene = false;
		public int BoundaryDelayFrames = 10;
		int _elapsedBoundaryDelayFrames;
		bool _useSoftBoundariesEditorSetting;
		#endregion

		protected override void Awake()
		{
			base.Awake();
			
			_useSoftBoundariesEditorSetting = UseSoftBoundaries;

			ProCamera2D.AddPositionDeltaChanger(this);
			ProCamera2D.AddSizeOverrider(this);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			_elapsedBoundaryDelayFrames = 0;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			if(ProCamera2D == null) return;

			ProCamera2D.RemovePositionDeltaChanger(this);
			ProCamera2D.RemoveSizeOverrider(this);
		}

		#region IPositionDeltaChanger implementation

		public Vector3 AdjustDelta(float deltaTime, Vector3 originalDelta)
		{
			if (!enabled || !UseNumericBoundaries)
				return originalDelta;
			
			if (UseBoundaryDelayOnEnterScene && _elapsedBoundaryDelayFrames < BoundaryDelayFrames)
			{
				UseSoftBoundaries = false;
				_elapsedBoundaryDelayFrames++;
			}
			else
				UseSoftBoundaries = _useSoftBoundariesEditorSetting;

			// Check movement in the horizontal dir
			IsCameraPositionHorizontallyBounded = false;
			ProCamera2D.IsCameraPositionLeftBounded = false;
			ProCamera2D.IsCameraPositionRightBounded = false;
			IsCameraPositionVerticallyBounded = false;
			ProCamera2D.IsCameraPositionTopBounded = false;
			ProCamera2D.IsCameraPositionBottomBounded = false;
			var newPosH = Vector3H(ProCamera2D.LocalPosition) + Vector3H(originalDelta);
			var newPosV = Vector3V(ProCamera2D.LocalPosition) + Vector3V(originalDelta);
			var halfScreenWidth = ProCamera2D.ScreenSizeInWorldCoordinates.x * .5f;
			var halfScreenHeight = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;
			var cushionH = UseSoftBoundaries ? ProCamera2D.ScreenSizeInWorldCoordinates.x * SoftAreaSize : 0f;
			var cushionV = UseSoftBoundaries ? ProCamera2D.ScreenSizeInWorldCoordinates.y * SoftAreaSize : 0f;
			if (UseLeftBoundary && newPosH - halfScreenWidth < LeftBoundary + cushionH)
			{
				if (UseSoftBoundaries)
				{
					if (Vector3H(originalDelta) <= 0f)
						newPosH = Mathf.SmoothDamp(
							Mathf.Max(LeftBoundary + halfScreenWidth, Vector3H(ProCamera2D.LocalPosition)),
							Mathf.Max(LeftBoundary + halfScreenWidth, newPosH),
							ref _smoothnessVelX,
							(((LeftBoundary + halfScreenWidth) - Vector3H(ProCamera2D.LocalPosition) + cushionH) / cushionH) * Softness, float.MaxValue, deltaTime);
					else
						newPosH = Mathf.Max(LeftBoundary + halfScreenWidth, newPosH);
				}
				else
					newPosH = LeftBoundary + halfScreenWidth;

				IsCameraPositionHorizontallyBounded = true;
				ProCamera2D.IsCameraPositionLeftBounded = true;
			}

			if (UseRightBoundary && newPosH + halfScreenWidth > RightBoundary - cushionH)
			{
				if (UseSoftBoundaries)
				{
					if (Vector3H(originalDelta) >= 0f)
						newPosH = Mathf.SmoothDamp(
							Mathf.Min(RightBoundary - halfScreenWidth, Vector3H(ProCamera2D.LocalPosition)),
							Mathf.Min(RightBoundary - halfScreenWidth, newPosH),
							ref _smoothnessVelX,
							((Vector3H(ProCamera2D.LocalPosition) - (RightBoundary - halfScreenWidth) + cushionH) / cushionH) * Softness, float.MaxValue, deltaTime);
					else
						newPosH = Mathf.Min(RightBoundary - halfScreenWidth, newPosH);
				}
				else
					newPosH = RightBoundary - halfScreenWidth;

				IsCameraPositionHorizontallyBounded = true;
				ProCamera2D.IsCameraPositionRightBounded = true;
			}

			// Check movement in the vertical dir
			if (UseBottomBoundary && newPosV - halfScreenHeight < BottomBoundary + cushionV)
			{
				if (UseSoftBoundaries)
				{
					if (Vector3V(originalDelta) <= 0f)
						newPosV = Mathf.SmoothDamp(
							Mathf.Max(BottomBoundary + halfScreenHeight, Vector3V(ProCamera2D.LocalPosition)),
							Mathf.Max(BottomBoundary + halfScreenHeight, newPosV),
							ref _smoothnessVelY,
							(((BottomBoundary + halfScreenHeight) + cushionV - Vector3V(ProCamera2D.LocalPosition)) / cushionH) * Softness, float.MaxValue, deltaTime);
					else
						newPosV = Mathf.Max(BottomBoundary + halfScreenHeight, newPosV);
				}
				else
					newPosV = BottomBoundary + halfScreenHeight;

				IsCameraPositionVerticallyBounded = true;
				ProCamera2D.IsCameraPositionBottomBounded = true;
			}

			if (UseTopBoundary && newPosV + halfScreenHeight > TopBoundary - cushionV)
			{
				if (UseSoftBoundaries)
				{
					if (Vector3V(originalDelta) >= 0f)
						newPosV = Mathf.SmoothDamp(
							Mathf.Min(TopBoundary - halfScreenHeight, Vector3V(ProCamera2D.LocalPosition)),
							Mathf.Min(TopBoundary - halfScreenHeight, newPosV),
							ref _smoothnessVelY,
							((Vector3V(ProCamera2D.LocalPosition) - (TopBoundary - halfScreenHeight) + cushionV) / cushionH) * Softness, float.MaxValue, deltaTime);
					else
						newPosV = Mathf.Min(TopBoundary - halfScreenHeight, newPosV);
				}
				else
					newPosV = TopBoundary - halfScreenHeight;

				IsCameraPositionVerticallyBounded = true;
				ProCamera2D.IsCameraPositionTopBounded = true;
			}

			// Return the new delta
			return VectorHV(newPosH - Vector3H(ProCamera2D.LocalPosition), newPosV - Vector3V(ProCamera2D.LocalPosition));
		}

		public int PDCOrder { get { return _pdcOrder; } set { _pdcOrder = value; } }
		int _pdcOrder = 4000;

		#endregion

		#region ISizeOverrider implementation

		public float OverrideSize(float deltaTime, float originalSize)
		{
			if (!enabled || !UseNumericBoundaries)
				return originalSize;

			var newSize = originalSize;

			// Set new size if outside boundaries
			var cameraMaxSize = new Vector2(RightBoundary - LeftBoundary, TopBoundary - BottomBoundary);
			if (UseRightBoundary && UseLeftBoundary && originalSize * ProCamera2D.GameCamera.aspect * 2f > cameraMaxSize.x)
			{
				newSize = cameraMaxSize.x / ProCamera2D.GameCamera.aspect * .5f;
			}

			if (UseTopBoundary && UseBottomBoundary && newSize * 2f > cameraMaxSize.y)
			{
				newSize = cameraMaxSize.y * .5f;
			}

			return newSize;
		}

		public int SOOrder { get { return _soOrder; } set { _soOrder = value; } }
		int _soOrder = 2000;

		#endregion

#if UNITY_EDITOR
		override protected void DrawGizmos()
		{
			base.DrawGizmos();

			if (UseNumericBoundaries)
			{
				var gameCamera = ProCamera2D.GetComponent<Camera>();
				var cameraDimensions = gameCamera.orthographic ? Utils.GetScreenSizeInWorldCoords(gameCamera) : Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(transform.localPosition)));
				float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

				Gizmos.color = EditorPrefsX.GetColor(PrefsData.NumericBoundariesColorKey, PrefsData.NumericBoundariesColorValue);

				if (UseTopBoundary)
					Gizmos.DrawRay(VectorHVD(Vector3H(transform.localPosition) - cameraDimensions.x * .5f, TopBoundary, cameraDepthOffset), transform.right * cameraDimensions.x);

				if (UseBottomBoundary)
					Gizmos.DrawRay(VectorHVD(Vector3H(transform.localPosition) - cameraDimensions.x * .5f, BottomBoundary, cameraDepthOffset), transform.right * cameraDimensions.x);

				if (UseRightBoundary)
					Gizmos.DrawRay(VectorHVD(RightBoundary, Vector3V(transform.localPosition) - cameraDimensions.y * .5f, cameraDepthOffset), transform.up * cameraDimensions.y);

				if (UseLeftBoundary)
					Gizmos.DrawRay(VectorHVD(LeftBoundary, Vector3V(transform.localPosition) - cameraDimensions.y * .5f, cameraDepthOffset), transform.up * cameraDimensions.y);

				if (UseSoftBoundaries)
				{
					Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, Gizmos.color.a * .3f);

					if (UseTopBoundary)
						Gizmos.DrawRay(VectorHVD(Vector3H(transform.localPosition) - cameraDimensions.x * .5f, TopBoundary - cameraDimensions.y * SoftAreaSize, cameraDepthOffset), transform.right * cameraDimensions.x);

					if (UseBottomBoundary)
						Gizmos.DrawRay(VectorHVD(Vector3H(transform.localPosition) - cameraDimensions.x * .5f, BottomBoundary + cameraDimensions.y * SoftAreaSize, cameraDepthOffset), transform.right * cameraDimensions.x);

					if (UseRightBoundary)
						Gizmos.DrawRay(VectorHVD(RightBoundary - cameraDimensions.x * SoftAreaSize, Vector3V(transform.localPosition) - cameraDimensions.y * .5f, cameraDepthOffset), transform.up * cameraDimensions.y);

					if (UseLeftBoundary)
						Gizmos.DrawRay(VectorHVD(LeftBoundary + cameraDimensions.x * SoftAreaSize, Vector3V(transform.localPosition) - cameraDimensions.y * .5f, cameraDepthOffset), transform.up * cameraDimensions.y);
				}
			}
		}
#endif
	}
}