using Flamenccio.DataHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Events;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Flamenccio.Localization
{
    public interface IDescribable
    {
        LocalizedString CompleteDescription(LocalizedString description);
    }

    /// <summary>
    /// Holds a localized name and description for the GameObject that this class is attached to.
    /// </summary>
    public class ObjectDescription : MonoBehaviour
    {
        [SerializeField, Tooltip("This is necessary to complete descriptions.")] private PropertyList propertyList; // this is needed to grab the needed properties to fill descriptions
        [SerializeField] private LocalizedString objectName;
        [SerializeField] private LocalizedString objectDescription;

        private void Awake()
        {
            FindDataSource();
        }

        public void FillDescription()
        {
            List<ObjectProperty> listCopy = new(propertyList.Properties); // copied to prevent altering the original list
            List<string> localVariables = new(objectDescription.Keys);

            // check if the propertyList accounts for all local variables
            List<string> listCopyNames = listCopy.Select(x => x.VariableName).ToList();
            var areListsEqualSize = listCopy.Count == localVariables.Count;
            var differenceLocalVariables = localVariables.Except(listCopyNames).Any();
            var differenceListCopy = listCopyNames.Except(localVariables).Any();

            if (!areListsEqualSize || differenceLocalVariables || differenceListCopy)
            {
                Debug.LogError($"Failed to fill all local variables! ({objectName.GetLocalizedString()})");
                return;
            }

            listCopy
                .ForEach(localVariable =>
                {
                    var currentVariable = objectDescription[localVariable.VariableName];
                    var currentVariableType = currentVariable.GetType();
                    var localVariableValue = localVariable.GetPropertyValue();

                    if (currentVariableType == typeof(IntVariable))
                    {
                        (currentVariable as IntVariable).Value = (int)localVariableValue;
                        return;
                    }

                    if (currentVariableType == typeof(StringVariable))
                    {
                        (currentVariable as StringVariable).Value = (string)localVariableValue;
                        return;
                    }

                    if (currentVariableType == typeof(FloatVariable))
                    {
                        (currentVariable as FloatVariable).Value = (float)localVariableValue;
                        return;
                    }

                    Debug.LogError($"Unsupported variable type: {currentVariableType}");
                });
        }

        public LocalizedString GetObjectName()
        {
            return objectName;
        }

        public LocalizedString GetObjectDescription()
        {
            FillDescription();

            return objectDescription;
        }

        /// <summary>
        /// Checks if the propertyList is not null.
        /// </summary>
        /// <exception cref="Exception">There is no attached property list</exception>
        private void FindDataSource()
        {
            if (propertyList == null)
            {
                Debug.LogError("There is no attached propertyList!");
                throw new Exception("No attached PropertyList");
            }
        }
    }
}
