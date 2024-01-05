using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace GAS.Editor.AbilitySystemComponent
{
    //[CustomEditor(typeof(Runtime.Component.AbilitySystemComponent))]
    public class AbilitySystemComponentEditor:UnityEditor.Editor
    {
        private Runtime.Component.AbilitySystemComponent _asset => (Runtime.Component.AbilitySystemComponent)target;
        
        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField(_asset.GetInstanceID().ToString(),GUILayout.Width(100));
                
                EditorGUILayout.BeginVertical(GUI.skin.box,GUILayout.Width(100));
                EditorGUILayout.LabelField("FixedTags");
                var fixedTags = _asset.GameplayTagAggregator.FixedTags.ConvertAll(tag => tag.Name);
                foreach (var tag in fixedTags)
                {
                    EditorGUILayout.LabelField(tag);
                }
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.BeginVertical(GUI.skin.box,GUILayout.Width(100));
                EditorGUILayout.LabelField("DynamicAddedTags");
                var dynamicAddedTags = _asset.GameplayTagAggregator.DynamicAddedTags.Keys.ToList().ConvertAll(tag => tag.Name);
                foreach (var tag in dynamicAddedTags)
                {
                    EditorGUILayout.LabelField(tag);
                }
                EditorGUILayout.EndVertical();
                
                var activeGameplayEffect = _asset.GameplayEffectContainer.ActiveGameplayEffects.ConvertAll(spec => spec.GameplayEffect.Asset.Name);
                EditorGUILayout.BeginVertical(GUI.skin.box,GUILayout.Width(100));
                EditorGUILayout.LabelField("ActiveGameplayEffect");
                foreach (var ge in activeGameplayEffect)
                {
                    EditorGUILayout.LabelField(ge);
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif