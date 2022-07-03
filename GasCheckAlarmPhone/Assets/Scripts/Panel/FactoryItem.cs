using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactoryItem : UIEventHelper
{
    public FactoryModel currentModel;
    public Text txt_factoryName;

    public Toggle tog_select;
    public Image img_background;

    void Start()
    {

    }

    public void InitData(FactoryModel model)
    {
        currentModel = model;
        tog_select.isOn = false;
        txt_factoryName.text = model.FactoryName;
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
