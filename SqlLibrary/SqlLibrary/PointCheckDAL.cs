using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class PointCheckDAL
{
    public static List<PointCheckModel> SelectAllPointCheckByCondition(int pageIndex, int pageSize, string userName, string deviceName, int deviceType, string startTime, string endTime, out int pageCount, out int rowCount)
    {
        pageCount = 0;
        rowCount = 0;
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        sb1.Append(@"select @RowCount=count(*),@pageCount=ceiling((count(*)+0.0)/@pageSize) 
		from (
		select ID,DeviceID,DeviceName,DeviceType,UserName,QrCodePath,CheckTime,Description,Result
        from PointCheck) temp_row  
        where 1=1 ");

        sb2.Append(@"select top (select @pageSize) *   
	from (select row_number() over(order by CheckTime desc) as rownumber,ID,DeviceID,DeviceName,DeviceType,UserName,QrCodePath,CheckTime,Description,Result from PointCheck) temp_row 
	where 1=1 and rownumber>(@pageIndex-1)*@pageSize ");

        List<SqlParameter> para = new List<SqlParameter>()
        {
            new SqlParameter("@pageIndex",pageIndex),
            new SqlParameter("@pageSize",pageSize),
            new SqlParameter("@pageCount",pageCount),
            new SqlParameter("@rowCount",rowCount),
        };
        sb1.Append(" and temp_row.DeviceType = @DeviceType ");
        sb2.Append(" and temp_row.DeviceType = @DeviceType ");
        para.Add(new SqlParameter("@DeviceType", deviceType));
        if (!string.IsNullOrEmpty(deviceName))
        {
            sb1.Append(" and temp_row.DeviceName = @DeviceName ");
            sb2.Append(" and temp_row.DeviceName = @DeviceName ");
            para.Add(new SqlParameter("@DeviceName", deviceName));
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
        DataTable dt = SqlHelper.ExecProcPage(sql.ToString(), out pageCount, out rowCount, para);
        List<PointCheckModel> modelList = new List<PointCheckModel>();
        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PointCheckModel model = new PointCheckModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.DeviceID = Convert.ToInt32(dt.Rows[i]["DeviceID"]);
                model.DeviceName = dt.Rows[i]["DeviceName"].ToString();
                model.DeviceType = Convert.ToInt32(dt.Rows[i]["DeviceType"]);
                model.UserName = dt.Rows[i]["UserName"].ToString();
                model.QrCodePath = dt.Rows[i]["QrCodePath"].ToString();
                model.CheckTime = Convert.ToDateTime(dt.Rows[i]["CheckTime"]);
                model.Description = dt.Rows[i]["Description"].ToString();
                model.Result = dt.Rows[i]["Result"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static bool InsertPointCheck(int deviceID, string deviceName, string userName, string qrCodePath, string description, string result, int deviceType)
    {
        string sql = @"insert into PointCheck (DeviceID,DeviceName,DeviceType,UserName,QrCodePath,Description,Result)values(@DeviceID,@DeviceName,@DeviceType,@UserName,@QrCodePath,@Description,@Result)";
        List<SqlParameter> parameter = new List<SqlParameter>{
                 new SqlParameter("@DeviceID",deviceID),
                 new SqlParameter("@DeviceName",deviceName),
                 new SqlParameter("@UserName",userName),
                 new SqlParameter("@QrCodePath",qrCodePath),
                 new SqlParameter("@Description",description),
                 new SqlParameter("@Result",result),
                 new SqlParameter("@DeviceType",deviceType),
            };
        int index = SqlHelper.ExecuteNonQuery(sql, parameter.ToArray());
        return index >= 1 ? true : false;
    }

}
