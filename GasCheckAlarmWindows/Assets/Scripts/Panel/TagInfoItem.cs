using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagInfoItem : MonoBehaviour
{
    public Text tagNameText;
    public Text probeCountText;

    public TreeMap treeMap;
    public void InitData(List<RealtimeDataModel> realtimeDataModelList, TreeMap treeMap)
    {
        this.treeMap = treeMap;
        tagNameText.text = treeMap.tagName;
        probeCountText.text = "（" + realtimeDataModelList.Count.ToString() + "）个";
        int warningLevel = -1;
        for (int i = 0; i < realtimeDataModelList.Count; i++)
        {
            RealtimeDataModel model = realtimeDataModelList[i];
            if (model.warningLevel > warningLevel)
            {
                warningLevel = model.warningLevel;
            }
        }
        Color color = FormatData.warningColorDic[warningLevel];
        ColorBlock cb = GetComponent<Button>().colors;
        cb.normalColor = color;
        cb.pressedColor = color;
        cb.highlightedColor = color;
        GetComponent<Button>().colors = cb;
    }
}
