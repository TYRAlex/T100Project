using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course916Part3
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _di;
        private GameObject _bell;
        private GameObject _mask;

        private GameObject _bg1;
        private GameObject _spines;
        private GameObject _spine3;
        private GameObject _spine4;

        private GameObject _spinebeizi;
        private GameObject _spineQiaoJi;
        private Transform _onClicks;

        private List<string> _cheackFinish;

        private Transform _spine4Mask;
        private Image _spineMask1;
        private Image _spineMask2;
        private Image _spineMask3;

        private Transform _music1;
        private Transform _music2;
        private Transform _music3;



        private List<Vector3> _v3Ups;
        private List<Vector3> _v3Downs;
        void Start(object o)
        {
            _curGo = (GameObject)o;
            Transform curTrans = _curGo.transform;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            _bg1 = curTrans.GetGameObject("Bg/RawImage1");
            _di = curTrans.GetGameObject("di");
            _bell = curTrans.GetGameObject("bell");
            _mask = curTrans.GetGameObject("mask");
            _spines = curTrans.GetGameObject("Spines");
            _spine3 = curTrans.GetGameObject("Spines/3");
            _spine4 = curTrans.GetGameObject("Spines/4");
            _spineQiaoJi= curTrans.GetGameObject("Spines/qiaoJi");
            _spinebeizi = curTrans.GetGameObject("Spines/beizi");
            _onClicks = curTrans.GetTransform("OnClicks");

            _spine4Mask = curTrans.GetTransform("Spine4Mask");
            _spineMask1 = curTrans.GetImage("Spine4Mask/1/mask");
            _spineMask2 = curTrans.GetImage("Spine4Mask/2/mask");
            _spineMask3 = curTrans.GetImage("Spine4Mask/3/mask");
            _music1 = curTrans.GetTransform("Spine4Mask/1/music");
            _music2 = curTrans.GetTransform("Spine4Mask/2/music");
            _music3 = curTrans.GetTransform("Spine4Mask/3/music");

            bool isNullBtn = curTrans.Find("Button")==null;
            if (isNullBtn)
            {
                GameObject go = new GameObject("Button");
                go.transform.SetParent(curTrans);
                go.AddComponent<Empty4Raycast>();
                PointerClickListener.Get(go).onClick = g=>{
                    GameInit();
                };
            }

            GameInit();          
        }

        private void QiaoJiMove(float endValue,float delay,Action callBack=null)
        {
            _spineQiaoJi.transform.GetRectTransform().DOAnchorPosX(endValue, delay).OnComplete(()=> { callBack?.Invoke(); });
        }

        private void SetScale(Transform transform,Vector3 vector3)
        {
            transform.localScale = vector3;
        }

        void InitPos()
        {
            _v3Ups = new List<Vector3>();
            var oX = 100;
            var oY = 100;
            var r = 100;

            for (int i =225; i >= 45; i--)
            {
                int x = (int)(oX + r * Math.Cos(i * Math.PI / 180));
                int y = (int)(oY + r * Math.Sin(i * Math.PI / 180));
                var pos = new Vector3(x, y,0);             
                _v3Ups.Add(pos);
            }
            _v3Downs = new List<Vector3>();
            oX = 300;
            oY = 300;
            for (int i = 225; i <= 405; i++)
            {
                int x = (int)(oX + r * Math.Cos(i * Math.PI / 180));
                int y = (int)(oY + r * Math.Sin(i * Math.PI / 180));           
                var pos = new Vector3(x, y,0);              
                _v3Downs.Add(pos);
            }
        }

        void GameInit()
        {
            _spinebeizi.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);

            InitPos();

            PlaySpineAni(_spinebeizi, "bc2");
            PlaySpineAni(_spine4, "kong");
            PlaySpineAni(_spineQiaoJi, "kong");
            QiaoJiMove(300, 0);
         
            _cheackFinish = new List<string>();
            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
                 
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _bell.Hide(); _mask.Hide(); _onClicks.gameObject.Hide(); _bg1.Hide();_di.Hide();
            _spine3.Show(); _spines.Show(); _spine4Mask.gameObject.Hide();

            SpineManager.instance.DoAnimation(_spine3, "animation", false);
            AddOnClickEvent();

            InitStateSpine4Mask();
       
            GameStart();
        }


        void InitStateSpine4Mask()
        {
            for (int i = 0; i < _spine4Mask.childCount; i++)
            {
                var maskImg = _spine4Mask.GetChild(i).Find("mask").GetImage();
                maskImg.fillAmount = 1;
                var musicTra = _spine4Mask.GetChild(i).Find("music").transform;
                for (int j = 0; j < musicTra.childCount; j++)
                {
                    var child = musicTra.GetChild(j);
                    child.localPosition = new Vector2(0, 0);
                    var childImg =child.GetImage();

                    childImg.color = new Color(1, 1, 1, 0);
                }
            }
        }

        void GameStart()
        {
            PlayBgm(0);
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,0,()=> { _mask.Show(); },()=>{ _mask.Hide(); _onClicks.gameObject.Show(); }));
        }


        void AddOnClickEvent()
        {
            for (int i = 0; i < _onClicks.childCount; i++)
            {
                var child = _onClicks.GetChild(i).gameObject;
                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = OnClick;
            }
        }

        private void OnClick(GameObject go)
        {
            BtnPlaySound();
            var name = go.name;
            _mask.Show();
            SoundManager.instance.ShowVoiceBtn(false);

            if (!_cheackFinish.Contains(name))           
                _cheackFinish.Add(name);
            
            switch (name)
            {
                case "1":
                    Delay(0.45f,()=> { PlayVoice(2); });  
                    First(() => { IsFinish();  });
                    break;
                case "2":
                    PlayVoice(0);
                    Secend(() => { IsFinish(); });
                    break;
                case "3":
                    PlayVoice(1);
                    Thirdly(() => { IsFinish(); });
                    break;
            }
        }

        void IsFinish()
        {
            SoundManager.instance.ShowVoiceBtn(_cheackFinish.Count == _onClicks.childCount);
        }

        void First(Action action)
        {
           
            PlaySpineAni(_spine3, "animation2", One);
            void One() { PlaySpineAni(_spine4,"aj", Two); };
            void Two() { PlaySpineAni(_spine4, "a1", Three); };
            void Three() { BellSpeck(1, Four); }; 
            void Four() { Delay(2.2f, () => { PlaySpineAni(_spine4, "a2",()=> { Delay(1.0f,()=> { PlaySpineAni(_spine4, "ac2", () => { Five(); }); }); }); });}; 
            void Five() { BellSpeck(2, () => { _spinebeizi.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1); PlaySpineAni(_spinebeizi, "bc", Six); }); };
            void Six() { PlaySpineAni(_spinebeizi, "b1", Seven); };
            void Seven() { PlaySpineAni(_spinebeizi, "b2", Eight); };
            void Eight() { BellSpeck(3, null, Nine); };
            void Nine() { PointerClickListener.Get(_mask).onClick = g => { PointerClickListener.Get(_mask).onClick = null; Ten(); }; };
            void Ten() { PlaySpineAni(_spine3, "animation5"); PlaySpineAni(_spinebeizi, "bc2", () => {  _mask.Hide(); action?.Invoke(); }); };
        }

        void Secend(Action action)
        {
            PlaySpineAni(_spine3, "animation3", One);
            void One() {  PlaySpineAni(_spine4, "cj", Two); };
            void Two() { BellSpeck(4, Three, Four); };
            void Three() { Delay(1.0f, () => { PlaySpineAni(_spine4, "c1"); }); };
            void Four() { PlaySpineAni(_spineQiaoJi, "dj2"); PlaySpineAni(_spine4, "dj", Five); };
            void Five() { Six(); Delay(1.0f,()=> { BellSpeck(5,null, Eight); }); };
            void Six() { PlaySpineAni(_spineQiaoJi,"d3", Seven); };
            void Seven() { QiaoJiMove(25, 0.5f,()=> { PlaySpineAni(_spine4, "d1"); }); };
            void Eight() { QiaoJiMove(300, 0,()=> { PlaySpineAni(_spineQiaoJi,"dj2", Nine); }); };
            void Nine() { PlaySpineAni(_spineQiaoJi,"d4", Ten); Delay(1.2f, () => { BellSpeck(6,null, Eleven); }); };
            void Ten() { QiaoJiMove(25, 0.5f, () => { PlaySpineAni(_spine4, "d2"); }); };
            void Eleven() { PointerClickListener.Get(_mask).onClick = g => { PointerClickListener.Get(_mask).onClick = null; Twelve(); }; };
            void Twelve() { PlaySpineAni(_spine3, "animation6"); PlaySpineAni(_spine4, "dc2"); PlaySpineAni(_spineQiaoJi, "dc4",()=> { QiaoJiMove(300,0); _mask.Hide(); action?.Invoke(); }); };
        
        }




        void Thirdly(Action action)
        {
            InitStateSpine4Mask();
            PlaySpineAni(_spine3, "animation4", One);
            void One() { _spine4Mask.gameObject.Show(); Delay(0.5f, () => { MusicTw(_music1,0.7f,()=> { MusicTw(_music2,0.5f,()=> { MusicTw(_music3,0.5f, Two); });  }); }); };   
            void Two() { BellSpeck(7);Delay(4.0f,()=> { PlaySpineAni(_spine4,"f1", Three); }); };
            void Three() { BellSpeck(8); ImgShow(_spineMask1,()=> { ImgShow(_spineMask2,()=> { ImgShow(_spineMask3, Four); }); }); };
            void Four() { PointerClickListener.Get(_mask).onClick = g => { PointerClickListener.Get(_mask).onClick = null; Delay(0.5f, () => { Five(); }); }; }
            void Five() { _spine4Mask.gameObject.Hide(); PlaySpineAni(_spine3, "animation7",()=> { _mask.Hide(); action?.Invoke(); }); PlaySpineAni(_spine4,"fc2",()=> { }); };
        }





        void PlaySpineAni(GameObject go,string name,Action callBack=null)
        {
           var time= SpineManager.instance.DoAnimation(go, name, false, () => { callBack?.Invoke(); });
           Debug.LogError("动画名："+name+"---时间："+time);
        }
       
        void BellSpeck(int soundIndex,Action callBack1=null,Action callBack2=null)
        {
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, soundIndex, callBack1, callBack2));                
        }

        void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        void MusicTw(Transform music, float delay, Action callBack, float dur = 0.4f)
        {           
            _mono.StartCoroutine(IEMusicTw(music,delay,callBack,dur));
        }


        IEnumerator IEMusicTw(Transform music,float delay,Action callBack, float dur = 0.4f)
        {
            int temp = 0;
         
            while (temp <= music.childCount-1)
            {
                yield return new WaitForSeconds(delay);
                var child = music.GetChild(temp).GetRectTransform() ;
                TwMove(child,_v3Ups.ToArray(),_v3Downs.ToArray(), dur);
                 temp++;
            }

            callBack?.Invoke();
        }



        private void TwMove(RectTransform rect, Vector3[] pos1, Vector3[] pos2,float dur=0.4f)
        {
            Image img = rect.GetImage();
            var imgTw1 = img.DOFade(1, 0.1f);
            var imgTw2 = img.DOFade(0, 0.1f);
            var move1 = rect.DOLocalPath(pos1, dur, PathType.CatmullRom);
            var move2 = rect.DOLocalPath(pos2, dur, PathType.CatmullRom);
            DOTween.Sequence().Append(imgTw1).Append(move1).Append(move2).Append(imgTw2);
        }


        private void ImgShow(Image image,Action action)
        {
            Delay(0.5f,()=> {
            image.DOFillAmount(0, 0.5f).OnComplete(()=> { action?.Invoke(); });
            });
        }

        IEnumerator IEDelay(float delay,Action callBack)
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
            switch (_talkIndex)
            {
                case 1:
                    _bell.Show();_bg1.Show(); _di.Show();
                    _spines.Hide();
                    _onClicks.gameObject.Hide();
                    BellSpeck(9);                    
                    break;
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

        #region 播放Audio
        private float PlayBgm(int index, bool isLoop = true, SoundManager.SoundType type = SoundManager.SoundType.BGM)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            Debug.Log(log);
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            Debug.Log(log);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            Debug.Log(log);
            return time;
        }
        #endregion
    }
}
