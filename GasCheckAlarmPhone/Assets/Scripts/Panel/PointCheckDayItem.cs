using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCheckDayItem : UIEventHelper
{
    public ProbeModel currentModel;
    public Text txt_probeName;
    public Text txt_factoryName;
    public Text txt_gasKind;
    public Image img_background;

    void Start()
    {
    }

    public void InitData(ProbeModel model)
    {
        currentModel = model;
        txt_probeName.text = model.ProbeName;
        txt_factoryName.text = model.FactoryName;
        txt_gasKind.text = model.GasKind;
    }

    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
