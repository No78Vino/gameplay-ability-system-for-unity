using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if !UNITY_2018_3_OR_NEWER
namespace Com.LuisPedroFonseca.ProCamera2D
{
    public static class ProCamera2DPrefs
    {	
	    
    	static bool _prefsLoaded;

        static Color[] _procamera2DGizmosColors;
        readonly static string[] _procamera2DGizmosKeys = new string[]
        {
            PrefsData.RoomsColorKey,
        	PrefsData.NumericBoundariesColorKey, 
        	PrefsData.TargetsMidPointColorKey,
        	PrefsData.InfluencesColorKey,
        	PrefsData.ShakeInfluenceColorKey,
        	PrefsData.OverallOffsetColorKey,
        	PrefsData.CamDistanceColorKey,
        	PrefsData.CamTargetPositionColorKey,
        	PrefsData.CamTargetPositionSmoothedColorKey,
        	PrefsData.CurrentCameraPositionColorKey,
        	PrefsData.CameraWindowColorKey,

        	PrefsData.ForwardFocusColorKey,

			PrefsData.ZoomToFitColorKey,

            PrefsData.RailsColorKey,

            PrefsData.PanEdgesColorKey,
	        
	        PrefsData.FitterFillColorKey,
	        PrefsData.FitterLineColorKey,

            PrefsData.BoundariesTriggerColorKey,

            PrefsData.InfluenceTriggerColorKey,

            PrefsData.ZoomTriggerColorKey,

            PrefsData.TriggerShapeColorKey
        };
        readonly static Color[] _procamera2DGizmosValues = new Color[]
        {
            PrefsData.RoomsColorValue,
        	PrefsData.NumericBoundariesColorValue, 
        	PrefsData.TargetsMidPointColorValue,
        	PrefsData.InfluencesColorValue,
        	PrefsData.ShakeInfluenceColorValue,
        	PrefsData.OverallOffsetColorValue,
        	PrefsData.CamDistanceColorValue,
        	PrefsData.CamTargetPositionColorValue,
        	PrefsData.CamTargetPositionSmoothedColorValue,
        	PrefsData.CurrentCameraPositionColorValue,
        	PrefsData.CameraWindowColorValue,

        	PrefsData.ForwardFocusColorValue,

			PrefsData.ZoomToFitColorValue,   

            PrefsData.RailsColorValue,

            PrefsData.PanEdgesColorValue,
	        
	        PrefsData.FitterFillColorValue,
	        PrefsData.FitterLineColorValue,

            PrefsData.BoundariesTriggerColorValue,

            PrefsData.InfluenceTriggerColorValue,

            PrefsData.ZoomTriggerColorValue,

            PrefsData.TriggerShapeColorValue
        };

        static Vector2 _scrollPos;

        [PreferenceItem("ProCamera2D")]
        static void PreferencesGUI()
        {
            // Load the preferences
            if (!_prefsLoaded)
            {
                _procamera2DGizmosColors = new Color[_procamera2DGizmosKeys.Length];
                for (int i = 0; i < _procamera2DGizmosColors.Length; i++) 
                {
                	_procamera2DGizmosColors[i] = EditorPrefsX.GetColor(_procamera2DGizmosKeys[i], _procamera2DGizmosValues[i]);
                }

                _prefsLoaded = true;
            }
		
            // Preferences GUI
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            GUILayout.Label("ProCamera2D", EditorStyles.boldLabel);
            for (int i = 0; i < _procamera2DGizmosColors.Length; i++) 
            {
            	_procamera2DGizmosColors[i] = EditorGUILayout.ColorField(_procamera2DGizmosKeys[i], _procamera2DGizmosColors[i]);

            	if(i == 9)
            	{
            		EditorGUILayout.Space();
            		GUILayout.Label("Extensions", EditorStyles.boldLabel);
            	}

                if(i == 16)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label("Triggers", EditorStyles.boldLabel);
                }
            }

            // Rails snapping
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("Rails Snapping", EditorStyles.boldLabel);
            EditorPrefs.SetFloat("RailsSnapping", EditorGUILayout.Slider(EditorPrefs.GetFloat("RailsSnapping"), .1f, 10f));

            // Rooms snapping
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("Rooms Snapping", EditorStyles.boldLabel);
            EditorPrefs.SetFloat("RoomsSnapping", EditorGUILayout.Slider(EditorPrefs.GetFloat("RoomsSnapping"), .01f, 10f));

            // Reset defaults
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Use defaults", GUILayout.Width(120)))
            {
                for (int i = 0; i < _procamera2DGizmosColors.Length; i++) 
	            {
	            	EditorPrefsX.SetColor(_procamera2DGizmosKeys[i], _procamera2DGizmosValues[i]);
	            }
            }

            EditorGUILayout.EndScrollView();
		
            // Save the preferences
            if (GUI.changed)
            {
                for (int i = 0; i < _procamera2DGizmosColors.Length; i++) 
		        {
		        	EditorPrefsX.SetColor(_procamera2DGizmosKeys[i], _procamera2DGizmosColors[i]);
		        }
            }
        }
    }
}
#endif
