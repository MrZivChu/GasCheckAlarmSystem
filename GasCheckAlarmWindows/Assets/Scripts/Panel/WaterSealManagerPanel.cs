using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WaterSealManagerPanel : UIEventHelper
{
    public Button btn_add;
    public Button btn_delete;
    public Button btn_edit;
    public Toggle wholeToggle;

    public GameObject addWaterSealPanel;
    public GameObject editWaterSealPanel;

    public Transform contentTrans;
    public Object itemRes;
    private void Start()
    {
        RegisterBtnClick(btn_add, OnAddWaterSeal);
        RegisterBtnClick(btn_delete, OnDeleteWaterSeal);
        RegisterBtnClick(btn_edit, OnEditWaterSeal);
        RegisterTogClick(wholeToggle, OnWholeToggle);
        EventManager.Instance.AddEventListener(NotifyType.UpdateWaterSealList, UpdateWaterSealListEvent);
    }

    void UpdateWaterSealListEvent(object data)
    {
        InitData();
    }

    void OnAddWaterSeal(Button btn)
    {
        if (addWaterSealPanel != null)
        {
            addWaterSealPanel.SetActive(true);
        }
    }

    void OnDeleteWaterSeal(Button btn)
    {
        List<WaterSealModel> idList = new List<WaterSealModel>();
        if (waterSealItemList != null && waterSealItemList.Count > 0)
        {
            for (int i = 0; i < waterSealItemList.Count; i++)
            {
                if (waterSealItemList[i].GetToggleStatus())
                {
                    idList.Add(waterSealItemList[i].currentModel);
                }
            }
        }
        if (idList.Count <= 0)
        {
            MessageBox.Instance.PopOK("未选择任何数据", null, "确定");
        }
        else
        {
            MessageBox.Instance.PopYesNo("确认删除？", null, () =>
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < idList.Count; i++)
                {
                    sb.Append(idList[i].ID + ",");
                }
                sb = sb.Remove(sb.Length - 1, 1);
                WWWForm form = new WWWForm();
                form.AddField("requestType", "DeleteWaterSealByID");
                form.AddField("idList", sb.ToString());
                GameUtils.PostHttp("WaterSeal.ashx", form, (content) =>
                {
                    EventManager.Instance.DisPatch(NotifyType.UpdateWaterSealList);
                    MessageBox.Instance.PopOK("删除成功", null, "确定");
                }, null);
            }, "取消", "确定");
        }
    }

    void OnEditWaterSeal(Button btn)
    {
        List<WaterSealItem> selectItemList = new List<WaterSealItem>();
        if (waterSealItemList != null && waterSealItemList.Count > 0)
        {
            for (int i = 0; i < waterSealItemList.Count; i++)
            {
                if (waterSealItemList[i].GetToggleStatus())
                {
                    selectItemList.Add(waterSealItemList[i]);
                }
            }
        }
        if (selectItemList.Count <= 0)
        {
            MessageBox.Instance.PopOK("未选择任何数据", null, "确定");
        }
        else if (selectItemList.Count > 1)
        {
            MessageBox.Instance.PopOK("一次只能选择一条数据", null, "确定");
        }
        else
        {
            WaterSealItem selectItem = selectItemList[0];
            editWaterSealPanel.SetActive(true);
            EditWaterSealPanel panel = editWaterSealPanel.GetComponent<EditWaterSealPanel>();
            panel.InitData(selectItem.currentModel);
        }
    }

    void OnWholeToggle(Toggle tog, bool isOn)
    {
        if (waterSealItemList != null && waterSealItemList.Count > 0)
        {
            for (int i = 0; i < waterSealItemList.Count; i++)
            {
                waterSealItemList[i].SetToggle(isOn);
            }
        }
    }

    private void OnEnable()
    {
        InitData();
    }

    List<WaterSealItem> waterSealItemList = new List<WaterSealItem>();
    private void InitData()
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllWaterSealByCondition");
        GameUtils.PostHttp("WaterSeal.ashx", form, (result) =>
        {
            List<WaterSealModel> list = JsonMapper.ToObject<List<WaterSealModel>>(result);
            InitGrid(list);
        }, null);
    }

    void InitGrid(List<WaterSealModel> list)
    {
        waterSealItemList.Clear();
        GameUtils.SpawnCellForTable<WaterSealModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            WaterSealItem item = currentObj.GetComponent<WaterSealItem>();
            item.InitData(data);
            item.SetBackgroundColor(index % 2 == 0 ? new Color(239 / 255.0f, 243 / 255.0f, 250 / 255.0f) : new Color(1, 1, 1));
            waterSealItemList.Add(item);
            currentObj.SetActive(true);
        });
    }
}
