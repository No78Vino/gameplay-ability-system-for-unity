using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DPointerInfluence))]
    public class ProCamera2DPointerInfluenceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
            var proCamera2DPointerInfluence = (ProCamera2DPointerInfluence)target;
            
            if(proCamera2DPointerInfluence.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);
                
            DrawDefaultInspector();
        }
    }
}