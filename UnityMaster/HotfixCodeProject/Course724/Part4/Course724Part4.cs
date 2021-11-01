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
    public class Course724Part4
    {
        private int _talkIndex;
        private MonoBehaviour _mono;
        private GameObject _curGo;
        private GameObject _bell;
        private GameObject _mask;

        private bool _isPlaying;

        private Transform _bg;
        private Transform _btns;
        private GameObject _spineGo;
        private GameObject _di;

        private Transform _videos;


        private RawImage _bg2Img;
        private Image _aSDImg;

        private VideoPlayer _videoPlayer;
        private RawImage _rtImg;


        private bool _isFinish;


        void Start(object o)
        {


            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _bell = curTrans.GetGameObject("bell");
            _bg = curTrans.Find("Bg");
            _spineGo = curTrans.GetGameObject("Spines/animation");
            _btns = curTrans.Find("Btns");
            _mask = curTrans.GetGameObject("mask");
            _di = curTrans.GetGameObject("di");
            _videos = curTrans.Find("Videos");

            _bg2Img = curTrans.GetRawImage("Bg/bg2");
            _aSDImg = curTrans.GetImage("Bg/ASD");



            _videoPlayer = curTrans.Find("VideoPlayer").GetComponent<VideoPlayer>();
           
            _rtImg = curTrans.GetRawImage("Videos/RTImg");

            GameInit();
            GameStart();
        }



        private void InitData()
        {

            _isPlaying = false;
            _talkIndex = 1;
            _bg2Img.color = new Color(1, 1, 1, 0);
            _aSDImg.color = new Color(1, 1, 1, 0);



            _rtImg.texture = null;
        }



        private void GameInit()
        {



            RemoveEvent(_mask);

            InitData();

            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            StopAllAudio(); StopAllCoroutines();



            HideAllChilds(_bg); HideAllChilds(_videos);

            _spineGo.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            PlaySpine(_spineGo, "kong", () => { PlaySpine(_spineGo, _spineGo.name); });
            _mask.Show(); _bell.Show(); _di.Show();
            ShowChilds(_bg, 0);
            AddEvents(_btns, OnClicks);

        }



        void GameStart()
        {
            PlayBgm(0);
            BellSpeck(_bell, 0, null, ShowVoiceBtn);
        }




        void TalkClick()
        {
            PlayOnClickSound();
            HideVoiceBtn();
            switch (_talkIndex)
            {
                case 1:
                    ShowChilds(_bg, 1); ShowChilds(_bg, 2);
                    _bg2Img.DOColor(new Color(1, 1, 1, 1), 1);
                    _aSDImg.DOColor(new Color(1, 1, 1, 1), 1);
                    BellSpeck(_bell, 1, null, ShowVoiceBtn);
                    break;
                case 2:
                    ShowChilds(_bg, 3); _bell.Hide(); _di.Hide();
                    _spineGo.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    BellSpeck(_bell, 2, null, () => { _mask.Hide(); });
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



            _mask.Show();

            PlayCommonSound(1);

            var name = go.name;

            var soundIndex = int.Parse(go.transform.GetChild(0).name);

            string url = string.Empty;
            int voiceIndex = -1;


            switch (name)
            {
                case "a":
                    url = "1.mp4";
                    voiceIndex = 0;
                    break;
                case "b":
                    url = "2.mp4";
                    voiceIndex = 1;
                    break;
                case "c":
                    url = "3.mp4";
                    voiceIndex = 2;
                    break;
            }
            _videoPlayer.targetTexture.Release();
            _videoPlayer.targetTexture.DiscardContents();
            _videoPlayer.url = GetVideoPath(url);
            _mono.StartCoroutine(PlayMp4());


            Delay(PlaySpine(_spineGo, name), () => {

                _mono.StartCoroutine(IsFinish(soundIndex, voiceIndex, OnClickMask));
            });


            void OnClickMask()
            {
                AddEvent(_mask, g => {
                    PlayCommonSound(2);
                    RemoveEvent(g);
                    HideAllChilds(_videos);
                    //_videoPlayer.Stop();
                   
                    Delay(PlaySpine(_spineGo, name + "2"), () => {                       
                        _mask.Hide(); _isPlaying = false;
                    });
                });
            }
        }

        IEnumerator IsFinish(int soundIndex, int voiceIndex, Action callBack)
        {
            while (!_isFinish)
            {
                yield return null;

            }

           
            _videoPlayer.Play();
            ShowChilds(_videos, 0);
            Delay(PlaySound(soundIndex), callBack);
            PlayVoice(voiceIndex);
        }

        IEnumerator PlayMp4()
        {

            _isFinish = false;

            _videoPlayer.Prepare();

            while (!_videoPlayer.isPrepared)
            { yield return null; }              //监听是否准备完毕。没有完成一直等待，完成后跳出循环，进行img赋值，让后播放                                  

            _rtImg.texture = _videoPlayer.targetTexture;

            _isFinish = true;

            StopCoroutines("PlayMp4");
        }




        #endregion


        #region 动态加载资源


        //private void ResourcesVideoPrefab(string url)
        //{         
        //  var go = ResourceManager.instance.LoadResourceAB<GameObject>(Util.GetHotfixPackage("Course724Part2"), "AVPro Video");         
        //  var video = GameObject.Instantiate(go, _videos);
        //  var displayUGUI = video.GetComponent<DisplayUGUI>();
        //  _mediaPlayer = displayUGUI._mediaPlayer;

        //  _mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, url, false);

        //}

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
                daiJi = "DAIJI"; speak = "DAIJIshuohua";
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
