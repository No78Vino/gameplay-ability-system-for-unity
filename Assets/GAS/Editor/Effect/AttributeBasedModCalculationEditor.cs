// #if UNITY_EDITOR
// namespace GAS.Editor
// {
//     using System.Collections.Generic;
//     using System.Linq;
//     using GAS.Runtime;
//     using UnityEditor;
//     using UnityEngine;
//
//     [CustomEditor(typeof(AttributeBasedModCalculation))]
//     public class AttributeBasedModCalculationEditor : UnityEditor.Editor
//     {
//         private static List<string> _attributeOptions;
//         private AttributeBasedModCalculation Asset => (AttributeBasedModCalculation)target;
//
//         private static List<string> AttributeOptions
//         {
//             get
//             {
//                 if (_attributeOptions == null)
//                 {
//                     _attributeOptions = new List<string>();
//                     var asset = AttributeSetAsset.LoadOrCreate();
//                     foreach (var attributeSetConfig in asset.AttributeSetConfigs)
//                     {
//                         var config = attributeSetConfig;
//                         foreach (var fullName in attributeSetConfig.AttributeNames.Select(shortName =>
//                                      $"AS_{config.Name}.{shortName}"))
//                             _attributeOptions.Add(fullName);
//                     }
//                 }
//
//                 return _attributeOptions;
//             }
//         }
//
//         private void OnValidate()
//         {
//             Save();
//         }
//
//         public override void OnInspectorGUI()
//         {
//             EditorGUI.BeginChangeCheck();
//
//             EditorGUILayout.BeginVertical(GUI.skin.box);
//
//             EditorGUILayout.HelpBox(
//                 "AttributeBasedModCalculation：基于属性的计算\n该类型是根据属性值计算Modifier模值的，计算公式为：AttributeValue * k + b 计算逻辑与ScalableFloatModCalculation一致。\n重点在于属性值的来源: 从谁身上(Attribute From)以什么方式(Capture Type)捕获哪个属性的值(Attribute Name)。",
//                 MessageType.Info);
//
//             GUILayout.Space(20);
//             EditorGUILayout.HelpBox("[从谁身上] Source: [游戏效果]的来源（创建者）; Target: [游戏效果]的目标（拥有者）。", MessageType.None);
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField("Attribute From:", GUILayout.Width(100));
//             Asset.attributeFromType =
//                 (AttributeBasedModCalculation.AttributeFrom)EditorGUILayout.EnumPopup(Asset.attributeFromType);
//             EditorGUILayout.EndHorizontal();
//
//             GUILayout.Space(20);
//             EditorGUILayout.HelpBox("[以什么捕获方式] Track: 追踪（实时）; SnapShot: 快照，在[游戏效果]被创建时会对来源和目标的属性进行快照。", MessageType.None);
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField("Capture Type:", GUILayout.Width(100));
//             Asset.captureType =
//                 (AttributeBasedModCalculation.GEAttributeCaptureType)EditorGUILayout.EnumPopup(Asset.captureType);
//             EditorGUILayout.EndHorizontal();
//
//             GUILayout.Space(20);
//
//             EditorGUILayout.HelpBox("[捕获哪个属性的值]", MessageType.None);
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField("Attribute Name:", GUILayout.Width(100));
//             var indexOfTag = AttributeOptions.IndexOf(Asset.attributeName);
//             var idx = EditorGUILayout.Popup("", indexOfTag, AttributeOptions.ToArray());
//             idx = Mathf.Clamp(idx, 0, AttributeOptions.Count - 1);
//             Asset.attributeName = AttributeOptions[idx];
//             if (!string.IsNullOrEmpty(Asset.attributeName))
//             {
//                 var split = Asset.attributeName.Split('.');
//                 Asset.attributeSetName = split[0];
//                 Asset.attributeShortName = split[1];
//             }
//
//             EditorGUILayout.EndHorizontal();
//
//             GUILayout.Space(20);
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField("K:", GUILayout.Width(30));
//             Asset.k = EditorGUILayout.FloatField(Asset.k, GUILayout.Width(70));
//             EditorGUILayout.EndHorizontal();
//
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField("B:", GUILayout.Width(30));
//             Asset.b = EditorGUILayout.FloatField(Asset.b, GUILayout.Width(70));
//             EditorGUILayout.EndHorizontal();
//
//             EditorGUILayout.EndVertical();
//
//             if (EditorGUI.EndChangeCheck())
//             {
//                 Save();
//             }
//         }
//
//         private void Save()
//         {
//             EditorUtility.SetDirty(Asset);
//             AssetDatabase.SaveAssets();
//         }
//     }
// }
// #endif