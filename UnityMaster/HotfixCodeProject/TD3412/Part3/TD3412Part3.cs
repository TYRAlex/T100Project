using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
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
    public class TD3412Part3
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject di;
        private GameObject bd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject anyBtn;

        private GameObject successSpine;
        private GameObject mask;

        private Transform _pos1;
        private Transform _pos2;
        private Transform _pos3;

        private Transform _level1;
        private GameObject[] _click1;
        private GameObject _ani1;
        private bool _canClick1;
        private bool[] _clickCount1;

        private Transform _level2;
        private GameObject[] _click2;
        private GameObject[] _ani2;
        private GameObject _return;
        private bool _canClick2;
        private bool[] _clickCount2;
        private GameObject _curObj;
        private int _curIndex;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            di = curTrans.Find("di").gameObject;
            di.SetActive(false);
            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);

            anyBtn = curTrans.Find("mask/Btn").gameObject;
            anyBtn.SetActive(false);
            //anyBtn.name = getBtnName(BtnEnum.bf);
            Util.AddBtnClick(anyBtn, OnClickAnyBtn);

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _pos1 = curTrans.Find("Pos1");
            _pos2 = curTrans.Find("Pos2");
            _pos3 = curTrans.Find("Pos3");

            _level1 = curTrans.Find("Level1");
            _click1 = new GameObject[_level1.Find("Click").childCount];
            for (int i = 0; i < _level1.Find("Click").childCount; i++)
            {
                _click1[i] = _level1.Find("Click").GetChild(i).gameObject;
                Util.AddBtnClick(_click1[i], ClickEventLevel1);
            }
            _clickCount1 = new bool[_level1.Find("Click").childCount];
            _ani1 = _level1.GetGameObject("Ani");
            _canClick1 = false;

            _level2 = curTrans.Find("Level2");
            _click2 = new GameObject[_level2.Find("Click").childCount];
            for (int i = 0; i < _level2.Find("Click").childCount; i++)
            {
                _click2[i] = _level2.Find("Click").GetChild(i).gameObject;
                Util.AddBtnClick(_click2[i], ClickEventLevel2);
            }
            _clickCount2 = new bool[_level2.Find("Click").childCount];
            _ani2 = new GameObject[_level2.Find("Ani").childCount];
            for (int i = 0; i < _level2.Find("Ani").childCount; i++)
            {
                _ani2[i] = _level2.Find("Ani").GetChild(i).gameObject;
            }
            _return = _level2.GetGameObject("Return");
            Util.AddBtnClick(_return, ReturnEvent);
            _canClick2 = false;

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameInit();
            GameStart();
        }

        //动画位置等初始化
        void GameInit()
        {
            _level1.localPosition = _pos2.localPosition;
            _level2.localPosition = _pos3.localPosition;
            SpineManager.instance.DoAnimation(_ani1, "0");

            _return.Hide();
            for (int i = 0; i < _ani2.Length; i++)
            {
                _ani2[i].Show();
            }

            for (int i = 0; i < _ani2.Length; i++)
            {
                SpineManager.instance.DoAnimation(_ani2[i], _ani2[i].name + "3", false);
            }
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
            SpineManager.instance.DoAnimation(anyBtn, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(anyBtn, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    GameStart();
                }
                else
                {
                    GameInit();
                }
                mask.gameObject.SetActive(false);

            });
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            //太好了，马上要绘画花园了，绘画花园的材料需要用到哪些呢？
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, 
            () => 
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    mask.Hide();
                    _canClick1 = true;
                }));
            }));
        }

        //DD说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bd, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                //认识完材料以后，来看看天空的颜色有哪些
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8, 
                () => 
                { 
                    _canClick1 = false;
                }, 
                () => 
                {
                    bd.Hide();
                    _level1.DOLocalMoveX(_pos1.localPosition.x, 1.0f);
                    _level2.DOLocalMoveX(_pos2.localPosition.x, 1.0f);
                    mono.StartCoroutine(WaitCoroutine(
                    () =>
                    {
                        _canClick2 = true;
                    },1.0f));
                }));
            }
            if (talkIndex == 2)
            {
                //天空可真美啊，我们看看天空怎么画的吧
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 12,
                () =>
                {
                    bd.Show();
                    _canClick2 = false;
                }, null));
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }

        #region 第一关
        //点击事件
        private void ClickEventLevel1(GameObject obj)
        {
            if(_canClick1)
            {
                _canClick1 = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                int index = obj.transform.GetSiblingIndex();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, index + 2,
                () =>
                {
                    SpineManager.instance.DoAnimation(_ani1, obj.name, false, () => { SpineManager.instance.DoAnimation(_ani1, "0", false); });
                    _clickCount1[index] = true;
                },
                () =>
                {
                    _canClick1 = true;
                    JudgeAllClick1();
                }, 0));
            }
        }

        //判断所有图是否已全部点击
        void JudgeAllClick1()
        {
            bool allClick = true;
            for (int i = 0; i < _clickCount1.Length; i++)
            {
                if(_clickCount1[i] != true)
                {
                    allClick = false;
                }
            }

            if (allClick)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }
        #endregion

        #region 第二关
        private void ClickEventLevel2(GameObject obj)
        {
            if (_canClick2)
            {
                _canClick2 = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1, false);
                _curIndex = obj.transform.GetSiblingIndex();
                _curObj = _ani2[_curIndex];
                SoundManager.instance.ShowVoiceBtn(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, _curIndex + 9,
                () =>
                {
                    _curObj.transform.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(_ani2[_curIndex], _ani2[_curIndex].name, false);
                    _clickCount2[_curIndex] = true;
                },
                () =>
                {
                    _return.Show();
                }, 0));
            }
        }

        //判断所有图是否已全部点击
        void JudgeAllClick2()
        {
            bool allClick = true;
            for (int i = 0; i < _clickCount2.Length; i++)
            {
                if (_clickCount2[i] != true)
                {
                    allClick = false;
                }
            }

            if (allClick)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }

        private void ReturnEvent(GameObject obj)
        {
            _return.Hide();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2, false);
            SpineManager.instance.DoAnimation(_curObj, _curObj.name + "2", false,
            () =>
            {
                _curObj.transform.SetSiblingIndex(_curIndex);
                _canClick2 = true;
                JudgeAllClick2();
            });
        }
        #endregion
    }
}
