using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace OneID
{
    public class OneIDDrag : MonoBehaviour , IDragHandler
    {
        public RectTransform DragRect;
        public bool IsActive = false;

        public void OnDrag(PointerEventData eventData)
        {
            if(!IsActive) return;
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(DragRect, eventData.position,
                eventData.pressEventCamera, out globalMousePos))
            {
                var r = RestrictRect(DragRect, globalMousePos.x, globalMousePos.y);
                this.transform.position = new Vector3(r.x, this.transform.position.y);
            }
        }
        
        Vector2 RestrictRect(RectTransform DragRect, float x, float y)
        {
            RectTransform rectTransform = this.transform as RectTransform;
            // 最小x坐标 = 容器当前x坐标 - 容器轴心距离左边界的距离 + UI轴心距离左边界的距离
            float minX = DragRect.position.x
                         - DragRect.pivot.x * DragRect.rect.width
                         + rectTransform.rect.width * rectTransform.pivot.x;

            // 最大x坐标 = 容器当前x坐标 + 容器轴心距离右边界的距离 - UI轴心距离右边界的距离
            float maxX = DragRect.position.x
                         + (1 - DragRect.pivot.x) * DragRect.rect.width
                         - rectTransform.rect.width * (1 - rectTransform.pivot.x);

            // 最小y坐标 = 容器当前y坐标 - 容器轴心距离底边的距离 + UI轴心距离底边的距离
            float minY = DragRect.position.y
                         - DragRect.pivot.y * DragRect.rect.height
                         + rectTransform.rect.height * rectTransform.pivot.y;

            // 最大y坐标 = 容器当前x坐标 + 容器轴心距离顶边的距离 - UI轴心距离顶边的距离
            float maxY = DragRect.position.y
                         + (1 - DragRect.pivot.y) * DragRect.rect.height
                         - rectTransform.rect.height * (1 - rectTransform.pivot.y);

            float rx = Mathf.Clamp(x, minX, maxX);
            float ry = Mathf.Clamp(y, minY, maxY);

            //float rx = Mathf.Min(Mathf.Max(x, 0), DragRect.rect.width);
            //float ry = Mathf.Min(Mathf.Max(y, 0), DragRect.rect.height);
            return new Vector2(rx, ry);
        }
    }
}