<%@ WebHandler Language="C#" Class="PointCheck" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.IO;

public class PointCheck : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string content = string.Empty;
        string requestType = context.Request["requestType"];
        if (requestType == "SelectAllPointCheckByCondition")
        {
            int pageIndex = Convert.ToInt32(context.Request["pageIndex"]);
            int pageSize = Convert.ToInt32(context.Request["pageSize"]);
            string userName = context.Request["userName"];
            string probeName = context.Request["probeName"];
            string checkTime = context.Request["checkTime"];
            int pageCount = Convert.ToInt32(context.Request["pageCount"]);
            int rowCount = Convert.ToInt32(context.Request["rowCount"]);
            List<PointCheckModel> list = PointCheckDAL.SelectAllPointCheckByCondition(pageIndex, pageSize, userName, probeName, checkTime, out pageCount, out rowCount);
            if (list.Count > 0)
            {
                string data = JsonConvert.SerializeObject(list);
                content = pageCount + "," + rowCount + "*" + data;
            }
        }
        else if (requestType == "InsertPointCheck")
        {
            SaveFile(context);
            //int probeID = Convert.ToInt32(context.Request["probeID"]);
            //string probeName = context.Request["probeName"];
            //string userName = context.Request["userName"];
            //string qrCodePath = context.Request["qrCodePath"];            
            //PointCheckDAL.InsertPointCheck(probeID, probeName, userName, qrCodePath);
        }
        context.Response.Write(content);
        context.Response.End();
    }

    void SaveFile(HttpContext context)
    {
        HttpRequest hr = context.Request;
        HttpFileCollection hfc = hr.Files;
        IList<HttpPostedFile> list = hfc.GetMultiple("file");
        for (int i = 0; i < list.Count; i++)
        {
            HttpPostedFile ff = list[i];
            ff.SaveAs("QrCodeImgs:\\" + ff.FileName);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}