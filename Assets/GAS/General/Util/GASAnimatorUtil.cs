using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace GAS.General
{
    public static class GASAnimatorUtil
    {
        /// <summary>
        ///     Only For Editor
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public static Dictionary<string, AnimationClip> GetAllAnimationState(this Animator animator, int layerIndex = 0)
        {
#pragma warning disable 162
#if UNITY_EDITOR
            var result = new Dictionary<string, AnimationClip>();

            var runtimeController = animator.runtimeAnimatorController;
            if (runtimeController == null)
            {
                Debug.LogError("RuntimeAnimatorController must not be null.");
                return null;
            }

            if (animator.runtimeAnimatorController is AnimatorOverrideController)
            {
                var overrideController =
                    AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(
                        AssetDatabase.GetAssetPath(runtimeController));
                if (overrideController == null)
                {
                    Debug.LogErrorFormat("AnimatorOverrideController must not be null.");
                    return null;
                }

                var controller =
                    AssetDatabase.LoadAssetAtPath<AnimatorController>(
                        AssetDatabase.GetAssetPath(overrideController.runtimeAnimatorController));
                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                overrideController.GetOverrides(overrides);
                // 获取 Layer 的状态机
                var stateMachine = controller.layers[layerIndex].stateMachine;
                // 遍历所有状态并打印名称
                foreach (var state in stateMachine.states)
                {
                    if (state.state.motion is AnimationClip clip)
                    {
                        foreach (var pair in overrides)
                        {
                            if (pair.Key.name == clip.name)
                            {
                                result.Add(state.state.name, pair.Value);
                                break;
                            }
                        }

                        if (!result.ContainsKey(state.state.name)) result.Add(state.state.name, clip);
                    }
                }
            }
            else
            {
                var controller =
                    AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(runtimeController));
                if (controller == null)
                {
                    Debug.LogErrorFormat("AnimatorController must not be null.");
                    return null;
                }

                // 获取第一个 Layer 的状态机
                var stateMachine = controller.layers[layerIndex].stateMachine;
                // 遍历所有状态并打印名称
                foreach (var state in stateMachine.states)
                    result.Add(state.state.name, state.state.motion as AnimationClip);
            }

            return result;
#endif
            return null;
#pragma warning restore 162
        }
    }
}