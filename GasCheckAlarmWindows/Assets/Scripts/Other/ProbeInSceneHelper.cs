using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProbeInSceneHelper : UIEventHelper
{
    public AddProbePanel addProbePanel;
    UnityEngine.Object probeResObj;
    UnityEngine.Object probeInfo3DPanelResObj;

    public static ProbeInSceneHelper instance;
    private void Awake()
    {
        instance = this;
        probeResObj = Resources.Load("Probe");
        probeInfo3DPanelResObj = Resources.Load("ProbeInfo3DPanel");
    }

    float doublePreTime = 0;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                float doubleNowTime = Time.realtimeSinceStartup;
                if (doubleNowTime - doublePreTime < 0.3f)
                {
                    ShowAddProbePanel();
                }
                doublePreTime = doubleNowTime;
            }
        }
    }

    public UpdatePosDirPanel updatePosDirPanel;
    void ShowAddProbePanel()
    {
        if (addProbePanel != null)
        {
            RaycastHit hit;
            bool isHit = RayHit(out hit);
            if (isHit)
            {
                if (JsonHandleHelper.gameConfig.isEnterPosDir)
                {
                    GameObject probeObj = Instantiate(probeResObj) as GameObject;
                    probeObj.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    probeObj.transform.LookAt(hit.point + (-hit.normal * 3));
                    string posDir = hit.point.x.ToString("0.00") + "," + hit.point.y.ToString("0.00") + "," + hit.point.z.ToString("0.00") + ";" + hit.normal.x.ToString("0.00") + "," + hit.normal.y.ToString("0.00") + "," + hit.normal.z.ToString("0.00");
                    updatePosDirPanel.posDir = posDir;
                    updatePosDirPanel.gameObject.SetActive(true);
                }
                else
                {
                    addProbePanel.position = hit.point;
                    addProbePanel.direction = hit.normal;
                    addProbePanel.gameObject.SetActive(true);
                }
            }
        }
    }

    bool RayHit(out RaycastHit hit)
    {
        hit = new RaycastHit();
        if (Camera.main)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit);
        }
        return false;
    }

    public Dictionary<int, Dictionary<ProbeInfo, ProbeInfo3DPanel>> probe3DDic = new Dictionary<int, Dictionary<ProbeInfo, ProbeInfo3DPanel>>();
    public void LoadAllProbe()
    {
        List<ProbeModel> list = ProbeDAL.SelectAllProbeByCondition();
        for (int i = 0; i < list.Count; i++)
        {
            ProbeModel model = list[i];
            SpawnProbe(model);
        }
    }

    public void DeleteProbe(ProbeModel model)
    {
        if (probe3DDic.ContainsKey(model.ID))
        {
            Dictionary<ProbeInfo, ProbeInfo3DPanel> value = probe3DDic[model.ID];
            foreach (var item in value)
            {
                Destroy(item.Key.gameObject);
                Destroy(item.Value.gameObject);
            }
            probe3DDic.Remove(model.ID);
        }
    }

    public void RefreshProbeStatus(RealtimeDataModel model)
    {
        if (probe3DDic.ContainsKey(model.ProbeID))
        {
            Dictionary<ProbeInfo, ProbeInfo3DPanel> value = probe3DDic[model.ProbeID];
            foreach (var item in value)
            {
                item.Value.RefreshRealtimeData(model.GasValue);
                if (model.GasValue > model.SecondAlarmValue)
                {
                    GameUtils.SetObjectHighLight(item.Key.gameObject, true, Color.red, Color.white);
                }
                else if (model.GasValue > model.FirstAlarmValue)
                {
                    GameUtils.SetObjectHighLight(item.Key.gameObject, true, Color.yellow, Color.white);
                }
                else
                {
                    GameUtils.SetObjectHighLight(item.Key.gameObject, true, Color.white, Color.white);
                }
            }
        }
    }

    public void SpawnProbe(ProbeModel model)
    {
        Vector3 position = Vector3.zero;
        Vector3 direction = Vector3.zero;

        string posDir = model.PosDir;
        if (!string.IsNullOrEmpty(posDir))
        {
            string[] temp = posDir.Split(new string[] { ";" }, System.StringSplitOptions.None);
            if (temp.Length >= 2)
            {
                string[] pos = temp[0].Split(new string[] { "," }, System.StringSplitOptions.None);
                string[] dir = temp[1].Split(new string[] { "," }, System.StringSplitOptions.None);
                if (pos.Length >= 3 && dir.Length >= 3)
                {
                    position = new Vector3(Convert.ToSingle(pos[0]), Convert.ToSingle(pos[1]), Convert.ToSingle(pos[2]));
                    direction = new Vector3(Convert.ToSingle(dir[0]), Convert.ToSingle(dir[1]), Convert.ToSingle(dir[2]));
                }
            }
        }

        GameObject probeObj = Instantiate(probeResObj) as GameObject;
        probeObj.transform.position = position;
        probeObj.transform.LookAt(position + (-direction * 3));
        ProbeInfo info = probeObj.GetComponent<ProbeInfo>();
        info.InitInfo(model);

        GameObject threeDCanvasRoot = GameObject.Find("3DCanvas");
        GameObject probe3DPanelObj = Instantiate(probeInfo3DPanelResObj) as GameObject;
        probe3DPanelObj.transform.SetParent(threeDCanvasRoot.transform);
        probe3DPanelObj.transform.localScale = new Vector3(0.004f, 0.004f, 1f);
        probe3DPanelObj.transform.position = new Vector3(position.x + 2.0f, position.y, position.z - 1.0f);
        ProbeInfo3DPanel panel = probe3DPanelObj.GetComponent<ProbeInfo3DPanel>();
        panel.InitInfo(model);

        probe3DDic.Add(model.ID, new Dictionary<ProbeInfo, ProbeInfo3DPanel>() { { info, panel } });
    }
}
