using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SaveDuringPlay
{
    /// <summary>A collection of tools for finding objects</summary>
    static class ObjectTreeUtil
    {
        /// <summary>
        /// Get the full name of an object, travelling up the transform parents to the root.
        /// </summary>
        public static string GetFullName(GameObject current)
        {
            if (current == null)
                return "";
            if (current.transform.parent == null)
                return "/" + current.name;
            return GetFullName(current.transform.parent.gameObject) + "/" + current.name;
        }

        /// <summary>
        /// Will find the named object, active or inactive, from the full path.
        /// </summary>
        public static GameObject FindObjectFromFullName(string fullName, GameObject[] roots)
        {
            if (string.IsNullOrEmpty(fullName) || roots == null)
                return null;

            string[] path = fullName.Split('/');
            if (path.Length < 2)   // skip leading '/'
                return null;

            Transform root = null;
            for (int i = 0; root == null && i < roots.Length; ++i)
                if (roots[i].name == path[1])
                    root = roots[i].transform;

            if (root == null)
                return null;

            for (int i = 2; i < path.Length; ++i)   // skip root
            {
                bool found = false;
                for (int c = 0; c < root.childCount; ++c)
                {
                    Transform child = root.GetChild(c);
                    if (child.name == path[i])
                    {
                        found = true;
                        root = child;
                        break;
                    }
                }
                if (!found)
                    return null;
            }
            return root.gameObject;
        }

        /// <summary>Finds all the root objects in a scene, active or not</summary>
        public static GameObject[] FindAllRootObjectsInScene()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        }


        /// <summary>
        /// This finds all the behaviours in scene, active or inactive, excluding prefabs
        /// </summary>
        public static T[] FindAllBehavioursInScene<T>() where T : MonoBehaviour
        {
            List<T> objectsInScene = new List<T>();
            foreach (T b in Resources.FindObjectsOfTypeAll<T>())
            {
                if (b == null)
                    continue;   // object was deleted
                GameObject go = b.gameObject;
                if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                    continue;
                if (EditorUtility.IsPersistent(go.transform.root.gameObject))
                    continue;
                objectsInScene.Add(b);
            }
            return objectsInScene.ToArray();
        }
    }

    class GameObjectFieldScanner
    {
        /// <summary>
        /// Called for each leaf field.  Return value should be true if action was taken.
        /// It will be propagated back to the caller.
        /// </summary>
        public OnLeafFieldDelegate OnLeafField;
        public delegate bool OnLeafFieldDelegate(string fullName, Type type, ref object value);

        /// <summary>
        /// Called for each field node, if and only if OnLeafField() for it or one
        /// of its leaves returned true.
        /// </summary>
        public OnFieldValueChangedDelegate OnFieldValueChanged;
        public delegate bool OnFieldValueChangedDelegate(
            string fullName, FieldInfo fieldInfo, object fieldOwner, object value);

        /// <summary>
        /// Called for each field, to test whether to proceed with scanning it.  Return true to scan.
        /// </summary>
        public FilterFieldDelegate FilterField;
        public delegate bool FilterFieldDelegate(string fullName, FieldInfo fieldInfo);

        /// <summary>
        /// Called for each behaviour, to test whether to proceed with scanning it.  Return true to scan.
        /// </summary>
        public FilterComponentDelegate FilterComponent;
        public delegate bool FilterComponentDelegate(MonoBehaviour b);

        /// <summary>
        /// The leafmost UnityEngine.Object
        /// </summary>
        public UnityEngine.Object LeafObject { get; private set; }

        /// <summary>
        /// Which fields will be scanned
        /// </summary>
        const BindingFlags kBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        bool ScanFields(string fullName, Type type, ref object obj)
        {
            bool doneSomething = false;

            // Check if it's a complex type
            bool isLeaf = true;
            if (obj != null
                && !typeof(Component).IsAssignableFrom(type)
                && !typeof(ScriptableObject).IsAssignableFrom(type)
                && !typeof(GameObject).IsAssignableFrom(type))
            {
                // Is it an array?
                if (type.IsArray)
                {
                    isLeaf = false;
                    var array = obj as Array;
                    object arrayLength = array.Length;
                    if (OnLeafField != null && OnLeafField(
                            fullName + ".Length", arrayLength.GetType(), ref arrayLength))
                    {
                        Array newArray = Array.CreateInstance(
                                array.GetType().GetElementType(), Convert.ToInt32(arrayLength));
                        Array.Copy(array, 0, newArray, 0, Math.Min(array.Length, newArray.Length));
                        array = newArray;
                        doneSomething = true;
                    }
                    for (int i = 0; i < array.Length; ++i)
                    {
                        object element = array.GetValue(i);
                        if (ScanFields(fullName + "[" + i + "]", array.GetType().GetElementType(), ref element))
                        {
                            array.SetValue(element, i);
                            doneSomething = true;
                        }
                    }
                    if (doneSomething)
                        obj = array;
                }
                else
                {
                    // Check if it's a complex type
                    FieldInfo[] fields = obj.GetType().GetFields(kBindingFlags);
                    if (fields.Length > 0)
                    {
                        isLeaf = false;
                        for (int i = 0; i < fields.Length; ++i)
                        {
                            string name = fullName + "." + fields[i].Name;
                            if (FilterField == null || FilterField(name, fields[i]))
                            {
                                object fieldValue = fields[i].GetValue(obj);
                                if (ScanFields(name, fields[i].FieldType, ref fieldValue))
                                {
                                    doneSomething = true;
                                    if (OnFieldValueChanged != null)
                                        OnFieldValueChanged(name, fields[i], obj, fieldValue);
                                }
                            }
                        }
                    }
                }
            }
            // If it's a leaf field then call the leaf handler
            if (isLeaf && OnLeafField != null)
                if (OnLeafField(fullName, type, ref obj))
                    doneSomething = true;

            return doneSomething;
        }

        bool ScanFields(string fullName, MonoBehaviour b)
        {
            bool doneSomething = false;
            LeafObject = b;

            FieldInfo[] fields = b.GetType().GetFields(kBindingFlags);
            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; ++i)
                {
                    string name = fullName + "." + fields[i].Name;
                    if (FilterField == null || FilterField(name, fields[i]))
                    {
                        object fieldValue = fields[i].GetValue(b);
                        if (ScanFields(name, fields[i].FieldType, ref fieldValue))
                            doneSomething = true;

                        // If leaf action was taken, propagate it up to the parent node
                        if (doneSomething && OnFieldValueChanged != null)
                            OnFieldValueChanged(fullName, fields[i], b, fieldValue);
                    }
                }
            }
            return doneSomething;
        }

        /// <summary>
        /// Recursively scan [SaveDuringPlay] MonoBehaviours of a GameObject and its children.
        /// For each leaf field found, call the OnFieldValue delegate.
        /// </summary>
        public bool ScanFields(GameObject go, string prefix = null)
        {
            bool doneSomething = false;
            if (prefix == null)
                prefix = "";
            else if (prefix.Length > 0)
                prefix += ".";

            MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
            for (int i = 0; i < components.Length; ++i)
            {
                MonoBehaviour c = components[i];
                if (c == null || (FilterComponent != null && !FilterComponent(c)))
                    continue;
                if (ScanFields(prefix + c.GetType().FullName + i, c))
                    doneSomething = true;
            }
            return doneSomething;
        }
    };
    


    /// <summary>
    /// Using reflection, this class scans a GameObject (and optionally its children)
    /// and records all the field settings.  This only works for "nice" field settings
    /// within MonoBehaviours.  Changes to the behaviour stack made between saving
    /// and restoring will fool this class.
    /// </summary>
    class ObjectStateSaver
    {
        string mObjectFullPath;

        Dictionary<string, string> mValues = new Dictionary<string, string>();

        /// <summary>
        /// Recursively collect all the field values in the MonoBehaviours
        /// owned by this object and its descendants.  The values are stored
        /// in an internal dictionary.
        /// </summary>
        public void CollectFieldValues(GameObject go)
        {
            mObjectFullPath = ObjectTreeUtil.GetFullName(go);
            GameObjectFieldScanner scanner = new GameObjectFieldScanner();
            scanner.FilterField = FilterField;
            scanner.FilterComponent = HasSaveDuringPlay;
            scanner.OnLeafField = (string fullName, Type type, ref object value) =>
                {
                    // Save the value in the dictionary
                    mValues[fullName] = StringFromLeafObject(value);
                    //Debug.Log(mObjectFullPath + "." + fullName + " = " + mValues[fullName]);
                    return false;
                };
            scanner.ScanFields(go);
        }

        public GameObject FindSavedGameObject(GameObject[] roots)
        {
            return ObjectTreeUtil.FindObjectFromFullName(mObjectFullPath, roots);
        }

        /// <summary>
        /// Recursively scan the MonoBehaviours of a GameObject and its children.
        /// For each field found, look up its value in the internal dictionary.
        /// If it's present and its value in the dictionary differs from the actual
        /// value in the game object, Set the GameObject's value using the value
        /// recorded in the dictionary.
        /// </summary>
        public bool PutFieldValues(GameObject go, GameObject[] roots)
        {
            GameObjectFieldScanner scanner = new GameObjectFieldScanner();
            scanner.FilterField = FilterField;
            scanner.FilterComponent = HasSaveDuringPlay;
            scanner.OnLeafField = (string fullName, Type type, ref object value) =>
                {
                    // Lookup the value in the dictionary
                    if (mValues.TryGetValue(fullName, out string savedValue)
                        && StringFromLeafObject(value) != savedValue)
                    {
                        //Debug.Log("Put " + mObjectFullPath + "." + fullName + " = " + mValues[fullName]);
                        value = LeafObjectFromString(type, mValues[fullName].Trim(), roots);
                        return true; // changed
                    }
                    return false;
                };
            scanner.OnFieldValueChanged = (fullName, fieldInfo, fieldOwner, value) =>
                {
                    fieldInfo.SetValue(fieldOwner, value);
                    if (PrefabUtility.GetPrefabInstanceStatus(go) != PrefabInstanceStatus.NotAPrefab)
                        PrefabUtility.RecordPrefabInstancePropertyModifications(scanner.LeafObject);
                    return true;
                };
            return scanner.ScanFields(go);
        }

        /// Ignore fields marked with the [NoSaveDuringPlay] attribute
        static bool FilterField(string fullName, FieldInfo fieldInfo)
        {
            var attrs = fieldInfo.GetCustomAttributes(false);
            foreach (var attr in attrs)
                if (attr.GetType().Name.Equals("NoSaveDuringPlayAttribute"))
                    return false;
            return true;
        }

        /// Only process components with the [SaveDuringPlay] attribute
        public static bool HasSaveDuringPlay(MonoBehaviour b)
        {
            var attrs = b.GetType().GetCustomAttributes(true);
            foreach (var attr in attrs)
                if (attr.GetType().Name.Equals("SaveDuringPlayAttribute"))
                    return true;
            return false;
        }

        /// <summary>
        /// Parse a string to generate an object.
        /// Only very limited primitive object types are supported.
        /// Enums, Vectors and most other structures are automatically supported,
        /// because the reflection system breaks them down into their primitive components.
        /// You can add more support here, as needed.
        /// </summary>
        static object LeafObjectFromString(Type type, string value, GameObject[] roots)
        {
            if (type == typeof(Single))
                return float.Parse(value);
            if (type == typeof(Double))
                return double.Parse(value);
            if (type == typeof(Boolean))
                return Boolean.Parse(value);
            if (type == typeof(string))
                return value;
            if (type == typeof(Int32))
                return Int32.Parse(value);
            if (type == typeof(UInt32))
                return UInt32.Parse(value);
            if (typeof(Component).IsAssignableFrom(type))
            {
                // Try to find the named game object
                GameObject go = ObjectTreeUtil.FindObjectFromFullName(value, roots);
                return (go != null) ? go.GetComponent(type) : null;
            }
            if (typeof(GameObject).IsAssignableFrom(type))
            {
                // Try to find the named game object
                return GameObject.Find(value);
            }
            if (typeof(ScriptableObject).IsAssignableFrom(type))
            {
                return AssetDatabase.LoadAssetAtPath(value, type);
            }
            return null;
        }

        static string StringFromLeafObject(object obj)
        {
            if (obj == null)
                return string.Empty;

            if (typeof(Component).IsAssignableFrom(obj.GetType()))
            {
                Component c = (Component)obj;
                if (c == null) // Component overrides the == operator, so we have to check
                    return string.Empty;
                return ObjectTreeUtil.GetFullName(c.gameObject);
            }
            if (typeof(GameObject).IsAssignableFrom(obj.GetType()))
            {
                GameObject go = (GameObject)obj;
                if (go == null) // GameObject overrides the == operator, so we have to check
                    return string.Empty;
                return ObjectTreeUtil.GetFullName(go);
            }
            if (typeof(ScriptableObject).IsAssignableFrom(obj.GetType()))
            {
                return AssetDatabase.GetAssetPath(obj as ScriptableObject);
            }
            return obj.ToString();
        }
    };


    /// <summary>
    /// For all registered object types, record their state when exiting Play Mode,
    /// and restore that state to the objects in the scene.  This is a very limited
    /// implementation which has not been rigorously tested with many objects types.
    /// It's quite possible that not everything will be saved.
    ///
    /// This class is expected to become obsolete when Unity implements this functionality
    /// in a more general way.
    ///
    /// To use this class,
    /// drop this script into your project, and add the [SaveDuringPlay] attribute to your class.
    ///
    /// Note: if you want some specific field in your class NOT to be saved during play,
    /// add a property attribute whose class name contains the string "NoSaveDuringPlay"
    /// and the field will not be saved.
    /// </summary>
    [InitializeOnLoad]
    public class SaveDuringPlay
    {
        /// <summary>Editor preferences key for SaveDuringPlay enabled</summary>
        public static string kEnabledKey = "SaveDuringPlay_Enabled";

        /// <summary>Enabled status for SaveDuringPlay.  
        /// This is a global setting, saved in Editor Prefs</summary>
        public static bool Enabled
        {
            get => EditorPrefs.GetBool(kEnabledKey, false);
            set
            {
                if (value != Enabled)
                {
                    EditorPrefs.SetBool(kEnabledKey, value);
                }
            }
        }

        static SaveDuringPlay()
        {
            // Install our callbacks
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += OnPlayStateChanged;
#else
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.playmodeStateChanged += OnPlayStateChanged;
#endif
        }

#if UNITY_2017_2_OR_NEWER
        static void OnPlayStateChanged(PlayModeStateChange pmsc)
        {
            if (Enabled)
            {
                switch (pmsc)
                {
                    // If exiting playmode, collect the state of all interesting objects
                    case PlayModeStateChange.ExitingPlayMode:
                        SaveAllInterestingStates();
                        break;
                    case PlayModeStateChange.EnteredEditMode when sSavedStates != null:
                        RestoreAllInterestingStates();
                        break;
                }
            }
        }
#else
        static void OnPlayStateChanged()
        {
            // If exiting playmode, collect the state of all interesting objects
            if (Enabled)
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
                    SaveAllInterestingStates();
            }
        }

        static float sWaitStartTime = 0;
        static void OnEditorUpdate()
        {
            if (Enabled && sSavedStates != null && !Application.isPlaying)
            {
                // Wait a bit for things to settle before applying the saved state
                const float WaitTime = 1f; // GML todo: is there a better way to do this?
                float time = Time.realtimeSinceStartup;
                if (sWaitStartTime == 0)
                    sWaitStartTime = time;
                else if (time - sWaitStartTime > WaitTime)
                {
                    RestoreAllInterestingStates();
                    sWaitStartTime = 0;
                }
            }
        }
