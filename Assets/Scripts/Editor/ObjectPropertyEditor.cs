using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flamenccio.FlamenccioEditor
{
    [CustomPropertyDrawer(typeof(DataHandling.ObjectProperty))]
    public class ObjectPropertyEditor : PropertyDrawer
    {
        private Dictionary<PropertyInfo, MonoBehaviour> monobehaviourProperties = new(); // connects monobehaviours to their public properties

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new();

            // create fields
            PropertyField objectProperty = new(property.FindPropertyRelative("Source"));
            PropertyField variableName = new(property.FindPropertyRelative("VariableName"));
            PopupField<PropertyInfo> propertyPopup = new("Properties");

            // bind elements
            SerializedProperty selectedObject = property.FindPropertyRelative("Source");

            RestorePropertySelection(ref propertyPopup, property);

            // add value change callback methods
            propertyPopup.RegisterValueChangedCallback(evt => UpdateAffectedProperties(evt, property));

            objectProperty.RegisterValueChangeCallback(evt =>
            {
                UpdateMonoBehaviourProperties(evt);
                UpdatePropertySelectorChoices(ref propertyPopup);
            });

            // add ui element to container
            container.Add(objectProperty);
            container.Add(propertyPopup);
            container.Add(variableName);

            return container;
        }

        /// <summary>
        /// Create a dictionary of PropertyInfos and their corresponding MonoBehaviour
        /// </summary>
        /// <param name="unityObject">The current unity object selected</param>
        /// <returns>The dictionary of PropertyInfo and MonoBehaviour</returns>
        private Dictionary<PropertyInfo, MonoBehaviour> GetMonobehaviourProperties(UnityEngine.Object unityObject)
        {
            Dictionary<PropertyInfo, MonoBehaviour> newDict = new();

            if (unityObject == null) return newDict;

            var monoBehaviours = unityObject.GetComponents<MonoBehaviour>();

            monoBehaviours
                .ToList()
                .ForEach(mb =>
                {
                    mb.GetType().GetProperties()
                        .Where(prop => !prop.IsDefined(typeof(ObsoleteAttribute), true)) // filter obsolete/deprecated properties
                        .ToList()
                        .ForEach(prop => newDict.Add(prop, mb));
                });

            return newDict;
        }

        /// <summary>
        /// Update the PropertyInfo-MonoBehaviour dictionary
        /// </summary>
        /// <param name="evt">Property change event</param>
        private void UpdateMonoBehaviourProperties(SerializedPropertyChangeEvent evt)
        {
            monobehaviourProperties.Clear();
            monobehaviourProperties = GetMonobehaviourProperties(evt.changedProperty.objectReferenceValue);
        }

        /// <summary>
        /// Update the property choices available to the user
        /// </summary>
        /// <param name="propertySelector">The PopupField to choose a Property from the selected object</param>
        private void UpdatePropertySelectorChoices(ref PopupField<PropertyInfo> propertySelector)
        {
            // show a popup of all Properties available on the selected Object
            List<PropertyInfo> popupChoices = new(
                monobehaviourProperties
                    .Select(entry => entry.Key)
                    );

            var selectorEmpty = propertySelector.choices.Except(popupChoices).Any();
            var choicesEmpty = popupChoices.Except(propertySelector.choices).Any();

            if (selectorEmpty && choicesEmpty)
            {
                propertySelector.value = null;
            }

            propertySelector.choices = popupChoices;
        }

        /// <summary>
        /// Restores the property selection to the property that was selected previously
        /// </summary>
        /// <param name="propertySelection">PopupField to select property</param>
        /// <param name="property">The SerializedProperty passed in through CreatePropertyGUI</param>
        private void RestorePropertySelection(ref PopupField<PropertyInfo> propertySelection, SerializedProperty property)
        {
            MonoBehaviour selectedMonoBehaviour = property.FindPropertyRelative("SourceScript").GetUnderlyingValue() as MonoBehaviour;
            string selectedPropertyName = property.FindPropertyRelative("PropertyName").GetUnderlyingValue() as string;

            if (selectedMonoBehaviour == null || string.IsNullOrEmpty(selectedPropertyName))
            {
                propertySelection.value = null;
                return;
            }

            propertySelection.value = selectedMonoBehaviour.GetType().GetProperty(selectedPropertyName);
        }

        /// <summary>
        /// Updates the properties affected by the property PopupField
        /// </summary>
        /// <param name="property">The SerializedProperty passed in by CreateInspectorGUI()</param>
        private void UpdateAffectedProperties(ChangeEvent<PropertyInfo> changeEvent, SerializedProperty property)
        {
            if (changeEvent.newValue == null)
            {
                property.FindPropertyRelative("PropertyName").stringValue = "";
                var source = property.FindPropertyRelative("Source").objectReferenceValue;
                property.FindPropertyRelative("SourceScript").SetUnderlyingValue(null);
            }
            else
            {
                property.FindPropertyRelative("PropertyName").stringValue = changeEvent.newValue.Name;
                var source = property.FindPropertyRelative("Source").objectReferenceValue;
                MonoBehaviour sourceScript;

                if (source is MonoBehaviour)
                {
                    sourceScript = source as MonoBehaviour;

                }
                else if (source is GameObject)
                {
                    sourceScript = source.GetComponent(changeEvent.newValue.DeclaringType) as MonoBehaviour;
                }
                else
                {
                    Debug.LogWarning($"Unknown type {source.GetType()}");
                    return;
                }
                property.FindPropertyRelative("SourceScript").SetUnderlyingValue(sourceScript);
            }
        }
    }
}
