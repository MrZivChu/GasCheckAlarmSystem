using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
        CSharpUtils.KillProcess("frpc");
        CSharpUtils.KillProcess("SerialPortDataCollectionSystem");
    }

    void OnApplicationQuit()
    {
        CSharpUtils.KillProcess("frpc");
        CSharpUtils.KillProcess("SerialPortDataCollectionSystem");
    }
}
