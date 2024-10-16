using System;
using GAS.ECS_TEST_RUNTIME_GEN_LIB;
using GAS.Runtime;
using GAS.RuntimeWithECS.Core;
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
            new ConfBasicInfo {Name = "Test_OneHit"},
            new ConfAssetTags {tags = new []{GTagList.Magic_Fire}},
            new ConfModifiers {modifierSettings = new []
            {
                new ModifierSetting()
                {
                    AttrSetCode = EcsGAttrSetCode.Fight_Monster,
                    AttrCode = EcsGAttrLib.HP,
                    Operation = GEOperation.Minus,
                    Magnitude = 10,
                    MMC = new MMCSettingConfig()
                    {
                        TypeCode = MMCTypeToCode.Map[typeof(MMCScalableFloat)],
                        floatParams = new []{0.5f,0},
                    }
                }
            }}
        };
        
        /// <summary>
        /// GE普通攻击,要求earth tag
        /// </summary>
        public static GameplayEffectComponentConfig[] GEConfig_ONEHIT_REQUIRED_EARTH_TAG =
        {
            new ConfBasicInfo {Name = "Test_OneHit"},
            new ConfAssetTags {tags = new []{GTagList.Magic_Fire}},
            new ConfApplicationRequiredTags{tags = new []{GTagList.Magic_Earth}},
            new ConfModifiers {modifierSettings = new []
            {
                new ModifierSetting()
                {
                    AttrSetCode = EcsGAttrSetCode.Fight_Monster,
                    AttrCode = EcsGAttrLib.HP,
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
            new ConfBasicInfo { Name = "Test_Burning" },
            new ConfAssetTags { tags = new[] { GTagList.Magic_Fire } },
            new ConfDuration { duration = 60 * 5, timeUnit = TimeUnit.Frame },
            new ConfPeriod { Period = 30, GameplayEffectSettings = new[] { GEConfig_ONEHIT } },
            // new ConfModifiers
            // {
            //     modifierSettings = new[]
            //     {
            //         new ModifierSetting()
            //         {
            //             AttrSetCode = EcsGAttrSetCode.Fight_Monster,
            //             AttrCode = EcsGAttrLib.HP,
            //             Operation = GEOperation.Minus,
            //             Magnitude = 10,
            //             MMC = new MMCSettingConfig()
            //             {
            //                 TypeCode = MMCTypeToCode.Map[typeof(MMCScalableFloat)],
            //                 floatParams = new[] { 0.5f, 0 },
            //             }
            //         }
            //     }
            // }
        };
        
        public static string[] FixedStringToStringArray(NativeArray<FixedString32Bytes> array)
        {
            var strings = new string[array.Length];
            for (var i = 0; i < array.Length; ++i)
                strings[i] = array[i].ToString();
            return strings;
        }
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
            var has = GasEntityManager.HasComponent<ComBasicInfo>(geEntity);
            var basicData = has ? GasEntityManager.GetComponentData<ComBasicInfo>(geEntity) : new ComBasicInfo();
            name = !has ? "NONE" : geEntity.ToString();
        }

        public void SetDuration(Entity geEntity)
        {
            var has = GasEntityManager.HasComponent<ComDuration>(geEntity);
            var dur = has ? GasEntityManager.GetComponentData<ComDuration>(geEntity) : new ComDuration();
            duration = has ? dur.duration : 0;
            timeUnit = has ? dur.timeUnit : TimeUnit.Frame;
            active = has && dur.active;
        }

        public void SetPeriod(Entity geEntity)
        {
            var has = GasEntityManager.HasComponent<ComPeriod>(geEntity);
            var p = has ? GasEntityManager.GetComponentData<ComPeriod>(geEntity) : new ComPeriod();
            var periodGEs = has ? new string[p.GameplayEffects.Length] : null;
            if (has)
                for (var j = 0; j < p.GameplayEffects.Length; j++)
                    periodGEs[j] = p.GameplayEffects[j].ToString();

            period = has ? p.Period : 0;
            periodGameplayEffects = periodGEs;
        }

        public void SetModifier(Entity geEntity)
        {
            var has = GasEntityManager.HasComponent<BuffEleModifier>(geEntity);
            var mods = has
                ? GasEntityManager.GetBuffer<BuffEleModifier>(geEntity)
                : new DynamicBuffer<BuffEleModifier>();
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