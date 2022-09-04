using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWaterSealPanel : UIEventHelper
{
    public InputField input_Medium;
    public InputField input_Number;
    public InputField input_InstallPosition;
    public InputField input_Category;
    public InputField input_DesignPressure;
    public InputField input_SerialNumber;

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
        string medium = input_Medium.text;
        string number = input_Number.text;
        string installPosition = input_InstallPosition.text;
        string category = input_Category.text;
        string designPressure = input_DesignPressure.text;
        string serialNumber = input_SerialNumber.text;
        WWWForm form = new WWWForm();
        form.AddField("requestType", "InsertWaterSeal");
        form.AddField("medium", medium);
        form.AddField("number", number);
        form.AddField("installPosition", installPosition);
        form.AddField("category", category);
        form.AddField("designPressure", designPressure);
        form.AddField("serialNumber", serialNumber);
        GameUtils.PostHttp("WaterSeal.ashx", form, (result) =>
        {
            int insertID = Convert.ToInt32(result);
            MessageBox.Instance.PopOK("新增成功", () =>
            {
                EventManager.Instance.DisPatch(NotifyType.UpdateWaterSealList);
                gameObject.SetActive(false);
            }, "确定");
        }, null);
    }
}
