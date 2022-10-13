using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CopyFileToPath
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> path = new List<string>() {
                "E:/GasCheckAlarmSystem/GasCheckAlarmWindows/Assets/Plugins/",
                "E:/GasCheckAlarmSystem/GasCheckAlarmWeb/Assets/Plugins/",
                "E:/GasCheckAlarmSystem/GasCheckAlarmServer/Bin/",
                "E:/GasCheckAlarmSystem/GasCheckAlarmPhone/Assets/Plugins/"
            };
            string fileName = "SqlLibrary.dll";
            string readFile = @"E:\GasCheckAlarmSystem\SqlLibrary\SqlLibrary\bin\" + fileName;
            for (int i = 0; i < path.Count; i++)
            {
                try
                {
                    Save(readFile, path[i] + "/" + fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
            }
        }

        static void Save(string readFile, string saveFile)
        {
            using (FileStream read = new FileStream(readFile, FileMode.Open))
            {
                using (FileStream write = new FileStream(saveFile, FileMode.Create))
                {
                    int count = 0;
                    byte[] bytes = new byte[1024 * 1024 * 1];
                    while ((count = read.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        write.Write(bytes, 0, count);
                    }
                }
            }
        }
    }
}
