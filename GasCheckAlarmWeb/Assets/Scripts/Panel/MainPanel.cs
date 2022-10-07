using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        togList[0].gameObject.SetActive(FormatData.currentUser.Authority == EAuthority.Admin);
        int selectIndex = FormatData.currentUser.Authority == EAuthority.Admin ? 0 : 1;
        togList[selectIndex].isOn = true;

        txt_userName.text = FormatData.currentUser.UserName + FormatData.authorityFormat[FormatData.currentUser.Authority];

        RegisterBtnClick(btn_exitGame, OnExitGame);
        RegisterTogClick(tog_openShoutWarning, OnTogOpenShoutWarning);
        RegisterTogClick(tog_closeShoutWarning, OnTogCloseShoutWarning);
        RegisterTogClick(tog_openShakeWarning, OnTogOpenShakeWarning);
        RegisterTogClick(tog_closeShakeWarning, OnTogCloseShaketWarning);
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeDataListEvent);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeDataListEvent);
    }

    void OnTogClick(Toggle tog, bool isOn, int index)
    {
        if (isOn)
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
        List<ProbeModel> list = (List<ProbeModel>)tdata;
        txt_normalCount.text = list.FindAll((it) => { return it.warningLevel == EWarningLevel.Normal; }).Count.ToString();
        txt_warningCount.text = list.FindAll((it) => { return it.warningLevel == EWarningLevel.FirstAlarm; }).Count.ToString();
        txt_errorCount.text = list.FindAll((it) => { return it.warningLevel == EWarningLevel.SecondAlarm; }).Count.ToString();
        txt_noConnectCount.text = list.FindAll((it) => { return it.warningLevel == EWarningLevel.NoResponse; }).Count.ToString();
        txt_allCount.text = list.Count.ToString();
    }

    void FixedUpdate()
    {
        UpdateTime(Time.deltaTime);
    }

    float updateTime = 1;
    float tempUpdateTime = 0;
    void UpdateTime(float deltaTime)
    {
        tempUpdateTime += deltaTime;
        if (tempUpdateTime >= updateTime)
        {
            tempUpdateTime = 0;
            txt_time.text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
