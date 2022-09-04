<%@ WebHandler Language="C#" Class="WaterSeal" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.IO;

public class WaterSeal : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string content = string.Empty;
        try
        {
            string requestType = context.Request["requestType"];
            if (requestType == "SelectAllWaterSealByCondition")
            {
                List<WaterSealModel> list = WaterSealDAL.SelectAllWaterSealByCondition();
                content = JsonConvert.SerializeObject(list);
            }
            else if (requestType == "SelectWaterSealByID")
            {
                int id = Convert.ToInt32(context.Request["id"]);
                WaterSealModel model = WaterSealDAL.SelectWaterSealByID(id);
                content = JsonConvert.SerializeObject(model);
            }
            else if (requestType == "SelectWaterSealBySerialNumber")
            {
                string serialNumber = context.Request["serialNumber"];
                WaterSealModel model = WaterSealDAL.SelectWaterSealBySerialNumber(serialNumber);
                content = JsonConvert.SerializeObject(model);
            }
            else if (requestType == "DeleteWaterSealByID")
            {
                string idList = context.Request["idList"];
                WaterSealDAL.DeleteWaterSealByID(idList);
            }
            else if (requestType == "EditWaterSealByID")
            {
                int id = Convert.ToInt32(context.Request["id"]);
                string medium = context.Request["medium"];
                string number = context.Request["number"];
                string installPosition = context.Request["installPosition"];
                string category = context.Request["category"];
                int designPressure = Convert.ToInt32(context.Request["designPressure"]);
                string serialNumber = context.Request["serialNumber"];
                WaterSealDAL.EditWaterSealByID(id, medium, number, installPosition, category, designPressure, serialNumber);
            }
            else if (requestType == "InsertWaterSeal")
            {
                string medium = context.Request["medium"];
                string number = context.Request["number"];
                string installPosition = context.Request["installPosition"];
                string category = context.Request["category"];
                int designPressure = Convert.ToInt32(context.Request["designPressure"]);
                string serialNumber = context.Request["serialNumber"];
                int id = WaterSealDAL.InsertWaterSeal(medium, number, installPosition, category, designPressure, serialNumber);
                content = id.ToString();
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