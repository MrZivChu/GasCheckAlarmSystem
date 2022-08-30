using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using UnityEngine.UI;
using System;
using LitJson;
using System.IO;

public class ShuiFengPointCheckPanel : MonoBehaviour
{
    public Image okImg;
    public RawImage cameraTexture;
    public Button goScanning;
    public Text showText;

    public Transform contentTrans;
    UnityEngine.Object itemRes;

    private void Awake()
    {
        itemRes = Resources.Load("PointCheckDayItem");
    }
    void Start()
    {
        goScanning.onClick.AddListener(DeviceInit);
    }

    bool isGoScanning = false;
    float interval = 0;
    void Update()
    {
        if (isGoScanning)
        {
            interval += Time.deltaTime;
            if (interval >= 6.5f)
            {
                ScanQRCode();
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
        form.AddField("requestType", "SelectAllProbeByCondition");
        GameUtils.PostHttp("Probe.ashx", form, (result) =>
        {
            List<ProbeModel> list = JsonMapper.ToObject<List<ProbeModel>>(result);
            int pageCount = 0, rowCount = 0;

            form = new WWWForm();
            form.AddField("requestType", "SelectAllPointCheckByCondition");
            form.AddField("pageIndex", 1);
            form.AddField("pageSize", 100);
            form.AddField("userName", FormatData.currentUser != null ? FormatData.currentUser.UserName : string.Empty);
            form.AddField("startTime", DateTime.Now.ToString("yyyy-MM-dd"));
            form.AddField("endTime", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            form.AddField("pageCount", pageCount);
            form.AddField("rowCount", rowCount);
            GameUtils.PostHttp("PointCheck.ashx", form, (r1) =>
            {
                List<PointCheckModel> pointCheckModelList = new List<PointCheckModel>();
                if (r1.Contains("*"))
                {
                    r1 = r1.Split('*')[1];
                    pointCheckModelList = JsonMapper.ToObject<List<PointCheckModel>>(r1);
                }
                InitGrid(list, pointCheckModelList);
            }, null);

        }, null);
    }

    void InitGrid(List<ProbeModel> probeList, List<PointCheckModel> pointCheckModelList)
    {
        for (int i = 0; i < probeList.Count; i++)
        {
            ProbeModel probeModel = probeList[i];
            PointCheckModel pointCheckModel = pointCheckModelList.Find((item) => { return item.ProbeID == probeModel.ID; });
            probeModel.isCheck = pointCheckModel != null;
        }
        probeList.Sort((a, b) =>
        {
            if (a.isCheck)
            {
                if (b.isCheck)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (b.isCheck)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

        });
        GameUtils.SpawnCellForTable<ProbeModel>(contentTrans, probeList, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            PointCheckDayItem item = currentObj.GetComponent<PointCheckDayItem>();
            item.InitData(data);
            item.SetBackgroundColor(data.isCheck ? new Color(0.01f, 0.57f, 0.47f) : new Color(0.5f, 0.5f, 0.5f));
            currentObj.SetActive(true);
        });
    }

    BarcodeReader barcodeReader;
    private WebCamTexture webCamTextrue;
    void DeviceInit()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        string deviceName = devices[0].name;
        webCamTextrue = new WebCamTexture(deviceName, 1, 1);
        cameraTexture.texture = webCamTextrue;
        webCamTextrue.Play();
        barcodeReader = new BarcodeReader();

        interval = 0;
        isGoScanning = true;
    }

    void ScanQRCode()
    {
        //Color32[] data = webCamTextrue.GetPixels32();
        //Result result = barcodeReader.Decode(data, webCamTextrue.width, webCamTextrue.height);
        //if (result != null)
        {
            isGoScanning = false;
            UploadQrCodeImg();
        }
        //else
        //{
        //    showText.text = "正在扫描识别中...";
        //}
    }

    void UploadQrCodeImg()
    {
        Texture2D texture2D = new Texture2D(webCamTextrue.width, webCamTextrue.height);
        texture2D.SetPixels(webCamTextrue.GetPixels());
        texture2D.Apply();
        byte[] bytes = texture2D.EncodeToJPG();

        string fileName = DateTime.Now.ToString("MMddHHmmss") + ".jpg";
        WWWForm form = new WWWForm();
        form.AddField("requestType", "UploadFile");
        form.AddBinaryData("file", bytes, fileName, "image/jpeg");
        GameUtils.PostHttpWebRequest("PointCheck.ashx", form, (qrCodePath) =>
        {
            string result = System.Text.Encoding.UTF8.GetString(qrCodePath);
            InsertPointCheck(result);
        }, (error) =>
        {
            print(error);
        });
    }

    void InsertPointCheck(string qrCodePath)
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "InsertPointCheck");
        form.AddField("probeID", 1);
        form.AddField("probeName", /*result.Text*/"探头1");
        form.AddField("userName", FormatData.currentUser != null ? FormatData.currentUser.UserName : "--");
        form.AddField("qrCodePath", qrCodePath);
        GameUtils.PostHttp("PointCheck.ashx", form, (content) =>
        {
            okImg.gameObject.SetActive(true);
            GameUtils.DelayWaitForSeconds(2, () =>
            {
                interval = 0;
                isGoScanning = true;
                okImg.gameObject.SetActive(false);
                showText.text = string.Empty;
            });

        }, null);
    }


}
