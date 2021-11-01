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

    public class TD6712Part4
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject anyBtn;

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

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

        //------------------------
        private GameObject _parent;
        private GameObject _ani;
        private GameObject e;

        private GameObject _aniMask;
        //------------------------

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

            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");

            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);


            anyBtn = curTrans.Find("mask/Btn").gameObject;
            anyBtn.SetActive(false);
            //anyBtn.name = getBtnName(BtnEnum.bf);
            Util.AddBtnClick(anyBtn, OnClickAnyBtn);

            pageBar = curTrans.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;

            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            btnBack = curTrans.Find("btnBack").gameObject;
            btnBack.SetActive(false);

            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            //----------------------------------------------------------------
            _parent = curTrans.GetGameObject("parent");
            _ani = _parent.transform.GetGameObject("ani");
            e = _parent.transform.GetGameObject("e");
            _aniMask = _parent.transform.GetGameObject("animask");
            Util.AddBtnClick(e, Onclick);

            //----------------------------------------------------------------
            GameInit();
            GameStart();
        }

        private void Onclick(GameObject obj)
        {
            _aniMask.Show();
            btnBack.Show();
            BackSpineAni();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, () => { btnBack.Hide(); }));
            SpineManager.instance.DoAnimation(_ani, "e1", false, () => { });
        }

        void BackSpineAni()
        {
            PointerClickListener.Get(_aniMask).onClick = go =>
            {
                _aniMask.Hide();
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
                btnBack.Show();
                SpineManager.instance.DoAnimation(_ani, "e2", false, () =>
                {
                    btnBack.Hide();
                    SoundManager.instance.ShowVoiceBtn(true);
                });
            };
        }


        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn)
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


        private void OnClickShow(GameObject obj)
        {
            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);

            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false,
                        () => { });
                });
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
                else if (obj.name == "fh")
                {
                    GameInit();
                }
                else
                {
                }

                mask.gameObject.SetActive(false);
                anyBtn.name = "Btn";
            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            curPageIndex = 0;
            SpineManager.instance.DoAnimation(_ani, "e", true);
            _aniMask.Hide();
            isPressBtn = false;
            textSpeed = 0.5f;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(curPageIndex * 1920, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name,
                false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name,
                false);
        }

        void GameStart()
        {
            mask.Show();
            bd.Show();

            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () =>
            {
                bd.Hide();
                mask.Hide();
                isPlaying = false;
            }));
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
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
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
                mask.Show();
                bd.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null, () => { }));
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
                            /* anyBtn.name = getBtnName(BtnEnum.fh);*/
                            caidaiSpine.SetActive(false);
                            successSpine.SetActive(false);
                            ac?.Invoke();
                        });
                });
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


        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;
            SpinePage.GetRectTransform()
                .DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() =>
                {
                    callBack?.Invoke();
                    isPlaying = false;
                });
        }

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData => { _prePressPos = downData.pressPosition; };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
                    if (!isRight)
                    {
                        if (curPageIndex <= 0 || isPlaying)
                            return;
                        SetMoveAncPosX(1);
                    }
                    else
                    {
                        if (curPageIndex >= SpinePage.childCount - 1 || isPlaying)
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
                i++;
            }

            callBack?.Invoke();
            yield break;
        }
    }
}