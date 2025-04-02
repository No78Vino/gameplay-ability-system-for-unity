// ================== 核心数据结构 ==================

using System;
using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeDataHelper.Ability;
using GAS.RuntimeDataHelper.Ability.AbilityParam;
using GAS.RuntimeDataHelper.GameplayEffect;
using UnityEngine;

[Serializable]
public struct JsonConfigData
{
    public string TypeFullName;  // 使用完整类型名
    public string Data;
}

[Serializable]
public class JsonConfigWrapper
{
    public List<JsonConfigData> Items = new();
}

// ================== 序列化代理 ==================
public static class JsonProxyHelper
{
    private static readonly Dictionary<string, Type> _typeCache = new();

    static JsonProxyHelper()
    {
        var abstractAbilityComponentType = typeof(BaseGameplayAbilityComponentConfigAsset);
        var abstractEffectComponentType = typeof(BaseGameplayEffectComponentConfigAsset);
        var abstractAbilityParamConfigType = typeof(AbilityParamConfigBase);
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()))
        {
            if (type.IsAbstract) continue;
            if (!abstractAbilityComponentType.IsAssignableFrom(type) 
                && !abstractEffectComponentType.IsAssignableFrom(type)
                && !abstractAbilityParamConfigType.IsAssignableFrom(type) ) continue;
            if (type.FullName != null) _typeCache[type.FullName] = type;
        }

        bool i = true;
    }

    public static string SerializeList<T>(List<T> list)
    {
        var wrapper = new JsonConfigWrapper();
        foreach (var item in list)
        {
            if (item == null) continue;

            var type = item.GetType();
            wrapper.Items.Add(new JsonConfigData
            {
                TypeFullName = type.FullName,
                Data = JsonUtility.ToJson(item)
            });
        }
        return JsonUtility.ToJson(wrapper);
    }

    public static List<T> DeserializeList<T>(string json)
    {
        var result = new List<T>();
        
        if (string.IsNullOrEmpty(json)) 
            return result;

        try
        {
            var wrapper = JsonUtility.FromJson<JsonConfigWrapper>(json);
            foreach (var data in wrapper.Items)
            {
                if (_typeCache.TryGetValue(data.TypeFullName, out var type))
                {
                    var obj = JsonUtility.FromJson(data.Data, type);
                    result.Add((T)obj);
                }
                else
                {
                    Debug.LogError($"Unknown type: {data.TypeFullName}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Deserialize failed: {e.Message}");
        }

        return result;
    }
    
    public static string Serialize<T>(T data)
    {
        return JsonUtility.ToJson(data);
    }
    
    public static T Deserialize<T>(JsonConfigData data)
    {
        if (_typeCache.TryGetValue(data.TypeFullName, out var type))
        {
            var obj = JsonUtility.FromJson(data.Data, type);
            return (T)obj;
        }

        Debug.LogError($"Unknown type: {data.TypeFullName}");
        return default;
    }
}
