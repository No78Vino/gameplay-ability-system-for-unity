using GAS.RuntimeDataHelper.Ability;
using GAS.RuntimeDataHelper.Tag;
using UnityEditor;

namespace GAS.Editor
{
    public static class GeneratorAll
    {
        [MenuItem("EX-GAS/CodeGenerate/Generate All")]
        public static void Gen()
        {
            GeneratorAbilityCodeLib.Gen();
            GeneratorGameplayTagCode.Gen();
        }
    }
}