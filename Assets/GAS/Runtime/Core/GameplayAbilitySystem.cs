using System.Collections.Generic;
using GAS.General;
using GAS.Runtime;
using UnityEngine;

namespace GAS
{
    public class GameplayAbilitySystem
    {
        private static GameplayAbilitySystem _gas;
        private GasHost _gasHost;

        private GameplayAbilitySystem()
        {
            AbilitySystemComponents = new List<AbilitySystemComponent>();
            GASTimer.InitStartTimestamp();
            GasHost.gameObject.SetActive(true);
        }

        public List<AbilitySystemComponent> AbilitySystemComponents { get; }

        private GasHost GasHost
        {
            get
            {
                if (_gasHost == null)
                {
                    _gasHost = new GameObject("GAS Host").AddComponent<GasHost>();
                    _gasHost.hideFlags = HideFlags.HideAndDontSave;
                    Object.DontDestroyOnLoad(_gasHost.gameObject);
                }

                return _gasHost;
            }
        }

        public static GameplayAbilitySystem GAS
        {
            get
            {
                _gas ??= new GameplayAbilitySystem();
                return _gas;
            }
        }

        public bool IsPaused => !GasHost.enabled;

        public void Register(AbilitySystemComponent abilitySystemComponent)
        {
            if (!GasHost.enabled)
            {
                Debug.LogWarning("[EX] GAS is paused, can't register new ASC!");
                return;
            }

            if (AbilitySystemComponents.Contains(abilitySystemComponent)) return;
            AbilitySystemComponents.Add(abilitySystemComponent);
        }

        public bool Unregister(AbilitySystemComponent abilitySystemComponent)
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
    }
}