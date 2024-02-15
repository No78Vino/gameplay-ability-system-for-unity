using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Allows multiple action tasks to be added to a single node.")]
    [TaskIcon("{SkinColor}StackedActionIcon.png")]
    public class StackedAction : Action
    {
        [InspectTask]
        public Action[] actions;
        public enum ComparisonType
        {
            Sequence,
            Selector
        }
        [Tooltip("Specifies if the tasks should be traversed with an AND (Sequence) or an OR (Selector).")]
        public ComparisonType comparisonType;
        [Tooltip("Should the tasks be labeled within the graph?")]
        public bool graphLabel;

        public override void OnAwake()
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }

                actions[i].GameObject = gameObject;
                actions[i].Transform = transform;
                actions[i].Owner = Owner;
#if UNITY_EDITOR || DLL_RELEASE || DLL_DEBUG
                actions[i].NodeData = new NodeData();
#endif
                actions[i].OnAwake();
            }
        }

        public override void OnStart()
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnStart();
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (actions == null) {
                return TaskStatus.Failure;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                var executionStatus = actions[i].OnUpdate();
#if UNITY_EDITOR || DLL_RELEASE || DLL_DEBUG
                actions[i].NodeData.ExecutionStatus = executionStatus;
                if (actions[i].NodeData.ExecutionStatus == TaskStatus.Running) {
                    Debug.LogWarning("Warning: The action task returned a status of running when action tasks should only return success or failure.");
                }
#endif
                if (comparisonType == ComparisonType.Sequence && executionStatus == TaskStatus.Failure) {
                    return TaskStatus.Failure;
                } else if (comparisonType == ComparisonType.Selector && executionStatus == TaskStatus.Success) {
                    return TaskStatus.Success;
                }
            }
            return comparisonType == ComparisonType.Sequence ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnFixedUpdate()
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnFixedUpdate();
            }
        }

        public override void OnLateUpdate()
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnLateUpdate();
            }
        }

        public override void OnEnd()
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnEnd();
            }
        }

        public override void OnTriggerEnter(Collider other)
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnTriggerEnter(other);
            }
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnTriggerEnter2D(other);
            }
        }

        public override void OnTriggerExit(Collider other)
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnTriggerExit(other);
            }
        }

        public override void OnTriggerExit2D(Collider2D other)
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnTriggerExit2D(other);
            }
        }

        public override void OnCollisionEnter(Collision collision)
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnCollisionEnter(collision);
            }
        }

        public override void OnCollisionEnter2D(Collision2D collision)
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnCollisionEnter2D(collision);
            }
        }

        public override void OnCollisionExit(Collision collision)
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnCollisionExit(collision);
            }
        }

        public override void OnCollisionExit2D(Collision2D collision)
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnCollisionExit2D(collision);
            }
        }

        public override string OnDrawNodeText()
        {
            if (actions == null || !graphLabel) {
                return string.Empty;
            }

            var text = string.Empty;
            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                if (!string.IsNullOrEmpty(text)) {
                    text += "\n";
                }
                text += actions[i].GetType().Name;
            }

            return text;
        }

        public override void OnReset()
        {
            if (actions == null) {
                return;
            }

            for (int i = 0; i < actions.Length; ++i) {
                if (actions[i] == null) {
                    continue;
                }
                actions[i].OnReset();
            }
        }
    }
}