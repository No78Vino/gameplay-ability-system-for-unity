using System;
using UnityEditor;
using System.Linq;
using GAS.General.Validation;
using UnityEngine;

namespace GAS.Editor
{
    public static class ScriptingDefineSymbolsHelper
    {
        public static void Add(string define)
        {
            if (!Validations.IsValidVariableName(define))
            {
                Debug.LogError($"Add scripting define symbol failed: {define}");
                return;
            }

            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            if (!definesString.Contains(define))
            {
                var newDefinesString = string.Join(";", definesString.Split(';').Append(define));
                Debug.Log($"change define: {definesString} -> {newDefinesString}");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, newDefinesString);
            }
        }

        public static void Remove(string define)
        {
            if (string.IsNullOrWhiteSpace(define))
            {
                Debug.LogError($"Remove scripting define symbol error: {define}");
                return;
            }

            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            if (definesString.Contains(define))
            {
                var newDefinesString = string.Join(";", definesString.Split(';').Where(d => d != define));
                Debug.Log($"change define: {definesString} -> {newDefinesString}");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, newDefinesString);
            }
        }
    }
}