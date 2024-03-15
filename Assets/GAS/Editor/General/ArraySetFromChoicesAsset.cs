#if UNITY_EDITOR
namespace GAS.Editor.General
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using GAS.Runtime;
    
    public class ArraySetFromChoicesAsset<T>
    {
        private List<T> choices;
        private string[] choiceNames;
        private CustomReorderableList<T> _reorderableList;
        
        string _title;
        
        public ArraySetFromChoicesAsset(T[] initData,List<T> choices,string title,Action<int,T> onEdit)
        {
            this.choices = choices;
            ChoiceNamesCache();
                
            _title = title;
            
            initData = initData ?? new T[0];
            _reorderableList = 
                CustomReorderableList<T>.Create(
                    initData ,
                    DrawListHeader,
                    onEdit,
                    ItemGUIDraw,
                    null,
                    null);
        }

        void ChoiceNamesCache()
        {
            switch (this.choices)
            {
                case List<string> stringChoices:
                    choiceNames = stringChoices.ToArray();
                    break;
                case List<ScriptableObject> soChoices:
                {
                    choiceNames=new string[soChoices.Count];
                    for (var i = 0; i < soChoices.Count; i++)
                    {
                        var path = AssetDatabase.GetAssetPath(soChoices[i]);
                        choiceNames[i] = path;
                    }

                    break;
                }
                case List<GameplayTag> tagChoices:
                {
                    choiceNames=new string[tagChoices.Count];
                    for (var i = 0; i < tagChoices.Count; i++)
                    {
                        choiceNames[i] = tagChoices[i].Name;
                    }

                    break;
                }
                default:
                {
                    choiceNames=new string[choices.Count];
                    for (var i = 0; i < choices.Count; i++)
                    {
                        choiceNames[i] = $"{i}:{choices[i].ToString()}";
                    }

                    break;
                }
            }
        }
        
        void DrawListHeader (Rect rect)
        {
            EditorGUI.LabelField(rect, _title);
        }

        void ItemGUIDraw(Rect rect, T item, int index)
        {
            var indexOfItem = choices.IndexOf(item);
            indexOfItem = Mathf.Clamp(indexOfItem, 0, choices.Count - 1);
            indexOfItem = EditorGUI.Popup(
                new Rect(rect.x, rect.y, 180, EditorGUIUtility.singleLineHeight),
                indexOfItem,
                choiceNames);
            T newItem = choices[indexOfItem];
            _reorderableList.UpdateItem(index, newItem);
        }

        public void OnGUI()
        {
            _reorderableList.OnGUI();
        }
        
        public List<T> GetItemList()
        {
            return _reorderableList.GetItemList();
        }
    }
}
#endif