using UnityEngine;
using System;
using System.Collections.Generic;

namespace RootMotion
{
    public enum ShowIfMode
    {
        Disabled = 0,
        Hidden = 1
    }
    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string propName { get; protected set; }
        public object propValue { get; protected set; }
        public object otherPropValue { get; protected set; }
        public bool indent { get; private set; }
        public ShowIfMode mode { get; protected set; }

        public ShowIfAttribute(string propertyName, object propertyValue = null, object otherPropertyValue = null, bool indent = false, ShowIfMode mode = ShowIfMode.Hidden)
        {
            this.propName = propertyName;
            this.propValue = propertyValue;
            this.otherPropValue = otherPropertyValue;
            this.indent = indent;
            this.mode = mode;
        }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ShowRangeIfAttribute : ShowIfAttribute
    {
        public float min { get; private set; }
        public float max { get; private set; }

        public ShowRangeIfAttribute(float min, float max, string propertyName, object propertyValue = null, object otherPropertyValue = null, bool indent = false, ShowIfMode mode = ShowIfMode.Hidden) : base (propertyName, propertyValue, otherPropertyValue, indent, mode)
        {
            this.min = min;
            this.max = max;
        }
    }

    /// <summary>
    /// Large header attribute for Editor.
    /// </summary>
    public class ShowLargeHeaderIf : ShowIfAttribute
    {

        public string name;
        public string color = "white";

        public ShowLargeHeaderIf(string name, string propertyName, object propertyValue = null, object otherPropertyValue = null, bool indent = false, ShowIfMode mode = ShowIfMode.Hidden) : base(propertyName, propertyValue, otherPropertyValue, indent, mode)
        {
            this.name = name;
            this.color = "white";
        }

        public ShowLargeHeaderIf(string name, string color, string propertyName, object propertyValue = null, object otherPropertyValue = null, bool indent = false, ShowIfMode mode = ShowIfMode.Hidden) : base(propertyName, propertyValue, otherPropertyValue, indent, mode)
        {
            this.name = name;
            this.color = color;
        }
    }

    /// <summary>
	/// Large header attribute for Editor.
	/// </summary>
	public class LargeHeader : PropertyAttribute
    {

        public string name;
        public string color = "white";

        public LargeHeader(string name)
        {
            this.name = name;
            this.color = "white";
        }

        public LargeHeader(string name, string color)
        {
            this.name = name;
            this.color = color;
        }
    }
}