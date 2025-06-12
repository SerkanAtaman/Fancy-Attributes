using System;

namespace SeroJob.FancyAttributes
{
    [System.Serializable]
    public class TypeReference
    {
        public string TypeFullName;

        public Type Type
        {
            get
            {
                try
                {
                    _type ??= Type.GetType(TypeFullName);
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                    _type = null;
                }
                return _type;
            }
        }
        private Type _type;

        public TypeReference()
        {
            TypeFullName = string.Empty;
        }

        public TypeReference(string fullTypeName)
        {
            TypeFullName = fullTypeName;
            _type = null;
        }

        public void Clear()
        {
            TypeFullName = string.Empty;
            _type = null;
        }
    }
}