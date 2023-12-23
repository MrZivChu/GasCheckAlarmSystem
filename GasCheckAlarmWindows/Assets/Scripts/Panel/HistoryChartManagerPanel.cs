using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCharts;
using XCharts.Runtime;

public class HistoryChartManagerPanel : UIEventHelper
{
    public SimplifiedLineChart simplifiedLineChart;
    public Text selectTimeText;
    public InputField probeNameInput;
    public InputField minutesInput;
    public Button btn_search;

    private void Awake()
    {
        selectTimeText.text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private void Start()
    {
        Title title = simplifiedLineChart.GetChartComponent<Title>();
        title.text = "单个探头历史数据折线图";
        //title.subText = "只显示选中时间往后三小时之内的历史数据";
        RegisterBtnClick(btn_search, OnSearch);
    }

    private void OnEnable()
    {
        ClearAll();
    }

    private void OnDisable()
    {
        ClearAll();
    }

    void ClearAll()
    {
        simplifiedLineChart.ClearData();
        simplifiedLineChart.RemoveAllSerie();
        simplifiedLineChart.SetAllDirty();
    }

    void OnSearch(Button btn)
    {
        ClearAll();
        if (!string.IsNullOrEmpty(probeNameInput.text))
        {
            ProbeModel model = ProbeDAL.SelectProbeIDProbeNameByProbeName(probeNameInput.text);
            if (model != null)
            {
                DateTime endTime = DateTime.Parse(selectTimeText.text);
                if (!string.IsNullOrEmpty(minutesInput.text))
                {
                    endTime = endTime.AddMinutes(Convert.ToInt32(minutesInput.text));
                }
                List<HistoryDataModel> list = HistoryDataDAL.SelectAllHistoryDataForChart(model.ID, selectTimeText.text, endTime.ToString("yyyy-MM-dd HH:mm:ss"));
                InitSimplifiedLineChart(list, model.ProbeName);
            }
            else
            {
                MessageBox.Instance.PopOK("未查询到此探头");
            }
        }
        else
        {
            MessageBox.Instance.PopOK("请输入探头名称");
        }
    }

    void InitSimplifiedLineChart(List<HistoryDataModel> list, string probeName)
    {
        if (list != null && list.Count > 0)
        {
            XAxis xAxis = simplifiedLineChart.GetChartComponent<XAxis>();
            xAxis.ClearData();
            for (int i = 0; i < list.Count; i++)
            {
                xAxis.AddData(list[i].CheckTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            SimplifiedLine simplifiedLine = simplifiedLineChart.AddSerie<SimplifiedLine>(probeName);
            double maxGasValue = 0;
            double minGasValue = 0;
            for (int i = 0; i < list.Count; i++)
            {
                HistoryDataModel model = list[i];
                double gasValue = model.GasValue;
                int gaskind = GlobalCheckGas.baseInfoDic_[model.ProbeID].GasKind;
                if (FormatData.gasKindFormat[gaskind].GasName == "氧气" || FormatData.gasKindFormat[gaskind].GasName == "天然气" || FormatData.gasKindFormat[gaskind].GasName == "石油气" || FormatData.gasKindFormat[gaskind].GasName == "可燃气")
                {
                    gasValue = gasValue / 10.0f;
                }
                SerieData serieData = new SerieData();
                serieData.data = new List<double>() { i, gasValue };
                //simplifiedLine.show = false;
                simplifiedLine.AddSerieData(serieData);
                maxGasValue = gasValue >= maxGasValue ? gasValue : maxGasValue;
                minGasValue = gasValue <= minGasValue ? gasValue : minGasValue;
            }
            YAxis yAxis = simplifiedLineChart.GetChartComponent<YAxis>();
            yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
            yAxis.min = minGasValue;
            yAxis.max = maxGasValue;
            simplifiedLineChart.AddChartComponentWhenNoExist<Legend>();
        }
    }
}
