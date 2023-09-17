using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CameraDAL
{
    public static int InsertCameraBaseData(string androidID, string ip, string port, string userName, string userPwd)
    {
        string sql = "select ID from Camera where AndroidID=@AndroidID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@AndroidID",androidID)
            };
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter);
        if (dt != null && dt.Rows.Count > 0)
        {
            sql = @"update Camera set IP=@IP,Port=@Port,UserName=@UserName,UserPwd=@UserPwd where AndroidID=@AndroidID";
            parameter = new SqlParameter[] {
                 new SqlParameter("@AndroidID",androidID),
                 new SqlParameter("@IP",ip),
                 new SqlParameter("@Port",port),
                 new SqlParameter("@UserName",userName),
                 new SqlParameter("@UserPwd",userPwd)
            };
            return SqlHelper.ExecuteNonQuery(sql, parameter);
        }
        else
        {
            sql = @"insert into Camera (AndroidID,IP,Port,UserName,UserPwd)values(@AndroidID,@IP,@Port,@UserName,@UserPwd) SELECT @@IDENTITY AS ID;";
            parameter = new SqlParameter[] {
                 new SqlParameter("@AndroidID",androidID),
                 new SqlParameter("@IP",ip),
                 new SqlParameter("@Port",port),
                 new SqlParameter("@UserName",userName),
                 new SqlParameter("@UserPwd",userPwd)
            };
            dt = SqlHelper.ExecuteDataTable(sql, parameter);
            return Convert.ToInt32(dt.Rows[0][0]);
        }
    }

    public static int InsertGasBaseData(string androidID, string machineID, string gases)
    {
        string sql = "select ID from Camera where AndroidID=@AndroidID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@AndroidID",androidID)
            };
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter);
        if (dt != null && dt.Rows.Count > 0)
        {
            sql = @"update Camera set MachineAddress=@MachineAddress,GasInfos=@GasInfos where AndroidID=@AndroidID";
            parameter = new SqlParameter[] {
                 new SqlParameter("@AndroidID",androidID),
                 new SqlParameter("@MachineAddress",machineID),
                 new SqlParameter("@GasInfos",gases)
            };
            return SqlHelper.ExecuteNonQuery(sql, parameter);
        }
        else
        {
            sql = @"insert into Camera (AndroidID,MachineAddress,GasInfos)values(@AndroidID,@MachineAddress,@GasInfos) SELECT @@IDENTITY AS ID;";
            parameter = new SqlParameter[] {
                 new SqlParameter("@AndroidID",androidID),
                 new SqlParameter("@MachineAddress",machineID),
                 new SqlParameter("@GasInfos",gases)
            };
            dt = SqlHelper.ExecuteDataTable(sql, parameter);
            return Convert.ToInt32(dt.Rows[0][0]);
        }
    }

    public static int UpdateRealtimeGasData(string androidID, string gasValues)
    {
        string sql = @"update Camera set GasValues=@GasValues where AndroidID=@AndroidID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@AndroidID",androidID),
                 new SqlParameter("@GasValues",gasValues)
            };
        return SqlHelper.ExecuteNonQuery(sql, parameter);
    }

    public static List<CameraModel> SelectAllCameraBaseData()
    {
        string sql = "select ID,IP,Port,UserName,UserPwd,GasInfos from Camera ";
        List<CameraModel> modelList = new List<CameraModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CameraModel model = new CameraModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.IP = dt.Rows[i]["IP"].ToString();
                model.Port = dt.Rows[i]["Port"].ToString();
                model.UserName = dt.Rows[i]["UserName"].ToString();
                model.UserPwd = dt.Rows[i]["UserPwd"].ToString();
                model.GasInfos = dt.Rows[i]["GasInfos"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<CameraModel> SelectCameraInfoForHistory()
    {
        string sql = "select AndroidID,IP,GasInfos from Camera ";
        List<CameraModel> modelList = new List<CameraModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CameraModel model = new CameraModel();
                model.AndroidID = dt.Rows[i]["AndroidID"].ToString();
                model.IP = dt.Rows[i]["IP"].ToString();
                model.GasInfos = dt.Rows[i]["GasInfos"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<CameraModel> SelectAllCameraRealtimeData()
    {
        string sql = "select ID,GasValues from Camera ";
        List<CameraModel> modelList = new List<CameraModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CameraModel model = new CameraModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.GasValues = dt.Rows[i]["GasValues"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }
}
