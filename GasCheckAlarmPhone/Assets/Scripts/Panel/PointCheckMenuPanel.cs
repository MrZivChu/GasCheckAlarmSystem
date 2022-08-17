using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCheckMenuPanel : UIEventHelper
{
    public Button btn_pointCheck;
    public Button btn_pointCheckManager;

    public GameObject pointCheckRoot;
    public GameObject pointCheckManagerRoot;

    void Start()
    {
        RegisterBtnClick(btn_pointCheck, OnPointCheck);
        RegisterBtnClick(btn_pointCheckManager, OnPointCheckManager);

        btn_pointCheck.gameObject.SetActive(FormatData.currentUser != null && FormatData.currentUser.Authority == 0);
        btn_pointCheckManager.gameObject.SetActive(FormatData.currentUser != null && FormatData.currentUser.Authority == 1);
        if (btn_pointCheck.gameObject.activeSelf)
        {
            OnPointCheck(btn_pointCheck);
        }
        else
        {
            OnPointCheckManager(btn_pointCheckManager);
        }
    }

    void OnPointCheck(Button btn)
    {
        ChangeTopMenuStyle(btn_pointCheck, btn_pointCheckManager);
        pointCheckRoot.SetActive(true);
        pointCheckManagerRoot.SetActive(false);
    }

    void OnPointCheckManager(Button btn)
    {
        ChangeTopMenuStyle(btn_pointCheckManager, btn_pointCheck);
        pointCheckRoot.SetActive(false);
        pointCheckManagerRoot.SetActive(true);
    }

    void ChangeTopMenuStyle(Button selectBtn, Button normalBtn1)
    {
        selectBtn.GetComponent<Image>().color = new Color(1, 1, 1);
        selectBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(41 / 255.0f, 141 / 255.0f, 125 / 255.0f);

        normalBtn1.GetComponent<Image>().color = new Color(5 / 255.0f, 147 / 255.0f, 122 / 255.0f);
        normalBtn1.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
    }
}
