///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public class AS_Fight : AttributeSet
    {
        #region ATK

        /// <summary>攻击力</summary>
        public AttributeBase ATK { get; } = new("AS_Fight", "ATK", 0f, CalculateMode.Stacking, (SupportedOperation)31, float.MinValue, float.MaxValue);

        public void InitATK(float value) => ATK.Init(value);
        public void SetCurrentATK(float value) => ATK.SetCurrentValue(value);
        public void SetBaseATK(float value) => ATK.SetBaseValue(value);
        public void SetMinATK(float value) => ATK.SetMinValue(value);
        public void SetMaxATK(float value) => ATK.SetMaxValue(value);
        public void SetMinMaxATK(float min, float max) => ATK.SetMinMaxValue(min, max);

        #endregion ATK

        #region HP

        /// <summary></summary>
        public AttributeBase HP { get; } = new("AS_Fight", "HP", 0f, CalculateMode.Stacking, (SupportedOperation)31, float.MinValue, float.MaxValue);

        public void InitHP(float value) => HP.Init(value);
        public void SetCurrentHP(float value) => HP.SetCurrentValue(value);
        public void SetBaseHP(float value) => HP.SetBaseValue(value);
        public void SetMinHP(float value) => HP.SetMinValue(value);
        public void SetMaxHP(float value) => HP.SetMaxValue(value);
        public void SetMinMaxHP(float min, float max) => HP.SetMinMaxValue(min, max);

        #endregion HP

        #region MP

        /// <summary></summary>
        public AttributeBase MP { get; } = new("AS_Fight", "MP", 0f, CalculateMode.Stacking, (SupportedOperation)31, float.MinValue, float.MaxValue);

        public void InitMP(float value) => MP.Init(value);
        public void SetCurrentMP(float value) => MP.SetCurrentValue(value);
        public void SetBaseMP(float value) => MP.SetBaseValue(value);
        public void SetMinMP(float value) => MP.SetMinValue(value);
        public void SetMaxMP(float value) => MP.SetMaxValue(value);
        public void SetMinMaxMP(float min, float max) => MP.SetMinMaxValue(min, max);

        #endregion MP

        #region POSTURE

        /// <summary></summary>
        public AttributeBase POSTURE { get; } = new("AS_Fight", "POSTURE", 0f, CalculateMode.Stacking, (SupportedOperation)31, float.MinValue, float.MaxValue);

        public void InitPOSTURE(float value) => POSTURE.Init(value);
        public void SetCurrentPOSTURE(float value) => POSTURE.SetCurrentValue(value);
        public void SetBasePOSTURE(float value) => POSTURE.SetBaseValue(value);
        public void SetMinPOSTURE(float value) => POSTURE.SetMinValue(value);
        public void SetMaxPOSTURE(float value) => POSTURE.SetMaxValue(value);
        public void SetMinMaxPOSTURE(float min, float max) => POSTURE.SetMinMaxValue(min, max);

        #endregion POSTURE

        #region SPEED

        /// <summary></summary>
        public AttributeBase SPEED { get; } = new("AS_Fight", "SPEED", 0f, CalculateMode.Stacking, (SupportedOperation)31, float.MinValue, float.MaxValue);

        public void InitSPEED(float value) => SPEED.Init(value);
        public void SetCurrentSPEED(float value) => SPEED.SetCurrentValue(value);
        public void SetBaseSPEED(float value) => SPEED.SetBaseValue(value);
        public void SetMinSPEED(float value) => SPEED.SetMinValue(value);
        public void SetMaxSPEED(float value) => SPEED.SetMaxValue(value);
        public void SetMinMaxSPEED(float min, float max) => SPEED.SetMinMaxValue(min, max);

        #endregion SPEED

        #region STAMINA

        /// <summary></summary>
        public AttributeBase STAMINA { get; } = new("AS_Fight", "STAMINA", 0f, CalculateMode.Stacking, (SupportedOperation)31, float.MinValue, float.MaxValue);

        public void InitSTAMINA(float value) => STAMINA.Init(value);
        public void SetCurrentSTAMINA(float value) => STAMINA.SetCurrentValue(value);
        public void SetBaseSTAMINA(float value) => STAMINA.SetBaseValue(value);
        public void SetMinSTAMINA(float value) => STAMINA.SetMinValue(value);
        public void SetMaxSTAMINA(float value) => STAMINA.SetMaxValue(value);
        public void SetMinMaxSTAMINA(float min, float max) => STAMINA.SetMinMaxValue(min, max);

        #endregion STAMINA

        public override AttributeBase this[string key]
        {
            get
            {
                switch (key)
                {
                    case "HP":
                        return HP;
                    case "MP":
                        return MP;
                    case "STAMINA":
                        return STAMINA;
                    case "POSTURE":
                        return POSTURE;
                    case "ATK":
                        return ATK;
                    case "SPEED":
                        return SPEED;
                }

                return null;
            }
        }

        public override string[] AttributeNames { get; } =
        {
            "HP",
            "MP",
            "STAMINA",
            "POSTURE",
            "ATK",
            "SPEED",
        };

        public override void SetOwner(AbilitySystemComponent owner)
        {
            _owner = owner;
            HP.SetOwner(owner);
            MP.SetOwner(owner);
            STAMINA.SetOwner(owner);
            POSTURE.SetOwner(owner);
            ATK.SetOwner(owner);
            SPEED.SetOwner(owner);
        }

        public static class Lookup
        {
            public const string HP = "AS_Fight.HP";
            public const string MP = "AS_Fight.MP";
            public const string STAMINA = "AS_Fight.STAMINA";
            public const string POSTURE = "AS_Fight.POSTURE";
            public const string ATK = "AS_Fight.ATK";
            public const string SPEED = "AS_Fight.SPEED";
        }
    }

    public static class GAttrSetLib
    {
        public static readonly IReadOnlyDictionary<string, Type> AttrSetTypeDict = new Dictionary<string, Type>
        {
            { "Fight", typeof(AS_Fight) },
        };

        public static readonly IReadOnlyDictionary<Type, string> TypeToName = new Dictionary<Type, string>
        {
            { typeof(AS_Fight), nameof(AS_Fight) },
        };

        public static readonly IReadOnlyList<string> AttributeFullNames = new List<string>
        {
            "AS_Fight.HP",
            "AS_Fight.MP",
            "AS_Fight.STAMINA",
            "AS_Fight.POSTURE",
            "AS_Fight.ATK",
            "AS_Fight.SPEED",
        };
    }
}