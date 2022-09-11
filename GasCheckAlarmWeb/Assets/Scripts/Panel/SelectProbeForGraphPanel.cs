using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class SelectProbeForGraphPanel : UIEventHelper
{
    ProbeModel probeModel;
    public Transform contentTrans;
    public GameObject itemRes;
    ToggleGroup togGroup;

    public Button btn_cancel;
    public Button btn_ok;
    [HideInInspector]
    public Vector3 uiPos;
    void Awake()
    {
        togGroup = itemRes.GetComponent<ToggleGroup>();
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_ok, OnOk);
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnOk(Button btn)
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "EditRealtimePos2DByID");
        form.AddField("id", probeModel.ID);
        form.AddField("pos2D", uiPos.x + "," + uiPos.y);
        GameUtils.PostHttpWebRequest("RealtimeData.ashx", form, null, null);

        MessageBox.Instance.PopOK("新增成功", () =>
        {
            gameObject.SetActive(false);
        }, "确定");
    }


    void OnEnable()
    {
        InitData();
    }

    private void InitData()
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllProbeByCondition");
        GameUtils.PostHttpWebRequest("Probe.ashx", form, (result) =>
        {
            string content = Encoding.UTF8.GetString(result);
            List<ProbeModel> list = JsonMapper.ToObject<List<ProbeModel>>(content);
            list = list.FindAll((item) =>
            {
                return string.IsNullOrEmpty(item.Pos2D) ? true : false;
            });
            InitGrid(list);
        }, null);
    }

    void InitGrid(List<ProbeModel> list)
    {
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
            SelectProbeForGraphItem item = currentObj.GetComponent<SelectProbeForGraphItem>();
            item.InitData(data);
            Toggle tog = currentObj.GetComponent<Toggle>();
            tog.isOn = false;
            tog.group = togGroup;
            RegisterTogClick<SelectProbeForGraphItem>(tog, item, OnTogSelected);
            currentObj.SetActive(true);
        });
    }

    void OnTogSelected(Toggle tog, bool isOn, SelectProbeForGraphItem data)
    {
        if (isOn)
        {
            probeModel = data.probeModel;
        }
    }
}
