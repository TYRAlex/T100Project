using System;
using System.Collections;
using System.Collections.Generic;
using ILFramework;
using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class IntergrateManager : MonoBehaviour
    {
        private GameObject _clickInfoGameObject;

        private GameObject _infoSpine;

        private GameObject _mainTarget;

        private GameObject _showFrame;
        private GameObject _clickStage1;
        private GameObject _clickStage2;
        private GameObject _backMaskClickArea;

        private Text _studentInfoText;

        private bool _canClose = true;

        private OneIDStudent _currentStudent;
        
        private void Awake()
        {
            _mainTarget = this.transform.GetGameObject("Main");
            _clickInfoGameObject = this.transform.GetGameObject("ClickInfo");
            _infoSpine = this.transform.GetGameObject("Info");
            _clickStage1 = this.transform.GetGameObject("ClickStage1");
            _clickStage2 = this.transform.GetGameObject("ClickStage2");
            _showFrame = this.transform.GetGameObject("ShowFrame");
            _backMaskClickArea = this.transform.GetGameObject("Mask");
            _studentInfoText = this.transform.GetTargetComponent<Text>("Text");
            PointerClickListener.Get(_clickInfoGameObject).clickDown = ClickAndShowIntergrateInfo;
            PointerClickListener.Get(_backMaskClickArea).clickDown = ClickAndBackToTheMainScene;
            PointerClickListener.Get(_clickStage2).clickDown = TestClickAndShow;
             PointerClickListener.Get(_clickStage1).clickDown = TestClickAndShow;     
        }

       

        public void JudgeStudentAndExcuteNext(OneIDStudent stu)
        {
            //Debug.LogError("xxxxxx"+stu.Name);
            _currentStudent = stu;
            //Debug.LogError("当前："+_currentStudent.Name+"的分数为："+_currentStudent.GetScore());
            ShowTheStudentNameInfo(stu);
            if (stu.GetScore() < 500f)
            {
                
                ShowStage(1);
                //Debug.LogError("显示");
            }
            else
            {
                ShowStage(2);
            }
        }

        private void ClickAndBackToTheMainScene(GameObject go)
        {
            if (_canClose)
            {
                if (_showFrame.activeSelf)
                {
                    _mainTarget.Show();
                    if (_currentStudent.GetScore() < 500)
                    {
                        SpineManager.instance.DoAnimation(_mainTarget, "m1",false);
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(_mainTarget, "m2", false,
                            () => SpineManager.instance.DoAnimation(_mainTarget, "m2-1", false));
                    }

                    _showFrame.Hide();
                    
                }
                if(_infoSpine.activeSelf)
                    _infoSpine.Hide();
            }
        }
        
        

        private void TestClickAndShow(GameObject go)
        {
            int number = go.name == "ClickStage2" ? 2 : 1;
            ShowStage(number);
            // _showFrame.Show();
            // _showFrame.transform.SetAsLastSibling();
            // _canClose = false;
            // string ani = "p1";
            // if (go.name.Equals("ClickStage2"))
            // {
            //     ani = "p2";
            // }
            //
            // SpineManager.instance.DoAnimation(_showFrame, ani, false,
            //     () => SpineManager.instance.DoAnimation(_showFrame, ani + "-1", false, () => _canClose = true));
        }

        void ShowStage(int stageNumber)
        {
            _showFrame.Show();
            _showFrame.transform.SetAsLastSibling();
            _canClose = false;
            string ani = "p1";
            if (stageNumber == 2)
            {
                ani = "p2";
            }

            SpineManager.instance.DoAnimation(_showFrame, ani, false, () =>
            {
                _canClose = true;
                _mainTarget.Show();
                if (_currentStudent.GetScore() < 500)
                {
                    SpineManager.instance.DoAnimation(_mainTarget, "m1",false);
                }
                else
                {
                    SpineManager.instance.DoAnimation(_mainTarget, "m2", false,
                        () => SpineManager.instance.DoAnimation(_mainTarget, "m2-1", false));
                }

                _showFrame.Hide();
            });
        }

        public void ShowTheStudentNameInfo(OneIDStudent stu)
        {
            string stuName = stu.ShowName;
            _studentInfoText.text = stuName + "的农场";
        }

        private void ClickAndShowIntergrateInfo(GameObject go)
        {
            _infoSpine.Show();
            _infoSpine.transform.SetAsLastSibling();
            _canClose = false;
            SpineManager.instance.DoAnimation(_infoSpine, "t", false,()=>_canClose=true);
        }

        void Start()
        {
            _infoSpine.Hide();
            //_showFrame.Hide();
            _canClose = true;
            float timer= SpineManager.instance.DoAnimation(_mainTarget, "m1");
            Delay(timer, () =>
            {
                SpineManager.instance.DoAnimation(_mainTarget, "m2", false,
                    () => SpineManager.instance.DoAnimation(_mainTarget, "m2-1", false));
            });
        }


        void Delay(float timer,Action callback)
        {
            StartCoroutine(DelayIE(timer, callback));
        }

        IEnumerator DelayIE(float timer,Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }


    }
}