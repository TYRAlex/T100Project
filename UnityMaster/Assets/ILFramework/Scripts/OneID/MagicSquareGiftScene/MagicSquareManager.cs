using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using ILFramework;
using UnityEngine;
using UnityEngine.UI;


namespace OneID
{
    public class MagicSquareManager : MonoBehaviour
    {
        private GameObject _randomCardSpine;
        private GameObject _clickSpine;

        private Transform _cardPanel;
        private Transform _studentInfoPanel;
        private Transform _needle;

        private int _randomNumber = 1;

        private int _moveToNextStudentIndex = 0;

        private int _currentStudentIndex = 0;

        private Transform _lightEffect;

        private List<GameObject> _headImageSpineList;

        private Dictionary<OneIDStudent, Image> _allStudentStatuDic;

        private bool _isPlaying = false;

        void Awake()
        {
            _cardPanel = this.transform.GetTransform("CardPanel");
            _studentInfoPanel = this.transform.GetTransform("StudentInfo");
            _randomCardSpine = this.transform.GetGameObject("RandomCard");
            _clickSpine = this.transform.GetGameObject("ClickSpine");
            _needle = this.transform.GetTransform("Needle");
            _lightEffect = this.transform.GetTransform("LightEffect");
            _allStudentStatuDic=new Dictionary<OneIDStudent, Image>();
            PointerClickListener.Get(_clickSpine.transform.GetGameObject("Click")).clickDown = ClickEvent;
            
        }

        void Start()
        {
            _currentStudentIndex = 0;
            //RandomCard2();
            ResetStudent();
            _lightEffect.gameObject.Hide();
            ResetStudentHeadImage();
            _isPlaying = false;

            JudgeImageAndSelect(_studentInfoPanel.GetChild(0));
            UpdateAllStudentCurrentStatu();
        }
        

        void ResetStudent()
        {
            Dictionary<string, OneIDStudent> studentDic = OneIDSceneManager.Instance.GetAllStudentDic();
            int i = 0;
            foreach (var temp in studentDic)
            {
                _cardPanel.GetChild(i).name = temp.Key;
                _studentInfoPanel.GetChild(i).name = temp.Key;
                i++;
            }
        }

        void ResetStudentHeadImage()
        {
            _headImageSpineList = new List<GameObject>();
            for (int i = 0; i < _studentInfoPanel.childCount; i++)
            {
                Transform targetImage = _studentInfoPanel.GetChild(i);
                OneIDSceneManager.Instance.SelectTheStudentByName(targetImage.name).IsMagicSqareGiftSelect = false;
                GameObject clickButton = targetImage.GetGameObject("Click");
                GameObject targetSpine = targetImage.GetGameObject(targetImage.name);
                _headImageSpineList.Add(targetSpine);
                targetSpine.Hide();
                Image studentStatuImage = targetImage.GetTargetComponent<Image>("Statu");

                _allStudentStatuDic.Add(OneIDSceneManager.Instance.SelectTheStudentByName(targetImage.name),
                    studentStatuImage);
                PointerClickListener.Get(clickButton).clickDown = ClickHeadAndSelect;
            }
        }

        void UpdateAllStudentCurrentStatu()
        {
            foreach (var temp in _allStudentStatuDic)
            {
                if (temp.Key.IsSignIn)
                {
                    temp.Value.sprite = OneIDSceneManager.Instance.GetStudentInfoManager.GetAllSprites()[16];
                }
                else
                {
                    temp.Value.sprite = OneIDSceneManager.Instance.GetStudentInfoManager.GetAllSprites()[17];
                }
            }
        }

        private void ClickHeadAndSelect(GameObject go)
        {
           
            if(_isPlaying)
                return;

            
            //Transform parent = go.transform.parent;
            JudgeImageAndSelect(go.transform.parent);
            //OneIDStudent stu = OneIDSceneManager.Instance.SelectTheStudentByName(parent.name);
            //if(stu.IsMagicSqareGiftSelect==true)
            //    return;
            
            //for (int i = 0; i < _headImageSpineList.Count; i++)
            //{
            //    _headImageSpineList[i].Hide();
            //}

            //_currentStudentIndex = parent.GetSiblingIndex();
            
            //parent.GetGameObject(parent.name).Show();
            
        }

