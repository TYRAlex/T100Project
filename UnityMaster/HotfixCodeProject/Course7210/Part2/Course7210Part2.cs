using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course7210Part2
    {
        private GameObject bell;

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject _qiuQianBell;

        private GameObject _bg1;
        private GameObject _bg2;

        private GameObject _card01;
        private GameObject _cardClick01;
        private GameObject _card02;
        private GameObject _cardClick02;
        private GameObject _card03;
        private GameObject _cardClick03;

        public GameObject _currentClickCard;
        public int _currentClickIndex;
        private string _currentSmallAni;

        //所有卡片放大特效
        private GameObject _clockAni;
        private GameObject _fanAni;
        private GameObject _carAni;
        private GameObject _childSlideAni;
        private GameObject _shipAni;
        private GameObject _wheelAni; 
        private GameObject _topAni;
        private GameObject _treeAni;
        private GameObject _pendulumAni;
        private GameObject _currentBigAni;

        private int _level;  //游戏等级
        private GameObject _clickReturn;     //返回原位的按钮（占满全屏）
        private GameObject _dc;  //对错图标
        private bool _canShowBtn;    //只有选对了卡牌才展示语音键

        void Start(object o)
        {
            curGo = (GameObject)o;
            RestartGame(curGo);
        }
        
        void RestartGame(GameObject obj)
        {
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            Input.multiTouchEnabled = false;

            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(false);
            _qiuQianBell = curTrans.GetGameObject("qiuqianbell");
            _qiuQianBell.SetActive(true);

            _bg1 = curTrans.GetGameObject("Bg1");
            _bg2 = curTrans.GetGameObject("Bg2");
            _bg1.SetActive(true);
            _bg2.SetActive(false);

            _card01 = curTrans.GetGameObject("CardPar/Card01");
            _cardClick01 = _card01.transform.GetGameObject("1");

            _card02 = curTrans.GetGameObject("CardPar/Card02");
            _cardClick02 = _card02.transform.GetGameObject("2");

            _card03 = curTrans.GetGameObject("CardPar/Card03");
            _cardClick03 = _card03.transform.GetGameObject("3");

            _clockAni = curTrans.GetGameObject("ShowAni/Level1/ClockAni");
            _clockAni.SetActive(false);
            _fanAni = curTrans.GetGameObject("ShowAni/Level1/FanAni");
            _fanAni.SetActive(false);
            _carAni = curTrans.GetGameObject("ShowAni/Level1/CarAni");
            _carAni.SetActive(false);
            _childSlideAni = curTrans.GetGameObject("ShowAni/Level2/ChildSlideAni");
            _childSlideAni.SetActive(false);
            _shipAni = curTrans.GetGameObject("ShowAni/Level2/ShipAni");
            _shipAni.SetActive(false);
            _wheelAni = curTrans.GetGameObject("ShowAni/Level2/WheelAni");
            _wheelAni.SetActive(false);
            _topAni = curTrans.GetGameObject("ShowAni/Level3/TopAni");
            _topAni.SetActive(false);
            _treeAni = curTrans.GetGameObject("ShowAni/Level3/TreeAni");
            _treeAni.SetActive(false);
            _pendulumAni = curTrans.GetGameObject("ShowAni/Level3/PendulumAni");
            _pendulumAni.SetActive(false);

            _dc = curTrans.GetGameObject("DC");
            _clickReturn = curTrans.GetGameObject("ClickReturnZone");
            _clickReturn.SetActive(false);
            talkIndex = 1;

            _level = 1;
            _canShowBtn = false;
            SoundManager.instance.ShowVoiceBtn(false);
            //Util.AddBtnClick(_restartBtn, RestartGame);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Util.AddBtnClick(_cardClick01, ShowKaPai);
            Util.AddBtnClick(_cardClick02, ShowKaPai);
            Util.AddBtnClick(_cardClick03, ShowKaPai);

            InitAni();
            GameStart();
        }

        private void InitAni()
        {
            SpineManager.instance.DoAnimation(_dc, "kong", false);

            SpineManager.instance.DoAnimation(_card01, "kong", false);
            SpineManager.instance.DoAnimation(_card02, "kong", false);
            SpineManager.instance.DoAnimation(_card03, "kong", false);
            _cardClick01.GetComponent<Empty4Raycast>().raycastTarget = false;
            _cardClick02.GetComponent<Empty4Raycast>().raycastTarget = false;
            _cardClick03.GetComponent<Empty4Raycast>().raycastTarget = false;
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Util.AddBtnClick(_clickReturn, AllReturn);
            mono.StartCoroutine(QiuQianSpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }

        //秋千bell说话协程
        IEnumerator QiuQianSpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(_qiuQianBell, "animation2");   //说话动画
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_qiuQianBell, "animation");    //待机动画
            if (method_2 != null)
            {
                method_2();
            }
        }

        //自定义动画协程
        IEnumerator AniCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, Action method_3 = null, float len = 0)
        {
            if (method_1 != null)
            {
                method_1();
            }

            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }

            if (method_2 != null)
            {
                method_2();
            }

            float ind = 0;
            ind = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, clipIndex, false);

            yield return new WaitForSeconds(ind);

            if (method_3 != null)
            {
                method_3();
            }
        }

        //bell说话协程(此协程不需要)
        //IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        //{
        //    if (len > 0)
        //    {
        //        yield return new WaitForSeconds(len);
        //    }
        //    float ind = 0;
        //    ind = SoundManager.instance.PlayClip(clipIndex);
        //    SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
        //    if (method_1 != null)
        //    {
        //        method_1();
        //    }

        //    yield return new WaitForSeconds(ind);
        //    SpineManager.instance.DoAnimation(bell, "DAIJI");

        //    if (method_2 != null)
        //    {
        //        method_2();
        //    }
        //}
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex == 1)
            {
                mono.StartCoroutine(QiuQianSpeckerCoroutine(SoundManager.SoundType.VOICE, 1, 
                ()=>
                {
                    _qiuQianBell.SetActive(false);
                    _bg1.SetActive(false);
                    _bg2.SetActive(true);
                    _cardClick01.GetComponent<Empty4Raycast>().raycastTarget = true;
                    _cardClick02.GetComponent<Empty4Raycast>().raycastTarget = true;
                    _cardClick03.GetComponent<Empty4Raycast>().raycastTarget = true;
                    _cardClick01.SetActive(false);
                    _cardClick02.SetActive(false);
                    _cardClick03.SetActive(false);
                     SpineManager.instance.DoAnimation(_card01, "7", true); 
                     SpineManager.instance.DoAnimation(_card02, "8", true); 
                     SpineManager.instance.DoAnimation(_card03, "9", true);
                },
                ()=>
                {
                    _cardClick01.SetActive(true);
                    _cardClick02.SetActive(true);
                    _cardClick03.SetActive(true);
                }
                ));
            }
            if(talkIndex == 2)
            {
                _canShowBtn = false;
                mono.StartCoroutine(QiuQianSpeckerCoroutine(SoundManager.SoundType.VOICE, 5,
                () =>
                {
                    _cardClick01.SetActive(false);
                    _cardClick02.SetActive(false);
                    _cardClick03.SetActive(false);
                    SpineManager.instance.DoAnimation(_card01, "4", true);
                    SpineManager.instance.DoAnimation(_card02, "5", true);
                    SpineManager.instance.DoAnimation(_card03, "6", true);
                },
                () =>
                {
                    _cardClick01.SetActive(true);
                    _cardClick02.SetActive(true);
                    _cardClick03.SetActive(true);
                    _level += 1;
                }
                ));
            }
            if (talkIndex == 3)
            {
                _canShowBtn = false;
                mono.StartCoroutine(QiuQianSpeckerCoroutine(SoundManager.SoundType.VOICE, 9,
                () =>
                {
                    _cardClick01.SetActive(false);
                    _cardClick02.SetActive(false);
                    _cardClick03.SetActive(false);
                    SpineManager.instance.DoAnimation(_card01, "1", true);
                    SpineManager.instance.DoAnimation(_card02, "2", true);
                    SpineManager.instance.DoAnimation(_card03, "3", true);
                },
                () =>
                {
                    _cardClick01.SetActive(true);
                    _cardClick02.SetActive(true);
                    _cardClick03.SetActive(true);
                    _level += 1;
                }
                ));
            }
            talkIndex++;
        }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }

        //点击卡牌事件
        private void ShowKaPai(GameObject obj)
        {
            _currentSmallAni = SpineManager.instance.GetCurrentAnimationName(obj.transform.parent.gameObject);
            if (_level == 1)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 11, false);
                SoundManager.instance.ShowVoiceBtn(false);
                if (obj == _cardClick01)
                {
                    ClickCardAni(2, true, _cardClick01, _clockAni, "c1", 0);
                }
                if (obj == _cardClick02)
                {
                    ClickCardAni(3, false, _cardClick02, _fanAni, "c2", 1);
                }
                if (obj == _cardClick03)
                {
                    ClickCardAni(4, false, _cardClick03, _carAni, "c3", 2);
                }
            }
            if (_level == 2)
            {
                SoundManager.instance.ShowVoiceBtn(false);
                if (obj == _cardClick01)
                {
                    ClickCardAni(6, false, _cardClick01, _childSlideAni, "b1", 3);
                }
                if (obj == _cardClick02)
                {
                    ClickCardAni(7, true, _cardClick02, _shipAni, "b2", 4);
                }
                if (obj == _cardClick03)
                {
                    ClickCardAni(8, false, _cardClick03, _wheelAni, "b3", 5);
                }
            }
            if (_level == 3)
            {
                SoundManager.instance.ShowVoiceBtn(false);
                if (obj == _cardClick01)
                {
                    ClickCardAni(10, false, _cardClick01, _topAni, "a1", 6);
                }
                if (obj == _cardClick02)
                {
                    ClickCardAni(11, false, _cardClick02, _treeAni, "a2", 7);
                }
                if (obj == _cardClick03)
                {
                    ClickCardAni(12, true, _cardClick03, _pendulumAni, "a3", 8);
                }
            }
        }

        //所有卡牌恢复原位与原大小
        void AllReturn(GameObject obj)
        {
            SpineManager.instance.DoAnimation(_dc, "kong", false);
            _currentBigAni.SetActive(false);

            _clickReturn.Hide();
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 11, false);
            SpineManager.instance.DoAnimation(_currentClickCard, GetReturnCard(),false, 
            ()=> 
            {
                SpineManager.instance.DoAnimation(_currentClickCard, _currentSmallAni, true);
                _cardClick01.SetActive(true);
                _cardClick02.SetActive(true);
                _cardClick03.SetActive(true);
                if (_canShowBtn)
                {
                    if (_level == 3)
                        SoundManager.instance.ShowVoiceBtn(false);
                    else
                        SoundManager.instance.ShowVoiceBtn(true);
                }
            });
        }

        //判断变小的卡牌所需动画
        string GetReturnCard()
        {
            if (_currentBigAni == _clockAni)
                return "c4";
            else if(_currentBigAni == _fanAni)
                return "c5";
            else if (_currentBigAni == _carAni)
                return "c6";
            else if (_currentBigAni == _childSlideAni)
                return "b4";
            else if (_currentBigAni == _shipAni)
                return "b5";
            else if (_currentBigAni == _wheelAni)
                return "b6";
            else if (_currentBigAni == _topAni)
                return "a4";
            else if (_currentBigAni == _treeAni)
                return "a5";
            else
                return "a6";
        }

        //点击卡牌时的动画操作
        void ClickCardAni(int clipIndex, bool dcBool, GameObject clickCard, GameObject bigShowAni, string cardAni, int soundIndex)
        {
            string dc1, dc2;
            int dcIndex;
            _currentBigAni = bigShowAni;    //当前应展示的运动动画
            if(dcBool)
            {
                dc1 = "d1";
                dc2 = "d2";
                dcIndex = 9;
                _canShowBtn = true;
            }
            else
            {
                dc1 = "c1";
                dc2 = "c2";
                dcIndex = 10;
            }

            if(clickCard == _cardClick01)
                _currentClickCard = _card01;
            else if(clickCard == _cardClick02)
                _currentClickCard = _card02;
            else
                _currentClickCard = _card03;

            _currentClickCard.transform.SetAsLastSibling();

            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 11, false);
            mono.StartCoroutine(AniCoroutine(SoundManager.SoundType.VOICE, clipIndex,
            () =>
            {
                _currentClickCard.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_currentClickCard, cardAni, false);
                _cardClick01.SetActive(false);
                _cardClick02.SetActive(false);
                _cardClick03.SetActive(false);
            },
            () =>
            {
                bigShowAni.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, soundIndex, true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, dcIndex, false);
                SpineManager.instance.DoAnimation(_dc, dc1);
            },
            () =>
            {
                _clickReturn.Show();
                SpineManager.instance.DoAnimation(_dc, dc2);
            }, 0.5f
            ));
        }
    }
}
