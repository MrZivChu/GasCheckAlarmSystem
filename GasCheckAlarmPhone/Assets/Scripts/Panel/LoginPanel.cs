using LitJson;
using System;
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
        SqlHelper.InitSqlConnection("hds16173015.my3w.com", "hds16173015_db", "hds16173015", "!@#Dz123");
        RegisterBtnClick(btn_login, OnLogin);

        input_name.text = GameUtils.GetString(nameKey, "");
        input_pwd.text = GameUtils.GetString(pwdKey, "");
    }

    void OnLogin(Button btn)
    {
        List<GasTypesModel> gasTypesList = GasTypesDAL.SelectAllGasTypes();
        if (gasTypesList == null || gasTypesList.Count == 0)
        {
            MessageBox.Instance.PopOK("气体类型为空，请在Windows平台添加气体类型", null, "确定");
            return;
        }
        for (int j = 0; j < gasTypesList.Count; j++)
        {
            GasTypesModel model = gasTypesList[j];
            FormatData.gasKindFormat[model.ID] = model;
        }

        string userName = input_name.text;
        string userPwd = input_pwd.text;
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPwd))
        {
            MessageBox.Instance.PopOK("用户名或密码不能为空", null, "确定");
            return;
        }
        List<UserModel> list = UserDAL.SelectUserByNamePwd(userName, userPwd);
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
}
