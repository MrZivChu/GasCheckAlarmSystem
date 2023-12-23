using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    class KB9000Protocol : ProtocolBase
    {
        public override string GetProtocolName()
        {
            return "KB9000协议";
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
            if (machineSerialPortInfo_.list.Count > 0)
            {
                //数据解析处理
                ProbeSerialPortInfo probeSerialPortInfo = machineSerialPortInfo_.list[0];

                int densityIndex = 8;//浓度低位索引
                int pointIndex = 12;//小数点低位索引
                int stateIndex = 14;//状态低位索引
                if (stageByteList_.Count <= stateIndex)
                    return true;

                byte stateHighByte = stageByteList_[stateIndex];
                byte stateLowByte = stageByteList_[stateIndex + 1];
                int stateValue = (stateHighByte * 256 + stateLowByte);
                if (stateValue != 1 || stateValue != 5 || stateValue != 6)
                {
                    return true;
                }

                byte densityHighByte = stageByteList_[densityIndex];
                byte densityLowByte = stageByteList_[densityIndex + 1];
                float densityValue = (densityHighByte * 256 + densityLowByte);

                byte pointHighByte = stageByteList_[pointIndex];
                byte pointLowByte = stageByteList_[pointIndex + 1];
                int pointValue = Convert.ToInt32(pointHighByte * 256 + pointLowByte);

                float value = densityValue / Convert.ToSingle(Math.Pow(10, pointValue));

                //插入历史数据
                HistoryDataDAL.AddHistoryData(probeSerialPortInfo.ProbeID, value, probeSerialPortInfo.MachineID);
                //更新实时数据
                ProbeDAL.EditRealtimeDataByID(probeSerialPortInfo.ProbeID, DateTime.Now, value);
            }
            return true;
        }

        public override void HandleSendData(MachineSerialPortInfo machineSerialPortInfo, out string sendContent)
        {
            machineSerialPortInfo_ = machineSerialPortInfo;
            int firstDec = Convert.ToInt32(machineSerialPortInfo_.FirstProbeDecAddress);
            string firstHex = firstDec.ToString("X4");

            StringBuilder sb = new StringBuilder();
            sb.Append(Convert.ToInt32(machineSerialPortInfo_.MachineAddress).ToString("X2")).Append(machineSerialPortInfo_.command).Append(firstHex).Append("0006");
            sendContent = HexHelper.GetTxtSendText(sb.ToString());
            readAllByteLength_ = 3 + 2 * 6 + 2;
            tempReadAllByteLength_ = 0;
            stageByteList_.Clear();

            LogHelper.AddLog("发送数据:{0} 应该读取的数据总量:{1}", sendContent, readAllByteLength_);
        }
    }
}
