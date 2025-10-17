using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class GASManagerInputSystem : SystemBase
    {
        private Entity _managerEntity;

        protected override void OnCreate()
        {
            _managerEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
        }

        protected override void OnUpdate()
        {
            if (GASManager.IsRunning)
            {
                if (!EntityManager.HasComponent<CGASRunningTag>(_managerEntity))
                    EntityManager.AddComponent<CGASRunningTag>(_managerEntity);
            }
            else
            {
                if (EntityManager.HasComponent<CGASRunningTag>(_managerEntity))
                    EntityManager.RemoveComponent<CGASRunningTag>(_managerEntity);
            }
        }
    }
}