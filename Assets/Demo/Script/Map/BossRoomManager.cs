using Demo.Script.Element;
using UnityEngine;

namespace Demo.Script.Map
{
    public class BossRoomManager:MonoBehaviour
    {
        public GateTrigger GateTrigger;

        public void ResetRoom()
        {
            // 清除 Boss，Player
            var boss = FindObjectOfType<BossBladeFang>();
            if(boss) DestroyImmediate(boss.gameObject);
            
            var player = FindObjectOfType<Player>();
            if(player) DestroyImmediate(player.gameObject);
            
            // 重新打开GateTrigger
            GateTrigger.gameObject.SetActive(true);
            
            // 开门
            GateTrigger.gate.transform.position = GateTrigger.gateOpenPosition;
        }
    }
}