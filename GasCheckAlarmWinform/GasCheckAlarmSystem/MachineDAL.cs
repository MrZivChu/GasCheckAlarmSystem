using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class ProbeSerialPortInfo
{
    public int ProbeID;
    public string ProbeAddress;
    public int MachineID;
}
public class MachineSerialPortInfo
{
    public string MachineAddress;
    public EProtocolType ProtocolType;
    public string command = "03";
    public string FirstProbeDecAddress;
    public string EndProbeDecAddress;
    public int BaudRate;
    public List<ProbeSerialPortInfo> list;
}

public class MachineDAL
{
    public static Dictionary<string, List<MachineSerialPortInfo>> SelectAllMachineSerialPortInfo()
    {
        Dictionary<string, List<MachineSerialPortInfo>> dic = new Dictionary<string, List<MachineSerialPortInfo>>();
        string sql = @"select ID,MailAddress,MachineID from Probe";
        List<ProbeSerialPortInfo> probeList = new List<ProbeSerialPortInfo>();
        DataTable dt = SqlHelper.ExecuteDataTable(sql, null);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            ProbeSerialPortInfo model = new ProbeSerialPortInfo();
            model.ProbeID = Convert.ToInt32(dt.Rows[i]["ID"].ToString());
            model.ProbeAddress = dt.Rows[i]["MailAddress"].ToString();
            model.MachineID = Convert.ToInt32(dt.Rows[i]["MachineID"]);
            probeList.Add(model);
        }

        sql = @"select ID,MailAddress,ProtocolType,BaudRate,PortName from Machine";
        dt = SqlHelper.ExecuteDataTable(sql, null);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string portName = dt.Rows[i]["PortName"].ToString();
            if (string.IsNullOrEmpty(portName))
            {
                continue;
            }
            int machineID = Convert.ToInt32(dt.Rows[i]["ID"].ToString());
            List<ProbeSerialPortInfo> list = probeList.FindAll(it => { return it.MachineID == machineID; });
            if (list != null && list.Count > 0)
            {
                int firstProbeDecAddress = Convert.ToInt32(list[0].ProbeAddress);
                int endProbeDecAddress = list.Count;
                if (list.Count > 1)
                {
                    for (int j = 1; j < list.Count; j++)
                    {
                        if (Convert.ToInt32(list[j].ProbeAddress) > endProbeDecAddress)
                        {
                            endProbeDecAddress = Convert.ToInt32(list[j].ProbeAddress);
                        }
                        else if (Convert.ToInt32(list[j].ProbeAddress) < firstProbeDecAddress)
                        {
                            firstProbeDecAddress = Convert.ToInt32(list[j].ProbeAddress);
                        }
                    }
                }
                MachineSerialPortInfo machineSerialPortInfo = new MachineSerialPortInfo();
                machineSerialPortInfo.ProtocolType = (EProtocolType)dt.Rows[i]["ProtocolType"];
                machineSerialPortInfo.MachineAddress = dt.Rows[i]["MailAddress"].ToString();
                machineSerialPortInfo.BaudRate = Convert.ToInt32(dt.Rows[i]["BaudRate"]);
                machineSerialPortInfo.list = list;
                machineSerialPortInfo.FirstProbeDecAddress = firstProbeDecAddress.ToString();
                machineSerialPortInfo.EndProbeDecAddress = endProbeDecAddress.ToString();
                if (!dic.ContainsKey(portName))
                {
                    dic[portName] = new List<MachineSerialPortInfo>();
                }
                dic[portName].Add(machineSerialPortInfo);
            }
        }
        return dic;
    }
}
