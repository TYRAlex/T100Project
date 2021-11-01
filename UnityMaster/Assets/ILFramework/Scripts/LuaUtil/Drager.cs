using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LuaFramework
{
    public class Drager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        Vector3 _startPos;
        public bool isCallBack3DPos = true;//是否回调3d坐标
        public int dragType = 0;
        public int index = 0;
        public bool isActived = true;
        public bool canMove = true;
        LuaFunction dragStart = null;
        LuaFunction drag = null;
        LuaFunction dragEnd = null;
        public RectTransform DragRect;
        public Droper[] drops = new Droper[0];

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isActived) return;
            if (dragStart != null)
            {
                if (isCallBack3DPos)
                    dragStart.Call<Vector3, int, int>(this.transform.position, dragType, index);
                else
                    dragStart.Call<Vector2, int, int>(eventData.position, dragType, index);
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
                    drag.Call<Vector3, int, int>(new Vector3(r.x, r.y), dragType, index);
                }
                else
                {
                    var r = RestrictRect(DragRect, globalMousePos.x, globalMousePos.y);
                    drag.Call<Vector2, int, int>(new Vector3(r.x, r.y), dragType, index);
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
            }
            if (dragEnd != null)
            {
                if (isCallBack3DPos)
                    dragEnd.Call<Vector3, int, int, bool>(this.transform.position, dragType, index, isMatch);
                else
                    dragEnd.Call<Vector2, int, int, bool>(eventData.position, dragType, index, isMatch);
            }
        }

        //限制范围
        Vector2 RestrictRect(RectTransform DragRect, float x, float y)
        {
            float rx = Mathf.Min(Mathf.Max(x, 0), DragRect.rect.width);
            float ry = Mathf.Min(Mathf.Max(y, 0), DragRect.rect.height);
            return new Vector2(rx, ry);
        }

        // Use this for initialization
        void Start()
        {
            _startPos = this.transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(GameObject rect)
        {
            DragRect = rect.GetComponent<RectTransform>();
        }

        public void AddDrops(GameObject[] objects)
        {
            List<Droper> l = new List<Droper>();
            foreach (var drop in objects)
            {
                l.Add(drop.GetComponent<Droper>());
            }
            drops = l.ToArray();
        }

        public void SetDragCallback(LuaFunction lstart = null, LuaFunction ldrag = null, LuaFunction lend = null)
        {
            this.dragStart = lstart;
            this.drag = ldrag;
            this.dragEnd = lend;
        }

        public void DoReset()
        {
            this.gameObject.SetActive(true);
            this.transform.localPosition = _startPos;
        }
    }
}
