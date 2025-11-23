using System;
using System.Collections.Generic;
using DemoForESC._Script.UI.View;

namespace DemoForESC._Script.UI
{
    public class UIConfig
    {
        public readonly static Dictionary<Type,string> WindowPathMap = new Dictionary<Type, string>()
        {
            [typeof(MaskWindow)] = "Assets/DemoForESC/Resources/Prefabs/UI/MaskWindow",
            [typeof(MenuWindow)] = "Assets/DemoForESC/Resources/Prefabs/UI/MenuWindow",
        };
    }
}