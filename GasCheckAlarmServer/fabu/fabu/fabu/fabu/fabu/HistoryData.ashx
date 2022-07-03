<%@ WebHandler Language="C#" Class="HistoryData" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

public class HistoryData : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string content = string.Empty;
        string requestType = context.Request["requestType"];
        if (requestType == "SelectAllHistoryDataByCondition")
        {
            int pageIndex = Convert.ToInt32(context.Request["pageIndex"]);
            int pageSize = Convert.ToInt32(context.Request["pageSize"]);
            string probeName = context.Request["probeName"];
            string gasKind = context.Request["gasKind"];
            DateTime startTime = Convert.ToDateTime(context.Request["startTime"]);
            DateTime endTime = Convert.ToDateTime(context.Request["endTime"]);
            int pageCount = Convert.ToInt32(context.Request["pageCount"]);
            int rowCount = Convert.ToInt32(context.Request["rowCount"]);
            List<HistoryDataModel> list = HistoryDataDAL.SelectAllHistoryDataByCondition(pageIndex, pageSize, probeName, gasKind, startTime, endTime, out pageCount, out rowCount);
            if (list.Count > 0)
            {
                string data = JsonConvert.SerializeObject(list);
                content = pageCount + "," + rowCount + "*" + data;
            }
        }
        else if (requestType == "DeleteHistoryDataByID")
        {
            string idList = context.Request["idList"];
            HistoryDataDAL.DeleteHistoryDataByID(idList);
        }
        else if (requestType == "DeleteHistoryDataBeforeWeek")
        {
            HistoryDataDAL.DeleteHistoryDataBeforeWeek();
        }
        else if (requestType == "DeleteAllHistoryData")
        {
            HistoryDataDAL.DeleteAllHistoryData();
        }
        context.Response.Write(content);
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}