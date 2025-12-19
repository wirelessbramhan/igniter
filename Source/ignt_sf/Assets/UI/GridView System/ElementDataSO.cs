using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    [CreateAssetMenu(menuName = "UI/GridView/Element Data", fileName = "EleData_New", order = 1)]
    public class ElementDataSO : ScriptableObject
    {
        public string PageName;
        public List<ElementData> ElementDatas;
        public Sprite DefaultBG;
        public TMP_FontAsset FontAssetGlobal;
        private Dictionary<ElementType, ElementData> DataDict;
        public bool isLoaded;
        public event Action OnConfigRaised;

        private void SaveToDict()
        {
            DataDict = new();

            for (int i = 0; i < ElementDatas.Count; i++)
            {
                ElementData data = ElementDatas[i];
                data.SetName(PageName);
                
                if (DataDict.ContainsKey(data.Type))
                {
                    DataDict[data.Type] = data;
                }

                else
                {
                    DataDict.Add(data.Type, data);
                }
            }

            isLoaded = DataDict.Count > 0;
        }

        void OnEnable()
        {
            OnConfigRaised += SaveToDict;
        }

        void OnDisable()
        {
            OnConfigRaised -= SaveToDict;
        }

        public void RaiseConfig()
        {
            OnConfigRaised?.Invoke();
        }

        public ElementData GetElementData(ElementType elementType)
        {
            ElementData data = null;

            if (isLoaded && DataDict.ContainsKey(elementType))
            {
                data = DataDict[elementType];
            }

            else
            {
                foreach (var item in ElementDatas)
                {
                    if (item.Type == elementType)
                    {
                        data = item;
                    }
                }
            }

            return data;
        }

        public void SetElementData(ElementData elementData)
        {
            if (DataDict.Count > 0 && DataDict.ContainsKey(elementData.Type))
            {
                DataDict[elementData.Type] = elementData;
            }

            else
            {
                DataDict.Add(elementData.Type, elementData);
            }
        }

        public void ShowDict()
        {
            if (DataDict.Count > 0)
            {
                ElementDatas.Clear();
                
                foreach (var eleData in DataDict)
                {
                    ElementDatas.Add(eleData.Value);
                }
            }
        }
    }
}
