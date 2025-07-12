using System;
using System.IO;
using System.Linq;
using GAS.Runtime;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    public class AbilityCollectionGenerator
    {
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ABILITY_LIB_CSHARP_SCRIPT_NAME}";
            GenerateAbilityCollection(filePath);
        }

        private static void GenerateAbilityCollection(string filePath)
        {
            //using var writer = new IndentedWriter(new StreamWriter(filePath));
        }
    }
}