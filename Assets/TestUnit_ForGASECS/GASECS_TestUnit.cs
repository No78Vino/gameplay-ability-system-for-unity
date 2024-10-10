using GAS.RuntimeWithECS.Core;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace TestUnit_ForGASECS
{
    public class GASECS_TestUnit : MonoBehaviour
    {
        public Entity asc;

        [DisplayAsString] 
        public string _ascName = "NULL";
            
        [Button(ButtonSizes.Medium)]
        void CreateASC()
        {
            asc = GASManager.EntityManager.CreateEntity();

            GASManager.EntityManager.SetName(asc, "TestUnit_ASCBaseCell");
            _ascName = asc.ToString();
        }

        [Button(ButtonSizes.Medium)]
        void ApplyGEToASC()
        {
            
        }

        [Button(ButtonSizes.Medium)]
        void RemoveGEFromASC()
        {
            
        }
    }
}