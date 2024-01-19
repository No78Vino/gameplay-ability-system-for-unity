///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using GAS.Runtime.Attribute;
using System;
using System.Collections.Generic;
namespace GAS.Runtime.AttributeSet
{
public class AS_Player:AttributeSet
{
    private AttributeBase _HP = new AttributeBase("AS_Player","HP");
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
    private AttributeBase _SKILL_CD = new AttributeBase("AS_Player","SKILL_CD");
    public AttributeBase SKILL_CD => _SKILL_CD;
    public void InitSKILL_CD(float value)
    {
        _SKILL_CD.SetBaseValue(value);
        _SKILL_CD.SetCurrentValue(value);
    }
      public void SetCurrentSKILL_CD(float value)
    {
        _SKILL_CD.SetCurrentValue(value);
    }
      public void SetBaseSKILL_CD(float value)
    {
        _SKILL_CD.SetBaseValue(value);
    }
    private AttributeBase _SPEED = new AttributeBase("AS_Player","SPEED");
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
    private AttributeBase _ATK = new AttributeBase("AS_Player","ATK");
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

      public override AttributeBase this[string key]
      {
          get
          {
              switch (key)
              {
                 case "HP":
                    return _HP;
                 case "SKILL_CD":
                    return _SKILL_CD;
                 case "SPEED":
                    return _SPEED;
                 case "ATK":
                    return _ATK;
              }
              return null;
          }
      }

      public override string[] AttributeNames { get; } =
      {
          "HP",
          "SKILL_CD",
          "SPEED",
          "ATK",
      };
}
public class AS_Enemy:AttributeSet
{
    private AttributeBase _HP = new AttributeBase("AS_Enemy","HP");
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
    private AttributeBase _ATK = new AttributeBase("AS_Enemy","ATK");
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

      public override AttributeBase this[string key]
      {
          get
          {
              switch (key)
              {
                 case "HP":
                    return _HP;
                 case "ATK":
                    return _ATK;
              }
              return null;
          }
      }

      public override string[] AttributeNames { get; } =
      {
          "HP",
          "ATK",
      };
}
public static class AttrSetUtil
{
    public static readonly Dictionary<string,Type> AttrSetTypeDict = new Dictionary<string, Type>()
    {
        {"Player",typeof(AS_Player)},
        {"Enemy",typeof(AS_Enemy)},
    };
    public static List<string> AttributeFullNames=new List<string>()
    {
        "AS_Player.HP",
        "AS_Player.SKILL_CD",
        "AS_Player.SPEED",
        "AS_Player.ATK",
        "AS_Enemy.HP",
        "AS_Enemy.ATK",
      };
}
}
