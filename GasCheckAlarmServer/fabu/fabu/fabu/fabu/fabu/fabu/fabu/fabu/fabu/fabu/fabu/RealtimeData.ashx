<%@ WebHandler Language="C#" Class="RealtimeData" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Newtonsoft.Json;

public class RealtimeData : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string content = string.Empty;
        string requestType = context.Request["requestType"];
        if (requestType == "SelectAllMachineByCondition")
        {
            string probeName = context.Request["probeName"];
            string gasKind = context.Request["gasKind"];
            List<RealtimeDataModel> list = RealtimeDataDAL.SelectAllRealtimeDataByCondition(probeName, gasKind);
            if (list.Count > 0)
            {
                content = JsonConvert.SerializeObject(list);
            }
        }
        else if (requestType == "EditRealtimePos2DByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            string pos2D = context.Request["pos2D"];
            RealtimeDataDAL.EditRealtimePos2DByID(id, pos2D);
        }
        else if (requestType == "DeleteRealtimePos2DByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            RealtimeDataDAL.DeleteRealtimePos2DByID(id);
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