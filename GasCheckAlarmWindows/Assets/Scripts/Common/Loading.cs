using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public static int index;
    public Slider slider;
    void Start()
    {
        slider.value = 0;
        StartCoroutine(LoadLevelAsyn(index));
    }

    //让loading效果更加圆滑
    //http://blog.csdn.net/huang9012/article/details/38659011
    IEnumerator LoadLevelAsyn(int index)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(index);

        int displayProgress = 0;
        int toProgress = 0;
        op.allowSceneActivation = false;
        //把AsyncOperation.allowSceneActivation设为false就可以禁止Unity加载完毕后自动切换场景
        //但allowSceneActivation设置成false,百分比最多到0.9
        while (op.progress < 0.9f)
        {
            toProgress = (int)(op.progress * 100);
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                slider.value = (displayProgress * 0.01f);
                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;
        //最后一步,此时场景已经结束
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            slider.value = (displayProgress * 0.01f);
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
    }
}
