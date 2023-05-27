using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

public class EngineEventTrigger
{
    [PostProcessBuild(1)]
    public static void AfterBuild(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log("打包成功，输出平台: " + target + "，输出路径: " + pathToBuiltProject);

        int lastIndex = pathToBuiltProject.LastIndexOf('/');
        string targetDir = pathToBuiltProject.Substring(0, lastIndex);
        CopyDirectory(Application.streamingAssetsPath + "/../Plugins/Camera/HCNetSDKCom", targetDir + "/GasCheckAlarmSystem_Data/Plugins/HCNetSDKCom");
    }

    public static void CopyDirectory(string srcDir, string targetDir)
    {
        DirectoryInfo source = new DirectoryInfo(srcDir);
        DirectoryInfo target = new DirectoryInfo(targetDir);

        if (!source.Exists)
        {
            Debug.LogError(source.FullName + "目录不存在");
            return;
        }
        if (!target.Exists)
        {
            target.Create();
            Debug.Log("创建目标目录：" + targetDir);
        }
        FileInfo[] files = source.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
        }
        DirectoryInfo[] dirs = source.GetDirectories();
        for (int j = 0; j < dirs.Length; j++)
        {
            CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
        }
        Debug.Log("拷贝文件夹：" + srcDir + "到" + targetDir + "成功");
    }
}
