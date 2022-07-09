using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProbeInfo3DPanel : UIEventHelper
{
    ProbeModel currentModel;
    public Text txt_name;
    public Text txt_gasKind;
    public Text txt_gasValue;
    public Text txt_firstValue;
    public Text txt_secondValue;

    public Button deleteBtn;
    private void Start()
    {
        RegisterBtnClick(deleteBtn, OnDelete);
    }

    void OnDelete(Button btn)
    {
        MessageBox.Instance.PopYesNo("确认删除？", null, () =>
        {
            WWWForm form = new WWWForm();
            form.AddField("requestType", "DeleteProbeByID");
            form.AddField("idList", currentModel.ID.ToString());
            GameUtils.PostHttp("Probe.ashx", form, null, null);

            EventManager.Instance.DisPatch(NotifyType.UpdateProbeList);
            ProbeInSceneHelper.instance.DeleteProbe(currentModel);
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

    void Update()
    {
        if (Camera.main)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(Vector3.up, 180);

            float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            if (distance > 20)
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
