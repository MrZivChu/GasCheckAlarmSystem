using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

public class HistoryDataDAL
{
    public static DataTable GetHistoryDataInsertTableSchema()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("ID", typeof(int)));
        dt.Columns.Add(new DataColumn("ProbeID", typeof(int)));
        dt.Columns.Add(new DataColumn("ProbeName", typeof(string)));
        dt.Columns.Add(new DataColumn("CheckTime", typeof(DateTime)));
        dt.Columns.Add(new DataColumn("GasValue", typeof(float)));
        dt.Columns.Add(new DataColumn("FactoryID", typeof(int)));
        dt.Columns.Add(new DataColumn("FactoryName", typeof(string)));
        dt.Columns.Add(new DataColumn("MachineID", typeof(int)));
        dt.Columns.Add(new DataColumn("MachineType", typeof(int)));
        dt.Columns.Add(new DataColumn("MachineName", typeof(string)));
        dt.Columns.Add(new DataColumn("GasKind", typeof(string)));
        dt.Columns.Add(new DataColumn("FirstAlarmValue", typeof(float)));
        dt.Columns.Add(new DataColumn("SecondAlarmValue", typeof(float)));
        return dt;
    }

    static DataTable historyDataTable_ = GetHistoryDataInsertTableSchema();
    public static void SingleInsertHistoryData(ProbeSerialPortInfo probeSerialPortInfo, float decValue)
    {
        if (isInserting)
            return;
        if (historyDataTable_ != null)
        {
            DataRow row = historyDataTable_.NewRow();
            row[1] = probeSerialPortInfo.ProbeID;
            row[2] = probeSerialPortInfo.ProbeName;
            row[3] = DateTime.Now;
            row[4] = decValue;
            row[5] = probeSerialPortInfo.FactoryID;
            row[6] = probeSerialPortInfo.FactoryName;
            row[7] = probeSerialPortInfo.MachineID;
            row[8] = probeSerialPortInfo.MachineType;
            row[9] = probeSerialPortInfo.MachineName;
            row[10] = probeSerialPortInfo.GasKind;
            row[11] = probeSerialPortInfo.FirstAlarmValue;
            row[12] = probeSerialPortInfo.SecondAlarmValue;
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
    private static void ThreadBulkInsert()
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
            File.WriteAllText("D:\\ThreadBulkInsertError.txt", "批量插入数据出错：" + ex.Message);
        }
    }
}
