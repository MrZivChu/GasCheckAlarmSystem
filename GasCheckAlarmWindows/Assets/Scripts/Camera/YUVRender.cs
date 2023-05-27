using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Threading;
using System.Runtime.InteropServices;
using PreviewDemo;

public class YUVRender : MonoBehaviour
{

    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_GetPort(ref int nPort);
    [DllImport("PlayCtrl")]
    public static extern uint PlayM4_GetLastError(int nPort);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_SetStreamOpenMode(int nPort, uint nMode);
    public const int STREAME_REALTIME = 0;
    public const int STREAME_FILE = 1;
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_OpenStream(int nPort, IntPtr pFileHeadBuf, uint nSize, uint nBufPoolSize);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_SetDisplayBuf(int nPort, uint nNum);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_SetOverlayMode(int nPort, int bOverlay, uint colorKey);
    public delegate void DECCBFUN(int nPort, IntPtr pBuf, int nSize, ref Frame_Info pFrameInfo, int nReserved1, int nReserved2);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_SetDecCallBackEx(int nPort, DECCBFUN DecCBFun, IntPtr pDest, int nDestSize);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_Play(int nPort, IntPtr hWnd);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_InputData(int nPort, IntPtr pBuf, uint nSize);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_SetDecCallBack(int nPort, DECCBFUN DecCBFun);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_Stop(int nPort);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_CloseStream(int nPort);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_FreePort(int nPort);

    int userID_ = -1;
    int realHandle_ = -1;
    Int32 port_ = -1;
    IntPtr ptrRealHandle_;
    int channelID_ = 1;

    private UInt32 dwBufSizes;
    private IntPtr pBuffers;
    int width_ = 0;//视屏的宽
    int height_ = 0;//视屏的高
    private Texture2D textureY = null;
    private Texture2D textureU = null;
    private Texture2D textureV = null;
    private byte[] VideoData;
    private bool isDataGet = false;
    private DateTime StartTime;
    private DateTime EndTime;
    public RawImage rawImage;
    public Toggle tog;

    DECCBFUN m_fDisplayFun = null;
    CHCNetSDK.REALDATACALLBACK RealData = null;
    CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo;

