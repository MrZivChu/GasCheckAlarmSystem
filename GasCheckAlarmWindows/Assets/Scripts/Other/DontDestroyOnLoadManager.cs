using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
        CSharpUtils.KillProcess("SerialPortDataCollectionSystem");
    }

    void OnApplicationQuit()
    {
        CSharpUtils.KillProcess("SerialPortDataCollectionSystem");
    }
}
