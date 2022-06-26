using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    //"标1协议","DZ-40-New","DZ-40-Old","标准协议","海湾"
    abstract class ProtocolBase
    {
        protected MachineSerialPortInfo machineSerialPortInfo_ = null;
        protected int baudRate_ = 4800;

        protected int tempReadAllByteLength_ = 0;
        protected int readAllByteLength_ = 0;
        protected List<byte> stageByteList_ = new List<byte>();

        public abstract string GetProtocolName();
        public abstract void HandleSendData(MachineSerialPortInfo machineSerialPortInfo, out string sendContent);
        public abstract bool HandleReceiveData(byte[] buffer,int bytesToReadCount);

        public virtual bool IsHandleOver()
        {
            return true;
        }
    }
}
