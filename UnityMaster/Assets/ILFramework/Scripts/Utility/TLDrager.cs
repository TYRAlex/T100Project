using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TLDrager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 _offset = Vector3.zero; //位置偏移量
    Vector3 _startPos;                 //初始位置

    public int dragType = 0;           //拖拽类型
    public int index = 0;              //索引用来确定具体对象
    public bool isActived = true;
    public bool canMove = true;
    public bool isChange = false;
    public bool useMouse = true;

    Action<Vector3, TLDrager> dragStart = null;           //这个V3返回的是transform.localPosition
    Action<Vector3, TLDrager> drag = null;                //这个V3返回的是transform.localPosition
    Action<Vector3, TLDrager,TLDroper,bool> dragEnd = null;       //这个V3返回的是transform.localPosition

    public RectTransform DragRect;   //拖拽限制范围

    public TLDroper[] drops = new TLDroper[0];
    public TLDroper[] failDrops = new TLDroper[0];

    private RectTransform _rectTransform;
    private TLDrager _tLDrager;

    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;

    public bool IsNeedOffset = true;

    void Awake()
    {
        _tLDrager = this.GetComponent<TLDrager>();
        _startPos = this.transform.localPosition;
        _rectTransform = GetComponent<RectTransform>();
    }


    #region 拖拽接口
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isActived) return;
        if (dragStart != null)
        {
            dragStart.Invoke(new Vector2(transform.localPosition.x, transform.localPosition.y), _tLDrager);
        }

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, eventData.position,
           eventData.pressEventCamera, out Vector3 globalMousePos))
        {
            if (IsNeedOffset)
            {
                _offset = _rectTransform.position - globalMousePos;
            }

            //  SetDragRange();
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDragRange();
        if (!isActived) return;
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(DragRect, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            if (canMove)
            {
                _rectTransform.position = globalMousePos + _offset;
                _rectTransform.position = DragRangeLimit(_rectTransform.position);
            }
        }
       
        //显示可放入特效
        for (int i = 0; i < drops.Length; i++)
        {
            var _drop = drops[i];
            if (RectTransformUtility.RectangleContainsScreenPoint(_drop.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera))
                _drop.showEffect(true, dragType);
            else
                _drop.showEffect(false);
        }

        if (drag != null)
            drag.Invoke(new Vector2(transform.localPosition.x, transform.localPosition.y), _tLDrager);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isActived) return;
        bool isMatch = false;
        TLDroper tLDroper = null;

        if (canMove)
        {
            for (int i = 0; i < drops.Length; i++)
            {
                var _drop = drops[i];
                if (RectTransformUtility.RectangleContainsScreenPoint(_drop.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera)&&useMouse)
                {
                    if (_drop.isActived)
                    {
                        isMatch = _drop.doAfter(dragType);
                        tLDroper = _drop;
                    }
                    _drop.showEffect(false);
                }
                if (Overlaps(_rectTransform, DragRect)&&!useMouse)
                {
                    if (_drop.isActived)
                    {
                        isMatch = _drop.doAfter(dragType);
                        tLDroper = _drop;
                    }
                    _drop.showEffect(false);
                }
            }

            foreach (var failDrop in failDrops)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(failDrop.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera)
                    && failDrop.isActived)
                    failDrop.DroperFail(failDrop.dropType);
            }
        }

        if (isChange)
        {
            isMatch = false;
            for (int i = 0; i < drops.Length; i++)
            {
                var _drop = drops[i];
                if (RectTransformUtility.RectangleContainsScreenPoint(_drop.GetComponent<RectTransform>(), this.transform.position, eventData.pressEventCamera))
                {
                    if (_drop.isActived)
                    {
                        isMatch = _drop.doAfter(dragType);
                        tLDroper = _drop;
                    }
                    _drop.showEffect(false);
                }
            }
            foreach (var failDrop in failDrops)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(failDrop.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera)
                    && failDrop.isActived)
                    failDrop.DroperFail(failDrop.dropType);
            }
        }

        if (dragEnd != null)
        {
            dragEnd.Invoke(new Vector2(transform.localPosition.x, transform.localPosition.y), _tLDrager, tLDroper ,isMatch);
        }
    }
    #endregion

    /// <summary>
    /// 设置拖拽最大值和最小值
    /// </summary>
    void SetDragRange()
    {
        if (DragRect == null)
        {
            Debug.LogError("DragRect is Null...");
            return;
        }
        var position = DragRect.transform.position;

        _minX = position.x - DragRect.pivot.x * DragRect.rect.width + _rectTransform.rect.width * _rectTransform.pivot.x* _rectTransform.localScale.x;
        _maxX = position.x + (1 - DragRect.pivot.x) * DragRect.rect.width - _rectTransform.rect.width * (1 - _rectTransform.pivot.x) * _rectTransform.localScale.x;
        _minY = position.y - DragRect.pivot.y * DragRect.rect.height + _rectTransform.rect.height * _rectTransform.pivot.y * _rectTransform.localScale.y;
        _maxY = position.y + (1 - DragRect.pivot.y) * DragRect.rect.height - _rectTransform.rect.height * (1 - _rectTransform.pivot.y) * _rectTransform.localScale.y;
    }

    /// <summary>
    /// 限制坐标范围
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    Vector3 DragRangeLimit(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
        pos.y = Mathf.Clamp(pos.y, _minY, _maxY);
        return pos;
    }

    private bool Overlaps(RectTransform a,RectTransform b)
    {
        return a.rect.Overlaps(b.rect);
    }

    public void DoReset()
    {
        this.transform.localPosition = _startPos;
    }

    public void SetDragCallback(Action<Vector3, TLDrager> lstart = null, Action<Vector3, TLDrager> ldrag = null, Action<Vector3,TLDrager,TLDroper,bool> lend = null)
    {
        dragStart = lstart;
        drag = ldrag;
        dragEnd = lend;
    }
}
