using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class CameraHistoryDAL
{
    public static List<CameraHistoryModel> SelectAllHistoryDataByCondition(string androidID, int pageIndex, int pageSize, int startTime, int endTime, out int pageCount, out int rowCount)
    {
        pageCount = 0;
        rowCount = 0;

        List<SqlParameter> para = new List<SqlParameter>()
        {
            new SqlParameter("@pageIndex",pageIndex),
            new SqlParameter("@pageSize",pageSize),
            new SqlParameter("@pageCount",pageCount),
            new SqlParameter("@rowCount",rowCount),
        };

        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        sb1.Append(@"select @RowCount=count(*),@pageCount=ceiling((count(*)+0.0)/@pageSize) from (select ID from CameraHistory where TimeStamp >= @StartCheckTime and TimeStamp <= @EndCheckTime and AndroidID = @AndroidID) temp_row ");
        sb2.Append(@"select top (select @pageSize) * from (select row_number() over(order by TimeStamp desc) as rownumber,ID,AndroidID,TimeStamp,GasValues from CameraHistory where TimeStamp >= @StartCheckTime and TimeStamp <= @EndCheckTime and AndroidID = @AndroidID ) temp_row where rownumber>(@pageIndex-1)*@pageSize ");

        para.Add(new SqlParameter("@StartCheckTime", startTime));
        para.Add(new SqlParameter("@EndCheckTime", endTime));
        para.Add(new SqlParameter("@AndroidID", androidID));

        StringBuilder sql = sb1.Append(sb2);
        DataTable dt = SqlHelper.ExecProcPage(sql.ToString(), out pageCount, out rowCount, para);
        List<CameraHistoryModel> modelList = new List<CameraHistoryModel>();
        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CameraHistoryModel model = new CameraHistoryModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.AndroidID = Convert.ToString(dt.Rows[i]["AndroidID"]);
                model.TimeStamp = Convert.ToInt32(dt.Rows[i]["TimeStamp"]);
                model.GasValues = Convert.ToString(dt.Rows[i]["GasValues"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static int Insert(string sql)
    {
        DataTable dt = SqlHelper.ExecuteDataTable(sql);
        int insertIndex = Convert.ToInt32(dt.Rows[0][0]);
        return insertIndex;
    }

    public static int InsertSingleData(string androidID, int timeStamp, string gasValues)
    {
        string sql = @"insert into CameraHistory (AndroidID,TimeStamp,GasValues)values(@AndroidID,@TimeStamp,@GasValues) SELECT @@IDENTITY AS ID;";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@AndroidID",androidID),
                 new SqlParameter("@TimeStamp",timeStamp),
                 new SqlParameter("@GasValues",gasValues)
            };
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter);
        return Convert.ToInt32(dt.Rows[0][0]);
    }

    public static bool DeleteHistoryData(int timeStamp)
    {
        string sql = @"delete from CameraHistory where TimeStamp <= @TimeStamp";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@TimeStamp",timeStamp)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static string SelectOneNeweastData()
    {
        string sql = @"select top(1) TimeStamp from CameraHistory order by TimeStamp desc";
        DataTable dt = SqlHelper.ExecuteDataTable(sql);
        if (dt.Rows.Count > 0)
        {
            return dt.Rows[0]["TimeStamp"].ToString();
        }
        return string.Empty;
    }

}
