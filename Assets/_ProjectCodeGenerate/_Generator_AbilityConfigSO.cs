using System;
using System.IO;
using GAS.Editor;
using GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset;
using GAS.RuntimeDataHelper.Helper;
using UnityEditor;
using UnityEngine;

namespace _ProjectCodeGenerate
{
    public static class _Generator_AbilityConfigSO
    {
        [MenuItem("EX-GAS/_ProjectGenerator/_AbilityConfigSO", priority = 0)]
        public static void Gen()
        {
            // var asset = GameplayTagsAsset.LoadOrCreate();
            // var tags = asset.Tags;
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath = $"{pathWithoutAssets}/Assets/GAS/RuntimeDataHelper/Ability/GEN_AbilityConfigSO.cs";
            GenerateAbilityConfigSO(filePath);
        }
        
         public static void GenerateAbilityConfigSO(string filePath)
        {
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");

            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset;");
            writer.WriteLine("using Sirenix.OdinInspector;");
            writer.WriteLine("using UnityEditor;");
            writer.WriteLine("using UnityEngine;");
            
            writer.WriteLine("");
            
            writer.WriteLine("namespace GAS.RuntimeDataHelper.Ability");
            writer.WriteLine("{");
            writer.WriteLine("  public class GEN_AbilityConfigSO:ScriptableObject");
            writer.WriteLine("  {");
            writer.WriteLine("    [TabGroup(\"AbilityConfig\",\"能力组件类型控制\",SdfIconType.TagsFill)]");
            writer.WriteLine("    [ValueDropdown(\"@EXEditorHelper.AbilityComponentTypeChoices\", IsUniqueList = true, HideChildProperties = true)]");
            writer.WriteLine("    public List<string> configTypes = new();");
            writer.WriteLine("");
            var subTypes = EXEditorHelper.AbilityComponentTypeChoices;
            
            foreach (var subType in subTypes)
            {
                writer.WriteLine($"    [TabGroup(\"AbilityConfig\",\"组件配置详情\",SdfIconType.Activity)]");
                writer.WriteLine($"    [LabelText(\"{subType.Text}\")]");
                writer.WriteLine($"    [ShowIf(nameof(Has{subType.Text}))]");
                writer.WriteLine($"    [OnValueChanged(nameof(OnConfigValueChanged))]");
                if(subType.Text is nameof(ConfAssetAbilityBaseInfo) or nameof(MCConfAssetAbilityLogic))
                    writer.WriteLine($"    [PropertyOrder(-1)]");
                writer.WriteLine($"    public {subType.Text} {subType.Text};");
                writer.WriteLine("");
            }
            
            foreach (var subType in subTypes)
            {
                writer.WriteLine($"    protected bool Has{subType.Text} => ");
                writer.WriteLine($"        configTypes.Any( x => x == typeof({subType.Text}).FullName);");
            }
            
            writer.WriteLine("");
            
            writer.WriteLine("    protected void OnConfigValueChanged()");
            writer.WriteLine("    {");
            writer.WriteLine("        CheckComponentConfigOwnAsset();");
            writer.WriteLine("        EditorUtility.SetDirty(this);");
            writer.WriteLine("        //AssetDatabase.SaveAssets();");
            writer.WriteLine("    }");
            
            writer.WriteLine("");
            
            writer.WriteLine("    protected BaseGameplayAbilityComponentConfigAsset GetConfigAsset(string type)");
            writer.WriteLine("    {");
            foreach (var subType in subTypes)
            {
                writer.WriteLine($"            if(type==typeof({subType.Text}).FullName)");
                writer.WriteLine($"                return Has{subType.Text}?{subType.Text}:null;");
            }
            writer.WriteLine("            return null;");
            writer.WriteLine("    }");
            
            writer.WriteLine("    protected virtual bool ValidateList(List<string> _, ref string errorMsg)");
            writer.WriteLine("    {");
            writer.WriteLine("        return false;");
            writer.WriteLine("    }");
            
            writer.WriteLine("    [OnInspectorInit]");
            writer.WriteLine("    private void InitializeList()");
            writer.WriteLine("    {");
            writer.WriteLine("        CheckComponentConfigOwnAsset();");
            writer.WriteLine("    }");
                
            writer.WriteLine("");
            
            writer.WriteLine("    protected void CheckComponentConfigOwnAsset()");
            writer.WriteLine("    {");
            foreach (var subType in subTypes)
            {
                writer.WriteLine($"        {subType.Text}?.SetOwnAsset(this);");
            }
            writer.WriteLine("    }");
            writer.WriteLine("  }");
            writer.WriteLine("}");
            

            Console.WriteLine($"Generated GEN_AbilityConfigSO at path: {filePath}");
        }
    }
}