using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    class StandardOneProtocol : ProtocolBase
    {
        public override string GetProtocolName()
        {
            return "标1协议";
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
            for (int j = 0; j < machineSerialPortInfo_.list.Count; j++)
            {
                //数据解析处理
                ProbeSerialPortInfo probeSerialPortInfo = machineSerialPortInfo_.list[j];

                int index = Convert.ToInt32(probeSerialPortInfo.ProbeAddress) * 2 + 1;
                if (index + 1 >= stageByteList_.Count)
                    continue;

                byte highByte = stageByteList_[index];
                byte lowByte = stageByteList_[index + 1];
                float decValue = (highByte * 256 + lowByte);

                //插入历史数据
                HistoryDataDAL.SingleInsertHistoryData(probeSerialPortInfo, decValue);
                //更新实时数据
                RealtimeDataDAL.EditRealtimeDataByID(probeSerialPortInfo.ProbeID, DateTime.Now, decValue);

                if (Form1.gameConfig_.isLog)
                {
                    string log = "解析【Type=" + machineSerialPortInfo_.MachineType + "-地址=" + machineSerialPortInfo_.MachineAddress + "】主机数据：probeID=" + probeSerialPortInfo.ProbeID + " probeName=" + probeSerialPortInfo.ProbeName + " index = " + index + " highByte = " + highByte + " lowByte=" + lowByte + " decvalue=" + decValue;
                    GasCheckAlarmSystem.Form1.instance.AddLog(log);
                }
            }
            return true;
        }

        public override void HandleSendData(MachineSerialPortInfo machineSerialPortInfo, out string sendContent)
        {
            machineSerialPortInfo_ = machineSerialPortInfo;
            int firstDec = Convert.ToInt32(machineSerialPortInfo_.FirstProbeDecAddress);
            int endDec = Convert.ToInt32(machineSerialPortInfo_.EndProbeDecAddress);
            string firstHex = firstDec.ToString("X4");
            string endHex = endDec.ToString("X4");

            int machineDec = Convert.ToInt32(machineSerialPortInfo_.MachineAddress);
            string machineHex = machineDec.ToString("X2");

            StringBuilder sb = new StringBuilder();
            sb.Append(machineHex).Append(machineSerialPortInfo_.command).Append(firstHex).Append(endHex);
            sendContent = HexHelper.GetTxtSendText(sb.ToString());
            readAllByteLength_ = 3 + 2 * endDec + 2;
            tempReadAllByteLength_ = 0;
            stageByteList_.Clear();

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
    }
}
