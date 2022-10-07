using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

public class SqlHelper
{
    static string connectionString = "server = {0}; database = {1}; Integrated Security = false; User ID = {2}; Password = {3}";

    public static void InitSqlConnection(string sqlIP, string sqlDatabase, string sqlUserId, string sqlUserPwd)
    {
        connectionString = string.Format(connectionString, sqlIP, sqlDatabase, sqlUserId, sqlUserPwd);
    }

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
                int result = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return result;
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
                object result = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return result;
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
                    cmd.Parameters.Clear();
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
                SqlDataReader result = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return result;
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
                SqlDataReader result = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return result;
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
                    cmd.Parameters.Clear();
                    return dt;
                }
            }
        }
    }

    public static DataTable ExecProcPage(string cmdText, out int pageCount, out int rowCount, List<SqlParameter> para)
    {
        pageCount = 0;
        rowCount = 0;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                para[2].Direction = ParameterDirection.Output;
                para[3].Direction = ParameterDirection.Output;
                cmd.Parameters.AddRange(para.ToArray());
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    pageCount = Convert.ToInt32(cmd.Parameters[2].Value);
                    rowCount = Convert.ToInt32(cmd.Parameters[3].Value);
                    cmd.Parameters.Clear();
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
