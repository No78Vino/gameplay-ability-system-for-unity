///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class XTag
    {
        public const int Ability = 200;
        public const int Event = 300;
        public const int State = 400;
        public const int Guide = 500;
        public const int Cooldown = 600;
        public const int Faction_Player = 1001;
        public const int Faction_Enemy = 1002;
        public const int Ability_Die = 2001;
        public const int Ability_Move = 2002;
        public const int Ability_Attack = 2003;
        public const int Ability_Defend = 2004;
        public const int Ability_Jump = 2005;
        public const int Ability_Dodge = 2006;
        public const int Ability_Magic = 2008;
        public const int Ability_Magic_Boom = 2009;
        public const int Ability_Gun = 2010;
        public const int Event_Moving = 3001;
        public const int Event_Dodging = 3002;
        public const int Event_Attacking = 3003;
        public const int State_Buff = 4001;
        public const int State_Debuff = 4002;
        public const int Guide_Type1 = 5001;
        public const int Guide_Type2 = 5002;
        public const int Guide_Type3 = 5003;
        public const int Cooldown_Dodge = 6001;
        public const int Faction = 1000000;
        public const int Ability_Gun_Aim = 2010001;
        public const int Ability_Gun_Shoot = 2010002;
        public const int Ability_Gun_SuperShoot = 2010003;
        public const int State_Buff_BulkUp = 4001001;
        public const int State_Buff_SpeedUp = 4001002;
        public const int State_Buff_SpCosting = 4001003;
        public const int State_Buff_Invincible = 4001004;

        public static void InitTagList()
        {
            TagHelper.InitTagMap(new Dictionary<int, GameplayTag>()
            {
                { Ability, new GameplayTag(Ability, new int[] {  }, new int[] { Ability_Die, Ability_Move, Ability_Attack, Ability_Defend, Ability_Jump, Ability_Dodge, Ability_Magic, Ability_Magic_Boom, Ability_Gun, Ability_Gun_Aim, Ability_Gun_Shoot, Ability_Gun_SuperShoot }) },
                { Event, new GameplayTag(Event, new int[] {  }, new int[] { Event_Moving, Event_Dodging, Event_Attacking }) },
                { State, new GameplayTag(State, new int[] {  }, new int[] { State_Buff, State_Debuff, State_Buff_BulkUp, State_Buff_SpeedUp, State_Buff_SpCosting, State_Buff_Invincible }) },
                { Guide, new GameplayTag(Guide, new int[] {  }, new int[] { Guide_Type1, Guide_Type2, Guide_Type3 }) },
                { Cooldown, new GameplayTag(Cooldown, new int[] {  }, new int[] { Cooldown_Dodge }) },
                { Faction_Player, new GameplayTag(Faction_Player, new int[] { Faction }, new int[] {  }) },
                { Faction_Enemy, new GameplayTag(Faction_Enemy, new int[] { Faction }, new int[] {  }) },
                { Ability_Die, new GameplayTag(Ability_Die, new int[] { Ability }, new int[] {  }) },
                { Ability_Move, new GameplayTag(Ability_Move, new int[] { Ability }, new int[] {  }) },
                { Ability_Attack, new GameplayTag(Ability_Attack, new int[] { Ability }, new int[] {  }) },
                { Ability_Defend, new GameplayTag(Ability_Defend, new int[] { Ability }, new int[] {  }) },
                { Ability_Jump, new GameplayTag(Ability_Jump, new int[] { Ability }, new int[] {  }) },
                { Ability_Dodge, new GameplayTag(Ability_Dodge, new int[] { Ability }, new int[] {  }) },
                { Ability_Magic, new GameplayTag(Ability_Magic, new int[] { Ability }, new int[] { Ability_Magic_Boom }) },
                { Ability_Magic_Boom, new GameplayTag(Ability_Magic_Boom, new int[] { Ability, Ability_Magic }, new int[] {  }) },
                { Ability_Gun, new GameplayTag(Ability_Gun, new int[] { Ability }, new int[] { Ability_Gun_Aim, Ability_Gun_Shoot, Ability_Gun_SuperShoot }) },
                { Event_Moving, new GameplayTag(Event_Moving, new int[] { Event }, new int[] {  }) },
                { Event_Dodging, new GameplayTag(Event_Dodging, new int[] { Event }, new int[] {  }) },
                { Event_Attacking, new GameplayTag(Event_Attacking, new int[] { Event }, new int[] {  }) },
                { State_Buff, new GameplayTag(State_Buff, new int[] { State }, new int[] { State_Buff_BulkUp, State_Buff_SpeedUp, State_Buff_SpCosting, State_Buff_Invincible }) },
                { State_Debuff, new GameplayTag(State_Debuff, new int[] { State }, new int[] {  }) },
                { Guide_Type1, new GameplayTag(Guide_Type1, new int[] { Guide }, new int[] {  }) },
                { Guide_Type2, new GameplayTag(Guide_Type2, new int[] { Guide }, new int[] {  }) },
                { Guide_Type3, new GameplayTag(Guide_Type3, new int[] { Guide }, new int[] {  }) },
                { Cooldown_Dodge, new GameplayTag(Cooldown_Dodge, new int[] { Cooldown }, new int[] {  }) },
                { Faction, new GameplayTag(Faction, new int[] {  }, new int[] { Faction_Player, Faction_Enemy }) },
                { Ability_Gun_Aim, new GameplayTag(Ability_Gun_Aim, new int[] { Ability, Ability_Gun }, new int[] {  }) },
                { Ability_Gun_Shoot, new GameplayTag(Ability_Gun_Shoot, new int[] { Ability, Ability_Gun }, new int[] {  }) },
                { Ability_Gun_SuperShoot, new GameplayTag(Ability_Gun_SuperShoot, new int[] { Ability, Ability_Gun }, new int[] {  }) },
                { State_Buff_BulkUp, new GameplayTag(State_Buff_BulkUp, new int[] { State, State_Buff }, new int[] {  }) },
                { State_Buff_SpeedUp, new GameplayTag(State_Buff_SpeedUp, new int[] { State, State_Buff }, new int[] {  }) },
                { State_Buff_SpCosting, new GameplayTag(State_Buff_SpCosting, new int[] { State, State_Buff }, new int[] {  }) },
                { State_Buff_Invincible, new GameplayTag(State_Buff_Invincible, new int[] { State, State_Buff }, new int[] {  }) },
            },
            new Dictionary<int, string>()
            {
                { Ability, "Ability" },
                { Event, "Event" },
                { State, "State" },
                { Guide, "Guide" },
                { Cooldown, "Cooldown" },
                { Faction_Player, "Faction.Player" },
                { Faction_Enemy, "Faction.Enemy" },
                { Ability_Die, "Ability.Die" },
                { Ability_Move, "Ability.Move" },
                { Ability_Attack, "Ability.Attack" },
                { Ability_Defend, "Ability.Defend" },
                { Ability_Jump, "Ability.Jump" },
                { Ability_Dodge, "Ability.Dodge" },
                { Ability_Magic, "Ability.Magic" },
                { Ability_Magic_Boom, "Ability.Magic.Boom" },
                { Ability_Gun, "Ability.Gun" },
                { Event_Moving, "Event.Moving" },
                { Event_Dodging, "Event.Dodging" },
                { Event_Attacking, "Event.Attacking" },
                { State_Buff, "State.Buff" },
                { State_Debuff, "State.Debuff" },
                { Guide_Type1, "Guide.Type1" },
                { Guide_Type2, "Guide.Type2" },
                { Guide_Type3, "Guide.Type3" },
                { Cooldown_Dodge, "Cooldown.Dodge" },
                { Faction, "Faction" },
                { Ability_Gun_Aim, "Ability.Gun.Aim" },
                { Ability_Gun_Shoot, "Ability.Gun.Shoot" },
                { Ability_Gun_SuperShoot, "Ability.Gun.SuperShoot" },
                { State_Buff_BulkUp, "State.Buff.BulkUp" },
                { State_Buff_SpeedUp, "State.Buff.SpeedUp" },
                { State_Buff_SpCosting, "State.Buff.SpCosting" },
                { State_Buff_Invincible, "State.Buff.Invincible" },
            }
            );
        }
    }
}
