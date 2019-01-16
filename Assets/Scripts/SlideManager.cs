using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlideManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    [SerializeField]
    private Camera watchingCamera = null;

    private HashSet<IDragListener> listeners = new HashSet<IDragListener>();

    private Vector2 screenSize;

    private void Awake()
    {
        this.screenSize = (new Vector2(Screen.width, Screen.height)) / (this.watchingCamera.orthographicSize * 2);
        float ratio = (float)Screen.width / Screen.height;
        this.screenSize /= ratio;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 position = eventData.position / this.screenSize;

        foreach (IDragListener listener in this.listeners)
        {
            listener.OnBeginDrag(position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = eventData.position / this.screenSize;

        foreach (IDragListener listener in this.listeners)
        {
            listener.OnDrag(position);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 position = eventData.position / this.screenSize;

        foreach (IDragListener listener in this.listeners)
        {
            listener.OnEndDrag(position);
        }
    }

    public void RegisterListener(IDragListener listener)
    {
        this.listeners.Add(listener);
    }

    public void UnregisterListener(IDragListener listener)
    {
        this.listeners.Remove(listener);
    }

    public interface IDragListener
    {
        void OnBeginDrag(Vector2 pos);
        void OnDrag(Vector2 pos);
        void OnEndDrag(Vector2 pos);
    }
}
