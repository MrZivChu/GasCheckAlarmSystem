using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System;
using System.IO;

public class ProbeConnectSettingPanel : UIEventHelper
{
    public Button btn_connect;
    public Button btn_disconnect;
    public Dropdown dropdown_portName;

    public Text txt_status;
    bool isConnect = false;

    List<string> portList;
    void Start()
    {
        RegisterBtnClick(btn_connect, OnConnect);
        RegisterBtnClick(btn_disconnect, OnDisConnect);

        string[] portNameArray = System.IO.Ports.SerialPort.GetPortNames();
        portList = new List<string>() { };
        portList.AddRange(portNameArray);
        dropdown_portName.AddOptions(portList);

        UpdateShowTip();
    }

    void OnConnect(Button btn)
    {
        if (portList != null && portList.Count > 0)
        {
            int portIndex = dropdown_portName.value;
            string commName = portList[portIndex].Trim();
            JsonHandleHelper.UpdateConfig(commName);
            OpenWinformExeProcess();
            UpdateShowTip();
        }
    }

    void OnDisConnect(Button btn)
    {
        CloseWinformExeProcess();
        UpdateShowTip();
    }

    void OpenWinformExeProcess()
    {
        CloseWinformExeProcess();
        string path = Application.streamingAssetsPath + "/SerialPortDataCollectionSystem.exe";
        System.Diagnostics.Process.Start(path);
        isConnect = true;
    }

    void CloseWinformExeProcess()
    {
        CSharpUtils.KillProcess("SerialPortDataCollectionSystem");
        isConnect = false;
    }

    void UpdateShowTip()
    {
        if (isConnect)
        {
            btn_connect.gameObject.SetActive(false);
            btn_disconnect.gameObject.SetActive(true);
            txt_status.text = "串口已连接";
        }
        else
        {
            btn_connect.gameObject.SetActive(true);
            btn_disconnect.gameObject.SetActive(false);
            txt_status.text = "串口已断开";
        }
    }
}
