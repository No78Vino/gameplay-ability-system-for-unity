using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [System.Serializable]
    public class ProCamera2DParallaxLayer
    {
        public Camera ParallaxCamera;

        [Range(0, 5)]
        public float Speed = 1.0f;
        
        [Range(0, 5)]
        public float SpeedX = 1.0f;
        
        [Range(0, 5)]
        public float SpeedY = 1.0f;

        public LayerMask LayerMask;

        [HideInInspector][System.NonSerialized]
        public Transform CameraTransform;
    }

    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-parallax/")]
    [ExecuteInEditMode]
    public class ProCamera2DParallax : BasePC2D, IPostMover
    {
        public static string ExtensionName = "Parallax";

        public List<ProCamera2DParallaxLayer> ParallaxLayers = new List<ProCamera2DParallaxLayer>();
        public bool ParallaxHorizontal = true;
        public bool ParallaxVertical = true;
        public bool ParallaxZoom = true;
        public Vector3 RootPosition = Vector3.zero;
        public int FrontDepthStart = 1;
        public int BackDepthStart = -1;
        public bool UseIndependentAxisSpeeds;
        public bool AutomaticallyConfigureCameraClearFlags = true;

        float _initialOrtographicSize;

        float[] _initialSpeeds;
        Coroutine _animateCoroutine;

        protected override void Awake()
        {
            base.Awake();

            if (ProCamera2D == null)
                return;

            if (Application.isPlaying)
                CalculateParallaxObjectsOffset();

            foreach (var layer in ParallaxLayers)
            {
                if (layer.ParallaxCamera != null)
                {
                    layer.CameraTransform = layer.ParallaxCamera.transform;
                }
            }

            // We need this so we can toggle on/off the parallax
            _initialSpeeds = new float[ParallaxLayers.Count];
            for (int i = 0; i < _initialSpeeds.Length; i++)
            {
                _initialSpeeds[i] = ParallaxLayers[i].Speed;
            }


            // Disable the extension if the camera is not in orthographic projection
            if (ProCamera2D.GameCamera != null)
            {
                _initialOrtographicSize = ProCamera2D.GameCamera.orthographicSize;

                if (!ProCamera2D.GameCamera.orthographic)
                    enabled = false;
            }

            ProCamera2D.AddPostMover(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if(ProCamera2D)
                ProCamera2D.RemovePostMover(this);
        }

        #region IPostMover implementation

        public void PostMove(float deltaTime)
        {
            if(enabled)
                Move();
        }

        public int PMOrder { get { return _pmOrder; } set { _pmOrder = value; } }

        int _pmOrder = 1000;

        #endregion

        #if UNITY_EDITOR
        void LateUpdate()
        {
            if(!Application.isPlaying)
                Move();
        }
        #endif

        public void CalculateParallaxObjectsOffset()
        {
            // Find all parallax objects
            var parallaxObjs = FindObjectsOfType<ProCamera2DParallaxObject>();

            // Create dictionary that links Unity layers to ProCamera2D parallax layers
            var layersDic = new Dictionary<int, ProCamera2DParallaxLayer>();
            for (int i = 0; i <= 31; i++)
            {
                foreach (var parallaxLayer in ParallaxLayers)
                {
                    if (parallaxLayer.LayerMask == (parallaxLayer.LayerMask | (1 << i)))
                    {
                        layersDic[i] = parallaxLayer;
                    }
                }
            }

            // Apply offset to parallax objects according to the parallax layer they're part of
            for (int i = 0; i < parallaxObjs.Length; i++)
            {
                // Position
                var parallaxObjPosition = parallaxObjs[i].transform.position - RootPosition;
                var x = Vector3H(parallaxObjPosition) * (UseIndependentAxisSpeeds ? layersDic[parallaxObjs[i].gameObject.layer].SpeedX : layersDic[parallaxObjs[i].gameObject.layer].Speed);
                var y = Vector3V(parallaxObjPosition) * (UseIndependentAxisSpeeds ? layersDic[parallaxObjs[i].gameObject.layer].SpeedY : layersDic[parallaxObjs[i].gameObject.layer].Speed);
                parallaxObjs[i].transform.position = VectorHVD(x, y, Vector3D(parallaxObjPosition)) + RootPosition;
            }
        }

        void Move()
        {
            var rootOffset = _transform.position - RootPosition;

            for (int i = 0; i < ParallaxLayers.Count; i++)
            {
                if (ParallaxLayers[i].CameraTransform != null)
                {
                    // Rect
                    ParallaxLayers[i].ParallaxCamera.rect = ProCamera2D.GameCamera.rect;
                    
                    // Position
                    float x = ParallaxHorizontal ? Vector3H(rootOffset) * (UseIndependentAxisSpeeds ? ParallaxLayers[i].SpeedX : ParallaxLayers[i].Speed) : Vector3H(rootOffset);
                    float y = ParallaxVertical ? Vector3V(rootOffset) * (UseIndependentAxisSpeeds ? ParallaxLayers[i].SpeedY : ParallaxLayers[i].Speed) : Vector3V(rootOffset);
                    ParallaxLayers[i].CameraTransform.position = RootPosition + VectorHVD(x, y, Vector3D(_transform.position));

                    // Zoom
                    if (ParallaxZoom)
                    {
                        ParallaxLayers[i].ParallaxCamera.orthographicSize = _initialOrtographicSize + (ProCamera2D.GameCamera.orthographicSize - _initialOrtographicSize) * ParallaxLayers[i].Speed;
                    }
                }
            }
        }

        /// <summary>
        /// Enable or disable parallaxing.
        /// </summary>
        /// <param name="value">On or off</param>
        /// <param name="duration">Duration.</param>
        /// <param name="easeType">Ease type.</param>
        public void ToggleParallax(bool value, float duration = 2f, EaseType easeType = EaseType.EaseInOut)
        {
            if (_initialSpeeds == null)
                return;

            if (_animateCoroutine != null)
                StopCoroutine(_animateCoroutine);

            _animateCoroutine = StartCoroutine(Animate(value, duration, easeType));
        }

        IEnumerator Animate(bool value, float duration, EaseType easeType)
        {
            var currentSpeeds = new float[ParallaxLayers.Count];
            for (int i = 0; i < currentSpeeds.Length; i++)
            {
                currentSpeeds[i] = ParallaxLayers[i].Speed;
            }

            var t = 0f;
            while (t <= 1.0f)
            {
                t += ProCamera2D.DeltaTime / duration;

                for (int i = 0; i < ParallaxLayers.Count; i++)
                {
                    if (value)
                        ParallaxLayers[i].Speed = Utils.EaseFromTo(currentSpeeds[i], _initialSpeeds[i], t, easeType);
                    else
                        ParallaxLayers[i].Speed = Utils.EaseFromTo(currentSpeeds[i], 1, t, easeType);
                }

                yield return ProCamera2D.GetYield();
            }
        }
    }
}