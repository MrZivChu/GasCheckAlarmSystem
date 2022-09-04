using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterSealPointCheckItem : UIEventHelper
{
    public WaterSealModel currentModel;
    public Text txt_Medium;
    public Text txt_Number;
    public Text txt_InstallPosition;
    public Image img_background;

    void Start()
    {
    }

    public void InitData(WaterSealModel model)
    {
        currentModel = model;
        txt_Medium.text = model.Medium;
        txt_InstallPosition.text = model.InstallPosition;
        txt_Number.text = model.Number;
    }

    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
