using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    class StandardProtocol : ProtocolBase
    {
        public override string GetProtocolName()
        {
            return "标准协议";
        }

        public override bool HandleReceiveData(byte[] buffer, int bytesToReadCount)
        {
            if (machineSerialPortInfo_ == null || bytesToReadCount == 0)
            {
                return false;
            }
            tempReadAllByteLength_ += bytesToReadCount;
            stageByteList_.AddRange(buffer);
            GasCheckAlarmSystem.Form1.instance.AddLog("本次接收数据量：" + bytesToReadCount + ",数据接收进度：" + tempReadAllByteLength_ + "/" + readAllByteLength_);
            if (tempReadAllByteLength_ < readAllByteLength_)
            {
                return false;
            }
            if (Convert.ToInt32(stageByteList_[0].ToString()) != Convert.ToInt32(machineSerialPortInfo_.MachineAddress))
            {
                GasCheckAlarmSystem.Form1.instance.AddLog("数据校验没通过");
                return false;
            }

            //数据解析处理
            int index = 3;
            if (index + 1 >= stageByteList_.Count)
                return false;

            byte highByte = stageByteList_[index];
            byte lowByte = stageByteList_[index + 1];
            float decValue = (highByte * 256 + lowByte);

            //插入历史数据
            HistoryDataDAL.SingleInsertHistoryData(probeSerialPortInfo_, decValue);
            //更新实时数据
            RealtimeDataDAL.EditRealtimeDataByID(probeSerialPortInfo_.ProbeID, DateTime.Now, decValue);

            if (Form1.gameConfig_.isLog)
            {
                string log = "解析【Type=" + machineSerialPortInfo_.MachineType + "-地址=" + machineSerialPortInfo_.MachineAddress + "】主机数据：probeID=" + probeSerialPortInfo_.ProbeID + " probeName=" + probeSerialPortInfo_.ProbeName + " index = " + index + " highByte = " + highByte + " lowByte=" + lowByte + " decvalue=" + decValue;
                GasCheckAlarmSystem.Form1.instance.AddLog(log);
            }
            return true;
        }

        public int readProbeIndex_ = 0;
        ProbeSerialPortInfo probeSerialPortInfo_;
        public override void HandleSendData(MachineSerialPortInfo machineSerialPortInfo, out string sendContent)
        {
            machineSerialPortInfo_ = machineSerialPortInfo;
            GasCheckAlarmSystem.Form1.instance.AddLog("标准协议发送进度:" + (readProbeIndex_ + 1) + "/" + machineSerialPortInfo_.list.Count);
            if (readProbeIndex_ >= machineSerialPortInfo_.list.Count)
            {
                sendContent = string.Empty;
                return;
            }
            probeSerialPortInfo_ = machineSerialPortInfo_.list[readProbeIndex_];
            string firstHex = probeSerialPortInfo_.ProbeAddress.Substring(2, 2);
            string endHex = "000001";

            StringBuilder sb = new StringBuilder();
            sb.Append(machineSerialPortInfo_.MachineAddress).Append(machineSerialPortInfo_.command).Append(firstHex).Append(endHex);
            sendContent = HexHelper.GetTxtSendText(sb.ToString());
            readAllByteLength_ = 3 + 2 + 2;
            tempReadAllByteLength_ = 0;
            stageByteList_.Clear();
            readProbeIndex_++;

            if (Form1.gameConfig_.isLog)
            {
                sb = new StringBuilder("SendData:").Append(sendContent);
                StringBuilder sb2 = new StringBuilder();
                for (int i = 0; i < machineSerialPortInfo_.list.Count; i++)
                {
                    sb2.Append("   " + machineSerialPortInfo_.list[i].ProbeID);
                }
                GasCheckAlarmSystem.Form1.instance.AddLog(sb.ToString() + " readAllByteLength:" + readAllByteLength_ + " probeList:" + sb2.ToString());
            }
        }

        public override bool IsHandleOver()
        {
            return readProbeIndex_ >= machineSerialPortInfo_.list.Count;
        }
    }
}
