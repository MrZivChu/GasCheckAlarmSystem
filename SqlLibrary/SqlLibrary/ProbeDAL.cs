using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class ProbeDAL
{
    public static List<ProbeModel> SelectAllProbeByCondition(string probeName = "", string gasKind = "")
    {
        string sql = @"select ID,MailAddress,ProbeName,GasKind,MachineID,Pos3D,Pos2D,SerialNumber,TagName,CheckTime,GasValue from Probe where 1=1 ";
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
                model.GasKind = Convert.ToInt32(dt.Rows[i]["GasKind"]);
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
                model.Pos3D = dt.Rows[i]["Pos3D"].ToString();
                model.Pos2D = dt.Rows[i]["Pos2D"].ToString();
                model.SerialNumber = dt.Rows[i]["SerialNumber"].ToString();
                model.TagName = dt.Rows[i]["TagName"].ToString();
                model.CheckTime = Convert.ToDateTime(dt.Rows[i]["CheckTime"]);
                model.GasValue = Convert.ToSingle(dt.Rows[i]["GasValue"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<ProbeModel> SelectIDProbeNameGasKindPos3D()
    {
        string sql = @"select ID,ProbeName,Pos3D,GasKind from Probe";
        List<ProbeModel> modelList = new List<ProbeModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProbeModel model = new ProbeModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.Pos3D = dt.Rows[i]["Pos3D"].ToString();
                model.GasKind = Convert.ToInt32(dt.Rows[i]["GasKind"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<ProbeModel> SelectIDCheckTimeGasValueGasKindMachineID()
    {
        string sql = @"select ID,CheckTime,GasValue,GasKind,MachineID from Probe";
        List<ProbeModel> modelList = new List<ProbeModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProbeModel model = new ProbeModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.CheckTime = Convert.ToDateTime(dt.Rows[i]["CheckTime"]);
                model.GasValue = Convert.ToSingle(dt.Rows[i]["GasValue"]);
                model.GasKind = Convert.ToInt32(dt.Rows[i]["GasKind"]);
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<ProbeModel> SelectIDProbeNameTagName()
    {
        string sql = @"select ID,ProbeName,TagName,GasKind from Probe";
        List<ProbeModel> modelList = new List<ProbeModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProbeModel model = new ProbeModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.TagName = dt.Rows[i]["TagName"].ToString();
                model.GasKind = Convert.ToInt32(dt.Rows[i]["GasKind"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<ProbeModel> SelectIDProbeNameGasKindMachineID()
    {
        string sql = @"select ID,ProbeName,GasKind,MachineID from Probe";
        List<ProbeModel> modelList = new List<ProbeModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProbeModel model = new ProbeModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.GasKind = Convert.ToInt32(dt.Rows[i]["GasKind"]);
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<ProbeModel> SelectIDProbeNameMachineIDPos2DWherePos2DHasValue()
    {
        string sql = @"select ID,ProbeName,MachineID,Pos2D from Probe where Pos2D != ''";
        List<ProbeModel> modelList = new List<ProbeModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProbeModel model = new ProbeModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
                model.Pos2D = dt.Rows[i]["Pos2D"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<ProbeModel> SelectIDProbeNameMachineIDWherePos2DNoValue()
    {
        string sql = @"select ID,ProbeName,MachineID from Probe where Pos2D is null or Pos2D = ''";
        List<ProbeModel> modelList = new List<ProbeModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProbeModel model = new ProbeModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static ProbeModel SelectProbeByID(int id)
    {
        string sql = @"select ID,MailAddress,ProbeName,GasKind,MachineID,Pos3D,Pos2D,SerialNumber,TagName,CheckTime,GasValue from Probe where ID = @ID";
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
            model.GasKind = Convert.ToInt32(dt.Rows[0]["GasKind"]);
            model.MachineID = Convert.ToInt32(dt.Rows[0]["MachineID"]);
            model.Pos3D = dt.Rows[0]["Pos3D"].ToString();
            model.Pos2D = dt.Rows[0]["Pos2D"].ToString();
            model.SerialNumber = dt.Rows[0]["SerialNumber"].ToString();
            model.TagName = dt.Rows[0]["TagName"].ToString();
            model.CheckTime = Convert.ToDateTime(dt.Rows[0]["CheckTime"]);
            model.GasValue = Convert.ToSingle(dt.Rows[0]["GasValue"]);
        }
        return model;
    }

    public static ProbeModel SelectProbeBySerialNumber(string serialNumber)
    {
        string sql = @"select ID,MailAddress,ProbeName,GasKind,MachineID,Pos3D,Pos2D,SerialNumber,TagName,CheckTime,GasValue from Probe where SerialNumber = @SerialNumber";
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
            model.GasKind = Convert.ToInt32(dt.Rows[0]["GasKind"]);
            model.MachineID = Convert.ToInt32(dt.Rows[0]["MachineID"]);
            model.Pos3D = dt.Rows[0]["Pos3D"].ToString();
            model.Pos2D = dt.Rows[0]["Pos2D"].ToString();
            model.SerialNumber = dt.Rows[0]["SerialNumber"].ToString();
            model.TagName = dt.Rows[0]["TagName"].ToString();
            model.CheckTime = Convert.ToDateTime(dt.Rows[0]["CheckTime"]);
            model.GasValue = Convert.ToSingle(dt.Rows[0]["GasValue"]);
        }
        return model;
    }

    public static bool DeleteProbeByID(string idList)
    {
        string sql = @"delete from Probe where ID in (" + idList + @");";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }

    public static bool EditProbeByID(int id, string mailAddress, string probeName, int machineID, int gasKind, string tagName, string serialNumber)
    {
        string sql = @"update Probe set MailAddress=@MailAddress,ProbeName=@ProbeName,GasKind=@GasKind,MachineID=@MachineID,SerialNumber=@SerialNumber,TagName=@TagName where ID=@ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@MailAddress",mailAddress),
                 new SqlParameter("@ProbeName",probeName),
                 new SqlParameter("@GasKind",gasKind),
                 new SqlParameter("@MachineID",machineID),
                 new SqlParameter("@SerialNumber",serialNumber),
                 new SqlParameter("@TagName",tagName),
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool EditProbePos3DByID(int id, string pos3D)
    {
        string sql = @"update Probe set Pos3D=@Pos3D where ID=@ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@Pos3D",pos3D)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static int InsertProbe(string mailAddress, string probeName, int gasKind, int machineID, string pos3D, string tagName, string serialNumber)
    {
        string sql = @"insert into Probe (MailAddress,ProbeName,GasKind,MachineID,Pos3D,SerialNumber,TagName)values(@MailAddress,@ProbeName,@GasKind,@MachineID,@Pos3D,@SerialNumber,@TagName) SELECT @@IDENTITY AS ID;";
        List<SqlParameter> parameter = new List<SqlParameter>{
                 new SqlParameter("@MailAddress",mailAddress),
                 new SqlParameter("@ProbeName",probeName),
                 new SqlParameter("@GasKind",gasKind),
                 new SqlParameter("@MachineID",machineID),
                 new SqlParameter("@Pos3D",pos3D),
                 new SqlParameter("@SerialNumber",serialNumber),
                 new SqlParameter("@TagName",tagName),
            };
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter.ToArray());
        int insertIndex = Convert.ToInt32(dt.Rows[0][0]);
        return insertIndex;
    }

    public static List<ProbeModel> SelectIDProbeNameGasKindByMachineID(int machineID)
    {
        string sql = @"select ID,ProbeName,GasKind from Probe where MachineID=@MachineID ";
        List<SqlParameter> parameter = new List<SqlParameter>{
                 new SqlParameter("@MachineID",machineID),
            };
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter.ToArray());
        List<ProbeModel> modelList = new List<ProbeModel>();
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProbeModel model = new ProbeModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.GasKind = Convert.ToInt32(dt.Rows[i]["GasKind"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static int SelectCountByMachineID(int machineID)
    {
        string sql = @"select count(*) from Probe where MachineID=@MachineID ";
        List<SqlParameter> parameter = new List<SqlParameter>{
                 new SqlParameter("@MachineID",machineID),
            };
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter.ToArray());
        if (dt.Rows.Count > 0)
        {
            return Convert.ToInt32(dt.Rows[0][0]);
        }
        return 0;
    }

    public static int SelectProbeIDByProbeName(string probeName)
    {
        string sql = @"select ID from Probe where ProbeName like '%" + probeName + "%'";
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            return Convert.ToInt32(dt.Rows[0]["ID"]);
        }
        return 0;
    }

    public static ProbeModel SelectProbeIDProbeNameByProbeName(string probeName)
    {
        ProbeModel model = null;
        string sql = @"select ID,ProbeName from Probe where ProbeName like '%" + probeName + "%'";
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            model = new ProbeModel();
            model.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
            model.ProbeName = dt.Rows[0]["ProbeName"].ToString();
        }
        return model;
    }

    public static bool EditProbePos2DByID(int id, string pos2D)
    {
        string sql = @"update Probe set Pos2D=@Pos2D where ID=@ID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@Pos2D",pos2D)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool DeleteProbePos2DByID(int id)
    {
        string sql = @"update Probe set Pos2D='' where ID=@ID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

}
