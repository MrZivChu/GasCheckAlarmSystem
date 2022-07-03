using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum PageType
{
    factoryCheck,
    machineManager,
    userManager,
    realTimeDataManager,
    planarGraphPanel,
    cubeInfo
}

public class MainPanel : UIEventHelper
{
    public static MainPanel instance;

    public Text txt_allCount;
    public Text txt_normalCount;
    public Text txt_warningCount;
    public Text txt_errorCount;
    public Text txt_noConnectCount;

    public Text txt_time;
    public Text txt_userName;

    public Toggle tog_factoryCheck;
    public Toggle tog_machineManager;
    public Toggle tog_userManager;
    public Toggle tog_realTimeDataManager;
    public Toggle tog_planarGraph;
    public Toggle tog_cubeInfo;

    public GameObject DeviceManagerPanel;
    public GameObject UserManagerPanel;
    public GameObject RealtimeDataManagerPanel;
    public GameObject PlanarGraphPanel;
    public GameObject CubeInfoPanel;

    public Button btn_exitGame;

    public bool isShoutWarning = true;
    public Toggle tog_openShoutWarning;
    public Toggle tog_closeShoutWarning;

    public Toggle tog_openShakeWarning;
    public Toggle tog_closeShakeWarning;

    PageType pageType_ = PageType.factoryCheck;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        RegisterTogClick(tog_factoryCheck, OnFactoryCheck);
        RegisterTogClick(tog_machineManager, OnMachineManager);
        RegisterTogClick(tog_userManager, OnUserManager);
        RegisterTogClick(tog_realTimeDataManager, OnRealTimeManager);
        RegisterTogClick(tog_planarGraph, OnPlanarGraphManager);
        RegisterTogClick(tog_cubeInfo, OnCubeInfoManager);
        tog_realTimeDataManager.isOn = true;

        RegisterBtnClick(btn_exitGame, OnExitGame);

        RegisterTogClick(tog_openShoutWarning, OnTogOpenShoutWarning);
        RegisterTogClick(tog_closeShoutWarning, OnTogCloseShoutWarning);

        RegisterTogClick(tog_openShakeWarning, OnTogOpenShakeWarning);
        RegisterTogClick(tog_closeShakeWarning, OnTogCloseShaketWarning);

        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeDataListEvent);
        if (FormatData.currentUser != null)
        {
            txt_userName.text = FormatData.currentUser.UserName + (FormatData.currentUser.Authority == 1 ? "  管理员" : "  普通用户");
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
        isShoutWarning = true;
    }

    void OnTogCloseShoutWarning(Toggle tog, bool isOn)
    {
        isShoutWarning = false;
    }

    void OnExitGame(Button btn)
    {
        Application.Quit();
    }

    void OnFactoryCheck(Toggle btn, bool isCheck)
    {
        if (isCheck)
        {
            ChangePage(PageType.factoryCheck);
        }
    }

    void OnMachineManager(Toggle btn, bool isCheck)
    {
        if (isCheck)
        {
            ChangePage(PageType.machineManager);
        }
    }

    void OnUserManager(Toggle btn, bool isCheck)
    {
        if (isCheck)
        {
            if (FormatData.currentUser.Authority == 1)
            {
                ChangePage(PageType.userManager);
            }
            else
            {
                MessageBox.Instance.PopOK("管理员权限才可查看", null, "确认");
            }
        }
    }

    void OnRealTimeManager(Toggle btn, bool isCheck)
    {
        if (isCheck)
        {
            ChangePage(PageType.realTimeDataManager);
        }
    }

    void OnPlanarGraphManager(Toggle btn, bool isCheck)
    {
        if (isCheck)
        {
            ChangePage(PageType.planarGraphPanel);
        }
    }

    void OnCubeInfoManager(Toggle btn, bool isCheck)
    {
        if (isCheck)
        {
            ChangePage(PageType.cubeInfo);
        }
    }

    void ChangePage(PageType pageType)
    {
        pageType_ = pageType;
        DeviceManagerPanel.SetActive(pageType == PageType.machineManager);
        UserManagerPanel.SetActive(pageType == PageType.userManager);
        RealtimeDataManagerPanel.SetActive(pageType == PageType.realTimeDataManager);
        PlanarGraphPanel.SetActive(pageType == PageType.planarGraphPanel);
        CubeInfoPanel.SetActive(pageType == PageType.cubeInfo);
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
            txt_time.text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
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
}
