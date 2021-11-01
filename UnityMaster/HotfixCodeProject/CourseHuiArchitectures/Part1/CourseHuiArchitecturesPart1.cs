using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseHuiArchitecturesPart1
    {
        enum FINGER_ID
        {
            ZeroFinger = 0, OneFinger = 1, TwoFinger = 2
        }

        GameObject curGo, npc;
        GameObject[] model, gameBtn;
        Camera modelCamera_O, modelCamera_P, bgCamera, captureCamera;

        int modelID, isOneClick;
        bool isOneFirst, isTwoFirst, isThreeClick, isThreeFirst;
        Vector3 iniModelRot, iniModelSca, iniModelPos;

        //two finger
        Vector3 currentScale;
        float beginTouchDistance;
        Touch twoOldTouch0;
        Touch twoOldTouch1;

        //three finger
        Touch threeOldTouch0;
        Touch threeOldTouch1;
        Touch threeOldTouch2;

        float tempScale;
        float deltaMoveX;

        //screenshot
        GameObject gameScene, showImage, showImageContent, showCapture, saveCaptureBtn, showCaptureBtn, captureBtn, content, captureContent, moveScreen, btnSpine, productFactory;
        GameObject[] dragerItem;
        Camera[] captureCameras;
        SpriteRenderer modelSprite;
        RawImage rawImage;
        Vector3 iniBtnPos, startPos;
        Dictionary<int, GameObject> captureDic, contentDic;
        Dictionary<int, Vector2[]> setGridDic;
        GridLayoutGroup gridLayout;
        Rect captureRect;
        int texCount, dragerIndex;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            model = GetChildren(curTrans.Find("model").gameObject);
            gameBtn = GetChildren(curTrans.Find("GameScene/UIPanel/gameBtn/titleBtn").gameObject);            
            modelCamera_O = curTrans.transform.Find("modelCamera_O").gameObject.GetComponent<Camera>();
            modelCamera_P = curTrans.transform.Find("modelCamera_P").gameObject.GetComponent<Camera>();
            bgCamera = curTrans.transform.Find("bgCamera").gameObject.GetComponent<Camera>();
            npc = curTrans.Find("npc").gameObject;
            

            //screenshot            
            gameScene = curTrans.Find("GameScene").gameObject;
            moveScreen = curTrans.Find("GameScene/UIPanel/moveScreen").gameObject;
            captureBtn = curTrans.Find("GameScene/UIPanel/captureBtn").gameObject;
            saveCaptureBtn = gameScene.transform.Find("UIPanel/gameBtn/saveBtn").gameObject;
            showCaptureBtn = captureBtn.transform.Find("showBtn").gameObject;
            content = captureBtn.transform.Find("itemCapture/Viewport/Content").gameObject;
            captureContent = curTrans.Find("GameScene/UIPanel/captureContent").gameObject;
            showImage = curTrans.Find("GameScene/UIPanel/showImage").gameObject;
            showImageContent = showImage.transform.Find("showCapture/Viewport/Content").gameObject;
            showCapture = curTrans.Find("GameScene/UIPanel/showCapture").gameObject;
            btnSpine = curTrans.Find("GameScene/UIPanel/btnSpine").gameObject;
            productFactory = curTrans.Find("GameScene/UIPanel/productFactory").gameObject;
            captureCamera = curTrans.transform.Find("captureCamera").gameObject.GetComponent<Camera>();
            modelSprite = curTrans.transform.Find("captureCamera/modelSprite").GetComponent<SpriteRenderer>();
            rawImage = curTrans.transform.Find("captureCamera/RawImage").GetComponent<RawImage>();
            dragerItem = curTrans.GetChildren(captureContent);
            captureDic = new Dictionary<int, GameObject>();
            contentDic = new Dictionary<int, GameObject>();
            gridLayout = showImageContent.GetComponent<GridLayoutGroup>();
            setGridDic = new Dictionary<int, Vector2[]>() { { 1, new Vector2[] { new Vector2(1200, 1200), new Vector2(0, 0)} },
                                                            { 2, new Vector2[] { new Vector2(800, 800), new Vector2(100, 0)} },
                                                            { 3, new Vector2[] { new Vector2(600, 600), new Vector2(30, 0)} },
                                                            { 4, new Vector2[] { new Vector2(800, 600), new Vector2(100, -100)} },};
            captureCameras = new Camera[] {  bgCamera, modelCamera_O };
            captureRect = new Rect(0, 0, 1920, 1080);
            if (curGo.GetComponent<ScreenToolManager>() == null)
            {
                curGo.AddComponent<ScreenToolManager>();
            }
                    
            
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SoundManager.instance.BgSoundPart2();
                GameInit();
                GameStart();
            });            
        }

        void GameInit()
        {            
            texCount = 0;
            tempScale = 1;
            deltaMoveX = 0;
            npc.SetActive(false);
            texCount = 0;
            dragerIndex = 0;
            isOneClick = 0;
            modelID = 0;

            //iniModelRot = model[modelID].transform.localEulerAngles;
            //iniModelSca = model[modelID].transform.localScale;
            //iniModelPos = model[modelID].transform.localPosition;
            iniModelRot = Vector3.zero;
            iniModelSca = Vector3.one;
            iniModelPos = Vector3.zero;
            iniBtnPos = btnSpine.transform.position;
            startPos = captureContent.transform.GetChild(0).transform.position;
            //captureDic.Clear();
            //contentDic.Clear();
            //captureBtn.transform.localPosition = new Vector3(1107f, 1f, 0);

            for (int i = 0; i < captureContent.transform.childCount; i++)
            {
                GameObject go = captureContent.transform.GetChild(i).gameObject;
                captureDic.Add(i, go);
                ILDrager ilDrager = go.GetComponent<ILDrager>();
                ilDrager.SetDragCallback(StartDrag, null, EndDrag);
            }

           
        }

        void GameStart()
        {
            //SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 0, null, () =>
            //{
            //    SoundManager.instance.BgSoundPart2();
            //});

            Util.AddBtnClick(gameBtn[0], ResortModel);
            Util.AddBtnClick(gameBtn[1], ResortModel);
            Util.AddBtnClick(saveCaptureBtn, SaveCapture);
            Util.AddBtnClick(showCaptureBtn, ShowCapture);
            Util.AddBtnClick(showImage, ClickShowImage);
        }

        void SaveCapture(GameObject button)
        {

            if (contentDic.Count < 4)
            {
                button.SetActive(false);
                btnSpine.transform.position = button.transform.position;
                SoundManager.instance.sheildGo.SetActive(true);
                SpineManager.instance.DoAnimation(btnSpine.transform.GetChild(0).gameObject, "jp", false, () =>
                {
                    btnSpine.transform.position = iniBtnPos;
                    button.SetActive(true);
                });

                texCount++;
                string fileName = Application.persistentDataPath + "/screenshot_" + texCount.ToString() + ".png";
                ScreenToolManager.instance.ScreenCapture(captureRect, fileName, () =>
                {
                    gameScene.SetActive(false);
                }, () =>
                {
                    gameScene.SetActive(true);
                    Image icon = moveScreen.GetComponent<Image>();
                    ScreenToolManager.instance.LoadImage(fileName, icon, null, () =>
                    {
                        Vector3 movePos = startPos - new Vector3(0, contentDic.Count * 120f, 0);

                        if (contentDic.Count == 0)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                            captureBtn.transform.DOMoveX(150f, 0.5f).SetEase(Ease.InExpo).OnComplete(() =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

                                moveScreen.SetActive(true);
                                moveScreen.transform.DOScale(0.08f, 0.5f);
                                moveScreen.transform.DOMove(movePos, 0.5f).OnComplete(() =>
                                {
                                    moveScreen.SetActive(false);
                                    moveScreen.transform.Identity();
                                    GameObject obj = captureContent.transform.GetChild(0).gameObject;
                                    obj.transform.SetParent(content.transform);
                                    obj.GetComponent<Image>().sprite = moveScreen.GetComponent<Image>().sprite;

                                    string[] str = obj.name.Split('_');
                                    int index = Convert.ToInt32(str[1]);
                                    captureDic.Remove(index);
                                    contentDic.Add(index, obj);
                                    SoundManager.instance.sheildGo.SetActive(false);
                                });
                            });
                        }
                        else
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

                            moveScreen.SetActive(true);
                            moveScreen.transform.DOScale(0.08f, 0.5f);
                            moveScreen.transform.DOMove(movePos, 0.5f).OnComplete(() =>
                            {
                                moveScreen.SetActive(false);
                                moveScreen.transform.Identity();
                                GameObject obj = captureContent.transform.GetChild(0).gameObject;
                                obj.transform.SetParent(content.transform);
                                obj.GetComponent<Image>().sprite = moveScreen.GetComponent<Image>().sprite;

                                string[] str = obj.name.Split('_');
                                int index = Convert.ToInt32(str[1]);
                                captureDic.Remove(index);
                                contentDic.Add(index, obj);
                                SoundManager.instance.sheildGo.SetActive(false);
                            });
                        }
                    });
                });
                //Debug.Log("rawImage:" + rawImage);
                //RenderTexture t1 = rawImage.texture as RenderTexture;
                //Texture t = t1 as Texture;
                //Debug.Log("t:" + t.name);
                //Sprite s = Sprite.Create(t as Texture2D, new Rect(0, 0, t.width, t.height),
                //                         Vector2.zero, 1f);
                //modelSprite.sprite = s;
                //modelSprite.gameObject.SetActive(true);
                //ScreenToolManager.instance.CameraCapture(captureCamera, captureRect, fileName);
                //modelSprite.gameObject.SetActive(false);
                //Debug.Log("dddd");
                ////ScreenToolManager.instance.CameraCapture(modelCamera_O, captureRect, fileName);
                //Image icon = moveScreen.GetComponent<Image>();
                //ScreenToolManager.instance.LoadImage(fileName, icon);

               
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            }           
        }

        void ShowCapture(GameObject button)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            int count = contentDic.Count;
            if (count > 0)
            {
                button.SetActive(false);
                btnSpine.transform.position = button.transform.position;
                SpineManager.instance.DoAnimation(btnSpine.transform.GetChild(0).gameObject, "jd", false, () =>
                {
                    btnSpine.transform.position = iniBtnPos;
                    button.SetActive(true);

                    showImage.SetActive(true);
                    gridLayout.cellSize = setGridDic[count][0];
                    gridLayout.spacing = setGridDic[count][1];
                    if (count == 4)
                    {
                        gridLayout.childAlignment = TextAnchor.UpperCenter;
                    }
                    else
                    {
                        gridLayout.childAlignment = TextAnchor.MiddleCenter;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        GameObject obj = showCapture.transform.GetChild(0).gameObject;
                        obj.transform.SetParent(showImageContent.transform);
                        obj.GetComponent<Image>().sprite = content.transform.GetChild(i).GetComponent<Image>().sprite;                        
                    }
                });                
            }    
        }

        void ClickShowImage(GameObject button)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

            button.SetActive(false);
            int showCount = contentDic.Count;
            for (int i = 0; i < showCount; i++)
            {
                GameObject obj = showImageContent.transform.GetChild(0).gameObject;
                obj.transform.SetParent(showCapture.transform);
            }
        }

        void StartDrag(Vector3 position, int type, int id)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
            dragerIndex = dragerItem[id].transform.GetSiblingIndex();
            dragerItem[id].transform.SetParent(productFactory.transform);
            //dragerItem[id].transform.DOScale(1.2f, 0.1f);
        }

        void EndDrag(Vector3 position, int type, int id, bool isMatch)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
            if (isMatch)
            {
                dragerItem[id].transform.SetParent(content.transform);
                dragerItem[id].transform.SetSiblingIndex(dragerIndex);
                //dragerItem[id].transform.DOScale(1f, 0.1f);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);

                dragerItem[id].transform.SetParent(captureContent.transform);
                contentDic.Remove(id);
                captureDic.Add(id, dragerItem[id]);

                if (contentDic.Count == 0)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    captureBtn.transform.DOMoveX(-150f, 0.5f).SetEase(Ease.InExpo);
                }
            }
        }

        void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount == 0)
                {
                    isTwoFirst = false;
                    isOneFirst = false;
                    isThreeClick = false;
                }
                else if (Input.touchCount == 1)
                {
                    isTwoFirst = false;
                    isThreeClick = false;
                    isThreeFirst = false;
                    MouseOneFinger();
                }
                else if (Input.touchCount == 2)
                {

                    isOneFirst = false;
                    isThreeClick = false;
                    isThreeFirst = false;
                    MouseTwoFinger();
                }
            }
            
            //else if (Input.touchCount == 3)
            //{
            //    isOneFirst = false;
            //    isTwoFirst = false;
            //    MouseThreeFinger();
            //}

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LeftMouseDown();
                }
                else if (Input.GetMouseButton(0))
                {
                    LeftMouse();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    LeftMouseUp();
                }
                else if (Input.GetMouseButton(2))
                {
                    MiddleMouse();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    ScrollWheelMouse();
                }
            }            
        }

        void LeftMouseDown()
        {
            Debug.Log("LeftMouse");
            deltaMoveX = Input.mousePosition.x;
            Debug.Log("deltaMoveX:" + deltaMoveX);
            Ray r = modelCamera_O.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 50));
            //Ray r1 = captureCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 50));
            RaycastHit hitInfo;
            if (Physics.Raycast(r, out hitInfo, 100, 1 << LayerMask.NameToLayer("Water")))
            {
                isOneClick = 0;
            }
            else if (Physics.Raycast(r, out hitInfo, 100, 1 << LayerMask.NameToLayer("UI")))
            {
                isOneClick = 1;
            }
            else
            {
                isOneClick = 2;
            }
            Debug.Log("isOneClick" + isOneClick);            
        }
                
        void LeftMouse()
        {
            //    float moveX = deltaMoveX - Input.mousePosition.x;
            //    Debug.Log("Input.mousePosition.x:" + Input.mousePosition.x);
            if (isOneClick == 1)
            {
                modelCamera_O.orthographic = false;
                modelCamera_O.fieldOfView = 27;

                Vector2 deltaPos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                model[modelID].transform.Rotate(-Vector3.up * deltaPos.x * 3f, Space.World);
                model[modelID].transform.Rotate(-Vector3.left * deltaPos.y * 3f, Space.World);
            }           
            //else 
            //{
            //    if (moveX > 100f || moveX < -100f)
            //    {
            //        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

            //        gameBtn[modelID].SetActive(false);
            //        model[modelID].SetActive(false);

            //        modelID++;
            //        if (modelID > 1)
            //        {
            //            modelID = 0;
            //        }

            //        if (model[modelID].transform.localEulerAngles != (iniModelRot + new Vector3(0, 90f, 0)))
            //        {
            //            modelCamera_O.orthographic = false;
            //            modelCamera_O.fieldOfView = 27;
            //        }

            //        gameBtn[modelID].SetActive(true);
            //        model[modelID].SetActive(true);
            //        //ResortModel(model[modelID]);
            //    }
            //}
        }

        void LeftMouseUp()
        {
            Debug.Log("deltaMoveX:" + deltaMoveX);
            float moveX = deltaMoveX - Input.mousePosition.x;
           
            if (isOneClick == 2)
            {
                if (moveX > 100f || moveX < -100f)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

                    gameBtn[modelID].SetActive(false);
                    model[modelID].SetActive(false);

                    modelID++;
                    if (modelID > 1)
                    {
                        modelID = 0;
                    }

                    if (model[modelID].transform.localEulerAngles != (iniModelRot + new Vector3(0, 90f, 0)))
                    {
                        modelCamera_O.orthographic = false;
                        modelCamera_O.fieldOfView = 27;
                    }

                    gameBtn[modelID].SetActive(true);
                    model[modelID].SetActive(true);
                    //ResortModel(model[modelID]);
                }
            }
        }

        void MiddleMouse()
        {
            //Debug.Log("MiddleMouse");
            model[modelID].transform.Translate(Input.GetAxis("Mouse X") * 0.5f, Input.GetAxis("Mouse Y") * 0.5f, 0, Space.World);
        }
        
        void ScrollWheelMouse()
        {
            //Debug.Log("ScrollWheelMouse" + Input.GetAxis("Mouse ScrollWheel"));
            
            tempScale += Input.GetAxis("Mouse ScrollWheel");

            //Debug.Log("tmpScale" + tempScale);
            tempScale = Mathf.Clamp(tempScale, 0.25f, 3f);
            model[modelID].transform.localScale = Vector3.one * tempScale;
        }

        void MouseOneFinger()
        {
            //Debug.Log("OneFinger");
            Touch oneFingerTouch;
            oneFingerTouch = Input.GetTouch(0);

            if (oneFingerTouch.phase == TouchPhase.Began && !isOneFirst)
            {   
                isOneFirst = true;
                Ray r = modelCamera_O.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 50));
                RaycastHit hitInfo;
                if (Physics.Raycast(r, out hitInfo, 100, 1 << LayerMask.NameToLayer("Water")))
                {
                    isOneClick = 0;
                }
                else if (Physics.Raycast(r, out hitInfo, 100, 1 << LayerMask.NameToLayer("UI")))
                {
                    isOneClick = 1;
                }
                else
                {
                    isOneClick = 2;
                }
                //Debug.Log("isOneClick" + isOneClick);
            }

            if (oneFingerTouch.phase == TouchPhase.Moved && isOneClick == 1)
            {
                modelCamera_O.orthographic = false;
                modelCamera_O.fieldOfView = 27;

                Vector2 deltaPos = oneFingerTouch.deltaPosition;
                model[modelID].transform.Rotate(-Vector3.up * deltaPos.x * 0.2f, Space.World);
                model[modelID].transform.Rotate(-Vector3.left * deltaPos.y * 0.2f, Space.World);
            }
            else if (oneFingerTouch.phase == TouchPhase.Ended && isOneClick == 2)
            {
                if (oneFingerTouch.deltaPosition.x > 5f || oneFingerTouch.deltaPosition.x < -5f)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

                    gameBtn[modelID].SetActive(false);
                    model[modelID].SetActive(false);

                    modelID++;
                    if (modelID > 1)
                    {
                        modelID = 0;
                    }

                    if (model[modelID].transform.localEulerAngles != (iniModelRot + new Vector3(0, 90f, 0)))
                    {
                        modelCamera_O.orthographic = false;
                        modelCamera_O.fieldOfView = 27;
                    }
                    
                    gameBtn[modelID].SetActive(true);
                    model[modelID].SetActive(true);
                    //ResortModel(model[modelID]);
                }
            }
        }

        void MouseTwoFinger()
        {
            //Debug.LogWarning("TwoFinger");
            if (!isTwoFirst)
            {
                twoOldTouch0 = Input.GetTouch(0);
                twoOldTouch1 = Input.GetTouch(1);
                beginTouchDistance = Vector2.Distance(twoOldTouch0.position, twoOldTouch1.position);
                currentScale = model[modelID].transform.localScale;
                isTwoFirst = true;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                //modelCamera_O.orthographic = false;
                //modelCamera_O.fieldOfView = 27;

                //Debug.LogWarning("TwoFinger1");
                Vector2 vector1 = Input.GetTouch(0).position - twoOldTouch0.position;
                Vector2 vector2 = Input.GetTouch(1).position - twoOldTouch1.position;
                float dir = Vector2.Dot(vector1.normalized, vector2.normalized);
                if (dir <= 1 && dir >= 0)
                {
                    //Debug.LogWarning("TwoFinger2");
                    model[modelID].transform.Translate(Input.GetTouch(0).deltaPosition.x * 0.01f, Input.GetTouch(0).deltaPosition.y * 0.01f, 0, Space.World);
                }
                else if (dir >= -1 && dir < 0)
                {
                    //Debug.LogWarning("TwoFinger3");
                    float currentTouchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    float tmpScale = (currentTouchDistance / beginTouchDistance) * 0.5f - 1f;
                    tmpScale += currentScale.x;
                    tmpScale = Mathf.Clamp(tmpScale, 0.25f, 3f);
                    model[modelID].transform.localScale = Vector3.one * tmpScale;
                }
            }
            //Debug.LogWarning("TwoFinger4");
        }

        void MouseThreeFinger()
        {
            //Debug.Log("ThreeFinger");
            if (Input.GetTouch(0).phase == TouchPhase.Began &&
                Input.GetTouch(1).phase == TouchPhase.Began &&
                Input.GetTouch(2).phase == TouchPhase.Began &&
                !isThreeClick)
            {
                modelCamera_O.orthographic = true;
                modelCamera_O.orthographicSize = 6;

                if (!isThreeFirst)
                {
                    ResortModel(model[modelID]);
                    isThreeFirst = true;
                }
                isThreeClick = true;

                threeOldTouch0 = Input.GetTouch(0);
                threeOldTouch1 = Input.GetTouch(1);
                threeOldTouch2 = Input.GetTouch(2);
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended &&
                Input.GetTouch(1).phase == TouchPhase.Ended &&
                Input.GetTouch(2).phase == TouchPhase.Ended)
            {
                Vector2 vector1 = Input.GetTouch(0).position - threeOldTouch0.position;
                Vector2 vector2 = Input.GetTouch(1).position - threeOldTouch1.position;
                Vector2 vector3 = Input.GetTouch(2).position - threeOldTouch2.position;

                float dir1 = Vector2.Dot(vector1.normalized, vector2.normalized);
                float dir2 = Vector2.Dot(vector2.normalized, vector3.normalized);
                float dir3 = Vector2.Dot(vector3.normalized, vector1.normalized);

                if (dir1 >= 0 && dir1 <= 1 &&
                    dir2 >= 0 && dir2 <= 1 &&
                    dir3 >= 0 && dir3 <= 1)
                {
                    if (vector1.magnitude >= 3 && vector2.magnitude >= 3 && vector3.magnitude >= 3)
                    {
                        if (vector1.y < 0 && Mathf.Abs(vector1.x) < 3f)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                            model[modelID].transform.Rotate(Vector3.left * 90f, Space.World);
                        }
                        else if (vector1.y > 0 && Mathf.Abs(vector1.x) < 3f)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                            model[modelID].transform.Rotate(Vector3.right * 90f, Space.World);
                        }
                        else if (vector1.x > 0 && Mathf.Abs(vector1.y) < 3f)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                            model[modelID].transform.Rotate(Vector3.up * 90f, Space.World);
                        }
                        else if (vector1.x < 0 && Mathf.Abs(vector1.y) < 3f)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                            model[modelID].transform.Rotate(Vector3.down * 90f, Space.World);
                        }
                    }
                }

            }
        }

        void ResortModel(GameObject obj)
        {
            modelCamera_O.orthographic = true;
            modelCamera_O.orthographicSize = 6;

            //string[] str = obj.name.Split('_');
            //int index = Convert.ToInt32(str[1]);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            model[modelID].transform.localEulerAngles = iniModelRot + new Vector3(0, 90f, 0);
            model[modelID].transform.localScale = iniModelSca;
            model[modelID].transform.localPosition = iniModelPos;
            tempScale = iniModelSca.x;
        }

        GameObject[] GetChildren(GameObject father)
        {
            GameObject[] children = new GameObject[father.transform.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = father.transform.GetChild(i).gameObject;
            }
            return children;
        }
        
    }
}
