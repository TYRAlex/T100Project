using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ILFramework
{
    public class ILDrager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerClickHandler
    {
        Vector3 _startPos;
        public bool isCallBack3DPos = true;//是否回调3d坐标
        public int dragType = 0;
        public int index = 0;
        public bool isActived = true;
        public bool canMove = true;
        public bool isChange = false;
        Action<Vector3, int, int> dragStart = null;
        Action<Vector3, int, int> drag = null;
        Action<Vector3, int, int, bool> dragEnd = null;

        Action<int> onClick = null;

        public RectTransform DragRect;
        public ILDroper[] drops = new ILDroper[0];
        public ILDroper [] failDrops = new ILDroper[0];
        //public override void Inited()
        //{
        //    base.Inited();
        //    RootPackage.PackageDomain.DelegateManager.RegisterMethodDelegate<Vector3, int, int>();
        //    RootPackage.PackageDomain.DelegateManager.RegisterMethodDelegate<Vector3, int, int, bool>();
        //}

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isActived) return;
            if (dragStart != null)
            {
                if (isCallBack3DPos)
                    dragStart.Invoke(transform.position, dragType, index);
                else
                    dragStart.Invoke(eventData.position, dragType, index);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isActived) return;
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(DragRect, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                var r = RestrictRect(DragRect, globalMousePos.x, globalMousePos.y);
                if (canMove) this.transform.position = new Vector3(r.x, r.y);
            }
            //显示可放入特效
            for (int i = 0; i < drops.Length; i++)
            {
                var _drop = drops[i];
                if (RectTransformUtility.RectangleContainsScreenPoint(_drop.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera))
                {
                    _drop.showEffect(true, dragType);
                }
                else
                {
                    _drop.showEffect(false);
                }
            }
            if (drag != null)
            {
                if (isCallBack3DPos)
                {
                    var r = RestrictRect(DragRect, globalMousePos.x, globalMousePos.y);
                    drag.Invoke(new Vector3(r.x, r.y), dragType, index);
                }
                else
                {
                    var r = RestrictRect(DragRect, globalMousePos.x, globalMousePos.y);
                    drag.Invoke(new Vector3(r.x, r.y), dragType, index);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isActived) return;
            bool isMatch = false;
            if (canMove)
            {
                for (int i = 0; i < drops.Length; i++)
                {
                    var _drop = drops[i];                 
                    if (RectTransformUtility.RectangleContainsScreenPoint(_drop.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera))
                    {                     
                        if (_drop.isActived && _drop.transform.childCount <= 1)
                        {                        
                            isMatch = _drop.doAfter(dragType);
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
            if(isChange)
            {
                isMatch = false;
                for (int i = 0; i < drops.Length; i++)
                {
                    var _drop = drops[i];
                    if (RectTransformUtility.RectangleContainsScreenPoint(_drop.GetComponent<RectTransform>(), this.transform.position, eventData.pressEventCamera))
                    {
                        if (_drop.isActived && _drop.transform.childCount <= 1)
                        {
                            isMatch = _drop.doAfter(dragType);
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
                if (isCallBack3DPos)
                    dragEnd.Invoke(transform.position, dragType, index, isMatch);
                else
                    dragEnd.Invoke(eventData.position, dragType, index, isMatch);
            }
        }

        //限制范围
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

        // Use this for initialization
        void Awake()
        {
            _startPos = this.transform.localPosition;
        }

        public void Init(GameObject rect)
        {
            DragRect = rect.GetComponent<RectTransform>();
        }

        public void AddDrops(GameObject[] objects)
        {
            List<ILDroper> l = new List<ILDroper>();
            foreach (var drop in objects)
            {
                l.Add(drop.GetComponent<ILDroper>());
            }
            drops = l.ToArray();
        }

        public void SetDragCallback(Action<Vector3, int, int> lstart = null, Action<Vector3, int, int> ldrag = null, Action<Vector3, int, int, bool> lend = null,Action<int> onClickIcon=null)
        {
            
            dragStart = lstart;
            drag = ldrag;
            dragEnd = lend;
            onClick = onClickIcon;
        }

        public void DoReset()
        {
            this.gameObject.SetActive(true);
            this.transform.localPosition = _startPos;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onClick!=null)            
                onClick.Invoke(index);
            
        }
    }
}
