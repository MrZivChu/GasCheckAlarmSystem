<%@ WebHandler Language="C#" Class="Probe" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.IO;

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
            content = JsonConvert.SerializeObject(list);
        }
        else if (requestType == "SelectProbeByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            ProbeModel model = ProbeDAL.SelectProbeByID(id);
            content = JsonConvert.SerializeObject(model);
        }
        else if (requestType == "SelectProbeBySerialNumber")
        {
            string serialNumber = context.Request["serialNumber"];
            ProbeModel model = ProbeDAL.SelectProbeBySerialNumber(serialNumber);
            content = JsonConvert.SerializeObject(model);
        }
        else if (requestType == "SelectIDProbeNameTagName")
        {
            List<ProbeModel> model = ProbeDAL.SelectIDProbeNameTagName();
            content = JsonConvert.SerializeObject(model);
        }
        else if (requestType == "SelectIDProbeNameMachineIDPos2DWherePos2DHasValue")
        {
            List<ProbeModel> model = ProbeDAL.SelectIDProbeNameMachineIDPos2DWherePos2DHasValue();
            content = JsonConvert.SerializeObject(model);
        }
        else if (requestType == "SelectIDCheckTimeGasValueGasKindMachineID")
        {
            List<ProbeModel> model = ProbeDAL.SelectIDCheckTimeGasValueGasKindMachineID();
            content = JsonConvert.SerializeObject(model);
        }
        else if (requestType == "SelectIDProbeNameGasKindMachineID")
        {
            List<ProbeModel> model = ProbeDAL.SelectIDProbeNameGasKindMachineID();
            content = JsonConvert.SerializeObject(model);
        }
        else if (requestType == "DeleteProbeByID")
        {
            string idList = context.Request["idList"];
            ProbeDAL.DeleteProbeByID(idList);
        }
        else if (requestType == "DeleteProbePos2DByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            ProbeDAL.DeleteProbePos2DByID(id);
        }
        else if (requestType == "EditProbePos2DByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            string pos2D = context.Request["pos2D"].ToString();
            ProbeDAL.EditProbePos2DByID(id, pos2D);
        }
        else if (requestType == "UploadPlanarGraphFile")
        {
            string filePath = context.Server.MapPath("../PlanarGraph/");
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
                content = ff.FileName;
            }
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