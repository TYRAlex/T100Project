using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using easyar;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    
    public class CourseCavePart7
    {
        private Transform _curTrans;

        private MonoBehaviour _mono;

        private Transform _easyARParent;

        private Transform _canvas;

        private Transform _backGround;

        private Transform _backGround2;

        private List<GameObject> _spineList;

        private WaitForSeconds _oneSecond;

        private WaitForSeconds _twoSecond;

        private float[] _posX ;

        private int _currentDW = 1;

        private int _oldCurrentDW = 1;

        //private Animator _logo;

        //private Transform _frameTransform;

        private bool _isFinishedReconize = false;

        private ImageTrackerFrameFilter[] _allImageTrackerFrameFilters;

        private VideoCameraDevice _videoCameraDevice;

        private GameObject _skipBtn;

        // private List<Transform> _allAnimalList;  

        //private RawImage _rawImage;
        //private WebCamTexture _webCamTexture;

        //private GameObject _bg;

        void Start(object o)
        {
            GameObject curGo = (GameObject) o;
            _mono = curGo.GetComponent<MonoBehaviour>();
            
            _curTrans = curGo.transform;
            // _bg = _curTrans.GetGameObject("Bg");
            // _bg.Show();
            //_rawImage = _curTrans.GetTargetComponent<RawImage>("Bg");
            GameInit(_curTrans);
            GameStart();
            
        }

        

        

        private void GameInit(Transform curTrans)
        {



            _skipBtn = GameObject.Find("MainCanvas").transform.GetGameObject("skipBtn");
            _skipBtn.Hide();
            _canvas = curTrans.GetTransform("Course70002Part3_htp");
            _backGround = _canvas.GetTransform("background");
            _backGround2 = _canvas.GetTransform("background2");
            
            
            _easyARParent = curTrans.GetTransform("EasyAR_ImageTracker-1");
            _backGround.gameObject.Show();
            _backGround2.gameObject.Show();
            LoadAllAnimal();
            
            AddAllSpines();
            _oneSecond = new WaitForSeconds(1f);
            _twoSecond = new WaitForSeconds(2f);
             _allImageTrackerFrameFilters = _easyARParent.GetComponentsInChildren<ImageTrackerFrameFilter>();
            _videoCameraDevice = _easyARParent.GetTransform("VideoCameraDevice").GetComponent<VideoCameraDevice>();
            _posX = new[] { 0f, -1371f, -1371f, -3861 };
           
            _backGround.gameObject.Hide();
            _backGround2.gameObject.Hide();
        }

        void LoadAllAnimal()
        {
            // _allAnimalList=new List<Transform>();
            Transform animalParent = _curTrans.GetTransform("Animals");
            //Transform animalParent = easyArGameObject.transform.Find("Animals");
            animalParent.gameObject.Show();
            for (int i = 0; i < animalParent.childCount; i++)
            {
                Transform target = animalParent.GetChild(i);
                target.gameObject.Show();
                // _allAnimalList.Add(target);
                SpineManager.instance.DoAnimation(target.gameObject, "daiji", false);
                target.GetComponent<ImageTargetController>().TargetFound += () =>
                {
                    GetCurrentTargetImage(target.name);
                };
                
            }
            
        }

        void GameStart()
        {
            SetVideoCameraDeviceVisible(true);
            RemoteAllImageTracker(true);
            _isFinishedReconize = false;
           
           //_mono.StartCoroutine(TestShow());
        }

        

        // IEnumerator CallWebCam()
        // {
        //     yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        //     if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        //     {
        //         _webCamTexture =new WebCamTexture();
        //         WebCamDevice[] devices = WebCamTexture.devices;
        //         _webCamTexture.deviceName = devices[0].name;
        //         
        //         _rawImage.texture = _webCamTexture;
        //         Debug.LogError("摄像头名字" + devices[0].name + "Texture:" + _webCamTexture.isReadable + " " + _rawImage.texture.filterMode);
        //
        //     }
        //
        //     Debug.LogError("打开摄像头");
        //     _webCamTexture.Play();
        // }

        void AddAllSpines()
        {
            _spineList = new List<GameObject>();
            Transform spines = _backGround.Find("Spines");
            for (int i = 0; i < spines.childCount; i++)
            {
                _spineList.Add(spines.GetChild(i).gameObject);
            }

            // for (int i = 0; i < _spineList.Count; i++)
            // {
            //     Debug.Log("SpineList:" + _spineList[i]);
            // }
        }

        GameObject FindTargetSpines(string name)
        {
            if (_spineList.Count > 0)
            {
                for (int i = 0; i < _spineList.Count; i++)
                {
                    GameObject target = _spineList[i];
                    if (target.name.Equals(name))
                    {
                        return target;
                    }
                }

                
            }
            else
            {
                Debug.LogError("SpineList数量为0，请检查！");
               
            }
            return null;
        }

        void RemoteAllImageTracker(bool isOpen)
        {
            for (int i = 0; i < _allImageTrackerFrameFilters.Length; i++)
            {
                _allImageTrackerFrameFilters[i].enabled = isOpen;
            }
        }
        
        public void GetCurrentTargetImage(string targetName)
        {
            
            if (_isFinishedReconize)
                return;
            
            _isFinishedReconize = true;

            RemoteAllImageTracker(false);
            //SetVideoCameraDeviceVisible(false);
            //Debug.Log("识别对象的名字" + targetName);
            switch (targetName)
            {
                case "z":
                    _currentDW = 1;
                    break;
                case "n":
                    _currentDW = 2;
                    break;
                case "qm":
                    _currentDW = 3;
                    break;
                case "dx":
                    _currentDW = 4;
                    break;
                case "ly":
                    _currentDW = 5;
                    break;

            }
            _mono.StartCoroutine(JudgeTargetAndPlaySpineAnimatorIE());
        }
        
        IEnumerator JudgeTargetAndPlaySpineAnimatorIE()
        {
            bool isMove = true;
            //print("背景移动时" + _currentDW);
            if (_currentDW < 5)
            {
                _backGround.gameObject.SetActive(true);
                ResetAllAnimalPos();
                //_backGround.transform.position = new Vector3(_backGround.position.x, 769f, 0);
                if (_oldCurrentDW != _currentDW)
                {
                    if (_oldCurrentDW == 2 && _currentDW == 3)
                    {
                        isMove = false;
                    }
                    else if (_oldCurrentDW == 3 && _currentDW == 2)
                    {
                        isMove = false;
                    }

                    if (isMove)
                    {
                        _backGround.DOLocalMove(
                            new Vector3(_posX[_currentDW - 1],
                                0, 0), 2f);
                        yield return _twoSecond;

                    }
                    _oldCurrentDW = _currentDW;
                }
            }
            else
            {
                _backGround2.gameObject.SetActive(true);
            }

            yield return PlaySpineAnimatorIE();
        
        }

        float TransferValuebyScreen(float targetValue, bool isWidth=true)
        {
            if (isWidth)
            {
                return targetValue * Screen.width / 1920f;
            }
            else
            {
                return targetValue * Screen.height / 1080f;
            }
        }

        void ResetAllAnimalPos()
        {
            SetAnimalOriginalPos("z", 280f);
            SetAnimalOriginalPos("qm", 4098f);
            SetAnimalOriginalPos("dx", 5504f);
            SetAnimalOriginalPos("n", 1609f);
            SetAnimalOriginalPos("shu", -1870.45f);
            // if (Math.Abs(Math.Abs(Screen.width - 1920f)) ==0)
            // {
            //     SetAnimalOriginalPos("z", 230f);
            //     SetAnimalOriginalPos("qm", 4098f);
            //     SetAnimalOriginalPos("dx", 5454.1f);
            //     SetAnimalOriginalPos("n", 1559f);
            //     SetAnimalOriginalPos("shu", -1870.45f);
            // }
            //
            // else if (Math.Abs(Screen.width - 1386f) == 0)
            // {
            //     SetAnimalOriginalPos("z", 280f);
            //     SetAnimalOriginalPos("qm", 4098f);
            //     SetAnimalOriginalPos("dx", 5504f);
            //     SetAnimalOriginalPos("n", 1609f);
            //     SetAnimalOriginalPos("shu", -1870.45f);
            // }
            // else if(Math.Abs(Screen.width - 661f) == 0)
            // {
            //     SetAnimalOriginalPos("z", 280f);
            //     SetAnimalOriginalPos("qm", 4098f);
            //     SetAnimalOriginalPos("dx", 5504f);
            //     SetAnimalOriginalPos("n", 1609f);
            //     SetAnimalOriginalPos("shu", -1870.45f);
            //     
            // }


        }

        void SetAnimalOriginalPos(string targetName,float value)
        {
            GameObject target = FindTargetSpines(targetName);
            if (targetName != "z" && targetName != "dx" && targetName != "shu")
            {
                var localPosition = target.transform.localPosition;
                localPosition = new Vector3(value, localPosition.y, localPosition.z);
               
                target.transform.localPosition = localPosition;
                target.transform.rotation=Quaternion.identity;
            }

           
            string animationName = null;
            switch (targetName)
            {
                case "shu":
                    animationName = "shu";
                    break;
                case "n":
                    target = target.transform.GetGameObject("n");
                    animationName = "daiji";
                    break;
                case "z":
                case "ly":
                    animationName = "daiji";
                    break;
                case "qm":
                    animationName = "animation2";
                    target = target.transform.GetGameObject("qm");
                    break;
                case "dx":
                    animationName = "animation";
                    break;
            }
            SpineManager.instance.DoAnimation(target, animationName, false);
        }

        IEnumerator PlaySpineAnimatorIE()
        {
            //print("CurrentDW" + _currentDW);
            switch (_currentDW)
            {
                case 1:
                    GameObject pig = FindTargetSpines("z");
                    GameObject tree = FindTargetSpines("shu");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    SpineManager.instance.DoAnimation(pig, "daiji", false);

                    yield return _twoSecond;

                    SpineManager.instance.DoAnimation(tree, "shu");
                    SpineManager.instance.DoAnimation(pig, "animation", false);
                    yield return _oneSecond;
                    SpineManager.instance.DoAnimation(pig, "animation2", false);
                    SpineManager.instance.DoAnimation(tree, "shu2", false);
                    yield return _oneSecond;
                    SpineManager.instance.DoAnimation(tree, "shu3", false);
                    SpineManager.instance.DoAnimation(pig, "animation3", false);
                    yield return new WaitForSeconds(2f);

                    //SpineManager.instance.DoAnimation()s\
                    break;
                case 2:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    GameObject nParent = FindTargetSpines("n");
                    GameObject n = nParent.transform.Find("n").gameObject;
                    SpineManager.instance.DoAnimation(n, "animation");
                    float nPos = nParent.transform.localPosition.x;
                    nParent.transform.DOLocalMove(new Vector3(nPos + 1200, -10, 0), 2f);
                    yield return _twoSecond;
                    nParent.transform.DORotate(new Vector3(0, 180f, 0), 1f);
                    yield return _oneSecond;
                    nParent.transform.DOLocalMove(new Vector3(nPos, -10, 0), 2f);
                    yield return _twoSecond;
                    nParent.transform.DORotate(Vector3.zero, 1);
                    yield return _oneSecond;
                    SpineManager.instance.DoAnimation(n, "daiji");
                    break;
                case 3:
                    GameObject qmParent = FindTargetSpines("qm");
                    GameObject qm = qmParent.transform.Find("qm").gameObject;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    yield return _twoSecond;
                    SpineManager.instance.DoAnimation(qm, "animation");
                    float qmPos = qmParent.transform.localPosition.x;
                    qmParent.transform.DOLocalMove(new Vector3(qmPos - 1200, -10, 0), 2f);
                    yield return _twoSecond;
                    qmParent.transform.DORotate(new Vector3(0, 180, 0), 1);
                    yield return _oneSecond;
                    qmParent.transform.DOLocalMove(new Vector3(qmPos, -10, 0), 2f);
                    yield return _twoSecond;
                    qmParent.transform.DORotate(Vector3.zero, 1f);
                    yield return _oneSecond;
                    SpineManager.instance.DoAnimation(qm, "animation2");
                    break;
                case 4:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    yield return _twoSecond;
                    GameObject dx = FindTargetSpines("dx");
                    SpineManager.instance.DoAnimation(dx, "animation2", false);
                    yield return new WaitForSeconds(4.67f);
                    SpineManager.instance.DoAnimation(dx, "animation");
                    break;
                case 5:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                    SpineManager.instance.DoAnimation(_canvas.Find("background2/ly").gameObject, "animation", false);
                    yield return _twoSecond;
                    break;

            }

            yield return _twoSecond;
            _isFinishedReconize = false;
            RemoteAllImageTracker(true);
            SetVideoCameraDeviceVisible(true);
            if (_currentDW == 1)
            {
                //_backGround.transform.localPosition = new Vector3(_backGround.localPosition.x, -350, 0);
                SpineManager.instance.DoAnimation(FindTargetSpines("z"), "daiji", false);
                GameObject shu = FindTargetSpines("shu");
                SpineManager.instance.SetTimeScale(shu, 50f);
                SpineManager.instance.DoAnimation(shu, "shu", false);
                yield return new WaitForSeconds(0.04f);
                SpineManager.instance.SetTimeScale(shu, 1f);
                //_backGround.transform.position=new Vector3(_backGround.position.x, 769f, 0);
            }

            _backGround.gameObject.SetActive(false);
            _backGround2.gameObject.SetActive(false);

        }


        void SetVideoCameraDeviceVisible(bool isOpen)
        {
            _videoCameraDevice.enabled = isOpen;
        }
        
        
        
        void OnDisable()
        {
            SetVideoCameraDeviceVisible(false);
        }
        
        
        

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }
    }
}
