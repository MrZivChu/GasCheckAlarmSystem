using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

public class SqlHelper
{
    public static string connectionString = "server=hds16173015.my3w.com;database=hds16173015_db;Integrated Security=false;User ID=hds16173015;Password=!@#Dz123";

    public static int ExecuteNonQuery(string cmdText,
        params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }
    }

    public static object ExecuteScalar(string cmdText,
        params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }
    }

    public static DataTable ExecuteDataTable(string cmdText,
        params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }

    public static SqlDataReader ExecuteDataReader(string cmdText,
        params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }
    }

    /// <summary>
    /// 执行存储过程查询，返回SqlDataReader
    /// </summary>
    /// <param name="cmdText"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static SqlDataReader ExecuteProcQueryDataReader(string procName,
         params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }
    }

    /// <summary>
    /// 执行存储过程查询，返回DataSet
    /// </summary>
    /// <param name="cmdText"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static DataSet ExecuteProcQueryDataSet(string procName,
         params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataSet dt = new DataSet();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }


    //批量插入
    public static void BulkInsert(DataTable dt, string tabelName)
    {
        if (dt != null && dt.Rows.Count != 0 && !string.IsNullOrEmpty(tabelName))
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = tabelName;
                    bulkCopy.BulkCopyTimeout = 10;
                    bulkCopy.BatchSize = dt.Rows.Count;
                    bulkCopy.WriteToServer(dt);
                }
            }
        }
    }
}