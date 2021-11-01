using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using unitycoder_MobilePaint;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CoursePiccasoCattlePart1
    {
        enum PAINT_COLOR
        {
            Black = 0, Blue = 1, Red = 2, White = 3
        }
        Color[] paintColor;

        GameObject curGo, uiAni, uiSpine,prePaint, preBtn, preCaptureBtn, tempBrushImg, screenAni, shieldGo;
        GameObject[] colorBtn, brushBtn, checkBtn, capture, uiBrushImg, points;
        Dictionary<int, string[]> uiAniDic;
        Dictionary<int, GameObject[]> uiBrushImgDic;
        MobilePaint drawingPaint;
        int currentSize, captureIndex, preBtnIndex, colorIndex, brushIndex, curCaptureIndex;
        int[] brushSize;
        bool isSave, isMax;
        Color currentColor;
        Vector3 uiAniPos;
        Rect rect;
        Camera drawingCamera;
        MonoBehaviour mono;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            //redo = curTrans.Find("startGame/uiBtn/checkBtn_2/redo_2_0").gameObject;
            //save = curTrans.Find("startGame/uiBtn/checkBtn_2/redo_2_1").gameObject;
            uiAni = curTrans.Find("uiGame/uiAni").gameObject;
            screenAni = curTrans.Find("uiGame/screenAni").gameObject;
            uiSpine = uiAni.transform.Find("uiSpine").gameObject;
            shieldGo = curTrans.Find("shieldGo").gameObject;
            drawingPaint = curTrans.Find("startGame/drawingPaint").GetComponent<MobilePaint>();           
            drawingCamera = curTrans.Find("startGame/drawingCamera").transform.GetComponent<Camera>();

            colorBtn = curTrans.GetChildren(curTrans.Find("uiGame/uiBtn/colorBtn_0").gameObject);
            brushBtn = curTrans.GetChildren(curTrans.Find("uiGame/uiBtn/brushBtn_1").gameObject);
            checkBtn = curTrans.GetChildren(curTrans.Find("uiGame/uiBtn/checkBtn_2").gameObject);
            capture = curTrans.GetChildren(curTrans.Find("uiGame/capture").gameObject);
            uiBrushImg = curTrans.GetChildren(curTrans.Find("uiGame/uiBrushImg").gameObject);
            points = curTrans.GetChildren(curTrans.Find("uiGame/points").gameObject);
            //mobilePaint = curTrans.GetChildren(drawingPaint);
            mono = curGo.GetComponent<MonoBehaviour>();
            uiAniDic = new Dictionary<int, string[]>() { { 0, new string[] { "c_black_ui", "c_blue_ui", "c_red_ui" } },
                                                         { 1, new string[] { "pen_ui", "brush_ui", "eraser_ui" } },
                                                         { 2, new string[] { "cl_ui", "qd_ui"} }};
            uiBrushImgDic = new Dictionary<int, GameObject[]>(){{ 0, curTrans.GetChildren(uiBrushImg[0])},
                                                                { 1, curTrans.GetChildren(uiBrushImg[1])},
                                                                { 2, curTrans.GetChildren(uiBrushImg[2])}};

            paintColor = new Color[] { new Color(36f/255f, 9f/255f, 8f/255f),
                                       new Color(58f/255f, 207f/255f, 255f/255f),
                                       new Color(255f/255f, 67f/255f, 58f/255f),
                                       new Color(255f/255f, 255f/255f, 255f/255f),};
            brushSize = new int[] { 7, 30, 25};
            rect = new Rect(0, 0, 1920, 1080);
            currentSize = 30;
            captureIndex = -1;
            curCaptureIndex = 9;
            colorIndex = 0;
            brushIndex = 0;
            uiSpine.SetActive(true);
            uiAniPos = uiAni.transform.position;
            currentColor = paintColor[(int)PAINT_COLOR.Black];
            isSave = true;
            isMax = false;
            //prePaint = mobilePaint[0].transform.GetChild(0).gameObject;
            //currentPaint = prePaint.GetComponent<MobilePaint>();
            //preCaptureBtn = capture[0];
            curGo.AddComponent<ScreenToolManager>();
            tempBrushImg = uiBrushImgDic[0][0];

            SceneInit();
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.mousePosition.x > points[0].transform.position.x &&
                    Input.mousePosition.x < points[1].transform.position.x &&
                    Input.mousePosition.y > points[0].transform.position.y &&
                    Input.mousePosition.y < points[1].transform.position.y)
                {
                    tempBrushImg.SetActive(true);
                    tempBrushImg.transform.position = Input.mousePosition;
                }
                else
                {
                    tempBrushImg.SetActive(false);
                }
            }
            else
            {
                tempBrushImg.SetActive(false);
            }
        }

        void SceneInit()
        {
            for (int i = 0; i < colorBtn.Length; i++)
            {
                Util.AddBtnClick(colorBtn[i], SetProperties);
            }
            for (int i = 0; i < brushBtn.Length; i++)
            {
                Util.AddBtnClick(brushBtn[i], SetProperties);
            }
            for (int i = 0; i < checkBtn.Length; i++)
            {
                Util.AddBtnClick(checkBtn[i], SetProperties);
            }
            for (int i = 0; i < capture.Length; i++)
            {
                Util.AddBtnClick(capture[i].gameObject, ClickCapture);
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //SpineManager.instance.DoAnimation(uiSpine, uiAniDic[2][1], false);
            //SkeletonGraphic skt = uiSpine.GetComponent<SkeletonGraphic>();
            //skt.material.shader = Shader.Find("Spine/Straight Alpha/Skeleton Fill");            
        }
        
        void SetProperties(GameObject btn)
        {
            SoundManager.instance.sheildGo.SetActive(true);

            string[] str = btn.name.Split('_');
            int inBtn = Convert.ToInt32(str[1]);
            int index = Convert.ToInt32(str[2]);
            btn.SetActive(false);
            if (preBtn != null && preBtnIndex != 2)
            {
                preBtn.SetActive(true);
            }
            uiAni.transform.position = btn.transform.position;
            preBtn = btn;
            preBtnIndex = inBtn;
            SpineManager.instance.DoAnimation(uiSpine, uiAniDic[inBtn][index], false, () =>
            {                
                if (inBtn == 2)
                {
                    uiAni.transform.position = uiAniPos;
                    btn.SetActive(true);
                }
                Debug.Log("uiSpine.activeSelf:" + uiSpine.activeSelf);
            });
            
            if (inBtn == 0)
            {
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 1, null, () =>
                {
                    SoundManager.instance.sheildGo.SetActive(false);
                });
                if (brushIndex != 2)
                {
                    currentColor = paintColor[index];
                    Debug.LogFormat("paintColor:{0},index:{1}", paintColor[index], index);
                    drawingPaint.SetPaintColor(currentColor);
                    colorIndex = index;
                }                              
            }
            else if (inBtn == 1)
            {
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 1, null, () =>
                {
                    SoundManager.instance.sheildGo.SetActive(false);
                });

                currentSize = brushSize[index];
                if (index == 2)
                {
                    //currentColor = paintColor[(int)PAINT_COLOR.White];
                    drawingPaint.SetPaintColor(paintColor[(int)PAINT_COLOR.White]);
                }
                else
                {
                    drawingPaint.SetPaintColor(currentColor);
                }

                currentSize = brushSize[index];
                drawingPaint.SetBrushSize(currentSize);
                brushIndex = index;
            }
            else if (inBtn == 2)
            {
                if (index == 0)
                {
                    SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 2, null, () =>
                    {
                        SoundManager.instance.sheildGo.SetActive(false);
                        screenAni.SetActive(false);
                        shieldGo.SetActive(false);
                        drawingPaint.gameObject.SetActive(true);
                    });
                    drawingPaint.ClearImage();
                }
                else if (index == 1)
                {
                    int drawingIndex;
                    
                    if (!isSave)
                    {
                        drawingIndex = curCaptureIndex;
                    }
                    else
                    {
                        captureIndex++;
                        if (captureIndex < 9)
                        {
                            drawingIndex = captureIndex;
                        }
                        else
                        {
                            drawingIndex = 9;
                            isMax = true;
                        }

                    }                   
                    
                    SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 3, null, () =>
                    {
                        SoundManager.instance.sheildGo.SetActive(false);
                    });                    
                    
                    Image icon = screenAni.GetComponent<Image>();
                    string fileName = Application.persistentDataPath +"/drawingScreen_" + drawingIndex.ToString() + ".png";
                    //CameraCapture(drawingCamera, rect, fileName);
                    //mono.StartCoroutine(LoadImage(fileName, icon));

                    ScreenToolManager.instance.CameraCapture(drawingCamera, rect, fileName);
                    ScreenToolManager.instance.LoadImage(fileName, icon, null, () =>
                    {
                        screenAni.SetActive(true);
                        screenAni.transform.DOScale(0.128f, 0.5f);
                        //screenAni.transform.DORotate(capture[captureIndex].transform.position, 0.5f);
                        screenAni.transform.DOMove(capture[drawingIndex].transform.position, 0.5f).OnComplete(() =>
                        {
                            isSave = true;
                            screenAni.SetActive(false);
                            screenAni.transform.Identity();
                            capture[drawingIndex].SetActive(true);
                            capture[drawingIndex].GetComponent<Image>().sprite = icon.sprite;

                            if (!isMax)
                            {
                                drawingPaint.ClearImage();
                            }
                        });
                    }); 
                }
            }

            if (brushIndex != 2)
            {
                tempBrushImg = uiBrushImgDic[brushIndex][colorIndex];
                curGo.ShowGameObject(uiBrushImg[brushIndex]);
                curGo.ShowGameObject(uiBrushImgDic[brushIndex][colorIndex]);
                tempBrushImg.SetActive(false);
            }
            else
            {
                tempBrushImg = uiBrushImgDic[brushIndex][0];
                curGo.ShowGameObject(uiBrushImg[brushIndex]);
                curGo.ShowGameObject(uiBrushImgDic[brushIndex][0]);
                tempBrushImg.SetActive(false);
            }
        }

        void CleanSave(GameObject btn)
        {

        }

        void ClickCapture(GameObject btn)
        {
            isSave = false;
            shieldGo.SetActive(true);
            SoundManager.instance.sheildGo.SetActive(true);
            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 0, null, () =>
            {
                SoundManager.instance.sheildGo.SetActive(false);
            });
            Image icon = btn.GetComponent<Image>();
            screenAni.GetComponent<Image>().sprite = icon.sprite;
            screenAni.SetActive(true);
            drawingPaint.gameObject.SetActive(false);
            int index = Convert.ToInt32(btn.name);
            curCaptureIndex = index;
            //if (preCaptureBtn != btn)
            //{                
            //    int childCount = prePaint.transform.parent.childCount;
            //    if (childCount > 1)
            //    {
            //        //prePaint.transform.parent.GetChild(1).name = prePaint.name;
            //        GameObject.Destroy(prePaint);
            //    }

            //    //isSave = false;
            //    string[] str = btn.name.Split('_');
            //    int index = Convert.ToInt32(str[0]);
            //    GameObject paintGame = mobilePaint[index].transform.GetChild(0).gameObject;
            //    GameObject copyPaintGame = GameObject.Instantiate(paintGame, mobilePaint[index].transform);
            //    copyPaintGame.SetActive(false);
            //    paintGame.SetActive(true);
            //    curGo.ShowGameObject(mobilePaint[index].gameObject);
            //    currentPaint = paintGame.GetComponent<MobilePaint>();
            //    if (brushIndex != 2)
            //    {
            //        drawingPaint.SetPaintColor(paintColor[colorIndex]);

            //    }
            //    else
            //    {
            //        drawingPaint.SetPaintColor(paintColor[(int)PAINT_COLOR.White]);
            //    }
            //    drawingPaint.SetBrushSize(brushSize[brushIndex]);
            //    captureIndex = index;
            //    prePaint = paintGame;
            //    preCaptureBtn = btn;
            //}            
        }        
    }
}