        void JudgeImageAndSelect(Transform parent)
        {
            OneIDStudent stu = OneIDSceneManager.Instance.SelectTheStudentByName(parent.name);
            if (stu.IsMagicSqareGiftSelect == true)
                return;
            OneIDSceneManager.Instance.PlayCommonSound(10);
            for (int i = 0; i < _headImageSpineList.Count; i++)
            {
                _headImageSpineList[i].Hide();
            }

            _currentStudentIndex = parent.GetSiblingIndex();

            parent.GetGameObject(parent.name).Show();
            if (stu.IsSignIn)
            {
                Debug.LogError("学生："+stu.Name+"已经签到了，所以可以玩");
                SpineManager.instance.DoAnimation(_clickSpine, "ck1", false);
            }
            else
            {
                Debug.LogError("学生："+stu.Name+"还没签到，所以不能玩");
                SpineManager.instance.DoAnimation(_clickSpine, "ck3", false);
            }
        }

        void MoveToSelectNextStudent()
        {
            _moveToNextStudentIndex++;
            if (_currentStudentIndex < OneIDSceneManager.Instance.GetStudentNumber()-1)
            {
                _currentStudentIndex++;
            }
            else
            {
                _currentStudentIndex = 0;
            }

            OneIDStudent stu =
                OneIDSceneManager.Instance.SelectTheStudentByName(_studentInfoPanel.GetChild(_currentStudentIndex).name);
            if (stu.IsMagicSqareGiftSelect || !stu.IsSignIn)
            {
                Debug.LogError("学生："+stu.Name+"已经登记或注册");
                if (_moveToNextStudentIndex <= OneIDSceneManager.Instance.GetStudentNumber())
                {
                    MoveToSelectNextStudent();
                }
            }
            else
            {
                for (int i = 0; i < _headImageSpineList.Count; i++)
                {
                    _headImageSpineList[i].Hide();
                    
                }

                Transform target = _studentInfoPanel.GetChild(_currentStudentIndex);
                target.GetGameObject(target.name).Show();
                _moveToNextStudentIndex = 0;
            }
        }


        void ClickEvent(GameObject go)
        {
            if (SpineManager.instance.GetCurrentAnimationName(_clickSpine) == "ck1")
            {
                OneIDSceneManager.Instance.PlayCommonSound(13);
                _isPlaying = true;
                SpineManager.instance.DoAnimation(_clickSpine, "ck2", false,
                    () => SpineManager.instance.DoAnimation(_clickSpine, "ck3", false, () =>
                    {
                        RandomCard2();
                    }));
                
            }

           
        }

       

        void RandomCard2()
        {
            StartCoroutine(RandomCardIE2());
        }


        IEnumerator RandomCardIE2()
        {
            float currentAccelerate = 20f;
            float waitTimer = 1f;
            bool isSlowDown = false;
            _randomNumber = Random.Range(1, 7);
            float targetRotateValue = GetTargetRotateValueByNumber(_randomNumber);
            bool isTheRightPos = false;
            bool isNextStep = false;
            OneIDSceneManager.Instance.PlayCommonSound(24);
            while (true)
            {
                _needle.transform.Rotate(Vector3.back * currentAccelerate);
                //print(_needle.eulerAngles.z);
                yield return new WaitForFixedUpdate();
                waitTimer -= Time.fixedDeltaTime;
                if (waitTimer <= 0)
                {
                    isSlowDown = true;
                }

                if (isSlowDown&& currentAccelerate>=2f)
                {
                    currentAccelerate /= 1.01f;
                }

                if (isSlowDown && currentAccelerate <= 2f&& isNextStep==false)
                {
                    if (_needle.eulerAngles.z > targetRotateValue)
                    {
                        isTheRightPos = true;
                        isNextStep = true;
                    }
                   
                }

                if (isTheRightPos&& isNextStep)
                {
                    if (_needle.eulerAngles.z - targetRotateValue <= 90f&& currentAccelerate >= 1f)
                    {
                        currentAccelerate /= 1.01f;
                    }

                    if (_needle.eulerAngles.z <= targetRotateValue)
                    {
                        ShowStudentCardContent();
                        OneIDSceneManager.Instance.PlayCommonSound(25);
                        //Debug.LogError(_randomNumber+"  :"+targetRotateValue);
                        break;
                    }
                }
            }
        }

        void ShowStudentCardContent()
        {
            ShowTargetCardInPanel(_randomNumber);
            RandomCardColorAndShowTargetColor(_cardPanel.GetChild(_currentStudentIndex).gameObject);
        }

