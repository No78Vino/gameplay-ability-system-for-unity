using System;
using DemoForESC._Script.Gen;
using GAS.Runtime;
using UnityEngine;

namespace DemoForESC._Script
{
    public class DemoLauncher : MonoBehaviour
    {
        private void Awake()
        {
            GEN_GASLauncher.Launch();
            GASManager.Run();

            var testE = GASManager.EntityManager.CreateEntity();
            GASManager.EntityManager.SetName(testE,"TestEntity");
            GASManager.EntityManager.AddComponent<CDuration>(testE);
        }
    }
}