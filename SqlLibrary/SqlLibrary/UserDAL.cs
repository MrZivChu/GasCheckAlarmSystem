using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class UserDAL
{
    public static List<UserModel> SelectUserByNamePwd(string accountName, string accountPwd)
    {
        string sql = "select * from Users where AccountName=@AccountName and AccountPwd=@AccountPwd";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@AccountName",accountName) ,
                 new SqlParameter("@AccountPwd",accountPwd),
            };
        List<UserModel> modelList = new List<UserModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, parameter);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                UserModel model = new UserModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.AccountName = dt.Rows[i]["AccountName"].ToString();
                model.AccountPwd = dt.Rows[i]["AccountPwd"].ToString();
                model.UserName = dt.Rows[i]["UserName"].ToString();
                model.UserNumber = dt.Rows[i]["UserNumber"].ToString();
                model.Authority = (EAuthority)(dt.Rows[i]["Authority"]);
                model.Phone = dt.Rows[i]["Phone"].ToString();

                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static List<UserModel> SelectAllUser()
    {
        string sql = "select * from Users ";
        List<UserModel> modelList = new List<UserModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                UserModel model = new UserModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.AccountName = dt.Rows[i]["AccountName"].ToString();
                model.AccountPwd = dt.Rows[i]["AccountPwd"].ToString();
                model.UserName = dt.Rows[i]["UserName"].ToString();
                model.UserNumber = dt.Rows[i]["UserNumber"].ToString();
                model.Authority = (EAuthority)(dt.Rows[i]["Authority"]);
                model.Phone = dt.Rows[i]["Phone"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }

    public static bool DeleteUserByID(string idList)
    {
        string sql = @"exec('delete from Users where ID in ('+@ID+')')";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",idList)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool EditUserByID(int id, string userName, string userNumber, string phone, int authority)
    {
        string sql = @"update Users set UserName=@UserName,UserNumber=@UserNumber,Phone=@Phone,Authority=@Authority where ID=@ID";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@ID",id),
                 new SqlParameter("@UserName",userName),
                 new SqlParameter("@UserNumber",userNumber),
                 new SqlParameter("@Phone",phone),
                 new SqlParameter("@Authority",authority)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static bool InsertUser(string accountName, string accountPwd, string userName, string userNumber, string phone, int authority)
    {
        string sql = @"insert into Users (AccountName,AccountPwd,UserName,UserNumber,Phone,Authority)values(@AccountName,@AccountPwd,@UserName,@UserNumber,@Phone,@Authority)";
        SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@AccountName",accountName),
                 new SqlParameter("@AccountPwd",accountPwd),
                 new SqlParameter("@UserName",userName),
                 new SqlParameter("@UserNumber",userNumber),
                 new SqlParameter("@Phone",phone),
                 new SqlParameter("@Authority",authority)
            };
        int result = SqlHelper.ExecuteNonQuery(sql, parameter);
        return result >= 1 ? true : false;
    }

    public static List<UserModel> SelectAllUserByCondition(string userName, string userNumber, string Phone, int authority)
    {
        string sql = @"select * from Users where 1=1 ";
        if (!string.IsNullOrEmpty(userName))
            sql += " and UserName like '%" + userName + "%' ";
        if (!string.IsNullOrEmpty(userNumber))
            sql += " and UserNumber like '%" + userNumber + "%' ";
        if (!string.IsNullOrEmpty(Phone))
            sql += " and Phone like '%" + Phone + "%' ";
        if (authority == 1 || authority == 0)
            sql += " and Authority like '%" + authority + "%' ";
        List<UserModel> modelList = new List<UserModel>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                UserModel model = new UserModel();
                model.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                model.AccountName = dt.Rows[i]["AccountName"].ToString();
                model.AccountPwd = dt.Rows[i]["AccountPwd"].ToString();
                model.UserName = dt.Rows[i]["UserName"].ToString();
                model.UserNumber = dt.Rows[i]["UserNumber"].ToString();
                model.Authority = (EAuthority)(dt.Rows[i]["Authority"]);
                model.Phone = dt.Rows[i]["Phone"].ToString();
                modelList.Add(model);
            }
        }
        return modelList;
    }
}
