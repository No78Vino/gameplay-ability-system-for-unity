using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using Cysharp.Threading.Tasks;
using Demo.Script.Element;
using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider2D))]
public class GateTrigger : MonoBehaviour
{
    public GameObject gate;
    public ProCamera2D proCamera2D;
    public Transform BossRoomFollowTarget;
    
    public Vector3 gateClosePosition;

    public GameObject bossPrefab;

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnEnterBossRoom().Forget();
        gameObject.SetActive(false);
    }

    async UniTask OnEnterBossRoom()
    {
        // 屏蔽玩家操作
        var player = FindObjectOfType<Player>();
        player.DeactivateMove();
        player.DisableInput();
        
        // 关门
        var startGatePosition = gate.transform.position;
        var closeTime = 0.5f;
        var timer = closeTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            gate.transform.position = Vector3.Lerp(startGatePosition, gateClosePosition, 1 - timer / closeTime);
            await UniTask.Yield();
        }
        gate.transform.position = gateClosePosition;
        
        // 摄像机移动
        proCamera2D.RemoveAllCameraTargets();
        proCamera2D.AddCameraTarget(BossRoomFollowTarget);
        
        // 召唤Boss
        var bossObject = Instantiate(bossPrefab, BossRoomFollowTarget.position, Quaternion.identity);
        var boss = bossObject.GetComponent<BossBladeFang>();
        boss.BT.enabled = false;
        boss.enabled = false;
        
        var bossShowTime = 3f;
        timer = bossShowTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            boss.transform.localScale = Vector3.one * Mathf.Lerp(0.05f, 1, 1 - timer / bossShowTime);
            await UniTask.Yield();
        }
        boss.transform.localScale = Vector3.one;
        
        // 显示Boss血条
        XUI.M.VM<MainUIVM>().ShowBossUI();
        await UniTask.Delay(500);
        
        // 激活Boss
        boss.enabled = true;
        boss.BT.enabled = true;
        player.EnableInput();
    }
}