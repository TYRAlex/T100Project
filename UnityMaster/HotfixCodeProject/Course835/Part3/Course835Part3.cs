using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course835Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;

        private Transform _redCard;
        private Transform _greenCard;

        private Vector3 _redcardOriginalPos;
        private Vector3 _greenCardOriginalPos;

        private GameObject _target;

        private Transform _handTrans;

        private Transform _putArea;
        //private Transform _mask;

        public Transform HandTrans
        {
            get
            {
                if (_handTrans == null)
                {
                    _handTrans = curGo.transform.GetTransform("Cards/Hand");
                }

                if (_handTrans == null)
                {
                    _handTrans = curGo.transform.GetTransform("Cards/RedCard/Hand");
                }
                if (_handTrans == null)
                {
                    _handTrans = curGo.transform.GetTransform("Cards/GreenCard/Hand");
                }

                return _handTrans;
            }
        }

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            bell = curTrans.Find("bell").gameObject;
            //_handTrans = curTrans.Find("Cards/Hand");
            _putArea = curTrans.GetTransform("Cards/PutArea");
            talkIndex = 1;
            _target = curTrans.GetGameObject("Target");
            //_mask = curTrans.GetTransform("mask");
            InitProperty(curTrans);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        void ResetAudioAndPlayBGM()
        {
            SoundManager.instance.StopAudio();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }

        void InitProperty(Transform curTrans)
        {
            mono.StopAllCoroutines();
            ResetAudioAndPlayBGM();
            SpineManager.instance.DoAnimation(_target, "c0", false);
            _redCard = curTrans.GetTransform("Cards/RedCard");
            _greenCard = curTrans.GetTransform("Cards/GreenCard");
            _redCard.gameObject.Show();
            _greenCard.gameObject.Show();
            _redcardOriginalPos = curTrans.GetTransform("Cards/RedCardOriginPos").localPosition;
            _greenCardOriginalPos = curTrans.GetTransform("Cards/GreenCardOriginPos").localPosition;
            _redCard.localPosition = _redcardOriginalPos;
            _greenCard.localPosition = _greenCardOriginalPos;
           
            //_handTrans.gameObject.Hide();
            // _redCard.SetDragCallback(null, null, DragRedCardEnd);
            // _greenCard.SetDragCallback(null, null, DragGreenCardEnd);
            // _mask.gameObject.Show();
            // _mask.SetAsLastSibling();
        }

        void MoveCardToTheRightArea()
        {
            mono.StartCoroutine(MoveCardTwiceIE());

        }

        IEnumerator MoveCardTwiceIE()
        {
            yield return MoveCardToTheRightAreaIE(_greenCard, _redCard);
            yield return new WaitForSeconds(2f);
            yield return MoveCardToTheRightAreaIE(_redCard, _greenCard, true);
        }

        IEnumerator MoveCardToTheRightAreaIE(Transform card1,Transform card2,bool isDoAni=false)
        {
            //yield return new WaitForSeconds(_currentAudioTime * 25f / 45f);
            Debug.Log("IsDoAni"+isDoAni);
            card1.gameObject.Show();
            card2.gameObject.Show();
            _redCard.localPosition = _redcardOriginalPos;
            _greenCard.localPosition = _greenCardOriginalPos;
            yield return new WaitForSeconds(0.2f);
            if (card1.name == "GreenCard")
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            else if (card1.name == "RedCard")
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            HandTrans.gameObject.Show();
            HandTrans.SetParent(card1);
            HandTrans.localPosition=Vector3.zero;
            while (Vector3.Distance(_putArea.localPosition,card1.localPosition)>5)
            {
                    //card1.DOLocalMove(_putArea.localPosition, 2f);
                    card1.localPosition = Vector3.Lerp(card1.localPosition, _putArea.localPosition, 2f*Time.fixedDeltaTime);
                    yield return new WaitForFixedUpdate();
            }
            HandTrans.SetParent(card2);
            HandTrans.localPosition=Vector3.zero;
            //card1.gameObject.Hide();
            if (card1.name == "RedCard")
            {
                card1.localPosition = _redcardOriginalPos;
            }
            else if (card1.name == "GreenCard")
            {
                card1.localPosition = _greenCardOriginalPos;
            }

            yield return new WaitForSeconds(1f);
            while (Vector3.Distance(_putArea.localPosition, card2.localPosition)>5f)
            {
                //card2.DOLocalMove(_putArea.localPosition, 2f);
                card2.localPosition =
                    Vector3.Lerp(card2.localPosition, _putArea.localPosition, 2f * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
            //card2.gameObject.Hide();
            if (card2.name == "RedCard")
            {
                card2.localPosition = _redcardOriginalPos;
            }
            else if (card2.name == "GreenCard")
            {
                card2.localPosition = _greenCardOriginalPos;
            }
            HandTrans.SetParent(card2.parent);
            HandTrans.gameObject.Hide();
            if (isDoAni)
            {
                yield return new WaitForSeconds(1.5f);
                SpineManager.instance.DoAnimation(_target, "c1", false,
                    () => SpineManager.instance.DoAnimation(_target, "c3", false));
            }
        
            _redCard.localPosition = _redcardOriginalPos;
            _greenCard.localPosition = _greenCardOriginalPos;
        }

        // void DragRedCardEnd(Vector3 position, int type, int index, bool isMatch)
        // {
        //     if (isMatch)
        //     {
        //         if (!_greenCard.gameObject.activeSelf)
        //         {
        //             //_redCard.gameObject.Show();
        //             _greenCard.gameObject.Show();
        //             _greenCard.DoReset();
        //             _redCard.DoReset();
        //         }
        //         else
        //         {
        //             _redCard.gameObject.Hide();
        //         }
        //     }
        //     else
        //     {
        //         _redCard.DoReset();
        //     }
        // }

        // void DragGreenCardEnd(Vector3 position, int type, int index, bool isMatch)
        // {
        //     if (isMatch)
        //     {
        //         _greenCard.gameObject.Hide();
        //         if (!_redCard.gameObject.activeSelf)
        //         {
        //
        //             SpineManager.instance.DoAnimation(_target, "c1", false,
        //                 () => SpineManager.instance.DoAnimation(_target, "c3", false, () =>
        //                 {
        //                     _greenCard.gameObject.Show();
        //                     _greenCard.DoReset();
        //                     _redCard.gameObject.Show();
        //                     _redCard.DoReset();
        //                 }));
        //
        //         }
        //
        //     }
        //     else
        //     {
        //         _greenCard.DoReset();
        //     }
        // }

        void GameStart()
        {
            mono.StopAllCoroutines();
            mono.StartCoroutine(
                SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () => MoveCardToTheRightArea()));

        }

        private float _currentAudioTime = 0f;
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            _currentAudioTime = ind;
            SpineManager.instance.DoAnimation(bell, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "daiji");

            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex == 1)
            {

            }
            talkIndex++;
        }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}
