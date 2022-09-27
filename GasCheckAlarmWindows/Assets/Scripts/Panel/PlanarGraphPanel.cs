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
    public Transform contentTrans;
    public SelectProbeForGraphPanel selectProbeForGraphPanel;
    public EditProbeForGraphPanel editProbeForGraphPanel;
    public Button btn_upload;

    string localGraphImgPath = string.Empty;
    private void Start()
    {
        localGraphImgPath = Application.persistentDataPath + "/graphImg.png";
        if (JsonHandleHelper.isRemoteServer)
        {
            LoadServerGraphImg();
        }
        else
        {
            StartCoroutine(GetLocalTexture(localGraphImgPath));
        }
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
        RegisterBtnClick(btn_upload, OnUploadImg);
        btn_upload.gameObject.SetActive(FormatData.currentUser.Authority == 1);
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

    void OnUploadImg(Button btn)
    {
        using (System.Windows.Forms.OpenFileDialog od = new System.Windows.Forms.OpenFileDialog())
        {
            od.Title = "请选择图片";
            od.Multiselect = false;
            od.Filter = "图片文件(*.jpg,*.png,*.bmp)|*.jpg;*.png;*.bmp";
            if (od.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (JsonHandleHelper.isRemoteServer)
                {
                    SaveFileToServer(od.FileName);
                }
                else
                {
                    SaveFileToLocal(od.FileName, localGraphImgPath);
                }
            }
        }
    }

    void SaveFileToServer(string selectPath)
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "UploadPlanarGraphFile");
        form.AddBinaryData("file", File.ReadAllBytes(selectPath), "PlanarGraph.jpg", "image/jpeg");
        GameUtils.PostHttpWebRequest("Probe.ashx", form, (content) =>
        {
            LoadServerGraphImg();
        }, (error) =>
        {
            MessageBox.Instance.PopOK(error, null, "确定");
        });
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

    void SaveFileToLocal(string selectPath, string savePath)
    {
        using (FileStream fs = new FileStream(selectPath, FileMode.Open))
        {
            using (FileStream fs2 = new FileStream(savePath, FileMode.Create))
            {
                byte[] b = new byte[1024 * 1024];
                int getv = 0;
                while ((getv = fs.Read(b, 0, b.Length)) > 0)
                {
                    fs2.Write(b, 0, getv);
                }
            }
        }
        StartCoroutine(GetLocalTexture(localGraphImgPath));
    }

    IEnumerator GetLocalTexture(string url)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (www.isDone && www.error == null)
            {
                Texture2D texture2D = www.texture;
                Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                contentTrans.GetComponent<Image>().sprite = sprite;
            }
        }
    }
}