        void RandomCardColorAndShowTargetColor(GameObject target)
        {
            StartCoroutine(RandomCardColorAndShowTargetColorIE(target));
        }

        IEnumerator RandomCardColorAndShowTargetColorIE(GameObject target)
        {
            //OneIDSceneManager.Instance.PlayCommonSound(13);
            int i = Random.Range(1, 7);
            float timer = 0f;
            while (true)
            {
                i = Random.Range(1, 7);
                SpineManager.instance.DoAnimation(target, "kp-A" + i, false);
                yield return new WaitForSeconds(0.2f);
                timer += 0.1f;
                if (timer > 2f)
                {
                    
                    ShowTargetBGColorAndExcuteNext(target);
                    OneIDSceneManager.Instance.PlayCommonSound(12);
                    break;
                }
            }
        }

        void ShowTargetBGColorAndExcuteNext(GameObject target)
        {
            int showNumber = _randomNumber;
            switch (_randomNumber)
            {
                case 6:
                    showNumber = 3;
                    break;
                case 3:
                    showNumber = 5;
                    break;
                case 5:
                    showNumber = 6;
                    break;

            }
            SpineManager.instance.DoAnimation(target, "kp-A" + showNumber, false, () =>
            {
                _lightEffect.gameObject.Show();
                _lightEffect.SetParent(target.transform);
                _lightEffect.localPosition=Vector3.zero;
                SpineManager.instance.DoAnimation(_lightEffect.gameObject, "star", false,
                    () => SpineManager.instance.DoAnimation(_lightEffect.gameObject, "star1", false,
                        () => _lightEffect.gameObject.Hide()));
                SpineManager.instance.DoAnimation(target, "kp-a" + showNumber, false, () =>
                {
                    SpineManager.instance.DoAnimation(target, "kp-b" + showNumber, false, () =>
                    {
                        ResetStudentAndButton(target);
                    });
                });
            });
        }

        void ResetStudentAndButton(GameObject target)
        {
            
            OneIDSceneManager.Instance.SelectTheStudentByName(target.name).IsMagicSqareGiftSelect = true;
            SpineManager.instance.DoAnimation(_randomCardSpine, "zp0", false);
            
            bool isAllStudentSlect = true;
            foreach (var temp in OneIDSceneManager.Instance.GetAllStudentDic())
            {
                if (!temp.Value.IsMagicSqareGiftSelect||!temp.Value.IsSignIn)
                    isAllStudentSlect = false;
            }
            if (!isAllStudentSlect)
            {
                SpineManager.instance.DoAnimation(_clickSpine, "ck1", false);
                _isPlaying = false;
                MoveToSelectNextStudent();
            }
            else
            {
                SpineManager.instance.DoAnimation(_clickSpine, "ck3", false);
            }

            
            
            
        }


        float GetTargetRotateValueByNumber(int number)
        {
            float targetValue = 0f;
            switch (number)
            {
                case 1:
                    targetValue = 87f;
                    break;
                case 2:
                    targetValue = 26f;
                    break;
                case 3:
                    targetValue = 326f;
                    break;
                case 4:
                    targetValue = 266f;
                    break;
                case 5:
                    targetValue = 207f;
                    break;
                case 6:
                    targetValue = 147f;
                    break;
            }

            return targetValue;
        }

        

        void RandomCard()
        {
            StartCoroutine(RandomCardIE());
        }

        IEnumerator RandomCardIE()
        {
            float currentTimer = 0.05f;
            int i = 0;
            float waitTimer = 1f;
            bool isSlowDown = false;
            _randomNumber = Random.Range(1, 7);
            while (true)
            {
               
                i++;
                ShowTargetCardInPanel(i);
                if (i==_randomNumber&&isSlowDown&& currentTimer>=0.4f)
                {
                    //ShowStudentCardContent();
                    break;
                }
                yield return new WaitForSeconds(currentTimer);
                waitTimer -= currentTimer;
                if (waitTimer <= 0)
                {
                    isSlowDown = true;
                }

                if (isSlowDown&& currentTimer<=0.4f)
                {
                    currentTimer *= 1.1f;
                }

                
                print(currentTimer);

                if (i >= 6)
                    i = 0;
            }
        }

       

        void ShowTargetCardInPanel(int index)
        {
            SpineManager.instance.DoAnimation(_randomCardSpine, "zp" + index, false);
        }
    }
}