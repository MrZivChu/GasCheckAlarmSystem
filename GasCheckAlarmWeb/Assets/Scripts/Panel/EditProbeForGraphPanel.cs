using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditProbeForGraphPanel : UIEventHelper
{
    ProbeModel currentModel;
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
        deleteBtn.gameObject.SetActive(FormatData.currentUser.Authority == EAuthority.Admin);
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnDelete(Button btn)
    {
        MessageBox.Instance.PopYesNo("确认删除？", null, () =>
        {
            WWWForm form = new WWWForm();
            form.AddField("requestType", "DeleteProbePos2DByID");
            form.AddField("id", currentModel.ID);
            GameUtils.PostHttpWebRequest("Probe.ashx", form, null, null);
            EventManager.Instance.DisPatch(NotifyType.UpdatePos2D);
            gameObject.SetActive(false);
        }, "取消", "确定");
    }

    public void InitInfo(ProbeModel model)
    {
        currentModel = model;
        txt_name.text = currentModel.ProbeName;
        txt_gasKind.text = FormatData.gasKindFormat[currentModel.GasKind].name;
        txt_gasValue.text = model.GasValue.ToString();
        txt_firstValue.text = FormatData.gasKindFormat[model.GasKind].minValue.ToString();
        txt_secondValue.text = FormatData.gasKindFormat[model.GasKind].maxValue.ToString();
    }
}
