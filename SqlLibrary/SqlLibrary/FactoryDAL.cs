using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System;

public class FactoryDAL
{
    public static List<FactoryModel> SelectAllFactoryByCondition(string factoryName = "")
    {
        string sql = @"select * from Factory where 1=1 ";
        if (!string.IsNullOrEmpty(factoryName))
            sql += " and FactoryName like '%" + factoryName + "%' ";
        List<FactoryModel> modelList = new List<FactoryModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                FactoryModel model = new FactoryModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.FactoryName = dt.Rows[i]["FactoryName"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static Dictionary<int, FactoryModel> SelectAllFactoryDic()
    {
        string sql = @"select * from Factory";
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        Dictionary<int, FactoryModel> dic = new Dictionary<int, FactoryModel>();
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                FactoryModel model = new FactoryModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.FactoryName = dt.Rows[i]["FactoryName"].ToString();
                if (!dic.ContainsKey(model.ID))
                {
                    dic[model.ID] = model;
                }
            }
        }
        return dic;
    }

    public static bool DeleteFactoryByID(string idList)
    {
        string sql = @"select ID from Machine where FactoryID in (" + idList + @")";
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            string machineIDStr = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                machineIDStr += Convert.ToInt32(dt.Rows[i]["ID"]) + ",";
            }
            machineIDStr = machineIDStr.TrimEnd(',');
            sql = @"delete from Probe where MachineID in (" + machineIDStr + @")";
            SqlHelper.ExecuteNonQuery(sql, null);
        }

        sql = @"delete from Machine where FactoryID in (" + idList + @");
        delete from Factory where ID in (" + idList + @");
        ";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }

    public static bool EditFactoryByID(int id, string factoryName)
    {
        //没必要更新历史数据
        string sql = @"update Factory set FactoryName=@FactoryName where ID=@ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@FactoryName",factoryName)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool InsertFactory(string factoryName)
    {
        string sql = @"insert into Factory (FactoryName)values(@FactoryName)";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@FactoryName",factoryName)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }
}
