using UnityEngine;
using UnityEditor;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [InitializeOnLoad]
    public class ProCamera2DHierarchyIcon : MonoBehaviour
    {
        static readonly Texture2D _icon;
        static readonly Texture2D _icon_plugin;

        static ProCamera2DHierarchyIcon()
        {
            _icon = AssetDatabase.LoadAssetAtPath("Assets/Gizmos/ProCamera2D/pro_camera_2d_icon.png", typeof(Texture2D)) as Texture2D;
            _icon_plugin = AssetDatabase.LoadAssetAtPath("Assets/Gizmos/ProCamera2D/pro_camera_2d_plugin_icon.png", typeof(Texture2D)) as Texture2D;

            if (_icon == null || _icon_plugin == null)
            {
                return;
            } 

            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
            EditorApplication.RepaintHierarchyWindow();
        }

        static void HierarchyItemCB(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (go == null)
            {
                return;
            }   

            if (_icon != null && go.GetComponent<ProCamera2D>() != null)
            {
                Rect r = new Rect(selectionRect);
                r.x = r.width - 5;

                GUI.Label(r, _icon);
                return;
            }

            if (_icon_plugin != null && go.GetComponent<BasePC2D>() != null)
            {
                Rect r = new Rect(selectionRect);
                r.x = r.width - 5;

                GUI.Label(r, _icon_plugin);
            }
        }
    }
}
