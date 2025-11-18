using UnityEngine;

namespace Cinemachine
{
    /// <summary>
    /// A class to get around the limitation in timeline that array members can't be animated.
    /// A fixed number of slots are made available, rather than a dynamic array.  
    /// If you want to add more slots, just modify this code.
    /// </summary>
    [RequireComponent(typeof(CinemachineTargetGroup))]
    [ExecuteAlways]
    [HelpURL(Documentation.BaseURL + "api/Cinemachine.GroupWeightManipulator.html")]
    public class GroupWeightManipulator : MonoBehaviour
    {
        /// <summary>The weight of the group member at index 0</summary>
        [Tooltip("The weight of the group member at index 0")]
        public float m_Weight0 = 1;
        /// <summary>The weight of the group member at index 1</summary>
        [Tooltip("The weight of the group member at index 1")]
        public float m_Weight1 = 1;
        /// <summary>The weight of the group member at index 2</summary>
        [Tooltip("The weight of the group member at index 2")]
        public float m_Weight2 = 1;
        /// <summary>The weight of the group member at index 3</summary>
        [Tooltip("The weight of the group member at index 3")]
        public float m_Weight3 = 1;
        /// <summary>The weight of the group member at index 4</summary>
        [Tooltip("The weight of the group member at index 4")]
        public float m_Weight4 = 1;
        /// <summary>The weight of the group member at index 5</summary>
        [Tooltip("The weight of the group member at index 5")]
        public float m_Weight5 = 1;
        /// <summary>The weight of the group member at index 6</summary>
        [Tooltip("The weight of the group member at index 6")]
        public float m_Weight6 = 1;
        /// <summary>The weight of the group member at index 7</summary>
        [Tooltip("The weight of the group member at index 7")]
        public float m_Weight7 = 1;

        CinemachineTargetGroup m_group;
        void Start()
        {
            m_group = GetComponent<CinemachineTargetGroup>();
        }

        void OnValidate()
        {
            m_Weight0 = Mathf.Max(0, m_Weight0);
            m_Weight1 = Mathf.Max(0, m_Weight1);
            m_Weight2 = Mathf.Max(0, m_Weight2);
            m_Weight3 = Mathf.Max(0, m_Weight3);
            m_Weight4 = Mathf.Max(0, m_Weight4);
            m_Weight5 = Mathf.Max(0, m_Weight5);
            m_Weight6 = Mathf.Max(0, m_Weight6);
            m_Weight7 = Mathf.Max(0, m_Weight7);
        }

        void Update()
        {
            if (m_group != null)
                UpdateWeights();
        }

        void UpdateWeights()
        {
            var targets = m_group.m_Targets;
            int last = targets.Length - 1;
            if (last < 0) return; targets[0].weight = m_Weight0;
            if (last < 1) return; targets[1].weight = m_Weight1;
            if (last < 2) return; targets[2].weight = m_Weight2;
            if (last < 3) return; targets[3].weight = m_Weight3;
            if (last < 4) return; targets[4].weight = m_Weight4;
            if (last < 5) return; targets[5].weight = m_Weight5;
            if (last < 6) return; targets[6].weight = m_Weight6;
            if (last < 7) return; targets[7].weight = m_Weight7;
        }
    }
}
