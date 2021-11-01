using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PaintDrager : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    BellPaintView paintView;
    public void Start()
    {
        paintView = LuaFramework.Util.GetPaintView();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //paintView.InDrawPoint(eventData.position);
        paintView.InDragUpdate(eventData.position);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        paintView.InDragUpdate(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        paintView.InDragEnd();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        paintView.InDragEnd();
    }

}
