using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class MachineDAL
{
    public static List<MachineModel> SelectAllMachineByCondition(string machineName = "", int factoryID = -1)
    {
        string sql = @"select ID,MailAddress,MachineName,FactoryID,ProtocolType,BaudRate
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
                model.FactoryID = Convert.ToInt32(dt.Rows[i]["FactoryID"]);
                model.ProtocolType = Convert.ToInt32(dt.Rows[i]["ProtocolType"]);
                model.BaudRate = Convert.ToInt32(dt.Rows[i]["BaudRate"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<MachineModel> SelectMachineName()
    {
        string sql = @"select ID,MachineName from Machine ";
        List<MachineModel> modelList = new List<MachineModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                MachineModel model = new MachineModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.MachineName = dt.Rows[i]["MachineName"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static Dictionary<int, MachineModel> SelectAllMachineDic()
    {

        string sql = @"select * from Machine";
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        Dictionary<int, MachineModel> dic = new Dictionary<int, MachineModel>();
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                MachineModel model = new MachineModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.MailAddress = dt.Rows[i]["MailAddress"].ToString();
                model.MachineName = dt.Rows[i]["MachineName"].ToString();
                model.FactoryID = Convert.ToInt32(dt.Rows[i]["FactoryID"]);
                model.ProtocolType = Convert.ToInt32(dt.Rows[i]["ProtocolType"]);
                model.BaudRate = Convert.ToInt32(dt.Rows[i]["BaudRate"]);
                if (!dic.ContainsKey(model.ID))
                {
                    dic[model.ID] = model;
                }
            }
        }
        return dic;
    }

    public static bool DeleteMachineByID(string idList)
    {
        string sql = @"delete from Machine where ID in (" + idList + @");
        delete from Probe where MachineID in (" + idList + @");
        ";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }

    public static bool EditMachineByID(int id, string mailAddress, string machineName, int factoryID, int protocolType, int baudRate)
    {
        //没必要更新历史数据
        string sql = @"update Machine set MailAddress=@MailAddress,MachineName=@MachineName,FactoryID=@FactoryID,ProtocolType=@ProtocolType,BaudRate=@BaudRate where ID=@ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@MailAddress",mailAddress),
                 new SqlParameter("@MachineName",machineName),
                 new SqlParameter("@FactoryID",factoryID),
                 new SqlParameter("@ProtocolType",protocolType),
                 new SqlParameter("@BaudRate",baudRate),
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool InsertMachine(string mailAddress, string machineName, int factoryID, int protocolType, int baudRate)
    {
        string sql = @"insert into Machine (MailAddress,MachineName,FactoryID,ProtocolType,BaudRate)values(@MailAddress,@MachineName,@FactoryID,@ProtocolType,@BaudRate)";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@MailAddress",mailAddress),
                 new SqlParameter("@MachineName",machineName),
                 new SqlParameter("@FactoryID",factoryID),
                 new SqlParameter("@ProtocolType",protocolType),
                 new SqlParameter("@BaudRate",baudRate),
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }


}
