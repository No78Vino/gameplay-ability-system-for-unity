// http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset

using UnityEngine;
using UnityEditor;
using System.IO;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public static class ScriptableObjectUtility
    {
        /// <summary>
        ///  This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateAsset<T>(string name = null) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            var assetPathAndName = "";
            if(string.IsNullOrEmpty(name))
                assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T) + ".asset");
            else
                assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + name + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
    }
}