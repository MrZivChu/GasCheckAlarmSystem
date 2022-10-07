using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NotifyType
{
    UpdateProbeList,
    UpdateWaterSealList,
    UpdateMachineList,
    UpdateFactoryList,
    UpdateUserList,
    UpdateRealtimeDataList,
    UpdateSerialPortStatus,
    InsertDeviceTag,
    DeleteDeviceTag,
    UpdatePos2D,
}

public class EventManager
{
    private static EventManager instance;
    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EventManager();
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    public delegate void SendMassage(object data = null);
    public Dictionary<NotifyType, SendMassage> dicListener = new Dictionary<NotifyType, SendMassage>();

    public void AddEventListener(NotifyType eventType, SendMassage funcCallBack)
    {
        if (!dicListener.ContainsKey(eventType))
        {
            dicListener.Add(eventType, funcCallBack);
        }
        else
        {
            dicListener[eventType] += funcCallBack;
        }
    }

    public void DeleteEventListener(NotifyType eventType, SendMassage funcCallBack)
    {
        if (dicListener.ContainsKey(eventType))
        {
            dicListener[eventType] -= funcCallBack;
        }
    }

    public void DisPatch(NotifyType eventType, object data = null)
    {
        foreach (NotifyType bevent in dicListener.Keys)
        {
            if (bevent == eventType)
            {
                SendMassage sm = dicListener[bevent];
                if (sm != null)
                    sm(data);
            }
        }
    }
}
