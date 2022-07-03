using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MachineManagerPanel : UIEventHelper
{
    public Button btn_add;
    public Button btn_delete;
    public Button btn_edit;
    public Button btn_select;

    public Toggle wholeToggle;

    public GameObject addMachinePanel;
    public GameObject editMachinePanel;

    public InputField input_machineName;
    public Dropdown dropdown_factory;

    public Transform contentTrans;
    Object itemRes;
    void Awake()
    {
        itemRes = Resources.Load("MachineItem");
    }

    private void Start()
    {
        RegisterBtnClick(btn_add, OnAddMachine);
        RegisterBtnClick(btn_delete, OnDeleteMachine);
        RegisterBtnClick(btn_edit, OnEditMachine);
        RegisterBtnClick(btn_select, OnSelectMachine);

        RegisterTogClick(wholeToggle, OnWholeToggle);

        EventManager.Instance.AddEventListener(NotifyType.UpdateMachineList, UpdateMachineListEvent);
    }

    void UpdateMachineListEvent(object data)
    {
        InitData();
    }

    void OnSelectMachine(Button btn)
    {
        string machineName = input_machineName.text;
        int dd = dropdown_factory.value;

        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllMachineByCondition");
        form.AddField("machineName", machineName);
        form.AddField("factoryID", dd == 0 ? -1 : factoryList[dd - 1].ID);
        GameUtils.PostHttp("Machine.ashx", form, (result) =>
        {
            List<MachineModel> list = JsonMapper.ToObject<List<MachineModel>>(result);
            InitGrid(list);
        }, null);
    }

    void OnAddMachine(Button btn)
    {
        addMachinePanel.SetActive(true);
    }

    void OnDeleteMachine(Button btn)
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
                form.AddField("requestType", "DeleteMachineByID");
                form.AddField("idList", sb.ToString());
                GameUtils.PostHttp("Machine.ashx", form, null, null);

                EventManager.Instance.DisPatch(NotifyType.UpdateMachineList);
                MessageBox.Instance.PopOK("删除成功", null, "确定");

            }, "取消", "确定");
        }
    }

    void OnEditMachine(Button btn)
    {
        List<MachineItem> selectItemList = new List<MachineItem>();
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
            MachineItem selectItem = selectItemList[0];
            editMachinePanel.SetActive(true);
            EditMachinePanel panel = editMachinePanel.GetComponent<EditMachinePanel>();
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

    List<MachineItem> itemList = new List<MachineItem>();
    private void InitData()
    {
        InitDropdown();

        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllMachineByCondition");
        GameUtils.PostHttp("Machine.ashx", form, (result) =>
        {
            List<MachineModel> list = JsonMapper.ToObject<List<MachineModel>>(result);
            InitGrid(list);
        }, null);
    }

    List<FactoryModel> factoryList;
    void InitDropdown()
    {
        dropdown_factory.ClearOptions();

        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllFactoryByCondition");
        GameUtils.PostHttp("Factory.ashx", form, (result) =>
        {
            factoryList = JsonMapper.ToObject<List<FactoryModel>>(result);
            if (factoryList != null && factoryList.Count > 0)
            {
                List<string> optionList = new List<string>() { "请选择" };
                for (int i = 0; i < factoryList.Count; i++)
                {
                    optionList.Add(factoryList[i].FactoryName);
                }
                dropdown_factory.AddOptions(optionList);
                dropdown_factory.value = 0;
            }
        }, null);
    }

    void InitGrid(List<MachineModel> list)
    {
        itemList.Clear();
        GameUtils.SpawnCellForTable<MachineModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            MachineItem item = currentObj.GetComponent<MachineItem>();
            item.InitData(data);
            item.SetBackgroundColor(index % 2 == 0 ? new Color(239 / 255.0f, 243 / 255.0f, 250 / 255.0f) : new Color(1, 1, 1));
            itemList.Add(item);
            currentObj.SetActive(true);
        });
    }
}
