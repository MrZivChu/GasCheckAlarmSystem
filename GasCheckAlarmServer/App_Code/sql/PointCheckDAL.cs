using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class PointCheckDAL
{
    public static List<PointCheckModel> SelectAllPointCheckByCondition(int pageIndex, int pageSize, string userName, string probeName, string startTime, string endTime, out int pageCount, out int rowCount)
    {
        pageCount = 0;
        rowCount = 0;
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        sb1.Append(@"select @RowCount=count(*),@pageCount=ceiling((count(*)+0.0)/@pageSize) 
		from (
		select ProbeID,ProbeName,UserName,QrCodePath,CheckTime
        from PointCheck) temp_row  
        where 1=1 ");

        sb2.Append(@"select top (select @pageSize) *   
	from (select row_number() over(order by CheckTime desc) as rownumber,ID,ProbeID,ProbeName,UserName,QrCodePath,CheckTime from PointCheck) temp_row 
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
        if (!string.IsNullOrEmpty(userName))
        {
            sb1.Append(" and temp_row.UserName = @UserName ");
            sb2.Append(" and temp_row.UserName = @UserName ");
            para.Add(new SqlParameter("@UserName", userName));
        }
        if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
        {
            sb1.Append(" and temp_row.CheckTime >= @StartCheckTime and temp_row.CheckTime <= @EndCheckTime ");
            sb2.Append(" and temp_row.CheckTime >= @StartCheckTime and temp_row.CheckTime <= @EndCheckTime ");
            para.Add(new SqlParameter("@StartCheckTime", startTime));
            para.Add(new SqlParameter("@EndCheckTime", endTime));
        }
        StringBuilder sql = sb1.Append(sb2);
        //UnityEngine.Debug.Log(sql.ToString());
        DataTable dt = SqlHelper.ExecProcPage(sql.ToString(), out pageCount, out rowCount, para);
        List<PointCheckModel> modelList = new List<PointCheckModel>();
        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PointCheckModel model = new PointCheckModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.ProbeID = Convert.ToInt32(dt.Rows[i]["ProbeID"]);
                model.ProbeName = dt.Rows[i]["ProbeName"].ToString();
                model.UserName = dt.Rows[i]["UserName"].ToString();
                model.QrCodePath = dt.Rows[i]["QrCodePath"].ToString();
                model.CheckTime = Convert.ToDateTime(dt.Rows[i]["CheckTime"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static bool InsertPointCheck(int probeID, string probeName, string userName, string qrCodePath)
    {
        string sql = @"insert into PointCheck (ProbeID,ProbeName,UserName,QrCodePath)values(@ProbeID,@ProbeName,@UserName,@QrCodePath)";
        List<SqlParameter> parameter = new List<SqlParameter>{
                 new SqlParameter("@ProbeID",probeID),
                 new SqlParameter("@ProbeName",probeName),
                 new SqlParameter("@UserName",userName),
                 new SqlParameter("@QrCodePath",qrCodePath)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter.ToArray());
        return result >= 1 ? true : false;
    }

}
