using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using UnityEngine.UI;
using System;
using LitJson;
using System.IO;

public class WaterSealPointCheckPanel : MonoBehaviour
{
    public List<Toggle> confirmTogList;
    public Image confirmImg;
    public Button confirmOkBtn;
    public Button confirmCancelBtn;
    public Text confirmProbeNameText;
    public InputField confirmInputField;
    public GameObject confirmPanel;

    public RawImage cameraTexture;
    public Button saomaBtn;
    public Text saomaTipText;

    public Transform contentTrans;
    public UnityEngine.Object itemRes;
    void Start()
    {
        saomaBtn.onClick.AddListener(OnSaoMa);
        confirmOkBtn.onClick.AddListener(OnConfirmOk);
        confirmCancelBtn.onClick.AddListener(OnConfirmCancel);
    }

    bool isGoScanning = false;
    float interval = 0;
    void Update()
    {
        if (isGoScanning)
        {
            interval += Time.deltaTime;
            if (interval >= 0.5f)
            {
                SaoMaRunning();
                interval = 0;
            }
        }
    }

    private void OnEnable()
    {
        UpdateList();
    }

    void UpdateList()
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllWaterSealByCondition");
        GameUtils.PostHttp("WaterSeal.ashx", form, (result) =>
        {
            List<WaterSealModel> list = JsonMapper.ToObject<List<WaterSealModel>>(result);
            if (list.Count > 0)
            {
                int pageCount = 0;
                int rowCount = 0;
                form = new WWWForm();
                form.AddField("requestType", "SelectAllPointCheckByCondition");
                form.AddField("deviceType", 1);
                form.AddField("userName", string.Empty);
                form.AddField("deviceName", string.Empty);
                form.AddField("pageIndex", 1);
                form.AddField("pageSize", 100);
                form.AddField("startTime", DateTime.Now.ToString("yyyy-MM-dd"));
                form.AddField("endTime", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                form.AddField("pageCount", pageCount);
                form.AddField("rowCount", rowCount);
                GameUtils.PostHttp("PointCheck.ashx", form, (r1) =>
                {
                    List<PointCheckModel> pointCheckModelList = new List<PointCheckModel>();
                    if (r1.Contains("|"))
                    {
                        r1 = r1.Split('|')[1];
                        pointCheckModelList = JsonMapper.ToObject<List<PointCheckModel>>(r1);
                    }
                    InitGrid(list, pointCheckModelList);
                }, null);
            }
        }, null);
    }

    //序列号  探头具体信息
    Dictionary<string, WaterSealModel> waterSealDic;
    void InitGrid(List<WaterSealModel> waterSealList, List<PointCheckModel> pointCheckModelList)
    {
        if (waterSealList == null || pointCheckModelList == null)
        {
            return;
        }
        if (waterSealList.Count <= 0)
        {
            return;
        }
        waterSealDic = new Dictionary<string, WaterSealModel>();
        for (int i = 0; i < waterSealList.Count; i++)
        {
            WaterSealModel waterSealModel = waterSealList[i];
            if (!string.IsNullOrEmpty(waterSealModel.SerialNumber) && !waterSealDic.ContainsKey(waterSealModel.SerialNumber))
            {
                waterSealDic[waterSealModel.SerialNumber] = waterSealModel;
            }
            PointCheckModel pointCheckModel = pointCheckModelList.Find((item) => { return item.DeviceID == waterSealModel.ID; });
            waterSealModel.isCheck = pointCheckModel != null;
        }
        waterSealList.Sort((a, b) =>
        {
            return a.isCheck ? (b.isCheck ? 0 : 1) : (b.isCheck ? -1 : 0);
        });
        GameUtils.SpawnCellForTable<WaterSealModel>(contentTrans, waterSealList, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            WaterSealPointCheckItem item = currentObj.GetComponent<WaterSealPointCheckItem>();
            item.InitData(data);
            item.SetBackgroundColor(data.isCheck ? new Color(0.01f, 0.57f, 0.47f) : new Color(0.5f, 0.5f, 0.5f));
            currentObj.SetActive(true);
        });
    }

    BarcodeReader barcodeReader;
    WebCamTexture webCamTextrue;
    void OnSaoMa()
    {
        if (webCamTextrue == null)
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            string deviceName = devices[0].name;
            webCamTextrue = new WebCamTexture(deviceName, 600, 600);
            webCamTextrue.Play();
            cameraTexture.texture = webCamTextrue;
            barcodeReader = new BarcodeReader();
        }
        interval = 0;
        isGoScanning = true;
    }

    void SaoMaRunning()
    {
        Color32[] data = webCamTextrue.GetPixels32();
        Result result = barcodeReader.Decode(data, webCamTextrue.width, webCamTextrue.height);
        //if (result != null)
        {
            isGoScanning = false;
            OnSaoMaComplete("0002");
        }
    }

    WaterSealModel currentWaterSealModel = null;
    void OnSaoMaComplete(string serialNumber)
    {
        if (waterSealDic != null && waterSealDic.Count > 0 && waterSealDic.ContainsKey(serialNumber))
        {
            currentWaterSealModel = waterSealDic[serialNumber];
            confirmInputField.text = string.Empty;
            confirmProbeNameText.text = currentWaterSealModel.Number;
            Texture2D texture2D = new Texture2D(webCamTextrue.width, webCamTextrue.height);
            texture2D.SetPixels(webCamTextrue.GetPixels());
            texture2D.Apply();
            Sprite sp = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2());
            confirmImg.sprite = sp;
            confirmPanel.SetActive(true);
        }
    }

    void OnConfirmCancel()
    {
        currentWaterSealModel = null;
        confirmPanel.SetActive(false);
    }

    void OnConfirmOk()
    {
        string description = confirmInputField.text;
        byte[] bytes = confirmImg.sprite.texture.EncodeToJPG();
        string fileName = DateTime.Now.ToString("MMddHHmmss") + ".jpg";
        WWWForm form = new WWWForm();
        form.AddField("requestType", "UploadFile");
        form.AddBinaryData("file", bytes, fileName, "image/jpeg");
        GameUtils.PostHttpWebRequest("PointCheck.ashx", form, (qrCodePath) =>
        {
            string result = System.Text.Encoding.UTF8.GetString(qrCodePath);
            InsertPointCheck(result, description);
            currentWaterSealModel = null;
            confirmPanel.SetActive(false);
        }, (error) =>
        {
            currentWaterSealModel = null;
            confirmPanel.SetActive(false);
            MessageBox.Instance.PopOK(error, null, "确定");
        });
    }

    void InsertPointCheck(string saomaImgServerPath, string description)
    {
        string togResult = string.Empty;
        for (int i = 0; i < confirmTogList.Count; i++)
        {
            togResult += confirmTogList[i].isOn + ",";
        }
        togResult = togResult.TrimEnd(',');
        if (currentWaterSealModel != null)
        {
            WWWForm form = new WWWForm();
            form.AddField("requestType", "InsertPointCheck");
            form.AddField("deviceID", currentWaterSealModel.ID);
            form.AddField("deviceName", currentWaterSealModel.Number);
            form.AddField("userName", FormatData.currentUser.UserName);
            form.AddField("qrCodePath", saomaImgServerPath);
            form.AddField("description", description);
            form.AddField("result", togResult);
            form.AddField("deviceType", 1);
            GameUtils.PostHttp("PointCheck.ashx", form, (content) =>
            {
                UpdateList();
                MessageBox.Instance.PopOK("点检成功", null, "确定");
            }, null);
        }
    }


}
