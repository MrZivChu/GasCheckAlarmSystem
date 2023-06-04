using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class CameraDAL
{
    public static bool DeleteCameraByID(string idList)
    {
        string sql = @"exec('delete from Camera where ID in ('+@ID+')')";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",idList)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool EditCameraByID(int id, string ip, string port, string userName, string userPwd, string machineAddress, string gasInfos, string gasValues)
    {
        string sql = @"update Camera set IP=@IP,Port=@Port,UserName=@UserName,UserPwd=@UserPwd,MachineAddress=@MachineAddress,GasInfos=@GasInfos,GasValues=@GasValues where ID=@ID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@IP",ip),
                 new SqlParameter("@Port",port),
                 new SqlParameter("@UserName",userName),
                 new SqlParameter("@UserPwd",userPwd),
                 new SqlParameter("@MachineAddress",machineAddress),
                 new SqlParameter("@GasInfos",gasInfos),
                 new SqlParameter("@GasValues",gasValues)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static int InsertCamera(string ip, string port, string userName, string userPwd, string machineAddress, string gasInfos, string gasValues)
    {
        string sql = @"insert into Camera (IP,Port,UserName,UserPwd,MachineAddress,GasInfos,GasValues)values(@IP,@Port,@UserName,@UserPwd,@MachineAddress,@GasInfos,@GasValues) SELECT @@IDENTITY AS ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@IP",ip),
                 new SqlParameter("@Port",port),
                 new SqlParameter("@UserName",userName),
                 new SqlParameter("@UserPwd",userPwd),
                 new SqlParameter("@MachineAddress",machineAddress),
                 new SqlParameter("@GasInfos",gasInfos),
                 new SqlParameter("@GasValues",gasValues)
            };
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter);
        int insertIndex = Convert.ToInt32(dt.Rows[0][0]);
        return insertIndex;
    }

    public static List<CameraModel> SelectAllCameras()
    {
        string sql = "select * from Camera ";
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
                model.MachineAddress = dt.Rows[i]["MachineAddress"].ToString();
                model.GasInfos = dt.Rows[i]["GasInfos"].ToString();
                model.GasValues = dt.Rows[i]["GasValues"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }
}
