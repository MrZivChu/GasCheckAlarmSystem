using System.Data;
using System.Data.SqlClient;

public class SqlHelper
{
    static string connectionString = "server={0};database={1};Integrated Security=false;User ID={2};Password={3}";

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