using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ExamSystem
{
    public class DBHelper
    {
        private SqlConnection connection;
        /// <summary>
        /// 数据库连接对象属性
        /// </summary>
        public SqlConnection Connection
        {
            get
            {
                string connectionString = "server=.;database=GCXY;Integrated Security=true";
                if (connection == null)
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();
                }
                else if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                else if (connection.State == System.Data.ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }
                return connection;
            }
        }


        /// <summary>
        /// 执行小版块分页
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagecount"></param>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public DataTable ProGetLittleLogo(int pageindex, int pagesize, out int pagecount, out int rowcount)
        {
            pagecount = 0;
            rowcount = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = Connection;
                cmd.CommandText = "ProTopicLogoLittle";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] para = { 
                                  new SqlParameter("@pageIndex",pageindex),
                                  new SqlParameter("@pageSize",pagesize),
                                  new SqlParameter("@pageCount",pagecount),
                                  new SqlParameter("@rowCount",rowcount),
                                      };
                para[2].Direction = ParameterDirection.Output;
                para[3].Direction = ParameterDirection.Output;
                cmd.Parameters.AddRange(para);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                pagecount = Convert.ToInt32(cmd.Parameters[2].Value);
                rowcount = Convert.ToInt32(cmd.Parameters[3].Value);
                return dt;
            }
        }


        /// <summary>
        /// 关注分页
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="userid"></param>
        /// <param name="pagecount"></param>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public DataTable ProGuanzhu(int pageindex, int pagesize, int userid, out int pagecount, out int rowcount)
        {
            pagecount = 0;
            rowcount = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = Connection;
                cmd.CommandText = "ProGuanzhu";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] para = { 
                                  new SqlParameter("@pageindex",pageindex),
                                  new SqlParameter("@pagesize",pagesize),
                                  new SqlParameter("@userid",userid),
                                  new SqlParameter("@pagecount",pagecount),
                                  new SqlParameter("@rowcount",rowcount),
                                      };
                para[3].Direction = ParameterDirection.Output;
                para[4].Direction = ParameterDirection.Output;
                cmd.Parameters.AddRange(para);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                pagecount = Convert.ToInt32(cmd.Parameters[3].Value);
                rowcount = Convert.ToInt32(cmd.Parameters[4].Value);
                return dt;
            }
        }


        /// <summary>
        /// 用户分页
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagecount"></param>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public DataTable ProUser(int pageindex, int pagesize, out int pagecount, out int rowcount)
        {
            pagecount = 0;
            rowcount = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = Connection;
                cmd.CommandText = "ProUser";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] para = { 
                                  new SqlParameter("@pageIndex",pageindex),
                                  new SqlParameter("@pageSize",pagesize),
                                  new SqlParameter("@pageCount",pagecount),
                                  new SqlParameter("@rowCount",rowcount),
                                      };
                para[2].Direction = ParameterDirection.Output;
                para[3].Direction = ParameterDirection.Output;
                cmd.Parameters.AddRange(para);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                pagecount = Convert.ToInt32(cmd.Parameters[2].Value);
                rowcount = Convert.ToInt32(cmd.Parameters[3].Value);
                return dt;
            }
        }


        /// <summary>
        /// 帖子分页
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagecount"></param>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public DataTable Protopic(int pageindex, int pagesize, out int pagecount, out int rowcount)
        {
            pagecount = 0;
            rowcount = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = Connection;
                cmd.CommandText = "Protopic";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] para = { 
                                  new SqlParameter("@pageIndex",pageindex),
                                  new SqlParameter("@pageSize",pagesize),
                                  new SqlParameter("@pageCount",pagecount),
                                  new SqlParameter("@rowCount",rowcount),
                                      };
                para[2].Direction = ParameterDirection.Output;
                para[3].Direction = ParameterDirection.Output;
                cmd.Parameters.AddRange(para);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                pagecount = Convert.ToInt32(cmd.Parameters[2].Value);
                rowcount = Convert.ToInt32(cmd.Parameters[3].Value);
                return dt;
            }
        }



        /// <summary>
        /// 关键字分页
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagecount"></param>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public DataTable ProKeyWord(int pageindex, int pagesize, out int pagecount, out int rowcount)
        {
            pagecount = 0;
            rowcount = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = Connection;
                cmd.CommandText = "ProKeyWord";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] para = { 
                                  new SqlParameter("@pageIndex",pageindex),
                                  new SqlParameter("@pageSize",pagesize),
                                  new SqlParameter("@pageCount",pagecount),
                                  new SqlParameter("@rowCount",rowcount),
                                      };
                para[2].Direction = ParameterDirection.Output;
                para[3].Direction = ParameterDirection.Output;
                cmd.Parameters.AddRange(para);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                pagecount = Convert.ToInt32(cmd.Parameters[2].Value);
                rowcount = Convert.ToInt32(cmd.Parameters[3].Value);
                return dt;
            }
        }

        /// <summary>
        /// 执行TopicLogo表大版块的分页
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagecount"></param>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public DataTable ProGetTopicLogo(int pageindex, int pagesize, out int pagecount, out int rowcount)
        {
            pagecount = 0;
            rowcount = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "ProTopicLogo";
                cmd.Connection = Connection;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] para = {
                              new SqlParameter("@pageIndex",pageindex), 
                              new SqlParameter("@pageSize",pagesize), 
                              new SqlParameter("@pageCount",pagecount), 
                              new SqlParameter("@rowCount",rowcount), 
                                      };
                para[2].Direction = ParameterDirection.Output;
                para[3].Direction = ParameterDirection.Output;
                cmd.Parameters.AddRange(para);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                pagecount = Convert.ToInt32(cmd.Parameters[2].Value);
                rowcount = Convert.ToInt32(cmd.Parameters[3].Value);
                return dt;
            }
        }




        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseDB()
        {
            if (connection.State == System.Data.ConnectionState.Open || connection.State == System.Data.ConnectionState.Broken)
            {
                connection.Close();
            }
        }


        public DataTable ProGetTopicByBianma(int pageindex, int pagesize, int topicbianma, out int pagecount, out int rowcount)
        {
            pagecount = 0;
            rowcount = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "louzhutiezi";
                cmd.Connection = Connection;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] para = { 
                                     new SqlParameter("@pageSize",pagesize),
                                    new SqlParameter("@pageIndex",pageindex),
                                      new SqlParameter("@topicBianma",topicbianma),
                                       new SqlParameter("@pageCount",pagecount),
                                      new SqlParameter("@RowCount",rowcount)
                                    };
                para[3].Direction = ParameterDirection.Output;
                para[4].Direction = ParameterDirection.Output;
                cmd.Parameters.AddRange(para);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                pagecount = Convert.ToInt32(cmd.Parameters[3].Value);
                rowcount = Convert.ToInt32(cmd.Parameters[4].Value);
                return dt;
            }
        }


        /// <summary>
        /// 找出审核通过的回帖信息
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="topicbianma"></param>
        /// <param name="pagecount"></param>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public DataTable ProGetHuitieMsg(int pageindex, int pagesize, int ToWhotopicNum, out int pagecount, out int rowcount)
        {
            pagecount = 0;
            rowcount = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "answers";
                cmd.Connection = Connection;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] para = { 
                                     new SqlParameter("@pageSize",pagesize),
                                    new SqlParameter("@pageIndex",pageindex),
                                      new SqlParameter("@ToWhotopicNum",ToWhotopicNum),
                                       new SqlParameter("@pageCount",pagecount),
                                      new SqlParameter("@RowCount",rowcount)
                                    };
                para[3].Direction = ParameterDirection.Output;
                para[4].Direction = ParameterDirection.Output;
                cmd.Parameters.AddRange(para);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                pagecount = Convert.ToInt32(cmd.Parameters[3].Value);
                rowcount = Convert.ToInt32(cmd.Parameters[4].Value);
                return dt;
            }
        }















        #region 执行分页存储过程,并输出总行数和总页数
        /// <summary>
        /// 执行分页存储过程,并输出总行数和总页数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="iDName">主键名</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="where">查询条件 id=1</param>
        /// <param name="orderby">排序条件--order by id</param>
        /// <param name="rowCount">out总行数</param>
        /// <param name="pageCount">out总页数</param>
        /// <returns></returns>
        public DataTable ExecProPageList(string tableName, string iDName, int pageIndex, int pageSize, string where, string orderby, out int rowCount, out int pageCount)
        {
            rowCount = 0;
            pageCount = 0;
            SqlParameter[] parameters = {
                new SqlParameter("@tn", SqlDbType.NVarChar,30),
                new SqlParameter("@idn", SqlDbType.NVarChar,20),
				new SqlParameter("@pi", SqlDbType.Int,4),
                new SqlParameter("@ps", SqlDbType.Int,4),
                new SqlParameter("@wh", SqlDbType.NVarChar,255),
                new SqlParameter("@rc", SqlDbType.Int,4),
                new SqlParameter("@pc", SqlDbType.Int,4),
                new SqlParameter("@oby", SqlDbType.NVarChar,255)};
            parameters[0].Value = tableName;
            parameters[1].Value = iDName;
            parameters[2].Value = pageIndex;
            parameters[3].Value = pageSize;
            parameters[4].Value = where;
            parameters[5].Value = rowCount;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[6].Value = pageCount;
            parameters[6].Direction = ParameterDirection.Output;
            parameters[7].Value = orderby;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Connection;
            cmd.CommandText = "GetPageDataSimple";// "GetPageDataOut";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parameters);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            rowCount = Convert.ToInt32(cmd.Parameters["@rc"].Value);
            pageCount = Convert.ToInt32(cmd.Parameters["@pc"].Value);
            return dt;
        }
        #endregion






        /// <summary>
        /// 执行分页存储过程
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="iDName">主键名</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public DataTable ExecProPageList(string tableName, string iDName, int pageIndex, int pageSize, string where)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@tn", SqlDbType.NVarChar,30),
                new SqlParameter("@idn", SqlDbType.NVarChar,20),
				new SqlParameter("@pi", SqlDbType.Int,4),
                new SqlParameter("@ps", SqlDbType.Int,4),
                new SqlParameter("@wh", SqlDbType.NVarChar,255)};
            parameters[0].Value = tableName;
            parameters[1].Value = iDName;
            parameters[2].Value = pageIndex;
            parameters[3].Value = pageSize;
            parameters[4].Value = where;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Connection;
            cmd.CommandText = "GetPageData";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parameters);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }




        /// <summary>
        /// 执行存储过程 - 返回受影响行数
        /// </summary>
        /// <param name="ProName">存储过程名</param>
        /// <param name="values">sql参数数组</param>
        /// <returns></returns>
        public int ExecProNonQ(string ProName, params SqlParameter[] values)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Connection;
            cmd.CommandText = ProName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(values);
            int result = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return result;
        }

        /// <summary>
        /// 执行存储过程 - 返回数据表
        /// </summary>
        /// <param name="ProName">存储过程名</param>
        /// <param name="values">sql参数数组</param>
        /// <returns></returns>
        public DataTable ExecProDataTable(string ProName, params SqlParameter[] values)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Connection;
            cmd.CommandText = ProName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(values);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmd.Parameters.Clear();
            return dt;
        }

        /// <summary>
        /// 执行存储过程 - 赋值给引用数据表
        /// </summary>
        /// <param name="ProName">存储过程名</param>
        /// <param name="dt">数据表对象</param>
        /// <param name="values">sql参数数组</param>
        /// <returns></returns>
        public void ExecProDataTable(string ProName, ref DataTable dt, params SqlParameter[] values)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Connection;
            cmd.CommandText = ProName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(values);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.Parameters.Clear();
            da.Fill(dt);
        }

        /// <summary>
        /// 执行存储过程 - 返回数据集
        /// </summary>
        /// <param name="ProName">存储过程名</param>
        /// <param name="values">sql参数数组</param>
        /// <returns></returns>
        public DataSet ExecProDataSet(string ProName, params SqlParameter[] values)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Connection;
            cmd.CommandText = ProName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(values);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="ProName">存储过程名</param>
        /// <param name="ds">数据集对象</param>
        /// <param name="values">sql参数数组</param>
        /// <returns></returns>
        public DataSet ExecProDataSet(string ProName, ref DataSet ds, params SqlParameter[] values)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Connection;
            cmd.CommandText = ProName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(values);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }

        /// <summary>
        /// 执行Command
        /// </summary>
        /// <param name="safeSql">sql语句</param>
        /// <returns></returns>
        public int ExecuteCommand(string safeSql)
        {
            SqlCommand cmd = new SqlCommand(safeSql, Connection);
            int result = cmd.ExecuteNonQuery();
            return result;
        }

        /// <summary>
        /// 执行Command
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="values">sql参数数组</param>
        /// <returns></returns>
        public int ExecuteCommand(string sql, params SqlParameter[] values)
        {
            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.Parameters.AddRange(values);
            int result = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return result;
        }

        /// <summary>
        /// 执行Scalar
        /// </summary>
        /// <param name="safeSql">sql语句</param>
        /// <returns></returns>
        public int GetScalar(string safeSql)
        {
            SqlCommand cmd = new SqlCommand(safeSql, Connection);
            int result = Convert.ToInt32(cmd.ExecuteScalar());
            return result;
        }

        /// <summary>
        /// 执行带sql参数的语句
        /// </summary>
        /// <param name="safeSql">sql参数数组</param>
        /// <returns></returns>
        public int GetScalar(params SqlParameter[] values)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Connection;
            cmd.CommandText = "Pro_InsertOrder";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(values);
            int result = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Parameters.Clear();
            return result;
        }

        /// <summary>
        /// 执行带sql参数的语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="values">sql参数列表</param>
        /// <returns></returns>
        public int GetScalar(string sql, params SqlParameter[] values)
        {
            object obj = null;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.Parameters.AddRange(values);
                obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseDB();
            }
            if (obj == null)
                return 0;
            else
                return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 执行sql语句 返回数据读取器对象
        /// </summary>
        /// <param name="safeSql">sql语句</param>
        /// <returns>数据读取器对象</returns>
        public SqlDataReader GetReader(string safeSql)
        {
            SqlCommand cmd = new SqlCommand(safeSql, Connection);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        /// <summary>
        /// 执行sql语句 返回数据读取器对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="values">sql参数列表</param>
        /// <returns>数据读取器对象</returns>
        public SqlDataReader GetReader(string sql, params SqlParameter[] values)
        {
            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.Parameters.AddRange(values);
            SqlDataReader reader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }

        /// <summary>
        /// 执行sql语句 返回数据集
        /// </summary>
        /// <param name="safeSql">sql语句</param>
        /// <returns>数据集</returns>
        public DataSet GetDataSet(string safeSql)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(safeSql, Connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            return ds;
        }

        /// <summary>
        /// 执行sql语句 返回数据集
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="values">sql参数列表</param>
        /// <returns>数据集</returns>
        public DataSet GetDataSet(string sql, params SqlParameter[] values)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.Parameters.AddRange(values);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }

        /// <summary>
        /// 执行sql语句 返回数据表
        /// </summary>
        /// <param name="safeSql">sql语句</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(string safeSql)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(safeSql, Connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            return ds.Tables[0];
        }
        /// <summary>
        /// 执行sql语句 返回数据表
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="values">sql参数列表</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(string sql, params SqlParameter[] values)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.Parameters.AddRange(values);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds.Tables[0];
        }

        /// <summary>
        /// 返回查询后的数据表第一行
        /// </summary>
        /// <param name="safeSql">安全的sql语句</param>
        public DataRow GetDataRow(string safeSql)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(safeSql, Connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            return ds.Tables[0].Rows[0];
        }

        /// <summary>
        /// 返回查询后的数据表第一行
        /// </summary>
        /// <param name="sql">安全的sql语句</param>
        /// <param name="values">sql参数集合</param>
        public DataRow GetDataRow(string sql, params SqlParameter[] values)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.Parameters.AddRange(values);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Parameters.Clear();
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }
    }
}
