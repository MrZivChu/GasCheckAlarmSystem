using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    class SerialPortRunner
    {
        string portName_ = string.Empty;
        SerialPort serialPort_ = null;
        MachineSerialPortInfo currentData_ = null;
        List<MachineSerialPortInfo> allDataList_ = null;
        public void Run(string portName, List<MachineSerialPortInfo> list)
        {
            if (string.IsNullOrEmpty(portName) || list == null || list.Count == 0)
            {
                return;
            }
            portName_ = portName;
            allDataList_ = list;
            tempQueryNextMachineDataIntervalTime = 0;
            canQueryNextMachineData = true;
        }

        int currentIndex = -1;
        void UpdateCurrentData()
        {
            if (allDataList_ != null && allDataList_.Count > 0)
            {
                if (currentIndex + 1 >= allDataList_.Count)
                {
                    currentIndex = 0;
                }
                else
                {
                    currentIndex++;
                }
                currentData_ = allDataList_[currentIndex];
            }
        }

        int openSerialPortMaxNums_ = 6;
        int tempOpenSerialPortNums_ = 0;
        void OpenSerialPort(int baudRate)
        {
            if (serialPort_ != null && serialPort_.IsOpen)
            {
                if (serialPort_.BaudRate == baudRate)
                {
                    return;
                }
            }
            try
            {
                CloseCurrentSerialPort();
                serialPort_ = new SerialPort();
                serialPort_.DataReceived += SerialPort_DataReceived;
                serialPort_.PortName = portName_;
                serialPort_.BaudRate = baudRate;
                serialPort_.DataBits = 8;
                serialPort_.Parity = Parity.None;
                serialPort_.StopBits = StopBits.One;
                serialPort_.Open();
                tempOpenSerialPortNums_ = 0;
            }
            catch
            {
                tempOpenSerialPortNums_++;
                if (tempOpenSerialPortNums_ < openSerialPortMaxNums_)
                {
                    OpenSerialPort(baudRate);
                }
            }
        }

        public void CloseCurrentSerialPort()
        {
            if (serialPort_ != null)
            {
                serialPort_.Close();
                serialPort_ = null;
            }
        }

        public void FixedUpdate(float deltaTime)
        {
            HandleData(deltaTime);
        }

        public float queryNextMachineDataIntervalTime = 1000.0f * 1.0f;//间隔多长时间查询下一个主机数据
        public float machineResponseOutTime = 1000.0f * 2.0f;//单个主机响应的超时时间
        float tempQueryNextMachineDataIntervalTime = 0;
        float tempMachineResponseOutTime = 0;
        bool canQueryNextMachineData = false;//是否可以查询下一个主机的数据
        bool canTimeMachineResponseOutTime = false;//是否可以计时主机响应的超时时间
        void HandleData(float deltaTime)
        {
            if (canQueryNextMachineData)
            {
                tempQueryNextMachineDataIntervalTime += deltaTime;
                if (tempQueryNextMachineDataIntervalTime >= queryNextMachineDataIntervalTime)
                {
                    tempQueryNextMachineDataIntervalTime = 0;
                    canQueryNextMachineData = false;
                    LogHelper.AddChangeLine();
                    LogHelper.AddLog("****************获取下一个主机的数据***************");

                    UpdateCurrentData();
                    OpenSerialPort(currentData_.BaudRate);
                    SendData(false);
                    tempMachineResponseOutTime = 0;
                    canTimeMachineResponseOutTime = true;
                }
            }
            if (canTimeMachineResponseOutTime)
            {
                tempMachineResponseOutTime += deltaTime;
                if (tempMachineResponseOutTime >= machineResponseOutTime)
                {
                    tempMachineResponseOutTime = 0;
                    canTimeMachineResponseOutTime = false;
                    LogHelper.AddLog("！！！！！！！！！！！请求数据超时！！！！！！！！！！！");
                    //标准协议需要一个探头一个探头去要数据
                    if (protocol_ != null && protocol_.IsHandleOver())
                    {
                        tempQueryNextMachineDataIntervalTime = 0;
                        canQueryNextMachineData = true;
                    }
                    else
                    {
                        SendData(true);
                    }
                }
            }
        }

        ProtocolBase protocol_ = null;
        public void SendData(bool isReuseProtocol)
        {
            if (currentData_ != null && serialPort_ != null && serialPort_.IsOpen)
            {
                if (!isReuseProtocol)
                {
                    if (currentData_.ProtocolType == EProtocolType.StandardOne)
                    {
                        protocol_ = new StandardOneProtocol();
                        LogHelper.AddLog("######使用标1协议######");
                    }
                    else if (currentData_.ProtocolType == EProtocolType.DZ40New)
                    {
                        protocol_ = new DZ40NewProtocol();
                        LogHelper.AddLog("######使用DZ-40-New协议######");
                    }
                    else if (currentData_.ProtocolType == EProtocolType.DZ40Old)
                    {
                        protocol_ = new DZ40OldProtocol();
                        LogHelper.AddLog("######使用DZ-40-Old协议######");
                    }
                    else if (currentData_.ProtocolType == EProtocolType.Standard)
                    {
                        protocol_ = new StandardProtocol();
                        LogHelper.AddLog("######使用标准协议######");
                    }
                    else if (currentData_.ProtocolType == EProtocolType.HaiWan)
                    {
                        protocol_ = new HaiWanProtocol();
                        LogHelper.AddLog("######使用海湾协议######");
                    }
                }
                string sendContent = string.Empty;
                protocol_.HandleSendData(currentData_, out sendContent);
                if (!string.IsNullOrEmpty(sendContent))
                {
                    byte[] sendbuffer = HexHelper.StrToHexByte(sendContent);
                    serialPort_.Write(sendbuffer, 0, sendbuffer.Length);
                }
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (currentData_ != null & protocol_ != null && serialPort_ != null)
                {
                    int bytesToReadCount = serialPort_.BytesToRead;
                    byte[] buffer = new byte[bytesToReadCount];
                    serialPort_.Read(buffer, 0, bytesToReadCount);
                    bool isReceiveOver = protocol_.HandleReceiveData(buffer, bytesToReadCount);
                    if (isReceiveOver)
                    {
                        canTimeMachineResponseOutTime = false;
                        if (protocol_.IsHandleOver())
                        {
                            tempQueryNextMachineDataIntervalTime = 0;
                            canQueryNextMachineData = true;
                        }
                        else
                        {
                            SendData(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AddLog("处理数据异常：" + ex.Message);
            }
        }
    }
}
