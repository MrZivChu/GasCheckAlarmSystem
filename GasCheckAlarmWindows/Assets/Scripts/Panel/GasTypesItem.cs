using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GasTypesItem : UIEventHelper
{
    public GasTypesModel currentModel;
    public InputField gasName;
    public InputField gasMinvalue;
    public InputField gasMaxvalue;
    public InputField expression;
    public Button deleteBtn;

    private void Start()
    {
        RegisterBtnClick(deleteBtn, OnDelete);
        RegisterInputFieldOnEndEdit(gasName, OnInputFieldEnd);
        RegisterInputFieldOnEndEdit(expression, OnInputFieldEnd);
        RegisterInputFieldOnEndEdit(gasMinvalue, OnInputFieldEnd);
        RegisterInputFieldOnEndEdit(gasMaxvalue, OnInputFieldEnd);
    }

    void OnDelete(Button btn)
    {
        GasTypesDAL.DeleteGasTypeByID(currentModel.ID.ToString());
        EventManager.Instance.DisPatch(NotifyType.DeleteGasTypes);
    }

    void OnInputFieldEnd(InputField input, string content)
    {
        int micifang = 0;
        if (!int.TryParse(expression.text, out micifang))
        {
            return;
        }
        GasTypesDAL.EditGasTypeByID(currentModel.ID, gasName.text, Convert.ToSingle(gasMinvalue.text), Convert.ToSingle(gasMaxvalue.text), micifang.ToString());
    }

    public void InitInfo(GasTypesModel model)
    {
        currentModel = model;
        gasName.text = model.GasName;
        gasMinvalue.text = model.MinValue.ToString();
        gasMaxvalue.text = model.MaxValue.ToString();
        expression.text = model.Expression;
    }

}
