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
        private Type[] _allChildTypes = null;
        private string[] _childClasNames = null;
        private GUIContent[] _displayedOptions = null;
        private GUIContent _label = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ChildReferenceDropdown dropdown = attribute as ChildReferenceDropdown;

            var dropdownRect = position;
            dropdownRect.height *= 0.5f;

            if (_allChildTypes == null)
            {
                _allChildTypes = GetAllChildTypes(dropdown.BaseType);
                _childClasNames = GetClassNames(_allChildTypes);
                _displayedOptions = GetContentsForClassNames(_childClasNames);
                _label = new GUIContent("Child Class");
            }

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
                EditorGUI.indentLevel++;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.managedReferenceValue == null) return EditorGUIUtility.singleLineHeight;

            return EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.singleLineHeight;
        }

        public Type[] GetAllChildTypes(Type baseType)
        {
            return Assembly.GetAssembly(baseType)
                            .GetTypes()
                            .Where(t => t.IsSubclassOf(baseType))
                            .ToArray();
        }

        public string[] GetClassNames(Type[] types)
        {
            string[] classNames = new string[types.Length + 1];
            for (int i = 1; i <= types.Length; i++)
            {
                classNames[i] = types[i - 1].Name;
            }
            classNames[0] = "None";
            return classNames;
        }

        public int GetSelectedReferenceIndex(string[] content, object selected)
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

        public static GUIContent[] GetContentsForClassNames(string[] names)
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