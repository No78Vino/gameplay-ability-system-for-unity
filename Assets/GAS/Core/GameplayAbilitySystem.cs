using System.Collections.Generic;
using EXTool;
using GAS.Runtime.AbilitySystemComponent;
using UnityEngine;

namespace GAS.Core
{

    public class GameplayAbilitySystem
    {
        private GasHost _gasHost;
        private List<AbilitySystemComponent> _abilitySystemComponents;
        public List<AbilitySystemComponent> AbilitySystemComponents => _abilitySystemComponents;

        private GasHost GasHost
        {
            get
            {
                if (_gasHost == null) _gasHost = new GameObject("GasHost").AddComponent<GasHost>();

                return _gasHost;
            }
        }

        private static GameplayAbilitySystem _gas;
        public static GameplayAbilitySystem GAS
        {
            get
            {
                _gas ??= new GameplayAbilitySystem();;
                return _gas;
            }
        }

        private GameplayAbilitySystem()
        {
            _abilitySystemComponents = new List<AbilitySystemComponent>();
            GasHost.gameObject.SetActive(true);
        }
        
        public void Register(AbilitySystemComponent abilitySystemComponent)
        {
            if (!GasHost.enabled)
            {
                EXLog.Warning("GAS is paused, can't register new ASC!");
                return;
            }
            if(_abilitySystemComponents.Contains(abilitySystemComponent)) return;
            _abilitySystemComponents.Add(abilitySystemComponent);
        }
        
        public bool Unregister(AbilitySystemComponent abilitySystemComponent)
        {
            if (!GasHost.enabled)
            {
                EXLog.Warning("GAS is paused, can't unregister ASC!");
                return false;
            }
            return _abilitySystemComponents.Remove(abilitySystemComponent);
        }
        
        public void Pause()
        {
            GasHost.enabled = false;
        }
        
        public void Unpause()
        {
            GasHost.enabled = true;
        }

        public bool IsPaused => !GasHost.enabled;
    }
}