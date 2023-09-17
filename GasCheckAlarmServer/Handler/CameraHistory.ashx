<%@ WebHandler Language="C#" Class="CameraHistory" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

public class CameraHistory : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        SqlHelper.InitSqlConnection("hds16173015.my3w.com", "hds16173015_db", "hds16173015", "!@#Dz123");

        string content = string.Empty;
        string requestType = context.Request["requestType"];
        if (requestType == "Insert")
        {
            string sql = context.Request["sql"];
            CameraHistoryDAL.Insert(sql);
        }
        else if (requestType == "DeleteHistoryData")
        {
            int timeStamp = Convert.ToInt32(context.Request["timeStamp"]);
            CameraHistoryDAL.DeleteHistoryData(timeStamp);
        }
        else if (requestType == "SelectOneNeweastData")
        {
            string count = CameraHistoryDAL.SelectOneNeweastData();
            content = string.IsNullOrEmpty(count) ? "0" : count;
        }
        else if (requestType == "InsertSingleData")
        {
            string androidID = Convert.ToString(context.Request["androidID"]);
            int timeStamp = Convert.ToInt32(context.Request["timeStamp"]);
            string gasValues = Convert.ToString(context.Request["gasValues"]);
            CameraHistoryDAL.InsertSingleData(androidID, timeStamp, gasValues);
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