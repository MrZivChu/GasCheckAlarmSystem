using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPanel : UIEventHelper
{
    public Button btn_login;
    public InputField input_name;
    public InputField input_pwd;
    public Toggle rememberPwdTog;

    string nameKey = "userName";
    string pwdKey = "userPwd";
    void Start()
    {
        RegisterBtnClick(btn_login, OnLogin);

        bool hasNameKey = GameUtils.HasKey(nameKey);
        if (hasNameKey)
        {
            input_name.text = GameUtils.GetString(nameKey, "");
        }
        bool hasPwdKey = GameUtils.HasKey(pwdKey);
        if (hasPwdKey)
        {
            input_pwd.text = GameUtils.GetString(pwdKey, "");
        }
    }

    void OnLogin(Button btn)
    {
        string userName = input_name.text;
        string userPwd = input_pwd.text;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPwd))
        {
            MessageBox.Instance.PopOK("用户名或密码不能为空", null, "确定");
            return;
        }

        List<UserModel> userList = UserDAL.SelectUserByNamePwd(userName, userPwd);
        if (userList.Count > 0)
        {
            FormatData.currentUser = userList[0];
            GameUtils.SetString(nameKey, userName);
            if (rememberPwdTog.isOn)
                GameUtils.SetString(pwdKey, userPwd);
            else
                GameUtils.RemoveKey(pwdKey);
            SceneManager.LoadScene("Env", LoadSceneMode.Single);
        }
        else
        {
            MessageBox.Instance.PopOK("不存在此用户", null, "确定");
        }
    }
}
