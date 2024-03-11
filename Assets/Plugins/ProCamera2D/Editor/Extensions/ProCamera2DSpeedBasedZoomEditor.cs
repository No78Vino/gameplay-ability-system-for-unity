using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DSpeedBasedZoom))]
    public class ProCamera2DSpeedBasedZoomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
            var proCamera2DSpeedBasedZoom = (ProCamera2DSpeedBasedZoom)target;

            if(proCamera2DSpeedBasedZoom.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

            DrawDefaultInspector();

            if (proCamera2DSpeedBasedZoom.MaxZoomInAmount < 1f)
                proCamera2DSpeedBasedZoom.MaxZoomInAmount = 1f;

            if (proCamera2DSpeedBasedZoom.MaxZoomOutAmount < 1f)
                proCamera2DSpeedBasedZoom.MaxZoomOutAmount = 1f;

            if (proCamera2DSpeedBasedZoom.CamVelocityForZoomOut <= proCamera2DSpeedBasedZoom.CamVelocityForZoomIn)
                proCamera2DSpeedBasedZoom.CamVelocityForZoomOut = proCamera2DSpeedBasedZoom.CamVelocityForZoomIn + .1f;

            if (proCamera2DSpeedBasedZoom.CamVelocityForZoomIn >= proCamera2DSpeedBasedZoom.CamVelocityForZoomOut)
                proCamera2DSpeedBasedZoom.CamVelocityForZoomIn = proCamera2DSpeedBasedZoom.CamVelocityForZoomOut - .1f;

            if (proCamera2DSpeedBasedZoom.CamVelocityForZoomIn < .5f)
                proCamera2DSpeedBasedZoom.CamVelocityForZoomIn = .5f;

            EditorGUILayout.Space();
            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Current Velocity (approx.)");
            GUILayout.Label(proCamera2DSpeedBasedZoom.CurrentVelocity.ToString());
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }
    }
}