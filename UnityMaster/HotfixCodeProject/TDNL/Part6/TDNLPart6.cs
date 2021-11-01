using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
namespace ILFramework.HotClass
{
    public class TDNLPart6
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _bell;

        private GameObject _mask;

        private GameObject _spine3;
        private Transform _c3Parent;
        private Transform _onClick3;
        private CanvasSizeFitter _canvasSizeFitter;

        private List<string> _onClickNames;

  
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

            _spine3 = curTrans.GetGameObject("Spine3");
            _c3Parent = curTrans.GetTransform("Spine3/c3Parent");
            _onClick3 = curTrans.GetTransform("Spine3/OnClick3");
            _canvasSizeFitter = _c3Parent.GetCanvasSizeFitter();


            GameStart();
        }

        void GameStart()
        {

            _mask.Show();
            Init();
           
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            
            _c3Parent.GetRectTransform().DOAnchorPosX(0, 1).OnComplete(() => {
                _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 3, () => { _mask.Show(); }, () => { _mask.Hide(); }));
            });

        }

        void Init()
        {
            PointerClickListener.Get(_mask).onClick = null;
            Debug.LogError("初始化 Part6");
            _talkIndex = 1;

            InitOnClickNames();

            _canvasSizeFitter.Action=()=>{           
                _c3Parent.GetRectTransform().anchoredPosition = new Vector2(_canvasSizeFitter.CurBackgroundV2.x, 0);
            };

            if (_canvasSizeFitter.CurBackgroundV2.x != 0)
            {
                Debug.LogError("重置c3Parent 位置...");
                _c3Parent.GetRectTransform().anchoredPosition = new Vector2(_canvasSizeFitter.CurBackgroundV2.x, 0);
            }

            for (int i = 0; i < _c3Parent.childCount; i++)
            {
                var child = _c3Parent.GetChild(i);

                int index = -1;
                switch (child.name)
                {
                    case "d":
                        index = 0;
                        break;
                    case "e":
                        index = 1;
                        break;
                    case "f":
                        index = 2;
                        break;
                }

                child.transform.SetSiblingIndex(index);              
            }

            for (int i = 0; i < _c3Parent.childCount; i++)
            {
                var child = _c3Parent.GetChild(i);
                SpineManager.instance.DoAnimation(child.gameObject, child.name, false);
            }

            AddOnClickEvent();
        }

        void AddOnClickEvent()
        {
            for (int i = 0; i < _onClick3.childCount; i++)
            {
                var child = _onClick3.GetChild(i).gameObject;
                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = OnClick;
            }
        }

        void OnClick(GameObject go)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
            string name = go.name;
            AddToOnClickNames(name);
            var aniGo = _c3Parent.Find(name).gameObject;
            aniGo.transform.SetAsLastSibling();
            Debug.LogError("aniGo.name："+ aniGo.name);
            _mask.Show();

            string showAniName = name + "2";
            string hideAniName = name + "4";

            SpineManager.instance.DoAnimation(aniGo, showAniName, false);

            int curAudioId = -1;
            switch (name)
            {
                case "d":
                    curAudioId = 0;
                    break;
                case "e":
                    curAudioId = 1;
                    break;
                case "f":
                    curAudioId = 2;
                    break;
            }

            bool isFinish = IsFinishOnClick(_onClick3);

            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, curAudioId, null, () => {

                PointerClickListener.Get(_mask).onClick = o => {

                    PointerClickListener.Get(_mask).onClick = null;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);

                    SpineManager.instance.DoAnimation(aniGo, hideAniName, false, () => {

                        _mask.Hide();

                        if (isFinish)
                        {  SoundManager.instance.ShowVoiceBtn(true);
                            _mask.Show();
                        }
                    });                
                };
            }));
        }

        #region 标记
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
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,4,()=> { _mask.Show(); },null));
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
