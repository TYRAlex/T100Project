using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class OpenCamera : MonoBehaviour
    {
        public RawImage CameraRender; //相机渲染的UI
        [HideInInspector]
        public WebCamTexture WebCam;
        private Camera _camera;

        public Image TestImage;

        public Rect RectTarget;

        public CameraSceneManager TheCameraSceneManager;

        public Image[] AllShowImage;

        private void Awake()
        {
            _camera = this.GetComponent<Camera>();
            TheCameraSceneManager = this.transform.parent.GetComponent<CameraSceneManager>();
            //_camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }

        private void OnEnable()
        {
            OpenCameraBackground();
        }

        private void OnDisable()
        {
            CloseCamera();
        }

        /// <summary>
        /// 打开摄像机
        /// </summary>
        public void OpenCameraBackground()
        {
            StartCoroutine("StartCam");
        }

        void CloseCamera()
        {
            StopCoroutine("StartCam");
            CameraRender.gameObject.Hide();
            if (WebCam)
                WebCam.Stop();
            Debug.Log("关闭");
        }

        public IEnumerator StartCam()
        {
            int maxl = Screen.width;
            if (Screen.height > Screen.width)
            {
                maxl = Screen.height;
            }

            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                if (WebCam != null)
                {
                    WebCam.Stop();
                }

                CameraRender.gameObject.SetActive(true); //打开渲染图
                WebCamDevice[] devices = WebCamTexture.devices; //获取可用设备
                if (devices.Length > 0)
                {
                    string devicename = devices[0].name;
                    WebCam = new WebCamTexture(devicename, maxl, maxl, 12)
                    {
                        wrapMode = TextureWrapMode.Repeat
                    };
                    CameraRender.texture = WebCam;
                    WebCam.Play();
                }
                else
                {
                    Debug.LogError("没有可用设备，请检查！");
                }

                
            }
        }

        /// <summary>
        /// 对相机截图
        /// </summary>
        /// <param name="rect">截屏的区域</param>
        /// <returns></returns>
        public Texture2D CaptureCamera()
        {
            Rect rect = CameraRender.rectTransform.rect;
            //创建一个RenderTexture对象
            RenderTexture rt = new RenderTexture((int) rect.width, (int) rect.height, 0);
            //临时设置相关相机的targetTexture为rt，并手动渲染相关相机
            _camera.targetTexture = rt;
            _camera.Render();
            //激活这个rt，并从中读取像素
            RenderTexture.active = rt;

            Texture2D screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, (int) rect.xMin, (int) rect.yMin);
            screenShot.Apply();
            //重置相关参数，以使用camera继续在屏幕中显示
            _camera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);


            byte[] bytes = screenShot.EncodeToPNG();
            string fileName = Application.dataPath + "/Screenshot.png";
            System.IO.File.WriteAllBytes(fileName, bytes);
            
            Debug.Log(string.Format("截屏了一张照片:{0}", fileName));
            return screenShot;

        }

        public void ScreenShot()
        {
            TheCameraSceneManager.TakePhotoAni(() =>
            {
                GetPhotoTexture();
            });
            
        }

        public void GetPhotoTexture()
        {
            StartCoroutine(GetTextureIE());
        }

        public void PauseTheWebcam()
        {
            if (WebCam)
                WebCam.Pause();
        }

        public void PlayTheWebcam()
        {
            if (WebCam)
                WebCam.Play();
        }

        IEnumerator GetTextureIE()
        {
            yield return new WaitForEndOfFrame();
            Texture2D t = new Texture2D((int) CameraRender.rectTransform.rect.width,
                (int) CameraRender.rectTransform.rect.height);
            //t.ReadPixels(CameraRender.rectTransform.rect, 0, 0, false);
            // Debug.LogFormat("x:{0},y:{1},Width:{2},Height{3}", CameraRender.transform.localPosition.x,
            //     CameraRender.transform.localPosition.y, CameraRender.rectTransform.rect.width,
            //     CameraRender.rectTransform.rect.height);
            t.ReadPixels(
                new Rect(
                    (Screen.width / 2f + CameraRender.transform.localPosition.x) -
                    CameraRender.rectTransform.rect.width / 2,
                    Screen.height / 2f + CameraRender.transform.localPosition.y -
                    CameraRender.rectTransform.rect.height / 2f, (int) CameraRender.rectTransform.rect.width,
                    (int) CameraRender.rectTransform.rect.height), 0, 0, false);
            t.Apply();
            //TestImage.sprite = Sprite.Create(t, new Rect(0, 0, 0, 0), new Vector2(0.5f, 0.5f));
            if (WebCam)
                WebCam.Pause();
            byte[] bytes = t.EncodeToPNG();
            //string fileName = Application.dataPath + "/Screenshot.png";
            //System.IO.File.WriteAllBytes(fileName, bytes);
            Sprite sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
            //TheCameraSceneManager.GetCurrentShowImage().GetComponent<Image>().sprite=sprite;
            AllShowImage[Convert.ToInt32(TheCameraSceneManager.GetCurrentStudentName())-1].sprite = sprite;
            TheCameraSceneManager.SetConfirmAndCancelButtonVisible(true);
            //Debug.Log(string.Format("截屏了一张照片:{0}", fileName));
        }

        public void ConfirmPictrue()
        {
            TheCameraSceneManager.ComfirmPicture();
            
        }


        public void CancelPictrue()
        {
            TheCameraSceneManager.CancelPicture();
            
            
            
        }

        
    }
}