    private void Update()
    {
        if (!tog.isOn)
        {
            return;
        }
        //左
        if (Input.GetKey(KeyCode.A))
        {
            if (realHandle_ >= 0)
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(realHandle_, (uint)CHCNetSDK.PAN_LEFT, 0, 4);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(userID_, channelID_, CHCNetSDK.PAN_LEFT, 0, 4);
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (realHandle_ >= 0)
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(realHandle_, (uint)CHCNetSDK.PAN_LEFT, 1, 4);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(userID_, channelID_, CHCNetSDK.PAN_LEFT, 1, 4);
            }
        }
        //上
        if (Input.GetKey(KeyCode.W))
        {
            if (realHandle_ >= 0)
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(realHandle_, CHCNetSDK.TILT_UP, 0, 4);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(userID_, channelID_, CHCNetSDK.TILT_UP, 0, 4);
            }
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            if (realHandle_ >= 0)
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(realHandle_, CHCNetSDK.TILT_UP, 1, 4);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(userID_, channelID_, CHCNetSDK.TILT_UP, 1, 4);
            }
        }
        // 右
        if (Input.GetKey(KeyCode.D))
        {
            if (realHandle_ >= 0)
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(realHandle_, CHCNetSDK.PAN_RIGHT, 0, 4);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(userID_, channelID_, CHCNetSDK.PAN_RIGHT, 0, 4);
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (realHandle_ >= 0)
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(realHandle_, CHCNetSDK.PAN_RIGHT, 1, 4);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(userID_, channelID_, CHCNetSDK.PAN_RIGHT, 1, 4);
            }
        }
        //下
        if (Input.GetKey(KeyCode.S))
        {
            if (realHandle_ >= 0)
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(realHandle_, CHCNetSDK.TILT_DOWN, 0, 4);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(userID_, channelID_, CHCNetSDK.TILT_DOWN, 0, 4);
            }
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (realHandle_ >= 0)
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(realHandle_, CHCNetSDK.TILT_DOWN, 1, 4);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(userID_, channelID_, CHCNetSDK.TILT_DOWN, 1, 4);
            }
        }
    }

    public void Connect(string ip, string port, string userName, string passWord)
    {
        if (rawImage.material == rawImage.defaultMaterial)
        {
            Material mm = Resources.Load("YUVMaitial") as Material;
            rawImage.material = Instantiate(mm);
        }

        if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
        {
            return;
        }
        if (userID_ < 0)
        {
            userID_ = CHCNetSDK.NET_DVR_Login_V30(ip, int.Parse(port), userName, passWord, ref DeviceInfo);
            if (userID_ < 0)
            {
                Debug.Log("登录失败，错误号：" + CHCNetSDK.NET_DVR_GetLastError());
            }
            else
            {
                Debug.Log("登录成功");
                VideoBtnClick();
            }
        }
    }

    void VideoBtnClick()
    {
        if (userID_ < 0)
        {
            Debug.Log("请先登录");
        }
        else
        {
            if (realHandle_ < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = IntPtr.Zero;   //直接打开视屏
                lpPreviewInfo.lChannel = channelID_;//预览的设备通道 the device channel number
                lpPreviewInfo.dwStreamType = 1;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推 可以切换码流类型查看不同的效果，选择最佳的类型
                lpPreviewInfo.dwLinkMode = 4;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = false; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 200; //播放库显示缓冲区最大帧数
                IntPtr pUser = IntPtr.Zero;//用户数据 user data
                RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数 real-time stream callback function 
                realHandle_ = CHCNetSDK.NET_DVR_RealPlay_V40(userID_, ref lpPreviewInfo, RealData, pUser);
                if (realHandle_ < 0)
                {
                    Debug.Log("预览失败, 错误号：" + CHCNetSDK.NET_DVR_GetLastError());
                }
                else
                {
                    Debug.Log("预览成功");
                }
            }
            else
            {

            }
        }
    }

    void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
    {
        dwBufSizes = dwBufSize;
        pBuffers = pBuffer;
        switch (dwDataType)
        {
            case CHCNetSDK.NET_DVR_SYSHEAD:
                if (dwBufSize > 0)
                {
                    if (port_ >= 0)
                    {
                        Debug.LogError("同一路码流不需要多次调用开流接口");
                        return;
                    }
                    //获取播放句柄 Get the port to play
                    if (!PlayM4_GetPort(ref port_))
                    {
                        Debug.LogError("PlayM4_GetPort failed, error code= " + PlayM4_GetLastError(port_));
                        break;
                    }
                    //设置流播放模式 Set the stream mode: real-time stream mode
                    if (!PlayM4_SetStreamOpenMode(port_, STREAME_REALTIME))
                    {
                        Debug.LogError("Set STREAME_REALTIME mode failed, error code= " + PlayM4_GetLastError(port_));
                    }
                    //打开码流，送入头数据 Open stream
                    if (!PlayM4_OpenStream(port_, pBuffer, dwBufSize, 2 * 1024 * 1024))
                    {
                        Debug.LogError("PlayM4_OpenStream failed, error code= " + PlayM4_GetLastError(port_));
                        break;
                    }
                    //设置显示缓冲区个数 Set the display buffer number
                    if (!PlayM4_SetDisplayBuf(port_, 15))
                    {
                        Debug.LogError("PlayM4_SetDisplayBuf failed, error code= " + PlayM4_GetLastError(port_));
                    }
                    //设置显示模式 Set the display mode
                    if (!PlayM4_SetOverlayMode(port_, 0, 0/* COLORREF(0)*/)) //play off screen 
                    {
                        Debug.LogError("PlayM4_SetOverlayMode failed, error code= " + PlayM4_GetLastError(port_));
                    }
                    //设置解码回调函数，获取解码后音视频原始数据 Set callback function of decoded data
                    m_fDisplayFun = new DECCBFUN(DecCallbackFUN);
                    if (!PlayM4_SetDecCallBack(port_, m_fDisplayFun))
                    {
                        Debug.LogError("PlayM4 CallBack Failed!");
                    }
                    //开始解码 Start to play                       
                    if (!PlayM4_Play(port_, ptrRealHandle_))
                    {
                        Debug.LogError("PlayM4_Play failed, error code= " + PlayM4_GetLastError(port_));
                        break;
                    }
                }
                break;
            case CHCNetSDK.NET_DVR_STREAMDATA:
                if (dwBufSizes > 0 && port_ != -1)
                {
                    for (int i = 0; i < 999; i++)
                    {
                        //送入码流数据进行解码 Input the stream data to decode
                        if (!PlayM4_InputData(port_, pBuffers, dwBufSizes))
                        {
                            Debug.LogError("PlayM4_InputData failed, error code= " + PlayM4_GetLastError(port_));
                            Thread.Sleep(2);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                break;
            default:
                Debug.Log("GetOtherData!");
                break;
        }
    }

    void DecCallbackFUN(int nPort, IntPtr pBuf, int nSize, ref Frame_Info pFrameInfo, int nReserved1, int nReserved2)
    {
        if (pFrameInfo.nType == 3) //#define T_YV12 3
        {
            width_ = pFrameInfo.nWidth;
            height_ = pFrameInfo.nHeight;
            VideoData = new byte[nSize];
            Marshal.Copy(pBuf, VideoData, 0, nSize);
            isDataGet = true;
        }
    }

    void FixedUpdate()
    {
        if (isDataGet)
        {
            ShowVideo(VideoData);
        }
    }

    void ShowVideo(byte[] data)
    {
        StartTime = DateTime.Now;
        if ((StartTime - EndTime).TotalMilliseconds > 25)
        {
            ShowYUVFrames(width_, height_, data);
            EndTime = DateTime.Now;
        }
    }

    void ShowYUVFrames(int width, int height, byte[] data)
    {
        StartCoroutine(readFrames(width, height, data));
    }

    IEnumerator readFrames(int Width, int Height, byte[] data)
    {
        yield return null;
        if (data.Length > 0)
        {
            byte[] dataY = new byte[Width * Height];
            byte[] dataU = new byte[Width * Height / 4];
            byte[] dataV = new byte[Width * Height / 4];

            Buffer.BlockCopy(data, 0, dataY, 0, Width * Height);
            Buffer.BlockCopy(data, Width * Height, dataU, 0, Width * Height / 4);
            Buffer.BlockCopy(data, Width * Height * 5 / 4, dataV, 0, Width * Height / 4);
            CreateTexture(Width, Height, data, dataY, dataU, dataV);
        }
    }

    void CreateTexture(int width, int height, byte[] data, byte[] dataY, byte[] dataU, byte[] dataV)
    {
        if (textureY == null)
        {
            textureY = new Texture2D(width, height, TextureFormat.Alpha8, false);
        }
        textureY.LoadRawTextureData(dataY);
        textureY.Apply();

        if (textureU == null)
        {
            textureU = new Texture2D(width / 2, height / 2, TextureFormat.Alpha8, false);
        }
        textureU.LoadRawTextureData(dataU);
        textureU.Apply();

        if (textureV == null)
        {
            textureV = new Texture2D(width / 2, height / 2, TextureFormat.Alpha8, false);
        }
        textureV.LoadRawTextureData(dataV);
        textureV.Apply();

        rawImage.material.SetTexture("_MainTex", textureY);
        rawImage.material.SetTexture("_MainTexU", textureU);
        rawImage.material.SetTexture("_MainTexV", textureV);
        rawImage.SetMaterialDirty();
    }

    void CloseVideo()
    {
        if (realHandle_ < 0)
        {
            return;
        }
        if (!CHCNetSDK.NET_DVR_StopRealPlay(realHandle_))
        {
            Debug.Log("停止播放失败, 错误码= " + CHCNetSDK.NET_DVR_GetLastError() + "\n" + realHandle_);
            return;
        }
        if (port_ >= 0)
        {
            if (!PlayM4_Stop(port_))
            {
                string str = "PlayM4_Stop failed, error code= " + PlayM4_GetLastError(port_);
                Debug.LogError(str);
            }
            if (!PlayM4_CloseStream(port_))
            {
                string str = "PlayM4_CloseStream failed, error code= " + PlayM4_GetLastError(port_);
                Debug.LogError(str);
            }
            if (!PlayM4_FreePort(port_))
            {
                string str = "PlayM4_FreePort failed, error code= " + PlayM4_GetLastError(port_);
                Debug.LogError(str);
            }
            port_ = -1;
        }
        realHandle_ = -1;
        isDataGet = false;
        Debug.Log("停止播放成功!");
    }

    void LogOut()
    {
        if (userID_ < 0)
        {
            return;
        }
        if (!CHCNetSDK.NET_DVR_Logout(userID_))
        {
            Debug.LogError("登出失败, 错误码 = " + CHCNetSDK.NET_DVR_GetLastError());
        }
        userID_ = -1;
        Debug.Log("登出成功!");
    }

    public void DisConnect()
    {
        CloseVideo();
        LogOut();
    }

}
