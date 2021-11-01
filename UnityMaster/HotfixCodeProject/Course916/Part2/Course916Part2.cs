using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;
namespace ILFramework.HotClass
{
    public class Course916Part2
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _spine2;
        private GameObject _spine21;
        private GameObject _onClicks;
        private GameObject _musicBtn;

        private GameObject _bell;
        private GameObject _mask;


        void Start(object o)
        {
            _curGo = (GameObject)o;
            Transform curTrans = _curGo.transform;
            _mono = _curGo.GetComponent<MonoBehaviour>();

            _bell = curTrans.GetGameObject("bell");

            _spine2 = curTrans.GetGameObject("Spines/2");
            _spine21 = curTrans.GetGameObject("Spines/21");
            _onClicks = curTrans.GetGameObject("OnClicks");
            _musicBtn = curTrans.GetGameObject("OnClicks/musicBtn");
            _mask = curTrans.GetGameObject("mask");

            GameInit();
        }

        void GameInit()
        {

            StopAllCoroutines();
            StopAllAudio();
                   
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _onClicks.Hide();
            AddBtnsEvent(_onClicks.transform, OnClickMusicBtn);

            GameStart();
          
        }

        void GameStart()
        {
            PlayBgm(0);
            PlaySpine(_spine2, "an"); PlaySpine(_spine21, "animation");
            BellSpeck(0, () => { _mask.Show(); }, () => { _mask.Hide(); _onClicks.Show(); });
        }

        void OnClickMusicBtn(GameObject go)
        {
            BtnPlaySound();
            _mask.Show();
            PlayVoice(0);
            PlaySpine(_spine2, "an2");
            PlaySpine(_spine21, "animation2",()=> { _mask.Hide(); });
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

        #region 播放Spine
        private float PlaySpine(GameObject go,string name,Action callBack=null,bool isLoop=false)
        {
            var time=  SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            string log = string.Format("SpineGoName:{0}---SpineAniName:{1}---time:{2}---isLoop:{3}",go.name,name,time,isLoop);
            Debug.Log(log);
            return time;
        }
        #endregion

        #region 播放Audio
        private float PlayBgm(int index,bool isLoop = true,SoundManager.SoundType type= SoundManager.SoundType.BGM)
        {            
            var time =  SoundManager.instance.PlayClip(type, index, isLoop);
            string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}",index,type,time,isLoop);
            Debug.Log(log);
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false ,SoundManager.SoundType type = SoundManager.SoundType.VOICE)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            Debug.Log(log);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false ,SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            Debug.Log(log);
            return time;
        }
        #endregion

        #region 停止Audio
        private void StopAllAudio()
        {
            string log = "StopAllAudio";
            Debug.Log(log);
            SoundManager.instance.StopAudio();                  
        }

        private void StopAudio(SoundManager.SoundType type)
        {
            string log = string.Format("StopAudio Type:{0}",type);
            Debug.Log(log);
            SoundManager.instance.StopAudio(type);
        }

        private void StopAudio(string audioName)
        {
            string log = string.Format("StopAudio Name:{0}", audioName);
            Debug.Log(log);
            SoundManager.instance.Stop(audioName);
        }
        #endregion

        #region 延时
        private void Delay(float delay,Action callBack)
        {
           _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        private void UpDate(bool isStart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isStart,delay, callBack));
        }

        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        IEnumerator IEUpdate(bool isStart,float delay, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
            }           
        }

        #endregion

        #region 停止协程

        private void StopAllCoroutines()
        {
            string log = "StopAllCoroutines";
            Debug.Log(log);
            _mono.StopAllCoroutines();
        }
    
        private void StopCoroutines(string methodName)
        {
            _mono.StopCoroutine(methodName);
        }

        private void StopCoroutines(IEnumerator routine)
        {
            _mono.StopCoroutine(routine);
        }

        private void StopCoroutines(Coroutine routine)
        {
            _mono.StopCoroutine(routine);
        }

        #endregion

        #region Bell讲话
        private void BellSpeck(int index,Action specking=null,Action speckend=null,SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            string log = string.Format("index:{0}---type:{1}", index, type);
            Debug.Log(log);
            _mono.StartCoroutine(SpeckerCoroutine(type,index, specking, speckend));
        }

        private void SetPos(RectTransform rect,Vector2 pos)
        {
            string log = string.Format("rect:{0}---pos:{1}", rect.name, pos);
            Debug.Log(log);
            rect.anchoredPosition = pos;
        }

        #endregion

        #region 添加Btn监听
        private void AddBtnsEvent(Transform parent, PointerClickListener.VoidDelegate callBack)
        {
            string log = string.Format("parentName:{0}---parentChildCount:{1}", parent.name,parent.childCount);
            Debug.Log(log);
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                RemoveEvent(child);
                AddEvent(child, callBack);
            }
        }
      
        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            string log = string.Format("AddEvent GoName:{0}", go.name);
            Debug.Log(log);
            PointerClickListener.Get(go).onClick = g=> {
                Debug.Log("OnClick GoName:"+g.name);
                callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            string log = string.Format("RemoveEvent GoName:{0}",go.name);
            Debug.Log(log);
            PointerClickListener.Get(go).onClick = null;
        }

        #endregion

        #region 修改Rect
        private void SetPos(RectTransform rect,Vector3 v3)
        {
            rect.anchoredPosition = v3;            
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }

        private void SetMove(RectTransform rect,Vector2 v2,float duration,Action callBack=null)
        {
            rect.DOAnchorPos(v2, duration).OnComplete(()=> { callBack?.Invoke();});
        }

        #endregion
    }
}
