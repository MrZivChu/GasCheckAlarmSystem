using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

class ClientSocketHelper
{
    public static ClientSocketHelper instance;

    Socket clientSocket = null;
    Thread thread = null;
    Action<string> dataReceiveAction;

    public ClientSocketHelper(Action<string> receiveAction)
    {
        instance = this;
        dataReceiveAction = receiveAction;
    }

    public void Connect(string ip = "127.0.0.1", int port = 6666)
    {
        IPAddress address = IPAddress.Parse(ip);
        IPEndPoint point = new IPEndPoint(address, port);
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(point);
        thread = new Thread(ReciveData);
        thread.Start();
    }

    bool isExit = false;
    void ReciveData()
    {
        while (!isExit)
        {
            if (clientSocket != null)
            {
                byte[] bytes = new byte[1024];
                int length = clientSocket.Receive(bytes);
                string result = Encoding.UTF8.GetString(bytes, 0, length);
                if (dataReceiveAction != null)
                    dataReceiveAction(result);
            }
        }
    }

    public void SendData(string content)
    {
        if (clientSocket != null)
            clientSocket.Send(Encoding.UTF8.GetBytes(content));
    }

    public void Close()
    {
        isExit = true;
        if (clientSocket != null)
        {
            clientSocket.Close();
            clientSocket = null;
        }
        thread = null;
        dataReceiveAction = null;
    }
}
