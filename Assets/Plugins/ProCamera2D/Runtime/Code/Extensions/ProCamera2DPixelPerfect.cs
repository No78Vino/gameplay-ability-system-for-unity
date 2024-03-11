using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	public enum AutoScaleMode
	{
		None,
		Floor,
		Ceil,
		Round
	}

	[HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-pixel-perfect/")]
	public class ProCamera2DPixelPerfect : BasePC2D, IPositionOverrider
	{
		public static string ExtensionName = "Pixel Perfect";

		public float PixelsPerUnit = 32;

		public AutoScaleMode ViewportAutoScale = AutoScaleMode.Round;

		public Vector2 TargetViewportSizeInPixels = new Vector2(80.0f, 50.0f);

		public int Zoom
		{
			get { return _zoom; }
			set
			{
				_zoom = value;
				ResizeCameraToPixelPerfect();
			}
		}
		[Range(1, 32)]
		[SerializeField]
		private int _zoom = 1;

		public bool SnapMovementToGrid;
		public bool SnapCameraToGrid = true;
		public bool DrawGrid;
		public Color GridColor = new Color(1f, 0f, 0f, .1f);
		public float GridDensity;
		
		/// <summary>
		/// Returns the current viewport scale. This can be useful for other components (like a CanvasScaler for example)
		/// </summary>
		public float ViewportScale { get; private set; }

		public float PixelStep
		{
			get
			{
#if UNITY_EDITOR
				if (!Application.isPlaying && _pixelStep < 0)
					ResizeCameraToPixelPerfect();
#endif
				return _pixelStep;
			}
		}

		float _pixelStep = -1;

		Transform _parent;

		override protected void Awake()
		{
			base.Awake();

			if (!ProCamera2D.GameCamera.orthographic)
			{
				enabled = false;
				return;
			}

			ResizeCameraToPixelPerfect();

			ProCamera2D.AddPositionOverrider(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			if(ProCamera2D)
				ProCamera2D.RemovePositionOverrider(this);
		}

		#region IPositionOverrider implementation

		public Vector3 OverridePosition(float deltaTime, Vector3 originalPosition)
		{
			if (!enabled)
				return originalPosition;

			// We do this so we can have the camera movement not snapping to the grid, while the sprites are
			var cameraPixelStep = _pixelStep;
			if (SnapMovementToGrid && !SnapCameraToGrid)
				cameraPixelStep = 1f / (PixelsPerUnit * (ViewportScale + Zoom - 1));

			// If shaking, align the shake parent position to pixel-perfect
			_parent = _transform.parent;
			if (_parent != null && _parent.position != Vector3.zero)
				_parent.position = VectorHVD(Utils.AlignToGrid(Vector3H(_parent.position), cameraPixelStep), Utils.AlignToGrid(Vector3V(_parent.position), cameraPixelStep), Vector3D(_parent.position));

			return VectorHVD(Utils.AlignToGrid(Vector3H(originalPosition), cameraPixelStep), Utils.AlignToGrid(Vector3V(originalPosition), cameraPixelStep), 0);
		}

		public int POOrder { get { return _poOrder; } set { _poOrder = value; } }
		int _poOrder = 2000;

		#endregion

#if UNITY_EDITOR
		void LateUpdate()
		{
			if (!Application.isPlaying)
				ResizeCameraToPixelPerfect();
		}
#endif

		/// <summary>
		/// Resizes the camera to a pixel perfect size
		/// </summary>
		public void ResizeCameraToPixelPerfect()
		{
			ViewportScale = CalculateViewportScale();

			_pixelStep = CalculatePixelStep(ViewportScale);

			var newSize = ((ProCamera2D.GameCamera.pixelHeight * .5f) * (1f / PixelsPerUnit)) / (ViewportScale + Zoom - 1);

			ProCamera2D.UpdateScreenSize(newSize);
		}

		public float CalculateViewportScale()
		{
			if (ViewportAutoScale == AutoScaleMode.None)
				return 1;

			float percentageX = ProCamera2D.GameCamera.pixelWidth / TargetViewportSizeInPixels.x;
			float percentageY = ProCamera2D.GameCamera.pixelHeight / TargetViewportSizeInPixels.y;

			float viewportScale = percentageX > percentageY ? percentageY : percentageX;

			switch (ViewportAutoScale)
			{
				case AutoScaleMode.Floor:
					viewportScale = Mathf.Floor(viewportScale);
					break;

				case AutoScaleMode.Ceil:
					viewportScale = Mathf.Ceil(viewportScale);
					break;

				case AutoScaleMode.Round:
					viewportScale = Mathf.Round(viewportScale);
					break;
			}

			if (viewportScale < 1)
				viewportScale = 1;

			return viewportScale;
		}

		float CalculatePixelStep(float viewportScale)
		{
			return SnapMovementToGrid ? 1f / PixelsPerUnit : 1f / (PixelsPerUnit * (viewportScale + Zoom - 1));
		}

#if UNITY_EDITOR
		override protected void DrawGizmos()
		{
			base.DrawGizmos();

			if (DrawGrid)
			{
				Gizmos.color = GridColor;
				var gridW = ProCamera2D.ScreenSizeInWorldCoordinates.x / 2;
				var gridH = ProCamera2D.ScreenSizeInWorldCoordinates.y / 2;

				float step = 1 / PixelsPerUnit;

				GridDensity = ProCamera2D.GameCamera.pixelWidth / (gridW * 2 / step);
				if (GridDensity < 4)
					return;

				Vector3 origin = transform.localPosition + 1 * transform.forward - VectorHV(gridW, -gridH);
				origin = VectorHVD(Utils.AlignToGrid(Vector3H(origin), step), Utils.AlignToGrid(Vector3V(origin), step), Vector3D(origin));

				for (float i = 0; i <= 2 * gridW; i += step)
				{
					Gizmos.DrawLine(origin + VectorHV(i, 0), origin + VectorHV(i, -2 * gridH));
				}

				for (float j = 0; j <= 2 * gridH; j += step)
				{
					Gizmos.DrawLine(origin + VectorHV(0, -j), origin + VectorHV(2 * gridW, -j));
				}
			}
		}
#endif
	}
}