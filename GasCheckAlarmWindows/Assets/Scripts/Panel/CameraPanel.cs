using PreviewDemo;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CameraPanel : UIEventHelper
{
    public Button addBtn;
    public Dropdown dropDown;
    public GameObject itemRes;
    public Transform contentTrans;
    public GameObject addCameraPanel;
    readonly string publicIp_ = "106.14.213.225";

    private void Awake()
    {
        CHCNetSDK.NET_DVR_Init();
        dropDown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void Start()
    {
        RegisterBtnClick(addBtn, OnAdd);
        EventManager.Instance.AddEventListener(NotifyType.AddCamera, AddCameraEvent);
        EventManager.Instance.AddEventListener(NotifyType.EditorCamera, EditorCameraEvent);
    }

    void OnDestroy()
    {
        CHCNetSDK.NET_DVR_Cleanup();
        EventManager.Instance.DeleteEventListener(NotifyType.AddCamera, AddCameraEvent);
        EventManager.Instance.DeleteEventListener(NotifyType.EditorCamera, EditorCameraEvent);
    }

    void OnDropdownValueChanged(int value)
    {
        InitGrid(cameraModelList_);
    }

    void AddCameraEvent(object data)
    {
        Debug.Log("增加摄像机数据回调");
        cameraModelList_ = CameraDAL.SelectAllCameras();
        ReRunFrp(cameraModelList_);
        InitGrid(cameraModelList_);
    }

    void EditorCameraEvent(object data)
    {
        Debug.Log("修改摄像机数据回调");
        cameraModelList_ = CameraDAL.SelectAllCameras();
        ReRunFrp(cameraModelList_);
        InitGrid(cameraModelList_);
    }

    void OnAdd(Button btn)
    {
        addCameraPanel.SetActive(true);
    }

    List<CameraModel> cameraModelList_;
    private void OnEnable()
    {
        Debug.Log("初始化数据");
        cameraModelList_ = CameraDAL.SelectAllCameras();
        InitDropDownList(cameraModelList_);
        ReRunFrp(cameraModelList_);
        InitGrid(cameraModelList_);
    }

    void InitGrid(List<CameraModel> list)
    {
        GameUtils.SpawnCellForTable<CameraModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            CameraItem item = currentObj.GetComponent<CameraItem>();
            item.Connect(data, publicIp_, IsUsePublicNetWork());
            currentObj.SetActive(true);
        }, false);
    }

    void ReRunFrp(List<CameraModel> list)
    {
        bool isIn = IsInSameNetworkPcWithCamera(list);
        if (isIn)
        {
            Debug.Log("此电脑和摄像头在同一个局域网，需要开启frp服务");
        }
        else
        {
            Debug.Log("此电脑和摄像头不在同一个局域网，不需要开启frp服务");
            return;
        }

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

    void InitDropDownList(List<CameraModel> list)
    {
        dropDown.options.Clear();
        if (list == null || list.Count == 0)
        {
            return;
        }
        bool isIn = IsInSameNetworkPcWithCamera(list);
        if (isIn)
        {
            List<string> strList = new List<string>() { "局域网访问", "广域网访问" };
            dropDown.AddOptions(strList);
        }
        else
        {
            List<string> strList = new List<string>() { "广域网访问" };
            dropDown.AddOptions(strList);
        }
        dropDown.value = 0;
    }

    bool IsUsePublicNetWork()
    {
        bool isIn = IsInSameNetworkPcWithCamera(cameraModelList_);
        if (isIn)
        {
            return dropDown.value == 1;
        }
        else
        {
            return dropDown.value == 0;
        }
    }

    bool IsInSameNetworkPcWithCamera(List<CameraModel> list)
    {
        if (list == null || list.Count <= 0)
        {
            return false;
        }
        string localIP = GameUtils.GetIP();
        int lastIndex1 = localIP.LastIndexOf('.');
        string deviceIP = list[0].IP;
        int lastIndex2 = deviceIP.LastIndexOf('.');
        if (deviceIP.Substring(0, lastIndex1) != localIP.Substring(0, lastIndex2))
        {
            return false;
        }
        return true;
    }
}
