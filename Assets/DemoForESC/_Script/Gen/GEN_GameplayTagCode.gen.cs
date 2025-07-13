///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;
using GAS.RuntimeWithECS.Tag;

namespace GAS.Runtime
{
    public static class GEN_GameplayTagCode
    {
        public const int Ability = -523153736;
        public const int Ability_Attack = 115429334;
        public const int Ability_Defend = 465592122;
        public const int Ability_Die = 265453460;
        public const int Ability_Dodge = 1796722389;
        public const int Ability_Jump = -183664866;
        public const int Ability_Move = -1903949459;
        public const int Ban = 696030461;
        public const int Ban_Motion = 1305984585;
        public const int CD = -1137859925;
        public const int CD_Dodge = 2094525428;
        public const int CD_FireBullet = 868182143;
        public const int CD_Skill0 = -4094734;
        public const int CD_Skill1 = -4094735;
        public const int Event = 667645442;
        public const int Event_Attacking = 1440924632;
        public const int Event_Defending = -2001503536;
        public const int Event_Dodging = 1531892232;
        public const int Event_Dying = 1569540943;
        public const int Event_InAir = -487023025;
        public const int Event_Moving = 798883614;
        public const int Event_PerfectDefending = 1419440733;
        public const int Faction = -168968830;
        public const int Faction_Enemy = -1091112840;
        public const int Faction_Player = 1030057057;
        public const int State = 1695958603;
        public const int State_Buff = 743086286;
        public const int State_Buff_BulkUp = 8988933;
        public const int State_Buff_DefendBuff = 1648387519;
        public const int State_Debuff = -1227811357;
        public const int State_Debuff_Death = -1760211843;
        public const int State_Debuff_LoseBalance = -639020104;
        public const int State_Debuff_Stun = -1559194643;

        public static void InitTagList()
        {
            GTagUtil.InitTagMap(new Dictionary<int, GameplayTag>()
            {
                { Ability, new GameplayTag(Ability, new int[] {  }, new int[] {  }) },
                { Ability_Attack, new GameplayTag(Ability_Attack, new int[] { Ability }, new int[] { Ability }) },
                { Ability_Defend, new GameplayTag(Ability_Defend, new int[] { Ability }, new int[] { Ability }) },
                { Ability_Die, new GameplayTag(Ability_Die, new int[] { Ability }, new int[] { Ability }) },
                { Ability_Dodge, new GameplayTag(Ability_Dodge, new int[] { Ability }, new int[] { Ability }) },
                { Ability_Jump, new GameplayTag(Ability_Jump, new int[] { Ability }, new int[] { Ability }) },
                { Ability_Move, new GameplayTag(Ability_Move, new int[] { Ability }, new int[] { Ability }) },
                { Ban, new GameplayTag(Ban, new int[] {  }, new int[] {  }) },
                { Ban_Motion, new GameplayTag(Ban_Motion, new int[] { Ban }, new int[] { Ban }) },
                { CD, new GameplayTag(CD, new int[] {  }, new int[] {  }) },
                { CD_Dodge, new GameplayTag(CD_Dodge, new int[] { CD }, new int[] { CD }) },
                { CD_FireBullet, new GameplayTag(CD_FireBullet, new int[] { CD }, new int[] { CD }) },
                { CD_Skill0, new GameplayTag(CD_Skill0, new int[] { CD }, new int[] { CD }) },
                { CD_Skill1, new GameplayTag(CD_Skill1, new int[] { CD }, new int[] { CD }) },
                { Event, new GameplayTag(Event, new int[] {  }, new int[] {  }) },
                { Event_Attacking, new GameplayTag(Event_Attacking, new int[] { Event }, new int[] { Event }) },
                { Event_Defending, new GameplayTag(Event_Defending, new int[] { Event }, new int[] { Event }) },
                { Event_Dodging, new GameplayTag(Event_Dodging, new int[] { Event }, new int[] { Event }) },
                { Event_Dying, new GameplayTag(Event_Dying, new int[] { Event }, new int[] { Event }) },
                { Event_InAir, new GameplayTag(Event_InAir, new int[] { Event }, new int[] { Event }) },
                { Event_Moving, new GameplayTag(Event_Moving, new int[] { Event }, new int[] { Event }) },
                { Event_PerfectDefending, new GameplayTag(Event_PerfectDefending, new int[] { Event }, new int[] { Event }) },
                { Faction, new GameplayTag(Faction, new int[] {  }, new int[] {  }) },
                { Faction_Enemy, new GameplayTag(Faction_Enemy, new int[] { Faction }, new int[] { Faction }) },
                { Faction_Player, new GameplayTag(Faction_Player, new int[] { Faction }, new int[] { Faction }) },
                { State, new GameplayTag(State, new int[] {  }, new int[] {  }) },
                { State_Buff, new GameplayTag(State_Buff, new int[] { State }, new int[] { State }) },
                { State_Buff_BulkUp, new GameplayTag(State_Buff_BulkUp, new int[] { State, State_Buff }, new int[] { State, State_Buff }) },
                { State_Buff_DefendBuff, new GameplayTag(State_Buff_DefendBuff, new int[] { State, State_Buff }, new int[] { State, State_Buff }) },
                { State_Debuff, new GameplayTag(State_Debuff, new int[] { State }, new int[] { State }) },
                { State_Debuff_Death, new GameplayTag(State_Debuff_Death, new int[] { State, State_Debuff }, new int[] { State, State_Debuff }) },
                { State_Debuff_LoseBalance, new GameplayTag(State_Debuff_LoseBalance, new int[] { State, State_Debuff }, new int[] { State, State_Debuff }) },
                { State_Debuff_Stun, new GameplayTag(State_Debuff_Stun, new int[] { State, State_Debuff }, new int[] { State, State_Debuff }) },
            },
            new Dictionary<int, string>()
            {
                { Ability, "Ability" },
                { Ability_Attack, "Ability.Attack" },
                { Ability_Defend, "Ability.Defend" },
                { Ability_Die, "Ability.Die" },
                { Ability_Dodge, "Ability.Dodge" },
                { Ability_Jump, "Ability.Jump" },
                { Ability_Move, "Ability.Move" },
                { Ban, "Ban" },
                { Ban_Motion, "Ban.Motion" },
                { CD, "CD" },
                { CD_Dodge, "CD.Dodge" },
                { CD_FireBullet, "CD.FireBullet" },
                { CD_Skill0, "CD.Skill0" },
                { CD_Skill1, "CD.Skill1" },
                { Event, "Event" },
                { Event_Attacking, "Event.Attacking" },
                { Event_Defending, "Event.Defending" },
                { Event_Dodging, "Event.Dodging" },
                { Event_Dying, "Event.Dying" },
                { Event_InAir, "Event.InAir" },
                { Event_Moving, "Event.Moving" },
                { Event_PerfectDefending, "Event.PerfectDefending" },
                { Faction, "Faction" },
                { Faction_Enemy, "Faction.Enemy" },
                { Faction_Player, "Faction.Player" },
                { State, "State" },
                { State_Buff, "State.Buff" },
                { State_Buff_BulkUp, "State.Buff.BulkUp" },
                { State_Buff_DefendBuff, "State.Buff.DefendBuff" },
                { State_Debuff, "State.Debuff" },
                { State_Debuff_Death, "State.Debuff.Death" },
                { State_Debuff_LoseBalance, "State.Debuff.LoseBalance" },
                { State_Debuff_Stun, "State.Debuff.Stun" },
            }
            );
        }
    }
}
