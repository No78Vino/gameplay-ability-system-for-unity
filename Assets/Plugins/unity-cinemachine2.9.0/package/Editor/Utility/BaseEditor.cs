using UnityEditor;
using System;
using System.Collections.Generic;
using Cinemachine.Utility;
using System.Linq.Expressions;

namespace Cinemachine.Editor
{
    /// <summary>
    /// A convenience base class for making inspector editors.
    /// </summary>
    /// <typeparam name="T">The class we're editing</typeparam>
    public class BaseEditor<T> : UnityEditor.Editor where T : class
    {
        /// <summary>
        /// The target object, cast as the same class as the object being edited
        /// </summary>
        protected T Target { get { return target as T; } }

        /// <summary>
        /// Return the Serialized property for a field, and exclude it from being automatically 
        /// displayed in the inspector.  Call this when you need to provide a custom UX for a field.
        /// </summary>
        /// <typeparam name="TValue">Magic experssion code</typeparam>
        /// <param name="expr">Call format is FindAndExcludeProperty(x => x.myField)</param>
        /// <returns>The serialized property for the field</returns>
        protected SerializedProperty FindAndExcludeProperty<TValue>(Expression<Func<T, TValue>> expr)
        {
            SerializedProperty p = FindProperty(expr);
            ExcludeProperty(p.name);
            return p;
        }

        /// <summary>
        /// Return the Serialized property for a field.
        /// </summary>
        /// <typeparam name="TValue">Magic experssion code</typeparam>
        /// <param name="expr">Call format is FindProperty(x => x.myField)</param>
        /// <returns>The serialized property for the field</returns>
        protected SerializedProperty FindProperty<TValue>(Expression<Func<T, TValue>> expr)
        {
            return serializedObject.FindProperty(FieldPath(expr));
        }

        /// <summary>
        /// Magic code to get the string name of a field.  Will not build if the field name changes.
        /// </summary>
        /// <typeparam name="TValue">Magic experssion code</typeparam>
        /// <param name="expr">Call format is FieldPath(x => x.myField)</param>
        /// <returns>The string name of the field</returns>
        protected string FieldPath<TValue>(Expression<Func<T, TValue>> expr)
        {
            return ReflectionHelpers.GetFieldPath(expr);
        }

        /// <summary>Obsolete, do not use.  Use the overload, which is more performant</summary>
        /// <returns>List of property names to exclude</returns>
        protected virtual List<string> GetExcludedPropertiesInInspector() { return mExcluded; }

        /// <summary>Get the property names to exclude in the inspector.</summary>
        /// <param name="excluded">Add the names to this list</param>
        protected virtual void GetExcludedPropertiesInInspector(List<string> excluded)
        {
            excluded.Add("m_Script");
        }

        /// <summary>
        /// Exclude a property from automatic inclusion in the inspector 
        /// when DrawRemainingPropertiesInInspector() is called
        /// </summary>
        /// <param name="propertyName">The property to exclude</param>
        protected void ExcludeProperty(string propertyName)
        {
            mExcluded.Add(propertyName);
        }

        /// <summary>Check whenther a property is in the excluded list</summary>
        /// <param name="propertyName">The property to check</param>
        /// <returns>True if property is excluded from automatic inclusion in the inspector 
        /// when DrawRemainingPropertiesInInspector() is called</returns>
        protected bool IsPropertyExcluded(string propertyName)
        {
            return mExcluded.Contains(propertyName);
        }

        /// <summary>
        /// Draw the inspector
        /// </summary>
        public override void OnInspectorGUI()
        {
            BeginInspector();
            DrawRemainingPropertiesInInspector();
        }

        List<string> mExcluded = new List<string>();

        /// <summary>
        /// Clients should call this at the start of OnInspectorGUI.  
        /// Updates the serialized object and Sets up for excluded properties.
        /// </summary>
        protected virtual void BeginInspector()
        {
            serializedObject.Update();
            mExcluded.Clear();
            GetExcludedPropertiesInInspector(mExcluded);
        }

        /// <summary>
        /// Draw a property in the inspector, if it is not excluded.  
        /// Property is marked as drawn, so will not be drawn again 
        /// by DrawRemainingPropertiesInInspector()
        /// </summary>
        /// <param name="p">The property to draw</param>
        protected virtual void DrawPropertyInInspector(SerializedProperty p)
        {
            if (!IsPropertyExcluded(p.name))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(p);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                ExcludeProperty(p.name);
            }
        }

        /// <summary>
        /// Draw all remaining unexcluded undrawn properties in the inspector.
        /// </summary>
        protected void DrawRemainingPropertiesInInspector()
        {
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, mExcluded.ToArray());
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
