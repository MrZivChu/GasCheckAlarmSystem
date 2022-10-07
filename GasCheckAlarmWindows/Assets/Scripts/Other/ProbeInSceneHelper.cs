using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ProbeInSceneHelper : UIEventHelper
{
    public AddProbePanel addProbePanel;
    public UpdatePosDirPanel updatePosDirPanel;
    public ProbeInfo3DPanel probeInfo3DPanel;
    public Transform probeListRoot;
    public Transform errorProbeListRoot;
    public UnityEngine.Object probeResObj;
    public UnityEngine.Object mainRealtimeDataItemResObj;

    private void Start()
    {
        LoadAllProbe();
        EventManager.Instance.AddEventListener(NotifyType.UpdateProbeList, UpdateProbeListEvent);
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeDataListEvent);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateProbeList, UpdateProbeListEvent);
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeDataListEvent);
    }

    void UpdateProbeListEvent(object data)
    {
        LoadAllProbe();
    }

    float doublePreTime = 0;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool isHasHandle = RaycastHitProbe();
            if (!isHasHandle && FormatData.currentUser.Authority == EAuthority.Admin)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    float doubleNowTime = Time.realtimeSinceStartup;
                    if (doubleNowTime - doublePreTime < 0.3f)
                    {
                        ShowOperateProbePanel();
                    }
                    doublePreTime = doubleNowTime;
                }
            }
        }
    }

    bool RaycastHitProbe()
    {
        RaycastHit hit;
        if (RayHit(out hit))
        {
            ProbeInfo probeInfo = hit.collider.GetComponent<ProbeInfo>();
            if (probeInfo != null)
            {
                probeInfo3DPanel.InitInfo(probeInfo.currentModel);
                probeInfo3DPanel.gameObject.SetActive(true);
                return true;
            }
        }
        return false;
    }

    void ShowOperateProbePanel()
    {
        if (addProbePanel != null)
        {
            RaycastHit hit;
            bool isHit = RayHit(out hit);
            if (isHit)
            {
                if (JsonHandleHelper.gameConfig.isEnterPosDir)
                {
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

    public Dictionary<int, ProbeInfo> probeIdInfoDic = new Dictionary<int, ProbeInfo>();
    public Dictionary<int, ProbeModel> probeIdProbeModelDic = new Dictionary<int, ProbeModel>();
    public void LoadAllProbe()
    {
        probeIdInfoDic.Clear();
        probeIdProbeModelDic.Clear();
        foreach (Transform item in probeListRoot)
        {
            Destroy(item.gameObject);
        }
        List<ProbeModel> list = ProbeDAL.SelectIDProbeNameGasKindPos3D();
        list.ForEach(it =>
        {
            SpawnProbe(it);
        });
    }

    public void SpawnProbe(ProbeModel model)
    {
        Vector3 position = Vector3.zero;
        Vector3 direction = Vector3.zero;
        string posDir = model.Pos3D;
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
        probeObj.transform.parent = probeListRoot;
        probeObj.transform.LookAt(position + (-direction * 3));
        ProbeInfo info = probeObj.GetComponent<ProbeInfo>();
        info.InitInfo(model);
        probeIdInfoDic.Add(model.ID, info);
        probeIdProbeModelDic[model.ID] = model;
    }

    public void UpdateRealtimeDataListEvent(object data)
    {
        if (!gameObject || !gameObject.activeSelf)
        {
            return;
        }
        List<ProbeModel> realtimeEventData = (List<ProbeModel>)data;
        realtimeEventData.ForEach(it => { it.ProbeName = probeIdProbeModelDic[it.ID].ProbeName; });
        Update3DProbeList(realtimeEventData);
        Update2DProbeList(realtimeEventData);
    }

    void Update3DProbeList(List<ProbeModel> list)
    {
        foreach (var model in list)
        {
            if (probeIdInfoDic.ContainsKey(model.ID))
            {
                ProbeInfo probeInfo = probeIdInfoDic[model.ID];
                probeInfo.currentModel.GasValue = model.GasValue;
                GameUtils.SetObjectHighLight(probeInfo.gameObject, true, FormatData.warningColorDic[model.warningLevel], Color.white);
            }
        }
    }

    void Update2DProbeList(List<ProbeModel> list)
    {
        GameUtils.SpawnCellForTable<ProbeModel>(errorProbeListRoot, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(mainRealtimeDataItemResObj) as GameObject;
                currentObj.transform.SetParent(errorProbeListRoot);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            MainRealtimeDataItem item = currentObj.GetComponent<MainRealtimeDataItem>();
            item.InitData(data, probeIdInfoDic[data.ID].gameObject);
            item.SetBackgroundColor(FormatData.warningColorDic[data.warningLevel]);
            currentObj.SetActive(true);
        });
    }
}
