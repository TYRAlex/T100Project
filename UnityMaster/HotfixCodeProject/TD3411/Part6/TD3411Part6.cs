using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }

    public class TD3411Part6
    {
        private int talkIndex;
        private MonoBehaviour _mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject dd;


        //**********************************************

        private GameObject _mask;
        private GameObject _parent;
        private int _firstPage = 0;
        private int _thisPage = 0;
        private int _endPage = 0;


        private Vector2 _prePressPos;
        private GameObject _pages;
        private List<GameObject> _pageList;

        private bool _canTouch;
        private GameObject _switchs; //切换

        GameObject L;
        GameObject L2;
        GameObject R;
        GameObject R2;

        GameObject _change;

        bool isPlaying;

        //**********************************************


        void Start(object o)
        {
            curGo = (GameObject)o;
            _mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            dd = curTrans.Find("DD").gameObject;
            dd.SetActive(true);


            //---------------
            _canTouch = true;
            _mask = curTrans.Find("mask").gameObject;
            _mask.SetActive(true);
            _parent = curTrans.GetGameObject("Parent");
            _pageList = new List<GameObject>();
            _switchs = _parent.transform.GetGameObject("Switchs");
            _pages = _parent.transform.GetGameObject("Pages");

            for (int i = 0; i < _pages.transform.GetChild(0).transform.childCount; i++)
            {
                var child = _pages.transform.GetChild(0).transform.GetChild(i).gameObject;

                _pageList.Add(child);
                for (int j = 0; j < child.transform.childCount; j++)
                {
                    var childOfPage = child.transform.GetChild(j).gameObject;

                    Util.AddBtnClick(childOfPage, OnClickOfSpine);
                }
            }


            _firstPage = 0;
            _endPage = _pageList.Count;

            L = _switchs.transform.GetGameObject("L");
            L2 = _switchs.transform.GetGameObject("L2");
            R = _switchs.transform.GetGameObject("R");
            R2 = _switchs.transform.GetGameObject("R2");
            _change = _pages.transform.GetGameObject("Mask");
            PointerClickListener.Get(_mask).onClick = null;


            //---------------

            SlideSwitchPage(_change);
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }


        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
            };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
                    if (!isRight)
                    {
                        if (_thisPage <= 0)
                            return;

                        else
                        {
                            PageChange(L);
                        }
                    }
                    else
                    {
                        if (_thisPage >= _endPage - 1)
                            return;
                        else
                        {
                            PageChange(R);
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum)
        {
            string result = string.Empty;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    break;
                default:
                    break;
            }

            return result;
        }


        private void GameInit()
        {
            talkIndex = 1;
            isPlaying = false;
            dd.SetActive(false);
            _mask.SetActive(false);
            Input.multiTouchEnabled = false;
            SpineInit(_pageList);
            SwitchsInit(_switchs);
            InitPostion();
        }

        void GameStart()
        {
            /* SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
             _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, null,
                 () => { SoundManager.instance.ShowVoiceBtn(true); }));*/
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(dd, "1");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(dd, "2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(dd, "1");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }

        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }


        //spine点击事件
        private void OnClickOfSpine(GameObject obj)
        {
            if (_canTouch)
            {
                _canTouch = false;
                isPlaying = true;
                _mask.Show();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                var child = obj.transform.GetChild(0).gameObject;
                SpineManager.instance.DoAnimation(child, obj.name, false,
                    () => { SpineManager.instance.DoAnimation(child, obj.name + "4", false, () => { isPlaying = false; }); });
                obj.transform.SetAsLastSibling();
                BackSpineAni(obj);
            }
        }

        private void BackSpineAni(GameObject gameObject)
        {
            PointerClickListener.Get(_mask).onClick = go =>
            {
                
                if(isPlaying==false)
                {
                    isPlaying = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    var child = gameObject.transform.GetChild(0).gameObject;
                    SpineManager.instance.DoAnimation(child, gameObject.name + "2", false,
                        () =>
                        {

                            SpineManager.instance.DoAnimation(child, gameObject.name + "3", false, () =>
                            {
                                _mask.Hide();
                                _canTouch = true;
                                isPlaying = false;
                            });
                        });
                }
               
            };
        }




        //spine初始化
        void SpineInit(List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var child = list[i];
                for (int j = 0; j < child.transform.childCount; j++)
                {
                    var grandSon = child.transform.GetChild(j);
                    SpineManager.instance.DoAnimation(grandSon.transform.GetChild(0).gameObject, grandSon.name + "3",
                        true);
                }
            }
        }


        //点击初始化
        void SwitchsInit(GameObject gameObject)
        {
            SpineManager.instance.DoAnimation(R2, "R2", true);
            SpineManager.instance.DoAnimation(L2, "L2", true);
            Util.AddBtnClick(L, PageChange);
            Util.AddBtnClick(R, PageChange);
        }

        //换页
        private void PageChange(GameObject obj)
        {

            if (obj.name == "R" && _thisPage < _endPage - 1)
            {
                _mask.Show();
                PointerClickListener.Get(_mask).onClick = go => { };
                _canTouch = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                SpineManager.instance.DoAnimation(R2, "R", false,
                    () => { SpineManager.instance.DoAnimation(R2, "R2", false, () => { }); });
                for (int i = 0; i < _pageList.Count; i++)
                {
                    var pos = new Vector3(_pageList[i].transform.localPosition.x - 1920, 0,
                        0);
                    _pageList[i].transform.DOLocalMove(pos, 1f).OnComplete(() =>
                    {
                        _mask.Hide();
                        _canTouch = true;
                    });
                }

                _thisPage++;
            }
            else if (obj.name == "L" && _thisPage > 0)
            {
                _mask.Show();
                PointerClickListener.Get(_mask).onClick = go => { };
                _canTouch = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                for (int i = 0; i < _pageList.Count; i++)
                {
                    SpineManager.instance.DoAnimation(L2, "L", false,
                        () => { SpineManager.instance.DoAnimation(L2, "L2", false, () => { }); });
                    var pos = new Vector3(_pageList[i].transform.localPosition.x + 1920, 0,
                        0);
                    _pageList[i].transform.DOLocalMove(pos, 1f).OnComplete(() =>
                    {
                        _mask.Hide();
                        _canTouch = true;
                    });
                }

                _thisPage--;
            }
        }


        //初始化
        void InitPostion()
        {
            for (int i = 0; i < _pageList.Count; i++)
            {
                var child = _pageList[i];
                Debug.Log(child.name);
                child.transform.localPosition =
                    new Vector3(i * 1920, 0);
            }
        }
    }
}