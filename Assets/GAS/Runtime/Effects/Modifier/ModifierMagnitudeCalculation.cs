using UnityEditor;
using UnityEngine;

namespace GAS.Runtime
{
    public abstract class ModifierMagnitudeCalculation : ScriptableObject
    {
        public abstract float CalculateMagnitude(GameplayEffectSpec spec, float modifierMagnitude);

#if UNITY_EDITOR
        private void OnValidate()
        {
            // if(Application.isPlaying) return;
            // EditorUtility.SetDirty(this);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
        }
#endif
    }
}