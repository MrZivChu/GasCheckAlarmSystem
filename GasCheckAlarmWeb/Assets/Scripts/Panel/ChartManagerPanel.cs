using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ChartManagerPanel : MonoBehaviour
{
    void OnEnable()
    {
        int pageCount = 0, rowCount = 0;
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllHistoryDataByCondition");
        form.AddField("pageIndex", 1);
        form.AddField("pageSize", 500);
        form.AddField("startTime", System.DateTime.MinValue.ToString());
        form.AddField("endTime", System.DateTime.MinValue.ToString());
        form.AddField("pageCount", pageCount);
        form.AddField("rowCount", rowCount);
        GameUtils.PostHttpWebRequest("HistoryData.ashx", form, (result) =>
        {
            string content = Encoding.UTF8.GetString(result);
            content = content.Split('*')[1];
            List<HistoryDataModel> list = JsonMapper.ToObject<List<HistoryDataModel>>(content);
        }, null);
    }
}
