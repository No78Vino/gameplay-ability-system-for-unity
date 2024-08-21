///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class GTagLib
    {
        /// <summary>Ability</summary>
        public static GameplayTag Ability { get; } = new("Ability");

        /// <summary>Ability.Attack</summary>
        public static GameplayTag Ability_Attack { get; } = new("Ability.Attack");

        /// <summary>Ability.Defend</summary>
        public static GameplayTag Ability_Defend { get; } = new("Ability.Defend");

        /// <summary>Ability.Die</summary>
        public static GameplayTag Ability_Die { get; } = new("Ability.Die");

        /// <summary>Ability.Dodge</summary>
        public static GameplayTag Ability_Dodge { get; } = new("Ability.Dodge");

        /// <summary>Ability.Jump</summary>
        public static GameplayTag Ability_Jump { get; } = new("Ability.Jump");

        /// <summary>Ability.Move</summary>
        public static GameplayTag Ability_Move { get; } = new("Ability.Move");

        /// <summary>Ban</summary>
        public static GameplayTag Ban { get; } = new("Ban");

        /// <summary>Ban.Motion</summary>
        public static GameplayTag Ban_Motion { get; } = new("Ban.Motion");

        /// <summary>CD</summary>
        public static GameplayTag CD { get; } = new("CD");

        /// <summary>CD.Dodge</summary>
        public static GameplayTag CD_Dodge { get; } = new("CD.Dodge");

        /// <summary>CD.FireBullet</summary>
        public static GameplayTag CD_FireBullet { get; } = new("CD.FireBullet");

        /// <summary>CD.Skill0</summary>
        public static GameplayTag CD_Skill0 { get; } = new("CD.Skill0");

        /// <summary>CD.Skill1</summary>
        public static GameplayTag CD_Skill1 { get; } = new("CD.Skill1");

        /// <summary>Event</summary>
        public static GameplayTag Event { get; } = new("Event");

        /// <summary>Event.Attacking</summary>
        public static GameplayTag Event_Attacking { get; } = new("Event.Attacking");

        /// <summary>Event.Defending</summary>
        public static GameplayTag Event_Defending { get; } = new("Event.Defending");

        /// <summary>Event.Dodging</summary>
        public static GameplayTag Event_Dodging { get; } = new("Event.Dodging");

        /// <summary>Event.Dying</summary>
        public static GameplayTag Event_Dying { get; } = new("Event.Dying");

        /// <summary>Event.InAir</summary>
        public static GameplayTag Event_InAir { get; } = new("Event.InAir");

        /// <summary>Event.Moving</summary>
        public static GameplayTag Event_Moving { get; } = new("Event.Moving");

        /// <summary>Event.PerfectDefending</summary>
        public static GameplayTag Event_PerfectDefending { get; } = new("Event.PerfectDefending");

        /// <summary>Faction</summary>
        public static GameplayTag Faction { get; } = new("Faction");

        /// <summary>Faction.Enemy</summary>
        public static GameplayTag Faction_Enemy { get; } = new("Faction.Enemy");

        /// <summary>Faction.Player</summary>
        public static GameplayTag Faction_Player { get; } = new("Faction.Player");

        /// <summary>State</summary>
        public static GameplayTag State { get; } = new("State");

        /// <summary>State.Buff</summary>
        public static GameplayTag State_Buff { get; } = new("State.Buff");

        /// <summary>State.Buff.BulkUp</summary>
        public static GameplayTag State_Buff_BulkUp { get; } = new("State.Buff.BulkUp");

        /// <summary>State.Buff.DefendBuff</summary>
        public static GameplayTag State_Buff_DefendBuff { get; } = new("State.Buff.DefendBuff");

        /// <summary>State.Debuff</summary>
        public static GameplayTag State_Debuff { get; } = new("State.Debuff");

        /// <summary>State.Debuff.Death</summary>
        public static GameplayTag State_Debuff_Death { get; } = new("State.Debuff.Death");

        /// <summary>State.Debuff.LoseBalance</summary>
        public static GameplayTag State_Debuff_LoseBalance { get; } = new("State.Debuff.LoseBalance");

        /// <summary>State.Debuff.Stun</summary>
        public static GameplayTag State_Debuff_Stun { get; } = new("State.Debuff.Stun");

        public static readonly IReadOnlyDictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
        {
            ["Ability"] = Ability,
            ["Ability.Attack"] = Ability_Attack,
            ["Ability.Defend"] = Ability_Defend,
            ["Ability.Die"] = Ability_Die,
            ["Ability.Dodge"] = Ability_Dodge,
            ["Ability.Jump"] = Ability_Jump,
            ["Ability.Move"] = Ability_Move,
            ["Ban"] = Ban,
            ["Ban.Motion"] = Ban_Motion,
            ["CD"] = CD,
            ["CD.Dodge"] = CD_Dodge,
            ["CD.FireBullet"] = CD_FireBullet,
            ["CD.Skill0"] = CD_Skill0,
            ["CD.Skill1"] = CD_Skill1,
            ["Event"] = Event,
            ["Event.Attacking"] = Event_Attacking,
            ["Event.Defending"] = Event_Defending,
            ["Event.Dodging"] = Event_Dodging,
            ["Event.Dying"] = Event_Dying,
            ["Event.InAir"] = Event_InAir,
            ["Event.Moving"] = Event_Moving,
            ["Event.PerfectDefending"] = Event_PerfectDefending,
            ["Faction"] = Faction,
            ["Faction.Enemy"] = Faction_Enemy,
            ["Faction.Player"] = Faction_Player,
            ["State"] = State,
            ["State.Buff"] = State_Buff,
            ["State.Buff.BulkUp"] = State_Buff_BulkUp,
            ["State.Buff.DefendBuff"] = State_Buff_DefendBuff,
            ["State.Debuff"] = State_Debuff,
            ["State.Debuff.Death"] = State_Debuff_Death,
            ["State.Debuff.LoseBalance"] = State_Debuff_LoseBalance,
            ["State.Debuff.Stun"] = State_Debuff_Stun,
        };
    }
}