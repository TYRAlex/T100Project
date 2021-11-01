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

    public class TD5621Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private GameObject pageBar;
        private Transform SpinePage;

        private Empty4Raycast[] e4rs;

        private GameObject rightBtn;
        private GameObject leftBtn;

        private GameObject btnBack;

        private int curPageIndex; //当前页签索引
        private Vector2 _prePressPos;

        private float textSpeed;


        private Transform SpineShow;

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

        private int flag = 0;


        private List<string> _nameList;

        //创作指引完全结束
        bool isEnd = false;

        void Start(object o)
        {
            curGo = (GameObject) o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(false);

            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);


            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }

            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);

            pageBar = curTrans.Find("Parts/PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("Parts/PageBar/MaskImg/SpinePage");
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            _nameList = new List<string>();
            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;

            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);


            SpineShow = curTrans.Find("Parts/spine0");
            SpineShow.gameObject.SetActive(true);
            for (int i = 0; i < SpineShow.childCount; i++)
            {
                Util.AddBtnClick(SpineShow.GetChild(i).gameObject, OnClickPlay);
            }

            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }


        /// <summary>
        /// 材料点击事件
        /// </summary>
        /// <param name="obj"></param>
        private void OnClickPlay(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;

            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);

            SoundManager.instance.ShowVoiceBtn(false);
            //播放对应的材料语音
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE,
                obj.transform.GetSiblingIndex() + 2, null, () =>
                {
                    isPlaying = false;
                    if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
                    {
                        flag += 1 << obj.transform.GetSiblingIndex();
                    }

                    if (flag == (Mathf.Pow(2, SpineShow.childCount) - 1))
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            SpineManager.instance.DoAnimation(SpineShow.gameObject, obj.name, false);
        }


        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= 1 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () =>
            {
                SetMoveAncPosX(-1);
                isPressBtn = false;
            });
        }

        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () =>
            {
                SetMoveAncPosX(1);
                isPressBtn = false;
            });
        }

        private GameObject tem;

        /// <summary>
        /// 图片返回
        /// </summary>
        /// <param name="obj"></param>
        private void OnClickBtnBack(GameObject obj)
        {
            if (isPressBtn)
                return;
            isPressBtn = true;
            if (_nameList.Count == 0)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name + "3", false, () =>
                {
                    tem.transform.SetSiblingIndex(level);
                    obj.SetActive(false);
                    isPlaying = false;
                    isPressBtn = false;
                    if (flag == 1 && !isEnd)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            });
        }


        private int level;

        /// <summary>
        ///  图片点击事件
        /// </summary>
        /// <param name="obj"></param>
        private void OnClickShow(GameObject obj)
        {
            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);

            SoundManager.instance.ShowVoiceBtn(false);
            tem = obj.transform.parent.gameObject;
            level = tem.transform.GetSiblingIndex();
            tem.transform.SetAsLastSibling();
            Debug.Log("点击:   " + obj.name);
            SpineManager.instance.DoAnimation(tem, obj.name, false, () =>
            {
                isPressBtn = true;
                btnBack.SetActive(true);
                RemoveName(obj);
                //播放对应图片语音                                
                Debug.Log("index:         " + level);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE,
                    level + 9, null, () => { isPressBtn = false; }));
            });
        }

        void RemoveName(GameObject go)
        {
            for (int i = 0; i < _nameList.Count; i++)
            {
                if (go.name == _nameList[i])
                {
                    _nameList.RemoveAt(i);
                }
            }
        }


        int GetChildIndex(GameObject parent, GameObject child)
        {
            int index = -1;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                if (child.name == parent.transform.GetChild(i).name) ;
            }

            Debug.Log(index + "        ");
            return index;
        }

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum, int index)
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

            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false);
                        GameInit();
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        switchBGM();
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.COMMONVOICE, 0));
                    });
                }
            });
        }

        private void GameInit()
        {
            Input.multiTouchEnabled = false;
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            textSpeed = 0.1f;
            flag = 0;
            mask.Hide();
            dbd.Hide();
            bd.Hide();
            leftBtn.transform.parent.gameObject.Hide();
            rightBtn.transform.parent.gameObject.Hide();
            anyBtns.gameObject.Hide();

            //SpineShow.transform.localPosition = new Vector3(-Screen.width, Screen.height/2);

            SlideSwitchPage(pageBar);
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            SpineShow.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            pageBar.transform.GetRectTransform().anchoredPosition = new Vector2(1920, 0);

        
            #region 图片初始化

            var a = SpinePage.transform.GetGameObject("a");
            var b = SpinePage.transform.GetGameObject("b");
            var c = SpinePage.transform.GetGameObject("c");
            var d = SpinePage.transform.GetGameObject("d");
            var e = SpinePage.transform.GetGameObject("e");

            /*a.transform.position = new Vector3(1920 , 1080);
            b.transform.position = new Vector3(1920, 1080);
            c.transform.position = new Vector3(1920 , 1080);
            d.transform.position = new Vector3(1920 * 2 , 1080);
            e.transform.position = new Vector3(1920 * 2, 1080);*/
            a.transform.SetSiblingIndex(0);
            b.transform.SetSiblingIndex(1);
            c.transform.SetSiblingIndex(2);
            d.transform.SetSiblingIndex(3);
            e.transform.SetSiblingIndex(4);
            SpineManager.instance.DoAnimation(a, "a3", true);
            SpineManager.instance.DoAnimation(b, "b3", true);
            SpineManager.instance.DoAnimation(c, "c3", true);
            SpineManager.instance.DoAnimation(d, "d3", true);
            SpineManager.instance.DoAnimation(e, "e3", true);
            for (int i = 0; i < SpinePage.transform.childCount; i++)
            {
                var child = SpinePage.transform.GetChild(i).gameObject;
                if (i <3)
                {
                    child.transform.GetRectTransform().anchoredPosition = new Vector2(8, 0);
                }
                else
                {
                    child.transform.GetRectTransform().anchoredPosition = new Vector2(1928, 0);
                }
            }

            #endregion

         
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                _nameList.Add(e4rs[i].name);
            }

            LRBtnUpdate();
        }

        void GameStart()
        {
            mask.Show();
            bd.Show();
            dbd.Hide();
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, () => { },
                () => { SoundManager.instance.ShowVoiceBtn(true); }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
        }

        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex,
            Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
            }

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, () => { },
                    () => { mask.Hide(); }));
            }

            if (talkIndex == 2)
            {
                //点击标志位
                flag = 0;
                mask.Show();
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 8, null, () =>
                {
                    mask.SetActive(false);
                    bd.SetActive(false);
                    SpineShow.transform.DOLocalMove(new Vector3(-Screen.width * 3, Screen.height/2), 1f);
                    pageBar.transform.DOLocalMove(new Vector3(0, 0), 1f).OnComplete(() =>
                    {
                        leftBtn.transform.parent.gameObject.Show();
                        rightBtn.transform.parent.gameObject.Show();
                        LRBtnUpdate();
                    });
                }));
            }

            if (talkIndex == 3)
            {
                bd.Show();
                mask.Show();
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 14, () => { },
                    () => { }));
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
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
                        () =>
                        {
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            caidaiSpine.SetActive(false);
                            successSpine.SetActive(false);
                            ac?.Invoke();
                        });
                });
        }

        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }


        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;

            SpinePage.GetRectTransform()
                .DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() =>
                {
                    LRBtnUpdate();
                    callBack?.Invoke();
                    isPlaying = false;
                });
        }

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData => { _prePressPos = downData.pressPosition; };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = Math.Abs(upData.position.x - _prePressPos.x);
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 300)
                {
                    if (!isRight)
                    {
                        if (curPageIndex <= 0 || isPlaying)
                            return;
                        SetMoveAncPosX(1);
                    }
                    else
                    {
                        if (curPageIndex >= 1 || isPlaying)
                            return;
                        SetMoveAncPosX(-1);
                    }
                }
            };
        }

        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0"); //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
                text.text += str[i];
                if (i == 25)
                {
                    text.text = "";
                }

                i++;
            }

            callBack?.Invoke();
            yield break;
        }

        private void LRBtnUpdate()
        {
            if (curPageIndex == 0)
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.name + "4", false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name,
                    false);
            }
            else if (curPageIndex == 1)
            {
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.name + "4", false);
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name,
                    false);
            }
            else
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name,
                    false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name,
                    false);
            }
        }
    }
}