<%@ WebHandler Language="C#" Class="Login" %>

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
        if (requestType == "DeleteCameraByID")
        {
            string ID = context.Request["id"];
            CameraDAL.DeleteCameraByID(ID);
        }
        else if (requestType == "EditCameraByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            string ip = context.Request["ip"];
            string port = context.Request["port"];
            string userName = context.Request["userName"];
            string userPwd = context.Request["userPwd"];
            string machineAddress = context.Request["machineAddress"];
            string gasInfos = context.Request["gasInfos"];
            string gasValues = context.Request["gasValues"];
            CameraDAL.EditCameraByID(id, ip, port, userName, userPwd, machineAddress, gasInfos, gasValues);
        }
        else if (requestType == "InsertCameraBaseData")
        {
            string ip = context.Request["ip"];
            string port = context.Request["port"];
            string userName = context.Request["userName"];
            string userPwd = context.Request["userPwd"];
            CameraDAL.InsertCameraBaseData(ip, port, userName, userPwd);
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