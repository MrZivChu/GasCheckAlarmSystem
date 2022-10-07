<%@ WebHandler Language="C#" Class="Login" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

public class Login : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        SqlHelper.InitSqlConnection("hds16173015.my3w.com", "hds16173015_db", "hds16173015", "!@#Dz123");

        string content = string.Empty;
        string requestType = context.Request["requestType"];
        if (requestType == "SelectUserByNamePwd")
        {
            string username = context.Request["accountName"];
            string userpwd = context.Request["accountPwd"];
            List<UserModel> list = UserDAL.SelectUserByNamePwd(username, userpwd);
            if (list.Count <= 0)
            {
                content = "error:不存在此用户";
            }
            else
            {
                content = JsonConvert.SerializeObject(list);
            }
        }
        else if (requestType == "SelectAllUser")
        {
            List<UserModel> list = UserDAL.SelectAllUser();
            if (list.Count > 0)
            {
                content = JsonConvert.SerializeObject(list);
            }
        }
        else if (requestType == "DeleteUserByID")
        {
            string idList = context.Request["idList"];
            UserDAL.DeleteUserByID(idList);
        }
        else if (requestType == "EditUserByID")
        {
            int id = Convert.ToInt32(context.Request["id"]);
            string userName = context.Request["userName"];
            string userNumber = context.Request["userNumber"];
            string phone = context.Request["phone"];
            int authority = Convert.ToInt32(context.Request["authority"]);
            UserDAL.EditUserByID(id, userName, userNumber, phone, authority);
        }
        else if (requestType == "InsertUser")
        {
            string accountName = context.Request["accountName"];
            string accountPwd = context.Request["accountPwd"];
            string userName = context.Request["userName"];
            string userNumber = context.Request["userNumber"];
            string phone = context.Request["phone"];
            int authority = Convert.ToInt32(context.Request["authority"]);
            UserDAL.InsertUser(accountName, accountPwd, userName, userNumber, phone, authority);
        }
        else if (requestType == "SelectAllUserByCondition")
        {
            string userName = context.Request["userName"];
            string userNumber = context.Request["userNumber"];
            string phone = context.Request["phone"];
            int authority = Convert.ToInt32(context.Request["authority"]);
            List<UserModel> list = UserDAL.SelectAllUserByCondition(userName, userNumber, phone, authority);
            if (list.Count > 0)
            {
                content = JsonConvert.SerializeObject(list);
            }
        }
        context.Response.Write(content);
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}