using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PointCheckItem : UIEventHelper
{
    public PointCheckModel currentModel;
    public Text txt_index;
    public Text txt_probeName;
    public Text txt_userName;
    public Text txt_checkTime;
    public Text txt_desc;
    public Image image;

    void Start()
    {
    }

    public void InitData(int index, PointCheckModel model)
    {
        currentModel = model;
        txt_index.text = index.ToString();
        txt_probeName.text = model.DeviceName;
        txt_userName.text = model.UserName;
        txt_checkTime.text = model.CheckTime.ToString("MM-dd HH:mm:ss");
        txt_desc.text = model.Description;

        GameUtils.GetHttpWebRequest("/QrCodeImgs/" + model.QrCodePath, (data) =>
        {
            Texture2D texture = new Texture2D(160, 120, TextureFormat.RGBA32, false);
            texture.LoadImage(data);
            texture.Apply();
            Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
            image.sprite = sp;
        }, null);
    }

    public Image img_background;
    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
