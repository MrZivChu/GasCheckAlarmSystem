using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceTagItemInfo : MonoBehaviour
{
    public TreeMap model;
    public Text nameText;
    public void InitData(TreeMap treeMap)
    {
        model = treeMap;
        nameText.text = treeMap.tagName;
    }
}
