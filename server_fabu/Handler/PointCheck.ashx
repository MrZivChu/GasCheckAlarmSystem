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
        try
        {
            string requestType = context.Request["requestType"];
            if (requestType == "SelectAllPointCheckByCondition")
            {
                int pageIndex = Convert.ToInt32(context.Request["pageIndex"]);
                int pageSize = Convert.ToInt32(context.Request["pageSize"]);
                int deviceType = Convert.ToInt32(context.Request["deviceType"]);
                string userName = context.Request["userName"];
                string deviceName = context.Request["deviceName"];
                string startTime = context.Request["startTime"];
                string endTime = context.Request["endTime"];
                int pageCount = Convert.ToInt32(context.Request["pageCount"]);
                int rowCount = Convert.ToInt32(context.Request["rowCount"]);
                List<PointCheckModel> list = PointCheckDAL.SelectAllPointCheckByCondition(pageIndex, pageSize, userName, deviceName, deviceType, startTime, endTime, out pageCount, out rowCount);
                content = pageCount + "," + rowCount;
                if (list.Count > 0)
                {
                    string data = JsonConvert.SerializeObject(list);
                    content += "|" + data;
                }
            }
            else if (requestType == "InsertPointCheck")
            {
                int probeID = Convert.ToInt32(context.Request["deviceID"]);
                string probeName = context.Request["deviceName"];
                string userName = context.Request["userName"];
                string qrCodePath = context.Request["qrCodePath"];
                string description = context.Request["description"];
                string result = context.Request["result"];
                int deviceType = Convert.ToInt32(context.Request["deviceType"]);
                PointCheckDAL.InsertPointCheck(probeID, probeName, userName, qrCodePath, description, result, deviceType);
            }
            else if (requestType == "UploadFile")
            {
                string floder = DateTime.Now.ToString("yyyyMMdd");
                string filePath = context.Server.MapPath("../QrCodeImgs/") + floder + "/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                HttpFileCollection hfc = context.Request.Files;
                IList<HttpPostedFile> list = hfc.GetMultiple("file");
                for (int i = 0; i < list.Count; i++)
                {
                    HttpPostedFile ff = list[i];
                    ff.SaveAs(filePath + ff.FileName);
                    content = floder + "/" + ff.FileName;
                }
            }
        }
        catch (Exception ex)
        {
            content = ex.Message;
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