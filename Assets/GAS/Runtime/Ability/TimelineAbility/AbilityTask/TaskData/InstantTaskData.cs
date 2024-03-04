using System;
using System.Linq;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    [Serializable]
    public class InstantTaskData : AbilityTaskData
    {
        public InstantAbilityTask Task { get; private set; }

        public InstantTaskData()
        {
            TaskData = new JsonData()
            {
                Type = typeof(DefaultInstantAbilityTask).FullName,
            };    
        }
        
        public override void Cache(AbilitySpec abilitySpec)
        {
            base.Cache(abilitySpec);
            Task = TaskBase as InstantAbilityTask;
        }

        public override AbilityTaskBase Load()
        {
            InstantAbilityTask task = null;
            var jsonData = TaskData.Data;
            var dataType = string.IsNullOrEmpty(TaskData.Type) ? typeof(DefaultInstantAbilityTask).FullName : TaskData.Type;

            var type = InstantTaskSonTypes.FirstOrDefault(sonType => sonType.FullName == dataType);
            if (type == null)
            {
                Debug.LogError("[EX] InstantAbilityTask SonType not found: " + dataType);
            }
            else
            {
                if (string.IsNullOrEmpty(jsonData))
                    task = Activator.CreateInstance(type) as InstantAbilityTask;
                else
                    task = JsonUtility.FromJson(jsonData, type) as InstantAbilityTask;
            }

            return task;
        }

        #region SonTypes

        private static Type[] _instantTaskSonTypes;

        public static Type[] InstantTaskSonTypes =>
            _instantTaskSonTypes ??= TypeUtil.GetAllSonTypesOf(typeof(InstantAbilityTask));

        #endregion
    }
}