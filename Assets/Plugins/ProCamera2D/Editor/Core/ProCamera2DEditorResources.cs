using UnityEngine;
using UnityEditor;

public static class ProCamera2DEditorResources 
{
	public const string FolderPath = "ProCamera2D";

    public static Texture InspectorHeader
    {
        get
        {
            if (_inspectorHeader == null)
            {
                string path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("ProCamera2DEditor")[0]).Replace("ProCamera2DEditor.cs", "") + "Images/inspector_header.png";
                _inspectorHeader = (Texture)AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
            }
            return _inspectorHeader;
        }
    }
    static Texture _inspectorHeader;


    public static Texture UserGuideIcon
    {
        get
        {
            if (_userGuideIcon == null)
            {
                string path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("ProCamera2DEditor")[0]).Replace("ProCamera2DEditor.cs", "") + "Images/user-guide-link.png";
                _userGuideIcon = (Texture)AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
            }
            return _userGuideIcon;
        }
    }
    static Texture _userGuideIcon;
}