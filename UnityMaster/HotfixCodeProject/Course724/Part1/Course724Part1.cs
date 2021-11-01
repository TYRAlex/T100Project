using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ILFramework.HotClass
{
    public class Course724Part1
    {
        private int _talkIndex;
        private MonoBehaviour _mono;
        private GameObject _curGo;     
        private GameObject _bell;
        private GameObject _mask;

        private bool _isPlaying ;

        private GameObject _step1;

        private GameObject _carGo;
        private GameObject _fontGo;

        private Image _carImg;
        private Image _fontImg;

        private Transform _spines;
        private Transform _spine1Btns;
        private GameObject _jingSpine;
        private GameObject _fangguangSpine;


        private GameObject _spine2Jing;
        private Transform _spine2BtnSpine;
        private Transform _spine2Btns;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
             var curTrans = _curGo.transform;

            _bell = curTrans.GetGameObject("bell");
            _step1 = curTrans.GetGameObject("Bg/step1");

            _mask = curTrans.GetGameObject("mask");
            _carGo = curTrans.GetGameObject("Bg/step1/car");
            _fontGo = curTrans.GetGameObject("Bg/step1/font");
            _carImg = curTrans.GetImage("Bg/step1/car");
            _fontImg = curTrans.GetImage("Bg/step1/font");

            _spines = curTrans.Find("Spines");
            _spine1Btns = curTrans.Find("Spines/Spine1/btns");

            _jingSpine = curTrans.GetGameObject("Spines/Spine1/jing");
            _fangguangSpine = curTrans.GetGameObject("Spines/Spine1/fangguang");

            _spine2Jing = curTrans.GetGameObject("Spines/Spine2/jing");
            _spine2BtnSpine = curTrans.Find("Spines/Spine2/btnSpine");
            _spine2Btns = curTrans.Find("Spines/Spine2/btns");

            GameInit();
            GameStart();
        }



        private void InitData()
        {
            _isPlaying = false;
            _talkIndex = 1;
            _carImg.color = new Color(1, 1, 1, 1);
            _fontImg.color = new Color(1, 1, 1, 0);
        }



        private void GameInit()
        {
            InitData();
          
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            StopAllAudio(); StopAllCoroutines();


            _carGo.Show();  _fontGo.Hide(); _mask.Hide();
            _step1.Show(); HideAllChilds(_spines);
            _bell.Show();
          
            AddEvents(_spine1Btns, OnClick1);
            AddEvents(_spine2Btns, OnClick2);
        }

     

        void GameStart()
        {
            PlayBgm(0);
            BellSpeck(_bell,0,null, ShowVoiceBtn);        
        }


        void TalkClick()
        {
            PlayOnClickSound();
            HideVoiceBtn();
            switch (_talkIndex)
            {
                case 1:
                    _carGo.Hide(); _fontGo.Show();
                    _fontImg.DOColor(new Color(1, 1, 1, 1), 1f);
                    BellSpeck(_bell,1,null,ShowVoiceBtn);
                    break;
                case 2:
                    _step1.Hide();_bell.Hide();

                    ShowChilds(_spines,0,go=> {PlaySpine(_jingSpine, _jingSpine.name + "1"); PlaySpine(_fangguangSpine, "0"); });
                    BellSpeck(_bell, 2, ()=> { _mask.Show(); }, ()=> { ShowVoiceBtn(); _mask.Hide(); } );
                    break;
                case 3:
                    HideChilds(_spines, 0);ShowChilds(_spines, 1,g=> { PlaySpine(_spine2Jing,_spine2Jing.name+"2"); InitSpines(_spine2BtnSpine,false);  });
                    BellSpeck(_bell,11,()=> { _mask.Show(); },()=> { ShowVoiceBtn(); _mask.Hide(); });
                    break;
                case 4:
                    HideChilds(_spines, 1); _fontGo.Hide(); _bell.Show();
                    _step1.Show();_carGo.Show();
                    BellSpeck(_bell, 15);
                    break;
            }         
            _talkIndex++;
        }


        #region 游戏逻辑

        private void OnClick1(GameObject go)
        {
            if (_isPlaying)
                return;
            _isPlaying = true;

            HideVoiceBtn();
          //  PlayOnClickSound();
            PlayVoice(1);
            var name = go.name;
            var soundIndex = int.Parse(go.transform.GetChild(0).name);
            PlaySpine(_fangguangSpine, name);
            var time=  PlaySound(soundIndex);
            Delay(time, () => { _isPlaying = false; ShowVoiceBtn(); });
         
        }

        private void OnClick2(GameObject go)
        {
            if (_isPlaying)
                return;
            _isPlaying = true;

            HideVoiceBtn();
            PlayOnClickSound();
            var name = go.name;

            var spineGo = FindSpineGo(_spine2BtnSpine, name);          
            var soundIndex = int.Parse(go.transform.GetChild(0).name);

            PlaySpine(spineGo, spineGo.name + "2");

            PlayVoice(0);
            switch (name)
            {
                case "a":   //减速
                    PlaySequenceSpine(_spine2Jing, new List<string> { "jiansu1", "jiansu2", "jiansu2", "jiansu2", "jiansu3" }, () => { StopAudio(SoundManager.SoundType.VOICE); _isPlaying = false;  });
                    Delay(0.6f, () => {Delay(PlaySound(soundIndex), () => { ShowVoiceBtn(); }); });
                    break;
                case "b":   //加速
                    PlaySequenceSpine(_spine2Jing, new List<string> { "jiasu1", "jiasu2", "jiasu2", "jiasu2","jiasu3" }, () => { StopAudio(SoundManager.SoundType.VOICE); _isPlaying = false; });
                    Delay(0.6f, () => {Delay(PlaySound(soundIndex), () => {  ShowVoiceBtn(); }); });
                    break;
                case "c":   //空挡
                    PlaySequenceSpine(_spine2Jing, new List<string> { "kongdang", "kongdang2", "kongdang3" }, () => { StopAudio(SoundManager.SoundType.VOICE);  });
              
                   
                    Delay(PlaySound(soundIndex), () => { _isPlaying = false; ShowVoiceBtn(); });
                    break;            
            }       
        }

        #endregion


        #region 常用函数

        #region 语音键显示隐藏
        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }
        #endregion

        #region 隐藏和显示

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }


        #endregion

        #region Spine相关

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

        private GameObject FindSpineGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        private void PlaySequenceSpine(GameObject go,List<string> spineNames,Action callBack=null)
        {
            _mono.StartCoroutine(IEPlaySequenceSpine(go, spineNames, callBack));
        }

        #endregion

        #region 音频相关

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

        #region 延时相关

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        private void UpDate(bool isStart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
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

        IEnumerator IEPlaySequenceSpine(GameObject go,List<string> spineNames,Action callBack=null)
        {
            for (int i = 0; i < spineNames.Count; i++)
            {
                var name = spineNames[i];
                var delay= PlaySpine(go, name);
                yield return new WaitForSeconds(delay);
            }
            callBack?.Invoke();
        }

        #endregion

        #region 停止协程

        private void StopAllCoroutines()
        {
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, bool isBell = true, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, isBell));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, bool isBell = true, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;
           
            if (isBell)
            {
                daiJi = "DAIJI"; speak = "DAIJIshuohua";
            }
            else
            {
                Debug.LogError("Role Spine Name...");
                daiJi = "daiji"; speak = "speak";
            }

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

        #region 监听相关

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

        #region 修改Rect相关

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
    
        #endregion
    }
}
