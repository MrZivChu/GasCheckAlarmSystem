using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditFactoryPanel : UIEventHelper
{
    public FactoryModel currentModel;
    public InputField input_factoryName;

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
        string factoryName = input_factoryName.text;

        WWWForm form = new WWWForm();
        form.AddField("requestType", "EditFactoryByID");
        form.AddField("id", currentModel.ID);
        form.AddField("factoryName", factoryName);
        GameUtils.PostHttpWebRequest("Factory.ashx", form, null, null);

        MessageBox.Instance.PopOK("修改成功", () =>
        {
            EventManager.Instance.DisPatch(NotifyType.UpdateFactoryList);
            gameObject.SetActive(false);
        }, "确定");
    }

    public void InitData(FactoryModel model)
    {
        currentModel = model;
        input_factoryName.text = model.FactoryName;
    }
}
