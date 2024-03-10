using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class HistoryDataDAL
{
    public static List<HistoryDataModel> SelectAllHistoryDataByCondition(int pageIndex, int pageSize, string startTime, string endTime, int probeID, out int pageCount, out int rowCount)
    {
        pageCount = 0;
        rowCount = 0;
        string whereStr1 = string.Empty;
        string whereStr2 = string.Empty;

        List<SqlParameter> para = new List<SqlParameter>()
        {
            new SqlParameter("@pageIndex",pageIndex),
            new SqlParameter("@pageSize",pageSize),
            new SqlParameter("@pageCount",pageCount),
            new SqlParameter("@rowCount",rowCount),
        };

        if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
        {
            whereStr1 += " and CheckTime >= @StartCheckTime and CheckTime <= @EndCheckTime ";
            whereStr2 += " and CheckTime >= @StartCheckTime and CheckTime <= @EndCheckTime ";
            para.Add(new SqlParameter("@StartCheckTime", startTime));
            para.Add(new SqlParameter("@EndCheckTime", endTime));
        }
        if (probeID > 0)
        {
            whereStr1 += " and ProbeID = @ProbeID ";
            whereStr2 += " and ProbeID = @ProbeID ";
            para.Add(new SqlParameter("@ProbeID", probeID));
        }
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        sb1.Append(@"select @RowCount=count(*),@pageCount=ceiling((count(*)+0.0)/@pageSize) from (select ID from HistoryData where 1=1 " + whereStr1 + ") temp_row ");
        sb2.Append(@"select top (select @pageSize) * from (select row_number() over(order by CheckTime desc) as rownumber,ID,ProbeID,GasValue,CheckTime,MachineID from HistoryData where 1=1 " + whereStr2 + ") temp_row where rownumber>(@pageIndex-1)*@pageSize ");

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
                model.GasValue = Convert.ToSingle(dt.Rows[i]["GasValue"]);
                model.CheckTime = Convert.ToDateTime(dt.Rows[i]["CheckTime"]);
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
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

    public static bool DeleteHistoryDataBeforeDays(int days)
    {
        DateTime date = DateTime.Now.AddDays(-days);
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

    public static List<HistoryDataModel> SelectHistoryDataForChart(int machineID, float firstAlarmValue, string startTime, string endTime)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(@"select ProbeID,CheckTime,GasValue,MachineID from HistoryData where MachineID = @MachineID and GasValue >= @GasValue ");
        List<SqlParameter> para = new List<SqlParameter>()
        {
            new SqlParameter("@GasValue",firstAlarmValue),
            new SqlParameter("@MachineID",machineID)
        };
        if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
        {
            sb.Append(" and CheckTime >= @StartCheckTime and CheckTime <= @EndCheckTime ");
            para.Add(new SqlParameter("@StartCheckTime", startTime));
            para.Add(new SqlParameter("@EndCheckTime", endTime));
        }
        sb.Append(" order by CheckTime desc ");
        DataTable dt = SqlHelper.ExecuteDataTable(sb.ToString(), para.ToArray());
        List<HistoryDataModel> modelList = new List<HistoryDataModel>();
        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                HistoryDataModel model = new HistoryDataModel();
                model.ProbeID = Convert.ToInt32(dt.Rows[i]["ProbeID"]);
                model.CheckTime = Convert.ToDateTime(dt.Rows[i]["CheckTime"]);
                model.GasValue = Convert.ToSingle(dt.Rows[i]["GasValue"]);
                model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<HistoryDataModel> SelectAllHistoryDataForChart(int probeID, string startTime, string endTime)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(@"select ProbeID,GasValue,CheckTime from HistoryData where ProbeID = @ProbeID and CheckTime >= @StartCheckTime and CheckTime <= @EndCheckTime");
        List<SqlParameter> para = new List<SqlParameter>()
        {
            new SqlParameter("@ProbeID",probeID),
            new SqlParameter("@StartCheckTime",startTime),
            new SqlParameter("@EndCheckTime",endTime),
        };
        sb.Append(" order by CheckTime desc ");
        DataTable dt = SqlHelper.ExecuteDataTable(sb.ToString(), para.ToArray());
        List<HistoryDataModel> modelList = new List<HistoryDataModel>();
        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                HistoryDataModel model = new HistoryDataModel();
                model.ProbeID = Convert.ToInt32(dt.Rows[i]["ProbeID"]);
                model.GasValue = Convert.ToSingle(dt.Rows[i]["GasValue"]);
                model.CheckTime = Convert.ToDateTime(dt.Rows[i]["CheckTime"]);
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static bool TableFenQu(string dateTime, string fileDir, string database)
    {
        string groupNameBefore = "group" + dateTime + "before";
        string fileNameBefore = "file" + dateTime + "before";
        string filePathBefore = fileDir + "\\" + fileNameBefore + ".ndf";

        string groupName = "group" + dateTime;
        string fileName = "file" + dateTime;
        string filePath = fileDir + "\\" + fileName + ".ndf";

        string sql = @"
--判断分区函数是否存在
IF EXISTS (SELECT 1 FROM sys.partition_functions WHERE name = 'fenquhanshu')
BEGIN
	--创建文件组
	alter database " + database + @" add filegroup " + groupName + @"
	--创建文件
	alter database " + database + @" add file(name='" + fileName + @"',filename='" + filePath + @"',size=1mb, filegrowth=1mb) to filegroup " + groupName + @";
    --定义下一个扩展分区方案
	ALTER PARTITION SCHEME fenqufangan NEXT USED " + groupName + @"
	--修改分区函数，追加一个分区，对应的分区方案就是上面新加的扩展分区方案
	alter PARTITION FUNCTION fenquhanshu()
	SPLIT RANGE ('" + dateTime + @"')
END
ELSE
BEGIN
	--创建文件组
	alter database " + database + @" add filegroup " + groupNameBefore + @"
	--创建文件
	alter database " + database + @" add file(name='" + fileNameBefore + @"',filename='" + filePathBefore + @"',size=1mb, filegrowth=1mb) to filegroup " + groupNameBefore + @";
	--创建文件组
	alter database " + database + @" add filegroup " + groupName + @"
	--创建文件
	alter database " + database + @" add file(name='" + fileName + @"',filename='" + filePath + @"',size=1mb, filegrowth=1mb) to filegroup " + groupName + @";
	--创建分区函数
	CREATE PARTITION FUNCTION fenquhanshu (datetime)
	AS RANGE RIGHT FOR VALUES ('" + dateTime + @"')
	--创建分区方案
	CREATE PARTITION SCHEME fenqufangan
	AS PARTITION fenquhanshu
	TO (" + groupNameBefore + @"," + groupName + @")
END";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }

    //查看分区表的数据情况：select * from FenQuTable where $PARTITION.fenquhanshu(checktime)=1
    public static bool RelateTable(string tableName, string columnName)
    {
        string sql = @"ALTER TABLE HistoryData DROP constraint PK_HistoryData;
IF not EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('" + tableName + @"', N'U') and NAME='newIndex')
BEGIN
    CREATE CLUSTERED INDEX newIndex ON " + tableName + @"(" + columnName + @")
                    ON fenqufangan(" + columnName + @")
END";
        int result = SqlHelper.ExecuteNonQuery(sql, null);
        return result >= 1 ? true : false;
    }
}
