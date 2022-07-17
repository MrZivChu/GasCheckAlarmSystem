using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QrCodeSpawnController : UIEventHelper
{
    public Button saveBtn;
    public Text msgText;
    public GameObject itemRes;
    public Transform contentTrans;

    private void Start()
    {
        RegisterBtnClick(saveBtn, OnSaveQrCode);
    }

    void OnSaveQrCode(Button btn)
    {
        using (System.Windows.Forms.FolderBrowserDialog od = new System.Windows.Forms.FolderBrowserDialog())
        {
            od.Description = "请选择二维码保存位置";
            if (od.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string saveFloder = od.SelectedPath;
                SaveByFloder(saveFloder);
            }
        }
    }

    void SaveByFloder(string saveFloder)
    {
        if (childList != null && childList.Count > 0 && !string.IsNullOrEmpty(saveFloder))
        {
            for (int i = 0; i < childList.Count; i++)
            {
                msgText.text = "保存进度：" + ((i + 1) / childList.Count) * 100 + "%";
                string saveFilePath = saveFloder + "\\" + (i + 1) + "-" + childList[i].model.ProbeName + ".jpg";
                Debug.Log("saveFilePath:" + saveFilePath);
                SaveFile(childList[i].GetQrCodeBytes(), saveFilePath);
            }
            msgText.text = "保存成功!";
        }
    }

    void SaveFile(byte[] bytes, string savePath)
    {
        using (FileStream fs2 = new FileStream(savePath, FileMode.Create))
        {
            fs2.Write(bytes, 0, bytes.Length);
        }
    }

    private void OnEnable()
    {
        msgText.text = string.Empty;
        List<ProbeModel> list = ProbeDAL.SelectAllProbeByCondition();
        InitGrid(list);
    }

    List<QrCodeSpawnItem> childList;
    void InitGrid(List<ProbeModel> list)
    {
        childList = new List<QrCodeSpawnItem>();
        GameUtils.SpawnCellForTable<ProbeModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            QrCodeSpawnItem cubeInfoItem = currentObj.GetComponent<QrCodeSpawnItem>();
            cubeInfoItem.InitData(data);
            currentObj.SetActive(true);
            childList.Add(cubeInfoItem);
        }, false);
    }
}
