using Unity.Entities;
using Unity.Collections;
using System.Collections.Generic;
using GAS.Runtime;

namespace GAS.Editor
{
    public static class EntityQueryHelper
    {
        /// <summary>
        /// 获取所有包含指定组件的实体列表
        /// </summary>
        public static List<Entity> GetAllEntitiesWithComponent<T>(
            Allocator allocator = Allocator.TempJob
        ) where T : IComponentData
        {
            World world = GASManager.EntityManager.World;
            var query = new EntityQueryBuilder(allocator)
                .WithAll<T>()
                .Build(world.EntityManager);

            using (var entities = query.ToEntityArray(allocator))
            {
                return new List<Entity>(entities);
            }
        }

        /// <summary>
        /// 获取实体及其组件数据
        /// </summary>
        public static List<(Entity, T)> GetAllComponentData<T>() where T : unmanaged, IComponentData
        {
            World world = GASManager.EntityManager.World;
            var entityManager = world.EntityManager;
            var query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<T>()
                .Build(entityManager);

            var entities = query.ToEntityArray(Allocator.TempJob);
            var components = query.ToComponentDataArray<T>(Allocator.TempJob);

            var result = new List<(Entity, T)>();
            for (int i = 0; i < entities.Length; i++)
            {
                result.Add((entities[i], components[i]));
            }

            entities.Dispose();
            components.Dispose();
            query.Dispose();

            return result;
        }
    }
}