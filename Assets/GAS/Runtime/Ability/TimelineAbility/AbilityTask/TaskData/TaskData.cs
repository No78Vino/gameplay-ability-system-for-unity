using System;
using System.Collections.Generic;
using System.Linq;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public class TaskData : AbilityTaskData
    {
        public TaskData()
        {
        }
        
        public AbilityTaskBase CreateTask(AbilityLogicBase abilityLogic)
        {
            return base.Create(abilityLogic);;
        }

        public override AbilityTaskBase Load()
        {
            AbilityTaskBase task = null;
           
            // var dataType = string.IsNullOrEmpty(TaskData.Type) ? typeof(DefaultAbilityTask).FullName : TaskData.Type;
            //
            // var type = OngoingTaskSonTypes.FirstOrDefault(sonType => sonType.FullName == dataType);
            // if (type == null)
            // {
            //     Debug.LogError("[EX] OngoingAbilityTask SonType not found: " + dataType);
            // }
            // else
            // {
            //     if (string.IsNullOrEmpty(jsonData))
            //         task = Activator.CreateInstance(type) as AbilityTaskBase;
            //     else
            //         task = JsonUtility.FromJson(jsonData, type) as AbilityTaskBase;
            // }

            return task;
        }

        #region SonTypes

        private static Type[] _ongoingTaskSonTypes;

        public static Type[] OngoingTaskSonTypes =>
            _ongoingTaskSonTypes ??= TypeUtil.GetAllSonTypesOf(typeof(AbilityTaskBase<>));

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