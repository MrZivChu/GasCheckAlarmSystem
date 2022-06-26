using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealtimeDataMenuPanel : UIEventHelper
{
    public Button btn_realtimeDataManager;
    public Button btn_chartManager;
    public Button btn_porbeSetting;
    public Button btn_porbeHistoryData;

    public GameObject RealtimeDataRoot;
    public GameObject chartRoot;
    public GameObject porbeSettingRoot;
    public GameObject probeHistoryDataRoot;

    void Start()
    {
        RegisterBtnClick(btn_realtimeDataManager, OnRealtimeDataManager);
        RegisterBtnClick(btn_chartManager, OnChartManager);
        RegisterBtnClick(btn_porbeSetting, OnProbeSetting);
        RegisterBtnClick(btn_porbeHistoryData, OnProbeHistoryData);

        ChangeTopMenuStyle(btn_porbeSetting, btn_realtimeDataManager, btn_chartManager, btn_porbeHistoryData);
        RealtimeDataRoot.SetActive(false);
        chartRoot.SetActive(false);
        porbeSettingRoot.SetActive(true);
        probeHistoryDataRoot.SetActive(false);
    }

    void OnProbeHistoryData(Button btn)
    {
        ChangeTopMenuStyle(btn_porbeHistoryData, btn_porbeSetting, btn_realtimeDataManager, btn_chartManager);
        RealtimeDataRoot.SetActive(false);
        chartRoot.SetActive(false);
        porbeSettingRoot.SetActive(false);
        probeHistoryDataRoot.SetActive(true);
    }

    void OnProbeSetting(Button btn)
    {
        ChangeTopMenuStyle(btn_porbeSetting, btn_realtimeDataManager, btn_chartManager, btn_porbeHistoryData);
        RealtimeDataRoot.SetActive(false);
        chartRoot.SetActive(false);
        porbeSettingRoot.SetActive(true);
        probeHistoryDataRoot.SetActive(false);
    }

    void OnRealtimeDataManager(Button btn)
    {
        ChangeTopMenuStyle(btn_realtimeDataManager, btn_chartManager, btn_porbeSetting, btn_porbeHistoryData);
        RealtimeDataRoot.SetActive(true);
        chartRoot.SetActive(false);
        porbeSettingRoot.SetActive(false);
        probeHistoryDataRoot.SetActive(false);
    }

    void OnChartManager(Button btn)
    {
        ChangeTopMenuStyle(btn_chartManager, btn_realtimeDataManager, btn_porbeSetting, btn_porbeHistoryData);
        RealtimeDataRoot.SetActive(false);
        chartRoot.SetActive(true);
        porbeSettingRoot.SetActive(false);
        probeHistoryDataRoot.SetActive(false);
    }

    void ChangeTopMenuStyle(Button selectBtn, Button normalBtn1, Button normalBtn2, Button normalBtn3)
    {
        selectBtn.GetComponent<Image>().color = new Color(1, 1, 1);
        selectBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(41 / 255.0f, 141 / 255.0f, 125 / 255.0f);

        normalBtn1.GetComponent<Image>().color = new Color(5 / 255.0f, 147 / 255.0f, 122 / 255.0f);
        normalBtn1.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);

        normalBtn2.GetComponent<Image>().color = new Color(5 / 255.0f, 147 / 255.0f, 122 / 255.0f);
        normalBtn2.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);

        normalBtn3.GetComponent<Image>().color = new Color(5 / 255.0f, 147 / 255.0f, 122 / 255.0f);
        normalBtn3.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
    }
}
