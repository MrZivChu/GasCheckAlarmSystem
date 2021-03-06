using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QrCodeSpawnItem : MonoBehaviour
{
    public Text descText;

    public ProbeModel model;
    public void InitData(ProbeModel model)
    {
        this.model = model;
        descText.text = model.ProbeName;
        DrawQRCode(model.ProbeName);
    }

    public RawImage qrCode;
    BarcodeWriter barcodeWriter;

    /// <summary>
    /// 绘制指定字符串信息的二维码显示到指定区域
    /// </summary>
    /// <param name="str">要生产二维码的字符串信息</param>
    /// <param name="width">二维码的宽度</param>
    /// <param name="height">二维码的高度</param>
    /// <returns>返回绘制好的图片信息</returns>
    public Texture2D ShowQRCode(string str, int width, int height)
    {
        // 实例化一个图片类
        Texture2D t = new Texture2D(width, height);
        // 获取二维码图片颜色数组信息
        Color32[] col32 = GeneQRCode(str, width, height);
        // 为图片设置绘制像素颜色信息
        t.SetPixels32(col32);
        // 设置信息更新应用下
        t.Apply();

        // 将整理好的图片信息显示到指定区域中
        return t;
    }

    /// <summary>
    /// 将指定字符串信息转换成二维码图片信息
    /// </summary>
    /// <param name="formatStr">要生产二维码的字符串信息</param>
    /// <param name="width">二维码的宽度</param>
    /// <param name="height">二维码的高度</param>
    /// <returns>返回二维码图片的颜色数组信息</returns>
    Color32[] GeneQRCode(string formatStr, int width, int height)
    {
        // 绘制二维码前进行一些设置
        ZXing.QrCode.QrCodeEncodingOptions options = new ZXing.QrCode.QrCodeEncodingOptions();
        // 设置字符串转换格式，确保字符串信息保持正确
        options.CharacterSet = "UTF-8";
        // 设置绘制区域的宽度和高度的像素值
        options.Width = width;
        options.Height = height;
        // 设置二维码边缘留白宽度（值越大，六百宽度大，二维码就减小）
        options.Margin = 1;

        // 实例化字符串绘制二维码工具
        barcodeWriter = new BarcodeWriter { Format = ZXing.BarcodeFormat.QR_CODE, Options = options };
        // 进行二维码绘制并进行返回图片的颜色数组信息
        return barcodeWriter.Write(formatStr);
    }

    Texture2D texture2D = null;
    /// <summary>
    /// 开始绘制指定信息的二维码
    /// </summary>
    public void DrawQRCode(string formatStr)
    {
        // 注意：这个宽高度大小256不要变，不然生成的信息不正确
        // 256有可能是这个ZXingNet插件指定大小的绘制像素点数值
        texture2D = ShowQRCode(formatStr, 256, 256);
        // 显示到UI界面的图片上
        qrCode.texture = texture2D;
    }

    public byte[] GetQrCodeBytes()
    {
        if (texture2D != null)
        {
            return texture2D.EncodeToJPG();
        }
        return null;
    }
}
