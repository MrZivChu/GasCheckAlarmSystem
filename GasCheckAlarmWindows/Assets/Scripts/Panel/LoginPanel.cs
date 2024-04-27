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

        input_name.text = GameUtils.GetString(nameKey, "");
        input_pwd.text = GameUtils.GetString(pwdKey, "");
    }

    void OnLogin(Button btn)
    {
        List<GasTypesModel> gasTypesList = GasTypesDAL.SelectAllGasTypes();
        if (gasTypesList == null || gasTypesList.Count == 0)
        {
            MessageBox.Instance.PopOK("请先添加气体类型，按F5即可设置", null, "确定");
            return;
        }
        try
        {
            for (int j = 0; j < gasTypesList.Count; j++)
            {
                GasTypesModel model = gasTypesList[j];
                FormatData.gasKindFormat[model.ID] = model;
            }
        }
        catch (System.Exception ex)
        {
            productNameText.text = ex.Message;
            return;
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
