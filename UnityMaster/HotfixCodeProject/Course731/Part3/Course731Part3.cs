using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ILFramework.HotClass
{
    public enum QuestionType
    {
        Single,         //单选
        Multiple,       //多选
        Judge,          //判断
    }

    public class Course731Part3
    {
        #region 通用变量
        int talkIndex;

        bool isPlaying = false;

        MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        GameObject Bg;
        BellSprites bellTextures;

        GameObject Max;
        #endregion

        #region 游戏变量
        int flag = 0;

        bool isDragEnd = false;

        GameObject star;
        GameObject unClickMask;

        mILDrager[] drags;
        mILDroper[] drops;
        #endregion

        #region 选择题变量
        GameObject _c;  //确认
        GameObject _maskC;  //遮挡确认键的遮罩
        GameObject[] _questionBg;     //题目背景
        GameObject[] _clickArray;   //每节课点击的父物体
        GameObject background;

        Transform _questionTra;
        Transform _click;

        QuestionType[] _questionType;     //题目类型

        Empty4Raycast[] _clickEmpty;    //所有点击物体

        Dictionary<int, int[]> _answerDic;  //答案字典,键为_level, 值为答案数组

        int _level;
        bool _canClick;
        #endregion
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            Input.multiTouchEnabled = false;
            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            LoadQuestion();

            LoadGame();

            GameInit();

            GameStart();
        }

        void LoadGame()
        {
            curTrans.Find("Text").gameObject.SetActive(true);

            star = curTrans.Find("Star").gameObject;
            star.SetActive(false);

            unClickMask = curTrans.Find("unClickMask").gameObject;
            unClickMask.SetActive(false);

            drops = curTrans.Find("Ball").GetComponentsInChildren<mILDroper>(true);

            drags = curTrans.Find("UI").GetComponentsInChildren<mILDrager>(true);
            drags = Sort(drags);

            for (int i = 0, n = drags.Length; i < n; ++i)
            {
                drags[i].gameObject.SetActive(true);
                drags[i].transform.localScale = Vector2.one;
                drags[i].SetDragCallback(OnBeginDrag, null, OnEndDrag);
                drags[i].canMove = true;

                drags[i].transform.GetRectTransform().anchoredPosition = new Vector2((i-1) * -550, -200);
            }

            for (int i = 0, n = drops.Length; i < n; ++i)
            {
                drops[i].gameObject.SetActive(true);
            }

            curTrans.Find("Question").gameObject.SetActive(false);
            curTrans.Find("Click").gameObject.SetActive(false);
        }

        void LoadQuestion()
        {
            _maskC = curTrans.Find("mask").gameObject;

            background = curTrans.Find("Background").gameObject;
            background.SetActive(false);

            //问题相关
            _questionTra = curTrans.Find("Question");
            _questionBg = new GameObject[_questionTra.childCount];
            for (int i = 0; i < _questionTra.childCount; i++)
            {
                _questionBg[i] = _questionTra.GetChild(i).gameObject;
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
            _c = curTrans.GetGameObject("c");
            _c.Hide();
            _c.GetComponent<SkeletonGraphic>().Initialize(true);

            Util.AddBtnClick(_c.transform.GetChild(0).gameObject, SureClick);

            SoundManager.instance.ShowVoiceBtn(false);

            #region 需要进行修改的部分
            //添加选择题类型
            _questionType = new QuestionType[1] { QuestionType.Multiple };

            //添加正确选项
            _answerDic = new Dictionary<int, int[]>
            {
                { 0, new int[3] { 0, 2, 3 } }
            };
            #endregion
        }

        void GameInit()
        {
            talkIndex = 1;
            flag = 0;

            _level = 0;
            _canClick = true;
            _maskC.Hide();

            //先隐藏所有题目与点击区域
            for (int i = 0; i < _questionBg.Length; i++)
            {
                _questionBg[i].Hide();
            }

            for (int i = 0; i < _clickArray.Length; i++)
            {
                _clickArray[i].Hide();
            }

            SpineManager.instance.DoAnimation(_c, _c.name, false);
        }

        void GameStart()
        {
            Max.SetActive(false);
            isPlaying = true;
            unClickMask.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            { 
                isPlaying = false;
                unClickMask.SetActive(false); 
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    curTrans.Find("Text").gameObject.SetActive(false);

                    for (int i = 0; i < drags.Length; ++i)
                    {
                        drags[i].gameObject.SetActive(false);
                    }

                    for (int i = 0; i < drops.Length; ++i)
                    {
                        drops[i].gameObject.SetActive(false);
                    }

                    _c.Show();
                    background.SetActive(true);
                    ShowCurQuestion();
                    curTrans.Find("Question").gameObject.SetActive(true);
                    curTrans.Find("Click").gameObject.SetActive(true);
                    unClickMask.SetActive(true);

                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () =>
                    {
                        unClickMask.SetActive(false);
                    }));  
                }));
            }

            talkIndex++;
        }

        #region 拖拽
        void OnBeginDrag(Vector3 pos, int type, int index)
        {
            SoundManager.instance.PlayClip(9);

            isDragEnd = false;

            drags[index].transform.SetAsLastSibling();
            drags[index].transform.position = Input.mousePosition;

            drags[index].transform.DOScale(Vector2.one * 1.15f, 0.2f).SetEase(Ease.OutCubic);
        }

        void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            isDragEnd = true;
            unClickMask.SetActive(true);

            drags[index].transform.localScale = Vector2.one;

            if (!isMatch)
            {
                SoundManager.instance.PlayClip(5);

                drags[index].DoReset();

                drags[index].transform.GetRectTransform().DOShakePosition(1f, 3).OnComplete(()=>
                {
                    unClickMask.SetActive(false);
                });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                float _time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5 + index);

                drags[index].gameObject.SetActive(false);

                mono.StartCoroutine(WaitFor(_time, ()=>
                {
                    if (++flag == 3) SoundManager.instance.ShowVoiceBtn(true);

                    unClickMask.SetActive(false);
                }));

                drags[index].transform.position = drops[index].transform.position;
                drags[index].canMove = false;
            }
        }
        #endregion

        #region 通用方法
        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        //排列拖拽数组
        mILDrager[] Sort(mILDrager[] list)
        {
            int n = list.Length;
            mILDrager[] ret = new mILDrager[n];

            for (int i = 0; i < n; ++i)
            {
                ret[list[i].index] = list[i];
            }

            return ret;
        }
        #endregion

        #region 选择题方法
        //根据等级show题目与点击区域
        void ShowCurQuestion()
        {
            //先隐藏所有题目与点击区域
            for (int i = 0; i < _questionBg.Length; i++)
            {
                _questionBg[i].Hide();
            }
            for (int i = 0; i < _clickArray.Length; i++)
            {
                _clickArray[i].Hide();
            }

            //展示对应的背景与点击区域，隐藏掉确认反馈的头像和选中效果
            _questionBg[_level].Show();
            for (int i = 0; i < _questionBg[_level].transform.childCount; i++)
            {
                _questionBg[_level].transform.GetChild(i).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_questionBg[_level].transform.GetChild(i).gameObject, "kong", false);
            }
            _clickArray[_level].Show();
            for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
            {
                Transform tra = _clickArray[_level].transform.GetChild(i).GetChild(0);
                tra.GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(tra.gameObject, "0", false);
            }
        }

        //点击选项
        void ClickEvent(GameObject obj)
        {
            if (_canClick)
            {
                SoundManager.instance.PlayClip(9);

                _canClick = false;
                if (_questionType[_level] != QuestionType.Multiple)
                {
                    if (SpineManager.instance.GetCurrentAnimationName(obj.transform.GetChild(0).gameObject) == "0")
                    {
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            SpineManager.instance.DoAnimation(_clickArray[_level].transform.GetChild(i).GetChild(0).gameObject, "0", false);
                        }


                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "animation", false,
                        () =>
                        {
                            _canClick = true;
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "0", false,
                        () =>
                        {
                            _canClick = true;
                        });
                    }
                }
                else
                {

                    if (SpineManager.instance.GetCurrentAnimationName(obj.transform.GetChild(0).gameObject) == "0")
                    {
                        obj.transform.GetChild(0).gameObject.Show();
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "animation", false,
                        () =>
                        {
                            _canClick = true;
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "0", false,
                        () =>
                        {
                            _canClick = true;
                        });
                    }
                }
            }
        }

        void SureClick(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(9);
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.transform.parent.gameObject.name + "2", false,
                () =>
                {
                    if (_questionType[_level] != QuestionType.Multiple)
                    {
                        _canClick = false;
                        int answerIndex = 10;   //初始值为10，模拟不选的错误情况
                        //获得选中的是哪个选项
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            GameObject o = _clickArray[_level].transform.GetChild(i).GetChild(0).gameObject;
                            if (SpineManager.instance.GetCurrentAnimationName(o) != "0")
                            {
                                answerIndex = i;
                            }
                        }

                        _clickArray[_level].Hide();

                        if (answerIndex == 10)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                            GameObject head = _questionBg[_level].transform.GetChild(_answerDic[_level][0]).gameObject;
                            head.Show();
                            SpineManager.instance.DoAnimation(head, "31", false, () =>
                            {
                                SpineManager.instance.DoAnimation(head, "3", true, () => { _canClick = true; });
                            });

                        }
                        else if (answerIndex == _answerDic[_level][0])
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);
                            GameObject head = _questionBg[_level].transform.GetChild(answerIndex).gameObject;
                            head.Show();
                            SpineManager.instance.DoAnimation(head, "11", false, () =>
                            {
                                SpineManager.instance.DoAnimation(head, "1", true, () => { _canClick = true; });
                            });
                        }
                        else
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                            GameObject head1 = _questionBg[_level].transform.GetChild(answerIndex).gameObject;
                            GameObject head2 = _questionBg[_level].transform.GetChild(_answerDic[_level][0]).gameObject;
                            head1.Show();
                            head2.Show();
                            SpineManager.instance.DoAnimation(head1, "21", false, () =>
                            {
                                SpineManager.instance.DoAnimation(head1, "2", true, () => { _canClick = true; });
                            });
                            SpineManager.instance.DoAnimation(head2, "11", false, () =>
                            {
                                SpineManager.instance.DoAnimation(head2, "1", true, () => { _canClick = true; });
                            });
                        }
                    }
                    else
                    {
                        _canClick = false;
                        int index = 0;
                        //获得选中的选项数量
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            GameObject o = _clickArray[_level].transform.GetChild(i).GetChild(0).gameObject;
                            if (SpineManager.instance.GetCurrentAnimationName(o) != "0")
                            {
                                index++;
                            }
                        }

                        int[] answerIndex = new int[index];
                        index = 0;
                        //储存选中的选项
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            GameObject o = _clickArray[_level].transform.GetChild(i).GetChild(0).gameObject;
                            if (SpineManager.instance.GetCurrentAnimationName(o) != "0")
                            {
                                answerIndex[index] = i;
                            }
                        }

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
                                    //j = _answerDic.Count;
                                    j = _answerDic[_level].Length;
                                }
                            }
                            if (SpineManager.instance.GetCurrentAnimationName(_clickArray[_level].transform.GetChild(i).GetChild(0).gameObject) != "0")
                                _clickAns = true;

                            //总共四种情况
                            if (_trueAns && _clickAns)
                            {
                                //两者都有，则选对了
                                GameObject head = _questionBg[_level].transform.GetChild(i).gameObject;
                                head.Show();
                                SpineManager.instance.DoAnimation(head, "11", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(head, "1", true, () => { _canClick = true; });
                                });

                                
                            }
                            else if (_trueAns && !_clickAns)
                            {
                                //正确选项有但没选中，则为漏选
                                GameObject head = _questionBg[_level].transform.GetChild(i).gameObject;
                                head.Show();
                                SpineManager.instance.DoAnimation(head, "31", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(head, "3", true, () => { _canClick = true; });
                                });
                                _isFalse = true;
                            }
                            else if (!_trueAns && _clickAns)
                            {
                                //正确选项没有但选中，则为错选
                                GameObject head = _questionBg[_level].transform.GetChild(i).gameObject;
                                head.Show();
                                SpineManager.instance.DoAnimation(head, "21", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(head, "2", true, () => { _canClick = true; });
                                });
                                _isFalse = true;

                                
                            }
                            else
                            {
                                //正确选项没有也没选中，不做操作
                            }
                        }
                        if (_isFalse)
                        {
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4));
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                        } 
                        else
                        {
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3));
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);
                        }
                        
                        
                        _clickArray[_level].Hide();
                        _canClick = true;
                    }

                    _maskC.Show();
                });
            }
        }
        #endregion

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
    }
}
