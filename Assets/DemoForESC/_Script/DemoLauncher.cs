using System;
using DemoForESC._Script.Gen;
using GAS.RuntimeWithECS.Core;
using UnityEngine;

namespace DemoForESC._Script
{
    public class DemoLauncher : MonoBehaviour
    {
        private void Awake()
        {
            GEN_GASLauncher.Launch();
            GASManager.Run();
        }
    }
}