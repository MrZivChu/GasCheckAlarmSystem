using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    class HaiWanProtocol : ProtocolBase
    {
        public override string GetProtocolName()
        {
            return "海湾协议";
        }

        public override bool HandleReceiveData(byte[] buffer, int bytesToReadCount)
        {
            if (machineSerialPortInfo_ == null || bytesToReadCount == 0)
            {
                return false;
            }
            tempReadAllByteLength_ += bytesToReadCount;
            stageByteList_.AddRange(buffer);
            LogHelper.AddLog("本次接收数据量:{0} 数据接收进度:{1}/{2}", bytesToReadCount, tempReadAllByteLength_, readAllByteLength_);
            if (tempReadAllByteLength_ < readAllByteLength_)
            {
                return false;
            }
            if (Convert.ToInt32(stageByteList_[0].ToString()) != Convert.ToInt32(machineSerialPortInfo_.MachineAddress))
            {
                LogHelper.AddLog("数据校验没通过");
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
                HistoryDataDAL.AddHistoryData(probeSerialPortInfo.ProbeID, decValue, probeSerialPortInfo.MachineID);
                //更新实时数据
                ProbeDAL.EditRealtimeDataByID(probeSerialPortInfo.ProbeID, DateTime.Now, decValue);
                LogHelper.AddLog("probeID:{0} highByte:{1} lowByte:{2} decvalue:{3}", probeSerialPortInfo.ProbeID, highByte, lowByte, decValue);
            }
            return true;
        }

        public override void HandleSendData(MachineSerialPortInfo machineSerialPortInfo, out string sendContent)
        {
            machineSerialPortInfo_ = machineSerialPortInfo;
            int firstDec = Convert.ToInt32(machineSerialPortInfo_.FirstProbeDecAddress) - 1;
            int endDec = Convert.ToInt32(machineSerialPortInfo_.EndProbeDecAddress);
            string firstHex = firstDec.ToString("X4");
            string endHex = endDec.ToString("X4");

            StringBuilder sb = new StringBuilder();
            sb.Append(machineSerialPortInfo.MachineAddress).Append(machineSerialPortInfo.command).Append(firstHex).Append(endHex);
            sendContent = HexHelper.GetTxtSendText(sb.ToString());
            readAllByteLength_ = 3 + 2 * endDec + 2;
            tempReadAllByteLength_ = 0;
            stageByteList_.Clear();
            
            LogHelper.AddLog("SendData:{0} readAllByteLength:{1} FirstProbeDecAddress:{2} EndProbeDecAddress:{3}", sendContent, readAllByteLength_, machineSerialPortInfo_.FirstProbeDecAddress, machineSerialPortInfo_.EndProbeDecAddress);
        }
    }
}
