using System;
using System.IO;
using GAS.Editor;
using GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset;
using GAS.RuntimeDataHelper.Helper;
using UnityEditor;
using UnityEngine;

namespace _ProjectCodeGenerate
{
    public static class _Generator_EffectConfigBase
    {
        [MenuItem("EX-GAS/_ProjectGenerator/_EffectConfig")]
        public static void Gen()
        {
            // var asset = GameplayTagsAsset.LoadOrCreate();
            // var tags = asset.Tags;
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath = $"{pathWithoutAssets}/Assets/GAS/RuntimeDataHelper/GameplayEffect/GameplayEffectConfigBase.gen.cs";
            GenerateGameplayEffectConfigBase(filePath);
        }

        public static void GenerateGameplayEffectConfigBase(string filePath)
        {
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");

            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using GAS.RuntimeDataHelper.GameplayEffect;");
            writer.WriteLine("using GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset;");
            writer.WriteLine("using Sirenix.OdinInspector;");

            
            writer.WriteLine("");
            
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.WriteLine("  [Serializable]");
            writer.WriteLine("  public class GameplayEffectConfigBase");
            writer.WriteLine("  {");
            writer.WriteLine("    [TabGroup(\"EffectConfig\",\"效果组件类型控制\",SdfIconType.TagsFill)]");
            writer.WriteLine("    [ValueDropdown(\"@EditGameplayEffectHelper.EffectComponentTypeChoices\", IsUniqueList = true, HideChildProperties = true)]");
            writer.WriteLine("    public List<string> configTypes = new();");
            writer.WriteLine("");
            var subTypes = EditGameplayEffectHelper.EffectComponentTypeChoices;
            
            foreach (var subType in subTypes)
            {
                writer.WriteLine($"    [TabGroup(\"EffectConfig\",\"配置详情\",SdfIconType.Activity)]");
                writer.WriteLine($"    [LabelText(\"{subType.Text}\")]");
                writer.WriteLine($"    [ShowIf(nameof(Has{subType.Text}))]");
                writer.WriteLine($"    [OnValueChanged(nameof(OnConfigValueChanged))]");
                if(subType.Text is nameof(ConfAssetEffectBasicInfo))
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
            writer.WriteLine("        //EditorUtility.SetDirty(this);");
            writer.WriteLine("        //AssetDatabase.SaveAssets();");
            writer.WriteLine("    }");
            
            writer.WriteLine("");
            
            writer.WriteLine("    public BaseGameplayEffectComponentConfigAsset GetConfigAsset(string type)");
            writer.WriteLine("    {");
            foreach (var subType in subTypes)
            {
                writer.WriteLine($"            if(type==typeof({subType.Text}).FullName)");
                writer.WriteLine($"                return Has{subType.Text}?{subType.Text}:null;");
            }
            writer.WriteLine("            return null;");
            writer.WriteLine("    }");
            
            writer.WriteLine("    protected bool ValidateList(List<string> _, ref string errorMsg)");
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
            

            Console.WriteLine($"Generated GameplayEffectConfigBase at path: {filePath}");
        }
    }
}