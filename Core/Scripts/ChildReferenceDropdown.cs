using System;
using UnityEngine;

namespace SeroJob.FancyAttributes
{
    public class ChildReferenceDropdown : PropertyAttribute, IDisposable
    {
        public Type BaseType;

        public ChildReferenceDropdown(Type type)
        {
            BaseType = type;
        }

        public void Dispose()
        {
            BaseType = null;
        }
    }
}