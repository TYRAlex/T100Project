using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public enum QuestionType
    {
        Single,         //��ѡ
        Multiple,       //��ѡ
        Judge,          //�ж�
        Answer,         //�����
    }

    public class Course7410Part4
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
        private GameObject[] _questionBg;     //��Ŀ����
        private QuestionType[] _questionType;     //��Ŀ����
        private Transform _click;
        private GameObject[] _clickArray;   //ÿ�ڿε���ĸ�����
        private Empty4Raycast[] _clickEmpty;    //���е������

        Dictionary<int, int[]> _answerDic;  //���ֵ�,��Ϊ_level, ֵΪ������
        private GameObject _a;  //��һ��
        private GameObject _b;  //��һ��
        private GameObject _c;  //ȷ��

        private int _level;
        private bool _canClickBtn;             //�ܷ����������
        private bool _canClickSure;            //�ܷ���ȷ�ϼ���          
        private bool _canClickChoose;          //�ܷ���ѡ���

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;

            //�������
            _question = curTrans.Find("Question");
            _questionBg = new GameObject[_question.childCount];
            for (int i = 0; i < _question.childCount; i++)
            {
                _questionBg[i] = _question.GetChild(i).gameObject;
            }

            //����������
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

            //��������ȷ��
            _a = curTrans.GetGameObject("a");
            _b = curTrans.GetGameObject("b");
            _c = curTrans.GetGameObject("c");
            Util.AddBtnClick(_a.transform.GetChild(0).gameObject, ChangeQuestion);
            Util.AddBtnClick(_b.transform.GetChild(0).gameObject, ChangeQuestion);
            Util.AddBtnClick(_c.transform.GetChild(0).gameObject, SureClick);

            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            #region ��Ҫ�����޸ĵĲ���
            //���ѡ��������
            _questionType = new QuestionType[3] { QuestionType.Multiple, QuestionType.Multiple,QuestionType.Answer };

            //�����ȷѡ��
            _answerDic = new Dictionary<int, int[]>();
            _answerDic.Add(0, new int[3] { 0,2,3 });
            _answerDic.Add(1, new int[2] { 1, 2 });
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
            CheckSureShow();
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

        void TalkClick()
        {
            PlayOnClickSound();
            HideVoiceBtn();
            if (talkIndex == 1)
            {

            }

            talkIndex++;
        }

        //���ݵȼ�show��Ŀ��������
        void ShowCurQuestion()
        {
            //����������Ŀ��������
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
                {
                    _clickArray[i].Show();
                    //���ص�bellͷ��͹�ѡЧ��
                    for (int j = 0; j < _clickArray[i].transform.childCount; j++)
                    {
                        Transform tra = _clickArray[i].transform.GetChild(j);
                        tra.GetGameObject("select").Hide();
                        InitSpine(tra.GetGameObject("select"));
                        PlaySpine(tra.GetGameObject("select"), "0", null, false);
                        tra.GetGameObject("head").Hide();
                    }
                }
            }
        }

        //�Ƿ�����������
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

        //���������ȷ�ϼ�
        void CheckSureShow()
        {
            if (_questionType[_level] == QuestionType.Answer)
                _c.Hide();
            else
                _c.Show();
        }

        //������
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
                    CheckSureShow();
                    CheckBtnShow();
                });
            }
        }

        //���ѡ��
        private void ClickEvent(GameObject obj)
        {
            if (_canClickChoose)
            {
                _canClickBtn = false;
                _canClickSure = false;
                _canClickChoose = false;
                PlayOnClickSound();

                if (!obj.transform.GetGameObject("select").activeSelf)
                {
                    //�Ƕ�ѡ��
                    if (_questionType[_level] != QuestionType.Multiple)
                    {
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            _clickArray[_level].transform.GetChild(i).GetGameObject("select").Hide();
                        }
                    }

                    InitSpine(obj.transform.GetGameObject("select"));
                    obj.transform.GetGameObject("select").Show();
                    PlaySpine(obj.transform.GetGameObject("select"), "animation",
                    () =>
                    {
                        _canClickBtn = true;
                        _canClickSure = true;
                        _canClickChoose = true;
                    }, false);
                }
                else
                {
                    obj.transform.GetGameObject("select").Hide();
                    _canClickBtn = true;
                    _canClickSure = true;
                    _canClickChoose = true;
                }
            }
        }

        //���ȷ�ϼ�
        private void SureClick(GameObject obj)
        {
            if (_canClickSure)
            {
                _canClickBtn = false;
                _canClickChoose = false;
                _canClickSure = false;
                float time = SoundManager.instance.PlayClip(9);
                PlaySpine(obj.transform.parent.gameObject, obj.transform.parent.gameObject.name + "2",
                () =>
                {
                    if (_questionType[_level] != QuestionType.Multiple)
                    {
                        int answerIndex = 10;   //��ʼֵΪ10��ģ�ⲻѡ�Ĵ������

                        //���ѡ�е����ĸ�ѡ��
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            GameObject o = _clickArray[_level].transform.GetChild(i).GetGameObject("select");
                            if (o.activeSelf)
                            {
                                answerIndex = i;
                                break;
                            }
                        }

                        //���ص���ѡЧ��
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            _clickArray[_level].transform.GetChild(i).GetGameObject("select").Hide();
                        }

                        if (answerIndex == 10)
                        {
                            Delay(time, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false); });
                            GameObject head = _clickArray[_level].transform.GetChild(_answerDic[_level][0]).GetGameObject("head");
                            InitSpine(head);
                            head.Show();
                            PlaySpine(head, "31", () =>
                            {
                                PlaySpine(head, "3", () => { _canClickBtn = true; }, true);
                            }, false);
                        }
                        else if (answerIndex == _answerDic[_level][0])
                        {
                            Delay(time, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false); });
                            GameObject head = _clickArray[_level].transform.GetChild(answerIndex).GetGameObject("head");
                            InitSpine(head);
                            head.Show();
                            PlaySpine(head, "11", () =>
                            {
                                PlaySpine(head, "1", () => { _canClickBtn = true; }, true);
                            }, false);
                        }
                        else
                        {
                            Delay(time, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false); });
                            GameObject head1 = _clickArray[_level].transform.GetChild(answerIndex).GetGameObject("head");
                            GameObject head2 = _clickArray[_level].transform.GetChild(_answerDic[_level][0]).GetGameObject("head");
                            InitSpine(head1);
                            InitSpine(head2);
                            head1.Show();
                            head2.Show();
                            PlaySpine(head1, "21", () =>
                            {
                                PlaySpine(head1, "2", () => { _canClickBtn = true; }, true);
                            }, false);

                            PlaySpine(head2, "11", () =>
                            {
                                PlaySpine(head2, "1", () => { _canClickBtn = true; }, true);
                            }, false);
                        }
                    }
                    else
                    {
                        bool _isFalse = false;  //�ж��Ƿ�Ӧ��������Ч

                        //����������ѡ���ж�ѡ��Դ���©ѡ״��
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            bool _trueAns = false;  //����������ѡ�����Ƿ�����ȷ��ѡ��
                            bool _clickAns = false;  //����������ѡ�����Ƿ���ѡ�е�ѡ��

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

                            //�ܹ��������
                            if (_trueAns && _clickAns)
                            {
                                //���߶��У���ѡ����
                                _clickArray[_level].transform.GetChild(i).GetGameObject("select").Hide();
                                GameObject head = _clickArray[_level].transform.GetChild(i).GetGameObject("head");
                                InitSpine(head);
                                head.Show();
                                PlaySpine(head, "11", () =>
                                {
                                    PlaySpine(head, "1", () => { _canClickBtn = true; }, true);
                                }, false);
                            }
                            else if (_trueAns && !_clickAns)
                            {
                                //��ȷѡ���е�ûѡ�У���Ϊ©ѡ
                                GameObject head = _clickArray[_level].transform.GetChild(i).GetGameObject("head");
                                InitSpine(head);
                                head.Show();
                                PlaySpine(head, "31", () =>
                                {
                                    PlaySpine(head, "3", () => { _canClickBtn = true; }, true);
                                }, false);
                                _isFalse = true;
                            }
                            else if (!_trueAns && _clickAns)
                            {
                                //��ȷѡ��û�е�ѡ�У���Ϊ��ѡ
                                _clickArray[_level].transform.GetChild(i).GetGameObject("select").Hide();
                                GameObject head = _clickArray[_level].transform.GetChild(i).GetGameObject("head");
                                InitSpine(head);
                                head.Show();
                                PlaySpine(head, "21", () =>
                                {
                                    PlaySpine(head, "2", () => { _canClickBtn = true; }, true);
                                }, false);
                                _isFalse = true;
                            }
                            else
                            {
                                //��ȷѡ��û��Ҳûѡ�У���������
                            }
                        }

                        if (_isFalse)
                            Delay(time, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false); });
                        else
                            Delay(time, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false); });

                    }
                }, false);
            }
        }

        #region ���ú���

        #region ������ť

        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }

        #endregion

        #region ���غ���ʾ

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }
        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
        }

        #endregion

        #region Spine���

        private void InitSpine(GameObject obj)
        {
            obj.GetComponent<SkeletonGraphic>().Initialize(true);
        }

        private void InitSpines(Transform parent, bool isKong = true, Action initCallBack = null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;
                if (isKong)
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name); });
                else
                    PlaySpine(child, child.name);
            }
            initCallBack?.Invoke();
        }

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

        private GameObject FindGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        #endregion

        #region ��Ƶ���
        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private float PlayBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, isLoop);
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, isLoop);
            return time;
        }

        private float PlayCommonBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, index, isLoop);
            return time;
        }

        private float PlayCommonVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, index, isLoop);
            return time;
        }

        private float PlayCommonSound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, index, isLoop);
            return time;
        }

        private void StopAllAudio()
        {
            SoundManager.instance.StopAudio();
        }

        private void StopAudio(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
        }

        private void StopAudio(string audioName)
        {
            SoundManager.instance.Stop(audioName);
        }

        #endregion

        #region ��ʱ���

        private void Delay(float delay, Action callBack)
        {
            mono.StartCoroutine(IEDelay(delay, callBack));
        }

        private void UpDate(bool isStart, float delay, Action callBack)
        {
            mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
        }

        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        IEnumerator IEUpdate(bool isStart, float delay, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
            }
        }

        #endregion

        #region ֹͣЭ��

        private void StopAllCoroutines()
        {
            mono.StopAllCoroutines();
        }

        private void StopCoroutines(string methodName)
        {
            mono.StopCoroutine(methodName);
        }

        private void StopCoroutines(IEnumerator routine)
        {
            mono.StopCoroutine(routine);
        }

        private void StopCoroutines(Coroutine routine)
        {
            mono.StopCoroutine(routine);
        }

        #endregion

        #region Bell����

        private void BellSpeck(GameObject go, SoundManager.SoundType type = SoundManager.SoundType.VOICE, int index = 0, Action specking = null, Action speckend = null)
        {
            mono.StartCoroutine(SpeckerCoroutine(go, type, index, specking, speckend));
        }

        IEnumerator SpeckerCoroutine(GameObject go, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {

            string daiJi = "DAIJI";
            string speak = "DAIJIshuohua";

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, daiJi);
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, speak);

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, daiJi);
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

        #region �������

        private void AddEvents(Transform parent, PointerClickListener.VoidDelegate callBack)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                RemoveEvent(child);
                AddEvent(child, callBack);
            }
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }
        #endregion

        #region �޸�Rect���

        private void SetPos(RectTransform rect, Vector2 pos)
        {
            rect.anchoredPosition = pos;
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }

        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null)
        {
            rect.DOAnchorPos(v2, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            callBack1?.Invoke();
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack2?.Invoke(); });
        }


        #endregion

        #region ���ֻ�
        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //�ո�ǻ���        
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(0.1f);
                text.text += str[i];
                i++;
            }
            callBack?.Invoke();
            yield break;
        }
        #endregion

        #endregion
    }
}
