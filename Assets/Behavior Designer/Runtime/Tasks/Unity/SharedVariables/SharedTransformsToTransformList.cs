using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedTransformList values from the Transforms. Returns Success.")]
    public class SharedTransformsToTransformList : Action
    {
        [Tooltip("The Transforms value")]
        public SharedTransform[] transforms;
        [RequiredField]
        [Tooltip("The SharedTransformList to set")]
        public SharedTransformList storedTransformList;

        public override void OnAwake()
        {
            storedTransformList.Value = new List<Transform>();
        }

        public override TaskStatus OnUpdate()
        {
            if (transforms == null || transforms.Length == 0) {
                return TaskStatus.Failure;
            }

            storedTransformList.Value.Clear();
            for (int i = 0; i < transforms.Length; ++i) {
                storedTransformList.Value.Add(transforms[i].Value);
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            transforms = null;
            storedTransformList = null;
        }
    }
}