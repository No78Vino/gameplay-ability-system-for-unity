using UnityEngine;

namespace DemoForESC._Script
{
    public class LoadMainSceneTrigger : MonoBehaviour
    {
        public void TriggerLoadMainScene()
        {
            EventCenter.Trigger("LoadMainScene");
        }
    }
}