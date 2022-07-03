<%@ WebHandler Language="C#" Class="Probe" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

public class Probe : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string content = string.Empty;
        string requestType = context.Request["requestType"];
        if (requestType == "SelectAllProbeByCondition")
        {
            string probeName = context.Request["probeName"];
            string gasKind = context.Request["gasKind"];
            List<ProbeModel> list = ProbeDAL.SelectAllProbeByCondition(probeName, gasKind);
            if (list.Count > 0)
            {
                content = JsonConvert.SerializeObject(list);
            }
        }
        else if (requestType == "SelectProbeByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            ProbeModel model = ProbeDAL.SelectProbeByID(id);
            content = JsonConvert.SerializeObject(model);
        }
        else if (requestType == "DeleteProbeByID")
        {
            string idList = context.Request["idList"];
            ProbeDAL.DeleteProbeByID(idList);
        }
        else if (requestType == "EditProbeByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            string mailAddress = context.Request["mailAddress"];
            string probeName = context.Request["probeName"];
            int machineID = Convert.ToInt32(context.Request["machineID"]);
            string gasKind = context.Request["gasKind"];
            string unit = context.Request["unit"];
            string firstAlarmValue = context.Request["firstAlarmValue"];
            string secondAlarmValue = context.Request["secondAlarmValue"];
            string machineName = context.Request["machineName"];
            ProbeDAL.EditProbeByID(id, mailAddress, probeName, machineID, gasKind, unit, firstAlarmValue, secondAlarmValue, machineName);
        }
        else if (requestType == "EditProbePosDirByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            string posDir = context.Request["posDir"];
            ProbeDAL.EditProbePosDirByID(id, posDir);
        }
        else if (requestType == "InsertProbe")
        {
            string mailAddress = context.Request["mailAddress"];
            string probeName = context.Request["probeName"];
            string gasKind = context.Request["gasKind"];
            string unit = context.Request["unit"];
            string firstAlarmValue = context.Request["firstAlarmValue"];
            string secondAlarmValue = context.Request["secondAlarmValue"];
            string posdir = context.Request["posdir"];
            int machineID = Convert.ToInt32(context.Request["machineID"]);
            string machineName = context.Request["machineName"];
            int factoryID = Convert.ToInt32(context.Request["factoryID"]);
            string factoryName = context.Request["factoryName"];
            int machineType = Convert.ToInt32(context.Request["machineType"]);
            int id = ProbeDAL.InsertProbe(mailAddress, probeName, gasKind, unit, firstAlarmValue, secondAlarmValue, posdir, machineID, machineName, factoryID, factoryName, machineType);
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