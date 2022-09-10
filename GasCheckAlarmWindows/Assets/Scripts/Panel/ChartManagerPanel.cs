using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts;

public class ChartManagerPanel : MonoBehaviour
{
    public LineChart chart;
    void OnEnable()
    {
        chart.RemoveData();
        int pageCount = 0, rowCount = 0;
        List<HistoryDataModel> list = HistoryDataDAL.SelectAllHistoryDataByCondition(1, 500, string.Empty, string.Empty, string.Empty, string.Empty, out pageCount, out rowCount);
        if (list != null && list.Count > 0)
        {
            chart.legend.itemWidth = 55;
            chart.legend.itemHeight = 25;
            chart.legend.location.top = -130;
            chart.legend.location.left = -10;
            List<string> probeNameList = new List<string>();
            Dictionary<string, List<float>> dic = new Dictionary<string, List<float>>();
            DateTime minTime = list[0].CheckTime;
            DateTime maxTime = list[0].CheckTime;
            for (int i = 0; i < list.Count; i++)
            {
                HistoryDataModel model = list[i];
                string key = model.MachineName + "-" + model.ProbeName;
                if (!probeNameList.Contains(key))
                {
                    probeNameList.Add(key);
                }
                float gasValue = (float)model.GasValue;

                if (dic.ContainsKey(key))
                {
                    dic[key].Add(gasValue);
                }
                else
                {
                    dic.Add(key, new List<float>() { (float)model.GasValue });
                }
                if (model.CheckTime < minTime)
                {
                    minTime = model.CheckTime;
                }
                else if (model.CheckTime > maxTime)
                {
                    maxTime = model.CheckTime;
                }
            }
            for (int i = 0; i < probeNameList.Count; i++)
            {
                chart.series.AddSerie(SerieType.Line, probeNameList[i]);
                Serie serie = chart.series.GetSerie(probeNameList[i]);
                serie.sampleType = SampleType.Peak;
                List<float> valueList = dic[probeNameList[i]];
                for (int j = 0; j < valueList.Count; j++)
                {
                    serie.AddYData(valueList[j]);
                }
            }
            int showXCount = 30;
            TimeSpan ts = maxTime - minTime;
            long averageTicks = ts.Ticks / showXCount;
            for (int i = 0; i < showXCount; i++)
            {
                DateTime result = minTime.Add(new TimeSpan(averageTicks * i));
                chart.xAxis0.AddData(result.ToString("MM-dd HH:mm:ss"));
            }
        }
    }
}
