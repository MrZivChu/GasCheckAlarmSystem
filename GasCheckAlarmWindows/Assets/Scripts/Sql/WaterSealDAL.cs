using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class WaterSealDAL
{
    public static List<WaterSealModel> SelectAllWaterSealByCondition()
    {
        string sql = @"select ID,Medium,Number,InstallPosition,Category,DesignPressure,SerialNumber from WaterSeal where 1=1 ";
        List<WaterSealModel> modelList = new List<WaterSealModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                WaterSealModel model = new WaterSealModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.Medium = dt.Rows[i]["Medium"].ToString();
                model.Number = dt.Rows[i]["Number"].ToString();
                model.InstallPosition = dt.Rows[i]["InstallPosition"].ToString();
                model.Category = dt.Rows[i]["Category"].ToString();
                model.DesignPressure = Convert.ToInt32(dt.Rows[i]["DesignPressure"]);
                model.SerialNumber = dt.Rows[i]["SerialNumber"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static WaterSealModel SelectWaterSealByID(int id)
    {
        string sql = @"select ID,Medium,Number,InstallPosition,Category,DesignPressure,SerialNumber from WaterSeal where ID = @ID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id)
            };
        WaterSealModel model = new WaterSealModel();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter);
        if (dt.Rows.Count > 0)
        {
            model.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
            model.Medium = dt.Rows[0]["Medium"].ToString();
            model.Number = dt.Rows[0]["Number"].ToString();
            model.InstallPosition = dt.Rows[0]["InstallPosition"].ToString();
            model.Category = dt.Rows[0]["Category"].ToString();
            model.DesignPressure = Convert.ToInt32(dt.Rows[0]["DesignPressure"]);
            model.SerialNumber = dt.Rows[0]["SerialNumber"].ToString();
        }
        return model;
    }

    public static WaterSealModel SelectWaterSealBySerialNumber(string serialNumber)
    {
        string sql = @"select ID,Medium,Number,InstallPosition,Category,DesignPressur,SerialNumber from WaterSeal where SerialNumber = @SerialNumber";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@SerialNumber",serialNumber)
            };
        WaterSealModel model = new WaterSealModel();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter);
        if (dt.Rows.Count > 0)
        {
            model.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
            model.Medium = dt.Rows[0]["Medium"].ToString();
            model.Number = dt.Rows[0]["Number"].ToString();
            model.InstallPosition = dt.Rows[0]["InstallPosition"].ToString();
            model.Category = dt.Rows[0]["Category"].ToString();
            model.DesignPressure = Convert.ToInt32(dt.Rows[0]["DesignPressure"]);
            model.SerialNumber = dt.Rows[0]["SerialNumber"].ToString();
        }
        return model;
    }

    public static bool DeleteWaterSealByID(string idList)
    {
        string sql = @"delete from WaterSeal where ID in (" + idList + @");";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }

    public static bool EditWaterSealByID(int id, string medium, string number, string installPosition, string category, int designPressure, string serialNumber)
    {
        string sql = @"update WaterSeal set Medium=@Medium,Number=@Number,InstallPosition=@InstallPosition,Category=@Category,DesignPressure=@DesignPressure,SerialNumber=@SerialNumber where ID=@ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@Medium",medium),
                 new SqlParameter("@Number",number),
                 new SqlParameter("@InstallPosition",installPosition),
                 new SqlParameter("@Category",category),
                 new SqlParameter("@DesignPressure",designPressure),
                 new SqlParameter("@SerialNumber",serialNumber),
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static int InsertWaterSeal(string medium, string number, string installPosition, string category, int designPressure, string serialNumber)
    {
        string sql = @"insert into WaterSeal (Medium,Number,InstallPosition,Category,DesignPressure,SerialNumber)values(@Medium,@Number,@InstallPosition,@Category,@DesignPressure,@SerialNumber) SELECT @@IDENTITY AS ID;";
        List<SqlParameter> parameter = new List<SqlParameter>{
                new SqlParameter("@Medium",medium),
                 new SqlParameter("@Number",number),
                 new SqlParameter("@InstallPosition",installPosition),
                 new SqlParameter("@Category",category),
                 new SqlParameter("@DesignPressure",designPressure),
                 new SqlParameter("@SerialNumber",serialNumber),
            };
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter.ToArray());
        return Convert.ToInt32(dt.Rows[0][0]);
    }

}
