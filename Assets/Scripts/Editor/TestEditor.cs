using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Flamenccio.Test
{
    [CustomEditor(typeof(TestPolygonCreator))]
    public class TestEditor : Editor
    {
        private TestPolygonCreator testPolygonCreator;
        private VisualElement squareElements;
        private VisualElement rectangleElements;
        private VisualElement polygonElements;
        private EnumField enumField;
        private SerializedProperty enumProperty;
        public VisualTreeAsset VisualTree;

        public void OnEnable()
        {
            testPolygonCreator = (TestPolygonCreator)target;
            enumProperty = serializedObject.FindProperty("Shape");
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement rootElement = new();
            VisualTree.CloneTree(rootElement);
            squareElements = rootElement.Q<VisualElement>("SquareElements");
            rectangleElements = rootElement.Q<VisualElement>("RectangleElements");
            polygonElements = rootElement.Q<VisualElement>("PolygonElements");
            enumField = rootElement.Q<EnumField>("Shape");

            var x = enumProperty.enumValueIndex;
            UpdateShapeDisplays((TestPolygonCreator.Shapes)x);

            enumField.RegisterValueChangedCallback((evt) =>
            {
                var newValue = evt.newValue;
                UpdateShapeDisplays((TestPolygonCreator.Shapes)newValue);
            });

            return rootElement;
        }

        private void UpdateShapeDisplays(TestPolygonCreator.Shapes shape)
        {
            squareElements.style.display = DisplayStyle.None;
            rectangleElements.style.display = DisplayStyle.None;
            polygonElements.style.display = DisplayStyle.None;

            switch (shape)
            {
                case TestPolygonCreator.Shapes.Square:
                    squareElements.style.display = DisplayStyle.Flex;
                    break;

                case TestPolygonCreator.Shapes.Rectangle:
                    rectangleElements.style.display = DisplayStyle.Flex;
                    break;

                case TestPolygonCreator.Shapes.Polygon:
                    polygonElements.style.display = DisplayStyle.Flex;
                    break;
            }
        }
    }

    [CustomPropertyDrawer(typeof(TestPolygonCreator.Polygon))]
    public class TestEditorPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new();

            PropertyField shapeField = new(property.FindPropertyRelative("Shape"));
            PropertyField rectangleSizeField = new(property.FindPropertyRelative("RectangleSize"));
            PropertyField squareSizeField = new(property.FindPropertyRelative("SquareSize"));
            PropertyField centerField = new(property.FindPropertyRelative("Center"));
            PropertyField originField = new(property.FindPropertyRelative("Origin"));
            PropertyField verticesList = new(property.FindPropertyRelative("Vertices"));

            SerializedProperty shapeEnum = property.FindPropertyRelative("Shape");
            TestPolygonCreator.Shapes shape = (TestPolygonCreator.Shapes)shapeEnum.enumValueIndex;

            container.Add(shapeField);
            container.Add(rectangleSizeField);
            container.Add(squareSizeField);
            container.Add(centerField);
            container.Add(originField);
            container.Add(verticesList);

            HideAll(container.Children());
            shapeField.style.display = DisplayStyle.Flex;

            Action<TestPolygonCreator.Shapes> UpdateShapeDisplays = (s) =>
            {
                HideAll(container.Children());
                shapeField.style.display = DisplayStyle.Flex;

                switch (s)
                {
                    case TestPolygonCreator.Shapes.Square:
                        centerField.style.display = DisplayStyle.Flex;
                        squareSizeField.style.display = DisplayStyle.Flex;
                        break;

                    case TestPolygonCreator.Shapes.Rectangle:
                        centerField.style.display = DisplayStyle.Flex;
                        rectangleSizeField.style.display = DisplayStyle.Flex;
                        break;

                    case TestPolygonCreator.Shapes.Polygon:
                        originField.style.display = DisplayStyle.Flex;
                        verticesList.style.display = DisplayStyle.Flex;
                        break;
                }
            };

            UpdateShapeDisplays(shape);

            shapeField.RegisterValueChangeCallback((evt) =>
            {
                UpdateShapeDisplays((TestPolygonCreator.Shapes)evt.changedProperty.enumValueIndex);
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