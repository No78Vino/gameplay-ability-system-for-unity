using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityLayerMask
{
    [TaskCategory("Unity/LayerMask")]
    [TaskDescription("Sets the layer of a GameObject.")]
    public class SetLayer : Action
    {
        [Tooltip("The GameObject to set the layer of")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the layer to set")]
        public SharedString layerName = "Default";

        public override TaskStatus OnUpdate()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            currentGameObject.layer = LayerMask.NameToLayer(layerName.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            layerName = "Default";
        }
    }
}