﻿using System.Collections.Generic;
using GAS.General;
using GAS.Runtime;
using UnityEngine;
using UnityEngine.Profiling;

namespace GAS
{
    public class GameplayAbilitySystem
    {
        private static GameplayAbilitySystem _gas;

        private GameplayAbilitySystem()
        {
            const int capacity = 1024;
            AbilitySystemComponents = new List<AbilitySystemComponent>(capacity);
            GASTimer.InitStartTimestamp();

            GasHost = new GameObject("GAS Host").AddComponent<GasHost>();
            GasHost.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(GasHost.gameObject);
            GasHost.gameObject.SetActive(true);
        }

        public List<AbilitySystemComponent> AbilitySystemComponents { get; }

        private GasHost GasHost { get; }

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
            // if (!GasHost.enabled)
            // {
            //     Debug.LogWarning("[EX] GAS is paused, can't register new ASC!");
            //     return;
            // }

            if (AbilitySystemComponents.Contains(abilitySystemComponent)) return;
            AbilitySystemComponents.Add(abilitySystemComponent);
        }

        public bool Unregister(AbilitySystemComponent abilitySystemComponent)
        {
            // if (!GasHost.enabled)
            // {
            //     Debug.LogWarning("[EX] GAS is paused, can't unregister ASC!");
            //     return false;
            // }

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

        public void ClearComponents()
        {
            foreach (var t in AbilitySystemComponents)
                t.Disable();

            AbilitySystemComponents.Clear();
        }

        public void Tick()
        {
            Profiler.BeginSample($"{nameof(GameplayAbilitySystem)}::Tick()");

            var abilitySystemComponents = ObjectPool.Instance.Fetch<List<AbilitySystemComponent>>();
            abilitySystemComponents.AddRange(AbilitySystemComponents);

            foreach (var abilitySystemComponent in abilitySystemComponents)
            {
                abilitySystemComponent.Tick();
            }

            abilitySystemComponents.Clear();
            ObjectPool.Instance.Recycle(abilitySystemComponents);

            Profiler.EndSample();
        }
    }
}