///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using GAS.Runtime.Attribute;
namespace GAS.Runtime.AttributeSet
{
public class AttrSet_PlayerA:AttributeSet
{
    private AttributeBase SKILL_CD = new AttributeBase("SKILL_CD");
    public AttributeBase GetSKILL_CD => SKILL_CD;
    public void InitSKILL_CD(float value)
    {
        SKILL_CD.SetBaseValue(value);
        SKILL_CD.SetCurrentValue(value);
    }
      public void SetCurrentSKILL_CD(float value)
    {
        SKILL_CD.SetCurrentValue(value);
    }
      public void SetBaseSKILL_CD(float value)
    {
        SKILL_CD.SetBaseValue(value);
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
    private AttributeBase BUFF_TIME = new AttributeBase("BUFF_TIME");
    public AttributeBase GetBUFF_TIME => BUFF_TIME;
    public void InitBUFF_TIME(float value)
    {
        BUFF_TIME.SetBaseValue(value);
        BUFF_TIME.SetCurrentValue(value);
    }
      public void SetCurrentBUFF_TIME(float value)
    {
        BUFF_TIME.SetCurrentValue(value);
    }
      public void SetBaseBUFF_TIME(float value)
    {
        BUFF_TIME.SetBaseValue(value);
    }

      public override AttributeBase this[string key]
      {
          get
          {
              switch (key)
              {
                 case "SKILL_CD":
                    return SKILL_CD;
                 case "MP":
                    return MP;
                 case "SPEED":
                    return SPEED;
                 case "BUFF_TIME":
                    return BUFF_TIME;
              }
              return null;
          }
      }
}
}
