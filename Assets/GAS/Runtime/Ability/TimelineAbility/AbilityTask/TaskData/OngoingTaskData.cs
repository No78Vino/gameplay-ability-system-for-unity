using System;
using System.Collections.Generic;
using System.Linq;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public class OngoingTaskData : AbilityTaskData
    {
        public OngoingTaskData()
        {
            TaskData = new JsonData()
            {
                Type = typeof(DefaultOngoingAbilityTask).FullName,
            };
        }
        
        public OngoingAbilityTask CreateTask(AbilitySpec abilitySpec)
        {
            var task = base.Create(abilitySpec);
            var ongoingAbilityTask = task as OngoingAbilityTask;
            return ongoingAbilityTask;
        }

        public override AbilityTaskBase Load()
        {
            OngoingAbilityTask task = null;
            var jsonData = TaskData.Data;
            var dataType = string.IsNullOrEmpty(TaskData.Type) ? typeof(DefaultOngoingAbilityTask).FullName : TaskData.Type;

            var type = OngoingTaskSonTypes.FirstOrDefault(sonType => sonType.FullName == dataType);
            if (type == null)
            {
                Debug.LogError("[EX] OngoingAbilityTask SonType not found: " + dataType);
            }
            else
            {
                if (string.IsNullOrEmpty(jsonData))
                    task = Activator.CreateInstance(type) as OngoingAbilityTask;
                else
                    task = JsonUtility.FromJson(jsonData, type) as OngoingAbilityTask;
            }

            return task;
        }

        #region SonTypes

        private static Type[] _ongoingTaskSonTypes;

        public static Type[] OngoingTaskSonTypes =>
            _ongoingTaskSonTypes ??= TypeUtil.GetAllSonTypesOf(typeof(OngoingAbilityTask));

        public static List<string> OngoingTaskSonTypeChoices
        {
            get
            {
                return OngoingTaskSonTypes.Select(sonType => sonType.FullName).ToList();
            }
        }
        #endregion
    }
}