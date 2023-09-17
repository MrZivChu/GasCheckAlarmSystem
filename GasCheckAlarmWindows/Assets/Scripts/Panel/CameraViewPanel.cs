using PreviewDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewPanel : UIEventHelper
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
        List<CameraModel> baseDataList = CameraDAL.SelectAllCameraBaseData();
        InitGrid(baseDataList);
    }

    float tempTime_ = 0;
    private void Update()
    {
        tempTime_ += Time.deltaTime;
        if (tempTime_ >= 1)
        {
            tempTime_ = 0;
            List<CameraModel> list = CameraDAL.SelectAllCameraRealtimeData();
            if (itemDic_ != null && list != null && list.Count > 0)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (itemDic_.ContainsKey(list[j].ID))
                    {
                        itemDic_[list[j].ID].UpdateGasValue(list[j].GasValues);
                    }
                }
            }
        }
    }

    Dictionary<int, CameraItem> itemDic_ = new Dictionary<int, CameraItem>();
    void InitGrid(List<CameraModel> list)
    {
        itemDic_.Clear();
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
            itemDic_.Add(data.ID, item);
        }, false);
    }
}
