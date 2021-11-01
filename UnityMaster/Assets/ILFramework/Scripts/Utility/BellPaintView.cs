using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BellPaintView : MonoBehaviour
{
    unitycoder_MobilePaint.MobilePaint mobilePaint;
    Vector2 _lastPoint;
    public Toggle eraserToggle;
    public GameObject painterMgr;

    void Start()
    {
        _PaintCheck();
        _lastPoint = Vector2.zero;
        //Show(false);
    }
    
    void _PaintCheck()
    {
        if (mobilePaint == null)
        {
            mobilePaint = unitycoder_MobilePaint.PaintManager.mobilePaint;
        }
    }

    public void Show(bool isShow)
    {
        if (painterMgr) painterMgr.SetActive(isShow);
    }

    public void ToggleEraserMode()
    {
        _PaintCheck();
        if (eraserToggle.isOn)
        {
            mobilePaint.SetDrawModeEraser();
        }
        else
        {
            mobilePaint.SetDrawModeBrush();
        }
    }

    public void InDragStart(Vector2 pos)
    {
        _PaintCheck();
        mobilePaint.InDragStart(pos);
    }

    public void InDragUpdate(Vector2 pos)
    {
        _PaintCheck();
        LerpPaint(pos);
    }

    //插点
    private void LerpPaint(Vector2 point)
    {
        float _brushLerpSize = 8;
        mobilePaint.InDragUpdate(point);

        if (_lastPoint == Vector2.zero)
        {
            _lastPoint = point;
            return;
        }

        float dis = Vector2.Distance(point, _lastPoint);
        if (dis > _brushLerpSize)
        {
            Vector2 dir = (point - _lastPoint).normalized;
            int num = (int)(dis / _brushLerpSize);
            for (int i = 0; i < num; i++)
            {
                Vector2 newPoint = _lastPoint + dir * (i + 1) * _brushLerpSize;
                mobilePaint.InDragUpdate(newPoint);
            }
        }
        _lastPoint = point;
    }

    public void InDragEnd()
    {
        _PaintCheck();
        _lastPoint = Vector2.zero;
        mobilePaint.InDragEnd();
    }
}
