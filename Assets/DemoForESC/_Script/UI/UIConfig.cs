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
            [typeof(MainWindow)] = "Assets/DemoForESC/Resources/Prefabs/UI/MainWindow",
            [typeof(GuideWindow)] = "Assets/DemoForESC/Resources/Prefabs/UI/GuideWindow",
            [typeof(DeathWindow)] = "Assets/DemoForESC/Resources/Prefabs/UI/DeathWindow",
        };
    }
}