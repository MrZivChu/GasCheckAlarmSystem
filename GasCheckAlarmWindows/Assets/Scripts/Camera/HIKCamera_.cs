using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using PreviewDemo;

public class HIKCamera_ : MonoBehaviour
{
    // add by gb 080131 version 4.9.0.1
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_GetPort(ref int nPort);
    [DllImport("PlayCtrl")]
    public static extern uint PlayM4_GetLastError(int nPort);
    [DllImport("PlayCtrl")]
    public static extern bool PlayM4_SetStreamOpenMode(int nPort, uint nMode);
    //Stream type
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
    private bool m_bInitSDK = false;
    private bool m_bRecord = false;
    private uint iLastErr = 0;
    public Int32 m_lUserID = -1;
    private Int32 m_lRealHandle = -1;
    private string str;
    private Int32 m_lPort = -1;
    private IntPtr m_ptrRealHandle;
    private CHCNetSDK.REALDATACALLBACK RealData = null;
    public CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo;
    public CHCNetSDK.NET_DVR_IPPARACFG_V40 m_struIpParaCfgV40;
    public CHCNetSDK.NET_DVR_STREAM_MODE m_struStreamMode;
    public CHCNetSDK.NET_DVR_IPCHANINFO m_struChanInfo;
    public CHCNetSDK.NET_DVR_IPCHANINFO_V40 m_struChanInfoV40;
    private DECCBFUN m_fDisplayFun = null;
    int width = 3840;//视屏的宽
    int height = 2160;//视屏的高
    public bool isPlaying;

   
    void OnEnable()
    {
        VideoMaterial = OriginMaterial;
        thread = new Thread(ReadData);
        thread.Start();
    }
    private bool mataol,mataolFlag = false;

    private void Update()
    {
        if (mataol&&mataolFlag)
        {
            this.GetComponent<UnityEngine.UI.Image>().material = OriginMaterial;
            this.GetComponent<UnityEngine.UI.Image>().sprite = null;
            mataolFlag = false;

        }
        else if(mataolFlag)
        {
            this.GetComponent<UnityEngine.UI.Image>().material = null;
            this.GetComponent<UnityEngine.UI.Image>().sprite = spriteGMC;
            mataolFlag = false;
        }
    }

    public string tip, tindex;
    private string tport, tuser, tpassword;
    public Material OriginMaterial;
    public Sprite spriteGMC;

    /// <summary>
    /// 初始化，一次就行（初始化->登录->获取视屏流）
    /// </summary>
    private void InitSDK()
    {
        m_bInitSDK = CHCNetSDK.NET_DVR_Init();
        if (m_bInitSDK == false)
        {
            Debug.Log("初始化失败");
            return;
        }
        else
        {
            //保存SDK日志 To save the SDK log
            //CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
            Debug.Log("初始化成功");
            //Login(tip, "8000", "admin", "abc123456");
            Login(tip, "8000", "admin", "bao608806");
        }
    }
    /// <summary>
    /// 登录（初始化->登录->获取视屏流）
    /// </summary>
    /// <param name="Ip"></param>
    /// <param name="Port"></param>
    /// <param name="UserName"></param>
    /// <param name="PassWord"></param>
    /// <param name="action"></param>
    public void Login(string Ip, string Port, string UserName, string PassWord)
    {
        if (m_lUserID < 0)
        {
            //登录设备
            m_lUserID = CHCNetSDK.NET_DVR_Login_V30(Ip, Int16.Parse(Port), UserName, PassWord, ref DeviceInfo);
            if (m_lUserID < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                Debug.Log("登录失败:错误号+" + iLastErr);
                return;
            }
            else
            {
                Debug.Log("登录成功");
            }
        }
        VideoBtnClick(int.Parse(tindex));
    }
    /// <summary>
    /// 登出
    /// </summary>
    public void LogOut()
    {
        //注销登录 Logout the device
        if (m_lRealHandle >= 0)
        {
            Debug.Log("Please stop live view firstly"); //登出前先停止预览 Stop live view before logout
            CloseVideo();
            //return;
        }
        if (!CHCNetSDK.NET_DVR_Logout(m_lUserID))
        {
            iLastErr = CHCNetSDK.NET_DVR_GetLastError();
            str = "NET_DVR_Logout failed, error code= " + iLastErr;
            Debug.Log(str);
            return;
        }
        Debug.Log("NET_DVR_Logout success!");
        m_lUserID = -1;
    }
  
