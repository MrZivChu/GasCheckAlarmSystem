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
    public GameObject itemRes;
    public Transform contentTrans;
    readonly string serverIP = "106.14.213.225";

    private void Awake()
    {
        CHCNetSDK.NET_DVR_Init();
    }

    void OnDestroy()
    {
        CHCNetSDK.NET_DVR_Cleanup();
    }

    private void OnEnable()
    {
        List<CameraModel> list = CameraDAL.SelectAllCameras();
        InitGrid(list);
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
            data.IP = serverIP;
            item.Connect(data);
            currentObj.SetActive(true);
        }, false);
    }
}
