using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Runtime
{
    public class CinemachineFixtureBase
    {
        protected readonly List<GameObject> m_GameObjectsToDestroy = new List<GameObject>();
        
        internal GameObject CreateGameObject(string name, params System.Type[] components)
        {
            var go = new GameObject();
            m_GameObjectsToDestroy.Add(go);
            go.name = name;
        
            foreach(var c in components)
                if (c.IsSubclassOf(typeof(Component)))
                    go.AddComponent(c);
        
            return go;
        }
        
        internal GameObject CreatePrimitive(PrimitiveType type)
        {
            var go = GameObject.CreatePrimitive(type);
            m_GameObjectsToDestroy.Add(go);

            return go;
        }

        [SetUp]
        public virtual void SetUp()
        {
            // force a uniform deltaTime, otherwise tests will be unstable
            CinemachineCore.UniformDeltaTimeOverride = 0.1f;
        }
        
        [TearDown]
        public virtual void TearDown()
        {
            foreach (var go in m_GameObjectsToDestroy)
                Object.Destroy(go);

            m_GameObjectsToDestroy.Clear();
            
            CinemachineCore.UniformDeltaTimeOverride = -1f;
        }
    }
}
