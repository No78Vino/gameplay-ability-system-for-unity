using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cinemachine.Editor
{
    [InitializeOnLoad]
    static class CinemachineEditorAnalytics
    {
        const int k_MaxEventsPerHour = 360;
        const int k_MaxNumberOfElements = 1000;
        const string k_VendorKey = "unity.cinemachine";
        
        // register an event handler when the class is initialized
        static CinemachineEditorAnalytics()
        {
            EditorApplication.playModeStateChanged += SendAnalyticsOnPlayEnter;
        }

        /// <summary>
        /// Send analytics event when using Create -> Cinemachine menu
        /// </summary>
        /// <param name="name">Name of the vcam created</param>
        public static void SendCreateEvent(string name)
        {
            if (!EditorAnalytics.enabled)
                return;

            var data = new CreateEventData
            {
                vcam_created = name,
            };

            // Register our event
            EditorAnalytics.RegisterEventWithLimit("cm_create_vcam", 
                k_MaxEventsPerHour, k_MaxNumberOfElements, k_VendorKey);

            // Send the data to the database
            EditorAnalytics.SendEventWithLimit("cm_create_vcam", data);
        }

        struct CreateEventData
        {
            public string vcam_created; // vcam created from Create -> Cinemachine menu
        }

        /// <summary>
        /// Send analytics event when using entering playmode
        /// </summary>
        /// <param name="state">State change to detect entering playmode</param>
        static void SendAnalyticsOnPlayEnter(PlayModeStateChange state)
        {
            // Only send analytics if it is enabled
            if (!EditorAnalytics.enabled)
                return;

            // Only send data when entering playmode
            if (state != PlayModeStateChange.EnteredPlayMode)
                return;

            var startTime = Time.realtimeSinceStartup;

            var cinemachineCore = CinemachineCore.Instance;
            var vcamCount = cinemachineCore.VirtualCameraCount;
            var vcamDatas = new List<VcamData>();

            // collect data from all vcams
            for (int i = 0; i < vcamCount; ++i)
            {
                var vcamBase = cinemachineCore.GetVirtualCamera(i);
                CollectData(vcamBase, i.ToString(), ref vcamDatas);
            }

            var projectData = new ProjectData
            {
                brain_count = cinemachineCore.BrainCount,
                vcam_count = cinemachineCore.VirtualCameraCount,
                cam_count = Camera.allCamerasCount,
                vcams = vcamDatas,
                time_elapsed = Time.realtimeSinceStartup - startTime,
            };

            // Register our event
            EditorAnalytics.RegisterEventWithLimit("cm_vcams_on_play", 
                k_MaxEventsPerHour, k_MaxNumberOfElements, k_VendorKey);
            
            // Send the data to the database
            EditorAnalytics.SendEventWithLimit("cm_vcams_on_play", projectData);
        }

        static void CollectData(CinemachineVirtualCameraBase vcamBase, string id, ref List<VcamData> vcamDatas)
        {
            if (vcamBase == null) return;
            
            var vcamData = new VcamData(id, vcamBase);
            
            // VirtualCamera
            var vcam = vcamBase as CinemachineVirtualCamera;
            if (vcam != null)
            {
                vcamData.SetTransitionsAndLens(vcam.m_Transitions, vcam.m_Lens);
                vcamData.SetComponents(vcam.GetComponentPipeline());
                
                vcamDatas.Add(vcamData);
                return;
            }     
            
#if CINEMACHINE_EXPERIMENTAL_VCAM
            // NewVirtualCamera or NewFreeLook
            var vcamNew = vcamBase as CinemachineNewVirtualCamera;
            if (vcamNew != null)
            {
                vcamData.SetTransitionsAndLens(vcamNew.m_Transitions, vcamNew.m_Lens);
                vcamData.SetComponents(vcamNew.ComponentCache);
                
                vcamDatas.Add(vcamData);
                return;
            }
#endif
            
            // Composite vcam (Freelook, Mixing, StateDriven, ClearShot...):
            var freeLook = vcamBase as CinemachineFreeLook;
            if (freeLook != null)
            {
                vcamData.SetTransitionsAndLens(freeLook.m_Transitions, freeLook.m_Lens);
            }
            vcamDatas.Add(vcamData);

            var vcamChildren = 
                vcamBase.GetComponentsInChildren<CinemachineVirtualCameraBase>();
            for (var c = 1; c < vcamChildren.Length; c++)
            {
                if ((CinemachineVirtualCameraBase)vcamChildren[c].ParentCamera == vcamBase)
                    CollectData(vcamChildren[c], id + "." + c, ref vcamDatas);
            }
        }

        [Serializable]
        struct ProjectData
        {
            public int brain_count;
            public int vcam_count;
            public int cam_count;
            public List<VcamData> vcams;
            public float time_elapsed;
        }

        [Serializable]
        struct VcamData
        {
            public string id;
            public string vcam_class;
            public bool has_follow_target;
            public bool has_lookat_target;
            public string blend_hint;
            public bool inherit_position;
            public string standby_update;
            public string mode_overwrite;
            public string body_component;
            public string aim_component;
            public string noise_component;
            public int custom_component_count;
            public string[] extensions;
            public int custom_extension_count;

            public VcamData(string id, CinemachineVirtualCameraBase vcamBase) : this()
            {
                var _ = 0;
                
                this.id = id;
                vcam_class = GetTypeName(vcamBase.GetType(), ref _);
                has_follow_target = vcamBase.Follow != null;
                has_lookat_target = vcamBase.LookAt != null;
                blend_hint = "";
                inherit_position = false;
                standby_update = vcamBase.m_StandbyUpdate.ToString();
                mode_overwrite = "";
                body_component = "";
                aim_component = "";
                noise_component = "";
                custom_component_count = 0;
                custom_extension_count = 0;
                var vcamExtensions = vcamBase.mExtensions;
                if (vcamExtensions != null)
                {
                    extensions = new string[vcamExtensions.Count];
                    for (var i = 0; i < vcamExtensions.Count; i++)
                    {
                        extensions[i] = (GetTypeName(vcamExtensions[i].GetType(), ref custom_extension_count));
                    }
                }
                else
                {
                    extensions = Array.Empty<string>();
                }
            }
            
            public void SetTransitionsAndLens(
                CinemachineVirtualCameraBase.TransitionParams transitions, LensSettings lens)
            {
                blend_hint = transitions.m_BlendHint.ToString();
                inherit_position = transitions.m_InheritPosition;
                mode_overwrite = lens.ModeOverride.ToString();
            }

            public void SetComponents(CinemachineComponentBase[] cmComps)
            {
                custom_component_count = 0;
                body_component = aim_component = noise_component = "";
                if (cmComps != null)
                {
                    for (var i = 0; i < cmComps.Length; i++)
                    {
#if CINEMACHINE_EXPERIMENTAL_VCAM
                        if (cmComps[i] == null) continue;
#endif
                        var componentName = GetTypeName(cmComps[i].GetType(), ref custom_component_count);
                        switch (cmComps[i].Stage)
                        {
                            case CinemachineCore.Stage.Body:
                                body_component = componentName;
                                break;
                            case CinemachineCore.Stage.Aim:
                                aim_component = componentName;
                                break;
                            case CinemachineCore.Stage.Noise:
                                noise_component = componentName;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            static string GetTypeName(Type type, ref int customTypeCount)
            {
                if (typeof(CinemachineBrain).Assembly != type.Assembly)
                {
                    ++customTypeCount;
                    return "Custom";
                }
                return type.Name;
            }
        }
    }
}
