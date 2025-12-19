using System.Collections.Generic;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    [CreateAssetMenu(menuName = "UI/GridView/EleData Handler", fileName = "EDH_New", order = 2)]
    public class EleDataHandlerSO : ScriptableObject
    {
        public List<ElementDataSO> AllElements;
        public List<GridDataSO> AllGridData;
        public Dictionary<ElementType, ElementData> AllElementDict;

        // private void ShowDict()
        // {
        //     AllElements = new();

        //     foreach (var item in AllElementDict)
        //     {
        //         var kvp = new KeyValuePair<ElementType, ElementData>(item.Key, item.Value);
        //         AllElements.Add(kvp);
        //     }
        // }

        public void AddToDict(ElementType elementType , ElementData data)
        {

            AllElementDict ??= new();

            if (AllElementDict.ContainsKey(elementType))
            {
                AllElementDict[elementType] = data;
            }

            else
            {
                AllElementDict.Add(elementType, data);
            }

            Debug.Log(AllElementDict[elementType]);

            //ShowDict();
        }
    }
}
