using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PageModel
{
    public string pageName;
    public GameObject page;
}

public class PageRoute : MonoBehaviour
{
    public static PageRoute instance;
    private Transform uiCanvas;
    void Awake()
    {
        instance = this;
        uiCanvas = transform.Find("UICanvas");
    }

    public UIEventHelper Push(string pageName)
    {
        PageModel model = FindPage(pageName);
        if (model != null)
        {
            var pb = model.page.GetComponent<UIEventHelper>();
            return pb;
        }
        else
        {
            GameObject go = (GameObject)Resources.Load(pageName);
            go.transform.SetParent(uiCanvas);
            PageModel newModel = new PageModel();
            newModel.pageName = pageName;
            newModel.page = go;
            list.Add(newModel);
            return go.GetComponent<UIEventHelper>();
        }
    }

    public void Pop(string pageName)
    {
        PageModel model = FindPage(pageName);
        if (model != null)
        {
            var pb = model.page.GetComponent<UIEventHelper>();
            //model.page.SetActive(false);
        }
    }

    public UIEventHelper Flip(string pageName)
    {
        PageModel model = FindPage(pageName);
        if (model != null)
        {
            model.page.SetActive(!model.page.activeSelf);
            return model.page.GetComponent<UIEventHelper>();
        }
        return null;
    }

    public List<PageModel> list = new List<PageModel>();
    public PageModel FindPage(string pageName)
    {
        if (list != null && list.Count > 0)
        {
            foreach (var item in list)
            {
                if (item.pageName == pageName)
                {
                    return item;
                }
            }
        }
        return null;
    }
}
