using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using UnityEngine.UI;
using System;
using LitJson;
using System.IO;

public class WaterSealDetailPanel : MonoBehaviour
{
    public List<Toggle> togList;
    public Image qrImg;
    public Text deviceNameText;
    public Button okBtn;
    void Start()
    {
        okBtn.onClick.AddListener(OnOk);
    }

    void OnOk()
    {
        gameObject.SetActive(false);
    }

    public void OnInit(PointCheckModel model)
    {
        deviceNameText.text = model.DeviceName;
        GameUtils.GetHttpWebRequest("/QrCodeImgs/" + model.QrCodePath, (data) =>
        {
            Texture2D texture = new Texture2D(600, 600, TextureFormat.RGBA32, false);
            texture.LoadImage(data);
            texture.Apply();
            Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
            qrImg.sprite = sp;
        }, null);
        string[] resultArray = model.Result.Split(',');
        for (int i = 0; i < togList.Count; i++)
        {
            togList[i].isOn = Convert.ToBoolean(resultArray[i]);
        }
    }
}
