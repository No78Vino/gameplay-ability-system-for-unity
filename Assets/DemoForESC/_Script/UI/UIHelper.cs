using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DemoForESC._Script.UI
{
    public static class UIHelper
    {
        /// <summary>
        /// 为Button添加悬停进入事件监听
        /// </summary>
        /// <param name="button">目标按钮</param>
        /// <param name="onHoverAction">悬停回调方法（可选）</param>
        public static void AddHoverListener(this Button button, System.Action<PointerEventData> onHoverAction = null)
        {
            // 获取或添加EventTrigger组件
            EventTrigger trigger = button.GetComponent<EventTrigger>();
            if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

            // 创建悬停进入事件
            var entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };

            // 设置事件回调
            entry.callback.AddListener((data) =>
            {
                onHoverAction?.Invoke((PointerEventData)data);
            });

            // 添加事件到触发器
            trigger.triggers.Add(entry);
        }
        
        public static void AddHoverOutListener(this Button button, System.Action<PointerEventData> onHoverOutAction = null)
        {
            // 获取或添加EventTrigger组件
            EventTrigger trigger = button.GetComponent<EventTrigger>();
            if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

            // 创建悬停退出事件
            var entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };

            // 设置事件回调
            entry.callback.AddListener((data) =>
            {
                onHoverOutAction?.Invoke((PointerEventData)data);
            });

            // 添加事件到触发器
            trigger.triggers.Add(entry);
        }
    }
}