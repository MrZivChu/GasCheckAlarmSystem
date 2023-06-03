using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraItem : MonoBehaviour
{
    public YUVRender yuvRender;
    public void Connect(CameraModel model)
    {
        yuvRender.DisConnect();
        yuvRender.Connect(model.IP, model.Port, model.UserName, model.UserPwd);
    }
}
