<%@ WebHandler Language="C#" Class="Factory" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

public class Factory : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string content = string.Empty;
        string requestType = context.Request["requestType"];
        if (requestType == "SelectAllFactoryByCondition")
        {
            string factoryName = context.Request["factoryName"];
            List<FactoryModel> list = FactoryDAL.SelectAllFactoryByCondition(factoryName);
            if (list.Count > 0)
            {
                content = JsonConvert.SerializeObject(list);
            }
        }
        else if (requestType == "DeleteFactoryByID")
        {
            string idList = context.Request["idList"];
            FactoryDAL.DeleteFactoryByID(idList);
        }
        else if (requestType == "EditFactoryByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            string factoryName = context.Request["factoryName"];
            FactoryDAL.EditFactoryByID(id, factoryName);
        }
        else if (requestType == "InsertFactory")
        {
            string factoryName = context.Request["factoryName"];
            FactoryDAL.InsertFactory(factoryName);
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