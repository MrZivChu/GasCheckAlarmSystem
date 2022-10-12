using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    abstract class ProtocolBase
    {
        protected MachineSerialPortInfo machineSerialPortInfo_ = null;

        protected int tempReadAllByteLength_ = 0;
        protected int readAllByteLength_ = 0;
        protected List<byte> stageByteList_ = new List<byte>();

        public abstract string GetProtocolName();
        public abstract void HandleSendData(MachineSerialPortInfo machineSerialPortInfo, out string sendContent);
        public abstract bool HandleReceiveData(byte[] buffer, int bytesToReadCount);

        public virtual bool IsHandleOver()
        {
            return true;
        }
    }
}
