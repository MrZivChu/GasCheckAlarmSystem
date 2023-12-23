using GasCheckAlarmSystem;
using System.Collections.Generic;

//读1号主机1号到3号气体浓度
//上位机发送：  
//    01       03        0001              0003            540B
// 主机地址    命令   探测器起始地址     读取寄存器数量      CRC校验

//上位接收数据：　
//    01 　　  03 　　 0A 　　　     0004            0000　　          0000         1676　　   
// 主机地址   命令   字节数量   一号探测器浓度   二号探测器浓度　   三号探测器浓度    CRC校验

public class SerialPortHelper
{
    List<SerialPortRunner> runnerList_ = null;
    public void Run()
    {
        Dictionary<string, List<MachineSerialPortInfo>> baseInfo_ = MachineDAL.SelectAllMachineSerialPortInfo();
        if (baseInfo_ != null && baseInfo_.Count > 0)
        {
            runnerList_ = new List<SerialPortRunner>();
            foreach (var item in baseInfo_)
            {
                SerialPortRunner runner = new SerialPortRunner();
                runner.Run(item.Key, item.Value);
                runnerList_.Add(runner);
            }
        }
    }

    public void FixedUpdate(float intervalTime)
    {
        if (runnerList_ != null && runnerList_.Count > 0)
        {
            runnerList_.ForEach(it =>
            {
                if (it != null)
                {
                    it.FixedUpdate(intervalTime);
                }
            });
        }
    }
}
