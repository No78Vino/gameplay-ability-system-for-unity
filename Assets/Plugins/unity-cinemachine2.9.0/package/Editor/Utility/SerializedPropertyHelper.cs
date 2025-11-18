using System;
using System.Linq.Expressions;
using UnityEditor;

namespace Cinemachine.Editor
{
    /// <summary>
    /// Helpers for the editor relating to SerializedPropertys
    /// </summary>
    public static class SerializedPropertyHelper
    {
        /// <summary>
        /// This is a way to get a field name string in such a manner that the compiler will
        /// generate errors for invalid fields.  Much better than directly using strings.
        /// Usage: instead of
        /// <code>
        /// "m_MyField";
        /// </code>
        /// do this:
        /// <code>
        /// MyClass myclass = null;
        /// SerializedPropertyHelper.PropertyName( () => myClass.m_MyField);
        /// </code>
        /// </summary>
        /// <param name="exp">Magic expression that resolves to a field: () => myClass.m_MyField</param>
        /// <returns></returns>
        public static string PropertyName(Expression<Func<object>> exp)
        {
            var body = exp.Body as MemberExpression;
            if (body == null)
            {
                var ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }
            return body.Member.Name;
        }

        /// <summary>
        /// A compiler-assisted (non-string-based) way to call SerializedProperty.FindProperty
        /// </summary>
        /// <param name="obj">The serialized object to search</param>
        /// <param name="exp">Magic expression that resolves to a field: () => myClass.m_MyField</param>
        /// <returns>The resulting SerializedProperty, or null</returns>
        public static SerializedProperty FindProperty(this SerializedObject obj, Expression<Func<object>> exp)
        {
            return obj.FindProperty(PropertyName(exp));
        }

        /// <summary>
        /// A compiler-assisted (non-string-based) way to call SerializedProperty.FindPropertyRelative
        /// </summary>
        /// <param name="obj">The serialized object to search</param>
        /// <param name="exp">Magic expression that resolves to a field: () => myClass.m_MyField</param>
        /// <returns>The resulting SerializedProperty, or null</returns>
        public static SerializedProperty FindPropertyRelative(this SerializedProperty obj, Expression<Func<object>> exp)
        {
            return obj.FindPropertyRelative(PropertyName(exp));
        }

        /// <summary>Get the value of a proprty, as an object</summary>
        /// <param name="property">The property to query</param>
        /// <returns>The object value of the property</returns>
        public static object GetPropertyValue(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var targetObjectClassType = targetObject.GetType();
            var field = targetObjectClassType.GetField(property.propertyPath);
            if (field != null)
                return field.GetValue(targetObject);
            return null;
        }
    }
}
