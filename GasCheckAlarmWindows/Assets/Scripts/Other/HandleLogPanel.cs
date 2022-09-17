using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleLogPanel : UIEventHelper
{
    public Button btn_ok;
    public Button btn_cancel;

    public Toggle isLogTog;
    public Toggle isOpenWaterSealTog;
    public Toggle isEnterProbePosTog;

    public InputField productNameInput;
    public InputField sqlIpInput;
    public InputField sqlDatabaseInput;
    public InputField sqlUserIDInput;
    public InputField sqlPwdInput;
    public InputField smsPhoneInput;
    void Start()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_ok, OnOk);
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

        JsonHandleHelper.UpdateConfig(isLog, isEnterPosDir, isOpenWaterSeal, productName, sqlIP, sqlDatabase, sqlUserId, sqlUserPwd, smsPhone);
        Application.Quit();
    }
}
