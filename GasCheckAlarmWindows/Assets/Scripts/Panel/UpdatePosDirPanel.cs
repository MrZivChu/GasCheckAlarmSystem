using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePosDirPanel : UIEventHelper
{
    [HideInInspector]
    public string posDir;

    public InputField input_probeID;
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
        int result = 0;
        if (int.TryParse(input_probeID.text, out result))
        {
            ProbeDAL.EditProbePosDirByID(result, posDir);
            EventManager.Instance.DisPatch(NotifyType.UpdateProbeList);
            gameObject.SetActive(false);
        }
    }
}
