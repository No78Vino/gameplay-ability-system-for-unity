using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public enum TriggerRailsMode
    {
        Disable,
        Enable
    }

    [HelpURLAttribute("http://www.procamera2d.com/user-guide/trigger-rails/")]
    public class ProCamera2DTriggerRails : BaseTrigger
    {
        public static string TriggerName = "Rails Trigger";

        [HideInInspector]
        public ProCamera2DRails ProCamera2DRails;

        public TriggerRailsMode Mode; 

        public float TransitionDuration = 1f;
        
        void Start()
        {
            if (ProCamera2D == null)
                return;
                
            if (ProCamera2DRails == null)
                ProCamera2DRails = FindObjectOfType<ProCamera2DRails>();

            if (ProCamera2DRails == null)
                Debug.LogWarning("Rails extension couldn't be found on ProCamera2D. Please enable it to use this trigger.");
        }

        protected override void EnteredTrigger()
        {
            base.EnteredTrigger();
            
            if(Mode == TriggerRailsMode.Enable)
                ProCamera2DRails.EnableTargets(TransitionDuration);
            else
                ProCamera2DRails.DisableTargets(TransitionDuration);
        }

        #if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            base.DrawGizmos();

            float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

            Gizmos.DrawIcon(
                VectorHVD(
                    Vector3H(transform.position), 
                    Vector3V(transform.position), 
                    cameraDepthOffset), 
                Mode == TriggerRailsMode.Enable ? "ProCamera2D/gizmo_icon_rails_enable.png" : "ProCamera2D/gizmo_icon_rails_disable.png", 
                false);
        }
        #endif
    }
}