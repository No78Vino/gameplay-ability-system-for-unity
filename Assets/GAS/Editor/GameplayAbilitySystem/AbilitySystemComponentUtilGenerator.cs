#if UNITY_EDITOR
namespace GAS.Editor.GameplayAbilitySystem
{
    using System;
    using System.IO;
    using GAS.Core;
    using UnityEngine;
    
    
    public static class AbilitySystemComponentUtilGenerator
    {
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath = $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ASCUTIL_CSHARP_SCRIPT_NAME}";
            GenerateASCUtil(filePath);
        }

        private static void GenerateASCUtil( string filePath)
        {
            using var writer = new StreamWriter(filePath);

            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("using System;");
            writer.WriteLine("using GAS.Runtime.Ability;");
            writer.WriteLine("using GAS.Runtime.AttributeSet;");
            writer.WriteLine("using GAS.Runtime.Tags;");
            writer.WriteLine("namespace GAS.Runtime.Component");
            writer.WriteLine("{");
            writer.WriteLine("      public static class AbilitySystemComponentExtension");
            writer.WriteLine("      {");
            writer.WriteLine("          public static Type[] PresetAttributeSetTypes(this AbilitySystemComponent asc)");
            writer.WriteLine("          {");
            writer.WriteLine("              if (asc.Preset == null) return null;");
            writer.WriteLine("              var attrSetTypes = new Type[asc.Preset.AttributeSets.Length];");
            writer.WriteLine("              for (var i = 0; i < asc.Preset.AttributeSets.Length; i++)");
            writer.WriteLine("                  attrSetTypes[i] = AttrSetUtil.AttrSetTypeDict[asc.Preset.AttributeSets[i]];");
            writer.WriteLine("              return attrSetTypes;");
            writer.WriteLine("          }");
            writer.WriteLine("          public static AbilityInstanceInfo[] PresetAbilityInstanceInfos(this AbilitySystemComponent asc)");
            writer.WriteLine("          {");
            writer.WriteLine("              if (asc.Preset == null) return null;");
            writer.WriteLine("              AbilityInstanceInfo[] infos=new AbilityInstanceInfo[asc.Preset.BaseAbilities.Length];");
            writer.WriteLine("              for (var i = 0; i < asc.Preset.BaseAbilities.Length; i++)");
            writer.WriteLine("              {");
            writer.WriteLine("                  var abilityAsset = asc.Preset.BaseAbilities[i];");
            writer.WriteLine("                  infos[i] = new AbilityInstanceInfo()");
            writer.WriteLine("                  {");
            writer.WriteLine("                      abilityAsset =  abilityAsset,");
            writer.WriteLine("                      abilityType = AbilityCollection.AbilityMap[abilityAsset.UniqueName].AbilityClassType");
            writer.WriteLine("                  };");
            writer.WriteLine("              }");
            writer.WriteLine("              return infos;");
            writer.WriteLine("          }");
            writer.WriteLine("          public static GameplayTag[] PresetBaseTags(this AbilitySystemComponent asc)");
            writer.WriteLine("          {");
            writer.WriteLine("              if (asc.Preset == null) return null;");
            writer.WriteLine("              return asc.Preset.BaseTags;");
            writer.WriteLine("          }");
            writer.WriteLine("          public static void InitWithPreset(this AbilitySystemComponent asc,int level, AbilitySystemComponentPreset preset = null)");
            writer.WriteLine("          {");
            writer.WriteLine("              asc.SetLevel(level);");
            writer.WriteLine("              if (preset != null) asc.SetPreset(preset);");
            writer.WriteLine("              if (asc.Preset == null) return;");
            writer.WriteLine("              asc.Init(asc.PresetBaseTags(), asc.PresetAttributeSetTypes(), asc.PresetAbilityInstanceInfos());");
            writer.WriteLine("          }");
            writer.WriteLine("      }");
            writer.WriteLine("}");
            
            Console.WriteLine($"Generated AbilitySystemComponentExtension at path: {filePath}");
        }
    }
}
#endif