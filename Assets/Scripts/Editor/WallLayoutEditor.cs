using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using Flamenccio.LevelObject.Stages;
using System;
using System.Collections.Generic;

namespace Flamenccio.FlamenccioEditor // this is a really stupid name
{
    [CustomEditor(typeof(WallLayout))]
    public class WallLayoutEditor : Editor
    {
        [SerializeField] private VisualTreeAsset visualTree;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();
            visualTree.CloneTree(root);

            return root;
        }
    }
    [CustomPropertyDrawer(typeof(WallLayout.WallAttributes))]
    public class WallAttributesPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new();

            PropertyField shapeField = new(property.FindPropertyRelative("Shape"));
            PropertyField centerField = new(property.FindPropertyRelative("Center"));
            PropertyField originField = new(property.FindPropertyRelative("Origin"));
            PropertyField verticesList = new(property.FindPropertyRelative("Vertices"));
            PropertyField sideLengthsField = new(property.FindPropertyRelative("SideLengths"));

            SerializedProperty shapeEnum = property.FindPropertyRelative("Shape");
            WallLayout.WallShape shape = (WallLayout.WallShape)shapeEnum.enumValueIndex;

            container.Add(shapeField);
            container.Add(centerField);
            container.Add(originField);
            container.Add(verticesList);
            container.Add(sideLengthsField);

            HideAll(container.Children());
            shapeField.style.display = DisplayStyle.Flex;

            Debug.Log("huH");

            Action<WallLayout.WallShape> UpdateShapeDisplays = (s) =>
            {
                HideAll(container.Children());
                shapeField.style.display = DisplayStyle.Flex;

                if (shapeEnum.enumValueIndex == (int)WallLayout.WallShape.Rectangle)
                {
                    centerField.style.display = DisplayStyle.Flex;
                    sideLengthsField.style.display = DisplayStyle.Flex;
                }
                else if (shapeEnum.enumValueIndex == (int)WallLayout.WallShape.Polygon)
                {
                    originField.style.display = DisplayStyle.Flex;
                    verticesList.style.display = DisplayStyle.Flex;
                }
            };

            UpdateShapeDisplays(shape);

            shapeField.RegisterValueChangeCallback((evt) =>
            {
                UpdateShapeDisplays((WallLayout.WallShape)evt.changedProperty.enumValueIndex);
            });

            return container;
        }
        private void HideAll(IEnumerable<VisualElement> elements)
        {
            foreach (VisualElement element in elements)
            {
                element.style.display = DisplayStyle.None;
            }
        }
    }
}
