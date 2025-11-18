using UnityEngine;
using System;
using UnityEditor;

namespace Cinemachine.Editor
{
    /// <summary>
    ///  This class contains setting for the Impulse system.  Specifically, it holds
    ///  the Impulse Channel definitions.  These work like Unity Layers, you can
    ///  define and name them, and create masks to filter only the layers you want.
    /// </summary>
    [Serializable]
    public class CinemachineImpulseChannels : ScriptableObject 
    {
        static CinemachineImpulseChannels sInstance = null;
        private static bool alreadySearched = false;

        /// <summary>Get the singleton instance of this object, or null if it doesn't exist</summary>
        public static CinemachineImpulseChannels InstanceIfExists
        {
            get
            {
                if (!alreadySearched)
                {
                    alreadySearched = true;
                    var guids = AssetDatabase.FindAssets("t:CinemachineImpulseChannels");
                    for (int i = 0; i < guids.Length && sInstance == null; ++i)
                        sInstance = AssetDatabase.LoadAssetAtPath<CinemachineImpulseChannels>(
                            AssetDatabase.GUIDToAssetPath(guids[i]));
                }
                if (sInstance != null)
                    sInstance.EnsureDefaultLayer();
                return sInstance;
            }
        }

        /// <summary>Get the singleton instance of this object.  Creates asset if nonexistant</summary>
        public static CinemachineImpulseChannels Instance
        {
            get
            {
                if (InstanceIfExists == null)
                {
                    string newAssetPath = EditorUtility.SaveFilePanelInProject(
                            "Create Impulse Channel Definition asset", "CinemachineImpulseChannels", "asset", 
                            "This editor-only file will contain the Impulse channels for this project");
                    if (!string.IsNullOrEmpty(newAssetPath))
                    {
                        sInstance = CreateInstance<CinemachineImpulseChannels>();
                        AssetDatabase.CreateAsset(sInstance, newAssetPath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }

                if (sInstance != null)
                {
                    sInstance.EnsureDefaultLayer();
                }
                return sInstance;
            }
        }

        // Make sure it has at least one layer in it
        void EnsureDefaultLayer()
        {
            if (ImpulseChannels == null || ImpulseChannels.Length == 0)
                ImpulseChannels = new string[] { "default" };
        }

        /// <summary>The currently-defined Impulse channel names</summary>
        public string[] ImpulseChannels;
    }
}
