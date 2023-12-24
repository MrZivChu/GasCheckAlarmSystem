using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCharts;
using XCharts.Runtime;

class RealtimeTempModel
{
    public double GasValue;
    public string CheckTime;
}

public class RealtimeChartManagerPanel : UIEventHelper
{
    public SimplifiedLineChart simplifiedLineChart;
    public InputField minutesInput;

    private void Start()
    {
        yAxis = simplifiedLineChart.GetChartComponent<YAxis>();
        xAxis = simplifiedLineChart.GetChartComponent<XAxis>();
        simplifiedLineChart.AddChartComponentWhenNoExist<Legend>();
        Title title = simplifiedLineChart.GetChartComponent<Title>();
        title.text = "探头实时数据折线图";
        title.subText = "显示每个探头的最新数据，并实时更新";
        minutesInput.text = GameUtils.GetInt("realTimeMinuInput", 1).ToString();
        RegisterInputFieldOnEndEdit(minutesInput, OnRealtimeMinuInputEnd);
    }

    void OnRealtimeMinuInputEnd(InputField input, string content)
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            int number = Convert.ToInt32(input.text);
            GameUtils.SetInt("realTimeMinuInput", number);
        }
    }

    //UpdateRealtimeDataList的注册和销毁由MainPanel代为注册和销毁
    //因为实时数据要一进入场景就立马获取数据，不能等此节目打开才去获取数据，这样太迟
    public void UpdateProbeListEvent(object data)
    {
        OnUpdateData((List<ProbeModel>)data);
        UpdateSimplifiedLineChart();
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

    Dictionary<string, List<ProbeModel>> dic_ = new Dictionary<string, List<ProbeModel>>();
    void OnUpdateData(List<ProbeModel> realtimeDataList)
    {
        lock (dic_)
        {
            if (realtimeDataList != null && realtimeDataList.Count > 0)
            {
                if (!string.IsNullOrEmpty(minutesInput.text))
                {
                    float min = Convert.ToSingle(minutesInput.text);
                    DateTime now = DateTime.Now;
                    foreach (var item in dic_)
                    {
                        for (int i = item.Value.Count - 1; i >= 0; i--)
                        {
                            TimeSpan ts = now - item.Value[i].CheckTime;
                            if (ts.TotalMinutes >= min)
                            {
                                item.Value.RemoveAt(i);
                            }
                        }
                    }
                }
                DateTime sameTime = realtimeDataList[0].CheckTime;
                foreach (var item in realtimeDataList)
                {
                    sameTime = item.CheckTime > sameTime ? item.CheckTime : sameTime;
                }
                realtimeDataList.ForEach(it =>
                {
                    if (GlobalCheckGas.baseInfoDic_.ContainsKey(it.ID))
                    {
                        ProbeModel model = GlobalCheckGas.baseInfoDic_[it.ID];
                        if (model != null)
                        {
                            if (!dic_.ContainsKey(model.ProbeName))
                            {
                                dic_[model.ProbeName] = new List<ProbeModel>();
                            }
                            maxGasValue = it.GasValue > maxGasValue ? it.GasValue : maxGasValue;
                            minGasValue = it.GasValue < minGasValue ? it.GasValue : minGasValue;
                            it.CheckTime = sameTime;
                            dic_[model.ProbeName].Add(it);
                        }
                    }
                });
            }
        }
    }


    Dictionary<string, bool> simplifiedLineStatus = new Dictionary<string, bool>();
    YAxis yAxis = null;
    XAxis xAxis = null;
    double maxGasValue = 0;
    double minGasValue = 0;
    void UpdateSimplifiedLineChart()
    {
        if (dic_.Count > 0 && gameObject.activeSelf)
        {
            List<Serie> list = simplifiedLineChart.series;
            for (int j = 0; j < list.Count; j++)
            {
                simplifiedLineStatus[list[j].serieName] = list[j].show;
            }
            simplifiedLineChart.RemoveAllSerie();
            foreach (var item in dic_)
            {
                SimplifiedLine simplifiedLine = simplifiedLineChart.AddSerie<SimplifiedLine>(item.Key);
                if (simplifiedLineStatus.ContainsKey(item.Key))
                {
                    simplifiedLine.show = simplifiedLineStatus[item.Key];
                }
                simplifiedLine.AnimationEnable(false);
                for (int i = 0; i < item.Value.Count; i++)
                {
                    SerieData serieData = new SerieData();
                    serieData.data = new List<double>() { i, item.Value[i].GasValue };
                    //simplifiedLine.show = false;
                    simplifiedLine.AddSerieData(serieData);
                }
            }
            if (yAxis != null)
            {
                yAxis.min = minGasValue;
                yAxis.max = maxGasValue;
                yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
            }
            if (xAxis != null)
            {
                xAxis.ClearData();
                foreach (var item in dic_)
                {
                    for (int j = 0; j < item.Value.Count; j++)
                    {
                        xAxis.AddData(item.Value[j].CheckTime.ToString("HH:mm:ss"));
                    }
                    break;
                }
            }
        }
    }
}
