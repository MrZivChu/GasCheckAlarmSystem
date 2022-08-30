using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class ProbeDAL
{
    public static List<ProbeModel> SelectAllProbeByCondition(string probeName = "", string gasKind = "")
    {
        string sql = @"select ID,MailAddress,ProbeName,GasKind,Unit,FirstAlarmValue,SecondAlarmValue,MachineName,MachineID,PosDir,FactoryID,FactoryName,MachineType,Pos2D,SerialNumber,TagName from Probe where 1=1 ";
        if (!string.IsNullOrEmpty(probeName))
            sql += " and ProbeName like '%" + probeName + "%' ";
        if (!string.IsNullOrEmpty(gasKind))
            sql += " and GasKind like '%" + gasKind + "%' ";
        sql += " order by MachineID";
        List<ProbeModel> modelList = new List<ProbeModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProbeModel model = new ProbeModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.MailAddress = dt.Rows[i]["MailAddress"].ToString();
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.GasKind = dt.Rows[i]["GasKind"].ToString();
                model.Unit = dt.Rows[i]["Unit"].ToString();
                model.FirstAlarmValue = Convert.ToSingle(dt.Rows[i]["FirstAlarmValue"]);
                model.SecondAlarmValue = Convert.ToSingle(dt.Rows[i]["SecondAlarmValue"]);
                model.MachineName = dt.Rows[i]["MachineName"].ToString();
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
                model.PosDir = dt.Rows[i]["PosDir"].ToString();
                model.FactoryID = Convert.ToInt32(dt.Rows[i]["FactoryID"]);
                model.FactoryName = dt.Rows[i]["FactoryName"].ToString();
                model.MachineType = Convert.ToInt32(dt.Rows[i]["MachineType"]);
                model.Pos2D = dt.Rows[i]["Pos2D"].ToString();
                model.SerialNumber = dt.Rows[i]["SerialNumber"].ToString();
                model.TagName = dt.Rows[i]["TagName"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static ProbeModel SelectProbeByID(int id)
    {
        string sql = @"select ID,MailAddress,ProbeName,GasKind,Unit,FirstAlarmValue,SecondAlarmValue,MachineName,MachineID,PosDir,FactoryID,FactoryName,MachineType,Pos2D,SerialNumber,TagName from Probe where ID = @ID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id)
            };
        ProbeModel model = new ProbeModel();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter);
        if (dt.Rows.Count > 0)
        {
            model.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
            model.MailAddress = dt.Rows[0]["MailAddress"].ToString();
            model.ProbeName = dt.Rows[0]["ProbeName"].ToString();
            model.GasKind = dt.Rows[0]["GasKind"].ToString();
            model.Unit = dt.Rows[0]["Unit"].ToString();
            model.FirstAlarmValue = Convert.ToSingle(dt.Rows[0]["FirstAlarmValue"]);
            model.SecondAlarmValue = Convert.ToSingle(dt.Rows[0]["SecondAlarmValue"]);
            model.MachineName = dt.Rows[0]["MachineName"].ToString();
            model.MachineID = Convert.ToInt32(dt.Rows[0]["MachineID"]);
            model.PosDir = dt.Rows[0]["PosDir"].ToString();
            model.FactoryID = Convert.ToInt32(dt.Rows[0]["FactoryID"]);
            model.FactoryName = dt.Rows[0]["FactoryName"].ToString();
            model.MachineType = Convert.ToInt32(dt.Rows[0]["MachineType"]);
            model.Pos2D = dt.Rows[0]["Pos2D"].ToString();
            model.SerialNumber = dt.Rows[0]["SerialNumber"].ToString();
            model.TagName = dt.Rows[0]["TagName"].ToString();
        }
        return model;
    }

    public static ProbeModel SelectProbeBySerialNumber(string serialNumber)
    {
        string sql = @"select ID,MailAddress,ProbeName,GasKind,Unit,FirstAlarmValue,SecondAlarmValue,MachineName,MachineID,PosDir,FactoryID,FactoryName,MachineType,Pos2D,SerialNumber,TagName from Probe where SerialNumber = @SerialNumber";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@SerialNumber",serialNumber)
            };
        ProbeModel model = new ProbeModel();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter);
        if (dt.Rows.Count > 0)
        {
            model.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
            model.MailAddress = dt.Rows[0]["MailAddress"].ToString();
            model.ProbeName = dt.Rows[0]["ProbeName"].ToString();
            model.GasKind = dt.Rows[0]["GasKind"].ToString();
            model.Unit = dt.Rows[0]["Unit"].ToString();
            model.FirstAlarmValue = Convert.ToSingle(dt.Rows[0]["FirstAlarmValue"]);
            model.SecondAlarmValue = Convert.ToSingle(dt.Rows[0]["SecondAlarmValue"]);
            model.MachineName = dt.Rows[0]["MachineName"].ToString();
            model.MachineID = Convert.ToInt32(dt.Rows[0]["MachineID"]);
            model.PosDir = dt.Rows[0]["PosDir"].ToString();
            model.FactoryID = Convert.ToInt32(dt.Rows[0]["FactoryID"]);
            model.FactoryName = dt.Rows[0]["FactoryName"].ToString();
            model.MachineType = Convert.ToInt32(dt.Rows[0]["MachineType"]);
            model.Pos2D = dt.Rows[0]["Pos2D"].ToString();
            model.SerialNumber = dt.Rows[0]["SerialNumber"].ToString();
            model.TagName = dt.Rows[0]["TagName"].ToString();
        }
        return model;
    }

    public static bool DeleteProbeByID(string idList)
    {
        string sql = @"delete from Probe where ID in (" + idList + @");
        delete from RealtimeData where ProbeID in (" + idList + @");
        ";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }

    public static bool EditProbeByID(int id, string mailAddress, string probeName, int machineID, string gasKind, string unit, string firstAlarmValue, string secondAlarmValue, string machineName, string tagName, string serialNumber)
    {
        //没必要更新历史数据
        string sql = @"update Probe set MailAddress=@MailAddress,ProbeName=@ProbeName,GasKind=@GasKind,Unit=@Unit,FirstAlarmValue=@FirstAlarmValue,SecondAlarmValue=@SecondAlarmValue,MachineID=@MachineID,MachineName=@MachineName,SerialNumber=@SerialNumber,TagName=@TagName where ID=@ID;
          update RealtimeData set ProbeName=@ProbeName,GasKind=@GasKind,Unit=@Unit,FirstAlarmValue=@FirstAlarmValue,SecondAlarmValue=@SecondAlarmValue,MachineID=@MachineID,MachineName=@MachineName,TagName=@TagName where ProbeID=@ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@MailAddress",mailAddress),
                 new SqlParameter("@ProbeName",probeName),
                 new SqlParameter("@GasKind",gasKind),
                 new SqlParameter("@Unit",unit),
                 new SqlParameter("@FirstAlarmValue",firstAlarmValue),
                 new SqlParameter("@SecondAlarmValue",secondAlarmValue),
                 new SqlParameter("@MachineID",machineID),
                 new SqlParameter("@MachineName",machineName),
                 new SqlParameter("@SerialNumber",serialNumber),
                 new SqlParameter("@TagName",tagName),
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool EditProbePosDirByID(int id, string posDir)
    {
        //没必要更新历史数据
        string sql = @"update Probe set PosDir=@PosDir where ID=@ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@PosDir",posDir)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static int InsertProbe(string mailAddress, string probeName, string gasKind, string unit, string firstAlarmValue, string secondAlarmValue, string posdir, int machineID, string machineName, int factoryID, string factoryName, int machineType, string tagName, string serialNumber)
    {
        string sql = @"insert into Probe (MailAddress,ProbeName,GasKind,Unit,FirstAlarmValue,SecondAlarmValue,MachineName,MachineID,PosDir,FactoryID,FactoryName,MachineType,SerialNumber,TagName)values(@MailAddress,@ProbeName,@GasKind,@Unit,@FirstAlarmValue,@SecondAlarmValue,@MachineName,@MachineID,@PosDir,@FactoryID,@FactoryName,@MachineType,@SerialNumber,@TagName) SELECT @@IDENTITY AS ID;";
        List<SqlParameter> parameter = new List<SqlParameter>{
                 new SqlParameter("@MailAddress",mailAddress),
                 new SqlParameter("@ProbeName",probeName),
                 new SqlParameter("@GasKind",gasKind),
                 new SqlParameter("@Unit",unit),
                 new SqlParameter("@FirstAlarmValue",firstAlarmValue),
                 new SqlParameter("@SecondAlarmValue",secondAlarmValue),
                 new SqlParameter("@MachineName",machineName),
                 new SqlParameter("@MachineID",machineID),
                 new SqlParameter("@PosDir",posdir),
                 new SqlParameter("@FactoryID",factoryID),
                 new SqlParameter("@FactoryName",factoryName),
                 new SqlParameter("@MachineType",machineType),
                 new SqlParameter("@SerialNumber",serialNumber),
                 new SqlParameter("@TagName",tagName),
            };
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter.ToArray());
        int insertIndex = Convert.ToInt32(dt.Rows[0][0]);
        sql = @"insert into RealtimeData (ProbeID,ProbeName,GasKind,Unit,FirstAlarmValue,SecondAlarmValue,MachineName,MachineID,FactoryID,FactoryName,MachineType,GasValue,TagName)values(@ProbeID,@ProbeName,@GasKind,@Unit,@FirstAlarmValue,@SecondAlarmValue,@MachineName,@MachineID,@FactoryID,@FactoryName,@MachineType,0,@TagName)";
        parameter.Add(new SqlParameter("@ProbeID", insertIndex));
        SqlHelper.ExecuteNonQuery(sql, parameter.ToArray());
        return insertIndex;
    }

}
