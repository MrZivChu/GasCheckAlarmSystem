using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddUserPanel : UIEventHelper
{
    public InputField input_accountName;
    public InputField input_accountPwd;
    public InputField input_userName;
    public InputField input_userNumber;
    public InputField input_phone;
    public Dropdown dropdown_authority;

    public Button btn_cancel;
    public Button btn_ok;

    void Start()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_ok, OnOk);
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnOk(Button btn)
    {
        string accountName = input_accountName.text;
        string accountPwd = input_accountPwd.text;
        string userName = input_userName.text;
        string address = input_userNumber.text;
        string number = input_phone.text;
        int authority = dropdown_authority.value;
        WWWForm form = new WWWForm();
        form.AddField("requestType", "InsertUser");
        form.AddField("accountName", accountName);
        form.AddField("accountPwd", accountPwd);
        form.AddField("userName", userName);
        form.AddField("userNumber", address);
        form.AddField("phone", number);
        form.AddField("authority", authority);
        GameUtils.PostHttp("User.ashx", form, null, null);
        MessageBox.Instance.PopOK("新增成功", () =>
        {
            EventManager.Instance.DisPatch(NotifyType.UpdateUserList);
            gameObject.SetActive(false);
        }, "确定");
    }

    private void OnEnable()
    {
        input_accountName.text = string.Empty;
        input_accountPwd.text = string.Empty;
        input_userName.text = string.Empty;
        input_userNumber.text = string.Empty;
        input_phone.text = string.Empty;

        dropdown_authority.ClearOptions();
        List<string> optionList = new List<string>() { "普通用户", "管理员" };
        dropdown_authority.AddOptions(optionList);
        dropdown_authority.value = 0;
    }
}
