using System;
using DemoForESC._Script.Gen;
using GAS.ECS_TEST_RUNTIME_GEN_LIB;
using GAS.Runtime;
using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.Ability.Component.CommonAbilityLogic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.Cue;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.Modifier.CommonUsage;
using Unity.Collections;
using Unity.Entities;

namespace TestUnit_ForGASECS
{
    public class TestASCUnitUtils
    {
        /// <summary>
        /// GE普通攻击
        /// </summary>
        public static GameplayEffectComponentConfig[] GEConfig_ONEHIT =
        {
            new ConfEffectBasicInfo {Name = "Test_OneHit"},
            //new ConfAssetTags {tags = new []{GEN_GameplayTagCode.Magic_Fire}},
            // new ConfModifiers {modifierSettings = new []
            // {
            //     new ModifierSetting()
            //     {
            //         AttrSetCode = GEN_AttrSetCode.Fight_Monster,
            //         AttrCode = GEN_AttributeCode.HP,
            //         Operation = GEOperation.Minus,
            //         Magnitude = 10,
            //         MMC = new MMCSettingConfig()
            //         {
            //             TypeCode = MMCTypeToCode.Map[typeof(MMCScalableFloat)],
            //             floatParams = new []{0.5f,0},
            //         }
            //     }
            // }},
            new ConfCueOnExecution()
            {
                cues = new CueInstant[]{ new CueLog(new CueLogParameters(){Message = "普通攻击",SourceType = CueSourceType.GameplayEffect})} 
            }
        };
        
        /// <summary>
        /// GE普通攻击,要求earth tag
        /// </summary>
        public static GameplayEffectComponentConfig[] GEConfig_ONEHIT_REQUIRED_EARTH_TAG =
        {
            new ConfEffectBasicInfo {Name = "Test_OneHit_Earth"},
            //new ConfAssetTags {tags = new []{GEN_GameplayTagCode.Magic_Fire}},
            //new ConfApplicationRequiredTags{tags = new []{GEN_GameplayTagCode.Magic_Earth}},
            new ConfModifiers {modifierSettings = new []
            {
                new ModifierSetting()
                {
                    //AttrSetCode = GEN_AttrSetCode.Fight_Monster,
                    AttrCode = GEN_AttributeCode.HP,
                    Operation = GEOperation.Minus,
                    Magnitude = 20,
                    MMC = new MMCSettingConfig()
                    {
                        TypeCode = MMCTypeToCode.Map[typeof(MMCScalableFloat)],
                        floatParams = new []{0.5f,0},
                    }
                }
            }}
        };

        /// <summary>
        /// GE燃烧buff
        /// </summary>
        public static GameplayEffectComponentConfig[] GEConfig_BURNING =
        {
            new ConfEffectBasicInfo { Name = "Test_Burning" },
            //new ConfAssetTags { tags = new[] { GEN_GameplayTagCode.Magic_Fire } },
            new ConfDuration { duration = 60 * 5, timeUnit = TimeUnit.Frame },
            new ConfPeriod { Period = 30, GameplayEffectSettings = new[] { GEConfig_ONEHIT } },
            new ConfModifiers
            {
                modifierSettings = new[]
                {
                    new ModifierSetting()
                    {
                        //AttrSetCode = GEN_AttrSetCode.Fight_Monster,
                        AttrCode = GEN_AttributeCode.ATK,
                        Operation = GEOperation.Add,
                        Magnitude = 66,
                        MMC = new MMCSettingConfig()
                        {
                            TypeCode = MMCTypeToCode.Map[typeof(MMCScalableFloat)],
                            floatParams = new[] { 1f, 0 },
                        }
                    }
                }
            }
        };
        
        public static string[] FixedStringToStringArray(NativeArray<FixedString32Bytes> array)
        {
            var strings = new string[array.Length];
            for (var i = 0; i < array.Length; ++i)
                strings[i] = array[i].ToString();
            return strings;
        }

