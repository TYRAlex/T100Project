using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace ILFramework.HotClass
{
    public class TDNLPart3
    {
      

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _bell;
        private GameObject _mask;

        private GameObject _spine1;
        private RectTransform _c1Parent;
        private GameObject _c1;
        private Transform _onClick1;
        private List<string> _onClickNames;

        private RectTransform _bg1;     
        private RectTransform _bg2;


        private GameObject _spine2;
        private Transform _c2Parent;
        private Transform _onClick2;
        private CanvasSizeFitter _canvasSizeFitter;
      
       

        void Start(object o)
        {

            
            _curGo = (GameObject)o;

           
            Transform curTrans = _curGo.transform;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            _mono.StopAllCoroutines();
            _bell = curTrans.Find("bell").gameObject;
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _mask = curTrans.GetGameObject("mask");
    

            _spine1 = curTrans.GetGameObject("Spines/Spine1");
            _c1Parent = curTrans.GetRectTransform("Spines/Spine1/c1Parent");
            _c1 = curTrans.GetGameObject("Spines/Spine1/c1Parent/c1");
            _onClick1 = curTrans.GetTransform("Spines/Spine1/OnClicks1");

            _bg1 = curTrans.GetRectTransform("Bg/Bg1");           
        

            _spine2 = curTrans.GetGameObject("Spines/Spine2");
            _spine2.Show();
            _c2Parent = curTrans.GetTransform("Spines/Spine2/c2Parent");

            _onClick2 = curTrans.GetTransform("Spines/Spine2/OnClick2");
            _onClick2.gameObject.Hide();

                   _bg2 = curTrans.GetRectTransform("Bg/Bg2");
            _canvasSizeFitter = null;

            _canvasSizeFitter = _bg2.GetCanvasSizeFitter();
            Init();
            GameStart();
        }

        void Init()
        {
            PointerClickListener.Get(_mask).onClick = null;
            Debug.LogError("环节3初始化");
            _bg1.anchoredPosition = new Vector2(0, 0);
            _c1Parent.anchoredPosition = new Vector2(0, 0);
          
            _canvasSizeFitter.Action = () => {
                _bg2.anchoredPosition = new Vector2(_canvasSizeFitter.CurBackgroundV2.x, 0);
                _c2Parent.GetRectTransform().anchoredPosition = new Vector2(_canvasSizeFitter.CurBackgroundV2.x, 0);
            };


            if (_canvasSizeFitter.CurBackgroundV2.x!=0)
            {
                Debug.LogError("重置位置Bg2....");
                _bg2.anchoredPosition = new Vector2(_canvasSizeFitter.CurBackgroundV2.x, 0);
                _c2Parent.GetRectTransform().anchoredPosition = new Vector2(_canvasSizeFitter.CurBackgroundV2.x, 0);

            }


            for (int i = 0; i < _c2Parent.childCount; i++)
            {
                var child = _c2Parent.GetChild(i);
                SpineManager.instance.DoAnimation(child.gameObject, child.name, false);
            }
        }


        void GameStart()
        {
           
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            FirstLink();       
        }

        #region 点击标记

        /// <summary>
        /// 初始化标记List
        /// </summary>
        void InitOnClickNames()
        {
            _onClickNames = new List<string>();
        }

        /// <summary>
        /// 添加点击标记
        /// </summary>
        /// <param name="onClickName"></param>
        void AddToOnClickNames(string onClickName)
        {
            if (!_onClickNames.Contains(onClickName))           
                _onClickNames.Add(onClickName);                      
        }

        /// <summary>
        /// 清楚所有标记
        /// </summary>
        void ClearOnClickNames()
        {
            _onClickNames.Clear();
        }

        /// <summary>
        /// 是否完成点击
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        bool IsFinishOnClick(Transform transform)
        {
            return _onClickNames.Count == transform.childCount;
        }
        #endregion

        #region 第一个环节

        /// <summary>
        /// 第一个环节
        /// </summary>
        void FirstLink()
        {
            InitOnClickNames();
            _spine1.Show();
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 0, () => { _mask.Show(); }, () => { _mask.Hide(); }));

            for (int i = 0; i < _onClick1.childCount; i++)
            {
                var child = _onClick1.GetChild(i).gameObject;
                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = FirstLinkOnClickEvent;
            }
                      
        }

        /// <summary>
        /// 第一环节点击事件
        /// </summary>
        /// <param name="go"></param>
        void FirstLinkOnClickEvent(GameObject go)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
            string name = go.name;
            AddToOnClickNames(name);
            _mask.Show();
            int curAudioIndex = -1;
            switch (name)
            {
                case "1":
                    curAudioIndex = 1;
                    break;
                case "2":
                    curAudioIndex = 2;
                    break;
                case "3":
                    curAudioIndex = 3;
                    break;
                case "4":
                    curAudioIndex = 4;
                    break;
                case "5":
                    curAudioIndex = 5;
                    break;
                case "6":
                    curAudioIndex = 6;
                    break;
            }

            bool isFinish = IsFinishOnClick(_onClick1);
            SpineManager.instance.DoAnimation(_c1, name, false);

            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,curAudioIndex,null,()=> {

                _mask.Hide();

                if (isFinish)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                    _mask.Show();
                }                 
            }));
        }

        #endregion

        #region 第一环节到第二环节动画
        private void  CutAniFirstToSecondLink(float distance, float moveSpeed)
        {
            _spine2.Show();
            var bg1Tweener = _bg1.DOAnchorPosX(-distance, moveSpeed);
            var c1ParentTweener = _c1Parent.DOAnchorPosX(-distance, moveSpeed);
            var bg2Tweener = _bg2.DOAnchorPosX(0, moveSpeed);
            var c2ParentTweener = _c2Parent.GetRectTransform().DOAnchorPosX(0, moveSpeed);
            DOTween.Sequence().Append(bg1Tweener).Join(c1ParentTweener).Join(bg2Tweener).Join(c2ParentTweener)
                              .OnComplete(() => { _spine1.Hide(); SecondLink(); });
        }
        #endregion

        #region 第二个环节

        /// <summary>
        /// 第二个环节
        /// </summary>
        private void SecondLink()
        {
            ClearOnClickNames();
           

            for (int i = 0; i < _onClick2.childCount; i++)
            {
                var child = _onClick2.GetChild(i).gameObject;
                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = SecondLinkOnClickEvent;
            }
            _onClick2.gameObject.Show();

            _mask.Hide();

        }


        /// <summary>
        /// 第二环节点击事件
        /// </summary>
        /// <param name="go"></param>
        void SecondLinkOnClickEvent(GameObject go)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            var name = go.name;
          
            AddToOnClickNames(name);
            var aniGo = _c2Parent.Find(name).gameObject;
           
            aniGo.transform.SetAsLastSibling();

            _mask.Show();
            string showAniName = name + "2";
            string hideAniName = name + "4";

            SpineManager.instance.DoAnimation(aniGo, showAniName, false);

            int curAudioId = -1;
            switch (name)
            {
                case "a":
                    curAudioId = 8;
                    break;
                case "b":
                    curAudioId = 9;
                    break;
                case "c":
                    curAudioId = 10;
                    break;        
            }
          
            bool isFinish = IsFinishOnClick(_onClick2);
          
            //Preload("1.mp4");
            //_istempClikc = true;


            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, curAudioId, null,()=> {

                PointerClickListener.Get(_mask).onClick = o =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    PointerClickListener.Get(_mask).onClick = null;
                    SpineManager.instance.DoAnimation(aniGo, hideAniName, false, () => {

                        if (isFinish)
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                            _mask.Show();
                        }
                        else
                        {
                            _mask.Hide();
                        }                     
                    });
                };
            }));
   
        }

  


        #endregion


      

        IEnumerator Delay(float delay,Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(_bell, "2");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_bell, "1");

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
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 7, () => { _mask.Show(); }, () =>
                    {
                        CutAniFirstToSecondLink(_canvasSizeFitter.CurBackgroundV2.x, 1.0f); //滑屏切换
                    }));
                    break;
                case 2:
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 11, () => { _mask.Show(); }));
                        break;
            }
            _talkIndex++;
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
