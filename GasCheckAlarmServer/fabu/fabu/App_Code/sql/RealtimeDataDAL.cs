using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class RealtimeDataDAL
{
    public static List<RealtimeDataModel> SelectAllRealtimeDataByCondition(string probeName = "",string gasKind = "")
    {
        string sql = @"select ID,ProbeID,CheckTime,GasValue,ProbeName,GasKind,Unit,FirstAlarmValue,SecondAlarmValue,MachineName,MachineID,FactoryID,FactoryName,MachineType,Pos2D from RealtimeData where 1=1 ";
        if (!string.IsNullOrEmpty(probeName))
            sql += " and ProbeName like '%" + probeName + "%' ";
        if (!string.IsNullOrEmpty(gasKind))
            sql += " and GasKind like '%" + gasKind + "%' ";
        sql += " order by MachineID";
        List<RealtimeDataModel> modelList = new List<RealtimeDataModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                RealtimeDataModel model = new RealtimeDataModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.ProbeID = Convert.ToInt32(dt.Rows[i]["ProbeID"]);
                model.CheckTime = Convert.ToDateTime(dt.Rows[i]["CheckTime"]);
                model.GasValue = Convert.ToSingle(dt.Rows[i]["GasValue"]);
                model.GasKind = dt.Rows[i]["GasKind"].ToString();
                model.Unit = dt.Rows[i]["Unit"].ToString();
                model.FirstAlarmValue = Convert.ToSingle(dt.Rows[i]["FirstAlarmValue"]);
                model.SecondAlarmValue = Convert.ToSingle(dt.Rows[i]["SecondAlarmValue"]);
                model.MachineName = dt.Rows[i]["MachineName"].ToString();
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
                model.FactoryID = Convert.ToInt32(dt.Rows[i]["FactoryID"]);
                model.FactoryName = dt.Rows[i]["FactoryName"].ToString();
                model.MachineType = Convert.ToInt32(dt.Rows[i]["MachineType"]);
                model.Pos2D = dt.Rows[i]["Pos2D"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static bool EditRealtimePos2DByID(int id, string pos2D)
    {
        //没必要更新历史数据
        string sql = @"update Probe set Pos2D=@Pos2D where ID=@ID;
        update RealtimeData set Pos2D=@Pos2D where ProbeID=@ID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@Pos2D",pos2D)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool DeleteRealtimePos2DByID(int id)
    {
        //没必要更新历史数据
        string sql = @"update Probe set Pos2D='' where ID=@ID;
        update RealtimeData set Pos2D='' where ProbeID=@ID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }
}
