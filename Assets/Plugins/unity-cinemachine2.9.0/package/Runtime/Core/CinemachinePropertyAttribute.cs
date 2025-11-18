using UnityEngine;

namespace Cinemachine
{
    /// <summary>
    /// Property applied to AxisState.  Used for custom drawing in the inspector.
    /// </summary>
    public sealed class AxisStatePropertyAttribute : PropertyAttribute {}

    /// <summary>
    /// Property applied to OrbitalTransposer.Heading.  Used for custom drawing in the inspector.
    /// </summary>
    public sealed class OrbitalTransposerHeadingPropertyAttribute : PropertyAttribute {}

    /// <summary>
    /// This attributs is obsolete and unused.
    /// </summary>
    public sealed class LensSettingsPropertyAttribute : PropertyAttribute {}

    /// <summary>
    /// Property applied to Vcam Target fields.  Used for custom drawing in the inspector.
    /// </summary>
    public sealed class VcamTargetPropertyAttribute : PropertyAttribute { }

    /// <summary>
    /// Property applied to CinemachineBlendDefinition.  Used for custom drawing in the inspector.
    /// </summary>
    public sealed class CinemachineBlendDefinitionPropertyAttribute : PropertyAttribute {}

    /// <summary>
    /// Invoke play-mode-save for a class.  This class's fields will be scanned
    /// upon exiting play mode, and its property values will be applied to the scene object.
    /// This is a stopgap measure that will become obsolete once Unity implements
    /// play-mode-save in a more general way.
    /// </summary>
    public sealed class SaveDuringPlayAttribute : System.Attribute {}

    /// <summary>
    /// Suppresses play-mode-save for a field.  Use it if the calsee has [SaveDuringPlay] 
    /// attribute but there are fields in the class that shouldn't be saved.
    /// </summary>
    public sealed class NoSaveDuringPlayAttribute : PropertyAttribute {}

    /// <summary>Property field is a Tag.</summary>
    public sealed class TagFieldAttribute : PropertyAttribute {}

    /// <summary>Property field is a NoiseSettings asset.</summary>
    public sealed class NoiseSettingsPropertyAttribute : PropertyAttribute {}    
    
    /// <summary>
    /// Used for custom drawing in the inspector.  Inspector will show a foldout with the asset contents
    /// </summary>
    public sealed class CinemachineEmbeddedAssetPropertyAttribute : PropertyAttribute 
    {
        /// <summary>If true, inspector will display a warning if the embedded asset is null</summary>
        public bool WarnIfNull;

        /// <summary>Standard constructor</summary>
        /// <param name="warnIfNull">If true, inspector will display a warning if the embedded asset is null</param>
        public CinemachineEmbeddedAssetPropertyAttribute(bool warnIfNull = false)
        {
            WarnIfNull = warnIfNull;
        }
    }
    
    /// <summary>
    /// Atrtribute to control the automatic generation of documentation.  This attribute is obsolete and not used.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.Undoc)]
    public sealed class DocumentationSortingAttribute : System.Attribute
    {
        /// <summary>Refinement level of the documentation</summary>
        public enum Level 
        { 
            /// <summary>Type is excluded from documentation</summary>
            Undoc, 
            /// <summary>Type is documented in the API reference</summary>
            API, 
            /// <summary>Type is documented in the highly-refined User Manual</summary>
            UserRef 
        };
        /// <summary>Refinement level of the documentation.  The more refined, the more is excluded.</summary>
        public Level Category { get; private set; }

        /// <summary>Contructor with specific values</summary>
        /// <param name="category">Documentation level</param>
        public DocumentationSortingAttribute(Level category)
        {
            Category = category;
        }
    }
}
