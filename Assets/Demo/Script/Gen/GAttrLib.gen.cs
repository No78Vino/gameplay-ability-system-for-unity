///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class GAttrLib
    {
        /// <summary>生命值</summary>
        public const string HP = "HP";

        /// <summary>法力值</summary>
        public const string MP = "MP";

        /// <summary>耐力值</summary>
        public const string STAMINA = "STAMINA";

        /// <summary>姿态</summary>
        public const string POSTURE = "POSTURE";

        /// <summary>攻击力</summary>
        public const string ATK = "ATK";

        /// <summary>移动速度</summary>
        public const string SPEED = "SPEED";

        // For facilitating the creation of a Value Dropdown in the editor.
        public static readonly IReadOnlyList<string> AttributeNames = new List<string>
        {
            HP,
            MP,
            STAMINA,
            POSTURE,
            ATK,
            SPEED,
        };
    }
}