using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using System;
using HighlightingSystem;
using UnityEngine.Networking;

public static class GameUtils
{
    /// <summary>
    /// 获取网络是否可用
    /// </summary>
    public static bool NetIsAvailable { get { return Application.internetReachability != NetworkReachability.NotReachable; } }

    /// <summary>
    /// wifi是否可用
    /// </summary>
    public static bool WifiIsAvailable { get { return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork; } }

    /// <summary>
    /// Get方式网络请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailed"></param>
    public static void GetHttp(string url, System.Action<string> onSuccess, System.Action<string> onFailed)
    {
        UnityEngine.EventSystems.EventSystem es = UnityEngine.EventSystems.EventSystem.current;
        es.StartCoroutine(HttpGet(url, onSuccess, onFailed));
    }

    private static System.Collections.IEnumerator HttpGet(string url, System.Action<string> onSuccess, System.Action<string> onFailed)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;

            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                onSuccess(www.text);
            }
            else
            {
                string error = www.url + " = " + www.error + " = " + www.text;
                onFailed(error);
            }
        }
    }

    public const string serverUrl = "http://www.huaiantegang.com";
    /// <summary>
    /// post方式网络请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="fields"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailed"></param>
    public static void PostHttp(string url, WWWForm form, System.Action<string> onSuccess, System.Action<string> onFailed)
    {
        url = serverUrl + "/Handler/" + url;
        UnityEngine.EventSystems.EventSystem es = UnityEngine.EventSystems.EventSystem.current;
        es.StartCoroutine(HttpPost(url, form, onSuccess, onFailed));
    }

    private static IEnumerator HttpPost(string url, WWWForm form, System.Action<string> onSuccess, System.Action<string> onFailed)
    {
        using (WWW www = new WWW(url, form.data))
        {
            yield return www;
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                if (onSuccess != null)
                {
                    onSuccess(www.text);
                }
            }
            else
            {
                if (onFailed != null)
                {
                    string error = www.url + " = " + www.error + " = " + www.text;
                    onFailed(error);
                }
            }
        }
    }

    // www.downloadHandler 是服务器往客户端发送的数据
    // www.uploadHandler   是客户端往服务器发送的数据

    public static void PostHttpWebRequest(string url, WWWForm form, System.Action<byte[]> onSuccess, System.Action<string> onFailed)
    {
        url = serverUrl + "/Handler/" + url;
        UnityEngine.EventSystems.EventSystem es = UnityEngine.EventSystems.EventSystem.current;
        es.StartCoroutine(HttpPostWebRequest(url, form, onSuccess, onFailed));
    }

    private static IEnumerator HttpPostWebRequest(string url, WWWForm form, System.Action<byte[]> onSuccess, System.Action<string> onFailed)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                if (onSuccess != null)
                {
                    onSuccess(www.downloadHandler.data);
                }
            }
            else
            {
                if (onFailed != null)
                {
                    string error = www.error + " " + www.isHttpError + " " + www.isNetworkError;
                    onFailed(error);
                }
            }
        }
    }

    public static void GetHttpWebRequest(string url, System.Action<byte[]> onSuccess, System.Action<string> onFailed)
    {
        url = serverUrl + url;
        UnityEngine.EventSystems.EventSystem es = UnityEngine.EventSystems.EventSystem.current;
        es.StartCoroutine(HttpGetWebRequest(url, onSuccess, onFailed));
    }

    private static IEnumerator HttpGetWebRequest(string url, System.Action<byte[]> onSuccess, System.Action<string> onFailed)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                if (onSuccess != null)
                    onSuccess(www.downloadHandler.data);
            }
            else
            {
                if (onFailed != null)
                    onFailed(www.error + " " + www.isHttpError + " " + www.isNetworkError);
            }
        }
    }

    #region PlayerPrefs
    static string GetKey(string key)
    {
        return AppConfig.APP_NAME + "_" + key;
    }

    public static bool HasKey(string key)
    {
        string name = GetKey(key);
        return PlayerPrefs.HasKey(name);
    }

    public static int GetInt(string key, int value)
    {
        string name = GetKey(key);
        if (PlayerPrefs.HasKey(name))
            return PlayerPrefs.GetInt(name);
        else
            return value;
    }

    public static string GetString(string key, string value)
    {
        string name = GetKey(key);
        if (PlayerPrefs.HasKey(name))
        {
            string str = PlayerPrefs.GetString(name);
            str = WWW.UnEscapeURL(str);
            return str;
        }
        else
        {
            return value;
        }
    }

    public static void SetInt(string key, int value)
    {
        string name = GetKey(key);
        PlayerPrefs.SetInt(name, value);
    }

    public static void SetString(string key, string value)
    {
        string name = GetKey(key);
        value = WWW.EscapeURL(value);//用url编码,否则无法识别中文
        PlayerPrefs.SetString(name, value);
    }

    public static void RemoveKey(string key)
    {
        string name = GetKey(key);
        PlayerPrefs.DeleteKey(name);
    }
    #endregion


    /// <summary>
    /// 设置对象高亮
    /// </summary>
    /// <param name="go"></param>
    /// <param name="isLight"></param>
    /// <param name="color"></param>
    public static void SetObjectHighLight(GameObject go, bool isLight, Color startColor = new Color(), Color endColor = new Color())
    {
        if (isLight)
        {
            Highlighter highlighter = go.GetComponent<Highlighter>();
            if (highlighter == null)
                highlighter = go.AddComponent<Highlighter>();
            highlighter.overlay = true;
            highlighter.tweenDuration = 0.6f;
            highlighter.tweenEasing = Easing.QuadIn;
            UnityEngine.Gradient gradient = new UnityEngine.Gradient();
            GradientColorKey[] gradientColorKey = new UnityEngine.GradientColorKey[] {
                new GradientColorKey() { color = startColor, time = 0 },
                new GradientColorKey() { color = endColor, time = 0.5f },
            };
            gradient.colorKeys = gradientColorKey;
            highlighter.tweenGradient = gradient;
            highlighter.TweenStart();
        }
        else
        {
            Highlighter highlighter = go.GetComponent<Highlighter>();
            if (highlighter)
            {
                highlighter.TweenStop();
            }
        }
    }

    public static void DelayWaitForSeconds(float delaySec, Action action, MonoBehaviour mono = null)
    {
        if (mono == null)
            mono = EventSystem.current;
        mono.StartCoroutine(DelayWaitForSecondsCoroutine(delaySec, action));
    }

    public static void DelayWaitForEndOfFrame(Action action, MonoBehaviour mono = null)
    {
        if (mono == null)
            mono = EventSystem.current;
        mono.StartCoroutine(DelayWaitForEndOfFrameCoroutine(action));
    }

    static IEnumerator DelayWaitForSecondsCoroutine(float delaySec, Action action)
    {
        yield return new WaitForSeconds(delaySec);
        if (action != null)
            action.Invoke();
    }

    static IEnumerator DelayWaitForEndOfFrameCoroutine(Action action)
    {
        yield return new WaitForEndOfFrame();
        if (action != null)
            action.Invoke();
    }

    public static void LoopCheck(Func<bool> checkAction, MonoBehaviour mono = null)
    {
        if (mono == null)
            mono = EventSystem.current;
        mono.StartCoroutine(LoopCheckCoroutine(checkAction));
    }

    static IEnumerator LoopCheckCoroutine(Func<bool> checkAction)
    {
        yield return new WaitUntil(checkAction);
    }

    public static void SpawnCellForTable<T>(Transform parent, List<T> dataList, Action<GameObject, T, bool, int> func, bool isHasTitleItem = true)
    {
        if (parent == null)
            return;
        if (dataList == null)
            return;

        int hasCount = isHasTitleItem ? parent.childCount - 1 : parent.childCount;
        int showCount = dataList.Count;

        int fuyongCount = 0;
        int spawnCount = 0;
        int hideCount = 0;

        if (showCount >= hasCount)
        {
            fuyongCount = hasCount;
            spawnCount = showCount - fuyongCount;
            hideCount = 0;
        }
        else
        {
            spawnCount = 0;
            fuyongCount = showCount;
            hideCount = hasCount - showCount;
        }

        if (fuyongCount > 0)
        {
            for (int i = 0; i < fuyongCount; i++)
            {
                GameObject go = parent.GetChild(isHasTitleItem ? i + 1 : i).gameObject;
                func(go, dataList[i], false, i);
            }
        }

        if (spawnCount > 0)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                func(parent.gameObject, dataList[fuyongCount + i], true, fuyongCount + i);
            }
        }

        if (hideCount > 0)
        {
            for (int i = fuyongCount; i < hasCount; i++)
            {
                parent.GetChild(isHasTitleItem ? i + 1 : i).gameObject.SetActive(false);
            }
        }
    }

    public static void GetAllChildren(List<Transform> list, Transform target)
    {
        list.Add(target);
        foreach (Transform item in target)
        {
            GetAllChildren(list, item);
        }
    }

    public static void SetMaterialFloatVariable(Transform target, string variableName, float value, bool isContainsChild = false)
    {
        List<Transform> allChildren = new List<Transform>();
        if (isContainsChild)
            GetAllChildren(allChildren, target);
        else
            allChildren.Add(target);
        if (allChildren != null && allChildren.Count > 0)
        {
            Transform currentTransform;
            Renderer renderer;
            for (int i = 0; i < allChildren.Count; i++)
            {
                currentTransform = allChildren[i];
                renderer = currentTransform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material m = renderer.material;
                    m.SetFloat(variableName, value);
                }
            }
        }
    }

    public static void Push(string pageName)
    {
        GameObject currentGo = GameObject.Instantiate(Resources.Load(pageName)) as GameObject;
        GameObject uiRoot = GameObject.Find("UI");
        currentGo.transform.SetParent(uiRoot.transform);
        RectTransform rtf = currentGo.GetComponent<RectTransform>();
        rtf.anchoredPosition = Vector3.zero;
        rtf.anchorMin = new Vector2(0, 0);
        rtf.anchorMax = new Vector2(1, 1);
        rtf.offsetMin = new Vector2(0, 0);
        rtf.offsetMax = new Vector2(0, 0);

        currentGo.transform.localScale = Vector3.one;
        currentGo.SetActive(true);
    }

    public static GameObject FindGameObject(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            GameObject target = GameObject.Find(path);
            if (target)
                return target;
            else
            {
                int startIndex = path.IndexOf("/");
                if (startIndex >= 0)
                {
                    string rootPath = path.Substring(0, startIndex);
                    GameObject root = GameObject.Find(rootPath);
                    if (root)
                    {
                        string childPath = path.Substring(startIndex + 1);
                        Transform result = root.transform.Find(childPath);
                        if (result)
                        {
                            return result.gameObject;
                        }
                    }
                }
            }
        }
        return null;
    }
}
