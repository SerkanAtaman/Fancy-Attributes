namespace SeroJob.FancyAttributes
{
    [System.Serializable]
    public class TypeReference
    {
        public string TypeFullName;

        public TypeReference()
        {
            Clear();
        }

        public TypeReference(string fullTypeName)
        {
            TypeFullName = fullTypeName;
        }

        public void Clear()
        {
            TypeFullName = string.Empty;
        }
    }
}