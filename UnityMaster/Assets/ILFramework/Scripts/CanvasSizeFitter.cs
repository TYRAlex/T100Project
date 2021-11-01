using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSizeFitter : MonoBehaviour
{
    public Canvas Canvas;

    public  int StageHeight = 1080;
    public  int StageWidth = 1920;

    /// <summary>
    /// 整体缩放率
    /// </summary>
    public  float ScaleFactor = 1;

    /// <summary>
    /// Canvas缩放率
    /// </summary>
    public  float CanvasScaleFactor;

    public  float ScaleX;
    public  float ScaleY;
    public Vector2 CurBackgroundV2;
   
    public Action Action;
    private void Start()
    {
        ScaleX = StageWidth / (float)Screen.width;
        ScaleY = StageHeight / (float)Screen.height;

        ScaleFactor = Mathf.Min(ScaleX, ScaleY);
        ScaleFactor *= Canvas.scaleFactor;
        CanvasScaleFactor = Canvas.scaleFactor;

        RectTransform rect = transform.GetComponent<RectTransform>();

        Vector2 background = new Vector2(StageWidth / ScaleFactor, StageHeight / ScaleFactor);
        CurBackgroundV2 = background;
       
        rect.sizeDelta = new Vector2(background.x, background.y);
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
      
        Action?.Invoke();
    }

   
}
   
