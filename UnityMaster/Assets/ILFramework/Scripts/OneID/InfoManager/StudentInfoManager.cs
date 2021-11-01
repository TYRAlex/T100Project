using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace OneID
{
    public class StudentInfoManager : MonoBehaviour
    {
        private BellSprites _bellSprites;

        private List<Sprite> _spritesList;

        private Transform _studentInfoPanel;
        private Dictionary<string, Image> _allStudentHeadImageDic;

        private GameObject _currentLightGameObject;
        private Dictionary<string,GameObject> _allStuLightDic;

        private Dictionary<OneIDStudent, Image> _allStudentSatu;

        private Transform _bgPanel;

        private Transform _currentStudentHeadImageTrasform;
        
        private void Awake()
        {
            _allStuLightDic=new Dictionary<string, GameObject>();
            _allStudentSatu=new Dictionary<OneIDStudent, Image>();
            
            _bgPanel = this.transform.GetTransform("StudentInfo/BG");
            this.transform.GetGameObject("StudentInfo").Hide();
            
            //Debug.LogError("初始化完毕");
        }

        private void Start()
        {
            InializedStudentSprite();
            InializedStudengImage();
            //print("student info start~");
        }

        public void UpdateStudentStatu()
        {
            foreach (var temp in _allStudentSatu)
            {
                if (temp.Key.IsSignIn)
                {
                    temp.Value.sprite = _bellSprites.sprites[16];
                }
                else
                {
                    temp.Value.sprite = _bellSprites.sprites[17];
                }
            }
        }

        public Sprite[] GetAllSprites()
        {
            if(_bellSprites.sprites==null)
                Debug.LogError("没有获取到相应的图片合集，请检查！");
            return _bellSprites.sprites;
        }

        void InializedStudentSprite()
        {
            _bellSprites = this.GetComponent<BellSprites>();
            Sprite[] sprites = _bellSprites.sprites;
            _spritesList=new List<Sprite>();
            for (int i = 0; i < sprites.Length; i++)
            {
                Sprite sprite = sprites[i];
                _spritesList.Add(sprite);
            }
        }

        void InializedStudengImage()
        {
            _studentInfoPanel = this.transform.GetTransform("StudentInfo");
            _allStudentHeadImageDic=new Dictionary<string, Image>();
            Transform grid = _studentInfoPanel.GetTransform("Grid");
            for (int i = 0; i < grid.childCount; i++)
            {
                Transform target = grid.GetChild(i);
                //print(target.name);
                _allStudentHeadImageDic.Add(target.name, target.GetComponent<Image>());
                GameObject clickGameObject= target.GetGameObject("Click");
               // print("kkkkkkkkkkkkk:"+target.GetGameObject("Light").name);
                
                _allStuLightDic.Add(target.name,target.GetGameObject("Light"));
                if (i == 0)
                {
                    _currentStudentHeadImageTrasform = clickGameObject.transform;
                }

                Image statuImage = target.GetTargetComponent<Image>("Statu");
                _allStudentSatu.Add(OneIDSceneManager.Instance.GetStudentByName(target.name), statuImage);
                PointerClickListener.Get(clickGameObject).clickDown = SelectStudentAndCheckTheIntergrate;
            }
        }

        public void SetTargetLightVisible(string stuName,bool isShow)
        {
            foreach (var temp in _allStuLightDic)
            {
                if (temp.Key.Equals(stuName))
                {
                    //Debug.LogError("设置成功"+stuName);
                    _currentLightGameObject = temp.Value;
                    if (temp.Value.activeSelf != isShow)
                        temp.Value.SetActive(isShow);
                }
                else
                {
                    temp.Value.Hide();
                }
            }
        }

        public void HideAllLight()
        {
            if (_allStuLightDic != null)
            {
                foreach (var temp in _allStuLightDic.Values)
                {
                    temp.Hide();
                }
            }

            
        }

        public void SelectStudentAndCheckTheIntergrate(GameObject go=null)
        {
            // bool isClickBack = false;
            // if (_currentLightGameObject != null)
            // {
            //     if (_currentLightGameObject == go.transform.parent.GetGameObject("Light"))
            //         isClickBack = true;
            // }
            // else
            // {
            //     _currentLightGameObject.Hide();
            //     GameObject stulight = go.transform.parent.GetGameObject("Light");
            //     stulight.Show();
            //     _currentLightGameObject = stulight;
            // }

            if (go == null)
            {
                go = _currentStudentHeadImageTrasform.gameObject;
            }
            //Debug.LogError("sssssssss");
            OneIDSceneManager.Instance.PlayCommonSound(10);
            if (OneIDSceneManager.Instance.GetCurrentSceneType() == OneID_SceneType.CameraPanel)
            {
                OneIDSceneManager.Instance.GetCameraSceneManger.ShowCurrentStudent(go.transform.parent.name);
                SetTargetLightVisible(go.transform.parent.name,true);
            }
            else
            {
                

                string studentName = go.transform.parent.name;
                Image targetImage = null;
                if (_allStudentHeadImageDic.TryGetValue(studentName, out targetImage))
                {
                    //Debug.LogError("1"+targetImage.name);
                    //targetImage.sprite = _spritesList.Find(p => p.name.Equals(studentName + "-1"));
                    // if (_currentLightGameObject != _allStuLightDic[studentName])
                    // {
                        //Debug.LogError("2"+_currentLightGameObject.name+" :"+_allStuLightDic[studentName].name);
                        OneIDSceneManager.Instance.ActivatyTheSelectStudent(studentName);
                        OneIDSceneManager.Instance.SetTheIntergratePanelVisibleSpecial(studentName,true);
                        SetTargetLightVisible(go.transform.parent.name,true);
                        _currentStudentHeadImageTrasform = go.transform.parent;
                    // }
                    // else
                    // {
                    //     OneIDSceneManager.Instance.ActivatyTheSelectStudent(studentName,true);
                    //     OneIDSceneManager.Instance.SetTheIntergratePanelVisibleSpecial(true);
                    //     SetTargetLightVisible(go.transform.parent.name,false);
                    //     //_currentLightGameObject = null;
                    // }
                }
                else
                {
                    Debug.LogError("学生名字不匹配，请检查！"+studentName);
                }
            }
        }


        public void ShowTargetBG(OneID_SceneType sceneType)
        {
            string sceneName = sceneType.ToString();
            bool isGetTheBGName = false;
            for (int i = 0; i < _bgPanel.childCount; i++)
            {
                Transform target = _bgPanel.GetChild(i);
                if (target.name.Equals(sceneName))
                {
                    target.gameObject.Show();
                    isGetTheBGName = true;
                }
                else
                {
                    target.gameObject.Hide();
                }
            }

            if (!isGetTheBGName)
            {
                Debug.LogError("没有找到对应的场景名称，请检查！" + sceneName);
            }

        }

        public List<Sprite> GetStudentSpritesList()
        {
            return _spritesList;
        }

        public Sprite GetTargetStudentSprite(int number)
        {
            Sprite target = _spritesList.Find(p => p.name == number.ToString());
            if (target == null)
            {
                Debug.LogError("找不到对应的学生编号，请检查！"+number);
            }

            return target;
        }
    }
}