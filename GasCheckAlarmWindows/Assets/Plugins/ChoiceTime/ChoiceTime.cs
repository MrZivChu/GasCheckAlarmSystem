using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceTime : MonoBehaviour
{
    public DateTime minDate_ = new DateTime(1999, 1, 1, 0, 0, 0);
    public DateTime maxDate_ = new DateTime(2050, 1, 1, 0, 0, 0);
    public DateTime selectDate_ = DateTime.Now;
    public Text ShowText;
    public GameObject ChoiceTimeObj;
    public Button ChoiceBtn;
    public List<DatePicker> _datePickerList;

    void Start()
    {
        ChoiceTimeObj.SetActive(false);
        ChoiceBtn.onClick.AddListener(() => { ChoiceTimeObj.SetActive(!ChoiceTimeObj.activeSelf); });
        OnSelectedTime();
    }

    public void OnSelectedTime()
    {
        ShowText.text = selectDate_.ToString("yyyy-MM-dd HH:mm:ss");
        _datePickerList.ForEach(it => it.RefreshDateList());
    }
}
