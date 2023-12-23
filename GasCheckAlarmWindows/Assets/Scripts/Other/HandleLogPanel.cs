﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HandleLogPanel : UIEventHelper
{
    public Button btn_ok;
    public Button btn_cancel;
    public Button gasBtn;

    public GameObject gasTypesPanel;

    public Toggle isLogTog;
    public Toggle isOpenWaterSealTog;
    public Toggle isEnterProbePosTog;

    public InputField productNameInput;
    public InputField sqlIpInput;
    public InputField sqlDatabaseInput;
    public InputField sqlUserIDInput;
    public InputField sqlPwdInput;
    public InputField smsPhoneInput;

    public Text configPath;

    public InputField alertWarnValueInput;
    public InputField alertWarnSecondsInput;
    void Start()
    {
        configPath.text = "配置文件路径：" + Application.persistentDataPath;
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_ok, OnOk);
        RegisterBtnClick(gasBtn, OnGasTypes);
    }

    private void OnEnable()
    {
        isLogTog.isOn = JsonHandleHelper.gameConfig.isLog;
        isOpenWaterSealTog.isOn = JsonHandleHelper.gameConfig.isOpenWaterSeal;
        isEnterProbePosTog.isOn = JsonHandleHelper.gameConfig.isEnterPosDir;

        productNameInput.text = JsonHandleHelper.gameConfig.productName;
        sqlIpInput.text = JsonHandleHelper.gameConfig.sqlIP;
        sqlDatabaseInput.text = JsonHandleHelper.gameConfig.sqlDatabase;
        sqlUserIDInput.text = JsonHandleHelper.gameConfig.sqlUserId;
        sqlPwdInput.text = JsonHandleHelper.gameConfig.sqlUserPwd;
        smsPhoneInput.text = JsonHandleHelper.gameConfig.smsPhone;

        alertWarnValueInput.text = JsonHandleHelper.gameConfig.alertWarnValue.ToString();
        alertWarnSecondsInput.text = JsonHandleHelper.gameConfig.alertWarnSeconds.ToString();
    }

    void OnGasTypes(Button btn)
    {
        gasTypesPanel.SetActive(true);
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnOk(Button btn)
    {
        bool isLog = isLogTog.isOn;
        bool isOpenWaterSeal = isOpenWaterSealTog.isOn;
        bool isEnterPosDir = isEnterProbePosTog.isOn;
        string productName = productNameInput.text;
        string sqlIP = sqlIpInput.text;
        string sqlDatabase = sqlDatabaseInput.text;
        string sqlUserId = sqlUserIDInput.text;
        string sqlUserPwd = sqlPwdInput.text;
        string smsPhone = smsPhoneInput.text;

        JsonHandleHelper.UpdateConfig(isLog, isEnterPosDir, isOpenWaterSeal, productName, sqlIP, sqlDatabase, sqlUserId, sqlUserPwd, smsPhone, Convert.ToInt32(alertWarnValueInput.text), Convert.ToInt32(alertWarnSecondsInput.text));
        Application.Quit();
    }
}
