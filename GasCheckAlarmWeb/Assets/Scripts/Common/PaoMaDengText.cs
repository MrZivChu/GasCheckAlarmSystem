using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// 跑马灯文字设置
/// </summary>
public class PaoMaDengText : MonoBehaviour
{

    [SerializeField]
    private Text pmdText;     //跑马灯text.
    private Queue<string> pmdQueue;     //跑马灯队列.
    private bool isEnd = false;   //判断当前text中的跑马灯是否跑完.

    private void OnEnable()
    {
        pmdQueue = new Queue<string>();
        pmdQueue.Enqueue(pmdText.text);
        StartCoroutine(Marquee());
    }

    public float speed = 50f;//滚动速度
    public IEnumerator Marquee()
    {
        float begin_X = 200;  //第一个字开始的位置 
        float end_X = 200;

        while (pmdQueue.Count > 0)
        {
            Vector3 pos = pmdText.rectTransform.localPosition;

            float duration = 10f;  //默认时间

            string msg = pmdQueue.Dequeue();
            pmdText.text = msg;
            float txetWidth = pmdText.preferredWidth;  //文本的长度...
            float distance = begin_X - end_X + txetWidth;   //自己体会...
            duration = distance / speed;

            isEnd = true;
            //Debug.Log("distance:" + -distance + " speed：" + speed + " duration：" + duration);

            while (true)//todo 看需求... (loop > 0)
            {
                pmdText.rectTransform.localPosition = new Vector3(begin_X, pos.y, pos.z);  //归位...

                pmdText.rectTransform.DOLocalMoveX(-distance, duration).SetEase(Ease.Linear);  //滚动...
                yield return new WaitForSeconds(duration);
            }
            yield return null;
        }
        isEnd = false;
        yield break;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
