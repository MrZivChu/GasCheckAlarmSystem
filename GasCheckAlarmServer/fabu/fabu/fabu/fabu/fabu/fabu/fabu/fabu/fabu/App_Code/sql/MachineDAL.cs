using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class MachineDAL
{
    public static List<MachineModel> SelectAllMachineByCondition(string machineName = "", int factoryID = -1)
    {
        string sql = @"select ID,MailAddress,MachineName,MachineType,FactoryName,FactoryID
        from Machine where 1=1 ";
        if (!string.IsNullOrEmpty(machineName))
            sql += " and MachineName like '%" + machineName + "%' ";
        if (!string.IsNullOrEmpty(factoryID.ToString()) && factoryID != -1)
            sql += " and FactoryID =" + factoryID;
        List<MachineModel> modelList = new List<MachineModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                MachineModel model = new MachineModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.MailAddress = dt.Rows[i]["MailAddress"].ToString();
                model.MachineName = dt.Rows[i]["MachineName"].ToString();
                model.MachineType = Convert.ToInt32(dt.Rows[i]["MachineType"]);
                model.FactoryID = Convert.ToInt32(dt.Rows[i]["FactoryID"]);
                model.FactoryName = dt.Rows[i]["FactoryName"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static bool DeleteMachineByID(string idList)
    {
        string sql = @"delete from Machine where ID in (" + idList + @");
        delete from Probe where MachineID in (" + idList + @");
        delete from RealtimeData where MachineID in (" + idList + @");
        ";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",idList)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }

    public static bool EditMachineByID(int id, string mailAddress, string machineName, int factoryID, string factoryName)
    {
        //没必要更新历史数据
        string sql = @"update Machine set MailAddress=@MailAddress,MachineName=@MachineName,FactoryID=@FactoryID,FactoryName=@FactoryName where ID=@ID;
        update Probe set FactoryID=@FactoryID,FactoryName=@FactoryName,MachineName=@MachineName where MachineID=@ID;
        update RealtimeData set FactoryID=@FactoryID,FactoryName=@FactoryName,MachineName=@MachineName where MachineID=@ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@MachineName",machineName),
                 new SqlParameter("@FactoryID",factoryID),
                 new SqlParameter("@FactoryName",factoryName),
                 new SqlParameter("@MailAddress",mailAddress)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool InsertMachine(string mailAddress, string machineName, int factoryID, string factoryName, int machineProtocol)
    {
        string sql = @"insert into Machine (MailAddress,MachineName,FactoryID,FactoryName,MachineType)values(@MailAddress,@MachineName,@FactoryID,@FactoryName,@MachineType)";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@MailAddress",mailAddress),
                 new SqlParameter("@MachineName",machineName),
                 new SqlParameter("@FactoryID",factoryID),
                 new SqlParameter("@FactoryName",factoryName),
                 new SqlParameter("@MachineType",machineProtocol)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }
}
