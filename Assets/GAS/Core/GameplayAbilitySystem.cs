using UnityEngine;

namespace GAS.Core
{
    public interface IGameplayAbilitySystem
    {
    }

    public class GameplayAbilitySystem : IGameplayAbilitySystem
    {
        private GasHost _gasHost;

        public GasHost GasHost
        {
            get
            {
                if (_gasHost == null) _gasHost = new GameObject("GasHost").AddComponent<GasHost>();

                return _gasHost;
            }
        }

        public static GameplayAbilitySystem Instance { get; private set; }

        public static void Create()
        {
            Instance = new GameplayAbilitySystem();
            Instance.Init();
        }

        public void Init()
        {
            GasHost.gameObject.SetActive(true);
        }
    }
}