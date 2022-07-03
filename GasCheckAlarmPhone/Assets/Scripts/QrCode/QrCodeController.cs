using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ZXing; // ZXing的.dll库文件的命名空间
using UnityEngine.UI;// UGUI命名空间

/// <summary>
/// 二维码测试
/// 二维码识别成字符串信息和字符串信息转换生成二维码图片
/// </summary>
public class QrCodeController : MonoBehaviour
{
    public RawImage cameraTexture;// 摄像机映射显示区域
    private WebCamTexture webCamTextrue;// 摄像机映射纹理

    // 二维码识别类
    BarcodeReader barcodeReader;// 库文件的对象（二维码信息保存的地方）

    /// <summary>
    /// 开启摄像机和准备工作
    /// </summary>
    void DeviceInit()
    {
        // 获取所有摄像机硬件
        WebCamDevice[] devices = WebCamTexture.devices;
        // 获取第一个摄像机硬件的名称
        string deviceName = devices[0].name;
        // 创建实例化一个摄像机显示区域
        Vector2 size = cameraTexture.GetComponent<RectTransform>().rect.size;
        webCamTextrue = new WebCamTexture(deviceName, (int)size.x, (int)size.y);
        // 显示的图片信息
        cameraTexture.texture = webCamTextrue;
        // 打开摄像机运行识别
        webCamTextrue.Play();

        // 实例化识别二维码信息存储对象
        barcodeReader = new BarcodeReader();

        // 开始执行更新识别
        interval = 0;
        isGoScanning = true;
    }

    Color32[] data;// 二维码图片信息以像素点颜色信息数组存放

    /// <summary>
    /// 识别摄像机图片中的二维码信息
    /// 打印二维码信息
    /// </summary>
    void ScanQRCode()
    {
        // 获取摄像机画面的像素颜色数组信息
        data = webCamTextrue.GetPixels32();
        // 获取图片中的二维码信息
        Result result = barcodeReader.Decode(data, webCamTextrue.width, webCamTextrue.height);
        // 如果获取到二维码信息了，打印出来
        if (result != null)
        {
            isGoScanning = false;
            showText.text = "扫描信息：" + result.Text;
        }
        else
        {
            showText.text = "正在扫描识别中...";
        }
    }

    public Button goScanning;
    public Text showText;

    void Start()
    {
        goScanning.onClick.AddListener(DeviceInit);
    }

    bool isGoScanning = false;
    float interval = 0;
    void Update()
    {
        if (isGoScanning)
        {
            interval += Time.deltaTime;
            if (interval >= 0.5f)
            {
                ScanQRCode();
                interval = 0;
            }
        }
    }

}