#endif

        /// <summary>
        /// If you need to get notified before state is collected for hotsave, this is the place
        /// </summary>
        public static OnHotSaveDelegate OnHotSave;

        /// <summary>Delegate for HotSave notification</summary>
        public delegate void OnHotSaveDelegate();

        /// Collect all relevant objects, active or not
        static HashSet<GameObject> FindInterestingObjects()
        {
            var objects = new HashSet<GameObject>();
            MonoBehaviour[] everything = ObjectTreeUtil.FindAllBehavioursInScene<MonoBehaviour>();
            foreach (var b in everything)
            {
                if (!objects.Contains(b.gameObject) && ObjectStateSaver.HasSaveDuringPlay(b))
                {
                    //Debug.Log("Found " + ObjectTreeUtil.GetFullName(b.gameObject) + " for hot-save");
                    objects.Add(b.gameObject);
                }
            }
            return objects;
        }

        static List<ObjectStateSaver> sSavedStates = null;

        static void SaveAllInterestingStates()
        {
            //Debug.Log("Exiting play mode: Saving state for all interesting objects");
            if (OnHotSave != null)
                OnHotSave();

            sSavedStates = new List<ObjectStateSaver>();
            var objects = FindInterestingObjects();
            foreach (var obj in objects)
            {
                var saver = new ObjectStateSaver();
                saver.CollectFieldValues(obj);
                sSavedStates.Add(saver);
            }
            if (sSavedStates.Count == 0)
                sSavedStates = null;
        }

        static void RestoreAllInterestingStates()
        {
            //Debug.Log("Updating state for all interesting objects");
            bool dirty = false;
            GameObject[] roots = ObjectTreeUtil.FindAllRootObjectsInScene();
            foreach (ObjectStateSaver saver in sSavedStates)
            {
                GameObject go = saver.FindSavedGameObject(roots);
                if (go != null)
                {
                    Undo.RegisterFullObjectHierarchyUndo(go, "SaveDuringPlay");
                    if (saver.PutFieldValues(go, roots))
                    {
                        //Debug.Log("SaveDuringPlay: updated settings of " + saver.ObjetFullPath);
                        EditorUtility.SetDirty(go);
                        dirty = true;
                    }
                }
            }
            if (dirty)
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            sSavedStates = null;
        }
    }
}
