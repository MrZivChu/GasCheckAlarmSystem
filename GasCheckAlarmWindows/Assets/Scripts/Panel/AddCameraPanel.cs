using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCameraPanel : UIEventHelper
{
    public InputField input_ip;
    public InputField input_port;
    public InputField input_userName;
    public InputField input_userPwd;

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
        if (string.IsNullOrEmpty(input_ip.text) || string.IsNullOrEmpty(input_port.text) || string.IsNullOrEmpty(input_userName.text) || string.IsNullOrEmpty(input_userPwd.text))
        {
            MessageBox.Instance.PopOK("每一项都必须填写!");
            return;
        }
        CameraDAL.InsertCamera(input_ip.text, input_port.text, input_userName.text, input_userPwd.text);
        MessageBox.Instance.PopOK("新增成功", () =>
        {
            EventManager.Instance.DisPatch(NotifyType.AddCamera);
            gameObject.SetActive(false);
        }, "确定");
    }
}
