using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CubeInfoPanel : UIEventHelper
{
    public GameObject itemRes;
    public GridLayoutGroup contentTrans;
    public InputField nameInput;
    public GameObject editorCubeStylePanel;

    int numberSize = 25;
    int hanziSize = 25;

    private void Start()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
        RegisterInputFieldOnEndEdit(nameInput, OnNameInputEnd);
        EventTriggerListener.Get(nameInput.gameObject).onEndDrag += (go, data) => { editorCubeStylePanel.SetActive(true); };
        nameInput.text = GameUtils.GetString("nameInput", "");

        hanziSize = GameUtils.GetInt("cubeHanziInput", 25);
        numberSize = GameUtils.GetInt("cubeNumInput", 25);

        contentTrans.constraintCount = GameUtils.GetInt("cubeColInput", 8);
        contentTrans.cellSize = new Vector2(GameUtils.GetInt("cubeWidthInput", 203), GameUtils.GetInt("cubeHeightInput", 215));
        contentTrans.spacing = new Vector2(GameUtils.GetInt("cubeHorInput", 10), GameUtils.GetInt("cubeVerInput", 10));
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
    }

    void OnNameInputEnd(InputField input, string content)
    {
        GameUtils.SetString("nameInput", input.text);
    }

    void UpdateRealtimeData(object data)
    {
        if (!gameObject || !gameObject.activeSelf)
        {
            return;
        }
        List<ProbeModel> list = (List<ProbeModel>)data;
        InitGrid(list);
    }

    void InitGrid(List<ProbeModel> list)
    {
        GameUtils.SpawnCellForTable<ProbeModel>(contentTrans.transform, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans.transform);
                currentObj.transform.localScale = Vector3.one;
                currentObj.transform.GetChild(0).GetComponent<Text>().fontSize = hanziSize;
                currentObj.transform.GetChild(1).GetComponent<Text>().fontSize = hanziSize;
                currentObj.transform.GetChild(2).GetComponent<Text>().fontSize = numberSize;
                currentObj.transform.GetChild(3).GetComponent<Text>().fontSize = numberSize;
                currentObj.transform.GetChild(4).GetComponent<Text>().fontSize = numberSize;
            }
            Vector3 position = currentObj.GetComponent<RectTransform>().anchoredPosition3D;
            position.z = 0;
            currentObj.GetComponent<RectTransform>().anchoredPosition3D = position;
            CubeInfoItem cubeInfoItem = currentObj.GetComponent<CubeInfoItem>();
            cubeInfoItem.InitData(data);
            currentObj.SetActive(true);
        }, false);
    }
}
