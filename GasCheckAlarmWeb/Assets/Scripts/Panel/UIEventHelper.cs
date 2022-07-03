using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using uTools;

public class UIEventHelper : MonoBehaviour
{
    public void RegisterBtnClick(Button btn, Action<Button> callback, bool isOnlyEvent = true)
    {
        if (isOnlyEvent)
            btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            callback(btn);
        });
    }

    public void RegisterBtnClick<T>(Button btn, T param, Action<Button, T> callback, bool isOnlyEvent = true)
    {
        if (isOnlyEvent)
            btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            callback(btn, param);
        });
    }

    public void RegisterTogClick(Toggle tog, Action<Toggle, bool> callback, bool isOnlyEvent = true)
    {
        if (isOnlyEvent)
            tog.onValueChanged.RemoveAllListeners();
        tog.onValueChanged.AddListener((isOn) =>
        {
            callback(tog, isOn);
        });
    }

    public void RegisterTogClick<T>(Toggle tog, T param, Action<Toggle, bool, T> callback, bool isOnlyEvent = true)
    {
        if (isOnlyEvent)
            tog.onValueChanged.RemoveAllListeners();
        tog.onValueChanged.AddListener((isOn) =>
        {
            callback(tog, isOn, param);
        });
    }

    public void RegisterInputFieldOnEndEdit(InputField inputField, Action<InputField, string> callback, bool isOnlyEvent = true)
    {
        if (isOnlyEvent)
            inputField.onEndEdit.RemoveAllListeners();
        inputField.onEndEdit.AddListener((content) =>
        {
            callback(inputField, content);
        });
    }

    public void RegisterInputFieldOnEndEdit<T>(InputField inputField, T param, Action<InputField, string, T> callback, bool isOnlyEvent = true)
    {
        if (isOnlyEvent)
            inputField.onEndEdit.RemoveAllListeners();
        inputField.onEndEdit.AddListener((content) =>
        {
            callback(inputField, content, param);
        });
    }

    public void RegisterDropDownOnValueChanged(Dropdown dropdown, Action<Dropdown, int> callback, bool isOnlyEvent = true)
    {
        if (isOnlyEvent)
            dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((content) =>
        {
            callback(dropdown, content);
        });
    }
}
