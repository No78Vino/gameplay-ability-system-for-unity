#if !UNITY_2019_3_OR_NEWER
#define CINEMACHINE_PHYSICS
#define CINEMACHINE_PHYSICS_2D
#endif

using UnityEditor;
using UnityEngine;

namespace Cinemachine.Editor
{
#if CINEMACHINE_PHYSICS || CINEMACHINE_PHYSICS_2D
    [CustomEditor(typeof(CinemachineCollisionImpulseSource))]
    internal sealed class CinemachineCollisionImpulseSourceEditor
        : BaseEditor<CinemachineCollisionImpulseSource>
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
            var collider = Target.GetComponent<Collider>();
            var collider2D = Target.GetComponent<Collider2D>();
            if ((collider == null || !collider.enabled) && (collider2D == null || !collider2D.enabled))
                EditorGUILayout.HelpBox(
                    "An active Collider or Collider2D component is required in order to detect "
                        + "collisions and generate Impulse events",
                    MessageType.Warning);

            DrawRemainingPropertiesInInspector();

            EditorGUILayout.Space();
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
#endif
}
