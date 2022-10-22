using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCharts;
using XCharts.Runtime;

public class ChartManagerPanel : UIEventHelper
{
    public SimplifiedLineChart simplifiedLineChart;
    public PieChart pieChart;
    public ScatterChart scatterChart;
    public UI.Dates.DatePicker datePicker;
    public Button btn_search;
    public Dropdown dropdown_machine;
    public Dropdown dropdown_chart;

    private void Start()
    {
        Title title = simplifiedLineChart.GetChartComponent<Title>();
        title.text = "探头历史数据折线图";
        title.subText = "只显示低报和高报时间点，其他时间点均为正常";
        simplifiedLineChart.RemoveAllSerie();

        Title title2 = pieChart.GetChartComponent<Title>();
        title2.text = "探头历史数据饼图";
        title2.subText = "只显示低报和高报数据占比";
        pieChart.RemoveAllSerie();

        Title title3 = scatterChart.GetChartComponent<Title>();
        title3.text = "探头历史数据散点图";
        title3.subText = "只显示低报和高报数据";
        scatterChart.RemoveAllSerie();

        RegisterBtnClick(btn_search, OnSearch);
        RegisterDropDownOnValueChanged(dropdown_chart, OnDropdownChart);
    }

    private void OnEnable()
    {
        InitMachine();
    }

    private void OnDisable()
    {
        simplifiedLineChart.RemoveAllSerie();
        simplifiedLineChart.ClearData();
        simplifiedLineChart.SetAllDirty();
        pieChart.RemoveAllSerie();
        pieChart.ClearData();
        pieChart.SetAllDirty();
        scatterChart.RemoveAllSerie();
        scatterChart.ClearData();
        scatterChart.SetAllDirty();
    }

    void OnDropdownChart(Dropdown dropdown, int index)
    {
        InitChart();
    }

    List<MachineModel> machineList_ = null;
    void InitMachine()
    {
        dropdown_machine.ClearOptions();
        machineList_ = MachineDAL.SelectIDMachineName();
        if (machineList_ != null && machineList_.Count > 0)
        {
            List<string> optionList = new List<string>();
            for (int i = 0; i < machineList_.Count; i++)
            {
                optionList.Add(machineList_[i].MachineName);
            }
            dropdown_machine.AddOptions(optionList);
            dropdown_machine.value = 0;
        }
    }


    List<ProbeModel> probeList_ = null;
    List<HistoryDataModel> historyDataList_ = null;
    void OnSearch(Button btn)
    {
        if (machineList_ != null && machineList_.Count > 0 && datePicker.VisibleDate.HasValue)
        {
            string startTime = datePicker.VisibleDate.Date.ToString("yyyy-MM-dd 00:00:01");
            string endTime = datePicker.VisibleDate.Date.ToString("yyyy-MM-dd 23:59:59");
            int machineID = machineList_[dropdown_machine.value].ID;
            probeList_ = ProbeDAL.SelectIDProbeNameGasKindWithMachineID(machineID);
            if (probeList_.Count > 0)
            {
                float firstAlarmValue = FormatData.gasKindFormat[probeList_[0].GasKind].minValue;
                historyDataList_ = HistoryDataDAL.SelectHistoryDataForChart(machineID, firstAlarmValue, startTime, endTime);
                historyDataList_.ForEach(it =>
                {
                    ProbeModel model = probeList_.Find(temp => temp.ID == it.ProbeID);
                    if (model != null)
                    {
                        it.probeName = model.ProbeName;
                    }
                });
                InitChart();
            }
        }
    }

    void InitChart()
    {
        if (probeList_ == null || probeList_.Count == 0 || historyDataList_ == null || historyDataList_.Count == 0)
            return;
        int index = dropdown_chart.value;
        simplifiedLineChart.gameObject.SetActive(index == 0);
        pieChart.gameObject.SetActive(index == 1);
        scatterChart.gameObject.SetActive(index == 2);
        if (index == 0)
        {
            InitSimplifiedLineChart();
        }
        if (index == 1)
        {
            InitPieChart();
        }
        if (index == 2)
        {
            InitScatterChart();
        }
    }

    void InitPieChart()
    {
        pieChart.RemoveAllSerie();
        pieChart.ClearData();
        Dictionary<string, int> probeNameCount = new Dictionary<string, int>();
        probeList_.ForEach((it) =>
        {
            if (!probeNameCount.ContainsKey(it.ProbeName))
            {
                probeNameCount[it.ProbeName] = 0;
            }
        });
        historyDataList_.ForEach((it) =>
        {
            if (probeNameCount.ContainsKey(it.probeName))
            {
                probeNameCount[it.probeName] = probeNameCount[it.probeName] + 1;
            }
        });
        Pie pie = pieChart.AddSerie<Pie>();
        foreach (var item in probeNameCount)
        {
            SerieData serieData = new SerieData();
            serieData.name = item.Key;
            serieData.data = new List<double>() { 0, item.Value, 3 };
            pie.AddSerieData(serieData);
        }
        pieChart.AddChartComponentWhenNoExist<Legend>();
    }

