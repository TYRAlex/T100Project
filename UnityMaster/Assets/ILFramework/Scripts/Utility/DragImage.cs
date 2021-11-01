using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragImage : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 directionHyp;
    public RectTransform CurDragRect;
    float temp = 0;
    public Vector2 StartPos;
    private Action<float> _drag = null;
    bool _isEnter;
    public void SetDragCallback(Action<float> drag = null)
    {
        _drag = drag;
    }

    public void Reset()
    {
        transform.localPosition = StartPos;
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (_isEnter)
        {


            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(CurDragRect, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                var r = RestrictRect(CurDragRect, globalMousePos.x, globalMousePos.y);
                this.transform.position = new Vector3(r.x, r.y);

                var anchoredPositionX = transform.GetComponent<RectTransform>().anchoredPosition.x;

                _drag?.Invoke(anchoredPositionX);
            }
        }
    }

    private Vector2 RestrictRect(RectTransform DragRect, float x, float y)
    {
        RectTransform rectTransform = this.transform as RectTransform;

        //最小x坐标 =容器当前x坐标  -容器轴心距离左边界的距离 + UI轴心距离左边界的距离
        float minX = DragRect.position.x - DragRect.pivot.x * DragRect.rect.width + rectTransform.rect.width * rectTransform.pivot.x;

        // 最大x坐标 = 容器当前x坐标 + 容器轴心距离右边界的距离 - UI轴心距离右边界的距离
        float maxX = DragRect.position.x + (1 - DragRect.pivot.x) * DragRect.rect.width - rectTransform.rect.width * (1 - rectTransform.pivot.x);

        // 最小y坐标 = 容器当前y坐标 - 容器轴心距离底边的距离 + UI轴心距离底边的距离
        float minY = DragRect.position.y - DragRect.pivot.y * DragRect.rect.height + rectTransform.rect.height * rectTransform.pivot.y;

        // 最大y坐标 = 容器当前x坐标 + 容器轴心距离顶边的距离 - UI轴心距离顶边的距离
        float maxY = DragRect.position.y + (1 - DragRect.pivot.y) * DragRect.rect.height - rectTransform.rect.height * (1 - rectTransform.pivot.y);

        float rx = Mathf.Clamp(x, minX, maxX);
        float ry = Mathf.Clamp(y, minY, maxY);

        return new Vector2(rx, ry);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isEnter = false;
    }
}
