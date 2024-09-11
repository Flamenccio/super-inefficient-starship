using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flamenccio.FlamenccioEditor
{
    [CustomEditor(typeof(DataHandling.PropertyList))]
    public class PropertyListEditor : Editor
    {
        [SerializeField] private VisualTreeAsset root;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new();
            root.CloneTree(container);

            return container;
        }
    }
}