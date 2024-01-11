#if UNITY_EDITOR
namespace GAS.Editor.General
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    
    public class ScriptableObjectReorderableList<T> where T : ScriptableObject
    {
        private CustomReorderableList<T> _reorderableList;
        private string _title;
        
        public ScriptableObjectReorderableList(T[] initData,string title)
        {
            _title = title;
            initData = initData ?? new T[0];
            _reorderableList = 
                CustomReorderableList<T>.Create(
                    initData ,
                    DrawListHeader,
                    OnEdit,
                    ItemGUIDraw,
                    null,
                    null);
        }
        
        void OnEdit(int index, T item)
        {
            if (item == null)
            {
                EditorUtility.DisplayDialog("Warning", $"{typeof(T).Name} is null", "OK");
                return;
            }
            GeneralAssetFloatInspector.Open(item);
        }
        
        void DrawListHeader (Rect rect)
        {
            EditorGUI.LabelField(rect, _title);
        }

        void ItemGUIDraw(Rect rect, T item, int index)
        {
            item = EditorGUI.ObjectField(new Rect(rect.x, rect.y,
                    200, EditorGUIUtility.singleLineHeight), item, typeof(T),
                false) as T;

            _reorderableList.UpdateItem(index, item);
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