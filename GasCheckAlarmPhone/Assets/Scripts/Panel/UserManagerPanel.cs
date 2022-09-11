using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UserManagerPanel : UIEventHelper
{
    public Button btn_add;
    public Button btn_delete;
    public Button btn_edit;
    public Button btn_select;

    public Toggle wholeToggle;

    public GameObject addUserPanel;
    public GameObject editUserPanel;

    public InputField input_userName;
    public InputField input_userNumber;
    public InputField input_phone;
    public Dropdown dropdown_authority;

    public Transform contentTrans;
    public Object itemRes;

    private void Start()
    {
        RegisterBtnClick(btn_add, OnAddUser);
        RegisterBtnClick(btn_delete, OnDeleteUser);
        RegisterBtnClick(btn_edit, OnEditUser);
        RegisterBtnClick(btn_select, OnSelectUser);

        RegisterTogClick(wholeToggle, OnWholeToggle);
        EventManager.Instance.AddEventListener(NotifyType.UpdateUserList, UpdateUserListEvent);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateUserList, UpdateUserListEvent);
    }

    void UpdateUserListEvent(object data)
    {
        InitData();
    }

    void OnSelectUser(Button btn)
    {
        string userName = input_userName.text;
        string userNumber = input_userNumber.text;
        string phone = input_phone.text;
        int authority = dropdown_authority.value == 1 ? 0 : (dropdown_authority.value == 2 ? 1 : 2);
        List<UserModel> list = UserDAL.SelectAllUserByCondition(userName, userNumber, phone, authority);
        InitGrid(list);
    }

    void OnAddUser(Button btn)
    {
        addUserPanel.SetActive(true);
    }

    void OnDeleteUser(Button btn)
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
            MessageBox.Instance.PopYesNo("确认删除？", null, () =>
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < idList.Count; i++)
                {
                    sb.Append(idList[i] + ",");
                }
                sb = sb.Remove(sb.Length - 1, 1);
                UserDAL.DeleteUserByID(sb.ToString());
                MessageBox.Instance.PopOK("删除成功", () =>
                {
                    EventManager.Instance.DisPatch(NotifyType.UpdateUserList);
                }, "确定");

            }, "取消", "确定");
        }
    }

    void OnEditUser(Button btn)
    {
        List<UserItem> selectItemList = new List<UserItem>();
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
            UserItem selectItem = selectItemList[0];
            editUserPanel.SetActive(true);
            EditUserPanel panel = editUserPanel.GetComponent<EditUserPanel>();
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

    List<UserItem> itemList = new List<UserItem>();
    private void InitData()
    {
        List<UserModel> list = UserDAL.SelectAllUser();
        InitGrid(list);
    }

    private void InitGrid(List<UserModel> list)
    {
        itemList.Clear();
        GameUtils.SpawnCellForTable<UserModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            UserItem item = currentObj.GetComponent<UserItem>();
            item.InitData(data);
            item.SetBackgroundColor(index % 2 == 0 ? new Color(239 / 255.0f, 243 / 255.0f, 250 / 255.0f) : new Color(1, 1, 1));
            itemList.Add(item);
            currentObj.SetActive(true);
        });
    }
}
