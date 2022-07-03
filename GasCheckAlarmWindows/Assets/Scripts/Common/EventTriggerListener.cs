using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventTriggerListener : EventTrigger
{
    public delegate void VoidDelegate(GameObject go, object data = null);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onDrag;
    public VoidDelegate onBeginDrag;
    public VoidDelegate onEndDrag;

    [HideInInspector]
    public object data;
    ScrollRect sr;
    static public EventTriggerListener Get(GameObject go, object data = null)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        listener.data = data;
        return listener;
    }

    void Start()
    {
        Transform firstParent = transform.parent;
        if (firstParent)
        {
            Transform secondParent = firstParent.parent;
            if (secondParent)
            {
                sr = secondParent.GetComponent<ScrollRect>();
            }
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (onClick != null) onClick(gameObject, data);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (onDown != null) onDown(gameObject, data);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (onEnter != null) onEnter(gameObject, data);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (onExit != null) onExit(gameObject, data);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (onUp != null) onUp(gameObject, data);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (onSelect != null) onSelect(gameObject, data);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        base.OnUpdateSelected(eventData);
        if (onUpdateSelect != null) onUpdateSelect(gameObject, data);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (sr)
        {
            sr.OnDrag(eventData);
        }
        base.OnDrag(eventData);
        if (onDrag != null) onDrag(gameObject, data);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (sr)
        {
            sr.OnBeginDrag(eventData);
        }
        base.OnBeginDrag(eventData);
        if (onBeginDrag != null) onBeginDrag(gameObject, data);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (sr)
        {
            sr.OnEndDrag(eventData);
        }
        base.OnEndDrag(eventData);
        if (onEndDrag != null) onEndDrag(gameObject, data);
    }
}