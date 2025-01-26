using System;

namespace SeroJob.FancyAttributes
{
    [System.Serializable]
    public class TypeReference
    {
        public Type TargetType;

        public TypeReference()
        {
            TargetType = null;
        }

        public TypeReference(Type targetType)
        {
            TargetType = targetType;
        }
    }
}