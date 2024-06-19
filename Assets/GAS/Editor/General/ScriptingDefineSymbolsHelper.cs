using System.Linq;
using GAS.General.Validation;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    public static class ScriptingDefineSymbolsHelper
    {
        public static void Add(string define)
        {
            if (!Validations.IsValidVariableName(define))
            {
                Debug.LogError($@"Add scripting define symbol error: ""{define}"" is not a valid variable name!");
                return;
            }

            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var defines = definesString.Split(';');
            if (defines.Contains(define))
            {
                Debug.Log($@"Add scripting define symbol failed: ""{define}"" already exists!");
                return;
            }

            var newDefinesString = string.Join(";", defines.Append(define));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, newDefinesString);
        }

        public static void Remove(string define)
        {
            if (string.IsNullOrWhiteSpace(define))
            {
                Debug.LogError($@"Remove scripting define symbol error: ""{define}"" is null or empty!");
                return;
            }

            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var defines = definesString.Split(';');
            if (!defines.Contains(define))
            {
                Debug.Log($@"Remove scripting define symbol failed: ""{define}"" does not exist!");
                return;
            }

            var newDefinesString = string.Join(";", defines.Where(d => d != define));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, newDefinesString);
        }
    }
}