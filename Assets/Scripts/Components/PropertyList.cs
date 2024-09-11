using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Flamenccio.DataHandling
{
    public class PropertyList : MonoBehaviour
    {
        public List<ObjectProperty> Properties = new();
    }

    /// <summary>
    /// Allows users to choose any object in the scene or assets and store any of their properties.
    /// </summary>
    [System.Serializable]
    public class ObjectProperty
    {
        /// <summary>
        /// Unity object to choose properties from.
        /// </summary>
        public UnityEngine.Object Source;

        /// <summary>
        /// The name of the selected property.
        /// </summary>
        public string PropertyName;

        /// <summary>
        /// A "nickname" for the selected property.
        /// </summary>
        public string VariableName;

        /// <summary>
        /// PropertyInfo of the selected property.
        /// </summary>
        public PropertyInfo SourceProperty;

        /// <summary>
        /// The MonoBehaviour where the selected property is from.
        /// </summary>
        public MonoBehaviour SourceScript;

        /// <summary>
        /// Update SourceProperty according to Source and DataName.
        /// </summary>
        public void UpdateSourceProperty()
        {
            if (Source == null)
            {
                Debug.LogError("Source is null.");
                return;
            }

            if (string.IsNullOrEmpty(PropertyName))
            {
                Debug.LogError("PropertyName is null or empty.");
                return;
            }

            if (SourceScript == null)
            {
                Debug.LogError("SourceScript is null");
                return;
            }

            var property = SourceScript.GetType().GetProperty(PropertyName);

            if (property == null)
            {
                Debug.LogError($"There is no such property with the name {PropertyName} in {Source}.");
                return;
            }

            SourceProperty = property;
        }

        /// <summary>
        /// Get the current value of this class's Data property. Automatically updates source property upon call.
        /// </summary>
        public object GetPropertyValue()
        {
            UpdateSourceProperty();

            if (SourceProperty == null)
            {
                Debug.LogError("SourceProperty is null.");
                return null;
            }

            if (SourceScript == null)
            {
                Debug.LogError("SourceScript is null.");
                return null;
            }

            return SourceProperty.GetValue(SourceScript);
        }
    }
}