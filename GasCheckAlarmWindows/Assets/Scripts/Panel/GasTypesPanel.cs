using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GasTypesPanel : UIEventHelper
{
    public Transform contentTrans;
    public UnityEngine.Object itemRes;
    public Button addBtn;
    public Button cancelBtn;
    public GameObject AddGasTypesPanel;

    void Start()
    {
        RegisterBtnClick(addBtn, OnAdd);
        RegisterBtnClick(cancelBtn, OnCancel);
        EventManager.Instance.AddEventListener(NotifyType.DeleteGasTypes, UpdateGasTypesEvent);
        EventManager.Instance.AddEventListener(NotifyType.InsertGasTypes, UpdateGasTypesEvent);
    }

    void UpdateGasTypesEvent(object data)
    {
        OnEnable();
    }

    void OnAdd(Button btn)
    {
        AddGasTypesPanel.SetActive(true);
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        List<GasTypesModel> list = GasTypesDAL.SelectAllGasTypes();
        InitGrid(list);
    }

    void InitGrid(List<GasTypesModel> list)
    {
        GameUtils.SpawnCellForTable<GasTypesModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            GasTypesItem item = currentObj.GetComponent<GasTypesItem>();
            item.InitInfo(data);
            currentObj.SetActive(true);
        }, false);
    }
}