        public static AbilityConfig AbilityConfig_Debug = new(new GameplayAbilityComponentConfig[]
        {
            //new ConfAbilityBaseInfo { Code = GEN_AbilityCode.DebugLog },
            //new ConfAbilityAssetTags { tags = new[] { GEN_GameplayTagCode.Magic_Fire } },
            new MCConfAbilityLogic { AbilityLogicType = typeof(ALDebugLog).FullName },
        });
    }

    [Serializable]
    public struct AttributeSetForShow
    {
        public int Code;
        public AttributeForShow[] Attrs;
    }

    [Serializable]
    public struct AttributeForShow
    {
        public int Code;
        public float BaseValue;
        public float CurrentValue;
        public float MinValue;
        public float MaxValue;
    }
    
    [Serializable]
    public struct EffectForShow
    {
        // BasicData
        public string name;
        public string Target;
        public string Source;

        // Duration
        public int duration;
        public TimeUnit timeUnit;
        public bool active;

        // Period
        public int period;
        public string[] periodGameplayEffects;

        // Tags
        public int[] AssetTags;
        public int[] GrantedTags;
        public int[] ApplicationRequiredTags;
        public int[] OngoingRequiredTags;
        public int[] ImmunityTags;
        public int[] RemoveEffectWithTags;

        // Modifiers
        public ModifierSetting[] modifiers;


        public static EntityManager GasEntityManager => GASManager.EntityManager;

        public void SetGameplayEffectEntity(Entity geEntity)
        {
            SetBasicData(geEntity);
            SetDuration(geEntity);
            SetPeriod(geEntity);
        }


        public void SetBasicData(Entity geEntity)
        {
            var has = GasEntityManager.HasComponent<CEffectBasicInfo>(geEntity);
            var basicData = has ? GasEntityManager.GetComponentData<CEffectBasicInfo>(geEntity) : new CEffectBasicInfo();
            name = !has ? "NONE" : geEntity.ToString();
        }

        public void SetDuration(Entity geEntity)
        {
            var has = GasEntityManager.HasComponent<CDuration>(geEntity);
            var dur = has ? GasEntityManager.GetComponentData<CDuration>(geEntity) : new CDuration();
            duration = has ? dur.duration : 0;
            timeUnit = has ? dur.timeUnit : TimeUnit.Frame;
            active = has && dur.active;
        }

        public void SetPeriod(Entity geEntity)
        {
            var has = GasEntityManager.HasComponent<CPeriod>(geEntity);
            var p = has ? GasEntityManager.GetComponentData<CPeriod>(geEntity) : new CPeriod();
            var periodGEs = has ? new string[p.GameplayEffects.Length] : null;
            if (has)
                for (var j = 0; j < p.GameplayEffects.Length; j++)
                    periodGEs[j] = p.GameplayEffects[j].ToString();

            period = has ? p.Period : 0;
            periodGameplayEffects = periodGEs;
        }

        public void SetModifier(Entity geEntity)
        {
            var has = GasEntityManager.HasComponent<BEModifier>(geEntity);
            var mods = has
                ? GasEntityManager.GetBuffer<BEModifier>(geEntity)
                : new DynamicBuffer<BEModifier>();
            var settings = new ModifierSetting[mods.Length];
            if (has)
                for (var j = 0; j < mods.Length; j++)
                    settings[j] = new ModifierSetting
                    {
                        AttrSetCode = mods[j].AttrSetCode, AttrCode = mods[j].AttrCode,
                        Operation = mods[j].Operation, Magnitude = mods[j].Magnitude,
                        MMC = new MMCSettingConfig
                        {
                            TypeCode = mods[j].MMC.TypeCode,
                            floatParams = mods[j].MMC.floatParams.ToArray(),
                            intParams = mods[j].MMC.intParams.ToArray(),
                            stringParams = TestASCUnitUtils.FixedStringToStringArray(mods[j].MMC.stringParams)
                        }
                    };

            modifiers = settings;
        }
    }
}