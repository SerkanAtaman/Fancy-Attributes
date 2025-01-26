using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SeroJob.FancyAttributes.Editor
{
    public static class FancyAttributesEditorUtils
    {
        public static Type GetSerializedReferenceType(SerializedProperty property)
        {
            try
            {
                var fieldTypeParts = property.managedReferenceFieldTypename.Split(' ');
                var assemblyName = fieldTypeParts[0];
                var typeName = fieldTypeParts[1];
                var assembly = Assembly.Load(assemblyName);
                return assembly.GetType(typeName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public static Type[] GetAllChildTypes(Type baseType)
        {
            return Assembly.GetAssembly(baseType)
                            .GetTypes()
                            .Where(t => t.IsSubclassOf(baseType))
                            .ToArray();
        }

        public static string[] GetClassNames(Type[] types, bool sortAlphabetically = true)
        {
            List<string> result = new();

            for (int i = 0; i < types.Length; i++)
            {
                result.Add(types[i].Name);
            }

            if (sortAlphabetically) result.Sort(StringComparer.OrdinalIgnoreCase);
            result.Insert(0, "None");

            return result.ToArray();
        }
    }
}