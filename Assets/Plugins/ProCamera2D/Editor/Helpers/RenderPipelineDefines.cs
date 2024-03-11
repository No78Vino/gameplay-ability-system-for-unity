using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

#if UNITY_2019_3_OR_NEWER
namespace Com.LuisPedroFonseca.ProCamera2D
{
    public static class RenderPipelineDefines
    {
        private const string HDRP_PACKAGE = "com.unity.render-pipelines.high-definition";
        private const string URP_PACKAGE = "com.unity.render-pipelines.universal";
 
        private const string TAG_HDRP = "USING_HDRP";
        private const string TAG_URP = "USING_URP";
 
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            var packagesRequest = Client.List(true);
            LoadPackages(packagesRequest);
        }
        
        private static void LoadPackages (ListRequest request)
        {
            if (request == null)
                return;
 
            // Wait for request to complete
            while (!request.IsCompleted)
            {
                Task.Delay(1_000).Wait();
            }
            
            if (request.Result == null)
                return;
 
            // Find out what packages are installed
            var packagesList = request.Result.ToList();
 
            var hasHDRP = packagesList.Find(x => x.name.Contains(HDRP_PACKAGE)) != null;
            var hasURP = packagesList.Find(x => x.name.Contains(URP_PACKAGE)) != null;
 
            DefinePreProcessors(hasHDRP, hasURP);
        }
 
        private static void DefinePreProcessors(bool defineHDRP, bool defineURP)
        {
            string originalDefineSymbols;
            string newDefineSymbols;
 
            List<string> defined;
            var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
 
            originalDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
            defined = originalDefineSymbols.Split(';').Where(x => !String.IsNullOrWhiteSpace(x)).ToList();

            void AppendRemoveTag(bool stat, string tag)
            {
                if (stat && !defined.Contains(tag))
                    defined.Add(tag);
                else if (!stat && defined.Contains(tag)) 
                    defined.Remove(tag);
            }

            AppendRemoveTag(defineHDRP, TAG_HDRP);
            AppendRemoveTag(defineURP, TAG_URP);
 
            newDefineSymbols = string.Join(";", defined);
            if(originalDefineSymbols != newDefineSymbols)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newDefineSymbols);
            }
        }
    }
}
#endif