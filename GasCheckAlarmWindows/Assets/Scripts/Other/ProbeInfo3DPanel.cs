using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProbeInfo3DPanel : UIEventHelper
{
    public ProbeModel currentModel;

    public Text txt_name;
    public Text txt_gasKind;
    public Text txt_gasValue;
    public Text txt_firstValue;
    public Text txt_secondValue;

    public Button deleteBtn;
    public Button okBtn;
    private void Start()
    {
        deleteBtn.gameObject.SetActive(FormatData.currentUser.Authority == 1);
        RegisterBtnClick(deleteBtn, OnDelete);
        RegisterBtnClick(okBtn, OnOk);
    }

    void OnOk(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnDelete(Button btn)
    {
        MessageBox.Instance.PopYesNo("确认删除？", null, () =>
        {
            gameObject.SetActive(false);
            ProbeDAL.DeleteProbeByID(currentModel.ID.ToString());
            EventManager.Instance.DisPatch(NotifyType.UpdateProbeList);
            MessageBox.Instance.PopOK("删除成功", null, "确定");
        }, "取消", "确定");
    }

    public void InitInfo(ProbeModel model)
    {
        currentModel = model;
        txt_name.text = currentModel.ProbeName;
        txt_gasKind.text = currentModel.GasKind;
        txt_gasValue.text = "0";
        txt_firstValue.text = currentModel.FirstAlarmValue.ToString();
        txt_secondValue.text = currentModel.SecondAlarmValue.ToString();
    }

    public void RefreshRealtimeData(float gasValue)
    {
        txt_gasValue.text = gasValue.ToString();
    }
}
