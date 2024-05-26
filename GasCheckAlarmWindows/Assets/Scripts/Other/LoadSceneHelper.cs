using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneHelper : MonoBehaviour
{
    public string sceneName = string.Empty;
    public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
    public bool isAsync = false;
    void Start()
    {
        if (isAsync)
        {
            StartCoroutine(LoadLevelAsync());
        }
        else
        {
            SceneManager.LoadScene(sceneName, loadSceneMode);
        }

    }

    IEnumerator LoadLevelAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        op.completed += Op_completed;
        int displayProgress = 0;
        int toProgress = 0;
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            toProgress = (int)(op.progress * 100);
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                yield return new WaitForEndOfFrame();
            }
        }
        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
    }

    private void Op_completed(AsyncOperation obj)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }

}
