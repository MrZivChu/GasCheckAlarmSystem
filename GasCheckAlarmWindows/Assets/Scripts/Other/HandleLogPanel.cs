using System;
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
    public Toggle isOpenCamera;
    public Toggle isGlobalImage;

    public InputField productNameInput;
    public InputField sqlIpInput;
    public InputField sqlDatabaseInput;
    public InputField sqlUserIDInput;
    public InputField sqlPwdInput;
    public InputField smsPhoneInput;

    public InputField alertWarnValueInput;
    public InputField alertWarnSecondsInput;
    void Start()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_ok, OnOk);
        RegisterBtnClick(gasBtn, OnGasTypes);
    }

    private void OnEnable()
    {
        isLogTog.isOn = JsonHandleHelper.gameConfig.isLog;
        isOpenCamera.isOn = JsonHandleHelper.gameConfig.isOpenCamera;
        isGlobalImage.isOn = JsonHandleHelper.gameConfig.isOpenGlobalImage;
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
        JsonHandleHelper.UpdateConfig(isLogTog.isOn, isEnterProbePosTog.isOn, isOpenWaterSealTog.isOn, productNameInput.text, sqlIpInput.text, sqlDatabaseInput.text, sqlUserIDInput.text, sqlPwdInput.text, smsPhoneInput.text, Convert.ToInt32(alertWarnValueInput.text), Convert.ToInt32(alertWarnSecondsInput.text), isOpenCamera.isOn, isGlobalImage.isOn);
        CSharpUtils.StartProcess(Application.streamingAssetsPath + "/restart.bat", string.Empty);
        Application.Quit();
    }
}
