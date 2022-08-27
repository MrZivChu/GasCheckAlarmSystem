﻿using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using GasCheckAlarmSystem;

//读1号主机1号到5号气体浓度
//上位机发送：   01      03       0000              0005            D409
//            主机地址  命令 探测器起始地址      读取寄存器数量 CRC校验

//上位接收数据：　01 　　03 　　0A 　　　 0004              0000　　    
//            主机地址 命令字节数量 一号探测器浓度  二号探测器浓度　
//0000              0000　　      0000              1676　　    
//三号探测器浓度 四号探测器浓度  五号探测器浓度 CRC校验


public class SerialPortHelper
{
    public static Dictionary<int, int> baudRateFormat = new Dictionary<int, int>() {
        { 0,4800 },{ 1,9600 },{ 2,9600 },{ 3,4800 },{ 4,4800 }
    };


    SerialPort serialPort_;
    string portName_ = string.Empty;
    int baudRate_ = -1;
    public void StartConnect(string portName)
    {
        portName_ = portName;
        HandleMachineData();
    }

    List<MachineSerialPortInfo> machineSerialPortInfoBaseList_;
    public void HandleMachineData()
    {
        try
        {
            List<ProbeSerialPortInfo> infoList = MachineDAL.SelectAllMachineSerialPortInfo();
            //主机地址对应它所关联的探头列表
            Dictionary<string, List<ProbeSerialPortInfo>> dic = new Dictionary<string, List<ProbeSerialPortInfo>>();
            if (infoList != null && infoList.Count > 0)
            {
                for (int i = 0; i < infoList.Count; i++)
                {
                    ProbeSerialPortInfo info = infoList[i];
                    if (dic.ContainsKey(info.MachineAddress))
                    {
                        dic[info.MachineAddress].Add(info);
                    }
                    else
                    {
                        dic.Add(info.MachineAddress, new List<ProbeSerialPortInfo>() { info });
                    }
                }
            }
            if (dic != null && dic.Keys.Count > 0)
            {
                machineSerialPortInfoBaseList_ = new List<MachineSerialPortInfo>();
                foreach (var item in dic)
                {
                    MachineSerialPortInfo infoBase = new MachineSerialPortInfo();
                    infoBase.MachineAddress = item.Key;
                    if (item.Value.Count > 0)
                    {
                        infoBase.MachineType = item.Value[0].MachineType;
                        int firstProbeDecAddress = Convert.ToInt32(item.Value[0].ProbeAddress);
                        int endProbeDecAddress = firstProbeDecAddress;
                        if (item.Value.Count > 1)
                        {
                            for (int i = 1; i < item.Value.Count; i++)
                            {
                                if (Convert.ToInt32(item.Value[i].ProbeAddress) > endProbeDecAddress)
                                {
                                    endProbeDecAddress = Convert.ToInt32(item.Value[i].ProbeAddress);
                                }
                                else if (Convert.ToInt32(item.Value[i].ProbeAddress) < firstProbeDecAddress)
                                {
                                    firstProbeDecAddress = Convert.ToInt32(item.Value[i].ProbeAddress);
                                }
                                infoBase.MachineType = item.Value[i].MachineType;
                            }
                        }
                        infoBase.FirstProbeDecAddress = firstProbeDecAddress.ToString();
                        infoBase.EndProbeDecAddress = endProbeDecAddress.ToString();
                    }
                    infoBase.list = item.Value;
                    machineSerialPortInfoBaseList_.Add(infoBase);
                }
            }
            machineSerialPortInfoBaseList_.Sort((a, b) =>
            {
                return a.MachineType < b.MachineType ? 0 : 1;
            });
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show("处理主机数据出错：" + ex.Message);
            File.WriteAllText("D:\\HandleMachineData.txt", ex.Message);
        }
    }

