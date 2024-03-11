using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-pixel-perfect/")]
    [ExecuteInEditMode]
    public class ProCamera2DPixelPerfectSprite : BasePC2D, IPostMover
    {
        public bool IsAMovingObject;
        public bool IsAChildSprite;
        public Vector2 LocalPosition;

        [Range(-8, 32)]
        public int SpriteScale = 0;

        Sprite _sprite;

        ProCamera2DPixelPerfect _pixelPerfectPlugin;

        [SerializeField]
        Vector3 _initialScale = Vector3.one;
        int _prevSpriteScale;

        override protected void Awake()
        {
            base.Awake();

            if (ProCamera2D == null)
            {
                enabled = false;
                return;
            }

            GetPixelPerfectPlugin();

            GetSprite();

            ProCamera2D.AddPostMover(this);
        }

        void Start()
        {
            SetAsPixelPerfect();
        }

        #region IPostMover implementation

        public void PostMove(float deltaTime)
        {
            if(enabled)
                Step();
        }

        public int PMOrder { get { return _pmOrder; } set { _pmOrder = value; } }

        int _pmOrder = 2000;

        #endregion

        #if UNITY_EDITOR
        void LateUpdate()
        {
            if(enabled && !Application.isPlaying && !IsAMovingObject)
                SetAsPixelPerfect();
                
            if(!Application.isPlaying)
                Step();
        }
        #endif
        
        void Step()
        {
            if (_pixelPerfectPlugin == null || !_pixelPerfectPlugin.enabled)
                return;

            if (IsAMovingObject)
                SetAsPixelPerfect();

            _prevSpriteScale = SpriteScale;
        }

        void GetPixelPerfectPlugin()
        {
            _pixelPerfectPlugin = ProCamera2D.GetComponent<ProCamera2DPixelPerfect>();
            
            #if UNITY_EDITOR
            if(_pixelPerfectPlugin == null)
                Debug.LogWarning("PixelPerfect extension not present. Please add it to the ProCamera2D core.");
            #endif
        }

        void GetSprite()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                _sprite = spriteRenderer.sprite;
        }

        public void SetAsPixelPerfect()
        {
            #if UNITY_EDITOR
            if (Vector3H == null)
                base.Awake();

            if (_sprite == null)
                GetSprite();

            if (_pixelPerfectPlugin == null)
                GetPixelPerfectPlugin();

            if (Vector3H == null || _sprite == null || _pixelPerfectPlugin == null)
                return;
            #endif

            // Reset position
            if (IsAChildSprite)
                _transform.localPosition = VectorHVD(
                    Utils.AlignToGrid(LocalPosition.x, _pixelPerfectPlugin.PixelStep), 
                    Utils.AlignToGrid(LocalPosition.y, _pixelPerfectPlugin.PixelStep), 
                    Vector3D(_transform.localPosition));

            // Position
            _transform.position = VectorHVD(
                Utils.AlignToGrid(Vector3H(_transform.position), _pixelPerfectPlugin.PixelStep), 
                Utils.AlignToGrid(Vector3V(_transform.position), _pixelPerfectPlugin.PixelStep),
                Vector3D(_transform.position));

            // Scale
            if (SpriteScale == 0)
            {
                //The user was at 0 scale the last update, so save the current scale
                if (_prevSpriteScale == 0)
                    _initialScale = _transform.localScale;
                //The user just changed the scale to 0, so restore the original scale
                else
                    _transform.localScale = _initialScale;
            }
            else
            {
                var adjustedSpriteScale = SpriteScale < 0 ? 1f / (float)SpriteScale * -1f : SpriteScale;
                var scale = _sprite.pixelsPerUnit * adjustedSpriteScale * (1 / _pixelPerfectPlugin.PixelsPerUnit);

                _transform.localScale = new Vector3(
                    Mathf.Sign(_transform.localScale.x) * scale, 
                    Mathf.Sign(_transform.localScale.y) * scale, 
                    _transform.localScale.z);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(ProCamera2D != null)
                ProCamera2D.RemovePostMover(this);
        }
    }
}