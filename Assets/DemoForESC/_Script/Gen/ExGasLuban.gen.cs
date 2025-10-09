///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;
using cfg;
using SimpleJSON;
using System.IO;
using UnityEngine;
using GAS.RuntimeWithECS;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.ComponentConfig;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.Static;

namespace GAS.Runtime
{
    public static class EXLuban
    {
        public const string GAME_CONF_DIR = "Assets/DemoForESC/Resources/Tables";
        private static Tables _tables;
        public static Tables Tables
        {
            get
            {
                if (_tables == null) LoadTables();
                return _tables;
            }
        }

        public static void LoadTables()
        {
            if (_tables != null) return;
            _tables = new Tables(file => JSON.Parse(File.ReadAllText($"{GAME_CONF_DIR}/{file}.json")));
        }

        public static AbilitySystemCellConfig GetAscConfig(int id)
        {
            var data = Tables.Tbasc.Get(id);
            if (data == null)
            {
                Debug.LogError($"ASC_ID:{id}  不存在.");
                return new AbilitySystemCellConfig(Array.Empty<int>(), Array.Empty<int>(),Array.Empty<AbilityConfig>(), 0);
            }
            var abilityIds = data.Ability;
            var abilities = new AbilityConfig[abilityIds.Length];
            for (var i = 0; i < abilityIds.Length; i++)
            {
                var abilityId = abilityIds[i];
                //abilities[i] = GetAbilityConfig(abilityId);
            }
            return new AbilitySystemCellConfig(data.Tag, data.AttrSet, abilities, data.Level);
        }

        public static GameplayCueConfig GetGameplayCueConfig(int id)
        {
            var data = Tables.TbgameplayCue.Get(id);
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
            var cueLogic = data.CueLogic;
            var cueLogicName = cueLogic.GetType().Name;
            var cueParamType = CueHelper.GetCueLogicParamType(cueLogicName);
            var cueParam = Activator.CreateInstance(cueParamType) as ICueParameter;
            if (cueParam != null)
            {
                if (cueLogic is cfg.GameplayCueLog gameplayCueLog)
                {
                    var gameplayCueLogParam = cueParam as GAS.Runtime.CueParamString;
                    gameplayCueLogParam?.SetValue(gameplayCueLog.Value);
                    cueParam = gameplayCueLogParam;
                }
            }
            return new GameplayCueConfig(cueType, cueParam, data.RequiredTag.ToArray(), data.ImmunityTag.ToArray());
        }
    }
}
