using System.Collections.Generic;  
using System.Linq;  
using Framework.Core;  
using UnityEngine;  
  
namespace Framework.Unit  
{  
    /// <summary>  
    /// 单位管理器（MonoBehaviour 单例）。  
    /// - 维护场景内所有 UnitBase 的注册表  
    /// - 提供按 GameplayTag / 类型 查询接口  
    /// - 提供 SpawnUnit 工厂方法  
    /// 生命周期：随 GameEntry 的 DontDestroyOnLoad GameObject 常驻。  
    /// </summary>  
    public class UnitManager : MonoBehaviour  
    {  
        // ── 单例 ──  
        private static UnitManager _instance;  
        public static UnitManager Instance  
        {  
            get  
            {  
                if (_instance == null)  
                    _instance = FindObjectOfType<UnitManager>();  
                return _instance;  
            }  
        }  
  
        private void Awake()  
        {  
            if (_instance != null && _instance != this)  
            {  
                Destroy(gameObject);  
                return;  
            }  
            _instance = this;  
        }  
  
        // ── 注册表 ──  
        private readonly List<UnitBase> _units = new();  
  
        /// <summary>注册单位（由 UnitBase.Awake 自动调用）</summary>  
        public void Register(UnitBase unit)  
        {  
            if (!_units.Contains(unit))  
                _units.Add(unit);  
        }  
  
        /// <summary>注销单位（由 UnitBase.OnDestroy 自动调用）</summary>  
        public void Unregister(UnitBase unit)  
        {  
            _units.Remove(unit);  
        }  
  
        // ── 查询接口 ──  
  
        /// <summary>  
        /// 返回持有指定 GameplayTag 的所有单位。  
        /// 内部调用 AbilitySystemComponent.HasTag(tagId)。  
        /// </summary>  
        public List<UnitBase> GetUnitsWithTag(int tagId)  
            => _units.Where(u => u != null && u.HasTag(tagId)).ToList();  
  
        /// <summary>  
        /// 返回场景中第一个指定类型的单位（如 PlayerUnit）。  
        /// </summary>  
        public T GetUnit<T>() where T : UnitBase  
            => _units.OfType<T>().FirstOrDefault();  
  
        /// <summary>  
        /// 返回场景中所有指定类型的单位（如所有 EnemyUnit）。  
        /// </summary>  
        public List<T> GetUnits<T>() where T : UnitBase  
            => _units.OfType<T>().ToList();  
  
        /// <summary>返回当前注册的全部单位（只读副本）</summary>  
        public IReadOnlyList<UnitBase> AllUnits => _units;  
  
        // ── Spawn 工厂 ──  
  
        /// <summary>  
        /// 实例化并返回单位。  
        /// prefab 上必须挂有继承自 UnitBase 的组件，且 _ascPresetId 已在 prefab 上配置。  
        /// UnitBase.Awake 会自动调用 Register，无需手动注册。  
        /// </summary>  
        public UnitBase SpawnUnit(GameObject prefab, Vector3 position, Quaternion rotation = default)  
        {  
            var go = Instantiate(prefab, position, rotation == default ? Quaternion.identity : rotation);  
            var unit = go.GetComponent<UnitBase>();  
            if (unit == null)  
                Debug.LogError($"[UnitManager] SpawnUnit: prefab '{prefab.name}' 上没有 UnitBase 组件！");  
            return unit;  
        }  
  
        /// <summary>  
        /// 清除所有已注册单位（关卡卸载时调用，不 Destroy GameObject）。  
        /// </summary>  
        public void Clear()  
        {  
            _units.Clear();  
        }  
    }  
}