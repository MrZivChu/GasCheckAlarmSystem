<%@ WebHandler Language="C#" Class="DeviceTag" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

public class DeviceTag : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string content = string.Empty;
        string requestType = context.Request["requestType"];
        if (requestType == "SelectAllDeviceTag")
        {
            List<DeviceTagModel> list = DeviceTagDAL.SelectAllDeviceTag();
            content = JsonConvert.SerializeObject(list);
        }
        else if (requestType == "DeleteDeviceTagByID")
        {
            string idList = context.Request["idList"];
            DeviceTagDAL.DeleteDeviceTagByID(idList);
        }
        else if (requestType == "EditDeviceTagByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            string position = context.Request["position"];
            DeviceTagDAL.EditDeviceTagByID(id, position);
        }
        else if (requestType == "InsertDeviceTag")
        {
            string tagName = context.Request["tagName"];
            int parentID = Convert.ToInt32(context.Request["parentID"]);
            string position = context.Request["position"];
            int id = DeviceTagDAL.InsertDeviceTag(tagName, parentID, position);
            content = id.ToString();
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