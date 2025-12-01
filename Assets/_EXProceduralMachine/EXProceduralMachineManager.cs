using UnityEngine;

namespace EXProceduralMachine
{
    public class EXProceduralMachineManager
    {
        public static EXProceduralMachineManager Instance => _instance ??= new EXProceduralMachineManager();
        private static EXProceduralMachineManager _instance;

        private Transform _managerRoot;
        
        public EXProceduralMachineManager()
        {
            _managerRoot = new GameObject("EXProceduralMachineManager").transform;
        }

        public void Dispose()
        {
            Object.Destroy(_managerRoot);
        }
    }
}