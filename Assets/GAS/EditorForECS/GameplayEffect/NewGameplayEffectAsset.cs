using System.Collections.Generic;
using GAS.RuntimeWithECS.GameplayEffect;
using UnityEngine;

namespace GAS.EditorForECS.GameplayEffect
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
    public class NewGameplayEffectAsset : ScriptableObject
    {
        public GameplayEffectComponentConfig[] components;
    }
}