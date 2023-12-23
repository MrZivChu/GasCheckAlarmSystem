using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    class WeiTaiProtocol : ProtocolBase
    {
        public override string GetProtocolName()
        {
            return "惟泰协议";
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
            if (ConfigHandleHelper.GetConfig().isLog)
            {
                string content = string.Empty;
                stageByteList_.ForEach(it => content += it + "-");
                LogHelper.AddLog("接收数据:{0}", content);
            }
            for (int j = 0; j < machineSerialPortInfo_.list.Count; j++)
            {
                //数据解析处理
                ProbeSerialPortInfo probeSerialPortInfo = machineSerialPortInfo_.list[j];

                int index = Convert.ToInt32(probeSerialPortInfo.ProbeAddress) * 2 + 1;
                if (stageByteList_.Count <= index + 1)
                    continue;

                byte highByte = stageByteList_[index];
                byte lowByte = stageByteList_[index + 1];
                float decValue = (highByte * 256 + lowByte) / 10;

                //插入历史数据
                HistoryDataDAL.AddHistoryData(probeSerialPortInfo.ProbeID, decValue, probeSerialPortInfo.MachineID);
                //更新实时数据
                ProbeDAL.EditRealtimeDataByID(probeSerialPortInfo.ProbeID, DateTime.Now, decValue);
            }
            return true;
        }

        public override void HandleSendData(MachineSerialPortInfo machineSerialPortInfo, out string sendContent)
        {
            machineSerialPortInfo_ = machineSerialPortInfo;
            int firstDec = Convert.ToInt32(machineSerialPortInfo_.FirstProbeDecAddress);
            int endDec = Convert.ToInt32(machineSerialPortInfo_.EndProbeDecAddress);
            string firstHex = "0003";
            string endHex = endDec.ToString("X4");

            int machineDec = Convert.ToInt32(machineSerialPortInfo_.MachineAddress);
            string machineHex = machineDec.ToString("X2");

            StringBuilder sb = new StringBuilder();
            sb.Append(machineHex).Append(machineSerialPortInfo_.command).Append(firstHex).Append(endHex);
            sendContent = HexHelper.GetTxtSendText(sb.ToString());
            readAllByteLength_ = 3 + 2 * endDec + 2;
            tempReadAllByteLength_ = 0;
            stageByteList_.Clear();

            LogHelper.AddLog("发送数据:{0} 应该读取的数据总量:{1}", sendContent, readAllByteLength_);
        }
    }
}
