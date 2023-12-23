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
    public Transform contentTrans;
    public InputField nameInput;

    public InputField colInput;
    public InputField hanziInput;
    public InputField numInput;
    public InputField widthInput;
    public InputField heightInput;
    public InputField horSpaceInput;
    public InputField verSpaceInput;

    public GridLayoutGroup gridLayoutGroup;

    private void Start()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
        RegisterInputFieldOnEndEdit(nameInput, OnNameInputEnd);
        RegisterInputFieldOnEndEdit(colInput, OnColumInputEnd);
        RegisterInputFieldOnEndEdit(hanziInput, OnHanziInputEnd);
        RegisterInputFieldOnEndEdit(numInput, OnNumberInputEnd);
        RegisterInputFieldOnEndEdit(widthInput, OnWidthInputEnd);
        RegisterInputFieldOnEndEdit(heightInput, OnHeightInputEnd);
        RegisterInputFieldOnEndEdit(horSpaceInput, OnHorSpaceInputEnd);
        RegisterInputFieldOnEndEdit(verSpaceInput, OnVerSpaceInputEnd);

        nameInput.text = GameUtils.GetString("nameInput", "");
        colInput.text = GameUtils.GetInt("cubeColInput", 8).ToString();
        hanziInput.text = GameUtils.GetInt("cubeHanziInput", 25).ToString();
        numInput.text = GameUtils.GetInt("cubeNumInput", 25).ToString();
        widthInput.text = GameUtils.GetInt("cubeWidthInput", 203).ToString();
        heightInput.text = GameUtils.GetInt("cubeHeightInput", 215).ToString();
        horSpaceInput.text = GameUtils.GetInt("cubeHorInput", 10).ToString();
        verSpaceInput.text = GameUtils.GetInt("cubeVerInput", 10).ToString();

        //OnColumInputEnd(colInput, colInput.text);
        //OnWidthInputEnd(widthInput, widthInput.text);
        //OnHeightInputEnd(heightInput, heightInput.text);
        //OnHorSpaceInputEnd(horSpaceInput, horSpaceInput.text);
        //OnVerSpaceInputEnd(verSpaceInput, verSpaceInput.text);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
    }

    void OnNameInputEnd(InputField input, string content)
    {
        GameUtils.SetString("nameInput", input.text);
    }

    void OnColumInputEnd(InputField input, string content)
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            int number = Convert.ToInt32(input.text);
            gridLayoutGroup.constraintCount = number;
            GameUtils.SetInt("cubeColInput", number);
        }
    }

    void OnHanziInputEnd(InputField input, string content)
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            int number = Convert.ToInt32(input.text);
            foreach (Transform item in contentTrans)
            {
                item.GetChild(0).GetComponent<Text>().fontSize = number;
                item.GetChild(1).GetComponent<Text>().fontSize = number;
            }
            GameUtils.SetInt("cubeHanziInput", number);
        }
    }

    void OnNumberInputEnd(InputField input, string content)
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            int number = Convert.ToInt32(input.text);
            foreach (Transform item in contentTrans)
            {
                item.GetChild(2).GetComponent<Text>().fontSize = number;
                item.GetChild(3).GetComponent<Text>().fontSize = number;
                item.GetChild(4).GetComponent<Text>().fontSize = number;
            }
            GameUtils.SetInt("cubeNumInput", number);
        }
    }

    void OnWidthInputEnd(InputField input, string content)
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            int number = Convert.ToInt32(input.text);
            Vector2 v2 = gridLayoutGroup.cellSize;
            v2.x = number;
            gridLayoutGroup.cellSize = v2;
            GameUtils.SetInt("cubeWidthInput", number);
        }
    }

    void OnHeightInputEnd(InputField input, string content)
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            int number = Convert.ToInt32(input.text);
            Vector2 v2 = gridLayoutGroup.cellSize;
            v2.y = number;
            gridLayoutGroup.cellSize = v2;
            GameUtils.SetInt("cubeHeightInput", number);
        }
    }

    void OnHorSpaceInputEnd(InputField input, string content)
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            int number = Convert.ToInt32(input.text);
            Vector2 v2 = gridLayoutGroup.spacing;
            v2.x = number;
            gridLayoutGroup.spacing = v2;
            GameUtils.SetInt("cubeHorInput", number);
        }
    }

    void OnVerSpaceInputEnd(InputField input, string content)
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            int number = Convert.ToInt32(input.text);
            Vector2 v2 = gridLayoutGroup.spacing;
            v2.y = number;
            gridLayoutGroup.spacing = v2;
            GameUtils.SetInt("cubeVerInput", number);
        }
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
        GameUtils.SpawnCellForTable<ProbeModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                //if (!string.IsNullOrEmpty(hanziInput.text))
                //{
                //    int number = Convert.ToInt32(hanziInput.text);
                //    currentObj.transform.GetChild(0).GetComponent<Text>().fontSize = number;
                //    currentObj.transform.GetChild(1).GetComponent<Text>().fontSize = number;
                //}
                //if (!string.IsNullOrEmpty(numInput.text))
                //{
                //    int number = Convert.ToInt32(numInput.text);
                //    currentObj.transform.GetChild(2).GetComponent<Text>().fontSize = number;
                //    currentObj.transform.GetChild(3).GetComponent<Text>().fontSize = number;
                //    currentObj.transform.GetChild(4).GetComponent<Text>().fontSize = number;
                //}
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
