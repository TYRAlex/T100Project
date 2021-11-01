using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ILFramework;
using ILRuntime.Runtime.Debugger.Protocol;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace OneID
{
    public enum OneID_SceneType
    {
        CameraPanel=0,
        PicturePanel,
        Video1,
        TD5623Part1,
        SelectPanel,
        GamePanel,
        TD5623Part2,
        Video2,
        TD5623Part4,
        Video3,
        TD5623Part6,
        Video4,
        TD5623Part7,
        Video5,
        SelectPanel2,
        TD5623Part8,
        GetTicketPanel,
        Video6,
        MagicSquareGiftPanel,
        UploadPanel,
        TD5623Part9,
        PresentationPanel,
        Video7,
        
        
        End,
        InterGratePanel
    }

    public enum OneID_ButtonShow
    {
        Both=0,
        Last,
        Next,
        End
    }

    public class OneIDSceneManager : MonoBehaviour
    {

        public AudioClip[] CommonSound;

        public AudioSource SoundSource;

        public AudioSource BGMSource;

        public static OneIDSceneManager Instance;

        
        
        private StudentInfoManager _studentInfoManager;
        
        private Dictionary<string, OneIDStudent> _studentDic;

        private Dictionary<string, OneIDStudent> _vsSutdentDic;

        private Dictionary<string, GameObject> _SceneGameObjectDic;

        private Dictionary<string, GameObject> _audioSourceGameObjectDic;

        private CameraSceneManager _cameraSceneManager;

        private Transform _scenePanel;
        private Transform _switchButtonPanel;
        private Button _lastButton;
        private Button _nextButton;
        private GameObject _enterIntergrateButton;

        private string[] _testStudentNameArray = new[] {"小A", "小B", "小C", "小D", "小E", "小F", "小G", "小H"};

        public CameraSceneManager GetCameraSceneManger
        {
            get
            {
                if (_cameraSceneManager == null)
                    _cameraSceneManager = _scenePanel.GetTargetComponent<CameraSceneManager>("CameraPanel");
                return _cameraSceneManager;
            }
        }

        public StudentInfoManager GetStudentInfoManager
        {
            get
            {
                if (_studentInfoManager == null)
                    _studentInfoManager = this.transform.GetTargetComponent<StudentInfoManager>("Info");
                return _studentInfoManager;
            }
        }

        private OneID_SceneType _intergrateLastSceneType;
        private OneID_SceneType _currentSceneType=OneID_SceneType.CameraPanel;
        private OneID_SceneType _lastSceneTpe = OneID_SceneType.CameraPanel;
        private int _sceneTypeNumber = 0;
        private void Awake()
        {
            Instance = this;
            _studentInfoManager = this.transform.GetTargetComponent<StudentInfoManager>("Info");
            _switchButtonPanel = this.transform.GetTransform("SwitchButton");
            _scenePanel = this.transform.GetTransform("ScenePanel");
            _lastButton = _switchButtonPanel.GetTargetComponent<Button>("Last");
            _nextButton = _switchButtonPanel.GetTargetComponent<Button>("Next");
            _enterIntergrateButton = this.transform.GetGameObject("IntergrateEnterButton/Click");
            _vsSutdentDic=new Dictionary<string, OneIDStudent>();
            _SceneGameObjectDic=new Dictionary<string, GameObject>();
            _audioSourceGameObjectDic=new Dictionary<string, GameObject>();
            InializedAllStudent();
            AddOneStudentToVSDic("1");
            AddOneStudentToVSDic("2");
            
            PointerClickListener.Get(_enterIntergrateButton).clickDown = ClickAndEnterIntergratePanel;
        }

        


        private void Start()
        {
            _vsSutdentDic.Clear();
            _switchButtonPanel.gameObject.Hide();
            _currentSceneType = OneID_SceneType.CameraPanel;
            _sceneTypeNumber = (int) OneID_SceneType.End;
            _SceneGameObjectDic.Clear();
            SetEnterIntergrateButtonVisilble(false);
            SetStudentInfoVisible(false);
            //ShowTargetPanel(OneID_SceneType.CameraPanel);
        }

        public void PlayCommonSound(int index, bool isLoop = false)
        {
            //Debug.LogError("playCommonSound:"+index);
            AudioClip currentClip = CommonSound[index];
            AudioSource source = SoundSource;
            if (isLoop)
            {
                
                source.loop = isLoop;
                source.clip = currentClip;
                source.Play();
            }
            else
            {
                source.PlayOneShot(currentClip);
            }

            
        }

        public void CreatAudioSource(int index,bool isLoop=false)
        {
            GameObject audioSourceGameObject = null;
            AudioSource source = null;
            if (!_audioSourceGameObjectDic.TryGetValue("AudioSource", out audioSourceGameObject))
            {
                audioSourceGameObject = new GameObject("AudioSource");
                _audioSourceGameObjectDic.Add("AudioSource", source.gameObject);
            }

            if (audioSourceGameObject.GetComponent<AudioSource>() == null)
            {
                audioSourceGameObject.AddComponent<AudioSource>();
            }

            source = audioSourceGameObject.GetComponent<AudioSource>();


            if(!isLoop)
                source.PlayOneShot(CommonSound[index]);
            else
            {
                source.clip = CommonSound[index];
                source.Play();
                source.loop = true;
            }
        }

        public void PlayCommonBGM(int index=0)
        {
            AudioClip currentClip = CommonSound[index];
            BGMSource.clip = currentClip;
            BGMSource.loop = true;
            BGMSource.Play();
        }

        public void StopAudioSource(string targetName="all")
        {
            switch (targetName)
            {
                case "all":
                    if(SoundSource.isPlaying)
                        SoundSource.Stop();
                    if(BGMSource.isPlaying)
                        BGMSource.Stop();
                    break;
                case "sound":
                    if(SoundSource.isPlaying)
                        SoundSource.Stop();
                    break;
                    
                case "bgm":
                    if(BGMSource.isPlaying)
                        BGMSource.Stop();
                    break;
                default:
                    Debug.LogError("输错名字啦，请检查！:" +targetName);
                    break;
            }
        }

        public void SetSwitchButtonVisible(bool isShow,OneID_ButtonShow buttonShow=OneID_ButtonShow.Both )
        {
            _switchButtonPanel.gameObject.SetActive(isShow);
            if (isShow)
            {
                switch (buttonShow)
                {
                    case OneID_ButtonShow.Both:
                        _lastButton.gameObject.Show();
                        _nextButton.gameObject.Show();
                        break;
                    case OneID_ButtonShow.Last:
                        _lastButton.gameObject.Show();
                        _nextButton.gameObject.Hide();
                        break;
                    case OneID_ButtonShow.Next:
                        _lastButton.gameObject.Hide();
                        _nextButton.gameObject.Show();
                        break;
                }
            }
        }

        public void SwitchTotheNextScene()
        {
            int currentTypeIndex = (int) _currentSceneType;
            if (currentTypeIndex < _sceneTypeNumber-1)
            {
                PlayCommonSound(1);
                int nextTypeIndex = currentTypeIndex + 1;
                OneID_SceneType nextSceneType = (OneID_SceneType) nextTypeIndex;
                _currentSceneType = nextSceneType;
                
                OpenTargetScene(nextSceneType);
                SoundManager.instance.ShowVoiceBtn(false);
            }
        }

        public void SwitchToTheLastScene()
        {
            int currentTypeIndex = (int) _currentSceneType;
            if (currentTypeIndex > 0)
            {
                PlayCommonSound(1);
                int nextTypeIndex = currentTypeIndex-1;
                OneID_SceneType nextSceneType = (OneID_SceneType) nextTypeIndex;
                _currentSceneType = nextSceneType;
                //Debug.LogError(nextSceneType.ToString());
                OpenTargetScene(nextSceneType);
                SoundManager.instance.ShowVoiceBtn(false);
            }
        }

        public int GetStudentNumber()
        {
            return _studentDic.Keys.Count;
        }

        public Dictionary<string, OneIDStudent> GetAllStudentDic()
        {
            return _studentDic;
        }

        public OneIDStudent GetStudentByName(string stuName)
        {
            return _studentDic[stuName];
        }

        public OneIDStudent SelectTheStudentByName(string stuName)
        {
            if (_studentDic.ContainsKey(stuName))
                return _studentDic[stuName];
            else
            {
                Debug.LogError("不存在这个学生的名字！ 请检查：" + stuName);
                return null;
            }
        }

        public OneIDStudent GetSelectSudent()
        {
            foreach (var student in _studentDic)
            {
                if (student.Value.IsSelect)
                {
                    return student.Value;
                }
            }

            Debug.LogError("所有学生都没有点击激活，请检查！");
            return null;
        }


        public Sprite GetStudentSprite(int number)
        {
            return _studentInfoManager.GetTargetStudentSprite(number);
        }

        public OneIDStudent AddOneStudentToVSDic(string name)
        {
            OneIDStudent student = null;
            if (_studentDic.ContainsKey(name))
            {
                student = _studentDic[name];
                if (!_vsSutdentDic.ContainsKey(name))
                    _vsSutdentDic.Add(name, student);
                else
                {
                    _vsSutdentDic[name] = student;
                }
            }
            else
            {
                Debug.LogError("名称不存在，请检查!"+name);
               
            }
            //Debug.LogError("加入成功"+name+" Count"+GetVSStudentDic().Count);
            return student;
        }

        
        public Dictionary<string,OneIDStudent> GetVSStudentDic()
        {
            return _vsSutdentDic;
        }

        public void ChangeThePKStudentStatu(string name,bool isSelect)
        {
            if (_studentDic.ContainsKey(name))
            {
                _studentDic[name].IsPKSelect = isSelect;
            }
            else
            {
                Debug.LogError("名称不存在，请检查！" + name);
            }
        }

        public void ActivatyTheSelectStudent(string stuName,bool isActive=true)
        {
            OneIDStudent stu = null;
            // foreach (var key in _studentDic.Keys)
            // {
            //     Debug.LogError("key:"+key);
            // }
            if (_studentDic.TryGetValue(stuName, out stu))
            {
                //Debug.LogError("学生："+stu.Name+"被激活");
                stu.IsSelect = isActive;
            }
            else
            {
                Debug.LogError("学生的名字对不上，请检查！" + stuName);
            }
        }




        void InializedAllStudent()
        {
            _studentDic=new Dictionary<string, OneIDStudent>();
            for (int i = 1; i <= 8; i++)
            {
                _studentDic.Add(i.ToString(),new OneIDStudent(i.ToString(),_testStudentNameArray[8-i]));
            }
            
        }

        public void SetTheIntergratePanelVisibleSpecial(string stuName,bool isShow)
        {
            // GameObject intergratePanelGameObject = null;
            // if (!_SceneGameObjectDic.TryGetValue("InterGratePanel", out intergratePanelGameObject))
            // {
            //     intergratePanelGameObject= ResourceManager.instance.LoadCommonPrefab("InterGratePanel");
            //     _SceneGameObjectDic.Add("InterGratePanel",intergratePanelGameObject);
            //     intergratePanelGameObject.transform.SetParent(_scenePanel);
            //     intergratePanelGameObject.transform.localPosition=Vector3.zero;
            // }
            // intergratePanelGameObject.SetActive(isShow);
            // intergratePanelGameObject.transform.SetAsLastSibling();
            GameObject intergratePanelGameObject = null;
            if (isShow)
            {
               
                if (_SceneGameObjectDic.TryGetValue("InterGratePanel", out intergratePanelGameObject))
                {
                    IntergrateManager intermgr = intergratePanelGameObject.GetComponent<IntergrateManager>();
                    //OneIDStudent selectStudent = GetSelectSudent();
                    OneIDStudent selectStudent = GetStudentByName(stuName);
                    intermgr.JudgeStudentAndExcuteNext(selectStudent);
                }
            }
            else
            {
                intergratePanelGameObject = _SceneGameObjectDic["InterGratePanel"];
                SetSwitchButtonVisible(true);
                intergratePanelGameObject.Hide();
            }
        }

        public void OpenTargetScene(OneID_SceneType sceneType)
        {
            ShowTargetPanel(sceneType);
            foreach (var sceneTarget in _SceneGameObjectDic)
            {
                //Debug.LogError("1:"+sceneTarget.name+"2:"+sceneName);
                if(sceneTarget.Key.Equals(sceneType.ToString()))
                    sceneTarget.Value.Show();
                else
                {
                    sceneTarget.Value.Hide();
                }
            }
        }

        public void LoadAllVideoPlayer()
        {
            for (int i = 1; i <= 7; i++)
            {
                ResourceManager.instance.LoadOneIDCommonPrefabAsyn("Video" + i);
            }
        }

        public GameObject ShowTargetPanel(OneID_SceneType sceneType)
        {
            StopAllCoroutines();
            string sceneName = sceneType.ToString();
            
            //Debug.LogError("上一个场景的名称：" + _lastSceneTpe.ToString());
            GameObject sceneGameObject = null;
            if (sceneType != OneID_SceneType.CameraPanel)
            {
                //int lastSceneNumber = (int) sceneType - 1;
                //OneID_SceneType lastSceneType = (OneID_SceneType) lastSceneNumber;
                if (_lastSceneTpe.ToString().Contains("TD"))
                {
                    //Debug.LogError("关闭所有"+ _lastSceneTpe.ToString());
                    SoundManager.instance.StopAllSoundCoroutine();
                    SoundManager.instance.StopAll();
                    GameManager.instance.StopUnity(_lastSceneTpe.ToString());
                    // if(!sceneName.Contains("1"))
                    // {
                    //     int number = Convert.ToInt32(sceneName.Substring(10, 1));
                    //     number--;
                    //     GameManager.instance.StopUnity("TD5623Part" + number);
                    // }
                }
            }

            SetEnterIntergrateButtonVisilble(true);
            
            if (sceneName.Contains("TD"))
            {
                StopAudioSource();
                if (!_SceneGameObjectDic.TryGetValue(sceneName, out sceneGameObject))
                {
                    Debug.LogError("1111111111"+sceneName);
                    sceneGameObject = GameObject.Find(sceneName);
                    if (sceneGameObject != null)
                    {
                        Debug.LogError("22222222");
                        _SceneGameObjectDic.Add(sceneName,sceneGameObject);
                    }
                    else
                    {
                        Debug.Log("当前物体名称为："+sceneGameObject.name);
                    }
                }

                if (sceneGameObject != null)
                {
                    sceneGameObject.Show();
                    sceneGameObject.transform.SetParent(_scenePanel);
                     // sceneGameObject.GetComponent<RectTransform>().anchoredPosition=Vector2.zero;
                     // sceneGameObject.GetComponent<RectTransform>().sizeDelta=Vector2.zero;
                    sceneGameObject.transform.SetAsLastSibling();
                    GameManager.instance.PlayUnity(sceneName);
                    _currentSceneType = sceneType;
                }
                else
                {
                    Debug.LogError("找不到游戏物体，请检查！" + sceneName);
                }
            }
            else
            {
                if (sceneName.Equals("GamePanel"))
                {
                    PlayCommonBGM(23);
                }
                else if (sceneName.Contains("Video"))
                {
                    StopAudioSource("all");
                }
                else
                {
                    PlayCommonBGM();
                }

                if (!_SceneGameObjectDic.TryGetValue(sceneName, out sceneGameObject))
                {
                    //Debug.LogError("sdfffff:"+sceneName);
                    sceneGameObject= ResourceManager.instance.LoadCommonPrefab(sceneName);
                    sceneGameObject.name = sceneName;
                    _SceneGameObjectDic.Add(sceneName,sceneGameObject);
                }
                sceneGameObject.Show();
                sceneGameObject.transform.SetParent(_scenePanel);
                sceneGameObject.GetComponent<RectTransform>().anchoredPosition=Vector2.zero;
                sceneGameObject.GetComponent<RectTransform>().sizeDelta=Vector2.zero;
                sceneGameObject.transform.SetAsLastSibling();
                //if (sceneType != OneID_SceneType.InterGratePanel)
                _currentSceneType = sceneType;
                if (sceneType != OneID_SceneType.CameraPanel)
                    _studentInfoManager.HideAllLight();
                //Debug.LogError("111111111"+sceneGameObject.transform.localPosition);
            }

            if (sceneType == OneID_SceneType.CameraPanel || sceneType == OneID_SceneType.InterGratePanel)
            {
                SetStudentInfoVisible(true);
                _studentInfoManager.ShowTargetBG(sceneType);
                _studentInfoManager.UpdateStudentStatu();
            }
            else
            {
                SetStudentInfoVisible(false);
            }

            if (sceneName.Equals("Video7"))
            {
                SetSwitchButtonVisible(true,OneID_ButtonShow.Last);
            }
            else if (sceneType == OneID_SceneType.PicturePanel|| sceneName.Contains("TD") || sceneName.Contains("Video")
                     || sceneType==OneID_SceneType.GamePanel|| sceneType==OneID_SceneType.MagicSquareGiftPanel
                     || sceneType==OneID_SceneType.SelectPanel2 || sceneType==OneID_SceneType.GetTicketPanel || sceneType== OneID_SceneType.UploadPanel|| sceneType== OneID_SceneType.PresentationPanel)
            {
                SetSwitchButtonVisible(true);
            }
            else if (sceneType == OneID_SceneType.InterGratePanel)
            {
                SetSwitchButtonVisible(false);
            }
            // else
            // {
            //     SetSwitchButtonVisible(false);
            // }

            if (sceneName.Contains("Video")&&sceneGameObject!=null)
            {
                
                VideoPlayer vp=sceneGameObject.GetComponent<VideoPlayer>();
                vp.targetTexture.Release();
                //Debug.Log("VideoPlayer Prepard statu:"+vp.isPrepared);
                RawImage rawImage= sceneGameObject.transform.GetTargetComponent<RawImage>("videoImage");
                rawImage.texture = vp.targetTexture;
                
                vp.targetTexture.DiscardContents();
                
                vp.Play();
            }
            _lastSceneTpe = _currentSceneType;

            return sceneGameObject;
        }

        public void AddGameSceneObject(GameObject target)
        {
            if (!_SceneGameObjectDic.ContainsKey(target.name))
            {
                _SceneGameObjectDic.Add(target.name,target);
            }
            else
            {
                _SceneGameObjectDic[target.name] = target;
            }
        }

        public void SetStudentInfoVisible(bool isShow)
        {
            _studentInfoManager.transform.GetGameObject("StudentInfo").SetActive(isShow);
        }

        // public StudentInfoManager GetStudentInfoManager()
        // {
        //     return _studentInfoManager;
        // }

        public OneID_SceneType GetCurrentSceneType()
        {
            return _currentSceneType;
        }

        public void SetEnterIntergrateButtonVisilble(bool isShow)
        {
            GameObject target = _enterIntergrateButton.transform.parent.gameObject;
            if (target.activeSelf != isShow)
                target.SetActive(isShow);
        }

        private void ClickAndEnterIntergratePanel(GameObject go)
        {
            GameObject parent = go.transform.parent.gameObject;
            PlayCommonSound(1);
            if (_currentSceneType == OneID_SceneType.InterGratePanel)
            {
                
                SpineManager.instance.DoAnimation(parent, "an3", false,
                    () =>
                    {
                        
                        OpenTargetScene(_intergrateLastSceneType);
                        
                       // Debug.LogError("if:"+_currentSceneType);
                        SpineManager.instance.DoAnimation(parent, "an1", false);
                    });
                
            }
            else
            {
                _intergrateLastSceneType = _currentSceneType;
                SpineManager.instance.DoAnimation(parent, "an2", false,()=>
                {
                    
                    OpenTargetScene(OneID_SceneType.InterGratePanel);
                    _studentInfoManager.SelectStudentAndCheckTheIntergrate();
                });
                
                //Debug.LogError("else:"+_currentSceneType);
            }
        }
        
    }
}