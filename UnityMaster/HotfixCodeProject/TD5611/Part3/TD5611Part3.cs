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
    public class TD5611Part3
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject di;
        private GameObject bd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject _pos1;
        private GameObject _pos2;
        private GameObject _pos3;

        private Transform _level1;
        private GameObject[] _clickLevel1;
        private GameObject _ani;
        private bool[] _allClickLevel1;
        private bool _canClick;

        private Transform _level2;
        private GameObject[] _clickLevel2;
        private GameObject _return;
        private bool[] _allClickLevel2;
        private int _curIndex;
        private GameObject _curObj;
        //private GameObject anyBtn;

        private GameObject successSpine;
        private GameObject mask;


        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            di = curTrans.Find("di").gameObject;
            di.SetActive(false);
            bd = curTrans.Find("BD").gameObject; 
            bd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);

            _pos1 = curTrans.GetGameObject("Pos1");
            _pos2 = curTrans.GetGameObject("Pos2");
            _pos3 = curTrans.GetGameObject("Pos3");
            _level1 = curTrans.Find("Level1");
            _level2 = curTrans.Find("Level2");
            _return = curTrans.GetGameObject("Return");

            //anyBtn = curTrans.Find("mask/Btn").gameObject;
            //anyBtn.SetActive(false);
            //Util.AddBtnClick(anyBtn, OnClickAnyBtn);

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <returns></returns>
        //public string getBtnName(BtnEnum btnEnum)
        //{
        //    string result = string.Empty;
        //    switch (btnEnum)
        //    {
        //        case BtnEnum.bf:
        //            result = "bf";
        //            break;
        //        case BtnEnum.fh:
        //            result = "fh";
        //            break;
        //        case BtnEnum.ok:
        //            result = "ok";
        //            break;
        //        default:
        //            break;
        //    }
        //    SpineManager.instance.DoAnimation(anyBtn, result + "2", false);
        //    return result;
        //}

        //private void OnClickAnyBtn(GameObject obj)
        //{
        //    BtnPlaySound();
        //    SpineManager.instance.DoAnimation(anyBtn, obj.name, false, () =>
        //    {
        //        if (obj.name == "bf")
        //        {
        //            GameStart();
        //        }
        //        else
        //        {
        //            GameInit();
        //        }
        //        mask.gameObject.SetActive(false);
        //        anyBtn.name = "Btn";
        //    });
        //}

        private void GameInit()
        {
            talkIndex = 1;
            _allClickLevel1 = new bool[_level1.childCount];
            for (int i = 0; i < _allClickLevel1.Length; i++)
            {
                _allClickLevel1[i] = false;
            }

            _allClickLevel2 = new bool[_level2.childCount];
            for (int i = 0; i < _allClickLevel2.Length; i++)
            {
                _allClickLevel2[i] = false;
            }

            //添加事件
            for (int i = 0; i < _level1.childCount; i++)
            {
                Util.AddBtnClick(_level1.GetChild(i).gameObject, ClickEventLevel1);
            }

            for (int i = 0; i < _level2.childCount; i++)
            {
                Util.AddBtnClick(_level2.GetChild(i).gameObject, ClickEventLevel2);
            }
            Util.AddBtnClick(_return, ReturnEvent);

            //动画初始化
            for (int i = 0; i < _level1.childCount; i++)
            {
                GameObject o = _level1.GetChild(i).gameObject;
                SpineManager.instance.DoAnimation(o.transform.GetGameObject(o.name), o.name + "2", false);
            }
            for (int i = 0; i < _level2.childCount; i++)
            {
                GameObject o = _level2.transform.GetChild(i).gameObject;
                SpineManager.instance.DoAnimation(o.transform.Find(o.name).gameObject, o.name + "4", false);
            }

            _level1.GetComponent<RectTransform>().anchoredPosition = _pos2.GetComponent<RectTransform>().anchoredPosition;
            _level2.GetComponent<RectTransform>().anchoredPosition = _pos3.GetComponent<RectTransform>().anchoredPosition;
            _return.Hide();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            //小朋友们，接下来我们先看看制作椰子树需要什么材料吧！
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, 
            () => 
            {
                mask.Hide();
                _canClick = true;
            }));
        }

        //等待协程
        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        //bell说话协程
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
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                _canClick = false;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, null, 
                () => 
                {
                    bd.Hide();
                    _level1.transform.DOMoveX(_pos1.transform.position.x, 1.0f);
                    _level2.transform.DOMoveX(_pos2.transform.position.x, 1.0f);
                    mono.StartCoroutine(WaitCoroutine(
                    () =>
                    {
                        _return.Hide();
                        _canClick = true;
                    }, 1.0f));
                }));
            }
            if (talkIndex == 2)
            {
                _canClick = false;
                bd.Show();
                _return.Hide();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, null, null));
            }
            talkIndex++;
        }
        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(successSpine, "1", false, () => { SpineManager.instance.DoAnimation(successSpine, "2", false, () => {/* anyBtn.name = getBtnName(BtnEnum.fh);*/ successSpine.SetActive(false); ac?.Invoke(); }); });
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

        #region 第一关
        //点击事件
        private void ClickEventLevel1(GameObject obj)
        {
            if (_canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                _canClick = false;
                JudgeClick(obj);
                string name = obj.name;
                GameObject o = obj.transform.GetGameObject(name);
                SpineManager.instance.DoAnimation(o, name, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(o, name + "2", false);
                });
            }
        }

        //判断点击的是第几个
        void JudgeClick(GameObject obj)
        {
            int index = obj.transform.GetSiblingIndex();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, index + 1, null,
            () =>
            {
                _allClickLevel1[index] = true;
                JudgeAllClickLevel1();
                _canClick = true;
            }));
        }

        //判断是否全部点击
        void JudgeAllClickLevel1()
        {
            bool all = true;
            for (int i = 0; i < _allClickLevel1.Length; i++)
            {
                if (_allClickLevel1[i] == false)
                {
                    all = false;
                    break;
                }
            }

            if (all)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }
        #endregion

        #region 第二关

        //点击事件
        void ClickEventLevel2(GameObject obj)
        {
            if (_canClick)
            {
                SoundManager.instance.ShowVoiceBtn(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1, false);
                _canClick = false;
                string name = obj.name;
                _curIndex = obj.transform.GetSiblingIndex();
                _allClickLevel2[_curIndex] = true;
                _curObj = obj;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, _curIndex + 6,
                () =>
                {
                    _curObj.transform.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(obj.transform.GetGameObject(name), name, false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(obj.transform.GetGameObject(name), name + "3", false);
                    });
                },
                () =>
                {
                    _return.Show();
                }, 0));
            }
        }

        //返回事件
        private void ReturnEvent(GameObject obj)
        {
            _return.Hide();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2, false);

            string name = _curObj.name;
            SpineManager.instance.DoAnimation(_curObj.transform.GetGameObject(name), name + "2", false,
            () =>
            {
                SpineManager.instance.DoAnimation(_curObj.transform.GetGameObject(name), name + "4", false);
                _curObj.transform.SetSiblingIndex(_curIndex);
                _canClick = true;
                JudgeAllClickLevel2();
            });
        }

        //判断所有图是否已全部点击
        void JudgeAllClickLevel2()
        {
            bool all = true;
            for (int i = 0; i < _allClickLevel2.Length; i++)
            {
                if (_allClickLevel2[i] == false)
                {
                    all = false;
                    break;
                }
            }

            if (all)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }
        #endregion
    }
}
