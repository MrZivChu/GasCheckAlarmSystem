using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System;

public class GasTypesDAL
{

    public static List<GasTypesModel> SelectAllGasTypes()
    {
        string sql = @"select * from GasTypes";
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        List<GasTypesModel> list = new List<GasTypesModel>();
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GasTypesModel model = new GasTypesModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.GasName = dt.Rows[i]["GasName"].ToString();
                model.MinValue = Convert.ToSingle(dt.Rows[i]["MinValue"]);
                model.MaxValue = Convert.ToSingle(dt.Rows[i]["MaxValue"]);
                list.Add(model);
            }
        }
        return list;
    }

    public static bool DeleteGasTypeByID(string idList)
    {
        string sql = @"delete from GasTypes where ID in (" + idList + ")";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }

    public static bool EditGasTypeByID(int id, string gasName, float minValue, float maxValue)
    {
        string sql = @"update GasTypes set GasName=@GasName,MinValue=@MinValue,MaxValue=@MaxValue where ID=@ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@GasName",gasName),
                 new SqlParameter("@MinValue",minValue),
                 new SqlParameter("@MaxValue",maxValue),
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool InsertGasType(string gasName, float minValue, float maxValue)
    {
        string sql = @"insert into GasTypes (GasName,MinValue,MaxValue)values(@GasName,@MinValue,@MaxValue)";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@GasName",gasName),
                 new SqlParameter("@MinValue",minValue),
                 new SqlParameter("@MaxValue",maxValue),
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }
}
