using ignt.sports.cricket.UI;
using UnityEngine;

namespace ignt.sports.cricket.core
{
    [CreateAssetMenu]
    public class FileHandlerSO : FileHandlerSOBase<ColorData>
    {
        public ColorData Current;

        [ContextMenu("Save Data")]
        public void SaveGame()
        {
            SaveAuthData(Current);
            Current = null;
        }

        [ContextMenu("Load Data")]
        public void LoadGame()
        {
            var loadData = LoadAuthData();
            Current = loadData;
        }

        protected override ColorData LoadAuthData()
        {
            return base.LoadAuthData();
        }

        protected override bool SaveAuthData(ColorData data)
        {
            return base.SaveAuthData(data);
        }
    }
}
