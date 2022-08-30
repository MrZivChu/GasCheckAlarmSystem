using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FactoryManagerPanel : UIEventHelper
{
    public Button btn_add;
    public Button btn_delete;
    public Button btn_edit;
    public Button btn_search;
    public Toggle wholeToggle;

    public GameObject addFactoryPanel;
    public GameObject editFactoryPanel;
    public InputField input_factoryName;

    public Transform contentTrans;
    public Object itemRes;
    private void Start()
    {
        RegisterBtnClick(btn_add, OnAddFactory);
        RegisterBtnClick(btn_delete, OnDeleteFactory);
        RegisterBtnClick(btn_edit, OnEditFactory);
        RegisterBtnClick(btn_search, OnSearchFactory);
        RegisterTogClick(wholeToggle, OnWholeToggle);

        EventManager.Instance.AddEventListener(NotifyType.UpdateFactoryList, UpdateFactoryListEvent);
    }

    void UpdateFactoryListEvent(object data)
    {
        InitData();
    }

    void OnSearchFactory(Button btn)
    {
        string factoryName = input_factoryName.text;
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllFactoryByCondition");
        form.AddField("factoryName", factoryName);
        GameUtils.PostHttp("Factory.ashx", form, (result) =>
        {
            List<FactoryModel> list = JsonMapper.ToObject<List<FactoryModel>>(result);
            InitGrid(list);
        }, null);
    }

    void OnAddFactory(Button btn)
    {
        addFactoryPanel.SetActive(true);
    }

    void OnDeleteFactory(Button btn)
    {
        List<int> idList = new List<int>();
        if (itemList != null && itemList.Count > 0)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].GetToggleStatus())
                {
                    idList.Add(itemList[i].currentModel.ID);
                }
            }
        }
        if (idList.Count <= 0)
        {
            MessageBox.Instance.PopOK("未选择任何数据", null, "确定");
        }
        else
        {
            MessageBox.Instance.PopYesNo("确认删除吗？\r\n会删除关联的数据\r\n请谨慎操作！", null, () =>
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < idList.Count; i++)
                {
                    sb.Append(idList[i] + ",");
                }
                sb = sb.Remove(sb.Length - 1, 1);
                WWWForm form = new WWWForm();
                form.AddField("requestType", "DeleteFactoryByID");
                form.AddField("idList", sb.ToString());
                GameUtils.PostHttp("Factory.ashx", form, (content) =>
                {
                    EventManager.Instance.DisPatch(NotifyType.UpdateFactoryList);
                    MessageBox.Instance.PopOK("删除成功", null, "确定");
                }, null);

            }, "取消", "确定");
        }
    }

    void OnEditFactory(Button btn)
    {
        List<FactoryItem> selectItemList = new List<FactoryItem>();
        if (itemList != null && itemList.Count > 0)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].GetToggleStatus())
                {
                    selectItemList.Add(itemList[i]);
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
            FactoryItem selectItem = selectItemList[0];
            editFactoryPanel.SetActive(true);
            EditFactoryPanel panel = editFactoryPanel.GetComponent<EditFactoryPanel>();
            panel.InitData(selectItem.currentModel);
        }
    }

    void OnWholeToggle(Toggle tog, bool isOn)
    {
        if (itemList != null && itemList.Count > 0)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].SetToggle(isOn);
            }
        }
    }

    private void OnEnable()
    {
        InitData();
    }

    List<FactoryItem> itemList = new List<FactoryItem>();
    private void InitData()
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllFactoryByCondition");
        GameUtils.PostHttp("Factory.ashx", form, (result) =>
        {
            List<FactoryModel> list = JsonMapper.ToObject<List<FactoryModel>>(result);
            InitGrid(list);
        }, null);
    }

    void InitGrid(List<FactoryModel> list)
    {
        itemList.Clear();
        GameUtils.SpawnCellForTable<FactoryModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            FactoryItem item = currentObj.GetComponent<FactoryItem>();
            item.InitData(data);
            item.SetBackgroundColor(index % 2 == 0 ? new Color(239 / 255.0f, 243 / 255.0f, 250 / 255.0f) : new Color(1, 1, 1));
            itemList.Add(item);
            currentObj.SetActive(true);
        });
    }
}
