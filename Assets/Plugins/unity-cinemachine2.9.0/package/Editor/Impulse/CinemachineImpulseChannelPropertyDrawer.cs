using UnityEngine;
using UnityEditor;

namespace Cinemachine.Editor
{
    [CustomPropertyDrawer(typeof(CinemachineImpulseChannelPropertyAttribute))]
    internal sealed class CinemachineImpulseChannelPropertyDrawer : PropertyDrawer
    {
        const float hSpace = 2;
        GUIContent mAddLabel = new GUIContent("Edit...", "Add, remove, or rename channels");
        string[] mLayerList = null;

        void UpdateLayerList()
        {
            CinemachineImpulseChannels settings = CinemachineImpulseChannels.InstanceIfExists;
            int numLayers = 0;
            if (settings != null && settings.ImpulseChannels != null)
                numLayers = settings.ImpulseChannels.Length;
            numLayers = Mathf.Clamp(numLayers, 1, 31);
            if (mLayerList == null || mLayerList.Length != numLayers)
                mLayerList = new string[numLayers];
            for (int i = 0; i < numLayers; ++i)
            {
                mLayerList[i] = string.Format(
                    "{0}: {1}", i, 
                    (settings == null || settings.ImpulseChannels.Length <= i) 
                        ? "default" : settings.ImpulseChannels[i]);
            }
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            UpdateLayerList();
            float addWidth = GUI.skin.button.CalcSize(mAddLabel).x;
            rect.width -= addWidth + hSpace;
            
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int value = EditorGUI.MaskField(rect, label, property.intValue, mLayerList);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue  = value;
                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.showMixedValue = false;

            rect.x += rect.width + hSpace; rect.width = addWidth; rect.height -= 1;
            if (GUI.Button(rect, mAddLabel))
                if (CinemachineImpulseChannels.Instance != null)
                    Selection.activeObject = CinemachineImpulseChannels.Instance;
        }
    }
}
