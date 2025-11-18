using UnityEngine;
using UnityEditor;

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineImpulseSource))]
    internal sealed class CinemachineImpulseSourceEditor : BaseEditor<CinemachineImpulseSource>
    {
        float m_TestForce = 1;
        GUIContent m_TestButton = new GUIContent(
            "Invoke", "Play-mode only: Generate an impulse with the default velocity scaled by this amount");
        GUIContent m_TestLabel = new GUIContent(
            "Test with Force", "Generate an impulse with the default velocity scaled by an amount");

        public override void OnInspectorGUI()
        {
            BeginInspector();
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox(
                "Connect your impulse-generating event "
                    + "to one of the GenerateImpulse API methods defined in this script.",
                MessageType.Info);
            DrawRemainingPropertiesInInspector();

            GUI.enabled = EditorApplication.isPlaying;
            {
                var r1 = EditorGUILayout.GetControlRect();
                r1 = EditorGUI.PrefixLabel(r1, m_TestLabel);
                var testButtonWidth = GUI.skin.button.CalcSize(m_TestButton).x;
                var r2 = r1;
                r1.width = testButtonWidth;
                r2.x += testButtonWidth + 3; r2.width -= testButtonWidth + 3;

                m_TestForce = EditorGUI.Slider(r2, m_TestForce, 0.1f, 20f);
                if (GUI.Button(r1, m_TestButton))
                    Target.GenerateImpulseWithForce(m_TestForce);
            }
            GUI.enabled = true;
        }
    }
}
