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
        #region HP

        private AttributeBase _HP = new AttributeBase("AS_Fight", "HP");

        public AttributeBase HP => _HP;

        public void InitHP(float value)
        {
            _HP.SetBaseValue(value);
            _HP.SetCurrentValue(value);
        }

        public void SetCurrentHP(float value)
        {
            _HP.SetCurrentValue(value);
        }

        public void SetBaseHP(float value)
        {
            _HP.SetBaseValue(value);
        }

        #endregion HP

        #region MP

        private AttributeBase _MP = new AttributeBase("AS_Fight", "MP");

        public AttributeBase MP => _MP;

        public void InitMP(float value)
        {
            _MP.SetBaseValue(value);
            _MP.SetCurrentValue(value);
        }

        public void SetCurrentMP(float value)
        {
            _MP.SetCurrentValue(value);
        }

        public void SetBaseMP(float value)
        {
            _MP.SetBaseValue(value);
        }

        #endregion MP

        #region STAMINA

        private AttributeBase _STAMINA = new AttributeBase("AS_Fight", "STAMINA");

        public AttributeBase STAMINA => _STAMINA;

        public void InitSTAMINA(float value)
        {
            _STAMINA.SetBaseValue(value);
            _STAMINA.SetCurrentValue(value);
        }

        public void SetCurrentSTAMINA(float value)
        {
            _STAMINA.SetCurrentValue(value);
        }

        public void SetBaseSTAMINA(float value)
        {
            _STAMINA.SetBaseValue(value);
        }

        #endregion STAMINA

        #region POSTURE

        private AttributeBase _POSTURE = new AttributeBase("AS_Fight", "POSTURE");

        public AttributeBase POSTURE => _POSTURE;

        public void InitPOSTURE(float value)
        {
            _POSTURE.SetBaseValue(value);
            _POSTURE.SetCurrentValue(value);
        }

        public void SetCurrentPOSTURE(float value)
        {
            _POSTURE.SetCurrentValue(value);
        }

        public void SetBasePOSTURE(float value)
        {
            _POSTURE.SetBaseValue(value);
        }

        #endregion POSTURE

        #region ATK

        private AttributeBase _ATK = new AttributeBase("AS_Fight", "ATK");

        public AttributeBase ATK => _ATK;

        public void InitATK(float value)
        {
            _ATK.SetBaseValue(value);
            _ATK.SetCurrentValue(value);
        }

        public void SetCurrentATK(float value)
        {
            _ATK.SetCurrentValue(value);
        }

        public void SetBaseATK(float value)
        {
            _ATK.SetBaseValue(value);
        }

        #endregion ATK

        #region SPEED

        private AttributeBase _SPEED = new AttributeBase("AS_Fight", "SPEED");

        public AttributeBase SPEED => _SPEED;

        public void InitSPEED(float value)
        {
            _SPEED.SetBaseValue(value);
            _SPEED.SetCurrentValue(value);
        }

        public void SetCurrentSPEED(float value)
        {
            _SPEED.SetCurrentValue(value);
        }

        public void SetBaseSPEED(float value)
        {
            _SPEED.SetBaseValue(value);
        }

        #endregion SPEED

        public override AttributeBase this[string key]
        {
            get
            {
                switch (key)
                {
                    case "HP":
                        return _HP;
                    case "MP":
                        return _MP;
                    case "STAMINA":
                        return _STAMINA;
                    case "POSTURE":
                        return _POSTURE;
                    case "ATK":
                        return _ATK;
                    case "SPEED":
                        return _SPEED;
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
            _HP.SetOwner(owner);
            _MP.SetOwner(owner);
            _STAMINA.SetOwner(owner);
            _POSTURE.SetOwner(owner);
            _ATK.SetOwner(owner);
            _SPEED.SetOwner(owner);
        }
    }

    public static class GAttrSetLib
    {
        public static readonly Dictionary<string, Type> AttrSetTypeDict = new Dictionary<string, Type>()
        {
            { "Fight", typeof(AS_Fight) },
        };

        public static List<string> AttributeFullNames = new List<string>()
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