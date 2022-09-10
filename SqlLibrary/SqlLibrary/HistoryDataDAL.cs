using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class HistoryDataDAL
{
    public static List<HistoryDataModel> SelectAllHistoryDataByCondition(int pageIndex, int pageSize, string probeName, string gasKind, string startTime, string endTime, out int pageCount, out int rowCount)
    {
        pageCount = 0;
        rowCount = 0;
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        sb1.Append(@"select @RowCount=count(*),@pageCount=ceiling((count(*)+0.0)/@pageSize) 
		from (
		select ProbeName,GasKind,GasValue,CheckTime
        from HistoryData) temp_row  
        where 1=1 ");

        sb2.Append(@"select top (select @pageSize) *   
	from (select row_number() over(order by CheckTime desc) as rownumber,ID,FactoryID,MachineID,ProbeID,CheckTime,GasValue,FactoryName,MachineName,ProbeName,GasKind,FirstAlarmValue,SecondAlarmValue,MachineType 
        from HistoryData) temp_row 
	where 1=1 and rownumber>(@pageIndex-1)*@pageSize ");

        List<SqlParameter> para = new List<SqlParameter>()
        {
            new SqlParameter("@pageIndex",pageIndex),
            new SqlParameter("@pageSize",pageSize),
            new SqlParameter("@pageCount",pageCount),
            new SqlParameter("@rowCount",rowCount),
        };

        if (!string.IsNullOrEmpty(probeName))
        {
            sb1.Append(" and temp_row.ProbeName = @ProbeName ");
            sb2.Append(" and temp_row.ProbeName = @ProbeName ");
            para.Add(new SqlParameter("@ProbeName", probeName));
        }
        if (!string.IsNullOrEmpty(gasKind))
        {
            sb1.Append(" and temp_row.GasKind = @GasKind ");
            sb2.Append(" and temp_row.GasKind = @GasKind ");
            para.Add(new SqlParameter("@GasKind", gasKind));
        }
        if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
        {
            sb1.Append(" and temp_row.CheckTime >= @StartCheckTime and temp_row.CheckTime <= @EndCheckTime ");
            sb2.Append(" and temp_row.CheckTime >= @StartCheckTime and temp_row.CheckTime <= @EndCheckTime ");
            para.Add(new SqlParameter("@StartCheckTime", startTime));
            para.Add(new SqlParameter("@EndCheckTime", endTime));
        }

        StringBuilder sql = sb1.Append(sb2);
        DataTable dt = SqlHelper.ExecProcPage(sql.ToString(), out pageCount, out rowCount, para);
        List<HistoryDataModel> modelList = new List<HistoryDataModel>();
        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                HistoryDataModel model = new HistoryDataModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.ProbeID = Convert.ToInt32(dt.Rows[i]["ProbeID"]);
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.CheckTime = Convert.ToDateTime(dt.Rows[i]["CheckTime"]);
                model.GasValue = Convert.ToSingle(dt.Rows[i]["GasValue"]);
                model.FactoryID = Convert.ToInt32(dt.Rows[i]["FactoryID"]);
                model.FactoryName = dt.Rows[i]["FactoryName"].ToString();
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
                model.MachineName = dt.Rows[i]["MachineName"].ToString();
                model.GasKind = dt.Rows[i]["GasKind"].ToString();
                model.FirstAlarmValue = Convert.ToSingle(dt.Rows[i]["FirstAlarmValue"]);
                model.SecondAlarmValue = Convert.ToSingle(dt.Rows[i]["SecondAlarmValue"]);
                model.MachineType = Convert.ToInt32(dt.Rows[i]["MachineType"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static bool DeleteHistoryDataByID(string idList)
    {
        string sql = @"delete from HistoryData where ID in (" + idList + @")";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }

    public static bool DeleteHistoryDataBeforeWeek()
    {
        DateTime date = DateTime.Now.AddDays(-3);
        string sql = @"delete from HistoryData where CheckTime <= @CheckTime";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@CheckTime",date)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool DeleteAllHistoryData()
    {
        string sql = @"truncate table HistoryData";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }
}
