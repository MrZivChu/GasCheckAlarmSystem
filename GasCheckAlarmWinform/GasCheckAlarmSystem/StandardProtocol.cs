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
            //数据解析处理
            int index = 1 * 2 + 1;
            if (stageByteList_.Count <= index + 1)
                return false;

            byte highByte = stageByteList_[index];
            byte lowByte = stageByteList_[index + 1];
            float decValue = (highByte * 256 + lowByte);

            //插入历史数据
            HistoryDataDAL.AddHistoryData(probeSerialPortInfo_.ProbeID, decValue, probeSerialPortInfo_.MachineID);
            //更新实时数据
            ProbeDAL.EditRealtimeDataByID(probeSerialPortInfo_.ProbeID, DateTime.Now, decValue);
            return true;
        }

        public int readProbeIndex_ = 0;
        ProbeSerialPortInfo probeSerialPortInfo_;
        public override void HandleSendData(MachineSerialPortInfo machineSerialPortInfo, out string sendContent)
        {
            machineSerialPortInfo_ = machineSerialPortInfo;
            LogHelper.AddLog("标准协议发送进度:{0}/{1}", (readProbeIndex_ + 1), machineSerialPortInfo_.list.Count);
            if (readProbeIndex_ >= machineSerialPortInfo_.list.Count)
            {
                sendContent = string.Empty;
                return;
            }
            probeSerialPortInfo_ = machineSerialPortInfo_.list[readProbeIndex_];
            string firstHex = Convert.ToInt32(probeSerialPortInfo_.ProbeAddress.Substring(2, 2)).ToString("X2");
            string endHex = "000001";

            StringBuilder sb = new StringBuilder();
            sb.Append(Convert.ToInt32(machineSerialPortInfo_.MachineAddress).ToString("X2")).Append(machineSerialPortInfo_.command).Append(firstHex).Append(endHex);
            sendContent = HexHelper.GetTxtSendText(sb.ToString());
            readAllByteLength_ = 3 + 2 * 1 + 2;
            tempReadAllByteLength_ = 0;
            stageByteList_.Clear();
            readProbeIndex_++;

            LogHelper.AddLog("发送数据:{0} 应该读取的数据总量:{1}", sendContent, readAllByteLength_);
        }

        public override bool IsHandleOver()
        {
            return readProbeIndex_ >= machineSerialPortInfo_.list.Count;
        }
    }
}
