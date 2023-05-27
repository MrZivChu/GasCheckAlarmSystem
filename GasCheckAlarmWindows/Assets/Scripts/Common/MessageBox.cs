using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : UIEventHelper
{
    private static GameObject messageBoxObj;
    private static GameObject canvas;
    public static MessageBox Instance
    {
        get
        {
            if (canvas == null)
            {
                canvas = GameObject.Find("UICanvas");
            }
            if (messageBoxObj == null)
            {
                UnityEngine.Object obj = Resources.Load("MessageBox");
                messageBoxObj = Instantiate(obj) as GameObject;
                messageBoxObj.transform.parent = canvas.transform;
                messageBoxObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                messageBoxObj.transform.localScale = Vector3.one;
            }
            return messageBoxObj.GetComponent<MessageBox>();
        }
    }

    public Button leftBtn;
    public Button rightBtn;
    public Text tipText;

    public Text leftBtnText;
    public Text rightBtnText;

    public void PopOK(string tip, Action callback = null, string btnText = "确定")
    {
        leftBtn.gameObject.SetActive(false);
        rightBtn.gameObject.SetActive(true);
        rightBtnText.text = btnText;
        tipText.text = tip;
        gameObject.SetActive(true);

        RegisterBtnClick(rightBtn, (go) =>
       {
           if (callback != null)
           {
               callback();
           }
           gameObject.SetActive(false);
       });
    }

    public void PopYesNo(string tip, Action leftCallback, Action rightCallback, string tleftBtnText, string trightBtnText)
    {
        leftBtnText.text = tleftBtnText;
        rightBtnText.text = trightBtnText;
        tipText.text = tip;
        leftBtn.gameObject.SetActive(true);
        rightBtn.gameObject.SetActive(true);
        gameObject.SetActive(true);

        RegisterBtnClick(leftBtn, (go) =>
       {
           if (leftCallback != null)
           {
               leftCallback();
           }
           gameObject.SetActive(false);
       });

        RegisterBtnClick(rightBtn, (go) =>
       {
           if (rightCallback != null)
           {
               rightCallback();
           }
           gameObject.SetActive(false);
       });
    }
}