    void SerialPortConnect(string portName, int baudRate, int dataBits, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
    {
        try
        {
            CloseCurrentSerialPort();
            serialPort_ = new SerialPort();
            serialPort_.DataReceived += SerialPort_DataReceived;
            serialPort_.ErrorReceived += SerialPort_ErrorReceived;
            serialPort_.PortName = portName;
            serialPort_.BaudRate = baudRate;
            serialPort_.DataBits = dataBits;
            serialPort_.Parity = parity;
            serialPort_.StopBits = stopBits;
            serialPort_.Open();
        }
        catch (Exception ex)
        {
            CloseCurrentSerialPort();
            Form1.instance.AddLog("SerialPortConnect 串口Open出错：" + ex.Message);
            File.WriteAllText("D:\\SerialPortConnect.txt", ex.Message);
        }
    }

    private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        CloseCurrentSerialPort();
        Form1.instance.AddLog("SerialPort_ErrorReceived Error：" + e.EventType.ToString());
        File.WriteAllText("D:\\SerialPort_ErrorReceived.txt", "串口接收数据出错：" + e.EventType.ToString());
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            Form1.instance.AddLog("数据接收发生回调DataReceived");
            if (machineSerialPortInfo_ != null && machineSerialPortInfo_.list != null && machineSerialPortInfo_.list.Count > 0)
            {
                if (protocol_ != null && serialPort_ != null)
                {
                    tempMachineResponseOutTime = 0;
                    canTimeMachineResponseOutTime = false;
                    int bytesToReadCount = serialPort_.BytesToRead;
                    byte[] buffer = new byte[bytesToReadCount];
                    serialPort_.Read(buffer, 0, bytesToReadCount);
                    bool isReceiveOver = protocol_.HandleReceiveData(buffer, bytesToReadCount);
                    if (isReceiveOver)
                    {
                        if (protocol_.IsHandleOver())
                        {
                            tempQueryNextMachineDataIntervalTime = 0;
                            canQueryNextMachineData = true;
                            protocol_ = null;
                        }
                        else
                        {
                            SendData();
                        }
                    }
                }
                else
                {
                    Form1.instance.AddLog("发生异常，串口没有开启");
                }
            }
            else
            {
                Form1.instance.AddLog("发生异常，没有数据");
            }
        }
        catch (Exception ex)
        {
            File.WriteAllText("D:\\DataReceivedError.txt", ex.Message);
        }
    }

    public void SendData()
    {
        try
        {
            if (machineSerialPortInfo_ != null && serialPort_ != null && serialPort_.IsOpen)
            {
                if (protocol_ == null)
                {
                    if (machineSerialPortInfo_.MachineType == 0)
                    {
                        protocol_ = new StandardOneProtocol();
                        Form1.instance.AddLog("######开启标1协议######");
                    }
                    else if (machineSerialPortInfo_.MachineType == 1)
                    {
                        protocol_ = new DZ40NewProtocol();
                        Form1.instance.AddLog("######开启DZ-40-New协议######");
                    }
                    else if (machineSerialPortInfo_.MachineType == 2)
                    {
                        protocol_ = new DZ40OldProtocol();
                        Form1.instance.AddLog("######开启DZ-40-Old协议######");
                    }
                    else if (machineSerialPortInfo_.MachineType == 3)
                    {
                        protocol_ = new StandardProtocol();
                        Form1.instance.AddLog("######开启标准协议######");
                    }
                    else if (machineSerialPortInfo_.MachineType == 4)
                    {
                        protocol_ = new HaiWanProtocol();
                        Form1.instance.AddLog("######开启海湾协议######");
                    }
                }
                else
                {
                    Form1.instance.AddLog("######复用之前协议" + protocol_.GetProtocolName() + "######");
                }
                string sendContent = string.Empty;
                protocol_.HandleSendData(machineSerialPortInfo_, out sendContent);
                if (!string.IsNullOrEmpty(sendContent))
                {
                    tempMachineResponseOutTime = 0;
                    canTimeMachineResponseOutTime = true;
                    byte[] sendbuffer = HexHelper.StrToHexByte(sendContent);
                    serialPort_.Write(sendbuffer, 0, sendbuffer.Length);
                }
            }
        }
        catch (Exception ex)
        {
            File.WriteAllText("D:\\SendDataError.txt", ex.Message);
        }
    }

    public void FixedUpdate(float deltaTime)
    {
        HandleData(deltaTime);
    }

    public float queryWholeMachineDataIntervalTime = 1000.0f * 0.0f;//间隔多长时间再刷新一波实时数据
    public float queryNextMachineDataIntervalTime = 1000.0f * 1.0f;//间隔多长时间查询下一个主机数据
    public float machineResponseOutTime = 1000.0f * 2.0f;//单个主机响应的超时时间

    float tempQueryWholeMachineDataIntervalTime = 0;
    float tempQueryNextMachineDataIntervalTime = 0;
    float tempMachineResponseOutTime = 0;

    bool canQueryWholeMachineData = false;//是否可以查询下一波主机的数据
    bool canQueryNextMachineData = true;//是否可以查询下一个主机的数据
    bool canTimeMachineResponseOutTime = false;//是否可以计时主机响应的超时时间

    int readMachineIndex = 0;
    MachineSerialPortInfo machineSerialPortInfo_ = null;
    ProtocolBase protocol_ = null;
    void HandleData(float deltaTime)
    {
        if (machineSerialPortInfoBaseList_ != null && machineSerialPortInfoBaseList_.Count > 0)
        {
            if (canQueryWholeMachineData)
            {
                tempQueryWholeMachineDataIntervalTime += deltaTime;
                if (tempQueryWholeMachineDataIntervalTime >= queryWholeMachineDataIntervalTime)
                {
                    readMachineIndex = 0;

                    tempQueryWholeMachineDataIntervalTime = 0;
                    canQueryWholeMachineData = false;

                    tempQueryNextMachineDataIntervalTime = 0;
                    canQueryNextMachineData = true;
                }
            }
            if (canQueryNextMachineData)
            {
                tempQueryNextMachineDataIntervalTime += deltaTime;
                if (tempQueryNextMachineDataIntervalTime >= queryNextMachineDataIntervalTime)
                {
                    tempQueryNextMachineDataIntervalTime = 0;
                    canQueryNextMachineData = false;

                    if (readMachineIndex < machineSerialPortInfoBaseList_.Count)
                    {
                        machineSerialPortInfo_ = machineSerialPortInfoBaseList_[readMachineIndex];
                        if (machineSerialPortInfo_.MachineType == 0)
                        {
                            Form1.instance.AddLog("标1主机需要做个0.2s延迟请求");
                            GasCheckAlarmSystem.Timer timer = new GasCheckAlarmSystem.Timer(0.2f, QueryNextMachine);
                            TimerHelper.GetInstance().AddTimer(timer);
                        }
                        else
                        {
                            QueryNextMachine();
                        }
                    }
                    else
                    {
                        Form1.instance.AddChangeLineLog();
                        Form1.instance.AddLog("$$$$$$$$准备刷新新一轮数据$$$$$$$$$\n");
                        tempQueryWholeMachineDataIntervalTime = 0;
                        canQueryWholeMachineData = true;
                    }
                }
            }
            if (canTimeMachineResponseOutTime)
            {
                tempMachineResponseOutTime += deltaTime;
                if (tempMachineResponseOutTime >= machineResponseOutTime)
                {
                    Form1.instance.AddLog("！！！！！！读取数据超时！！！！！！");
                    tempMachineResponseOutTime = 0;
                    canTimeMachineResponseOutTime = false;

                    if (protocol_ != null && protocol_.IsHandleOver())
                    {
                        tempQueryNextMachineDataIntervalTime = 0;
                        canQueryNextMachineData = true;
                        protocol_ = null;
                    }
                    else
                    {
                        SendData();
                    }
                }
            }
        }
    }

    private void QueryNextMachine()
    {
        Form1.instance.AddChangeLineLog();
        Form1.instance.AddLog("**********************读取【Type=" + machineSerialPortInfo_.MachineType + "-地址=" + machineSerialPortInfo_.MachineAddress + "】主机数据**********************");

        readMachineIndex++;
        int baudRate = baudRateFormat[machineSerialPortInfo_.MachineType];
        if (baudRate != baudRate_ || serialPort_ == null)
        {
            baudRate_ = baudRate;
            Form1.instance.AddLog("重新开启串口：" + baudRate_);
            SerialPortConnect(portName_, baudRate_, 8, Parity.None, StopBits.One);
        }
        else
        {
            Form1.instance.AddLog("复用串口：" + baudRate_);
        }
        if (serialPort_ == null || (serialPort_ != null && !serialPort_.IsOpen))
        {
            Form1.instance.AddLog("串口开启失败，请求下一个主机数据");
            tempQueryNextMachineDataIntervalTime = 0;
            canQueryNextMachineData = true;
            protocol_ = null;
            return;
        }
        SendData();
    }

    public void CloseCurrentSerialPort()
    {
        if (serialPort_ != null)
        {
            serialPort_.Close();
            serialPort_ = null;
        }
    }
}
