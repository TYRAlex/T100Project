using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public enum QuestionType
    {
        Single,         //单选
        Multiple,       //多选
        Judge,          //判断
    }

    public class Course935Part3
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;
        private GameObject _mask;

        private Transform _question;
        private GameObject[] _questionBg;         //题目背景
        private QuestionType[] _questionType;     //题目类型
        private Transform _click;
        private GameObject[] _clickArray;         //每节课点击的父物体
        private Empty4Raycast[] _clickEmpty;      //所有点击物体

        Dictionary<int, int[]> _answerDic;  //答案字典,键为_level, 值为答案数组
        private GameObject _a;  //上一题
        private GameObject _b;  //下一题
        private GameObject _c;  //确认

        private int _level;
        private bool _canClickBtn;             //能否点击上下题等
        private bool _canClickSure;            //能否点击确认键等          
        private bool _canClickChoose;          //能否点击选项等

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            SoundManager.instance.ShowVoiceBtn(false);

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;

            //问题相关
            _question = curTrans.Find("Question");
            _questionBg = new GameObject[_question.childCount];
            for (int i = 0; i < _question.childCount; i++)
            {
                _questionBg[i] = _question.GetChild(i).gameObject;
            }

            //点击物体相关
            _click = curTrans.Find("Click");
            _clickArray = new GameObject[_click.childCount];
            for (int i = 0; i < _click.childCount; i++)
            {
                _clickArray[i] = _click.GetChild(i).gameObject;
            }
            _clickEmpty = _click.gameObject.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _clickEmpty.Length; i++)
            {
                Util.AddBtnClick(_clickEmpty[i].gameObject, ClickEvent);
            }

            //上下题与确认
            _a = curTrans.GetGameObject("a");
            _b = curTrans.GetGameObject("b");
            _c = curTrans.GetGameObject("c");
            Util.AddBtnClick(_a.transform.GetChild(0).gameObject, ChangeQuestion);
            Util.AddBtnClick(_b.transform.GetChild(0).gameObject, ChangeQuestion);
            Util.AddBtnClick(_c.transform.GetChild(0).gameObject, SureClick);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            #region 需要进行修改的部分
            //添加选择题类型
            _questionType = new QuestionType[3] { QuestionType.Multiple, QuestionType.Multiple, QuestionType.Multiple };

            //添加正确选项
            _answerDic = new Dictionary<int, int[]>();
            _answerDic.Add(0, new int[2] { 2, 3 });
            _answerDic.Add(1, new int[2] { 0, 3 });
            _answerDic.Add(2, new int[2] { 0, 2 });
            #endregion

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            _level = 0;
            _canClickBtn = true;
            _canClickSure = true;
            _canClickChoose = true;

            ShowCurQuestion();
            CheckBtnShow();

            SpineManager.instance.DoAnimation(_a, _a.name, false);
            SpineManager.instance.DoAnimation(_b, _b.name, false);
            SpineManager.instance.DoAnimation(_c, _c.name, false);
            _mask.Hide();
            bell.Hide();
        }

        void GameStart()
        {

        }


        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bell;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void StartWait(Action method_1 = null, float len = 0)
        {
            mono.StartCoroutine(WaitCoroutine(method_1, len));
        }

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }

            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //根据等级show题目与点击区域
        void ShowCurQuestion()
        {
            //隐藏其他题目与点击区域
            for (int i = 0; i < _questionBg.Length; i++)
            {
                _questionBg[i].Hide();
                if (i == _level)
                    _questionBg[i].Show();
            }
            for (int i = 0; i < _clickArray.Length; i++)
            {
                _clickArray[i].Hide();
                if (i == _level)
                    _clickArray[i].Show();
            }

            //隐藏掉bell头像和勾选效果
            for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
            {
                Transform tra = _clickArray[_level].transform.GetChild(i);
                tra.GetGameObject("select").Hide();
                SpineManager.instance.DoAnimation(tra.GetGameObject("head"), "kong", false);
                tra.GetGameObject("head").Hide();
                //tra.GetGameObject("select").GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                //SpineManager.instance.DoAnimation(tra.GetGameObject("select"), "0", false);
                //tra.GetGameObject("head").GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                //SpineManager.instance.DoAnimation(tra.GetGameObject("head"), "kong", false);
            }
        }

        //是否隐藏上下题
        void CheckBtnShow()
        {
            if (_level == 0)
            {
                _a.Hide();
                _b.Show();
            }
            else if (_level == _questionBg.Length - 1)
            {
                _a.Show();
                _b.Hide();
            }
            else
            {
                _a.Show();
                _b.Show();
            }
        }

        //上下题
        private void ChangeQuestion(GameObject obj)
        {
            if (_canClickBtn)
            {
                _canClickBtn = false;
                GameObject o = obj.transform.parent.gameObject;

                SoundManager.instance.PlayClip(9);
                SpineManager.instance.DoAnimation(o, o.name + "2", false,
                () =>
                {
                    if (obj.name == "a")
                        _level--;
                    else
                        _level++;

                    _canClickBtn = true;
                    _canClickChoose = true;
                    _canClickSure = true;
                    ShowCurQuestion();
                    CheckBtnShow();
                });
            }
        }

        //点击选项
        private void ClickEvent(GameObject obj)
        {
            if (_canClickChoose)
            {
                _canClickChoose = false;
                SoundManager.instance.PlayClip(9);

                if (!obj.transform.GetGameObject("select").activeSelf)
                {
                    //非多选题
                    if (_questionType[_level] != QuestionType.Multiple)
                    {
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            _clickArray[_level].transform.GetChild(i).GetGameObject("select").Hide();
                        }
                    }
                    obj.transform.GetGameObject("select").GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    obj.transform.GetGameObject("select").Show();
                    SpineManager.instance.DoAnimation(obj.transform.GetGameObject("select"), "animation", false,
                    () =>
                    {
                        if (_canClickSure)
                        {
                            _canClickChoose = true;
                        }
                        else
                        {
                            _canClickChoose = false;
                        }
                    });
                }
                else
                {
                    obj.transform.GetGameObject("select").Hide();
                    _canClickChoose = true;
                }
            }
        }

        private void SureClick(GameObject obj)
        {
            if (_canClickSure)
            {
                _canClickBtn = false;
                _canClickChoose = false;
                _canClickSure = false;
                float time = SoundManager.instance.PlayClip(9);
                
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.transform.parent.gameObject.name + "2", false,
                () =>
                {
                    if (_questionType[_level] != QuestionType.Multiple)
                    {
                        int answerIndex = 10;   //初始值为10，模拟不选的错误情况

                        //获得选中的是哪个选项
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            GameObject o = _clickArray[_level].transform.GetChild(i).GetGameObject("select");
                            if (o.activeSelf)
                            {
                                answerIndex = i;
                                break;
                            }
                        }

                        //隐藏掉勾选效果
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            _clickArray[_level].transform.GetChild(i).GetGameObject("select").Hide();
                        }

                        if (answerIndex == 10)
                        {
                            StartWait(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false); }, time);
                            GameObject head = _clickArray[_level].transform.GetChild(_answerDic[_level][0]).GetGameObject("head");
                            head.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            head.Show();
                            SpineManager.instance.DoAnimation(head, "31", false, () =>
                            {
                                SpineManager.instance.DoAnimation(head, "3", true, () => { _canClickBtn = true; });
                            });
                        }
                        else if (answerIndex == _answerDic[_level][0])
                        {
                            StartWait(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false); }, time);
                            GameObject head = _clickArray[_level].transform.GetChild(answerIndex).GetGameObject("head");
                            head.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            head.Show();
                            SpineManager.instance.DoAnimation(head, "11", false, () =>
                            {
                                SpineManager.instance.DoAnimation(head, "1", true, () => { _canClickBtn = true; });
                            });
                        }
                        else
                        {
                            StartWait(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false); }, time);
                            GameObject head1 = _clickArray[_level].transform.GetChild(answerIndex).GetGameObject("head");
                            GameObject head2 = _clickArray[_level].transform.GetChild(_answerDic[_level][0]).GetGameObject("head");
                            head1.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            head2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            head1.Show();
                            head2.Show();
                            SpineManager.instance.DoAnimation(head1, "21", false, () =>
                            {
                                SpineManager.instance.DoAnimation(head1, "2", true, () => { _canClickBtn = true; });
                            });
                            SpineManager.instance.DoAnimation(head2, "11", false, () =>
                            {
                                SpineManager.instance.DoAnimation(head2, "1", true, () => { _canClickBtn = true; });
                            });
                        }
                    }
                    else
                    {
                        bool _isFalse = false;  //判断是否应播错误音效

                        //遍历的所有选项判断选项对错与漏选状况
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            bool _trueAns = false;  //遍历的所有选项里是否有正确的选项
                            bool _clickAns = false;  //遍历的所有选项里是否有选中的选项

                            for (int j = 0; j < _answerDic[_level].Length; j++)
                            {
                                if (_answerDic[_level][j] == i)
                                {
                                    _trueAns = true;
                                    j = _answerDic[_level].Length;
                                }
                            }

                            if (_clickArray[_level].transform.GetChild(i).GetGameObject("select").activeSelf)
                                _clickAns = true;

                            //总共四种情况
                            if (_trueAns && _clickAns)
                            {
                                //两者都有，则选对了
                                _clickArray[_level].transform.GetChild(i).GetGameObject("select").Hide();
                                GameObject head = _clickArray[_level].transform.GetChild(i).GetGameObject("head");
                                head.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                head.Show();
                                SpineManager.instance.DoAnimation(head, "11", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(head, "1", true, () => { _canClickBtn = true; });
                                });
                            }
                            else if (_trueAns && !_clickAns)
                            {
                                //正确选项有但没选中，则为漏选
                                GameObject head = _clickArray[_level].transform.GetChild(i).GetGameObject("head");
                                head.Show();
                                head.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                SpineManager.instance.DoAnimation(head, "31", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(head, "3", true, () => { _canClickBtn = true; });
                                });
                                _isFalse = true;
                            }
                            else if (!_trueAns && _clickAns)
                            {
                                //正确选项没有但选中，则为错选
                                _clickArray[_level].transform.GetChild(i).GetGameObject("select").Hide();
                                GameObject head = _clickArray[_level].transform.GetChild(i).GetGameObject("head");
                                head.Show();
                                head.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                SpineManager.instance.DoAnimation(head, "21", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(head, "2", true, () => { _canClickBtn = true; });
                                });
                                _isFalse = true;
                            }
                            else
                            {
                                //正确选项没有也没选中，不做操作
                            }
                        }

                        if (_isFalse)
                            StartWait(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false); }, time);
                        else
                            StartWait(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false); }, time);

                    }
                });
            }
        }
    }
}
