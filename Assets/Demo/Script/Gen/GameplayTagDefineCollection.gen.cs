///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////
using System.Collections.Generic;
namespace GAS.Runtime.Tags
{
public static class GameplayTagSumCollection
{
    public static GameplayTag Ability { get;} = new GameplayTag("Ability");
    public static GameplayTag Ability_Dodge { get;} = new GameplayTag("Ability.Dodge");
    public static GameplayTag Ability_Attack { get;} = new GameplayTag("Ability.Attack");
    public static GameplayTag Ability_Move { get;} = new GameplayTag("Ability.Move");
    public static GameplayTag State { get;} = new GameplayTag("State");
    public static GameplayTag State_Buff { get;} = new GameplayTag("State.Buff");
    public static GameplayTag State_Buff_Healing { get;} = new GameplayTag("State.Buff.Healing");
    public static GameplayTag State_Debuff { get;} = new GameplayTag("State.Debuff");
    public static GameplayTag State_Debuff_Freeze { get;} = new GameplayTag("State.Debuff.Freeze");
    public static GameplayTag State_Debuff_Burning { get;} = new GameplayTag("State.Debuff.Burning");
    public static GameplayTag Event { get;} = new GameplayTag("Event");
    public static GameplayTag Event_Attacking { get;} = new GameplayTag("Event.Attacking");
    public static GameplayTag Event_Moving { get;} = new GameplayTag("Event.Moving");

      public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
      {
         ["Ability"] = Ability,
         ["Ability.Dodge"] = Ability_Dodge,
         ["Ability.Attack"] = Ability_Attack,
         ["Ability.Move"] = Ability_Move,
         ["State"] = State,
         ["State.Buff"] = State_Buff,
         ["State.Buff.Healing"] = State_Buff_Healing,
         ["State.Debuff"] = State_Debuff,
         ["State.Debuff.Freeze"] = State_Debuff_Freeze,
         ["State.Debuff.Burning"] = State_Debuff_Burning,
         ["Event"] = Event,
         ["Event.Attacking"] = Event_Attacking,
         ["Event.Moving"] = Event_Moving,
      };
}
}
