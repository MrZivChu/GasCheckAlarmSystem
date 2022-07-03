using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class MachineDAL
{
    public static List<ProbeSerialPortInfo> SelectAllMachineSerialPortInfo()
    {
        string sql = @"select p.FactoryName,p.FactoryID,p.ProbeName,m.ID as MachineID,m.MailAddress as MachineAddress,m.MachineName,m.MachineType,p.ProbeName,p.ID as ProbeID,p.MailAddress as ProbeAddress,p.FirstAlarmValue,p.SecondAlarmValue,p.GasKind
        from Machine as m
        inner join Probe as p
        on m.ID = p.MachineID";
        List<ProbeSerialPortInfo> modelList = new List<ProbeSerialPortInfo>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProbeSerialPortInfo model = new ProbeSerialPortInfo();
                model.ProbeID = Convert.ToInt32(dt.Rows[i]["ProbeID"].ToString());
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
                model.MachineName = dt.Rows[i]["MachineName"].ToString();
                model.MachineType = Convert.ToInt32(dt.Rows[i]["MachineType"]);
                model.FactoryID = Convert.ToInt32(dt.Rows[i]["FactoryID"]);
                model.FactoryName = dt.Rows[i]["FactoryName"].ToString();
                model.MachineAddress = dt.Rows[i]["MachineAddress"].ToString();
                model.ProbeAddress = dt.Rows[i]["ProbeAddress"].ToString();
                model.FirstAlarmValue = Convert.ToSingle(dt.Rows[i]["FirstAlarmValue"].ToString());
                model.SecondAlarmValue = Convert.ToSingle(dt.Rows[i]["SecondAlarmValue"].ToString());
                model.GasKind = dt.Rows[i]["GasKind"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }
}
