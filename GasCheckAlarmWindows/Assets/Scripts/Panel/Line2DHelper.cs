using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line2DHelper : MonoBehaviour
{
    public RectTransform startObj;
    public RectTransform endObj;

    LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Init(RectTransform startObj, RectTransform endObj)
    {
        this.startObj = startObj;
        this.endObj = endObj;
    }

    void Update()
    {
        if (lineRenderer && startObj && endObj)
        {
            Vector3 startPos = startObj.localPosition;
            startPos.y -= startObj.rect.size.y / 2;
            Vector3 endPos = endObj.localPosition;
            endPos.y += endObj.rect.size.y / 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }
}
