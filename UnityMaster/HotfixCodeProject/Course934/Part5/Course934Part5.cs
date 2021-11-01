using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Video;
using System.Diagnostics;

namespace ILFramework.HotClass
{
    public class Course934Part5
    {
        private int _talkIndex;
        private MonoBehaviour _mono;
        private GameObject _curGo;
      
        private GameObject _mask;

        private bool _isPlaying;

        private Transform _bg;
        private Transform _btns;
        private GameObject _spineGo;


        private Transform _videos;


        private VideoPlayer _videoPlayer;
        private RawImage _rtImg;


        private bool _isFinish;


        void Start(object o)
        {


            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

          
            _bg = curTrans.Find("Bg");
            _spineGo = curTrans.GetGameObject("Spines/animation");
            _btns = curTrans.Find("Btns");
            _mask = curTrans.GetGameObject("mask");

            _videos = curTrans.Find("Videos");





            _videoPlayer = curTrans.Find("VideoPlayer").GetComponent<VideoPlayer>();
            _videoPlayer.targetTexture.Release();
            _rtImg = curTrans.GetRawImage("Videos/RTImg");

            GameInit();
            GameStart();
        }



        private void InitData()
        {

            _isPlaying = false;
            _talkIndex = 1;




            _rtImg.texture = null;
        }



        private void GameInit()
        {



            RemoveEvent(_mask);

            InitData();

            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            StopAllAudio(); StopAllCoroutines();

            _spineGo.GetComponent<SkeletonGraphic>().Initialize(true);

            HideAllChilds(_bg); HideAllChilds(_videos);


            PlaySpine(_spineGo, _spineGo.name);

            _mask.Hide(); 

            ShowChilds(_bg, 0);
            AddEvents(_btns, OnClicks);

        }



        void GameStart()
        {
                   
        }




        void TalkClick()
        {
            PlayOnClickSound();
            HideVoiceBtn();
            switch (_talkIndex)
            {
                case 1:

                    break;
                case 2:

                    break;
            }

            _talkIndex++;
        }


        #region 游戏逻辑

        private void OnClicks(GameObject go)
        {
            if (_isPlaying)
                return;
            _isPlaying = true;

           
            _videos.SetAsFirstSibling();

            _mask.Show();

            PlayCommonSound(1);

            var name = go.name;
            var spineName = string.Empty;

            string url = string.Empty;

            switch (name)
            {
                case "a":
                    url = "1.mp4";
                    spineName = "animation2";
                    break;             
            }

            _videoPlayer.targetTexture.DiscardContents();
            _videoPlayer.url = GetVideoPath(url);



            _mono.StartCoroutine(PlayMp4());


            Delay(PlaySpine(_spineGo, spineName), () => {

                _mono.StartCoroutine(IsFinish(OnClickMask));
            });


            void OnClickMask()
            {
                AddEvent(_mask, g => {
                    PlayCommonSound(2);
                    RemoveEvent(g);
                    _videos.SetAsFirstSibling();
                    _videoPlayer.targetTexture.Release();
                    HideAllChilds(_videos);
                
                    Delay(PlaySpine(_spineGo, "animation3"), () => {
                        _mask.Hide(); _isPlaying = false;
                    });
                });
            }
        }

        IEnumerator IsFinish(Action callBack)
        {
            while (!_isFinish)
            {
                yield return null;

            }

            var mp4Time = (float)_videoPlayer.length;  //获取视频的时长一定要等videoPlayer.isPrepared完成了，才能获取到视频时长
                                                       //    UnityEngine.Debug.LogError(":" + mp4Time);
            _videoPlayer.Play();
            _videos.SetSiblingIndex(4);
            ShowChilds(_videos, 0);
            Delay(mp4Time, callBack);

        }

        IEnumerator PlayMp4()
        {

            _isFinish = false;

            _videoPlayer.Prepare();

            while (!_videoPlayer.isPrepared)
            { yield return null; }              //监听是否准备完毕。没有完成一直等待，完成后跳出循环，进行img赋值，让后播放                                  

            _rtImg.texture = _videoPlayer.targetTexture;
            _videoPlayer.Play();
            _videoPlayer.Pause();
            _isFinish = true;

            StopCoroutines("PlayMp4");
        }




        #endregion


        #region 动态加载资源



        private string GetVideoPath(string videoPath)
        {
            var path = LogicManager.instance.GetVideoPath(videoPath);
            return path;
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

        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }
        private void ShowChilds(Transform parent, string name, Action<GameObject> callBack = null)
        {
            var go = parent.Find(name).gameObject;
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

        private void PlaySequenceSpine(GameObject go, List<string> spineNames)
        {
            _mono.StartCoroutine(IEPlaySequenceSpine(go, spineNames));
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

        IEnumerator IEPlaySequenceSpine(GameObject go, List<string> spineNames)
        {
            for (int i = 0; i < spineNames.Count; i++)
            {
                var name = spineNames[i];
                var delay = PlaySpine(go, name);
                yield return new WaitForSeconds(delay);
            }
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
                daiJi = "daiji"; speak = "daijishuohua";
            }
            else
            {
                //  Debug.LogError("Role Spine Name...");
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
