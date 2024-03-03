using System.Collections.Generic;
using System.Linq;
using GAS.General;
using GAS.Runtime.Ability;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class MyData
{
    public string name;
    public int age;
    // 其他成员...
}

public class MySonData : MyData
{
    public string sonName;

    public int sonAge;
    // 其他成员...
}

[System.Serializable]
public class JsonData
{
    public string Type;
    public string Data;
}

public class TestMyScriptableObject
{
    [MenuItem( "Tools/TestMyScriptableObject")]
    public static void Test()
    {
        // 创建一个 ScriptableObject 对象
        MyScriptableObject myScriptableObject = ScriptableObject.CreateInstance<MyScriptableObject>();

        // 创建一些数据
        List<TestClassForGAS> dataList = new List<TestClassForGAS>
        {
            new TestClassForGASSonB {x = 1, Y = "Hello Y", bSon = 2, bSonW = "World B"},
            new TestClassForGASSon {x = 4, Y = "Hello Y", z = 5, W = "World"}
        };

        // 保存数据
        myScriptableObject.SaveData(dataList);

        // 保存 ScriptableObject 对象
        AssetDatabase.CreateAsset(myScriptableObject, "Assets/_Test/MyCustomData.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 加载 ScriptableObject 对象
        MyScriptableObject loadedScriptableObject = AssetDatabase.LoadAssetAtPath<MyScriptableObject>("Assets/_Test/MyCustomData.asset");

        // 加载数据
        List<TestClassForGAS> loadedDataList = loadedScriptableObject.LoadData();

        // 打印数据
        foreach (var data in loadedDataList)
        {
            Debug.Log(data);
        }
    }
}

// 自定义的 ScriptableObject 类
[CreateAssetMenu(fileName = "MyCustomData", menuName = "Custom Data")]
public class MyScriptableObject : ScriptableObject
{
    public List<JsonData> jsonDataList;

    public void SaveData(List<TestClassForGAS> dataList)
    {
        jsonDataList = new List<JsonData>();
        foreach (var data in dataList)
        {
            string jsonData = JsonUtility.ToJson(data);
            string dataType = data.GetType().FullName;

            JsonData jsonDataObject = new JsonData
            {
                Type = dataType,
                Data = jsonData
            };

            jsonDataList.Add(jsonDataObject);
        }
    }

    public List<TestClassForGAS> LoadData()
    {
        List<TestClassForGAS> dataList = new List<TestClassForGAS>();
        foreach (var jsonDataObject in jsonDataList)
        {
            string jsonData = jsonDataObject.Data;
            string dataType = jsonDataObject.Type;
            
            var sonTypes = TypeUtil.GetAllSonTypesOf(typeof(TestClassForGAS));
            System.Type type = sonTypes.FirstOrDefault(sonType => sonType.FullName == dataType);

            if(type == null)
            {
               Debug.LogError( "Son Type not found: " + dataType);
            }
            else
            {
                object data = JsonUtility.FromJson(jsonData, type);
                dataList.Add(data as TestClassForGAS);
            }
        }

        return dataList;
    }
}