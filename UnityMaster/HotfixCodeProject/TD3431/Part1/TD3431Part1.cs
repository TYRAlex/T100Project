using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class TD3431Part1
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


        private GameObject bj;
        private RectTransform jzPanelMaskRect;
        private RectTransform jzPanel;
        private Transform jia;
        private Transform entrance;
        private Transform jiaApplePos;
        private Transform jiaPos;
        private Transform jiaApple;


        private Transform wwjPanel;
        private Transform wwjApplePos;
        private Transform wwjAppleShow;

        private GameObject bgLight;
        private GameObject bgLight2;
        private Transform rockingBar;
        private GameObject btn;

        private Transform fixedPanel;
        private Transform fixedStartPos;
        private Transform fixedEndPos;
        private Transform fixedApple;
        private GameObject fixedAppleShow;
        private Transform fixedShowStartPos;
        private Transform fixedShowEndPos;

        private float angle;
        public float Angle
        {
            get => angle;
            set
            {
                if (value > 20)
                {
                    angle = 20;
                }
                else if (value < -20)
                {
                    angle = -20;
                }
                else
                {
                    angle = value;
                }
            }
        }

        //胜利动画名字
        private string sz;
        bool isPlaying = false;

        int curLevel = 0;

        List<string> list;
        List<string> list2;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();



            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);


            bd = curTrans.Find("mask/BD").gameObject;
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

            bj = curTrans.Find("bj").gameObject;
            jzPanelMaskRect = curTrans.Find("jzPanelMask").GetRectTransform();
            jzPanel = jzPanelMaskRect.Find("jzPanel").GetRectTransform();
            jiaPos = jzPanel.Find("startPos");
            jia = jzPanel.Find("jia");
            entrance = jia.Find("entrance");
            jiaApplePos = entrance.Find("startPos");
            jiaApple = entrance.Find("pg");

            wwjPanel = curTrans.Find("wwjPanel");
            wwjApplePos = wwjPanel.Find("startPos");
            wwjAppleShow = wwjPanel.Find("pg");


            bgLight = curTrans.Find("d-y").gameObject;
            bgLight2 = curTrans.Find("d-y/d-h").gameObject;
            rockingBar = curTrans.Find("d-y/rockingBar");
            SlideSwitchPage(rockingBar.gameObject);

            btn = curTrans.Find("d-y/nn1").gameObject;
            Util.AddBtnClick(btn.transform.GetChild(0).gameObject, OnClickBtn);

            fixedPanel = curTrans.Find("fixedPanel");
            fixedStartPos = fixedPanel.Find("startPos");
            fixedEndPos = fixedPanel.Find("endPos");
            fixedAppleShow = fixedPanel.Find("pg").gameObject;
            fixedApple = fixedPanel.Find("pg-");
            fixedShowStartPos = fixedPanel.Find("appleStartPos");
            fixedShowEndPos = fixedPanel.Find("appleEndPos");

            //替换胜利动画需要替换spine 
            sz = "3-5-z";
            list = new List<string> { "d", "a", "f" };
            list2 = new List<string> { "c", "b", "e" };


            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            curLevel = 0;
            GameInit();
            //GameStart();
        }

        Vector3 temVec3;
        Vector2 temVec;
        private void SlideSwitchPage(GameObject rayCastTarget)
        {

            UIEventListener.Get(rayCastTarget).onBeginDrag = beginDragData =>
            {
                temVec = beginDragData.position;
            };
            UIEventListener.Get(rayCastTarget).onDrag = dragData =>
             {
                 if (isPlaying)
                     return;
                 float temX = dragData.position.x;
                 float jzPanelXMax = jzPanel.anchoredPosition.x + 10;
                 float jzPanelXMin = jzPanel.anchoredPosition.x - 10;
                 if (temX > temVec.x)
                 {
                     rockingBar.eulerAngles = new Vector3(0, 0, Angle--);
                     if (jzPanelXMax > 220)
                         return;
                     jzPanel.anchoredPosition = new Vector2(jzPanelXMax, jzPanel.anchoredPosition.y);
                 }
                 if (temX < temVec.x)
                 {
                     if (jzPanelXMin < -220)
                         return;
                     rockingBar.eulerAngles = new Vector3(0, 0, Angle++);
                     jzPanel.anchoredPosition = new Vector2(jzPanelXMin, jzPanel.anchoredPosition.y);
                 }
             };

            UIEventListener.Get(rayCastTarget).onEndDrag = endDragData =>
            {
                rockingBar.eulerAngles = temVec3;
                Angle = 0;
            };
        }


        private void OnClickBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(btn, obj.name, false,
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    SpineManager.instance.DoAnimation(jia.gameObject, jia.name + 2, false,
                        () =>
                        {
                            jia.GetRectTransform().DOAnchorPosY(jia.GetRectTransform().anchoredPosition.y * 2, 1).OnComplete(
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(jia.gameObject, jia.name + 1, false,
                                        () =>
                                        {
                                            DetectionEvent();
                                        });
                                });
                        });
                });
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
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); isPlaying = false; GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); curLevel = 0; GameInit(); ShowAppleSpine(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2)); });
                }

            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            isPlaying = false;

            temVec3 = Vector3.zero;
            temVec = Vector2.zero;
            rockingBar.eulerAngles = temVec3;
            Angle = 0;

            SpineManager.instance.DoAnimation(jia.gameObject, jia.name + 2, false);
            SpineManager.instance.DoAnimation(bj, bj.name, true);
            jia.position = jiaPos.position;
            jiaApple.position = jiaApplePos.position;
            jiaApple.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(jiaApple.gameObject, "kong", false);

            wwjAppleShow.position = wwjApplePos.position;
            wwjAppleShow.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(wwjAppleShow.gameObject, fixedAppleShow.name + "-" + list[curLevel], true);
            SpineManager.instance.SetTimeScale(bgLight, 0.5f);
            SpineManager.instance.SetTimeScale(bgLight2, 0.5f);
            SpineManager.instance.DoAnimation(bgLight, bgLight.name + 1, false);
            SpineManager.instance.DoAnimation(bgLight2, bgLight2.name + 1, false);
            SpineManager.instance.DoAnimation(btn, btn.name, false);
            fixedAppleShow.transform.position = fixedShowEndPos.position;
            fixedApple.position = fixedStartPos.position;
            
        }

        void ShowAppleSpine()
        {
            SpineManager.instance.DoAnimation(fixedAppleShow, fixedAppleShow.name + "-" + list2[curLevel], true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            fixedAppleShow.transform.DOMove(fixedShowStartPos.position,1).SetEase(Ease.InCirc);
            SpineManager.instance.DoAnimation(wwjAppleShow.gameObject, fixedAppleShow.name + "-" + list[curLevel], true);
            SpineManager.instance.DoAnimation(fixedApple.gameObject, fixedApple.name + list[curLevel] + 3, true);
        }

        IEnumerator WaitTime(float time, Action ac)
        {
            yield return new WaitForSeconds(time);
            ac?.Invoke();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 3, true);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () => { mask.SetActive(false); bd.SetActive(false); ShowAppleSpine(); }));
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
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
                () =>
                {
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
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


        void DetectionEvent()
        {
            if(IsRectTransformOverlap(wwjAppleShow.GetChild(0).GetRectTransform(), jia.GetChild(0).GetRectTransform()))
            {              
                SpineManager.instance.DoAnimation(jiaApple.gameObject, jiaApple.name + "-" + list[curLevel] + 2, true);
                wwjAppleShow.GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(wwjAppleShow.gameObject, "kong", false);

                SpineManager.instance.DoAnimation(bgLight, bgLight.name + 2, false);
                SpineManager.instance.DoAnimation(bgLight2, bgLight2.name + 2, false);
                jia.GetRectTransform().DOAnchorPosY(jia.GetRectTransform().anchoredPosition.y / 2, 1).OnComplete(
                               () =>
                               {
                                   jzPanel.DOAnchorPosX(-176, 1).OnComplete(
                                       () =>
                                       {
                                           SpineManager.instance.DoAnimation(jia.gameObject, jia.name + 2, false,
                                            () =>
                                            {

                                                SpineManager.instance.DoAnimation(jiaApple.gameObject, jiaApple.name + "-" + list[curLevel] + 3, true);
                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                                                jiaApple.DOLocalMoveY(-500, 1).OnComplete(
                                                    () =>
                                                    {
                                                        fixedApple.DOMove(fixedEndPos.position, 1).OnComplete(
                                                            () =>
                                                            {
                                                                SpineManager.instance.DoAnimation(fixedApple.gameObject, fixedApple.name + list[curLevel] + 4, true);
                                                                SpineManager.instance.DoAnimation(fixedAppleShow.gameObject, fixedAppleShow.name + "-" + list2[curLevel] + 4, true);
                                                                mono.StartCoroutine(
                                                                     WaitTime(2f,
                                                                    () =>
                                                                    {
                                                                        curLevel++;
                                                                        if (curLevel >= list.Count)
                                                                        {
                                                                            isPlaying = false;
                                                                            playSuccessSpine();
                                                                        }
                                                                        else
                                                                        {
                                                                            GameInit();
                                                                            ShowAppleSpine();
                                                                        }
                                                                    })
                                                                        );
                                                            });
                                                    });

                                            });
                                       });
                               });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
                jia.GetRectTransform().DOAnchorPosY(jia.GetRectTransform().anchoredPosition.y / 2, 1).OnComplete(
                            () =>
                            {

                                SpineManager.instance.DoAnimation(jia.gameObject, jia.name + 2, false,
                                 () =>
                                 {
                                     isPlaying = false;
                                 });

                            });
            }

        }


        public static bool IsRectTransformOverlap(RectTransform rect1, RectTransform rect2)
        {
            float rect1MinX = rect1.position.x - rect1.rect.width / 2;
            float rect1MaxX = rect1.position.x + rect1.rect.width / 2;
            float rect1MinY = rect1.position.y - rect1.rect.height / 2;
            float rect1MaxY = rect1.position.y + rect1.rect.height / 2;

            float rect2MinX = rect2.position.x - rect2.rect.width / 2;
            float rect2MaxX = rect2.position.x + rect2.rect.width / 2;
            float rect2MinY = rect2.position.y - rect2.rect.height / 2;
            float rect2MaxY = rect2.position.y + rect2.rect.height / 2;

            bool xNotOverlap = rect1MaxX <= rect2MinX || rect2MaxX <= rect1MinX;
            bool yNotOverlap = rect1MaxY <= rect2MinY || rect2MaxY <= rect1MinY;

            bool notOverlap = xNotOverlap || yNotOverlap;

            return !notOverlap;
        }

    }
    }
