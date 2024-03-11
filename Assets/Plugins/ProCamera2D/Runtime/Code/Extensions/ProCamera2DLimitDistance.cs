using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-limit-distance/")]
	public class ProCamera2DLimitDistance : BasePC2D, IPositionDeltaChanger
	{
		public static string ExtensionName = "Limit Distance";

		public bool UseTargetsPosition = true;

		public bool LimitTopCameraDistance = true;
		[Range(.1f, 1f)]
		public float MaxTopTargetDistance = .8f;

		public bool LimitBottomCameraDistance = true;
		[Range(.1f, 1f)]
		public float MaxBottomTargetDistance = .8f;

		public bool LimitLeftCameraDistance = true;
		[Range(.1f, 1f)]
		public float MaxLeftTargetDistance = .8f;

		public bool LimitRightCameraDistance = true;
		[Range(.1f, 1f)]
		public float MaxRightTargetDistance = .8f;

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

			var horizontalDeltaMovement = Vector3H(originalDelta);
			var horizontalExtra = false;

			var verticalDeltaMovement = Vector3V(originalDelta);
			var verticalExtra = false;

			var comparisonPosition = UseTargetsPosition
				? new Vector2(Vector3H(ProCamera2D.TargetsMidPoint), Vector3V(ProCamera2D.TargetsMidPoint))
				: new Vector2(Vector3H(ProCamera2D.CameraTargetPosition), Vector3V(ProCamera2D.CameraTargetPosition));

			// Top
			if (LimitTopCameraDistance)
			{
				var verticalArea = (ProCamera2D.ScreenSizeInWorldCoordinates.y / 2) * MaxTopTargetDistance;
				if (comparisonPosition.y > verticalDeltaMovement + Vector3V(ProCamera2D.LocalPosition) + verticalArea)
				{
					verticalDeltaMovement = comparisonPosition.y - (Vector3V(ProCamera2D.LocalPosition) + verticalArea);
					verticalExtra = true;
				}
			}

			// Bottom
			if (LimitBottomCameraDistance)
			{
				var verticalArea = (ProCamera2D.ScreenSizeInWorldCoordinates.y / 2) * MaxBottomTargetDistance;
				if (comparisonPosition.y < verticalDeltaMovement + Vector3V(ProCamera2D.LocalPosition) - verticalArea)
				{
					verticalDeltaMovement = comparisonPosition.y - (Vector3V(ProCamera2D.LocalPosition) - verticalArea);
					verticalExtra = true;
				}
			}

			// Left
			if (LimitLeftCameraDistance)
			{
				var horizontalArea = (ProCamera2D.ScreenSizeInWorldCoordinates.x / 2) * MaxLeftTargetDistance;
				if (comparisonPosition.x < horizontalDeltaMovement + Vector3H(ProCamera2D.LocalPosition) - horizontalArea)
				{
					horizontalDeltaMovement = comparisonPosition.x - (Vector3H(ProCamera2D.LocalPosition) - horizontalArea);
					horizontalExtra = true;
				}
			}

			// Right
			if (LimitRightCameraDistance)
			{
				var horizontalArea = (ProCamera2D.ScreenSizeInWorldCoordinates.x / 2) * MaxRightTargetDistance;
				if (comparisonPosition.x > horizontalDeltaMovement + Vector3H(ProCamera2D.LocalPosition) + horizontalArea)
				{
					horizontalDeltaMovement = comparisonPosition.x - (Vector3H(ProCamera2D.LocalPosition) + horizontalArea);
					horizontalExtra = true;
				}
			}

			var h = horizontalExtra ? ProCamera2D.CameraTargetPositionSmoothed.x + horizontalDeltaMovement - Vector3H(originalDelta) : ProCamera2D.CameraTargetPositionSmoothed.x;
			var v = verticalExtra ? ProCamera2D.CameraTargetPositionSmoothed.y + verticalDeltaMovement - Vector3V(originalDelta) : ProCamera2D.CameraTargetPositionSmoothed.y;
			ProCamera2D.CameraTargetPositionSmoothed = new Vector2(h, v);

			return VectorHV(horizontalDeltaMovement, verticalDeltaMovement);
		}

		public int PDCOrder { get { return _pdcOrder; } set { _pdcOrder = value; } }
		int _pdcOrder = 2000;
		#endregion

#if UNITY_EDITOR
		protected override void DrawGizmos()
		{
			base.DrawGizmos();

			var gameCamera = ProCamera2D.GetComponent<Camera>();
			var cameraDimensions = gameCamera.orthographic ? Utils.GetScreenSizeInWorldCoords(gameCamera) : Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(transform.position)));
			float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

			Gizmos.color = EditorPrefsX.GetColor(PrefsData.CamDistanceColorKey, PrefsData.CamDistanceColorValue);
			if (LimitTopCameraDistance)
				Gizmos.DrawRay(VectorHVD(Vector3H(transform.position) - cameraDimensions.x / 2, Vector3V(transform.position) + (cameraDimensions.y / 2) * MaxTopTargetDistance, cameraDepthOffset), transform.right * cameraDimensions.x);

			if (LimitBottomCameraDistance)
				Gizmos.DrawRay(VectorHVD(Vector3H(transform.position) - cameraDimensions.x / 2, Vector3V(transform.position) - (cameraDimensions.y / 2) * MaxBottomTargetDistance, cameraDepthOffset), transform.right * cameraDimensions.x);

			if (LimitLeftCameraDistance)
				Gizmos.DrawRay(VectorHVD(Vector3H(transform.position) - (cameraDimensions.x / 2) * MaxLeftTargetDistance, Vector3V(transform.position) - cameraDimensions.y / 2, cameraDepthOffset), transform.up * cameraDimensions.y);

			if (LimitRightCameraDistance)
				Gizmos.DrawRay(VectorHVD(Vector3H(transform.position) + (cameraDimensions.x / 2) * MaxRightTargetDistance, Vector3V(transform.position) - cameraDimensions.y / 2, cameraDepthOffset), transform.up * cameraDimensions.y);
		}
#endif
	}
}