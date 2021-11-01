using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{   
    public class TD6752Part3
    {
        private int _talkIndex;               //点击语音健Index
        private int _recordOnClickCardNums;   //记录卡牌点击次数
      
        private MonoBehaviour _mono;

        private GameObject _curGo;            //当前环节GameObject
        private GameObject _sBD;              //小布丁
        private GameObject _mask;             //Mask

        private Transform _cardParent;        //卡牌父物体

        private List<string> _recordOnClickCardNames;   //记录点击卡牌名List   

        private bool _isFinish;                //是否完成一遍

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _sBD = curTrans.GetGameObject("sBD");
            _mask = curTrans.GetGameObject("mask");
            _cardParent = curTrans.Find("Spines");

			Input.multiTouchEnabled = false;			

            GameInit();
            GameStart();
        }

        #region 游戏逻辑

        /// <summary>
        /// 游戏初始化
        /// </summary>
        void GameInit()
        {
            _talkIndex = 1;                    
            _recordOnClickCardNums=3;
            _recordOnClickCardNames = new List<string>();
            _isFinish = false;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            SoundManager.instance.StopAudio();
            _mono.StopAllCoroutines();


            //初始化卡牌Spine
            var cardSGs = _cardParent.GetComponentsInChildren<SkeletonGraphic>();
            foreach (var sG in cardSGs)
            {
                sG.Initialize(true);
                PlaySpine(sG.gameObject, sG.gameObject.name);
            }

            //绑定卡牌点击事件
            var cardE4Rs = _cardParent.GetComponentsInChildren<Empty4Raycast>();
            foreach (var cardE4R in cardE4Rs)
                AddEvent(cardE4R.gameObject, OnClickCard);
                 
            //移除Mask监听
            RemoveEvent(_mask);      
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        void GameStart()
        {
		    _mask.Show(); _sBD.Show(); 
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            BellSpeck(_sBD, 0, null, () => { _mask.Hide(); _sBD.Hide(); });

        }

        /// <summary>
        /// 点击语音健
        /// </summary>
        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(9);
            _mask.Show(); _sBD.Show();

            switch (_talkIndex)
            {
                case 1:
                    _isFinish = true;
                    BellSpeck(_sBD, 4, null, () => { _mask.Hide(); _sBD.Hide(); });
                    break;
            }
            _talkIndex++;
        }

    

   

        /// <summary>
        /// 点击卡牌
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

            var spineName1 = name + "1";    //放大
            var spineName2 = name + "2";    //缩小

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
                    if (!_isFinish && _recordOnClickCardNames.Count == _recordOnClickCardNums)
                        SoundManager.instance.ShowVoiceBtn(true);
                });
            }
        }
   
        #endregion



        #region 常用函数

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
