namespace Com.LuisPedroFonseca.ProCamera2D
{
    using UnityEngine;
    using UnityEditor;

    public static class Cleanup
    {
        [MenuItem("Edit/Cleanup Missing Scripts")]
        static void CleanupMissingScripts()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                var gameObject = Selection.gameObjects[i];

                // We must use the GetComponents array to actually detect missing components
                var components = gameObject.GetComponents<Component>();

                // Create a serialized object so that we can edit the component list
                var serializedObject = new SerializedObject(gameObject);
                // Find the component list property
                var prop = serializedObject.FindProperty("m_Component");

                // Track how many components we've removed
                int r = 0;

                // Iterate over all components
                for (int j = 0; j < components.Length; j++)
                {
                    // Check if the ref is null
                    if (components[j] == null)
                    {
                        // If so, remove from the serialized component array
                        prop.DeleteArrayElementAtIndex(j - r);
                        // Increment removed count
                        r++;

                        Debug.Log("Removed missing script");
                    }
                }

                // Apply our changes to the game object
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}