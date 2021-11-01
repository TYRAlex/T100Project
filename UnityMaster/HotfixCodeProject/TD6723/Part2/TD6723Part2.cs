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

    public class TD6723Part2
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;


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

        private List<String> _name;

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

            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);


            pageBar = curTrans.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            // SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = true;
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);
            _name=new List<string>();
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;

            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);


            SpineShow = curTrans.Find("spine0");
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
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);

       
            //播放对应的材料语音
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND,
                obj.transform.GetSiblingIndex(), null, () =>
                {
                    isPlaying = false;
                    if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
                    {
                        flag += 1 << obj.transform.GetSiblingIndex();
                    }
                    if (flag == 511)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            Debug.Log("flag"+flag);
            SpineManager.instance.DoAnimation(SpineShow.GetGameObject("ani"), obj.name, false);
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
        
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name + "3", false, () =>
                {
                    obj.SetActive(false);
                    isPlaying = false;
                    isPressBtn = false;
                    if (_name.Count==0)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            });
        }


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
            tem = obj.transform.parent.gameObject;
            SoundManager.instance.ShowVoiceBtn(false);
            var index = 0;
            if (obj.name == "a")
            {
                index = 3;
            }

            if (obj.name == "b")
            {
                index = 4;
            }
            SpineManager.instance.DoAnimation(tem, obj.name, false, () =>
            {
                isPressBtn = true;
                btnBack.SetActive(true);
                ReName(obj);
                //播放对应图片语音                                
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE,
                    index, null, () => { isPressBtn = false; }));
            });
            
        }

        void ReName(GameObject go)
        {
            for (int i = 0; i < _name.Count; i++)
            {
                if(go.name==_name[i]) _name.RemoveAt(i);
            }
        }


        private void GameInit()
        {
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

            SlideSwitchPage(pageBar);
            SpinePage.GetRectTransform().anchoredPosition=new Vector2(0,0);
            SpineShow.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            pageBar.transform.GetRectTransform().anchoredPosition = new Vector2(1920, 0);
            
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name + "3",
                    false);
                if(SpinePage.GetChild(i).name=="a") SpinePage.GetChild(i).SetSiblingIndex(0);
            }
            _name.Clear();
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                _name.Add(e4rs[i].name);
            }
            
            
            SpineManager.instance.DoAnimation(SpineShow.GetChild(0).gameObject, "0", true);

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
                bd.Hide();
                SpineShow.transform.DOMove(new Vector3(-Screen.width/2 , Screen.height/2), 1f);
                pageBar.transform.DOMove(new Vector3(Screen.width/2, Screen.height/2), 1f).OnComplete(() =>
                {
                    mask.Show();
                    bd.Show();
                    leftBtn.transform.parent.gameObject.Show();
                    rightBtn.transform.parent.gameObject.Show();
                    LRBtnUpdate();
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null, () =>
                    {
                        mask.Hide();
                        bd.Hide();
                    }));
                });
            }

            if (talkIndex == 3)
            {
                mask.Show();
                bd.Show();
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 5, null, () =>
                {
                   
                }));
            }

            talkIndex++;
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
                        if (curPageIndex >= SpinePage.childCount - 1 || isPlaying)
                            return;
                        SetMoveAncPosX(-1);
                    }
                }
            };
        }

        private void LRBtnUpdate()
        {
            if (curPageIndex == 0)
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.name + "4", false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
            else if (curPageIndex ==  1)
            {
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.name + "4", false);
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            }
            else
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
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
    }
}