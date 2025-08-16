///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using GAS.RuntimeWithECS;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.ComponentConfig;
using GAS.RuntimeWithECS.GameplayEffect;
using SimpleJSON;
using UnityEngine;

namespace GAS.Runtime
{
    public static class XLubanExtension
    {
        public const string GAME_CONF_DIR = "Assets/DemoForESC/Resources/Tables";
        
        private static cfg.Tables _tables;

        public static cfg.Tables Tables
        {
            get
            {
                if (_tables == null) LoadTables();
                return _tables;
            }
        }

        public static void LoadTables()
        {
            if (_tables != null) return; // Already loaded
            _tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText($"{GAME_CONF_DIR}/{file}.json")));
        }

        public static AbilitySystemCellConfig GetAscConfig(int id)
        {
            var data = _tables.Tbasc.Get(id);
            if (data == null)
            {
                Debug.LogError($"ASC_ID:{id}  不存在.");
                return new AbilitySystemCellConfig(
                    Array.Empty<int>(), Array.Empty<int>(), 
                    Array.Empty<AbilityConfig>(), 0);
            }
            
            var abilityIds = data.Ability;
            var abilities = new AbilityConfig[abilityIds.Length];
            for (var i = 0; i < abilityIds.Length; i++)
            {
                var abilityId = abilityIds[i];
                abilities[i] = GetAbilityConfig(abilityId);
            }
            return new AbilitySystemCellConfig(data.Tag, data.AttrSet, abilities, data.Level);
        }
        // public static CueConfig GetGameplayCueConfig(int id)
        // {
        //     var configs = new GameplayAbilityComponentConfig[0];
        //     return new AbilityConfig(configs);
        // }
        
        public static GameplayCueConfig GetGameplayCueConfig(int id)
        {
            var data = _tables.TbgameplayCue.Get(id);
            if (data == null)
            {
                Debug.LogError($"Cue_ID:{id}  不存在.");
                return null;
            }

            var cueType = CueHelper.GetCueType(data.CueLogic.GetType().Name);
            if (cueType == null)
            {
                Debug.LogError($"Cue_ID:{id}  CueType:{data.CueLogic.GetType().Name} 不存在.");
                return null;
            }
            // TODO
            var cueLogic = data.CueLogic;
            ICueParameter cueParam = null;
            if (cueLogic is cfg.GameplayCueLog gameplayCueLog)
            {
                cueParam = new CueParamString(gameplayCueLog.Value);
            }
            return new GameplayCueConfig(cueType, cueParam, data.RequiredTag.ToArray(), data.ImmunityTag.ToArray());
        }
        
        
        public static GameplayEffectConfig GetGameplayEffectConfig(int id)
        {
            var configs = new GameplayEffectComponentConfig[0];
            return new GameplayEffectConfig(configs);
        }
        
        public static AbilityConfig GetAbilityConfig(int id)
        {
            var data = _tables.Tbability.Get(id);
            if (data == null)
            {
                Debug.LogError($"Ability_ID:{id}  不存在.");
                return new AbilityConfig(Array.Empty<GameplayAbilityComponentConfig>());
            }
            var configs = new List<GameplayAbilityComponentConfig>();
            // cost	cdEffect cd	assetTags	cancelAbilityWithTags	blockAbilityWithTags	activationOwnedTags	activationRequiredTags	activationBlockedTags	abilityLogic													
            if (data.Cost != 0)
            {
                var costConfig = _tables.TbgameplayEffect.Get(data.Cost);
                // if (costConfig != null)
                // {
                //     configs.Add(costConfig.GetConfig());
                // }
            }
            return new AbilityConfig(configs.ToArray());
        }
    }
}