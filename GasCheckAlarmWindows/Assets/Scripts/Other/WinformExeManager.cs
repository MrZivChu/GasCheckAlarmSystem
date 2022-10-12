﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinformExeManager : MonoBehaviour
{
    void Start()
    {
        ReOpenWinformExeProcess();
        EventManager.Instance.AddEventListener(NotifyType.UpdateProbeList, ReFresh);
        EventManager.Instance.AddEventListener(NotifyType.UpdateMachineList, ReFresh);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateProbeList, ReFresh);
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateMachineList, ReFresh);
        CloseWinformExeProcess();
    }

    void ReFresh(object data)
    {
        ReOpenWinformExeProcess();
    }

    void ReOpenWinformExeProcess()
    {
        CloseWinformExeProcess();
        System.Diagnostics.Process.Start(Application.streamingAssetsPath + "/SerialPortDataCollectionSystem.exe");
    }

    void CloseWinformExeProcess()
    {
        CSharpUtils.KillProcess("SerialPortDataCollectionSystem");
    }
}