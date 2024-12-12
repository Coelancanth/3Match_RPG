using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ElementVisualConfig", menuName = "Game/Element Visual Config")]
public class ElementVisualConfig : ScriptableObject
{
    [Serializable]
    public class ElementVisualData
    {
        public string elementType;
        public Sprite sprite;
        public Color color = Color.white;
    }

    public ElementVisualData[] elementVisuals;

    private Dictionary<string, ElementVisualData> visualDataMap;

    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        visualDataMap = new Dictionary<string, ElementVisualData>();
        foreach (var visualData in elementVisuals)
        {
            visualDataMap[visualData.elementType] = visualData;
        }
    }

    public ElementVisualData GetVisualData(string elementType)
    {
        if (visualDataMap == null)
        {
            InitializeDictionary();
        }

        return visualDataMap.TryGetValue(elementType, out var data) ? data : null;
    }
}