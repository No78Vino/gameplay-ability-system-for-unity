using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-limit-speed/")]
    public class ProCamera2DLimitSpeed : BasePC2D, IPositionDeltaChanger
    {
        public static string ExtensionName = "Limit Speed";

        public bool LimitHorizontalSpeed = true;
        public float MaxHorizontalSpeed = 2f;
        public bool LimitVerticalSpeed = true;
        public float MaxVerticalSpeed = 2f;

        #if UNITY_EDITOR
        public float CurrentSpeedHorizontal;
        public float CurrentSpeedVertical;
        #endif

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

            var fps = 1f / deltaTime;

            var newHorizontalDeltaMovement = Vector3H(originalDelta) * fps;
            var newVerticalDeltaMovement = Vector3V(originalDelta) * fps;

            if (LimitHorizontalSpeed)
                newHorizontalDeltaMovement = Mathf.Clamp(newHorizontalDeltaMovement, -MaxHorizontalSpeed, MaxHorizontalSpeed);

            if (LimitVerticalSpeed)
                newVerticalDeltaMovement = Mathf.Clamp(newVerticalDeltaMovement, -MaxVerticalSpeed, MaxVerticalSpeed);
            
            #if UNITY_EDITOR
            CurrentSpeedHorizontal = newHorizontalDeltaMovement;
            CurrentSpeedVertical = newVerticalDeltaMovement;
            #endif

            return VectorHV(newHorizontalDeltaMovement * deltaTime, newVerticalDeltaMovement * deltaTime);
        }

        public int PDCOrder { get { return _pdcOrder; } set { _pdcOrder = value; } }

        int _pdcOrder = 1000;

        #endregion
    }
}