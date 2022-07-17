
using UnityEngine;

using ZXing; // ZXing的.dll库文件的命名空间
using UnityEngine.UI;// UGUI命名空间（用来显示摄像机内容的图片）


// 二维码扫描识别功能
public class QRCodeScanning : MonoBehaviour
{
    [Header("摄像机检测界面")]
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
        webCamTextrue = new WebCamTexture(deviceName, 400, 300);
        // 显示的图片信息
        cameraTexture.texture = webCamTextrue;
        // 打开摄像机运行识别
        webCamTextrue.Play();

        // 实例化识别二维码信息存储对象
        barcodeReader = new BarcodeReader();
    }


    Color32[] data;// 二维码图片信息以像素点颜色信息数组存放

    /// <summary>
    /// 识别摄像机图片中的二维码信息
    /// 打印二维码识别到的信息
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
            Debug.Log(result.Text);// ===》===》===》 这是从二维码识别出来的信息
        }
    }

    // Start 初始化函数
    void Start()
    {
        DeviceInit();// 初始化下
    }

    float interval = 3;// 扫描识别间隔时间
    /// <summary>/// 更新测试/// </summary>
	void Update()
    {
        // 每隔一段时间进行一次进行识别二维码信息
        interval += Time.deltaTime;
        if (interval >= 3f)
        {
            ScanQRCode();// 扫描
            interval = 0;
        }
    }
}
