using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Runtime.Effects;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GAS.Editor.General
{
    public class ScriptableObjectUnionEditor : EditorWindow
    {
        Type scriptableObjectType;
        private ReorderableList reorderableList;
        private Vector2 scrollPosition;
        private string searchFilter = "";
        List<ScriptableObject> assets = new List<ScriptableObject>();
        
        UnityEditor.Editor editor;

        void SearchAllAssets()
        {
            var assetGUIDs = AssetDatabase.FindAssets("t:" + scriptableObjectType.Name + " " + searchFilter);
            assets = assetGUIDs.Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .Where(asset => asset != null)
                .ToList();
        }
        
        void Init<T>() where T : ScriptableObject
        {
            scriptableObjectType = typeof(T);
            SearchAllAssets();
            
            reorderableList = new ReorderableList(assets, typeof(T), false, false, true, true);

            reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "ScriptableObjects");
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = (T)reorderableList.list[index];
                var path = AssetDatabase.GetAssetPath(element);
                var assetName = path.Split('/').Last();
                assetName = assetName.Split(".asset").First();
                EditorGUI.LabelField(rect, assetName);
            };

            reorderableList.onSelectCallback = list =>
            {
                var asset = (T)reorderableList.list[list.index];
                Selection.activeObject = asset;
                editor = UnityEditor.Editor.CreateEditor(Selection.activeObject);
            };

            if (assets.Count > 0)
            {
                Selection.activeObject = assets.First();
                editor = UnityEditor.Editor.CreateEditor(Selection.activeObject);
            }
        }

        private void OnGUI()
        {
            if (scriptableObjectType == null) return;
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(300));
            
            searchFilter = EditorGUILayout.TextField("", searchFilter);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            reorderableList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
            
            
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (assets.Count == 0)
            {
                EditorGUILayout.LabelField("No assets found.");
            }
            else
            {
                editor.OnInspectorGUI();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        [MenuItem("Test Window/Custom ScriptableObject Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<ScriptableObjectUnionEditor>("ScriptableObject Editor");
            window.Init<GameplayEffectAsset>();
            window.minSize = new Vector2(800, 600);
            window.Show();
        }
    }
}