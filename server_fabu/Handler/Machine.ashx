<%@ WebHandler Language="C#" Class="Machine" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

public class Machine : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string content = string.Empty;
        string requestType = context.Request["requestType"];
        if (requestType == "SelectAllMachineByCondition")
        {
            string machineName = context.Request["machineName"];
            string factoryIDStr = context.Request["factoryID"];
            int factoryID = -1;
            bool success = int.TryParse(factoryIDStr, out factoryID);
            if (!success)
            {
                factoryID = -1;
            }
            List<MachineModel> list = MachineDAL.SelectAllMachineByCondition(machineName, factoryID);
            content = JsonConvert.SerializeObject(list);
        }
        else if (requestType == "DeleteMachineByID")
        {
            string idList = context.Request["idList"];
            MachineDAL.DeleteMachineByID(idList);
        }
        else if (requestType == "EditMachineByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            string mailAddress = context.Request["mailAddress"];
            string machineName = context.Request["machineName"];
            int factoryID = Convert.ToInt32(context.Request["factoryID"]);
            string factoryName = context.Request["factoryName"];
            MachineDAL.EditMachineByID(id, mailAddress, machineName, factoryID, factoryName);
        }
        else if (requestType == "InsertMachine")
        {
            string mailAddress = context.Request["mailAddress"];
            string machineName = context.Request["machineName"];
            int factoryID = Convert.ToInt32(context.Request["factoryID"]);
            string factoryName = context.Request["factoryName"];
            int machineProtocol = Convert.ToInt32(context.Request["machineProtocol"]);
            MachineDAL.InsertMachine(mailAddress, machineName, factoryID, factoryName, machineProtocol);
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