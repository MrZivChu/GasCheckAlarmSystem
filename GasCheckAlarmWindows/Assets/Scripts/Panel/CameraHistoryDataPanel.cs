using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CameraHistoryDataPanel : UIEventHelper
{
    public Button btn_prePage;
    public Button btn_nextPage;
    public Dropdown drop_cameraID;
    public Text txt_pageCount;
    int pageIndex_ = 1;
    int pageCount_ = 1;
    int rowCount = 1;
    int pageSize_ = 13;

    public UI.Dates.DatePicker dateRange;

    public Transform contentTrans;
    public UnityEngine.Object itemRes;
    private void Start()
    {
        RegisterBtnClick(btn_prePage, OnPrePagel);
        RegisterBtnClick(btn_nextPage, OnNextPage);
        RegisterDropDownOnValueChanged(drop_cameraID, OnSelectCameraID);
        dateRange.Config.Events.OnDaySelected.AddListener(SelectTime);
    }

    int gasTypeCount = 4;
    int startTime_;
    int endTime_;
    string androidID_;
    Dictionary<string, string> ipAndroidIDDic = new Dictionary<string, string>();
    void SelectTime(DateTime time)
    {
        Debug.Log("SelectTime = " + time.ToString("yyyy-MM-dd HH:mm::ss"));
        pageIndex_ = 1;
        FormatTime(time);
        InitData();
    }

    List<CameraModel> baseInfoList_ = new List<CameraModel>();
    private void OnEnable()
    {
        baseInfoList_ = CameraDAL.SelectCameraInfoForHistory();
        if (baseInfoList_ != null && baseInfoList_.Count > 0)
        {
            dateRange.Ref_InputField.text = DateTime.Now.ToString("yyyy-MM-dd");
            FormatTime(DateTime.Now);
            InitDropdownList();
            InitData();
        }
    }

    void InitDropdownList()
    {
        drop_cameraID.ClearOptions();
        ipAndroidIDDic.Clear();
        List<string> list = new List<string>();
        for (int i = 0; i < baseInfoList_.Count; i++)
        {
            list.Add(baseInfoList_[i].IP);
            androidID_ = baseInfoList_[0].AndroidID;
            if (!ipAndroidIDDic.ContainsKey(baseInfoList_[i].IP))
            {
                ipAndroidIDDic.Add(baseInfoList_[i].IP, baseInfoList_[i].AndroidID);
            }
        }
        drop_cameraID.AddOptions(list);
        drop_cameraID.value = 0;
    }

    void FormatTime(DateTime time)
    {
        DateTime dt = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
        startTime_ = CSharpUtils.GetTimeStamp(dt);
        dt = dt.AddDays(1);
        endTime_ = CSharpUtils.GetTimeStamp(dt);
    }

    void OnSelectCameraID(Dropdown dropdown, int index)
    {
        Debug.Log("OnSelectCameraID = " + index);
        if (index < baseInfoList_.Count)
        {
            if (ipAndroidIDDic.ContainsKey(baseInfoList_[index].IP))
            {
                pageIndex_ = 1;
                androidID_ = ipAndroidIDDic[baseInfoList_[index].IP];
                InitData();
            }
        }
    }

    void OnPrePagel(Button btn)
    {
        if (pageIndex_ - 1 >= 1)
        {
            pageIndex_--;
            InitData();
        }
    }

    void OnNextPage(Button btn)
    {
        if (pageIndex_ + 1 <= pageCount_)
        {
            pageIndex_++;
            InitData();
        }
    }

    private void InitData()
    {
        Debug.Log(androidID_ + "=" + startTime_ + "=" + endTime_ + "=" + pageIndex_ + "=" + pageSize_);
        List<CameraHistoryModel> historyDataModelList = CameraHistoryDAL.SelectAllHistoryDataByCondition(androidID_, pageIndex_, pageSize_, startTime_, endTime_, out pageCount_, out rowCount);
        List<CustomeCameraGasModel> result = new List<CustomeCameraGasModel>();
        for (int i = 0; i < historyDataModelList.Count; i++)
        {
            string gasValues = historyDataModelList[i].GasValues;
            CameraModel cameraModel = baseInfoList_.Find(it => it.AndroidID == historyDataModelList[i].AndroidID);
            if (cameraModel != null && !string.IsNullOrEmpty(gasValues))
            {
                string[] gasValuesArray = gasValues.Split(',');
                string[] gasInfosArray = cameraModel.GasInfos.Split(';');
                if (gasValuesArray.Length >= gasTypeCount && gasInfosArray.Length >= gasTypeCount)
                {
                    for (int j = 0; j < gasTypeCount; j++)
                    {
                        string[] singleArray = gasInfosArray[j].Split(',');
                        print(gasInfosArray[j]);
                        if (singleArray.Length >= gasTypeCount)
                        {
                            int value = 0;
                            if (int.TryParse(gasValuesArray[j], out value))
                            {
                                CustomeCameraGasModel model = new CustomeCameraGasModel();
                                model.ip = cameraModel.IP;
                                model.gasName = singleArray[1];
                                model.value = value;
                                int firstWarnValue = 0;
                                int.TryParse(singleArray[2], out firstWarnValue);
                                model.firstWarnValue = firstWarnValue;
                                int secondWarnValue = 0;
                                int.TryParse(singleArray[3], out secondWarnValue);
                                model.secondWarnValue = secondWarnValue;
                                model.timeStamp = historyDataModelList[i].TimeStamp;
                                result.Add(model);
                            }
                        }
                    }
                }
            }
        }
        txt_pageCount.text = pageIndex_ + "/" + pageCount_;
        int gasSpanIndex = 0;
        GameUtils.SpawnCellForTable<CustomeCameraGasModel>(contentTrans, result, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            CameraHistoryDataItem item = currentObj.GetComponent<CameraHistoryDataItem>();
            item.InitData(pageSize_ * (pageIndex_ - 1) * gasTypeCount + index + 1, data.ip, data.gasName, data.value, data.firstWarnValue, data.secondWarnValue, data.timeStamp);
            Color color = gasSpanIndex < gasTypeCount ? new Color(239 / 255.0f, 243 / 255.0f, 250 / 255.0f) : new Color(1, 1, 1);
            item.SetBackgroundColor(color);
            currentObj.SetActive(true);
            gasSpanIndex++;
            if (gasSpanIndex == gasTypeCount * 2)
            {
                gasSpanIndex = 0;
            }
        });
    }
}

class CustomeCameraGasModel
{
    public string ip;
    public string gasName;
    public int value;
    public int firstWarnValue;
    public int secondWarnValue;
    public int timeStamp;
}