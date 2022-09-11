using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanarGraphPanel : UIEventHelper
{
    public GameObject itemRes;
    public Camera uiCamera;
    public Transform contentTrans;
    public SelectProbeForGraphPanel selectProbeForGraphPanel;
    public EditProbeForGraphPanel editProbeForGraphPanel;

    private void Start()
    {
        LoadServerGraphImg();
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
    }

    void UpdateRealtimeData(object data)
    {
        if (!gameObject || !gameObject.activeSelf)
        {
            return;
        }
        RealtimeEventData realtimeEventData = (RealtimeEventData)data;
        InitData(realtimeEventData);
    }

    float doublePreTime = 0;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                float doubleNowTime = Time.realtimeSinceStartup;
                if (doubleNowTime - doublePreTime < 0.3f && FormatData.currentUser.Authority == 1)
                {
                    InsertProbe();
                }
                doublePreTime = doubleNowTime;
            }
        }
    }

    void InsertProbe()
    {
        RectTransform graphPanelRT = contentTrans.GetComponent<RectTransform>();
        float halfWidth = graphPanelRT.rect.size.x / 2;
        float halfHeight = graphPanelRT.rect.size.y / 2;
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(graphPanelRT, Input.mousePosition, uiCamera, out uiPosition);
        print(uiPosition.x + "," + uiPosition.y);
        if (Mathf.Abs(uiPosition.x) > halfWidth || Mathf.Abs(uiPosition.y) > halfHeight)
        {
            print("click out range");
            return;
        }
        selectProbeForGraphPanel.uiPos = uiPosition;
        selectProbeForGraphPanel.gameObject.SetActive(true);
    }

    private void InitData(RealtimeEventData realtimeEventData)
    {
        List<RealtimeDataModel> list = new List<RealtimeDataModel>();
        list.AddRange(realtimeEventData.firstList.FindAll((item) =>
        {
            return !string.IsNullOrEmpty(item.Pos2D) ? true : false;
        }));
        list.AddRange(realtimeEventData.noResponseList.FindAll((item) =>
        {
            return !string.IsNullOrEmpty(item.Pos2D) ? true : false;
        }));
        list.AddRange(realtimeEventData.normalList.FindAll((item) =>
        {
            return !string.IsNullOrEmpty(item.Pos2D) ? true : false;
        }));
        list.AddRange(realtimeEventData.secondList.FindAll((item) =>
        {
            return !string.IsNullOrEmpty(item.Pos2D) ? true : false;
        }));
        InitGrid(list);
    }

    void InitGrid(List<RealtimeDataModel> list)
    {
        GameUtils.SpawnCellForTable<RealtimeDataModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
            }
            string[] pos = data.Pos2D.Split(new string[] { "," }, System.StringSplitOptions.None);
            Vector3 position = new Vector3(Convert.ToSingle(pos[0]), Convert.ToSingle(pos[1]), 0);
            currentObj.GetComponent<RectTransform>().anchoredPosition3D = position;
            PlanarGraphItem planarGraphItem = currentObj.GetComponent<PlanarGraphItem>();
            planarGraphItem.InitData(data);
            RegisterBtnClick<PlanarGraphItem>(currentObj.GetComponent<Button>(), planarGraphItem, OnButtonClick);
            currentObj.SetActive(true);
        }, false);
    }

    void OnButtonClick(Button btn, PlanarGraphItem data)
    {
        editProbeForGraphPanel.InitInfo(data.realtimeDataModel);
        editProbeForGraphPanel.gameObject.SetActive(true);
    }

    void LoadServerGraphImg()
    {
        GameUtils.GetHttpWebRequest("/PlanarGraph/PlanarGraph.jpg", (data) =>
        {
            Texture2D texture = new Texture2D(160, 120, TextureFormat.RGBA32, false);
            texture.LoadImage(data);
            texture.Apply();
            Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
            contentTrans.GetComponent<Image>().sprite = sp;
        }, null);
    }
}
