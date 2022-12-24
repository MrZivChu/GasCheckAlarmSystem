using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using UnityEngine.UI;
using System;
using LitJson;
using System.IO;

public class ProbePointCheckPanel : MonoBehaviour
{
    public Image confirmImg;
    public Button confirmOkBtn;
    public Button confirmCancelBtn;
    public Text confirmProbeNameText;
    public InputField confirmInputField;
    public GameObject confirmPanel;

    public Button saomaBtn;
    public SaomaPanel saomaPanel;

    public Transform contentTrans;
    public UnityEngine.Object itemRes;
    void Start()
    {
        saomaBtn.onClick.AddListener(OnSaoMa);
        confirmOkBtn.onClick.AddListener(OnConfirmOk);
        confirmCancelBtn.onClick.AddListener(OnConfirmCancel);
    }

    private void OnEnable()
    {
        UpdateList();
    }

    void UpdateList()
    {
        List<ProbeModel> list = ProbeDAL.SelectAllProbeByCondition();
        if (list.Count > 0)
        {
            int pageCount = 0;
            int rowCount = 0;
            List<PointCheckModel> pointCheckModelList = PointCheckDAL.SelectAllPointCheckByCondition(1, 100, string.Empty, string.Empty, 0, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), out pageCount, out rowCount);
            InitGrid(list, pointCheckModelList);
        }
    }

    //序列号  探头具体信息
    Dictionary<string, ProbeModel> probeDic;
    void InitGrid(List<ProbeModel> probeList, List<PointCheckModel> pointCheckModelList)
    {
        if (probeList == null || pointCheckModelList == null)
        {
            return;
        }
        if (probeList.Count <= 0)
        {
            return;
        }
        probeDic = new Dictionary<string, ProbeModel>();
        for (int i = 0; i < probeList.Count; i++)
        {
            ProbeModel probeModel = probeList[i];
            if (!string.IsNullOrEmpty(probeModel.SerialNumber) && !probeDic.ContainsKey(probeModel.SerialNumber))
            {
                probeDic[probeModel.SerialNumber] = probeModel;
            }
            PointCheckModel pointCheckModel = pointCheckModelList.Find((item) => { return item.DeviceID == probeModel.ID; });
            probeModel.isCheck = pointCheckModel != null;
        }
        probeList.Sort((a, b) =>
        {
            return a.isCheck ? (b.isCheck ? 0 : 1) : (b.isCheck ? -1 : 0);
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
            ProbePointCheckItem item = currentObj.GetComponent<ProbePointCheckItem>();
            item.InitData(data);
            item.SetBackgroundColor(data.isCheck ? new Color(0.01f, 0.57f, 0.47f) : new Color(0.5f, 0.5f, 0.5f));
            currentObj.SetActive(true);
        });
    }

    void OnSaoMa()
    {
        saomaPanel.Run(SaomaSuccess);
    }

    void SaomaSuccess(string content, WebCamTexture camera)
    {
        if (!string.IsNullOrEmpty(content))
        {
            int startIndex = content.IndexOf("点位编号");
            int endIndex = content.IndexOf("生产厂家");
            if (endIndex > startIndex)
            {
                content = content.Substring(startIndex + 5, endIndex - startIndex - 6);
                OnSaoMaComplete(content, camera);
                return;
            }
            else
            {
                MessageBox.Instance.PopOK("生产厂家必须在点位编号后面，信息为：" + content, null, "确定");
                return;
            }
        }
        MessageBox.Instance.PopOK("二维码信息为空", null, "确定");
    }

    ProbeModel currentProbeModel = null;
    void OnSaoMaComplete(string serialNumber, WebCamTexture camera)
    {
        Debug.Log(serialNumber);
        if (probeDic != null && probeDic.Count > 0 && probeDic.ContainsKey(serialNumber))
        {
            currentProbeModel = probeDic[serialNumber];
            confirmInputField.text = string.Empty;
            confirmProbeNameText.text = currentProbeModel.ProbeName;
            Texture2D texture2D = new Texture2D(camera.width, camera.height);
            texture2D.SetPixels(camera.GetPixels());
            texture2D.Apply();
            Sprite sp = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2());
            confirmImg.sprite = sp;
            confirmPanel.SetActive(true);
        }
    }

    void OnConfirmCancel()
    {
        currentProbeModel = null;
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
            currentProbeModel = null;
            confirmPanel.SetActive(false);
        }, (error) =>
        {
            currentProbeModel = null;
            confirmPanel.SetActive(false);
            MessageBox.Instance.PopOK(error, null, "确定");
        });
    }

    void InsertPointCheck(string saomaImgServerPath, string description)
    {
        if (currentProbeModel != null)
        {
            PointCheckDAL.InsertPointCheck(currentProbeModel.ID, currentProbeModel.ProbeName, FormatData.currentUser.UserName, saomaImgServerPath, description, string.Empty, 0);
            UpdateList();
            MessageBox.Instance.PopOK("点检成功", null, "确定");
        }
    }
}
