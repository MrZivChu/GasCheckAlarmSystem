using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DateType
{
    year,
    month,
    day,
    hour,
    minute,
    second
}
public class DatePicker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ChoiceTime choiceTime_;
    public Button upBtn;
    public Button downBtn;
    public DateType dateType_;
    public int itemNum_ = 1;
    public GameObject itemObj_;
    public Transform itemParent_;

    private float updateLength_;
    void Awake()
    {
        itemObj_.SetActive(false);
        updateLength_ = itemObj_.GetComponent<RectTransform>().sizeDelta.y;
        upBtn.onClick.AddListener(OnUpTime);
        downBtn.onClick.AddListener(OnDownTime);
        for (int i = 0; i < itemNum_; i++)
        {
            int real_i = -(itemNum_ / 2) + i;
            SpawnItem(real_i);
        }
    }

    void OnUpTime()
    {
        UpdateTime(-1);
    }

    void OnDownTime()
    {
        UpdateTime(1);
    }

    void SpawnItem(float real_i)
    {
        GameObject item = Instantiate(itemObj_);
        item.SetActive(true);
        item.transform.SetParent(itemParent_);
        item.transform.localScale = Vector3.one;
        item.transform.localEulerAngles = Vector3.zero;
        item.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        if (real_i != 0)
        {
            item.GetComponent<Text>().color = new Color(1, 1, 1, 1 - 0.2f - (Mathf.Abs(real_i) / (itemNum_ / 2 + 1)));
        }
    }

    Vector3 oldDragPos;
    public void OnBeginDrag(PointerEventData eventData)
    {
        oldDragPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.y - oldDragPos.y) >= updateLength_)
        {
            oldDragPos = eventData.position;
            UpdateTime(eventData.position.y > oldDragPos.y ? 1 : -1);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemParent_.localPosition = Vector3.zero;
    }

    void UpdateTime(int num)
    {
        DateTime data = new DateTime();
        if (dateType_ == DateType.year)
        {
            data = choiceTime_.selectDate_.AddYears(num);
        }
        else if (dateType_ == DateType.month)
        {
            data = choiceTime_.selectDate_.AddMonths(num);
        }
        else if (dateType_ == DateType.day)
        {
            data = choiceTime_.selectDate_.AddDays(num);
        }
        else if (dateType_ == DateType.hour)
        {
            data = choiceTime_.selectDate_.AddHours(num);
        }
        else if (dateType_ == DateType.minute)
        {
            data = choiceTime_.selectDate_.AddMinutes(num);
        }
        else if (dateType_ == DateType.second)
        {
            data = choiceTime_.selectDate_.AddSeconds(num);
        }
        if (IsInDate(data, choiceTime_.minDate_, choiceTime_.maxDate_))
        {
            choiceTime_.selectDate_ = data;
            choiceTime_.OnSelectedTime();
        }
    }

    public void RefreshDateList()
    {
        for (int i = 0; i < itemNum_; i++)
        {
            switch (dateType_)
            {
                case DateType.year:
                    itemParent_.GetChild(i).GetComponent<Text>().text = choiceTime_.selectDate_.ToString("yyyy");
                    break;
                case DateType.month:
                    itemParent_.GetChild(i).GetComponent<Text>().text = choiceTime_.selectDate_.ToString("MM");
                    break;
                case DateType.day:
                    itemParent_.GetChild(i).GetComponent<Text>().text = choiceTime_.selectDate_.ToString("dd");
                    break;
                case DateType.hour:
                    itemParent_.GetChild(i).GetComponent<Text>().text = choiceTime_.selectDate_.ToString("HH");
                    break;
                case DateType.minute:
                    itemParent_.GetChild(i).GetComponent<Text>().text = choiceTime_.selectDate_.ToString("mm");
                    break;
                case DateType.second:
                    itemParent_.GetChild(i).GetComponent<Text>().text = choiceTime_.selectDate_.ToString("ss");
                    break;
            }
        }
    }

    bool IsInDate(DateTime dt, DateTime dt_min, DateTime dt_max)
    {
        return dt.CompareTo(dt_min) >= 0 && dt.CompareTo(dt_max) <= 0;
    }
}
