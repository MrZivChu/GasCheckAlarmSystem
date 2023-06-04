using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class HexHelper
{
    /// <summary>
    /// CRC校验,返回添加CRC校验码后的字符串
    /// </summary>
    /// <param name="data">十六进制字符串</param>
    /// <returns>modbus-CRC校验码</returns>
    public static Int16 CRCForModbus(byte[] data)
    {
        //计算并填写CRC校验码
        int crc = 0xffff;
        for (int n = 0; n < data.Length; n++)
        {
            byte i;
            crc = crc ^ data[n];
            for (i = 0; i < 8; i++)
            {
                int TT;
                TT = crc & 1;
                crc = crc >> 1;
                crc = crc & 0x7fff;
                if (TT == 1)
                {
                    crc = crc ^ 0xa001;
                }
                crc = crc & 0xffff;
            }
        }
        crc = ((crc & 0xFF) << 8 | (crc >> 8));//高低字节换位
        return (Int16)crc;
    }

    /// <summary>
    /// 计算校验码并返回带校验码的16进制字符串
    /// </summary>
    /// <param name="data">16进制字符串(不带校验码)</param>
    /// <returns>带校验码的16进制字符串</returns>
    public static string GetTxtSendText(string data)
    {
        //处理数字转换
        string sendnoNull1 = data.Trim();//去掉字符串前后的空格
        string sendnoNull2 = sendnoNull1.Replace(" ", "");//去掉字符串中间的空格
        string sendNOComma = sendnoNull2.Replace(',', ' ');    //去掉英文逗号
        string sendNOComma1 = sendNOComma.Replace('，', ' '); //去掉中文逗号
        string strSendNoComma2 = sendNOComma1.Replace("0x", "");   //去掉0x
        string sendBuf = strSendNoComma2.Replace("0X", "");   //去掉0X

        byte[] crcbuf = StrToHexByte(sendBuf);//将16进制字符串转换成字节
        Int16 int16 = CRCForModbus(crcbuf);
        string crcString = int16.ToString("X4");//获得校验码
        return data + " " + crcString;//返回数据+校验码
    }
    /// <summary>
    /// 计算接收到的数据的校验码
    /// </summary>
    /// <param name="buffer">接收到的字节数组</param>
    /// <returns>16进制的字符串校验码</returns>
    public static string CRCForModbus_receive(byte[] buffer)
    {
        try
        {
            byte[] _lstByte = new byte[buffer.Length - 2];
            for (int i = 0; i < _lstByte.Length; i++)
            {
                _lstByte[i] = buffer[i];
            }
            Int16 _returnValue = CRCForModbus(_lstByte);
            return Convert.ToString(_returnValue, 16);
        }
        catch
        {
            return "0";
        }
    }

    /// <summary>
    /// 将字节数组转换成16进制字符串的方法
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ByteToHexStr(byte[] bytes)
    {
        string returnStr = "";
        if (bytes != null)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                returnStr += bytes[i].ToString("X2");//X2表示16进制显示2位
            }
        }
        return returnStr;
    }

    /// <summary>
    /// 将16进制字符串转换成字节数组的方法
    /// </summary>
    /// <param name="hexString">16进制字符串</param>
    /// <returns>字节数组</returns>
    public static byte[] StrToHexByte(string hexString)
    {
        hexString = hexString.Replace(" ", "");
        if ((hexString.Length % 2) != 0)
            hexString += " ";
        byte[] returnBytes = new byte[hexString.Length / 2];
        for (int i = 0; i < returnBytes.Length; i++)
        {
            returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
        }
        return returnBytes;
    }
}
