using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class TD5665Part2
    {

        private int _talkIndex;               //���������Index
        private int _cardPageCurIndex;        //����ҳ��ǰIndex    
        private int _cardPageMinIndex;        //����ҳ��СIndex 
        private int _cardPageMaxIndex;        //����ҳ���Index
        private int _recordOnClickCardNums;   //��¼���Ƶ������
        private int _smallPartCurIndex;       //С���ڵ�ǰIndex

        private MonoBehaviour _mono;

        private GameObject _curGo;            //��ǰ����GameObject
        private GameObject _silde;            //�󶨻����¼�GameObject
        private GameObject _sBD;              //С����
        private GameObject _mask;             //Mask
        private GameObject _last;             //��ȥ��һС�����¼�GameObject
        private GameObject _next;             //��ȥ��һС�����¼�GameObject

        private Transform _materialsParent;   //���ϸ�����    
        private Transform _cardParent;        //���Ƹ�����
        private Transform _arrowsParent;      //��ͷ������

        private RectTransform _materialsParentRect;
        private RectTransform _cardParentRect;

        private Vector2 _prePressPos;

        private List<string> _recordOnClickCardNames;   //��¼���������List   
        private List<bool> _smallPartIsFinishs;         //��¼С�����Ƿ����List

        private SkeletonGraphic _l2sG;
        private SkeletonGraphic _r2sG;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _silde = curTrans.GetGameObject("BG");
            _sBD = curTrans.GetGameObject("sBD");
            _mask = curTrans.GetGameObject("mask");
            _last = curTrans.GetGameObject("LastBtn");
            _next = curTrans.GetGameObject("NextBtn");

            _materialsParent = curTrans.Find("Parts/1");
            _cardParent = curTrans.Find("Parts/2/Mask/Pages");
            _arrowsParent = curTrans.Find("Parts/Switchs");

            _materialsParentRect = _materialsParent.GetRectTransform();
            _cardParentRect = _cardParent.GetRectTransform();

            Input.multiTouchEnabled = false;
            DOTween.KillAll();

            _l2sG = _arrowsParent.transform.Find("L2").GetComponent<SkeletonGraphic>();
            _r2sG = _arrowsParent.transform.Find("R2").GetComponent<SkeletonGraphic>();

            GameInit();
            GameStart();
        }

        #region ��Ϸ�߼�

        /// <summary>
        /// ��Ϸ��ʼ��
        /// </summary>
        void GameInit()
        {
            _talkIndex = 1;
            _smallPartCurIndex = 0;

            _recordOnClickCardNames = new List<string>();
            _smallPartIsFinishs = new List<bool> { false, false };

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            SoundManager.instance.StopAudio();
            _mono.StopAllCoroutines();


            //��ʼ������Spine
            var materialsSpineGo = _materialsParent.Find("0").gameObject;
            materialsSpineGo.GetComponent<SkeletonGraphic>().Initialize(true);
            PlaySpine(materialsSpineGo, materialsSpineGo.name);

            //��ʼ������Spine
            var cardSGs = _cardParent.GetComponentsInChildren<SkeletonGraphic>();
            foreach (var sG in cardSGs)
            {
                sG.Initialize(true);
                PlaySpine(sG.gameObject, sG.gameObject.name);
            }

            //�󶨲��ϵ���¼�
            var materialsE4Rs = _materialsParent.GetComponentsInChildren<Empty4Raycast>();
            foreach (var materialsE4R in materialsE4Rs)
                AddEvent(materialsE4R.gameObject, OnClickMaterials);


            //�󶨿��Ƶ���¼�
            var cardE4Rs = _cardParent.GetComponentsInChildren<Empty4Raycast>();
            foreach (var cardE4R in cardE4Rs)
                AddEvent(cardE4R.gameObject, OnClickCard);

            //�󶨿��Ƽ�ͷ�л��¼�
            var arrowsE4Rs = _arrowsParent.GetComponentsInChildren<Empty4Raycast>(true);
            foreach (var arrowsE4R in arrowsE4Rs)
                AddEvent(arrowsE4R.gameObject, OnClickArrows);



            //��С�����л��¼�
            AddEvent(_last, OnClickLast);
            AddEvent(_next, OnClickNext);

            //�Ƴ�Mask����
            RemoveEvent(_mask);

            //�Ƴ���������
            RemoveSlideEvent();

            //���ò��ϺͿ��Ƹ�����λ��
            _materialsParentRect.anchoredPosition = new Vector2(0, 0);
            _cardParentRect.anchoredPosition = new Vector2(1920, 0);

            ControllePartInteriorSwitchsShow();

            _arrowsParent.gameObject.Show();
            _l2sG.Initialize(true); _r2sG.Initialize(true);
            _arrowsParent.gameObject.Hide();

            _mask.Show(); _sBD.Show();
        }

        /// <summary>
        /// ��Ϸ��ʼ
        /// </summary>
        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            BellSpeck(_sBD, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); });
        }

        /// <summary>
        /// ���������
        /// </summary>
        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(9);
            _mask.Show(); _sBD.Show();

            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sBD, 1, null, () => { _mask.Hide(); _sBD.Hide(); SoundManager.instance.ShowVoiceBtn(true); });
                    break;
                case 2:
                    _smallPartIsFinishs[_smallPartCurIndex] = true;
                    BellSpeck(_sBD, 10, null, () => { _mask.Hide(); _sBD.Hide(); ControllePartInteriorSwitchsShow(); });
                    break;
                case 3:
                    _smallPartIsFinishs[_smallPartCurIndex] = true;
                    BellSpeck(_sBD, 16, null, () => { _mask.Hide(); _sBD.Hide(); ControllePartInteriorSwitchsShow(); });
                    break;

            }
            _talkIndex++;
        }

        /// <summary>
        /// ���ȥ��һС����
        /// </summary>
        /// <param name="go"></param>
        private void OnClickNext(GameObject go)
        {
            _mask.Show();

            ClickFeedback(go.transform, () => {

                _smallPartCurIndex++;

                switch (_smallPartCurIndex)
                {
                    case 1:       //����ȥ����1С����

                        MaterialsToCard(0, 1, 5, () => {
                            Delay(1.0f, () => {
                                _arrowsParent.gameObject.Show();
                                AddSlideSwitchPage();
                            });
                        });
                        break;
                }

                Delay(1.0f, () => { IsBorder(); _mask.Hide(); });
                ControllePartInteriorSwitchsShow();

            });

        }

        /// <summary>
        /// ����ȥ����1
        /// </summary>
        /// <param name="startIndex">���ƿ�ʼIndex</param>
        /// <param name="endIndex">���ƽ���Index</param>
        /// <param name="onClickNums">�������</param>
        /// <param name="callBack">��ɻص�</param>
        private void MaterialsToCard(int startIndex, int endIndex, int onClickNums, Action callBack = null)
        {
            _cardPageCurIndex = startIndex;
            _cardPageMinIndex = startIndex;
            _cardPageMaxIndex = endIndex;

            _recordOnClickCardNums = onClickNums;

            _materialsParentRect.DOAnchorPosX(_materialsParentRect.anchoredPosition.x - 1920, 1);
            _cardParentRect.DOAnchorPosX(_cardParentRect.anchoredPosition.x - 1920, 1);

            callBack?.Invoke();

        }



        /// <summary>
        /// ���ȥ��һС����
        /// </summary>
        /// <param name="go"></param>
        private void OnClickLast(GameObject go)
        {
            _mask.Show();
            ClickFeedback(go.transform, () => {
                _smallPartCurIndex--;

                switch (_smallPartCurIndex)
                {
                    case 0:     //����1С����ȥ����                  
                        CardToMaterials();
                        break;
                }

                Delay(1.0f, () => { _mask.Hide(); });
                ControllePartInteriorSwitchsShow();

            });

        }

        /// <summary>
        /// ����1ȥ����
        /// </summary>
        private void CardToMaterials()
        {
            RemoveSlideEvent();

            int offset = (_cardPageCurIndex - 0) * 1920; ;
            _materialsParentRect.anchoredPosition = new Vector2(_materialsParentRect.anchoredPosition.x - offset, 0);
            offset = offset + 1920;
            _materialsParentRect.DOAnchorPosX(_materialsParentRect.anchoredPosition.x + offset, 1);
            _cardParentRect.DOAnchorPosX(_cardParentRect.anchoredPosition.x + offset, 1);
            _l2sG.Initialize(true);
            _r2sG.Initialize(true);
            _arrowsParent.gameObject.Hide();
        }

        /// <summary>
        /// ����С�������л�Btn��ʾ
        /// </summary>
        /// <param name="curSmallPartIndex">��ǰС����Index</param>
        /// <param name="smallPartIsFinishs">С�����Ƿ����List</param>
        private void ControllePartInteriorSwitchsShow()
        {
            bool isFirstIndex = _smallPartCurIndex == 0;
            bool isEndIndex = _smallPartCurIndex == _smallPartIsFinishs.Count - 1;

            if (isFirstIndex)
            {
                _last.Hide();
                if (_smallPartIsFinishs[_smallPartCurIndex])
                    _next.Show();
                else
                    _next.Hide();
            }
            else if (isEndIndex)
            {
                _next.Hide();
                if (_smallPartIsFinishs[_smallPartCurIndex])
                    _last.Show();
                else
                    _last.Hide();
            }
            else
            {
                if (_smallPartIsFinishs[_smallPartCurIndex])
                {
                    _last.Show();
                    _next.Show();
                }
                else
                {
                    _last.Hide();
                    _next.Hide();
                }
            }
        }

        /// <summary>
        /// �Ƿ�߽�
        /// </summary>
        /// <param name="curPageIndex">��ǰ����ҳIndex</param>
        private void IsBorder()
        {

            var l = _arrowsParent.Find("L").gameObject;
            var r = _arrowsParent.Find("R").gameObject;
            var l2 = _arrowsParent.Find("L2").gameObject;
            var r2 = _arrowsParent.Find("R2").gameObject;


            if (_cardPageCurIndex == _cardPageMinIndex)
            {
                l.Hide(); r.Show();
                PlaySpine(l2, "L4");
                PlaySpine(r2, r2.name);
            }
            else if (_cardPageCurIndex == _cardPageMaxIndex)
            {
                l.Show(); r.Show();
                PlaySpine(l2, l2.name);
                r.Hide();
                PlaySpine(r2, "R4");
            }
            else
            {
                l.Show(); r.Show();
                PlaySpine(l2, l2.name);
                PlaySpine(r2, r2.name);
            }
        }

        /// <summary>
        /// ����л����Ƽ�ͷ
        /// </summary>
        /// <param name="go"></param>
        private void OnClickArrows(GameObject go)
        {
            SoundManager.instance.PlayClip(9);
            var name = go.name;
            var spineGo = _arrowsParent.Find(name + "2").gameObject;
            PlaySpine(spineGo, name);
            SwitchPage(name == "L");
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="go"></param>
        private void OnClickMaterials(GameObject go)
        {
            _mask.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            SoundManager.instance.ShowVoiceBtn(false);

            var name = go.name;
            var spineGo = _materialsParent.Find("0").gameObject;
            var soundIndex = int.Parse(go.transform.GetChild(0).name);
            PlaySpine(spineGo, name);
            BellSpeck(_sBD, soundIndex, null, () =>
            {
                _mask.Hide();
                if (!_smallPartIsFinishs[_smallPartCurIndex])
                    SoundManager.instance.ShowVoiceBtn(true);
            });
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="go"></param>
        private void OnClickCard(GameObject go)
        {

            _mask.Show();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            var name = go.name;

            bool isContains = _recordOnClickCardNames.Contains(name);
            if (!isContains)
                _recordOnClickCardNames.Add(name);

            var spineGo = go.transform.parent.Find(name + "3").gameObject;

            spineGo.transform.SetAsLastSibling();

            var soundIndex = int.Parse(go.transform.GetChild(0).name);

            var spineName1 = name + "1";    //�Ŵ�
            var spineName2 = name + "2";    //��С

            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, soundIndex);

            PlaySpine(spineGo, spineName1);

            Delay(time, () => { AddEvent(_mask, OnClickMask); });

            void OnClickMask(GameObject g)
            {
                RemoveEvent(g);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
                PlaySpine(spineGo, spineName2, () =>
                {
                    _mask.Hide();
                    if (!_smallPartIsFinishs[_smallPartCurIndex] && _recordOnClickCardNames.Count == _recordOnClickCardNums)
                        SoundManager.instance.ShowVoiceBtn(true);
                });
            }
        }

        /// <summary>
        /// Add�����л�����
        /// </summary>
        private void AddSlideSwitchPage()
        {

            UIEventListener.Get(_silde).onDown = downData => { _prePressPos = downData.pressPosition; };

            UIEventListener.Get(_silde).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
                    if (!isRight)
                        SwitchPage(true);
                    else
                        SwitchPage(false);
                }
            };
        }

        /// <summary>
        /// �Ƴ���������
        /// </summary>
        private void RemoveSlideEvent()
        {
            var uiL = _silde.GetComponent<UIEventListener>();
            if (uiL != null)
                _silde.transform.RemoveComponent<UIEventListener>();
        }



        /// <summary>
        /// �л�����ҳ
        /// </summary>
        /// <param name="isLeftSilde">�Ƿ���</param>
        private void SwitchPage(bool isLeftSilde)
        {

            if (isLeftSilde)
            {
                if (_cardPageCurIndex <= _cardPageMinIndex)
                    return;

                _cardPageCurIndex--;

            }
            else
            {
                if (_cardPageCurIndex >= _cardPageMaxIndex)
                    return;
                _cardPageCurIndex++;

            }

            _mask.Show();

            SoundManager.instance.ShowVoiceBtn(false);

            float value = isLeftSilde ? 1920f : -1920f;
            value = _cardParentRect.anchoredPosition.x + value;

            _cardParentRect.DOAnchorPosX(value, 1.0f).OnComplete(() =>
            {
                IsBorder();
                _mask.Hide();
                if (!_smallPartIsFinishs[_smallPartCurIndex] && _recordOnClickCardNames.Count == _recordOnClickCardNums)
                    SoundManager.instance.ShowVoiceBtn(true);
            });
        }

        #endregion

        #region ���ú���

        void ClickFeedback(Transform tar, Action callBack)
        {
            SoundManager.instance.PlayClip(9);

            Vector2 curScale = tar.localScale;

            tar.DOScale(curScale * 0.9f, 1 / 6f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                tar.DOScale(curScale, 1 / 3f).SetEase(Ease.InOutSine);
            });
            Delay(0.5f, callBack);
        }

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, float len = 0)
        {

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            RemoveEvent(go);
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }

        #endregion

    }
}
