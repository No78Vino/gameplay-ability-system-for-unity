using UnityEngine;
using UnityEditor;

namespace Cinemachine.Editor
{
    [CustomPropertyDrawer(typeof(CinemachineImpulseManager.EnvelopeDefinition))]
    internal sealed class CinemachineImpulseEnvelopePropertyDrawer : PropertyDrawer
    {
        const int vSpace = 2;
        static bool mExpanded = true;

        CinemachineImpulseManager.EnvelopeDefinition myClass
            = new CinemachineImpulseManager.EnvelopeDefinition(); // to access name strings

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            rect.height = height;
            mExpanded = EditorGUI.Foldout(rect, mExpanded, label, true);
            if (mExpanded)
            {
                const float indentAmount = 15;
                rect.width -= indentAmount; rect.x += indentAmount;
                float oldWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth -= indentAmount;

                rect.y += EditorGUIUtility.singleLineHeight + vSpace;
                DrawCurveTimeProperty(
                    rect, new GUIContent("Attack", "The custom shape of the attack curve.  Leave it blank for a default shape"),
                    property.FindPropertyRelative(() => myClass.m_AttackShape),
                    property.FindPropertyRelative(() => myClass.m_AttackTime));

                rect.y += EditorGUIUtility.singleLineHeight + vSpace;
#if false // with "forever" button... dangerous because signal never goes away!
                var holdProp = property.FindPropertyRelative(() => myClass.m_SustainTime);
                InspectorUtility.MultiPropertyOnLine(
                    rect, new GUIContent(holdProp.displayName, holdProp.tooltip),
                    new SerializedProperty[] { holdProp, property.FindPropertyRelative(() => myClass.m_HoldForever) },
                    new GUIContent[] { GUIContent.none, new GUIContent("forever") });
#else
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(() => myClass.m_SustainTime));
#endif
                rect.y += EditorGUIUtility.singleLineHeight + vSpace;
                DrawCurveTimeProperty(
                    rect, new GUIContent("Decay", "The custom shape of the decay curve.  Leave it blank for a default shape"),
                    property.FindPropertyRelative(() => myClass.m_DecayShape),
                    property.FindPropertyRelative(() => myClass.m_DecayTime));

                rect.y += EditorGUIUtility.singleLineHeight + vSpace;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(() => myClass.m_ScaleWithImpact));

                EditorGUIUtility.labelWidth = oldWidth;
            }
        }

        void DrawCurveTimeProperty(
            Rect rect, GUIContent label,
            SerializedProperty curveProp, SerializedProperty timeProp)
        {
            float floatFieldWidth = EditorGUIUtility.singleLineHeight * 2.5f;

            GUIContent timeText = new GUIContent(" s", timeProp.tooltip);
            var textDimensions = GUI.skin.label.CalcSize(timeText);

            rect = EditorGUI.PrefixLabel(rect, label);

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width -= floatFieldWidth + textDimensions.x;

            Rect r = rect; r.height += 1; r.y -= 1;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(r, curveProp, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                curveProp.animationCurveValue = InspectorUtility.NormalizeCurve(curveProp.animationCurveValue);
                if (curveProp.animationCurveValue.length < 1)
                    curveProp.animationCurveValue = new AnimationCurve();
                curveProp.serializedObject.ApplyModifiedProperties();
            }

            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = textDimensions.x;
            rect.x += rect.width; rect.width = floatFieldWidth + EditorGUIUtility.labelWidth;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, timeProp, timeText);
            if (EditorGUI.EndChangeCheck())
                timeProp.floatValue = Mathf.Max(timeProp.floatValue, 0);
            EditorGUIUtility.labelWidth = oldWidth;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + vSpace;
            if (mExpanded)
                height *= 5;
            return height;
        }
    }
}
