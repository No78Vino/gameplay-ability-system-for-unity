using UnityEngine;
using UnityEditor;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ProCamera2DPixelPerfectSprite))]
    public class ProCamera2DPixelPerfectSpriteEditor : Editor
    {
        MonoScript _script;
        GUIContent _tooltip;

        void OnEnable()
        {
            var proCamera2DPixelPerfectSprite = (ProCamera2DPixelPerfectSprite)target;
            
            _script = MonoScript.FromMonoBehaviour(proCamera2DPixelPerfectSprite);
        }

        public override void OnInspectorGUI()
        {
            var proCamera2DPixelPerfectSprite = (ProCamera2DPixelPerfectSprite)target;

            if (proCamera2DPixelPerfectSprite.ProCamera2D == null)
            {
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);
                return;
            }

            // No sprite found
            var hasSprite = proCamera2DPixelPerfectSprite.GetComponent<SpriteRenderer>() != null;
            if (!hasSprite)
                EditorGUILayout.HelpBox("This component needs a Sprite renderer on the same GameObject!", MessageType.Error, true);


            // Rigidbody, collider, character controller warning
            if (proCamera2DPixelPerfectSprite.IsAMovingObject &&
                (proCamera2DPixelPerfectSprite.GetComponent<Rigidbody>() != null ||
                 proCamera2DPixelPerfectSprite.GetComponent<Rigidbody2D>() != null ||
                 proCamera2DPixelPerfectSprite.GetComponent<Collider>() != null ||
                 proCamera2DPixelPerfectSprite.GetComponent<Collider2D>() != null ||
                 proCamera2DPixelPerfectSprite.GetComponent<CharacterController>() != null))
            {
                EditorGUILayout.HelpBox("You should not add this component to GameObjects that have physics components, because rounding the sprite to a pixel-perfect position might interfere with the physics calculations. Please add the sprite as a child of this GameObject and add this component to it instead.", MessageType.Warning, true);
            }

            serializedObject.Update();

            // Show script link
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);

            // Moving object
            _tooltip = new GUIContent("Is A Moving Object", "If checked, the object position will be aligned to pixel perfect every frame. To improve performance, enable only if the object (or its parent) move.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsAMovingObject"), _tooltip);

            // Child sprite
            if (proCamera2DPixelPerfectSprite.IsAMovingObject && proCamera2DPixelPerfectSprite.transform.parent != null)
            {
                _tooltip = new GUIContent("Use Parent Movement", "Enable if you're moving the parent of this sprite instead of the sprite itself.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsAChildSprite"), _tooltip);
            }
            else
            {
                proCamera2DPixelPerfectSprite.IsAChildSprite = false;
            }

            // Local Position
            if (proCamera2DPixelPerfectSprite.IsAChildSprite)
            {
                EditorGUILayout.BeginHorizontal();

                _tooltip = new GUIContent("Local Position", "If you're using the parent movement, you have to set this sprite local position using this value.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("LocalPosition"), _tooltip);

                if (GUILayout.Button("R", GUILayout.Width(20)))
                    serializedObject.FindProperty("LocalPosition").vector2Value = Vector2.zero;

                EditorGUILayout.EndHorizontal();
            }

            // Pixel Scale
            _tooltip = new GUIContent("Sprite Scale", "The scale of this sprite");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SpriteScale"), _tooltip);

            // Scale 0 warning
            if (proCamera2DPixelPerfectSprite.SpriteScale == 0)
            {
                EditorGUILayout.HelpBox("A scale of 0 allows you to manually scale the sprite, however, it doesn't guarantee that it will be pixel-perfect. Use a scale different than 0 to guarantee your sprite size is pixel-perfect.", MessageType.Warning, true);
            }

            // Save properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}