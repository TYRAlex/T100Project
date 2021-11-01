using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course746Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject _mainTargetSpine;
        private GameObject _highLightSpine;
        private GameObject _textLightSpine;
        private Transform _clickPanel;
        private List<string> _allStructList;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            _mainTargetSpine = curTrans.GetGameObject("MainTarget");
            _highLightSpine = curTrans.GetGameObject("HighLight");
            _textLightSpine = curTrans.GetGameObject("TextLight");
            _clickPanel = curTrans.GetTransform("ClickPanel");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            isPlaying = false;
            for (int i = 0; i < _clickPanel.childCount; i++)
            {
                GameObject target = _clickPanel.GetChild(i).gameObject;
                PointerClickListener.Get(target).clickDown = ClickEvent;

            }
            _allStructList=new List<string>();
        }

        private void ClickEvent(GameObject go)
        {
            if(!isPlaying)
                return;
            isPlaying = false;
            string targetName = go.name;
            int targetSpineIndex = 1;
            int voiceIndex = 2;
            if (!_allStructList.Contains(targetName))
                _allStructList.Add(targetName);
            switch (targetName)
            {
                case "Geer":
                    //滑轮
                    targetSpineIndex = 5;
                    voiceIndex = 6;
                    break;
                case "nacelle":
                    //吊篮
                    targetSpineIndex = 4;
                    voiceIndex = 5;
                    break;
                case "BridgeSpan":
                    //桥跨
                    targetSpineIndex = 3;
                    voiceIndex = 4;
                    break;
                case "Shore":
                    //支撑柱
                    targetSpineIndex = 2;
                    voiceIndex = 3;
                    break;
                case "Rope":
                    //绳索
                    targetSpineIndex = 1;
                    voiceIndex = 2;
                    break;
            }

            SpineManager.instance.DoAnimation(_highLightSpine, "fg" + targetSpineIndex, false);
            SpineManager.instance.DoAnimation(_textLightSpine, "zi" + targetSpineIndex, false);
            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, voiceIndex, false);
            Delay(timer, () =>
            {
                
                if (_allStructList.Count >= 5)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
                else
                {
                    isPlaying = true;
                }

                
            });
        }


        void GameStart()
        {
            Max.SetActive(true);
            //isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true);  }));
        
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
                speaker = Max;
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



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                    {
                        Max.Hide();
                        isPlaying = true;
                    }));
                    break;
                case 2:
                    Max.Show();
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 7));
                    break;
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }

        void Delay(float timer, Action callback)
        {
            mono.StartCoroutine(DelayIE(timer, callback));
        }

        IEnumerator DelayIE(float timer, Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }
    }
}
