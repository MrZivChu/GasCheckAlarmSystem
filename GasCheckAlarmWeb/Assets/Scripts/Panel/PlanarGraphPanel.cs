using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanarGraphPanel : UIEventHelper
{
    public GameObject itemRes;
    public Transform contentTrans;
    public SelectProbeForGraphPanel selectProbeForGraphPanel;
    public EditProbeForGraphPanel editProbeForGraphPanel;

    private void Start()
    {
        LoadServerGraphImg();
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
        EventManager.Instance.AddEventListener(NotifyType.UpdatePos2D, UpdatePos2D);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
        EventManager.Instance.DeleteEventListener(NotifyType.UpdatePos2D, UpdatePos2D);
    }

    void UpdatePos2D(object data)
    {
        InitData();
    }

    void UpdateRealtimeData(object data)
    {
        if (!gameObject || !gameObject.activeSelf)
        {
            return;
        }
        List<ProbeModel> realtimeEventData = (List<ProbeModel>)data;
        realtimeEventData.ForEach(it =>
        {
            if (dic.ContainsKey(it.ID))
            {
                dic[it.ID].InitRealtimeData(it);
            }
        });
    }

    float doublePreTime = 0;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                float doubleNowTime = Time.realtimeSinceStartup;
                if (doubleNowTime - doublePreTime < 0.3f && FormatData.currentUser.Authority == EAuthority.Admin)
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(graphPanelRT, Input.mousePosition, Camera.main, out uiPosition);
        print(uiPosition.x + "," + uiPosition.y);
        if (Mathf.Abs(uiPosition.x) > halfWidth || Mathf.Abs(uiPosition.y) > halfHeight)
        {
            print("click out range");
            return;
        }
        selectProbeForGraphPanel.uiPos = uiPosition;
        selectProbeForGraphPanel.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        InitData();
    }

    void InitData()
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectIDProbeNameMachineIDPos2DWherePos2DHasValue");
        GameUtils.PostHttpWebRequest("Probe.ashx", form, (bytes) =>
        {
            string content = Encoding.UTF8.GetString(bytes);
            List<ProbeModel> list = JsonMapper.ToObject<List<ProbeModel>>(content);
            InitGrid(list);
        }, null);
    }

    Dictionary<int, PlanarGraphItem> dic = new Dictionary<int, PlanarGraphItem>();
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
            }
            string[] pos = data.Pos2D.Split(new string[] { "," }, System.StringSplitOptions.None);
            Vector3 position = new Vector3(Convert.ToSingle(pos[0]), Convert.ToSingle(pos[1]), 0);
            currentObj.GetComponent<RectTransform>().anchoredPosition3D = position;
            PlanarGraphItem planarGraphItem = currentObj.GetComponent<PlanarGraphItem>();
            planarGraphItem.InitData(data);
            RegisterBtnClick<PlanarGraphItem>(currentObj.GetComponent<Button>(), planarGraphItem, OnButtonClick);
            currentObj.SetActive(true);
            if (!dic.ContainsKey(data.ID))
            {
                dic[data.ID] = planarGraphItem;
            }
        }, false);
    }

    void OnButtonClick(Button btn, PlanarGraphItem data)
    {
        editProbeForGraphPanel.InitInfo(data.currentModel);
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
