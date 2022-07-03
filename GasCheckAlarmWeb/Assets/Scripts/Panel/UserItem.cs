using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserItem : MonoBehaviour
{
    public UserModel currentModel;
    public Text txt_accountName;
    public Text txt_userName;
    public Text txt_userNumber;
    public Text txt_phone;
    public Text txt_authority;

    public Toggle tog_select;
    public Image img_background;
    void Start()
    {

    }

    public void InitData(UserModel model)
    {
        currentModel = model;
        txt_accountName.text = model.AccountName;
        txt_userName.text = model.UserName;
        txt_userNumber.text = model.UserNumber;
        txt_phone.text = model.Phone;
        txt_authority.text = model.Authority == 0 ? "普通用户" : "管理员";
    }

    public void SetToggle(bool isOn)
    {
        tog_select.isOn = isOn;
    }

    public bool GetToggleStatus()
    {
        return tog_select.isOn;
    }

    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
