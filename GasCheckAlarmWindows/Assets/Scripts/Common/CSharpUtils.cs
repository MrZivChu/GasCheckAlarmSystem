using SharpCompress.Archive.Zip;
using SharpCompress.Reader.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CSharpUtils
{
    #region 字符串AES加密解密
    //加密
    public static string Encrypt(string toEncrypt, string salt)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(salt);
        SHA256 sha256 = new SHA256Managed();
        keyArray = sha256.ComputeHash(keyArray);

        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
        byte[] ivArray = UTF8Encoding.UTF8.GetBytes("writedbyMrzivchu");

        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.IV = ivArray;
        rDel.Mode = CipherMode.CBC;
        rDel.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = rDel.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return System.Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    //解密
    public static string Decrypt(string toDecrypt, string salt)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(salt);
        SHA256 sha256 = new SHA256Managed();
        keyArray = sha256.ComputeHash(keyArray);

        byte[] ivArray = UTF8Encoding.UTF8.GetBytes("writedbyMrzivchu");
        byte[] toEncryptArray = System.Convert.FromBase64String(toDecrypt);
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.IV = ivArray;
        rDel.Mode = CipherMode.CBC;
        rDel.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = rDel.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return UTF8Encoding.UTF8.GetString(resultArray);
    }
    #endregion

    #region 字符串编码
    /// <summary>
    /// Base64编码
    /// </summary>
    public static string Encode(string message)
    {
        byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(message);
        return System.Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Base64解码
    /// </summary>
    public static string Decode(string message)
    {
        byte[] bytes = System.Convert.FromBase64String(message);
        return Encoding.GetEncoding("utf-8").GetString(bytes);
    }
    #endregion

    #region 计算MD5值
    /// <summary>
    /// 计算字符串的MD5值
    /// 加密现时最流行也是据说最安全的算法是MD5算法，MD5是一种不可逆的算法，也就是 明文经过加密后，根据加密过的密文无法还原出明文来。
    /// 网站的密码就可以用这个加密
    /// </summary>
    public static string StringMD5(string source)
    {
        byte[] Bytes = Encoding.UTF8.GetBytes(source);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] result = md5.ComputeHash(Bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
                builder.Append(result[i].ToString("x2"));
            return builder.ToString();
        }
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string FileMD5(string file)
    {
        try
        {
            if (File.Exists(file))
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            return "";
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("FileMD5 fail, error:" + ex.Message);
        }
    }
    #endregion


    #region 解压缩文件或者文件夹
    /// <summary>
    /// 压缩文件
    /// </summary>
    /// <param name="zipFile">生成的压缩文件</param>
    /// <param name="sourcefile">要被压缩的源文件</param>
    /// <param name="fileName">解压后的文件名称，如果文件名是这样的：/gui/comm.ab，那么会生成gui这个文件夹，并在此文件夹下解压文件，并名为comm.ab</param>
    /// <returns></returns>
    public static long compressFile(string zipFile, string sourcefile, string fileName)
    {
        long size = 0;
        using (Stream s = File.OpenWrite(zipFile))
        {
            using (var ws = SharpCompress.Writer.WriterFactory.Open(s, SharpCompress.Common.ArchiveType.Zip, SharpCompress.Common.CompressionType.Deflate))
            {
                ws.Write(fileName, File.OpenRead(sourcefile), null);
            }
            size = s.Length;
        }
        return size;
    }

    public static bool UncompressMemory(string rootFloder, byte[] bytes)
    {
        using (var ms = new MemoryStream(bytes))
        {
            using (var ar = SharpCompress.Archive.ArchiveFactory.Open(ms))
            {
                foreach (var item in ar.Entries)
                {
                    if (!item.IsDirectory)
                    {
                        string file = rootFloder + item.FilePath;
                        string dir = Path.GetDirectoryName(file);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                        using (FileStream fs = new FileStream(file, FileMode.Create))
                        {
                            item.WriteTo(fs);
                        }
                    }
                }
            }
        }
        return true;
    }

    public static long compressFiles(string zipFile, List<string> files, int startPos, string profix)
    {
        long size = 0;
        using (Stream s = File.OpenWrite(zipFile))
        {
            using (var ws = SharpCompress.Writer.WriterFactory.Open(s, SharpCompress.Common.ArchiveType.Zip, SharpCompress.Common.CompressionType.Deflate))
            {
                foreach (var item in files)
                {
                    ws.Write(profix + item.Substring(startPos), File.OpenRead(item), null);
                }
            }
            size = s.Length;
        }
        return size;
    }
    #endregion

    public static T[] MergerArray<T>(T[] First, T[] Second)
    {
        T[] result = new T[First.Length + Second.Length];
        First.CopyTo(result, 0);
        Second.CopyTo(result, First.Length);
        return result;
    }


    /// <summary>
    /// 杀掉FoxitReader进程
    /// </summary>
    /// <param name="strProcessesByName"></param>
    public static void KillProcess(string processName)
    {
        foreach (Process p in Process.GetProcesses())
        {
            if (!p.HasExited && p.ProcessName.Contains(processName))
            {
                try
                {
                    p.Kill();
                    p.WaitForExit(); // possibly with a timeout
                    UnityEngine.Debug.Log("已杀掉进程！！！" + processName);
                }
                catch (Win32Exception e)
                {
                    UnityEngine.Debug.Log("杀进程出错:" + e.Message.ToString());
                }
                catch (InvalidOperationException e)
                {
                    UnityEngine.Debug.Log("杀进程出错:" + e.Message.ToString());
                }
            }

        }
    }
}
