using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealtimeDataMenuPanel : UIEventHelper
{
    public List<Button> btnList;
    public List<GameObject> panelList;

    void Start()
    {
        for (int i = 0; i < btnList.Count; i++)
        {
            RegisterBtnClick<int>(btnList[i], i, OnClick);
        }
        btnList[0].gameObject.SetActive(FormatData.currentUser.Authority == 1);
        if(FormatData.currentUser.Authority == 1)
        {
            OnClick(btnList[0], 0);
        }
        else
        {
            OnClick(btnList[1], 1);
        }
    }

    void OnClick(Button btn, int index)
    {
        for (int i = 0; i < panelList.Count; i++)
        {
            if (i == index)
            {
                panelList[i].SetActive(true);
                btnList[i].GetComponent<Image>().color = new Color(1, 1, 1);
                btnList[i].transform.GetChild(0).GetComponent<Text>().color = new Color(41 / 255.0f, 141 / 255.0f, 125 / 255.0f);
            }
            else
            {
                panelList[i].SetActive(false);
                btnList[i].GetComponent<Image>().color = new Color(5 / 255.0f, 147 / 255.0f, 122 / 255.0f);
                btnList[i].transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1);
            }
        }
    }
}
