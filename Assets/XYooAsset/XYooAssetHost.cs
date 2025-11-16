using System.Collections;
using UnityEngine;
using YooAsset;

namespace XYooAsset
{
    public class XYooAssetHost : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log($"资源系统运行模式：{ EPlayMode.OfflinePlayMode}");
            DontDestroyOnLoad(gameObject);
        }
    }
}