<%@ WebHandler Language="C#" Class="Camera" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

public class Camera : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        SqlHelper.InitSqlConnection("hds16173015.my3w.com", "hds16173015_db", "hds16173015", "!@#Dz123");

        string content = string.Empty;
        string requestType = context.Request["requestType"];
        if (requestType == "InsertGasBaseData")
        {
            string androidID = context.Request["androidID"];
            string machineID = context.Request["machineID"];
            string gases = context.Request["gases"];
            CameraDAL.InsertGasBaseData(androidID, machineID, gases);
        }
        else if (requestType == "InsertCameraBaseData")
        {
            string androidID = context.Request["androidID"];
            string ip = context.Request["ip"];
            string port = context.Request["port"];
            string userName = context.Request["userName"];
            string userPwd = context.Request["userPwd"];
            CameraDAL.InsertCameraBaseData(androidID, ip, port, userName, userPwd);
        }
        else if (requestType == "UpdateRealtimeGasData")
        {
            string androidID = context.Request["androidID"];
            string gasValues = context.Request["gasValues"];
            CameraDAL.UpdateRealtimeGasData(androidID, gasValues);
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