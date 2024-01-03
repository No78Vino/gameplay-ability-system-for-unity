#if UNITY_EDITOR
namespace GAS.Editor.GameplayAbilitySystem
{
    using System.Collections.Generic;
    using Runtime.Component;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;
    
    public class GASWatcher : OdinEditorWindow
    {
        private const string BOXGROUP_TIPS = "Tips";
        private const string BOXGROUP_TIPS_RUNNINGTIP = "Tips/Running tip";
        private const string BOXGROUP_ASC = "Ability System Components";
        
        private bool IsPlaying => Application.isPlaying;
        
        [HideLabel] 
        [DisplayAsString(TextAlignment.Center, true)]
        public string windowTitle = "<size=18><b>EX Gameplay Ability System Watcher</b></size>";

        [BoxGroup(BOXGROUP_TIPS)] 
        [HideLabel] 
        [DisplayAsString(TextAlignment.Left, true)]
        public string tips = "This window is used to monitor the runtime state of the Gameplay Ability System. \n" +
                             "It is recommended to open this window in the editor when debugging the Gameplay Ability System. ";

        [BoxGroup(BOXGROUP_TIPS_RUNNINGTIP, false)]
        [HideLabel]
        [DisplayAsString(TextAlignment.Center, true)]
        [HideIf("IsPlaying")]
        public string onlyForGameRunning =
            "<size=16><b><color=yellow>This monitor is only available when the game is running.</color></b></size>";

        [BoxGroup(BOXGROUP_ASC, true, true)]
        [TableList(AlwaysExpanded = true, HideToolbar = true, IsReadOnly = true)]
        [ShowIf("IsPlaying")]
        public List<IAbilitySystemComponent> AbilitySystemComponents = new List<IAbilitySystemComponent>();

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying) AbilitySystemComponents = Core.GameplayAbilitySystem.GAS.AbilitySystemComponents;
        }

        [MenuItem("EX-GAS/Runtime GAS Watcher", priority = 3)]
        private static void OpenWindow()
        {
            var window = GetWindow<GASWatcher>();
            window.Show();
        }
    }
}
#endif