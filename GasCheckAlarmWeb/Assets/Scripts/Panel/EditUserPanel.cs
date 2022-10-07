using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditUserPanel : UIEventHelper
{
    public UserModel currentModel;
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
        string userName = input_userName.text;
        string userNumber = input_userNumber.text;
        string phone = input_phone.text;
        int authority = dropdown_authority.value;
        WWWForm form = new WWWForm();
        form.AddField("requestType", "EditUserByID");
        form.AddField("id", currentModel.ID);
        form.AddField("userName", userName);
        form.AddField("userNumber", userNumber);
        form.AddField("phone", phone);
        form.AddField("authority", authority);
        GameUtils.PostHttpWebRequest("User.ashx", form, null, null);
        MessageBox.Instance.PopOK("修改成功", () =>
        {
            EventManager.Instance.DisPatch(NotifyType.UpdateUserList);
            gameObject.SetActive(false);
        }, "确定");
    }

    public void InitData(UserModel model)
    {
        currentModel = model;
        input_userName.text = model.UserName;
        input_userNumber.text = model.UserNumber;
        input_phone.text = model.Phone;

        dropdown_authority.ClearOptions();
        foreach (var item in FormatData.authorityFormat)
        {
            Dropdown.OptionData data = new Dropdown.OptionData(item.Value);
            dropdown_authority.options.Add(data);
        }
        dropdown_authority.value = (int)model.Authority;
    }
}
