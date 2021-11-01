using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseBusySpiderPart2
    {
        GameObject curGo;
        GameObject npc, spider, spiderSpine, line, ring, brush;
        LineRenderer lineRenderer;
        Color paintColor;
        Texture2D texture;
        Slider slider;
        Text text;
        float paintSize;
        int posCount;
        int voiceIndex;

        //screenshot
        GameObject showImage, deleteBtn, addBtn, screen, captureBtn, content, captureContent, tempCapture, moveScreen;
        Vector3 iniPos, iniRot, iniSca, startPos;
        Dictionary<int,GameObject> captureDic, contentDic;
        Dictionary<GameObject, string> pathDic;
        Camera captureCamera;
        Rect captureRect;
        bool isUp, isClickCapture, isFirstShow;
        int texCount;
        string capturePath;
        MonoBehaviour mono;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            npc = curTrans.Find("npc").gameObject;
            spider = curTrans.Find("gameScene/spider").gameObject;
            spiderSpine = spider.transform.Find("spiderSpine").gameObject;
            line = spider.transform.Find("lineRender").gameObject;
            texture = curTrans.Find("UIPanel/setProperties/Color/color").GetComponent<RawImage>().texture as Texture2D;
            slider = curTrans.Find("UIPanel/setProperties/Slider").GetComponent<Slider>();
            text = slider.transform.Find("Text").GetComponent<Text>();
            ring = curTrans.Find("UIPanel/setProperties/Color/ring").gameObject;
            lineRenderer = line.GetComponent<LineRenderer>();
            brush = curTrans.Find("UIPanel/setColor/BrushColor/BrushColor1").gameObject;
            paintSize = 7;
            voiceIndex = -1;
            //paintColor = Color.white;

            //screenshot
            captureCamera = curTrans.Find("captureCamera").GetComponent<Camera>();
            showImage = curTrans.Find("UIPanel/showImage").gameObject;
            screen = showImage.transform.Find("screen").gameObject;
            deleteBtn = screen.transform.Find("deleteBtn").gameObject;
            addBtn = screen.transform.Find("addBtn").gameObject;
            captureBtn = curTrans.Find("UIPanel/captureBtn").gameObject;
            content = captureBtn.transform.Find("Scroll View/Viewport/Content").gameObject;
            captureContent = curTrans.Find("UIPanel/captureContent").gameObject;
            moveScreen = curTrans.Find("UIPanel/moveScreen").gameObject;
            captureDic = new Dictionary<int, GameObject>();
            contentDic = new Dictionary<int, GameObject>();
            pathDic = new Dictionary<GameObject, string>();
            //captureRect = (curTrans.Find("gameScene/bg").transform as RectTransform).rect;
            captureRect = (curGo.transform as RectTransform).rect;
            mono = curTrans.GetComponent<MonoBehaviour>();            

            SceneInit();
        }

        void SceneInit()
        {
            isUp = true;
            isFirstShow = false;
            texCount = 0;
            voiceIndex = 0;
            spiderSpine.SetActive(true);
            iniPos = screen.transform.position;
            iniRot = screen.transform.localEulerAngles;
            iniSca = screen.transform.localScale;
            startPos = captureContent.transform.GetChild(0).transform.position;
            for (int i = 0; i < captureContent.transform.childCount; i++)
            {
                GameObject go = captureContent.transform.GetChild(i).gameObject;
                captureDic.Add(i, go);
                Util.AddBtnClick(go, ClickCapture);
            }

            StartGame();
        }

        void StartGame()
        {
            
            //SoundManager.instance.sheildGo.SetActive(true);
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                //SoundManager.instance.sheildGo.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                Util.AddBtnClick(captureBtn, ClickCaptureBtn);
                Util.AddBtnClick(showImage, ClickCaptureBtn);
                spider.transform.Find("hiImage").gameObject.SetActive(false);
            });
            SpineManager.instance.DoAnimation(spiderSpine, "idle", true);
            ILDrager spiderDrager = spider.GetComponent<ILDrager>();
            spiderDrager.SetDragCallback(StartDrag, Drag, EndDrag);
            //ILDrager ringDrager = ring.GetComponent<ILDrager>();
            //ringDrager.SetDragCallback(RingStartDrag, RingDrag, RingEndDrag);
            slider.onValueChanged.AddListener((float value) => ChangeSize(value));

            lineRenderer.numCapVertices = 2;
            lineRenderer.numCornerVertices = 2;
            posCount = 1;
        }

        void ClickCaptureBtn(GameObject btn)
        {
            SoundManager.instance.sheildGo.SetActive(true);
            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 2, null, () =>
            {
                SoundManager.instance.sheildGo.SetActive(false);
            });
            
            if (isUp)
            {
                isUp = false;
                captureBtn.transform.DOMoveY(976, 0.5f).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    showImage.SetActive(true);
                    //screen.SetActive(false);
                });                
            }
            else
            {
                isUp = true;
                if (isFirstShow)
                {
                    File.Delete(capturePath);
                    isFirstShow = false;
                }
                screen.SetActive(false);
                captureBtn.transform.DOMoveY(1135, 0.5f).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    showImage.SetActive(false);                    
                });
            }
            Debug.Log("sheildGo" + SoundManager.instance.sheildGo.activeSelf);
        }
       
        void ChangeSize(float value)
        {
            paintSize = value * 0.5f;
            text.text = value.ToString();
        }

        void StartDrag(Vector3 pos, int type, int id)
        {
            paintColor = brush.GetComponent<Image>().color;
            SpineManager.instance.DoAnimation(spiderSpine, "walk", true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            lineRenderer.startColor = paintColor;
            lineRenderer.endColor = paintColor;
            lineRenderer.startWidth = paintSize;
            lineRenderer.endWidth = paintSize;
            posCount = 1;
        }

        void Drag(Vector3 pos, int type, int id)
        {
            lineRenderer.positionCount = posCount;
            lineRenderer.SetPosition(posCount - 1, line.transform.position);
            posCount++;
        }

        void EndDrag(Vector3 pos, int type, int id, bool isMatch)
        {
            voiceIndex++;
            if (voiceIndex > 3)
            {
                voiceIndex = 1;
            }

            //SoundManager.instance.sheildGo.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, voiceIndex, null, () =>
            {
                //SoundManager.instance.sheildGo.SetActive(false);
                ShowCapture();
            });
            SpineManager.instance.DoAnimation(spiderSpine, "idle", true);                     
        }

        void ShowCapture()
        {
            if (content.transform.childCount < 8)
            {
                texCount++;
                isClickCapture = false;
                isFirstShow = true;
                string path = Application.persistentDataPath + "/ScreenShot_" + texCount.ToString() + ".png";
                capturePath = path;
                Debug.Log("screenPath:" + path);

                CaptureScreen(captureCamera, captureRect, path);
                ClickCaptureBtn(captureBtn);
                screen.SetActive(true);
                mono.StartCoroutine(LoadImage(path, screen));
                Util.AddBtnClick(addBtn, AddCapture);
                Util.AddBtnClick(deleteBtn, DeleteCapture);

                //mono.StartCoroutine(CaptureScreen(captureRect, path, () =>
                //{
                //    captureBtn.SetActive(true);
                //    ClickCaptureBtn(captureBtn);
                //    screen.SetActive(true);

                //    mono.StartCoroutine(LoadImage(path, screen));
                //    //mono.StartCoroutine(LoadImageWWW(path, screen));
                //    Util.AddBtnClick(addBtn, AddCapture);
                //    Util.AddBtnClick(deleteBtn, DeleteCapture);
                //}));
            }
            else
            {
                ClickCaptureBtn(captureBtn);
                screen.SetActive(false);
            }
           
        }

        void AddCapture(GameObject btn)
        {
            isFirstShow = false;
            SoundManager.instance.sheildGo.SetActive(true);
            if (captureDic.Count > 0 && !isClickCapture)
            {
                screen.SetActive(false);
                moveScreen.SetActive(true);
                GameObject obj = captureContent.transform.GetChild(0).gameObject;
                obj.transform.SetParent(content.transform);
                Sprite sprite = screen.GetComponent<Image>().sprite;
                obj.GetComponent<Image>().sprite = sprite;
                moveScreen.GetComponent<Image>().sprite = sprite;
                Debug.Log("contentDic.Count:" + contentDic.Count);
                Vector3 movePos = startPos + new Vector3(contentDic.Count * 169f, 0, 0);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                moveScreen.transform.DOScale(0.08f, 0.5f);
                moveScreen.transform.DOMove(movePos, 0.5f).OnComplete(() =>
                {
                    moveScreen.SetActive(false);
                    moveScreen.transform.position = iniPos;
                    //moveScreen.transform.localEulerAngles = iniRot;
                    moveScreen.transform.localScale = iniSca;
                    obj.SetActive(true);
                    SoundManager.instance.sheildGo.SetActive(false);
                });

                string[] str = obj.name.Split('_');
                int index = Convert.ToInt32(str[1]);
                captureDic.Remove(index);
                contentDic.Add(index, obj);
                pathDic.Add(obj, capturePath);
            }
            else
            {
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 3, null, () =>
                {
                    SoundManager.instance.sheildGo.SetActive(false);
                });
            }
        }

        void DeleteCapture(GameObject btn)
        {
            screen.SetActive(false);
            isFirstShow = false;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            if (isClickCapture)
            {
                GameObject obj = tempCapture;
                obj.SetActive(false);
                obj.transform.SetParent(captureContent.transform);

                string[] str = obj.name.Split('_');
                int index = Convert.ToInt32(str[1]);
                contentDic.Remove(index);
                captureDic.Add(index, obj);
                File.Delete(pathDic[obj]);
                pathDic.Remove(obj);
            }
            else
            {
                File.Delete(capturePath);
            }
            
        }
        
        void ClickCapture(GameObject btn)
        {
            SoundManager.instance.sheildGo.SetActive(true);
            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 4, null, () =>
            {
                SoundManager.instance.sheildGo.SetActive(false);
            });
            if (isFirstShow)
            {
                File.Delete(capturePath);
                isFirstShow = false;
            }            
            isClickCapture = true;
            tempCapture = btn;
            screen.SetActive(true);
            Sprite sprite = btn.GetComponent<Image>().sprite;
            screen.GetComponent<Image>().sprite = sprite;
        }

        //void CaptureScreen()
        //{
        //    string filename = Application.persistentDataPath + "/ScreenShot_" + texCount.ToString() + ".png";
        //    ScreenCapture.CaptureScreenshot(filename, 0);
        //    Debug.Log(1);
        //}

        IEnumerator CaptureScreen(Rect rect, string fileName, Action callBack = null)
        {
            captureBtn.SetActive(false);
            SoundManager.instance.skipBtn.SetActive(false);            
            yield return new WaitForEndOfFrame();
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
            Rect rt = new Rect(0, 0, 1920, 1080);
            screenShot.ReadPixels(rt, 0, 0);
            screenShot.Apply();

            byte[] bytes = screenShot.EncodeToPNG();
            System.IO.File.WriteAllBytes(fileName, bytes);
            callBack?.Invoke();

            mono.StopCoroutine(CaptureScreen(rect, fileName, callBack));
        }

        Texture2D CaptureScreen(Camera camera, Rect rect, string filename)
        {
            RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, 0);
            camera.gameObject.SetActive(true);
            camera.targetTexture = render;
            camera.Render();

            RenderTexture.active = render;
            Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
            Rect rt = new Rect(0, 0, 1920, 1080);
            tex.ReadPixels(rt, 0, 0);
            tex.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            UnityEngine.Object.Destroy(render);

            byte[] bytes = tex.EncodeToPNG();
            //string filename = Application.persistentDataPath + "/ScreenShot_" + texCount.ToString() + ".png";
            System.IO.File.WriteAllBytes(filename, bytes);
            //texCount++;
            Debug.Log("filename:" + filename);
            return tex;
        }

        IEnumerator LoadImage(string path, GameObject icon)
        {
            string fileName = "file://" + path;
            yield return new WaitForEndOfFrame();
            UnityWebRequest wr = new UnityWebRequest(fileName);
            DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
            wr.downloadHandler = texDl;
            yield return wr.SendWebRequest();
            if (!wr.isNetworkError)
            {
                Texture2D t = texDl.texture;
                Debug.Log("texDl.texture:" + texDl.texture);
                Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                                         Vector2.zero, 1f);
                Debug.Log("Sprite:" + s);
                icon.GetComponent<Image>().sprite = s;
            }
            else
            {
                Debug.Log("wr.error:" + wr.error);
                Debug.Log("wr.isNetworkError:" + wr.isNetworkError);
            }
            Debug.Log("LoadPath:" + path);
            mono.StopCoroutine(LoadImage(path, icon));
        }

        IEnumerator LoadImageWWW(string path, GameObject icon)
        {
            yield return new WaitForEndOfFrame();
            WWW www = new WWW(path);
            yield return www;
            if (www.isDone)
            {
                Debug.Log("www.isDone:" + www.error);
                Texture2D t = www.texture;
                Debug.Log("www.texture:" + www.texture.name);
                Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                                         Vector2.zero, 1f);
                Debug.Log("Sprite:" + s.name);
                icon.GetComponent<Image>().sprite = s;
            }
            else
            {
                Debug.Log("www.Done:" + www.error);
            }           

            mono.StopCoroutine(LoadImageWWW(path, icon));
        }

        //void RingStartDrag(Vector3 pos, int type, int id)
        //{
        //    paintColor = texture.GetPixelBilinear((int)pos.x, (int)pos.y);
        //}

        //void RingDrag(Vector3 pos, int type, int id)
        //{
        //    paintColor = texture.GetPixelBilinear((int)pos.x, (int)pos.y);
        //    Debug.Log("pos:" + pos);
        //    Debug.Log("paintColor:" + paintColor);
        //}

        //void RingEndDrag(Vector3 pos, int type, int id, bool isMatch)
        //{
        //    paintColor = texture.GetPixelBilinear((int)pos.x, (int)pos.y);
        //}
    }
}
