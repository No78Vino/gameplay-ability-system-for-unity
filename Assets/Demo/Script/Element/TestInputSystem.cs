using GAS.Runtime;
using UnityEngine;

namespace Demo.Script.Element
{
    public class TestInputSystem : MonoBehaviour
    {
        public GameplayEffectAsset testEffect;

        void Update()
        {
            // 当F1键弹起时，触发事件
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("F1键弹起");
                // 触发事件
                ApplyTestEffectToBoss();
            }
        }

        void ApplyTestEffectToBoss()
        {
            var boss = FindObjectOfType<BossBladeFang>();
            if (boss != null)
            {
                var effect = new GameplayEffect(testEffect);
                boss.ASC.ApplyGameplayEffectToSelf(effect);
            }
        }
    }
}