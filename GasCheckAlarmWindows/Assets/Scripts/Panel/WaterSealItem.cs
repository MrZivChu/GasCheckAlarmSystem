using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterSealItem : UIEventHelper
{
    public WaterSealModel currentModel;
    public Text txt_Medium;
    public Text txt_Number;
    public Text txt_InstallPosition;
    public Text txt_Category;
    public Text txt_DesignPressure;

    public Toggle tog_select;
    public Image img_background;

    void Start()
    {

    }

    public void InitData(WaterSealModel model)
    {
        currentModel = model;
        tog_select.isOn = false;
        txt_Medium.text = model.Medium;
        txt_Number.text = model.Number;
        txt_InstallPosition.text = model.InstallPosition;
        txt_Category.text = model.Category;
        txt_DesignPressure.text = model.DesignPressure.ToString();
    }

    public void SetToggle(bool isOn)
    {
        tog_select.isOn = isOn;
    }

    public bool GetToggleStatus()
    {
        return tog_select.isOn;
    }

    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
