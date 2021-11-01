using ILFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectMoveManager : Manager<ScrollRectMoveManager>, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    public List<float> points;
    public List<GameObject> grids;
    private int count;
    float startPosx;
    bool isDrag = true;
    float startTime;
    float smooting = 0.2f;
    float moveX = 0.1f;
    float targethorizontal;
    public int index = 0;
    Action speak;
   
    public void CreateManager(Transform parent,Action _speak = null)
    {
        scrollRect = parent.GetComponent<ScrollRect>();
        points = new List<float>();
        grids = new List<GameObject>();
        points.Add(0);
        count = scrollRect.transform.Find("Viewport/Content").childCount;
        for (int i = 1; i < count; i++)
        {
            points.Add((float)i / (count - 1.0f));
        }
        for (int i = 0; i < count; i++)
        {
            grids.Add(scrollRect.transform.Find("Viewport/Content").GetChild(i).gameObject);
        }
        speak = _speak;
        speak?.Invoke();
    }
    void FixedUpdate()
    {
        if(!isDrag)
        {
            startTime += Time.deltaTime;
            float t = startTime * smooting;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targethorizontal, t);

            if( Mathf.Abs(grids[index].transform.position.x - Screen.width/2) < 0.1)
            {
                isDrag = true;
                scrollRect.horizontalNormalizedPosition = targethorizontal;
                startTime = 0;
                speak?.Invoke();
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        startTime = 0;
        startPosx = scrollRect.horizontalNormalizedPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float Posx = scrollRect.horizontalNormalizedPosition;
        if(Posx > 1)
        {
            Posx = 1;
        }
        else if(Posx < 0)
        {
            Posx = 0;
        }
        if (startPosx > 1)
        {
            startPosx = 1;
        }
        else if (startPosx < 0)
        {
            startPosx = 0;
        }
        float distance = startPosx - Posx;
        if(Mathf.Abs(distance/ points[1]) < moveX)
        {
            isDrag = false;
            targethorizontal = points[index];
        }
        else
        {
            if (distance < 0)
            {
                if (index != points.Count - 1)
                {
                    isDrag = false;
                    index++;
                    targethorizontal = points[index];
                }
            }
            else if (distance > 0)
            {
                if (index != 0)
                {
                    isDrag = false;
                    index--;
                    targethorizontal = points[index];
                }
            }
        }
    }
}
