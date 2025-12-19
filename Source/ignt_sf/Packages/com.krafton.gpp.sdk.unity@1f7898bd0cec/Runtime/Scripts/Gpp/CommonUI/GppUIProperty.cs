namespace Gpp.CommonUI
{
    internal class GppUIProperty
    {
        public string PrefabPath { get; set; }
        public bool AllowMultipleInstance { get; set; }

        public GppUIProperty(string prefabPath, bool allowMultipleInstance = false)
        {
            PrefabPath = prefabPath;
            AllowMultipleInstance = allowMultipleInstance;
        }
    }
}