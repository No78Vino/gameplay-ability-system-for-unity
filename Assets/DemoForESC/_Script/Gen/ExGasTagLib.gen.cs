///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class XTag
    {
        public const int Faction = 100;
        public const int Faction_Player = 1001;
        public const int Faction_Enemy = 1002;
        public const int Ability = 200;
        public const int Ability_Die = 2001;
        public const int Ability_Move = 2002;
        public const int Ability_Attack = 2003;
        public const int Ability_Defend = 2004;
        public const int Ability_Jump = 2005;
        public const int Ability_Dodge = 2006;
        public const int Event = 300;
        public const int Event_Moving = 3001;
        public const int Event_Dodging = 3002;
        public const int Event_Attacking = 3003;
        public const int State = 400;
        public const int State_Buff = 4001;
        public const int State_Debuff = 4002;
        public const int State_Buff_BulkUp = 4001001;
        public const int State_Buff_SpeedUp = 4001002;

        public static void InitTagList()
        {
            GTagUtil.InitTagMap(new Dictionary<int, GameplayTag>()
            {
                { Faction, new GameplayTag(Faction, new int[] {  }, new int[] { Faction_Player, Faction_Enemy }) },
                { Faction_Player, new GameplayTag(Faction_Player, new int[] { Faction }, new int[] {  }) },
                { Faction_Enemy, new GameplayTag(Faction_Enemy, new int[] { Faction }, new int[] {  }) },
                { Ability, new GameplayTag(Ability, new int[] {  }, new int[] { Ability_Die, Ability_Move, Ability_Attack, Ability_Defend, Ability_Jump, Ability_Dodge }) },
                { Ability_Die, new GameplayTag(Ability_Die, new int[] { Ability }, new int[] {  }) },
                { Ability_Move, new GameplayTag(Ability_Move, new int[] { Ability }, new int[] {  }) },
                { Ability_Attack, new GameplayTag(Ability_Attack, new int[] { Ability }, new int[] {  }) },
                { Ability_Defend, new GameplayTag(Ability_Defend, new int[] { Ability }, new int[] {  }) },
                { Ability_Jump, new GameplayTag(Ability_Jump, new int[] { Ability }, new int[] {  }) },
                { Ability_Dodge, new GameplayTag(Ability_Dodge, new int[] { Ability }, new int[] {  }) },
                { Event, new GameplayTag(Event, new int[] {  }, new int[] { Event_Moving, Event_Dodging, Event_Attacking }) },
                { Event_Moving, new GameplayTag(Event_Moving, new int[] { Event }, new int[] {  }) },
                { Event_Dodging, new GameplayTag(Event_Dodging, new int[] { Event }, new int[] {  }) },
                { Event_Attacking, new GameplayTag(Event_Attacking, new int[] { Event }, new int[] {  }) },
                { State, new GameplayTag(State, new int[] {  }, new int[] { State_Buff, State_Debuff, State_Buff_BulkUp, State_Buff_SpeedUp }) },
                { State_Buff, new GameplayTag(State_Buff, new int[] { State }, new int[] { State_Buff_BulkUp, State_Buff_SpeedUp }) },
                { State_Debuff, new GameplayTag(State_Debuff, new int[] { State }, new int[] {  }) },
                { State_Buff_BulkUp, new GameplayTag(State_Buff_BulkUp, new int[] { State, State_Buff }, new int[] {  }) },
                { State_Buff_SpeedUp, new GameplayTag(State_Buff_SpeedUp, new int[] { State, State_Buff }, new int[] {  }) },
            },
            new Dictionary<int, string>()
            {
                { Faction, "Faction" },
                { Faction_Player, "Faction.Player" },
                { Faction_Enemy, "Faction.Enemy" },
                { Ability, "Ability" },
                { Ability_Die, "Ability.Die" },
                { Ability_Move, "Ability.Move" },
                { Ability_Attack, "Ability.Attack" },
                { Ability_Defend, "Ability.Defend" },
                { Ability_Jump, "Ability.Jump" },
                { Ability_Dodge, "Ability.Dodge" },
                { Event, "Event" },
                { Event_Moving, "Event.Moving" },
                { Event_Dodging, "Event.Dodging" },
                { Event_Attacking, "Event.Attacking" },
                { State, "State" },
                { State_Buff, "State.Buff" },
                { State_Debuff, "State.Debuff" },
                { State_Buff_BulkUp, "State.Buff.BulkUp" },
                { State_Buff_SpeedUp, "State.Buff.SpeedUp" },
            }
            );
        }
    }
}
