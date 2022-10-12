using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    public class SerialPortInstance
    {
        SerialPort serialPort_ = null;
        SerialPortComData serialPortComData_ = null;
        MachineSerialPortInfo machineSerialPortInfo_ = null;
        ProtocolBase protocolBase_ = null;
        int machineIndex_ = 0;

        public void Run(SerialPortComData data)
        {
            if (data == null || data.list == null || data.list.Count <= 0)
            {
                return;
            }
            InitSerialPort(data);
            SendData();
        }

        void InitSerialPort(SerialPortComData data)
        {
            serialPortComData_ = data;
            serialPort_ = new SerialPort();
            serialPort_.DataReceived += DataReceived;
            serialPort_.PortName = serialPortComData_.portName;
            serialPort_.BaudRate = serialPortComData_.baudRate;
            serialPort_.DataBits = 8;
            serialPort_.Parity = Parity.None;
            serialPort_.StopBits = StopBits.One;
            serialPort_.Open();
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToReadCount = serialPort_.BytesToRead;
            byte[] buffer = new byte[bytesToReadCount];
            serialPort_.Read(buffer, 0, bytesToReadCount);
            bool isReceiveOver = HandleReceiveData(buffer, bytesToReadCount);
            if (isReceiveOver)
            {
                SendData();
            }
        }

        public void SendData()
        {
            if (machineIndex_ > serialPortComData_.list.Count)
            {

            }
            machineSerialPortInfo_ = serialPortComData_.list[machineIndex_];
            InitProtocol(machineSerialPortInfo_.MachineType);
            string sendContent = protocolBase_.SendData(machineSerialPortInfo_);
            byte[] sendbuffer = HexHelper.StrToHexByte(sendContent);
            serialPort_.Write(sendbuffer, 0, sendbuffer.Length);
            machineIndex_++;
        }

        protected float machineResponseOutTime = 1000.0f * 2.0f;//单个请求响应的超时时间 2秒
        bool canTimeMachineResponseOutTime = false;//是否可以计时主机响应的超时时间
        float tempMachineResponseOutTime = 0;
        public void Update(float deltaTime)
        {
            if (canTimeMachineResponseOutTime)
            {
                tempMachineResponseOutTime += deltaTime;
                if (tempMachineResponseOutTime >= machineResponseOutTime)
                {
                    LogHelper.AddLog("标一主机【地址{0} 类型:{1}】 ！！！！！！读取数据超时！！！！！！", machineSerialPortInfo_.MachineAddress, machineSerialPortInfo_.MachineType);
                    tempMachineResponseOutTime = 0;
                    canTimeMachineResponseOutTime = false;
                    SendData();
                }
            }
        }

        void InitProtocol(int machineType)
        {
            if (machineType == 0)
            {
                protocolBase_ = new StandardOneProtocol();
            }
            else if (machineType == 1)
            {
                protocolBase_ = new DZ40NewProtocol();
            }
            else if (machineType == 2)
            {
                protocolBase_ = new DZ40OldProtocol();
            }
            else if (machineType == 3)
            {
                protocolBase_ = new StandardProtocol();
            }
            else if (machineType == 4)
            {
                protocolBase_ = new HaiWanProtocol();
            }
        }
    }
}
