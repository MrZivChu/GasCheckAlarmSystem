using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HistoryFenQu : MonoBehaviour
{
    //删除93天前的数据
    readonly int days = 93;
    //超过此天数，创建分区
    readonly int fenquDays = 7;
    void Start()
    {
        //程序启动执行一次删除历史数据的操作
        HistoryDataDAL.DeleteHistoryDataBeforeDays(days);
        CheckFenQu();
    }

    void CheckFenQu()
    {
        DateTime now = DateTime.Now;
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.ndf");
        if (files.Length > 0)
        {
            string newFileTime = string.Empty;
            foreach (var file in files)
            {
                string fileTime = Path.GetFileNameWithoutExtension(file).Substring(4, 8);
                if (fileTime.Length == 8)
                {
                    int result = 0;
                    if (int.TryParse(fileTime, out result))
                    {
                        if (string.Compare(fileTime, newFileTime) > 0)
                        {
                            newFileTime = fileTime;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(newFileTime))
            {
                DateTime dt = new DateTime(Convert.ToInt32(newFileTime.Substring(0, 4)), Convert.ToInt32(newFileTime.Substring(4, 2)), Convert.ToInt32(newFileTime.Substring(6, 2)));
                if ((now - dt).TotalDays > fenquDays)
                {
                    Debug.Log("创建分区：" + now.ToString("yyyyMMdd") + "=" + newFileTime);
                    HistoryDataDAL.TableFenQu(now.ToString("yyyyMMdd"), Application.persistentDataPath.Replace("/", "\\"), JsonHandleHelper.gameConfig.sqlDatabase);
                }
            }
        }
        else
        {
            Debug.Log("第一次创建分区");
            HistoryDataDAL.TableFenQu(now.ToString("yyyyMMdd"), Application.persistentDataPath.Replace("/", "\\"), JsonHandleHelper.gameConfig.sqlDatabase);
            HistoryDataDAL.RelateTable("HistoryData", "CheckTime");
        }
    }

    float tempDeleteHistoryDataTime = 0;
    float deleteHistoryDataTime = 60 * 60 * 24 * 1;
    void Update()
    {
        tempDeleteHistoryDataTime += Time.deltaTime;
        if (tempDeleteHistoryDataTime >= deleteHistoryDataTime)
        {
            tempDeleteHistoryDataTime = 0;
            HistoryDataDAL.DeleteHistoryDataBeforeDays(days);
            CheckFenQu();
        }
    }
}
