using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Effect
{
    public class GameplayEffectMaker:EditorWindow
    {
        [MenuItem("EX GAS/GameplayEffectMaker")]
        static void Open()
        {
            var window = GetWindow<GameplayEffectMaker>();
            window.titleContent = new GUIContent("GameplayEffectMaker");
            window.Show();
        }
    }
}