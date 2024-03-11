using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DZoomToFitTargets))]
    public class ProCamera2DZoomToFitTargetsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
            var proCamera2DZoomToFitTargets = (ProCamera2DZoomToFitTargets)target;

            if(proCamera2DZoomToFitTargets.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

            DrawDefaultInspector();

            if (proCamera2DZoomToFitTargets.ZoomInBorder > proCamera2DZoomToFitTargets.ZoomOutBorder)
                proCamera2DZoomToFitTargets.ZoomOutBorder = proCamera2DZoomToFitTargets.ZoomInBorder;

            if (proCamera2DZoomToFitTargets.ZoomOutBorder < proCamera2DZoomToFitTargets.ZoomInBorder)
                proCamera2DZoomToFitTargets.ZoomInBorder = proCamera2DZoomToFitTargets.ZoomOutBorder;

            if (proCamera2DZoomToFitTargets.ZoomInBorder <= 0f)
                proCamera2DZoomToFitTargets.ZoomInBorder = 0f;

            if (proCamera2DZoomToFitTargets.ZoomInBorder >= 1f)
                proCamera2DZoomToFitTargets.ZoomInBorder = 1f;

            if (proCamera2DZoomToFitTargets.ZoomOutBorder <= 0f)
                proCamera2DZoomToFitTargets.ZoomOutBorder = 0f;

            if (proCamera2DZoomToFitTargets.ZoomOutBorder >= 1f)
                proCamera2DZoomToFitTargets.ZoomOutBorder = 1f;
    
            if (proCamera2DZoomToFitTargets.ZoomInSmoothness < 0f)
                proCamera2DZoomToFitTargets.ZoomInSmoothness = 0f;

            if (proCamera2DZoomToFitTargets.ZoomOutSmoothness < 0f)
                proCamera2DZoomToFitTargets.ZoomOutSmoothness = 0f;

            if (proCamera2DZoomToFitTargets.MaxZoomInAmount < 1f)
                proCamera2DZoomToFitTargets.MaxZoomInAmount = 1f;

            if (proCamera2DZoomToFitTargets.MaxZoomOutAmount < 1f)
                proCamera2DZoomToFitTargets.MaxZoomOutAmount = 1f;
        }
    }
}