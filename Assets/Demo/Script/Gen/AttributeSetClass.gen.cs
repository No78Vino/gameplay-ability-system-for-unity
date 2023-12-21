///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using GAS.Runtime.Attribute;
namespace GAS.Runtime.AttributeSet
{
public class AttrSet_Fight:AttributeSet
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

      public override AttributeBase this[string key]
      {
          get
          {
              switch (key)
              {
                 case "HP":
                    return HP;
              }
              return null;
          }
      }
}
}
