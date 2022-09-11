using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditProbeForGraphPanel : UIEventHelper
{
    RealtimeDataModel currentModel;
    public Text txt_name;
    public Text txt_gasKind;
    public Text txt_gasValue;
    public Text txt_firstValue;
    public Text txt_secondValue;

    public Button deleteBtn;
    public Button btn_cancel;
    private void Start()
    {
        RegisterBtnClick(deleteBtn, OnDelete);
        RegisterBtnClick(btn_cancel, OnCancel);
        deleteBtn.gameObject.SetActive(FormatData.currentUser.Authority == 1);
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnDelete(Button btn)
    {
        MessageBox.Instance.PopYesNo("确认删除？", null, () =>
        {
            RealtimeDataDAL.DeleteRealtimePos2DByID(currentModel.ProbeID);
            MessageBox.Instance.PopOK("删除成功", () =>
            {
                gameObject.SetActive(false);
            }, "确定");
            gameObject.SetActive(false);
        }, "取消", "确定");
    }

    public void InitInfo(RealtimeDataModel model)
    {
        currentModel = model;
        txt_name.text = currentModel.ProbeName;
        txt_gasKind.text = currentModel.GasKind;
        txt_gasValue.text = model.GasValue.ToString();
        txt_firstValue.text = currentModel.FirstAlarmValue.ToString();
        txt_secondValue.text = currentModel.SecondAlarmValue.ToString();
    }
}
