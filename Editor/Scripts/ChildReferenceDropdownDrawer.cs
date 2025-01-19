using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;

namespace SeroJob.FancyAttributes.Editor
{
    [CustomPropertyDrawer(typeof(ChildReferenceDropdown))]
    public class ChildReferenceDropdownDrawer : PropertyDrawer, IDisposable
    {
        private Type _type = null;
        private Type[] _allChildTypes = null;
        private string[] _childClasNames = null;
        private GUIContent[] _displayedOptions = null;
        private GUIContent _label = null;
        private string _errorMessage = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects) return;

            _type ??= GetSerializedReferenceType(property);

            if (_type == null)
            {
                property.managedReferenceValue = null;
                return;
            }

            if (_type.IsInterface)
            {
                _errorMessage = "Interfaces can not be used with ChildReferenceDropdown attribute";
                ShowError(position);
                property.managedReferenceValue = null;
                return;
            }

            _errorMessage = null;

            if (_allChildTypes == null)
            {
                _allChildTypes = GetAllChildTypes(_type);
                _childClasNames = GetClassNames(_allChildTypes);
                _displayedOptions = GetContentsForClassNames(_childClasNames);
                _label = new GUIContent("Child Class");
            }

            var dropdownRect = position;
            dropdownRect.height = EditorGUIUtility.singleLineHeight;

            var currentSelected = GetSelectedReferenceIndex(_childClasNames, property.managedReferenceValue);
            var targetIndex = EditorGUI.Popup(dropdownRect, _label, currentSelected, _displayedOptions);

            if (targetIndex == 0)
            {
                property.managedReferenceValue = null;
            }
            else if (targetIndex != currentSelected)
            {
                property.managedReferenceValue = Activator.CreateInstance(_allChildTypes[targetIndex - 1]);
            }

            if (property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                var selectedRect = position;
                selectedRect.height *= 0.5f;
                selectedRect.position += new Vector2(0, EditorGUIUtility.singleLineHeight);

                EditorGUI.BeginProperty(selectedRect, label, property);
                EditorGUI.PropertyField(selectedRect, property, label, true);
                EditorGUI.EndProperty();
                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects) return 0f;
            if (_errorMessage != null) return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
            if (property.managedReferenceValue == null) return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        void ShowError(Rect propertyPosition)
        {
            EditorGUI.HelpBox(propertyPosition, _errorMessage, MessageType.Error);
        }

        Type GetSerializedReferenceType(SerializedProperty property)
        {
            try
            {
                var fieldTypeParts = property.managedReferenceFieldTypename.Split(' ');
                var assemblyName = fieldTypeParts[0];
                var typeName = fieldTypeParts[1];
                var assembly = Assembly.Load(assemblyName);
                return assembly.GetType(typeName);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        Type[] GetAllChildTypes(Type baseType)
        {
            return Assembly.GetAssembly(baseType)
                            .GetTypes()
                            .Where(t => t.IsSubclassOf(baseType))
                            .ToArray();
        }

        string[] GetClassNames(Type[] types)
        {
            string[] classNames = new string[types.Length + 1];
            for (int i = 1; i <= types.Length; i++)
            {
                classNames[i] = types[i - 1].Name;
            }
            classNames[0] = "None";
            return classNames;
        }

        int GetSelectedReferenceIndex(string[] content, object selected)
        {
            if (selected == null) return 0;
            if (content == null) return 0;

            string selectedName = selected.GetType().Name;

            for (int i = 0; i < content.Length; i++)
            {
                if (string.Equals(selectedName, content[i]))
                {
                    return i;
                }
            }

            return 0;
        }

        static GUIContent[] GetContentsForClassNames(string[] names)
        {
            var result = new GUIContent[names.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new GUIContent()
                {
                    text = names[i]
                };
            }

            return result;
        }

        public void Dispose()
        {
            _allChildTypes = null;
            _childClasNames = null;
            _displayedOptions = null;
            _label = null;
        }
    }
}