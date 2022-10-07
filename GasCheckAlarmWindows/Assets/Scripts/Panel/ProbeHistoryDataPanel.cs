using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ProbeHistoryDataPanel : UIEventHelper
{
    public Button btn_search;
    public Button btn_saveExcel;
    public Button btn_deleteAllData;

    public Button btn_prePage;
    public Button btn_nextPage;
    public Text txt_pageCount;
    int pageIndex = 1;
    int pageCount = 1;
    int rowCount = 1;
    int pageSize = 13;

    public UI.Dates.DatePicker_DateRange dateRange;

    public Transform contentTrans;
    public UnityEngine.Object itemRes;
    private void Start()
    {
        RegisterBtnClick(btn_search, OnSearch);
        RegisterBtnClick(btn_saveExcel, OnSaveExcel);
        RegisterBtnClick(btn_deleteAllData, OnDeleteAllData);

        RegisterBtnClick(btn_prePage, OnPrePagel);
        RegisterBtnClick(btn_nextPage, OnNextPage);
        btn_deleteAllData.gameObject.SetActive(FormatData.currentUser.Authority == EAuthority.Admin);
    }

    List<ProbeModel> baseInfoList_ = new List<ProbeModel>();
    private void OnEnable()
    {
        baseInfoList_ = ProbeDAL.SelectIDProbeNameGasKindMachineID();
        InitData();
    }

    void OnPrePagel(Button btn)
    {
        if (pageIndex - 1 >= 1)
        {
            pageIndex--;
            InitData();
        }
    }

    void OnNextPage(Button btn)
    {
        if (pageIndex + 1 <= pageCount)
        {
            pageIndex++;
            InitData();
        }
    }

    void OnDeleteAllData(Button btn)
    {
        HistoryDataDAL.DeleteAllHistoryData();
        InitData();
    }

    void OnSearch(Button btn)
    {
        pageIndex = 1;
        InitData();
    }

    List<HistoryDataModel> historyDataModelList;
    private void InitData()
    {
        string startTime = dateRange.FromDate.HasValue ? dateRange.FromDate.Date.ToString("yyyy-MM-dd") : string.Empty; ;
        string endTime = dateRange.ToDate.HasValue ? dateRange.ToDate.Date.AddDays(1).ToString("yyyy-MM-dd") : string.Empty; ;
        historyDataModelList = HistoryDataDAL.SelectAllHistoryDataByCondition(pageIndex, pageSize, startTime, endTime, out pageCount, out rowCount);
        foreach (var item in historyDataModelList)
        {
            ProbeModel model = baseInfoList_.Find(it => it.ID == item.ProbeID);
            if (model != null)
            {
                item.ProbeID = model.ID;
                item.probeName = model.ProbeName;
                item.gasKind = model.GasKind;
                item.MachineID = model.MachineID;
            }
        }
        txt_pageCount.text = pageIndex + "/" + pageCount;
        InitGrid(historyDataModelList);
    }

    void InitGrid(List<HistoryDataModel> historyDataModelList)
    {
        GameUtils.SpawnCellForTable<HistoryDataModel>(contentTrans, historyDataModelList, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            ProbeHistoryDataItem item = currentObj.GetComponent<ProbeHistoryDataItem>();
            item.InitData(pageSize * (pageIndex - 1) + index + 1, data);
            item.SetBackgroundColor(index % 2 == 0 ? new Color(239 / 255.0f, 243 / 255.0f, 250 / 255.0f) : new Color(1, 1, 1));
            currentObj.SetActive(true);
        });
    }

    void OnSaveExcel(Button btn)
    {
        if (historyDataModelList != null && historyDataModelList.Count > 0)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Excel文件(*.xlsx)|*.xlsx|所有文件|*.*";//设置文件类型
            sfd.FileName = "探头实时数据";//设置默认文件名
            sfd.DefaultExt = "xlsx";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(sfd.FileName))
                {
                    System.DateTime startTime = System.DateTime.MinValue, endTime = System.DateTime.MinValue;
                    if (dateRange.FromDate.HasValue)
                        startTime = dateRange.FromDate.Date;
                    if (dateRange.ToDate.HasValue)
                        endTime = dateRange.ToDate.Date;
                    ExcelAccess.WriteExcel(sfd.FileName, "sheet1", historyDataModelList);
                    MessageBox.Instance.PopOK("保存成功", null, "确定");
                }
            }
        }
    }
}