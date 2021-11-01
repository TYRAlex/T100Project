using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class TDNLPart5
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _bell;

        private GameObject _bg;
        private Vector2 _prePressPos;
        private Transform _spine;
        private Transform _spinesMask;
        private Transform _onClicks;
        private Transform _btns;
        private GameObject _mask;

       
        private GameObject _left;
        private GameObject _right;

        private GameObject _leftAni;
        private GameObject _rightAni;

        private CanvasSizeFitter _canvasSizeFitter;
        private float _moveDis;    //移动的距离

        private int _curPageIndex;     //当前页签的index
        private int _pageIndexMin;    //页签index最小值
        private int _pageIndexMax;    //页签index最大值
   

        private float _offset;

      

        void Start(object o)
        {
            _curGo = (GameObject)o;
            Transform curTrans = _curGo.transform;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            _mono.StopAllCoroutines();

            _bell = curTrans.Find("bell").gameObject;
         
            _offset = -335;
            _talkIndex = 1;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _bg = curTrans.GetGameObject("Bg");
            UIEventListener.Get(_bg).onDown = null;
            UIEventListener.Get(_bg).onUp = null;

            UIEventListener.Get(_bg).onDown = OnDown;
            UIEventListener.Get(_bg).onUp = OnUp;

            _spine = curTrans.GetTransform("Spines");
            _spinesMask = curTrans.GetTransform("Spines/Mask");

            _onClicks = curTrans.GetTransform("OnClicks");
            _btns = curTrans.GetTransform("Btns");
            _mask = curTrans.GetGameObject("mask");


            _left = _btns.GetGameObject("LeftBtn");
            _right = _btns.GetGameObject("RightBtn");

            _leftAni = _btns.GetGameObject("leftAni");
            _rightAni = _btns.GetGameObject("RightAni");

            _canvasSizeFitter = _spine.GetCanvasSizeFitter();

            _canvasSizeFitter.Action = () => {

                _moveDis = _canvasSizeFitter.CurBackgroundV2.x;

                for (int i = 0; i < _spinesMask.childCount; i++)
                {
                    var child = _spinesMask.GetChild(i);
                    child.GetRectTransform().anchoredPosition = new Vector2(i * _moveDis+ _offset, 0);
                }            
            };

            if (_canvasSizeFitter.CurBackgroundV2.x != 0)
            {
                _moveDis = _canvasSizeFitter.CurBackgroundV2.x;
                for (int i = 0; i < _spinesMask.childCount; i++)
                {
                    var child = _spinesMask.GetChild(i);
                    child.GetRectTransform().anchoredPosition = new Vector2(i * _moveDis + _offset, 0);
                }
            }

          
            for (int i = 0; i < _spinesMask.childCount; i++)
            {
                var parent = _spinesMask.GetChild(i);
                for (int j = 0; j < parent.childCount; j++)
                {
                    var child = parent.GetChild(j).gameObject;

                    int index = -1;
                    switch (child.name)
                    {
                        case "a":
                        case "d":
                            index = 0;
                            break;
                        case "b":
                        case "e":
                            index = 1;
                            break;
                        case "c":
                        case "f":
                            index = 2;
                            break;
                    }
                    child.transform.SetSiblingIndex(index);
                }
            }

            for (int i = 0; i < _spinesMask.childCount; i++)
            {
                var parent = _spinesMask.GetChild(i);
                for (int j = 0; j < parent.childCount; j++)
                {
                    var child = parent.GetChild(j).gameObject;   
                    var name = child.name + "3";
                    SpineManager.instance.DoAnimation(child, name, false);
                }
            }

          
            InitData();

            GameStart();
        }

        private void OnDown(PointerEventData data)
        {
            _prePressPos = data.pressPosition;
        }

        private void OnUp(PointerEventData data)
        {
            float dis = (data.position - _prePressPos).magnitude;
            bool isRight = (_prePressPos.x - data.position.x) > 0 ? true : false;

            if (dis>100)
            {
                if (isRight)
                {
                    if (_curPageIndex >= _pageIndexMax)
                        return;

                    _curPageIndex++;
                    _mask.Show();

                    MoveAni(-_moveDis, 1.0f, () => {
                        _mask.Hide();
                    });
                }
                else
                {
                    if (_curPageIndex == _pageIndexMin)
                        return;

                    _curPageIndex--;
                    _mask.Show();

                    MoveAni(_moveDis, 1.0f, () => {
                        _mask.Hide();
                    });
                }
            }
        }



        private void InitData()
        {
            PointerClickListener.Get(_mask).onClick = null;
            Debug.LogError("Part5 初始化");
            _curPageIndex = 0;
            Debug.LogError("_curPageIndex 初始化："+ _curPageIndex);
            _pageIndexMin = 0;
            _pageIndexMax = _spinesMask.childCount-1;
      
        }


        void GameStart()
        {

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            AddOnClicksEvent();
            AddLeftOnClickEvent();
            AddRightOnClickEvent();

            _mask.Hide();                    
        }





        void AddLeftOnClickEvent()
        {
            PointerClickListener.Get(_left).onClick = null;
          
            PointerClickListener.Get(_left).onClick = go => {
                Debug.LogError("左击_curPageIndex：" + _curPageIndex);
                SpineManager.instance.DoAnimation(_leftAni, "z1", false);
                BtnPlaySound();
                if (_curPageIndex == _pageIndexMin)
                    return;

                _curPageIndex--;
                _mask.Show();

                MoveAni(_moveDis, 1.0f, () => {
                    _mask.Hide();
                });

            };
        }

        void AddRightOnClickEvent()
        {
            PointerClickListener.Get(_right).onClick = null;
         
            PointerClickListener.Get(_right).onClick = go => {
                Debug.LogError("右击_curPageIndex：" + _curPageIndex);
                SpineManager.instance.DoAnimation(_rightAni, "y1", false);

                BtnPlaySound();
                if (_curPageIndex >= _pageIndexMax)
                    return;

                _curPageIndex++;
                _mask.Show();

                MoveAni(-_moveDis, 1.0f, () => {
                    _mask.Hide();                
                });
            };
        }

        void MoveAni(float distance, float duration,Action action=null)
        {
            for (int i = 0; i < _spinesMask.childCount; i++)
            {
                var childRect = _spinesMask.GetChild(i).GetRectTransform();
                var endValue = childRect.anchoredPosition.x + distance;
                childRect.DOAnchorPosX(endValue, duration).OnComplete(()=> {
                    action?.Invoke();
                }); 
            }
        }


        /// <summary>
        /// 添加点击事件
        /// </summary>
        void AddOnClicksEvent()
        {
            for (int i = 0; i < _onClicks.childCount; i++)
            {
                var child = _onClicks.GetChild(i);
                PointerClickListener.Get(child.gameObject).onClick = OnClicks;
            }
        }

        void OnClicks(GameObject go)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
            _mask.Show();
           
            int index = int.Parse(go.name);

            Transform spineChild = _spinesMask.GetChild(_curPageIndex);
            GameObject aniGo = null;
            switch (spineChild.name)
            {
                case "0":

                    switch (index)
                    {
                        case 0:
                            aniGo = spineChild.GetGameObject("a");
                            break;
                        case 1:
                            aniGo = spineChild.GetGameObject("b");
                            break;
                        case 2:
                            aniGo = spineChild.GetGameObject("c");
                            break;
                    }

                    break;
                case "1":
                    switch (index)
                    {
                        case 0:
                            aniGo = spineChild.GetGameObject("d");
                            break;
                        case 1:
                            aniGo = spineChild.GetGameObject("e");
                            break;
                        case 2:
                            aniGo = spineChild.GetGameObject("f");
                            break;
                    }
                    break;

            }
            Debug.LogError("aniGo.name："+aniGo.name);
            aniGo.transform.SetAsLastSibling();
            string aniName1 = aniGo.name + "1";
            string aniName2 = aniGo.name + "2";

            SpineManager.instance.DoAnimation(aniGo, aniName1, false, () => {
                PointerClickListener.Get(_mask).onClick = o => {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    PointerClickListener.Get(_mask).onClick = null;
                    SpineManager.instance.DoAnimation(aniGo, aniName2, false, () => {
                        _mask.Hide();
                        aniGo.transform.SetSiblingIndex(index);
                    });
                };
            });
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
            SpineManager.instance.DoAnimation(_bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_bell, "DAIJI");

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
