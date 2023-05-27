using PreviewDemo;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CameraPanel : UIEventHelper
{
    public Button addBtn;
    public Dropdown dropDown;
    public GameObject itemRes;
    public Transform contentTrans;
    readonly string publicIp_ = "106.14.213.225";

    private void Awake()
    {
        CHCNetSDK.NET_DVR_Init();
        dropDown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDropdownValueChanged(int value)
    {
        InitGrid();
    }

    void Start()
    {
        RegisterBtnClick(addBtn, OnAdd);
        EventManager.Instance.AddEventListener(NotifyType.EditorCameraParams, EditorCameraParamsEvent);
    }

    void OnDestroy()
    {
        CHCNetSDK.NET_DVR_Cleanup();
        EventManager.Instance.DeleteEventListener(NotifyType.EditorCameraParams, EditorCameraParamsEvent);
    }

    void EditorCameraParamsEvent(object data)
    {
        GameObject item = (GameObject)data;
        CameraItem cameraItem = item.GetComponent<CameraItem>();
        CameraDAL.EditCameraByID(cameraItem.model_.ID, cameraItem.model_.IP, cameraItem.model_.Port, cameraItem.model_.UserName, cameraItem.model_.UserPwd);

        ReRunFrp();
        cameraItem.Connect(cameraItem.model_, publicIp_, dropDown.value == 1);
    }

    void OnAdd(Button btn)
    {
        int id = CameraDAL.InsertCamera("", "", "", "");
        CameraModel model = new CameraModel();
        model.ID = id;
        model.UserName = "admin";
        GameObject currentObj = InstanceItem();
        currentObj.transform.SetParent(contentTrans);
        CameraItem item = currentObj.GetComponent<CameraItem>();
        item.Connect(model, publicIp_, dropDown.value == 1);
        currentObj.SetActive(true);
    }

    private void OnEnable()
    {
        ReRunFrp();
        InitGrid();
    }

    void InitGrid()
    {
        List<CameraModel> list = CameraDAL.SelectAllCameras();
        GameUtils.SpawnCellForTable<CameraModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = InstanceItem();
            }
            CameraItem item = currentObj.GetComponent<CameraItem>();
            item.Connect(data, publicIp_, dropDown.value == 1);
            currentObj.SetActive(true);
        }, false);
    }

    GameObject InstanceItem()
    {
        GameObject currentObj = Instantiate(itemRes) as GameObject;
        currentObj.transform.SetParent(contentTrans);
        currentObj.transform.localScale = Vector3.one;
        currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        return currentObj;
    }

    void ReRunFrp()
    {
        List<CameraModel> list = CameraDAL.SelectAllCameras();
        CSharpUtils.KillProcess("frpc");
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[common]");
        sb.AppendLine("server_addr = " + publicIp_);
        sb.AppendLine("server_port = 7000");
        sb.AppendLine(" ");
        foreach (CameraModel model in list)
        {
            if (string.IsNullOrEmpty(model.IP) || string.IsNullOrEmpty(model.Port) || string.IsNullOrEmpty(model.UserPwd))
            {
                continue;
            }
            sb.AppendLine("[device" + model.Port + "]");
            sb.AppendLine("type = tcp");
            sb.AppendLine("local_ip = " + model.IP);
            sb.AppendLine("local_port = 8000");
            sb.AppendLine("remote_port = " + model.Port);
            sb.AppendLine(" ");
        }
        string frpIniPath = Application.streamingAssetsPath + "/frpc.ini";
        File.WriteAllText(frpIniPath, sb.ToString());
        string fileName = string.Format("{0}/frpc.exe", Application.streamingAssetsPath);
        string arguments = string.Format("-c {0}/frpc.ini", Application.streamingAssetsPath);
        CSharpUtils.StartProcess(fileName, arguments);
    }
}
