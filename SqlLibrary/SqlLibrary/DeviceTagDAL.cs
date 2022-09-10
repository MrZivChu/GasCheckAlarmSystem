using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System;

public class DeviceTagDAL
{
    public static List<DeviceTagModel> SelectAllDeviceTag()
    {
        string sql = @"select * from DeviceTag";
        List<DeviceTagModel> modelList = new List<DeviceTagModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DeviceTagModel model = new DeviceTagModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.TagName = dt.Rows[i]["TagName"].ToString();
                model.ParentID = Convert.ToInt32(dt.Rows[i]["ParentID"]);
                model.Position = dt.Rows[i]["Position"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static bool DeleteDeviceTagByID(string idList)
    {
        string sql = @"delete from DeviceTag where ID in (" + idList + @");";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",idList)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }

    public static bool EditDeviceTagByID(int id, string position)
    {
        string sql = @"update DeviceTag set Position=@Position where ID=@ID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@Position",position),
                 new SqlParameter("@ID",id)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static int InsertDeviceTag(string tagName, int parentID, string position)
    {
        string sql = @"insert into DeviceTag (TagName,ParentID,Position)values(@TagName,@ParentID,@Position) SELECT @@IDENTITY AS ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@TagName",tagName),
                 new SqlParameter("@ParentID",parentID),
                 new SqlParameter("@Position",position),
            };
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter);
        return Convert.ToInt32(dt.Rows[0][0]);
    }
}
