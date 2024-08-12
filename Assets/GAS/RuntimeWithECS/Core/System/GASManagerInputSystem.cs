using Unity.Entities;

namespace GAS.RuntimeWithECS.Core
{
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
                if (!EntityManager.HasComponent<GASRunningTag>(_managerEntity))
                    EntityManager.AddComponent<GASRunningTag>(_managerEntity);
            }
            else
            {
                if (EntityManager.HasComponent<GASRunningTag>(_managerEntity))
                    EntityManager.RemoveComponent<GASRunningTag>(_managerEntity);
            }
        }
    }
}