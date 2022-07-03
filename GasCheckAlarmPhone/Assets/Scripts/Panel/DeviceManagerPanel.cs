using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceManagerPanel : UIEventHelper
{
    public Button btn_probeManager;
    public Button btn_machineManager;
    public Button btn_factoryManager;

    public GameObject probeRoot;
    public GameObject machineRoot;
    public GameObject factoryRoot;

    void Start()
    {
        RegisterBtnClick(btn_probeManager, OnProbeManager);
        RegisterBtnClick(btn_machineManager, OnMachineManager);
        RegisterBtnClick(btn_factoryManager, OnFactoryManager);
        ChangeTopMenuStyle(btn_probeManager, btn_machineManager, btn_factoryManager);
        probeRoot.SetActive(true);
        machineRoot.SetActive(false);
        factoryRoot.SetActive(false);
    }

    void OnProbeManager(Button btn)
    {
        ChangeTopMenuStyle(btn_probeManager, btn_machineManager, btn_factoryManager);
        probeRoot.SetActive(true);
        machineRoot.SetActive(false);
        factoryRoot.SetActive(false);
    }

    void OnMachineManager(Button btn)
    {
        ChangeTopMenuStyle(btn_machineManager, btn_probeManager, btn_factoryManager);
        probeRoot.SetActive(false);
        machineRoot.SetActive(true);
        factoryRoot.SetActive(false);
    }

    void OnFactoryManager(Button btn)
    {
        ChangeTopMenuStyle(btn_factoryManager, btn_machineManager, btn_probeManager);
        probeRoot.SetActive(false);
        machineRoot.SetActive(false);
        factoryRoot.SetActive(true);
    }

    void ChangeTopMenuStyle(Button selectBtn, Button normalBtn1, Button normalBtn2)
    {
        selectBtn.GetComponent<Image>().color = new Color(1, 1, 1);
        selectBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(41 / 255.0f, 141 / 255.0f, 125 / 255.0f);

        normalBtn1.GetComponent<Image>().color = new Color(5 / 255.0f, 147 / 255.0f, 122 / 255.0f);
        normalBtn1.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);

        normalBtn2.GetComponent<Image>().color = new Color(5 / 255.0f, 147 / 255.0f, 122 / 255.0f);
        normalBtn2.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
    }
}
