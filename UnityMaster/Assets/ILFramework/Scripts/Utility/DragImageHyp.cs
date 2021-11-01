using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;


public class DragImageHyp : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{

    private Vector3 directionHyp;

    public float maxDistanceHyp;

    public RectTransform Zhen;
    public RectTransform DragRectUp;
    public RectTransform DragRectDown;

    public RectTransform CurDragRect;
    public float StartX;
    private bool _isEnter;

    private bool _isUp;

    Action<float ,float,bool> _drag = null;


    public void OnDrag(PointerEventData eventData)
    {
        bool isNull = CurDragRect == null;

        if (_isEnter)
        {
            if (isNull)
            {

                transform.position = eventData.position;

                if (Vector3.Distance(transform.localPosition, Vector3.zero) > maxDistanceHyp || Vector3.Distance(transform.localPosition, Vector3.zero) < maxDistanceHyp)
                {
                    directionHyp = transform.localPosition - Vector3.zero;
                    transform.localPosition = directionHyp.normalized * maxDistanceHyp;
                }
                Vector2 yuanAnc = transform.GetComponent<RectTransform>().anchoredPosition;
                Vector2 nor1 = (new Vector2(yuanAnc.x, yuanAnc.y)).normalized;
                Vector2 nor2 = (new Vector2(StartX, 0)).normalized;

                var angle = Vector2.Angle(nor2, nor1);
                Vector3 cross = Vector3.Cross(nor2, nor1);
                Zhen.rotation = Quaternion.Euler(0, 0, cross.z > 0 ? angle : -angle);

                bool isGtZero = cross.z > 0;

                if (isGtZero)
                {
                    CurDragRect = DragRectDown;
                    _isUp = false;
                }
                else
                {
                    CurDragRect = DragRectUp;
                    _isUp = true;
                }


            }
            else
            {
                Vector3 globalMousePos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(CurDragRect, eventData.position, eventData.pressEventCamera, out globalMousePos))
                {
                    var r = RestrictRect(CurDragRect, globalMousePos.x, globalMousePos.y);
                    this.transform.position = new Vector3(r.x, r.y);
                    if (Vector3.Distance(transform.localPosition, Vector3.zero) < maxDistanceHyp|| Vector3.Distance(transform.localPosition, Vector3.zero) > maxDistanceHyp)
                    {
                        directionHyp = transform.localPosition - Vector3.zero;
                        transform.localPosition = directionHyp.normalized * maxDistanceHyp;
                    }
                }
                Vector2 yuanAnc = transform.GetComponent<RectTransform>().anchoredPosition;
                Vector2 zhenAnc = Zhen.anchoredPosition;
                Vector2 nor1 = (new Vector2(yuanAnc.x, yuanAnc.y)).normalized;
                Vector2 nor2 = (new Vector2(StartX, 0)).normalized;

                var angle = Vector2.Angle(nor2, nor1);
                Vector3 cross = Vector3.Cross(nor2, nor1);
                Zhen.rotation = Quaternion.Euler(0, 0, cross.z > 0 ? angle : -angle);

                if (cross.z == 0 && angle == 0)
                {
                    if (CurDragRect == DragRectUp)
                    {
                        _isUp = false;
                        CurDragRect = DragRectDown;
                    }
                    else if (CurDragRect == DragRectDown)
                    {
                        _isUp = true;
                        CurDragRect = DragRectUp;
                    }
                }

                _drag?.Invoke(angle,cross.z,_isUp);
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


    //松开鼠标左键，回到原来位置
    public void OnEndDrag(PointerEventData eventData)
    {
        
    }


    public void Reset()
    {
        transform.localPosition = new Vector3(StartX, 0, 0);
        Zhen.rotation = Quaternion.Euler(0, 0, 0);
        Zhen.localPosition = new Vector3(0, 0, 0);
        CurDragRect = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isEnter = false;
    }

    public void SetDragCallback(Action<float,float,bool> drag=null)
    {
        _drag = drag;
    }
}