using UnityEngine;
using UnityEditor;

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineBasicMultiChannelPerlin))]
    [CanEditMultipleObjects]
    internal sealed class CinemachineBasicMultiChannelPerlinEditor 
        : BaseEditor<CinemachineBasicMultiChannelPerlin>
    {
        private void OnEnable()
        {
            NoiseSettingsPropertyDrawer.InvalidateProfileList();
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            bool needWarning = false;
            for (int i = 0; !needWarning && i < targets.Length; ++i)
                needWarning = (targets[i] as CinemachineBasicMultiChannelPerlin).m_NoiseProfile == null;
            if (needWarning)
                EditorGUILayout.HelpBox(
                    "A Noise Profile is required.  You may choose from among the NoiseSettings assets defined in the project.",
                    MessageType.Warning);
            DrawRemainingPropertiesInInspector();

            Rect rect = EditorGUILayout.GetControlRect(true);
            rect.width -= EditorGUIUtility.labelWidth; rect.x += EditorGUIUtility.labelWidth;
            if (GUI.Button(rect, "New random seed"))
            {
                for (int i = 0; i < targets.Length; ++i)
                    (targets[i] as CinemachineBasicMultiChannelPerlin).ReSeed();
            }
        }
    }
}
