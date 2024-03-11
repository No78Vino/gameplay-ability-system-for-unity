using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-pointer-influence/")]
    public class ProCamera2DPointerInfluence : BasePC2D, IPreMover
    {
        public static string ExtensionName = "Pointer Influence";

        public float MaxHorizontalInfluence = 3f;
        public float MaxVerticalInfluence = 2f;

        public float InfluenceSmoothness = .2f;

        Vector2 _influence;
        Vector2 _velocity;

        protected override void Awake()
        {
            base.Awake();

            ProCamera2D.AddPreMover(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(ProCamera2D)
                ProCamera2D.RemovePreMover(this);
        }

        override public void OnReset()
        {
            _influence = Vector2.zero;
            _velocity = Vector2.zero;
        }

        #region IPreMover implementation

        public void PreMove(float deltaTime)
        {
            if(enabled)
                ApplyInfluence(deltaTime);
        }

        public int PrMOrder { get { return _prmOrder; } set { _prmOrder = value; } }

        int _prmOrder = 3000;

        #endregion

        void ApplyInfluence(float deltaTime)
        {
            var mousePosViewport = ProCamera2D.GameCamera.ScreenToViewportPoint(Input.mousePosition);

            var mousePosViewportH = mousePosViewport.x.Remap(0, 1, -1, 1);
            var mousePosViewportV = mousePosViewport.y.Remap(0, 1, -1, 1);

            var hInfluence = mousePosViewportH * MaxHorizontalInfluence;
            var vInfluence = mousePosViewportV * MaxVerticalInfluence;

            _influence = Vector2.SmoothDamp(_influence, new Vector2(hInfluence, vInfluence), ref _velocity, InfluenceSmoothness, Mathf.Infinity, deltaTime);

            ProCamera2D.ApplyInfluence(_influence);
        }
    }
}
