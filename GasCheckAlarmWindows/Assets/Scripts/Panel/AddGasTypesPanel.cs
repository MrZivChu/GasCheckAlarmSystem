using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddGasTypesPanel : UIEventHelper
{
    public InputField gasName;
    public InputField gasMinvalue;
    public InputField gasMaxvalue;
    public InputField expression;
    public Button btn_cancel;
    public Button btn_add;
    public Text tipText;

    void Start()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_add, OnAdd);
    }

    private void OnEnable()
    {
        gasName.text = string.Empty;
        gasMinvalue.text = string.Empty;
        gasMaxvalue.text = string.Empty;
        expression.text = string.Empty;
        tipText.text = string.Empty;
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnAdd(Button btn)
    {
        float min = Convert.ToSingle(gasMinvalue.text);
        float max = Convert.ToSingle(gasMaxvalue.text);
        if (max <= min)
        {
            tipText.text = "最大值不能小于等于最小值";
            return;
        }
        GasTypesDAL.InsertGasType(gasName.text, min, max, expression.text);
        EventManager.Instance.DisPatch(NotifyType.InsertGasTypes);
        gameObject.SetActive(false);
    }

}