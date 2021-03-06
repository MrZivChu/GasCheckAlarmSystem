using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFactoryPanel : UIEventHelper
{
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
        form.AddField("requestType", "InsertFactory");
        form.AddField("factoryName", factoryName);
        GameUtils.PostHttp("Factory.ashx", form, null, null);

        MessageBox.Instance.PopOK("新增成功", () =>
        {
            EventManager.Instance.DisPatch(NotifyType.UpdateFactoryList);
            gameObject.SetActive(false);
        }, "确定");
    }

    private void OnEnable()
    {
        input_factoryName.text = string.Empty;
    }
}
