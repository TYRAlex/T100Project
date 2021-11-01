using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using DG.Tweening;
using UnityEngine.UI;
namespace ILFramework.HotClass
{
    public class Course8313Part2
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
         
        private GameObject _bell;
        private GameObject _Bg;
        private GameObject _mask;

        private Transform _uIs;
        private RectTransform _oneRect;
        private RectTransform _twoRect;

        private Vector2 _prePressPos;
        private int _curIndex;
        private int _indexMax;
   
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            Transform curTrans = _curGo.transform;
            _bell = curTrans.Find("bell").gameObject;
            _Bg = curTrans.Find("Bg").gameObject;
            _mask = curTrans.GetGameObject("mask");
            _uIs = curTrans.GetTransform("UIs");
            _oneRect = curTrans.GetRectTransform("UIs/One");
            _twoRect = curTrans.GetRectTransform("UIs/Two");

            GameInit();
        }

        void GameInit()
        {
            _curIndex = 0;
            _indexMax = _uIs.childCount - 1;

            _bell.Hide();
            _mask.Show();
            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _oneRect.anchoredPosition = new Vector2(0, 0);
            _twoRect.anchoredPosition = new Vector2(1920, 0);

            UIEventListener.Get(_Bg).onDown = null;
            UIEventListener.Get(_Bg).onUp = null;

            UIEventListener.Get(_Bg).onDown = OnDown;
            UIEventListener.Get(_Bg).onUp = OnUp;
            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            _mono.StartCoroutine(Delay(1.0f,()=> {
                _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,0,
                    ()=> { _mask.Show(); },
                    ()=> { _mask.Hide(); }));
            }));
        }

       
        private void OnDown(PointerEventData data)
        {
            _prePressPos = data.pressPosition;
        }

        private void OnUp(PointerEventData data)
        {
            float dis = (data.position - _prePressPos).magnitude;
            bool isRight = (_prePressPos.x - data.position.x) > 0 ? true : false;
            if (dis > 100)
            {
                if (isRight)
                {
                    if (_curIndex >= _indexMax)
                        return;
                    _curIndex++;
                    MoveAni(-1920, 1.0f);
                }
                else
                {
                    if (_curIndex == 0)
                        return;
                    _curIndex--;
                    MoveAni(1920, 1.0f);
                }
            }

        }

        void MoveAni(float distance, float duration, Action action = null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

            for (int i = 0; i < _uIs.childCount; i++)
            {
                var childRect = _uIs.GetChild(i).GetRectTransform();
                var endValue = childRect.anchoredPosition.x + distance;
                childRect.DOAnchorPosX(endValue, duration).OnComplete(() => {
                    action?.Invoke();
                });
            }
        }


        IEnumerator Delay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }


        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(_bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (_talkIndex == 1)
            {

            }
            _talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }
    }
}