    float validDataSpanMinutes = 10;//10分钟
    void InitSimplifiedLineChart()
    {
        simplifiedLineChart.RemoveAllSerie();
        simplifiedLineChart.ClearData();
        Dictionary<string, List<ChartSeriesModel>> probeNameValue = new Dictionary<string, List<ChartSeriesModel>>();
        probeList_.ForEach((it) =>
        {
            if (!probeNameValue.ContainsKey(it.ProbeName))
            {
                probeNameValue[it.ProbeName] = new List<ChartSeriesModel>();
            }
        });

        List<string> timeList = new List<string>();
        timeList.Add(datePicker.VisibleDate.Date.ToString("MM-dd 00:00"));
        DateTime preTime = historyDataList_[0].CheckTime;
        historyDataList_.ForEach((it) =>
        {
            if (probeNameValue.ContainsKey(it.probeName))
            {
                if ((it.CheckTime - preTime).TotalMinutes >= validDataSpanMinutes)
                {
                    preTime = it.CheckTime;
                    ChartSeriesModel model = new ChartSeriesModel();
                    model.checkTime = it.CheckTime.ToString("MM-dd HH:mm");
                    model.gasValue = it.GasValue;
                    probeNameValue[it.probeName].Add(model);
                    if (!timeList.Contains(it.CheckTime.ToString("MM-dd HH:mm")))
                    {
                        timeList.Add(it.CheckTime.ToString("MM-dd HH:mm"));
                    }
                }
            }
        });
        timeList.Add(datePicker.VisibleDate.Date.ToString("MM-dd 23:59"));
        timeList.Sort();
        XAxis xAxis = simplifiedLineChart.GetChartComponent<XAxis>();
        xAxis.ClearData();
        timeList.ForEach((it) =>
        {
            xAxis.AddData(it);
        });

        double maxValue = 0;
        foreach (var item in probeNameValue)
        {
            SimplifiedLine simplifiedLine = simplifiedLineChart.AddSerie<SimplifiedLine>(item.Key);
            for (int i = 0; i < timeList.Count; i++)
            {
                SerieData serieData = new SerieData();
                ChartSeriesModel model = item.Value.Find(temp =>
                {
                    return temp.checkTime.Equals(timeList[i]);
                });
                double value = model == null ? 0 : model.gasValue;
                value = value > 2000 ? 2000 : value;
                serieData.data = new List<double>() { i, value };
                //simplifiedLine.show = false;
                simplifiedLine.AddSerieData(serieData);
                if (value > maxValue)
                {
                    maxValue = value;
                }
            }
        }
        YAxis yAxis = simplifiedLineChart.GetChartComponent<YAxis>();
        yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
        yAxis.min = 0;
        yAxis.max = maxValue;
        simplifiedLineChart.AddChartComponentWhenNoExist<Legend>();
    }

    void InitScatterChart()
    {
        scatterChart.RemoveAllSerie();
        scatterChart.ClearData();
        Dictionary<string, List<ChartSeriesModel>> probeNameValue = new Dictionary<string, List<ChartSeriesModel>>();
        probeList_.ForEach((it) =>
        {
            if (!probeNameValue.ContainsKey(it.ProbeName))
            {
                probeNameValue[it.ProbeName] = new List<ChartSeriesModel>();
            }
        });

        List<string> timeList = new List<string>();
        timeList.Add(datePicker.VisibleDate.Date.ToString("MM-dd 00:00"));
        DateTime preTime = historyDataList_[0].CheckTime;
        historyDataList_.ForEach((it) =>
        {
            if (probeNameValue.ContainsKey(it.probeName))
            {
                if ((it.CheckTime - preTime).TotalMinutes >= validDataSpanMinutes)
                {
                    ChartSeriesModel model = new ChartSeriesModel();
                    model.checkTime = it.CheckTime.ToString("MM-dd HH:mm");
                    model.gasValue = it.GasValue;
                    probeNameValue[it.probeName].Add(model);
                    if (!timeList.Contains(it.CheckTime.ToString("MM-dd HH:mm")))
                    {
                        timeList.Add(it.CheckTime.ToString("MM-dd HH:mm"));
                    }
                }
            }
        });
        timeList.Add(datePicker.VisibleDate.Date.ToString("MM-dd 23:59"));
        timeList.Sort();
        XAxis xAxis = scatterChart.GetChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Category;
        xAxis.ClearData();
        timeList.ForEach((it) =>
        {
            xAxis.AddData(it);
        });

        double maxValue = 0;
        foreach (var item in probeNameValue)
        {
            Scatter scatter = scatterChart.AddSerie<Scatter>(item.Key);
            scatter.symbol.type = SymbolType.Triangle;
            scatter.symbol.size = 10;
            for (int i = 0; i < timeList.Count; i++)
            {
                SerieData serieData = new SerieData();
                ChartSeriesModel model = item.Value.Find(temp =>
                {
                    return temp.checkTime.Equals(timeList[i]);
                });
                double value = model == null ? 0 : model.gasValue;
                value = value > 2000 ? 2000 : value;
                serieData.name = model == null ? string.Empty : model.checkTime;
                serieData.data = new List<double>() { i, value };
                //scatter.show = false;
                scatter.AddSerieData(serieData);
                if (value > maxValue)
                {
                    maxValue = value;
                }
            }
        }
        YAxis yAxis = scatterChart.GetChartComponent<YAxis>();
        yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
        yAxis.min = 0;
        yAxis.max = maxValue;
        scatterChart.AddChartComponentWhenNoExist<Legend>();
    }
}

public class ChartSeriesModel
{
    public string checkTime;
    public double gasValue;
}
