using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length < 0)
                return;

            DataTable mHistoryDataTable = new DataTable();
            mHistoryDataTable.Columns.Add(new DataColumn("ID", typeof(int)));
            mHistoryDataTable.Columns.Add(new DataColumn("ProbeID", typeof(int)));
            mHistoryDataTable.Columns.Add(new DataColumn("CheckTime", typeof(DateTime)));
            mHistoryDataTable.Columns.Add(new DataColumn("GasValue", typeof(float)));

            for (int i = 0; i < args.Length - 2; i += 4)
            {
                if (i + 3 < args.Length)
                {
                    DataRow row = mHistoryDataTable.NewRow();
                    row[1] = Convert.ToInt32(args[i]);
                    row[2] = Convert.ToDateTime(args[i + 1] + " " + args[i + 2]);
                    row[3] = Convert.ToSingle(args[i + 3]);
                    mHistoryDataTable.Rows.Add(row);
                }
            }
            string tabelName = "HistoryData";
            string connstr = "server=127.0.0.1;database=GasCheckAlarm;Integrated Security=false;User ID=sa;Password=1;";
            if (mHistoryDataTable != null && mHistoryDataTable.Rows.Count != 0 && !string.IsNullOrEmpty(tabelName))
            {
                using (SqlConnection conn = new SqlConnection(connstr))
                {
                    conn.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = tabelName;
                        bulkCopy.BulkCopyTimeout = 10;
                        bulkCopy.BatchSize = mHistoryDataTable.Rows.Count;
                        bulkCopy.WriteToServer(mHistoryDataTable);
                    }
                }
            }
        }
    }
}
