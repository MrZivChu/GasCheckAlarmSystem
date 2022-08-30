﻿using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPanel : UIEventHelper
{
    public Button btn_login;
    public Text productNameText;
    public InputField input_name;
    public InputField input_pwd;
    public Toggle rememberPwdTog;

    string nameKey = "userName";
    string pwdKey = "userPwd";
    void Start()
    {
        productNameText.text = JsonHandleHelper.gameConfig.productName;
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

        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectUserByNamePwd");
        form.AddField("accountName", userName);
        form.AddField("accountPwd", userPwd);
        GameUtils.PostHttpWebRequest("User.ashx", form, (result) =>
        {
            string content = System.Text.Encoding.UTF8.GetString(result);
            if (content.Contains("error:"))
            {
                MessageBox.Instance.PopOK(content.Split(':')[1], null, "确定");
            }
            else
            {
                List<UserModel> list = JsonMapper.ToObject<List<UserModel>>(content);
                if (list.Count > 0)
                {
                    FormatData.currentUser = list[0];
                    GameUtils.SetString(nameKey, userName);
                    if (rememberPwdTog.isOn)
                        GameUtils.SetString(pwdKey, userPwd);
                    else
                        GameUtils.RemoveKey(pwdKey);
                    SceneManager.LoadScene("Main", LoadSceneMode.Single);
                }
                else
                {
                    MessageBox.Instance.PopOK("不存在此用户", null, "确定");
                }
            }
        }, (error) =>
        {
            MessageBox.Instance.PopOK(error, null, "确定");
        });
    }
}
