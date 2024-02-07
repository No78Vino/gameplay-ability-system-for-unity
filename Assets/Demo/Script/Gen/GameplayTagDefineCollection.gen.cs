///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////
using System.Collections.Generic;
namespace GAS.Runtime.Tags
{
public static class GameplayTagSumCollection
{
    public static GameplayTag Faction { get;} = new GameplayTag("Faction");
    public static GameplayTag Faction_Enemy { get;} = new GameplayTag("Faction.Enemy");
    public static GameplayTag Faction_Player { get;} = new GameplayTag("Faction.Player");
    public static GameplayTag Ability { get;} = new GameplayTag("Ability");
    public static GameplayTag Ability_Jump { get;} = new GameplayTag("Ability.Jump");
    public static GameplayTag Ability_Defend { get;} = new GameplayTag("Ability.Defend");
    public static GameplayTag Ability_Dodge { get;} = new GameplayTag("Ability.Dodge");
    public static GameplayTag Ability_Attack { get;} = new GameplayTag("Ability.Attack");
    public static GameplayTag Ability_Move { get;} = new GameplayTag("Ability.Move");
    public static GameplayTag State { get;} = new GameplayTag("State");
    public static GameplayTag State_Buff { get;} = new GameplayTag("State.Buff");
    public static GameplayTag State_Buff_BulkUp { get;} = new GameplayTag("State.Buff.BulkUp");
    public static GameplayTag State_Debuff { get;} = new GameplayTag("State.Debuff");
    public static GameplayTag State_Debuff_Stun { get;} = new GameplayTag("State.Debuff.Stun");
    public static GameplayTag State_Debuff_LoseBalance { get;} = new GameplayTag("State.Debuff.LoseBalance");
    public static GameplayTag Event { get;} = new GameplayTag("Event");
    public static GameplayTag Event_Defending { get;} = new GameplayTag("Event.Defending");
    public static GameplayTag Event_Dodging { get;} = new GameplayTag("Event.Dodging");
    public static GameplayTag Event_InAir { get;} = new GameplayTag("Event.InAir");
    public static GameplayTag Event_Attacking { get;} = new GameplayTag("Event.Attacking");
    public static GameplayTag Event_Moving { get;} = new GameplayTag("Event.Moving");
    public static GameplayTag CD { get;} = new GameplayTag("CD");
    public static GameplayTag CD_Skill0 { get;} = new GameplayTag("CD.Skill0");
    public static GameplayTag CD_Skill1 { get;} = new GameplayTag("CD.Skill1");
    public static GameplayTag CD_Dodge { get;} = new GameplayTag("CD.Dodge");
    public static GameplayTag Ban { get;} = new GameplayTag("Ban");
    public static GameplayTag Ban_Motion { get;} = new GameplayTag("Ban.Motion");

      public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
      {
         ["Faction"] = Faction,
         ["Faction.Enemy"] = Faction_Enemy,
         ["Faction.Player"] = Faction_Player,
         ["Ability"] = Ability,
         ["Ability.Jump"] = Ability_Jump,
         ["Ability.Defend"] = Ability_Defend,
         ["Ability.Dodge"] = Ability_Dodge,
         ["Ability.Attack"] = Ability_Attack,
         ["Ability.Move"] = Ability_Move,
         ["State"] = State,
         ["State.Buff"] = State_Buff,
         ["State.Buff.BulkUp"] = State_Buff_BulkUp,
         ["State.Debuff"] = State_Debuff,
         ["State.Debuff.Stun"] = State_Debuff_Stun,
         ["State.Debuff.LoseBalance"] = State_Debuff_LoseBalance,
         ["Event"] = Event,
         ["Event.Defending"] = Event_Defending,
         ["Event.Dodging"] = Event_Dodging,
         ["Event.InAir"] = Event_InAir,
         ["Event.Attacking"] = Event_Attacking,
         ["Event.Moving"] = Event_Moving,
         ["CD"] = CD,
         ["CD.Skill0"] = CD_Skill0,
         ["CD.Skill1"] = CD_Skill1,
         ["CD.Dodge"] = CD_Dodge,
         ["Ban"] = Ban,
         ["Ban.Motion"] = Ban_Motion,
      };
}
}
