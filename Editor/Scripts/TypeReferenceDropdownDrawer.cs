using UnityEngine;
using UnityEditor;
using System;

namespace SeroJob.FancyAttributes.Editor
{
    [CustomPropertyDrawer(typeof(TypeReferenceDropdown))]
    public class TypeReferenceDropdownDrawer : PropertyDrawer, IDisposable
    {
        private static Type[] _allChildTypes = null;
        private static string[] _childClasNames = null;
        private static GUIContent[] _displayedOptions = null;
        private string _errorMessage = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects) return;

            EditorGUI.BeginProperty(position, label, property);

            var typeReferenceAttribute = attribute as TypeReferenceDropdown;

            if (typeReferenceAttribute.BaseType == null)
            {
                _errorMessage = "There is no valid BaseType provided for TypeReferenceDropdown";
                ShowError(position);
                property.managedReferenceValue = null;
                return;
            }

            if (typeReferenceAttribute.BaseType.IsInterface)
            {
                _errorMessage = "Interfaces can not be used with ChildReferenceDropdown attribute";
                ShowError(position);
                property.managedReferenceValue = null;
                return;
            }

            _errorMessage = null;

            if (_allChildTypes == null)
            {
                _allChildTypes = FancyAttributesEditorUtils.GetAllChildTypes(typeReferenceAttribute.BaseType);
                _childClasNames = FancyAttributesEditorUtils.GetFullClassNames(_allChildTypes);
                _displayedOptions = GetContentsForClassNames(_childClasNames);
            }

            var dropdownRect = position;
            dropdownRect.height = EditorGUIUtility.singleLineHeight;

            var currentSelected = GetSelectedReferenceIndex(_childClasNames, property.managedReferenceValue);
            var targetIndex = EditorGUI.Popup(dropdownRect, new GUIContent("Target Type"), currentSelected, _displayedOptions);
            
            if (targetIndex <= 0)
            {
                property.managedReferenceValue ??= new TypeReference();
                ((TypeReference)property.managedReferenceValue).Clear();
            }
            else if (targetIndex != currentSelected)
            {
                property.managedReferenceValue = new TypeReference(_allChildTypes[targetIndex - 1].FullName);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects) return 0f;

            return EditorGUIUtility.singleLineHeight;
        }

        void ShowError(Rect propertyPosition)
        {
            EditorGUI.HelpBox(propertyPosition, _errorMessage, MessageType.Error);
        }

        int GetSelectedReferenceIndex(string[] content, object managedReferenceValue)
        {
            if (managedReferenceValue == null) return 0;
            if (content == null) return 0;

            var reference = (TypeReference)managedReferenceValue;

            if (string.IsNullOrWhiteSpace(reference.TypeFullName)) return 0;

            var selectedTypeName = reference.TypeFullName;

            for (int i = 0; i < content.Length; i++)
            {
                if (string.Equals(selectedTypeName, content[i]))
                {
                    return i;
                }
            }

            return 0;
        }

        static string GetDisplayNameFromFullName(string fullName)
        {
            try
            {
                var parts = fullName.Split(".");
                return parts[^1];
            }
            catch
            {
                return fullName;
            }
        }

        static GUIContent[] GetContentsForClassNames(string[] names)
        {
            var result = new GUIContent[names.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new GUIContent()
                {
                    text = GetDisplayNameFromFullName(names[i])
                };
            }

            return result;
        }

        public void Dispose()
        {
            _allChildTypes = null;
            _childClasNames = null;
            _displayedOptions = null;
        }
    }
}