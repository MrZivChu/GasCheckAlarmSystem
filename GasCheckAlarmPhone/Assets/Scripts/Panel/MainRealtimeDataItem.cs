using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainRealtimeDataItem : UIEventHelper
{
    public ProbeModel currentModel;
    GameObject target = null;

    public Text txt_probeName;
    public Text txt_gasValue;
    public Text txt_firstAlarmValue;
    public Text txt_secondAlarmValue;
    public Image img_background;
    public Button btn_focus;

    void Start()
    {
        RegisterBtnClick(btn_focus, OnFocus);
    }

    void OnFocus(Button btn)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera && target)
        {
            Vector3 direction = -target.transform.forward;
            mainCamera.transform.position = target.transform.position + direction.normalized * 10;
            mainCamera.transform.LookAt(target.transform);
        }
    }

    public void InitData(ProbeModel model, GameObject go)
    {
        target = go;
        currentModel = model;
        txt_probeName.text = model.ProbeName;
        txt_gasValue.text = model.GasValue.ToString();
        txt_firstAlarmValue.text = FormatData.gasKindFormat[model.GasKind].minValue.ToString();
        txt_secondAlarmValue.text = FormatData.gasKindFormat[model.GasKind].maxValue.ToString();
    }

    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
