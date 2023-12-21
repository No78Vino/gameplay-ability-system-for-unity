///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using GAS.Runtime.Attribute;
namespace GAS.Runtime.AttributeSet
{
public class AttrSet_Player:AttributeSet
{
    private AttributeBase HP = new AttributeBase("HP");
    public AttributeBase GetHP => HP;
    public void InitHP(float value)
    {
        HP.SetBaseValue(value);
        HP.SetCurrentValue(value);
    }
      public void SetCurrentHP(float value)
    {
        HP.SetCurrentValue(value);
    }
      public void SetBaseHP(float value)
    {
        HP.SetBaseValue(value);
    }
    private AttributeBase MP = new AttributeBase("MP");
    public AttributeBase GetMP => MP;
    public void InitMP(float value)
    {
        MP.SetBaseValue(value);
        MP.SetCurrentValue(value);
    }
      public void SetCurrentMP(float value)
    {
        MP.SetCurrentValue(value);
    }
      public void SetBaseMP(float value)
    {
        MP.SetBaseValue(value);
    }
    private AttributeBase SPEED = new AttributeBase("SPEED");
    public AttributeBase GetSPEED => SPEED;
    public void InitSPEED(float value)
    {
        SPEED.SetBaseValue(value);
        SPEED.SetCurrentValue(value);
    }
      public void SetCurrentSPEED(float value)
    {
        SPEED.SetCurrentValue(value);
    }
      public void SetBaseSPEED(float value)
    {
        SPEED.SetBaseValue(value);
    }
    private AttributeBase ATK = new AttributeBase("ATK");
    public AttributeBase GetATK => ATK;
    public void InitATK(float value)
    {
        ATK.SetBaseValue(value);
        ATK.SetCurrentValue(value);
    }
      public void SetCurrentATK(float value)
    {
        ATK.SetCurrentValue(value);
    }
      public void SetBaseATK(float value)
    {
        ATK.SetBaseValue(value);
    }

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
                 case "SPEED":
                    return SPEED;
                 case "ATK":
                    return ATK;
              }
              return null;
          }
      }
}
public class AttrSet_Enemy:AttributeSet
{
    private AttributeBase MP = new AttributeBase("MP");
    public AttributeBase GetMP => MP;
    public void InitMP(float value)
    {
        MP.SetBaseValue(value);
        MP.SetCurrentValue(value);
    }
      public void SetCurrentMP(float value)
    {
        MP.SetCurrentValue(value);
    }
      public void SetBaseMP(float value)
    {
        MP.SetBaseValue(value);
    }

      public override AttributeBase this[string key]
      {
          get
          {
              switch (key)
              {
                 case "MP":
                    return MP;
              }
              return null;
          }
      }
}
}
