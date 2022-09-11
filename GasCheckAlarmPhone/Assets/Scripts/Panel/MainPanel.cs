using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : UIEventHelper
{
    public Text txt_allCount;
    public Text txt_normalCount;
    public Text txt_warningCount;
    public Text txt_errorCount;
    public Text txt_noConnectCount;

    public Text txt_time;
    public Text txt_userName;

    public List<Toggle> togList;
    public List<GameObject> panelList;

    public Button btn_exitGame;
    public Toggle tog_openShoutWarning;
    public Toggle tog_closeShoutWarning;
    public Toggle tog_openShakeWarning;
    public Toggle tog_closeShakeWarning;

    void Start()
    {
        for (int i = 0; i < togList.Count; i++)
        {
            RegisterTogClick<int>(togList[i], i, OnTogClick);
        }
        togList[0].isOn = true;
        togList[1].gameObject.SetActive(FormatData.currentUser.Authority == 1);

        RegisterBtnClick(btn_exitGame, OnExitGame);
        RegisterTogClick(tog_openShoutWarning, OnTogOpenShoutWarning);
        RegisterTogClick(tog_closeShoutWarning, OnTogCloseShoutWarning);
        RegisterTogClick(tog_openShakeWarning, OnTogOpenShakeWarning);
        RegisterTogClick(tog_closeShakeWarning, OnTogCloseShaketWarning);

        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeDataListEvent);
        txt_userName.text = FormatData.currentUser.UserName + FormatData.authorityNameDic[FormatData.currentUser.Authority];
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeDataListEvent);
    }

    void OnTogClick(Toggle tog, bool isOn, int index)
    {
        for (int i = 0; i < panelList.Count; i++)
        {
            if (i == index)
            {
                panelList[i].SetActive(true);
            }
            else
            {
                panelList[i].SetActive(false);
            }
        }
    }

    void OnTogOpenShakeWarning(Toggle tog, bool isOn)
    {
        CameraShake.instance.UseCameraShake();
    }

    void OnTogCloseShaketWarning(Toggle tog, bool isOn)
    {
        CameraShake.instance.NotUseCameraShake();
    }

    void OnTogOpenShoutWarning(Toggle tog, bool isOn)
    {
        AudioManager.instance.SetIsShoutWarning(true);
    }

    void OnTogCloseShoutWarning(Toggle tog, bool isOn)
    {
        AudioManager.instance.SetIsShoutWarning(false);
    }

    void OnExitGame(Button btn)
    {
        Application.Quit();
    }

    void UpdateRealtimeDataListEvent(object tdata)
    {
        RealtimeEventData realtimeEventData = (RealtimeEventData)tdata;
        txt_normalCount.text = realtimeEventData.normalList.Count.ToString();
        txt_warningCount.text = realtimeEventData.firstList.Count.ToString();
        txt_errorCount.text = realtimeEventData.secondList.Count.ToString();
        txt_noConnectCount.text = realtimeEventData.noResponseList.Count.ToString();
        txt_allCount.text = (realtimeEventData.normalList.Count + realtimeEventData.firstList.Count + realtimeEventData.secondList.Count + realtimeEventData.noResponseList.Count).ToString();
    }

    void Update()
    {
        UpdateTime(Time.deltaTime);
    }

    float tempUpdateTime = 0;
    void UpdateTime(float deltaTime)
    {
        tempUpdateTime += deltaTime;
        if (tempUpdateTime >= 1.0f)
        {
            tempUpdateTime = 0;
            txt_time.text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