    /// <summary>
    /// 获取视屏流（初始化->登录->获取视屏流）
    /// </summary>
    /// <param name="CameraIndex">多路摄像机，具体那一路</param>
    public void VideoBtnClick(int CameraIndex)
    {
        if (m_lUserID < 0)
        {
            Debug.Log("Please login the device firstly!");
            return;
        }
        if (m_bRecord)
        {
            Debug.Log("Please stop recording firstly!");
            return;
        }
        if (m_lRealHandle < 0)
        {
            CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
            lpPreviewInfo.lChannel = CameraIndex;//预览的设备通道 the device channel number
            lpPreviewInfo.dwStreamType = 1;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            lpPreviewInfo.dwLinkMode = 4;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
            lpPreviewInfo.bBlocked = false; //0- 非阻塞取流，1- 阻塞取流
            lpPreviewInfo.dwDisplayBufNum = 120; //播放库显示缓冲区最大帧数
            IntPtr pUser = IntPtr.Zero;//用户数据 user data
            //直接打开视屏
            lpPreviewInfo.hPlayWnd = IntPtr.Zero;//预览窗口 live view window           
            RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数 real-time stream callback function 
            m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, RealData, pUser);
            if (m_lRealHandle < 0)
            {
                mataolFlag = true;
            }
            else
            {
                //预览成功
                Debug.Log("预览成功");
                mataol = true;
                mataolFlag = true;


            }
        }
        else
        {
            //Debug.Log("m_lRealHandle Error!");
        }
    }
    /// <summary>
    /// 停止视屏预览
    /// </summary>
    public void CloseVideo()
    {
        //停止预览 Stop live view 
        if (!CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
        {
            iLastErr = CHCNetSDK.NET_DVR_GetLastError();
            str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr + "\n" + m_lRealHandle;
            Debug.Log(str);
            return;
        }
        if ((m_lPort >= 0))
        {
            if (!PlayM4_Stop(m_lPort))
            {
                iLastErr = PlayM4_GetLastError(m_lPort);
                str = "PlayM4_Stop failed, error code= " + iLastErr;
                Debug.Log(str);
            }
            if (!PlayM4_CloseStream(m_lPort))
            {
                iLastErr = PlayM4_GetLastError(m_lPort);
                str = "PlayM4_CloseStream failed, error code= " + iLastErr;
                Debug.Log(str);
            }
            if (!PlayM4_FreePort(m_lPort))
            {
                iLastErr = PlayM4_GetLastError(m_lPort);
                str = "PlayM4_FreePort failed, error code= " + iLastErr;
                Debug.Log(str);
            }
            m_lPort = -1;
        }
        Debug.Log("NET_DVR_StopRealPlay succ!");
        m_lRealHandle = -1;
        isDataGet = false;
        isPlaying = false;
    }

    private UInt32 dwBufSizes;
    private IntPtr pBuffers;
    /// <summary>
    /// 视屏回调
    /// </summary>
    /// <param name="lRealHandle"></param>
    /// <param name="dwDataType"></param>
    /// <param name="pBuffer"></param>
    /// <param name="dwBufSize"></param>
    /// <param name="pUser"></param>
    public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
    {
        dwBufSizes = dwBufSize;
        pBuffers = pBuffer;
        //Debug.Log("RealDataCallBack:"+pBuffer);
        //Debug.Log(dwDataType);
        switch (dwDataType)
        {
            case CHCNetSDK.NET_DVR_SYSHEAD:     // sys head
                if (dwBufSize > 0)
                {
                    if (m_lPort >= 0)
                    {
                        return; //同一路码流不需要多次调用开流接口
                    }
                    //获取播放句柄 Get the port to play
                    if (!PlayM4_GetPort(ref m_lPort))
                    {
                        iLastErr = PlayM4_GetLastError(m_lPort);
                        str = "PlayM4_GetPort failed, error code= " + iLastErr;
                        Debug.Log(str);
                        break;
                    }
                    //设置流播放模式 Set the stream mode: real-time stream mode
                    if (!PlayM4_SetStreamOpenMode(m_lPort, STREAME_REALTIME))
                    {
                        iLastErr = PlayM4_GetLastError(m_lPort);
                        str = "Set STREAME_REALTIME mode failed, error code= " + iLastErr;
                        Debug.Log(str);
                    }
                    //打开码流，送入头数据 Open stream
                    if (!PlayM4_OpenStream(m_lPort, pBuffer, dwBufSize, 2 * 1024 * 1024))
                    {
                        iLastErr = PlayM4_GetLastError(m_lPort);
                        str = "PlayM4_OpenStream failed, error code= " + iLastErr;
                        Debug.Log(str);
                        break;
                    }

                    //设置显示缓冲区个数 Set the display buffer number
                    if (!PlayM4_SetDisplayBuf(m_lPort, 15))
                    {
                        iLastErr = PlayM4_GetLastError(m_lPort);
                        str = "PlayM4_SetDisplayBuf failed, error code= " + iLastErr;
                        Debug.Log(str);
                    }
                    //设置显示模式 Set the display mode
                    if (!PlayM4_SetOverlayMode(m_lPort, 0, 0/* COLORREF(0)*/)) //play off screen 
                    {
                        iLastErr = PlayM4_GetLastError(m_lPort);
                        str = "PlayM4_SetOverlayMode failed, error code= " + iLastErr;
                        Debug.Log(str);
                    }
                    //设置解码回调函数，获取解码后音视频原始数据 Set callback function of decoded data
                    m_fDisplayFun = new DECCBFUN(DecCallbackFUN);
                    bool decflag = false;
                    if (!(decflag=PlayM4_SetDecCallBack(m_lPort, m_fDisplayFun)))
                    {
                        Debug.Log("PlayM4 CallBack Failed!");
                    }
                    print(decflag);
                    //开始解码 Start to play                       
                    if (!PlayM4_Play(m_lPort, m_ptrRealHandle))
                    {
                        iLastErr = PlayM4_GetLastError(m_lPort);
                        str = "PlayM4_Play failed, error code= " + iLastErr;
                        Debug.Log(str);
                        break;
                    }
                }
                break;
            case CHCNetSDK.NET_DVR_STREAMDATA:     // video stream data
                //Debug.Log("GetStreamData!");
                if (dwBufSizes > 0 && m_lPort != -1)
                {
                    for (int i = 0; i < 999; i++)
                    {
                        //送入码流数据进行解码 Input the stream data to decode
                        if (!PlayM4_InputData(m_lPort, pBuffers, dwBufSizes))
                        {
                            iLastErr = PlayM4_GetLastError(m_lPort);
                            str = "PlayM4_InputData failed, error code= " + iLastErr;
                            Debug.Log(str);
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
    private byte[] VideoData;
    private bool isDataGet = false;
    /// <summary>
    /// 解码后视屏回调（YUV格式）
    /// </summary>
    /// <param name="nPort"></param>
    /// <param name="pBuf"></param>
    /// <param name="nSize"></param>
    /// <param name="pFrameInfo"></param>
    /// <param name="nReserved1"></param>
    /// <param name="nReserved2"></param>
    private void DecCallbackFUN(int nPort, IntPtr pBuf, int nSize, ref Frame_Info pFrameInfo, int nReserved1, int nReserved2)
    {
        if (pFrameInfo.nType == 3) //#define T_YV12 3
        {


            width = pFrameInfo.nWidth;
            height = pFrameInfo.nHeight;
            //Debug.Log("Video Width:" + width + " Height:" + height + " Size:" + nSize);

            VideoData = new byte[nSize];
            Marshal.Copy(pBuf, VideoData, 0, nSize);
            isDataGet = true;

        }
    }
    void FixedUpdate()
    {
        if (threadDirector)
        {
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start(); //  开始监视代码

            CreateTexture(width, height, flagDataY, flagDataU, flagDataV);
            threadDirector = false;

            //stopwatch.Stop(); //  停止监视
            //TimeSpan timeSpan = stopwatch.Elapsed; //  获取总时间
            //double seconds = timeSpan.TotalSeconds;  //  秒数
            //double milliseconds = timeSpan.TotalMilliseconds;  //  毫秒数
            //print(string.Format("{0}:{1}", seconds, milliseconds));

        }
    }

    //读取数据的线程
    public void ReadData()
    {

        InitSDK();
       
        while (true)
        {
            print("循环执行");
            if (isDataGet)
            {
                
                StartTime = DateTime.Now;
                if ((StartTime - EndTime).TotalMilliseconds > 25)
                {
                    flagData = VideoData;

                    EndTime = DateTime.Now;

                    if (flagData.Length > 0)
                    {
                        flagDataY = new byte[width * height];
                        flagDataU = new byte[width * height / 4];
                        flagDataV = new byte[width * height / 4];
                        Buffer.BlockCopy(flagData, 0, flagDataY, 0, width * height);
                        Buffer.BlockCopy(flagData, width * height, flagDataU, 0, width * height / 4);
                        Buffer.BlockCopy(flagData, width * height * 5 / 4, flagDataV, 0, width * height / 4);
                        flagData = null;
                        threadDirector = true;
                        //texY = new Texture2D(width, height, TextureFormat.Alpha8, false);
                    }
                }
            }
            
            Thread.Sleep(100);
        }
    }

    public byte[] flagData,flagDataY, flagDataU, flagDataV;
    public Thread thread;
    public bool threadDirector=false;
    private DateTime StartTime;
    private DateTime EndTime;
    /// <summary>
    /// 显示视屏流
    /// </summary>
    /// <param name="data">YV12视频流</param>
    private void ShowVideo(byte[] data)
    {
        StartTime = DateTime.Now;
        if ((StartTime - EndTime).TotalMilliseconds > 25)
        {
            //ShowYUVFrames(width, height, data);
            flagData = data;
            EndTime = DateTime.Now;
        }
    }
    [HideInInspector]  public Material VideoMaterial;
     
    private Texture2D texY, texU, texV;
    /// <summary>
    /// 显示YUV帧图片
    /// </summary>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <param name="data">YUV比特流</param>
    private void ShowYUVFrames(int width, int height, byte[] data)
    {
        StartCoroutine(readFrames(width, height, data));
    }
    /// <summary>
    /// 读取YUV帧协程
    /// </summary>
    /// <param name="Width"></param>
    /// <param name="Height"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerator readFrames(int Width, int Height, byte[] data)
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
            //CreateTexture(Width, Height, dataY, dataU, dataV);
        }
    }
    /// <summary>
    /// 通过YUV数据创建图片
    /// </summary>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <param name="dataY">灰度图片Y</param>
    /// <param name="dataU">灰度图片U</param>
    /// <param name="dataV">灰度图片V</param>
    private void CreateTexture(int width, int height, byte[] dataY, byte[] dataU, byte[] dataV)
    {
        //Y  
        if (texY)
        {
            Destroy(texY);
        }
        texY = new Texture2D(width, height, TextureFormat.Alpha8, false);
        texY.LoadRawTextureData(dataY);
        texY.Apply();

        //U  
        if (texU)
        {
            Destroy(texU);
        }
        texU = new Texture2D(width / 2, height / 2, TextureFormat.Alpha8, false);
        texU.LoadRawTextureData(dataU);
        texU.Apply();
        //V  
        if (texV)
        {
            Destroy(texV);
        }
        texV = new Texture2D(width / 2, height / 2, TextureFormat.Alpha8, false);
        texV.LoadRawTextureData(dataV);
        texV.Apply();
        threadDirector = true;
        //VideoMaterial.mainTexture = texY;
        VideoMaterial.SetTexture("_MainTex", texY);
        VideoMaterial.SetTexture("_UTex", texU);
        VideoMaterial.SetTexture("_VTex", texV);

    }
   
    void OnDisable()
    {
        if (thread != null)
        {
            thread.Abort();
            thread = null;
        }
        CloseVideo();
        CHCNetSDK.NET_DVR_Cleanup();
        Debug.Log("Quit Success! " + transform.name);

    }
    private void OnDestroy()
    {
        if (thread != null)
        {
            thread.Abort();
            thread = null;
        }
        CloseVideo();
       
    }
}