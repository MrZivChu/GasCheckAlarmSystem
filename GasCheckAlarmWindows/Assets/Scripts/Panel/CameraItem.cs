using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraItem : MonoBehaviour
{
    public GameObject root;
    public YUVRender yuvRender;
    public Button connectBtn;
    public Button deletBtn;
    public InputField ipInput;
    public InputField portInput;
    public InputField userNameInput;
    public InputField userPwdInput;

    public CameraModel model_ = null;
    void Awake()
    {
        EventTriggerListener.Get(gameObject).onEnter += OnEnter;
        EventTriggerListener.Get(gameObject).onExit += onExit;

        EventTriggerListener.Get(connectBtn.gameObject).onClick += OnClickEditorConnect;
        EventTriggerListener.Get(deletBtn.gameObject).onClick += OnClickDelete;
    }

    void OnClickEditorConnect(GameObject go, object data = null)
    {
        model_.IP = ipInput.text;
        model_.Port = portInput.text;
        model_.UserName = userNameInput.text;
        model_.UserPwd = userPwdInput.text;
        CameraDAL.EditCameraByID(model_.ID, model_.IP, model_.Port, model_.UserName, model_.UserPwd);
        EventManager.Instance.DisPatch(NotifyType.EditorCamera, gameObject);
    }

    void OnClickDelete(GameObject go, object data = null)
    {
        CameraDAL.DeleteCameraByID(model_.ID.ToString());
        yuvRender.DisConnect();
        Destroy(gameObject);
    }

    void OnEnter(GameObject go, object data = null)
    {
        root.SetActive(true);
    }

    void onExit(GameObject go, object data = null)
    {
        root.SetActive(false);
    }

    public void Connect(CameraModel model, string publicIP, bool isUsePublicNetwork)
    {
        model_ = model;
        ipInput.text = model.IP;
        portInput.text = model.Port;
        userNameInput.text = model.UserName;
        userPwdInput.text = model.UserPwd;
        yuvRender.DisConnect();
        yuvRender.Connect(isUsePublicNetwork ? publicIP : model_.IP, isUsePublicNetwork ? model_.Port : "8000", model_.UserName, model_.UserPwd);
    }
}
