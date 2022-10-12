using GasCheckAlarmSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

public class HistoryDataDAL
{
    static DataTable GetHistoryDataInsertTableSchema()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("ID", typeof(int)));
        dt.Columns.Add(new DataColumn("ProbeID", typeof(int)));
        dt.Columns.Add(new DataColumn("CheckTime", typeof(DateTime)));
        dt.Columns.Add(new DataColumn("GasValue", typeof(float)));
        dt.Columns.Add(new DataColumn("MachineID", typeof(int)));
        return dt;
    }

    static DataTable historyDataTable_ = GetHistoryDataInsertTableSchema();
    public static void AddHistoryData(int probeID, float decValue, int mchineID)
    {
        if (isInserting)
            return;
        if (historyDataTable_ != null)
        {
            DataRow row = historyDataTable_.NewRow();
            row[1] = probeID;
            row[2] = DateTime.Now;
            row[3] = decValue;
            row[4] = mchineID;
            historyDataTable_.Rows.Add(row);
            if (historyDataTable_.Rows.Count > 100)
            {
                isInserting = true;
                Thread t = new Thread(ThreadBulkInsert);
                t.Start();
            }
        }
    }

    static bool isInserting = false;
    static void ThreadBulkInsert()
    {
        try
        {
            if (historyDataTable_ != null && historyDataTable_.Rows.Count > 0)
            {
                SqlHelper.BulkInsert(historyDataTable_, "HistoryData");
                historyDataTable_.Clear();
                isInserting = false;
            }
        }
        catch (Exception ex)
        {
            LogHelper.AddLog("批量插入数据出错：" + ex.Message);
        }
    }
}
