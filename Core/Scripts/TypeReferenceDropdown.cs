using System;
using UnityEngine;

namespace SeroJob.FancyAttributes
{
    public class TypeReferenceDropdown : PropertyAttribute
    {
        public Type BaseType;

        public TypeReferenceDropdown(Type baseType)
        {
            BaseType = baseType;
        }
    }
}