using System.Collections.Generic;
using GAS.Runtime.Component;
using UnityEngine;

namespace GAS.Core
{
    public class GameplayAbilitySystem
    {
        private static GameplayAbilitySystem _gas;
        private GasHost _gasHost;

        private GameplayAbilitySystem()
        {
            AbilitySystemComponents = new List<IAbilitySystemComponent>();
            GasHost.gameObject.SetActive(true);
        }

        public List<IAbilitySystemComponent> AbilitySystemComponents { get; }

        private GasHost GasHost
        {
            get
            {
                if (_gasHost == null) _gasHost = new GameObject("GAS Host").AddComponent<GasHost>();

                return _gasHost;
            }
        }

        public static GameplayAbilitySystem GAS
        {
            get
            {
                _gas ??= new GameplayAbilitySystem();
                ;
                return _gas;
            }
        }

        public bool IsPaused => !GasHost.enabled;

        public void Register(IAbilitySystemComponent abilitySystemComponent)
        {
            if (!GasHost.enabled)
            {
                Debug.LogWarning("[EX] GAS is paused, can't register new ASC!");
                return;
            }

            if (AbilitySystemComponents.Contains(abilitySystemComponent)) return;
            AbilitySystemComponents.Add(abilitySystemComponent);
        }

        public bool Unregister(IAbilitySystemComponent abilitySystemComponent)
        {
            if (!GasHost.enabled)
            {
                Debug.LogWarning("[EX] GAS is paused, can't unregister ASC!");
                return false;
            }

            return AbilitySystemComponents.Remove(abilitySystemComponent);
        }

        public void Pause()
        {
            GasHost.enabled = false;
        }

        public void Unpause()
        {
            GasHost.enabled = true;
        }
        
        public void Reset()
        {
            AbilitySystemComponents.Clear();
        }
        
        #if UNITY_EDITOR
        public GasHost GetGasHost()
        {
            return GasHost;
        }
        #endif
    }
}