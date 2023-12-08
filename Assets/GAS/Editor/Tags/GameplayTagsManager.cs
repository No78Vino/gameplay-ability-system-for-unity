using UnityEditor;

namespace GAS.Editor.Tags
{
    public class GameplayTagsManager:EditorWindow
    {
        [MenuItem("GAS/Gameplay Tags Manager")]
        public static void Open()
        {
            GetWindow<GameplayTagsManager>("Gameplay Tags Manager");
        }
    }
}