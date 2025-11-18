using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Cinemachine.Editor
{
    [CustomPropertyDrawer(typeof(CinemachineImpulseDefinition))]
    internal sealed class CinemachineImpulseDefinitionPropertyDrawer : PropertyDrawer
    {
        const int vSpace = 2;
        const int kGraphHeight = 8; // lines

        #pragma warning disable 649 // variable never used
        CinemachineImpulseDefinition m_MyClass;
        #pragma warning restore 649

        GUIContent m_TimeText = null;
        float m_TimeTextWidth;

        SerializedProperty m_ShapeProperty;
        float m_ShapePropertyHeight;

        SerializedProperty m_ImpulseTypeProperty;
        
        SerializedProperty m_DissipationRateProperty;
        float m_SpreadPropertyHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            m_ImpulseTypeProperty = property.FindPropertyRelative(() => m_MyClass.m_ImpulseType);
            var mode = (CinemachineImpulseDefinition.ImpulseTypes)m_ImpulseTypeProperty.intValue;
            if (mode == CinemachineImpulseDefinition.ImpulseTypes.Legacy)
                return LegacyModeGetPropertyHeight(property, label);

            int lines = 3;

            m_ShapePropertyHeight = EditorGUIUtility.singleLineHeight + vSpace;
            m_ShapeProperty = property.FindPropertyRelative(() => m_MyClass.m_ImpulseShape);
            if (m_ShapeProperty.isExpanded)
            {
                lines += kGraphHeight;
                m_ShapePropertyHeight *= 1 + kGraphHeight;
            }
            m_ShapePropertyHeight -= vSpace;

            m_DissipationRateProperty = property.FindPropertyRelative(() => m_MyClass.m_DissipationRate);
            if (mode != CinemachineImpulseDefinition.ImpulseTypes.Uniform)
            {
                m_SpreadPropertyHeight = EditorGUIUtility.singleLineHeight + vSpace;
                if (m_DissipationRateProperty.isExpanded)
                {
                    lines += kGraphHeight;
                    m_SpreadPropertyHeight *= 1 + kGraphHeight;
                }
                m_SpreadPropertyHeight -= vSpace;
                lines += 2;
                if (mode == CinemachineImpulseDefinition.ImpulseTypes.Propagating)
                    ++lines;
            }
            return lines * (EditorGUIUtility.singleLineHeight + vSpace);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(rect, label, property);

            var mode = (CinemachineImpulseDefinition.ImpulseTypes)m_ImpulseTypeProperty.intValue;
            if (mode == CinemachineImpulseDefinition.ImpulseTypes.Legacy)
            {
                LegacyModeOnGUI(rect, property, label);
                return;
            }

            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative(() => m_MyClass.m_ImpulseChannel));
                rect.y += rect.height + vSpace;

            // Impulse type
            EditorGUI.PropertyField(rect, m_ImpulseTypeProperty);
            rect.y += rect.height + vSpace;
            if (mode != CinemachineImpulseDefinition.ImpulseTypes.Uniform)
            {
                // Propaation speed
                if (mode == CinemachineImpulseDefinition.ImpulseTypes.Propagating)
                {
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative(() => m_MyClass.m_PropagationSpeed));
                    rect.y += rect.height + vSpace;
                }

                // Dissipation
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(() => m_MyClass.m_DissipationDistance));
                rect.y += rect.height + vSpace;

                // Spread combo
                rect.height = m_SpreadPropertyHeight;
                DrawSpreadCombo(rect, property);
                rect.y += rect.height + vSpace; rect.height = EditorGUIUtility.singleLineHeight;
            }
            // Impulse Shape combo
            rect.height = m_ShapePropertyHeight;
            DrawImpulseShapeCombo(rect, property);
            rect.y += rect.height + vSpace; rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.EndProperty();
        }

        void DrawImpulseShapeCombo(Rect fullRect, SerializedProperty property)
        {
            float floatFieldWidth = EditorGUIUtility.fieldWidth + 2;

            SerializedProperty timeProp = property.FindPropertyRelative(() => m_MyClass.m_ImpulseDuration);
            if (m_TimeText == null)
            {
                m_TimeText = new GUIContent(" s", timeProp.tooltip);
                m_TimeTextWidth = GUI.skin.label.CalcSize(m_TimeText).x;
            }

            var graphRect = fullRect; 
            graphRect.y += EditorGUIUtility.singleLineHeight + vSpace;
            graphRect.height -= EditorGUIUtility.singleLineHeight + vSpace;

            var indentLevel = EditorGUI.indentLevel;

            Rect r = fullRect; r.height = EditorGUIUtility.singleLineHeight;
            r = EditorGUI.PrefixLabel(r, EditorGUI.BeginProperty(
                r, new GUIContent(m_ShapeProperty.displayName, m_ShapeProperty.tooltip), property));
            m_ShapeProperty.isExpanded =  EditorGUI.Foldout(r, m_ShapeProperty.isExpanded, GUIContent.none);

            bool isCustom = m_ShapeProperty.intValue == (int)CinemachineImpulseDefinition.ImpulseShapes.Custom;
            r.width -= floatFieldWidth + m_TimeTextWidth;
            if (isCustom)
                r.width -= 2 * r.height;
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.PropertyField(r, m_ShapeProperty, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                    InvalidateImpulseGraphSample();
                if (!isCustom && Event.current.type == EventType.Repaint && m_ShapeProperty.isExpanded)
                    DrawImpulseGraph(graphRect, CinemachineImpulseDefinition.GetStandardCurve(
                        (CinemachineImpulseDefinition.ImpulseShapes)m_ShapeProperty.intValue));
            }
            if (isCustom)
            {
                SerializedProperty curveProp = property.FindPropertyRelative(() => m_MyClass.m_CustomImpulseShape);
                r.x += r.width;
                r.width = 2 * r.height;
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(r, curveProp, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    curveProp.animationCurveValue = RuntimeUtility.NormalizeCurve(curveProp.animationCurveValue, true, false);
                    curveProp.serializedObject.ApplyModifiedProperties();
                    InvalidateImpulseGraphSample();
                }
                if (Event.current.type == EventType.Repaint && m_ShapeProperty.isExpanded)
                    DrawImpulseGraph(graphRect, curveProp.animationCurveValue);
            }

            // Time
            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = m_TimeTextWidth;
            r.x += r.width; r.width = floatFieldWidth + EditorGUIUtility.labelWidth;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(r, timeProp, m_TimeText);
            if (EditorGUI.EndChangeCheck())
                timeProp.floatValue = Mathf.Max(timeProp.floatValue, 0);
            EditorGUIUtility.labelWidth = oldWidth;
            
            EditorGUI.indentLevel = indentLevel;
        }

        const int kNumSamples = 100;
        Vector3[] m_ImpulseGraphSnapshot = new Vector3[kNumSamples+1];
        float m_ImpulseGraphZero;
        Vector2 m_ImpulseGraphSize;
        void InvalidateImpulseGraphSample() { m_ImpulseGraphSize = Vector2.zero; }

        void DrawImpulseGraph(Rect rect, AnimationCurve curve)
        {
            // Resample if necessary
            if (m_ImpulseGraphSize != rect.size)
            {
                m_ImpulseGraphSize = rect.size;
                float minY = 0;
                float maxY = 0.1f;
                for (int i = 0; i <= kNumSamples; ++i)
                {
                    var x = (float)i / kNumSamples;
                    var y = curve.Evaluate(x);
                    minY = Mathf.Min(y, minY);
                    maxY = Mathf.Max(y, maxY);
                    m_ImpulseGraphSnapshot[i] = new Vector2(x * rect.width, y);
                }
                var range = maxY - minY;
                m_ImpulseGraphZero = -minY / range;
                m_ImpulseGraphZero = rect.height * (1f - m_ImpulseGraphZero);

                // Apply scale
                for (int i = 0; i <= kNumSamples; ++i)
                    m_ImpulseGraphSnapshot[i].y = rect.height * (1f - (m_ImpulseGraphSnapshot[i].y - minY) / range);
            }
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 1));
            var oldMatrix = Handles.matrix;
            Handles.matrix = Handles.matrix * Matrix4x4.Translate(rect.position);
            Handles.color = new Color(0, 0, 0, 1); 
            Handles.DrawLine(new Vector3(0, m_ImpulseGraphZero, 0), new Vector3(rect.width, m_ImpulseGraphZero, 0));
            Handles.color = new Color(1, 0.8f, 0, 1); 
            Handles.DrawPolyLine(m_ImpulseGraphSnapshot);
            Handles.matrix = oldMatrix;
        }

        void DrawSpreadCombo(Rect fullRect, SerializedProperty property)
        {
            var graphRect = fullRect; 
            graphRect.y += EditorGUIUtility.singleLineHeight + vSpace;
            graphRect.height -= EditorGUIUtility.singleLineHeight + vSpace;

            var indentLevel = EditorGUI.indentLevel;

            Rect r = fullRect; r.height = EditorGUIUtility.singleLineHeight;
            r = EditorGUI.PrefixLabel(r, EditorGUI.BeginProperty(
                r, new GUIContent(m_DissipationRateProperty.displayName, m_DissipationRateProperty.tooltip), property));
            m_DissipationRateProperty.isExpanded =  EditorGUI.Foldout(r, m_DissipationRateProperty.isExpanded, GUIContent.none);

            EditorGUI.BeginChangeCheck();
                EditorGUI.Slider(r, m_DissipationRateProperty, 0, 1, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
                InvalidateSpreadGraphSample();
            if (Event.current.type == EventType.Repaint && m_DissipationRateProperty.isExpanded)
                DrawSpreadGraph(graphRect, m_DissipationRateProperty.floatValue);
            EditorGUI.indentLevel = indentLevel;
        }

        Vector3[] m_SpreadGraphSnapshot = new Vector3[kNumSamples+1];
        Vector2 m_SpreadGraphSize;
        void InvalidateSpreadGraphSample() { m_SpreadGraphSize = Vector2.zero; }

        void DrawSpreadGraph(Rect rect, float spread)
        {
            // Resample if necessary
            if (m_SpreadGraphSize != rect.size)
            {
                m_SpreadGraphSize = rect.size;
                for (int i = 0; i <= kNumSamples >> 1; ++i)
                {
                    var x = (float)i / kNumSamples;
                    var y = CinemachineImpulseManager.EvaluateDissipationScale(spread, Mathf.Abs(1 - x * 2));
                    m_SpreadGraphSnapshot[i] = new Vector2(x * rect.width, rect.height * (1 - y));
                    m_SpreadGraphSnapshot[kNumSamples - i] = new Vector2((1 - x) * rect.width, rect.height * (1 - y));
                }
            }
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 1));
            var oldMatrix = Handles.matrix;
            Handles.matrix = Handles.matrix * Matrix4x4.Translate(rect.position);
            Handles.color = new Color(0, 0, 0, 1); 
            Handles.DrawLine(new Vector3(rect.width * 0.5f, 0, 0), new Vector3(rect.width * 0.5f, rect.height, 0));
            Handles.color = new Color(0, 0.6f, 1, 1); 
            Handles.DrawPolyLine(m_SpreadGraphSnapshot);
            Handles.matrix = oldMatrix;
        }

        // Legacy mode
        float HeaderHeight { get { return EditorGUIUtility.singleLineHeight * 1.5f; } }
        float DrawHeader(Rect rect, string text)
        {
            float delta = HeaderHeight - EditorGUIUtility.singleLineHeight;
            rect.y += delta; rect.height -= delta;
            EditorGUI.LabelField(rect, new GUIContent(text), EditorStyles.boldLabel);
            return HeaderHeight;
        }

        string HeaderText(SerializedProperty property)
        {
            var attrs = property.serializedObject.targetObject.GetType()
                .GetCustomAttributes(typeof(HeaderAttribute), false);
            if (attrs != null && attrs.Length > 0)
                return ((HeaderAttribute)attrs[0]).header;
            return null;
        }
        
        List<string> mHideProperties = new List<string>();

        float LegacyModeGetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            SignalSourceAsset asset = null;
            float height = 0;
            mHideProperties.Clear();
            string prefix = prop.name;
            prop.NextVisible(true); // Skip outer foldout
            do
            {
                if (!prop.propertyPath.StartsWith(prefix))
                    break;
                string header = HeaderText(prop);
                if (header != null)
                    height += HeaderHeight + vSpace;

                // Do we hide this property?
                bool hide = false;
                if (prop.name == SerializedPropertyHelper.PropertyName(() => m_MyClass.m_RawSignal))
                    asset = prop.objectReferenceValue as SignalSourceAsset;
                if (prop.name == SerializedPropertyHelper.PropertyName(() => m_MyClass.m_RepeatMode))
                    hide = asset == null || asset.SignalDuration <= 0;
                else if (prop.name == SerializedPropertyHelper.PropertyName(() => m_MyClass.m_Randomize))
                    hide = asset == null || asset.SignalDuration > 0;
                else
                {
                    hide = prop.name == SerializedPropertyHelper.PropertyName(() => m_MyClass.m_ImpulseShape)
                        || prop.name == SerializedPropertyHelper.PropertyName(() => m_MyClass.m_CustomImpulseShape)
                        || prop.name == SerializedPropertyHelper.PropertyName(() => m_MyClass.m_ImpulseDuration)
                        || prop.name == SerializedPropertyHelper.PropertyName(() => m_MyClass.m_DissipationRate);
                }

                if (hide)
                    mHideProperties.Add(prop.name);
                else
                    height += EditorGUI.GetPropertyHeight(prop, false) + vSpace;
            } while (prop.NextVisible(prop.isExpanded));
            return height;
        }

        void LegacyModeOnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            string prefix = prop.name;
            prop.NextVisible(true); // Skip outer foldout
            do
            {
                if (!prop.propertyPath.StartsWith(prefix))
                    break;
                string header = HeaderText(prop);
                if (header != null)
                {
                    rect.height = HeaderHeight;
                    DrawHeader(rect, header);
                    rect.y += HeaderHeight + vSpace;
                }
                if (mHideProperties.Contains(prop.name))
                    continue;
                rect.height = EditorGUI.GetPropertyHeight(prop, false);
                EditorGUI.PropertyField(rect, prop);
                rect.y += rect.height + vSpace;
            } while (prop.NextVisible(prop.isExpanded));
        }
    }
}
