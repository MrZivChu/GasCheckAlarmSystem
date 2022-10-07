using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class ServerSocketHelper : MonoBehaviour
{
    public static ServerSocketHelper instance;

    void Start()
    {
        instance = this;

        IPAddress address = IPAddress.Parse("127.0.0.1");
        IPEndPoint point = new IPEndPoint(address, 6666);
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(point);
        serverSocket.Listen(100);
        acceptSocketThread = new Thread(AcceptSocket);
        acceptSocketThread.IsBackground = true;
        acceptSocketThread.Start();
    }

    Socket serverSocket = null;
    Socket clientSocket = null;
    Thread acceptSocketThread = null;
    Thread reciveDataThread = null;
    bool isAcceptSocket = true;
    bool isReceiveData = true;

    void AcceptSocket()
    {
        while (isAcceptSocket)
        {
            try
            {
                clientSocket = serverSocket.Accept();
                reciveDataThread = new Thread(new ParameterizedThreadStart(ReciveData));
                reciveDataThread.IsBackground = true;
                reciveDataThread.Start(clientSocket);
            }
            catch (System.Exception)
            {
            }
        }
    }

    string receiveData = string.Empty;
    void ReciveData(object mySocket)
    {
        SendData(currentCommName);
        EventManager.Instance.DisPatch(NotifyType.UpdateSerialPortStatus);
        while (isReceiveData)
        {
            try
            {
                byte[] bytes = new byte[1024];
                int length = ((Socket)mySocket).Receive(bytes);
                if (length > 0)
                {
                    receiveData = Encoding.UTF8.GetString(bytes, 0, length);
                    Debug.Log("receive data = " + receiveData);
                    //HandleRealtimeData(receiveData);
                    RunInMainThread.EventDispatcher.Run(HandleRealtimeData);
                }
            }
            catch (System.Exception)
            {
            }
        }
    }

    Dictionary<string, HistoryDataModel> realtimeDataModelDic = new Dictionary<string, HistoryDataModel>();
    void HandleRealtimeData()
    {
        if (!string.IsNullOrEmpty(receiveData))
        {
            Debug.Log("start HandleRealtimeData ");
            HistoryDataModel model = LitJson.JsonMapper.ToObject<HistoryDataModel>(receiveData);
        }
    }

    public Dictionary<string, HistoryDataModel> GetRealtimeData()
    {
        return realtimeDataModelDic;
    }

    public void SendData(string content)
    {
        if (clientSocket != null)
        {
            Debug.Log("send data = " + content);
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            clientSocket.Send(bytes, bytes.Length, SocketFlags.None);
        }
    }

    void OnApplicationQuit()
    {
        Exit();
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus && Application.isEditor)
        {
            //Exit();
        }
    }

    void Exit()
    {
        SendData("close");
        isAcceptSocket = false;
        isReceiveData = false;
        if (serverSocket != null)
        {
            serverSocket.Close();
            serverSocket = null;
        }
        clientSocket = null;
        acceptSocketThread = null;
        reciveDataThread = null;
    }

    string currentCommName = string.Empty;
    public void Connect(string commName)
    {
        currentCommName = commName;
        string path = Application.streamingAssetsPath + "/GasCheckAlarmSystem.exe";
        System.Diagnostics.Process.Start(path);
    }

    public void Close()
    {
        SendData("close");
        if (clientSocket != null)
        {
            clientSocket.Close();
            clientSocket = null;
        }
    }

    public bool GetIsConnect()
    {
        return clientSocket != null && clientSocket.Connected;
    }
}
