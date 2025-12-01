using UnityEngine;

namespace EXProceduralMachine
{
    public class EXProceduralMachineManager
    {
        public static EXProceduralMachineManager Instance => _instance ??= new EXProceduralMachineManager();
        private static EXProceduralMachineManager _instance;

        public Transform ManagerRoot { get; }

        public EXProceduralMachineManager()
        {
            ManagerRoot = new GameObject("EXProceduralMachineManager").transform;
        }

        public void Dispose()
        {
            Object.Destroy(ManagerRoot);
        }
    }
}