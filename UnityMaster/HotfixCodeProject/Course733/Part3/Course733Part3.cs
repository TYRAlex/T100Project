using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course733Part3
    {
        #region 常用变量
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        #endregion

        #region 游戏变量
        bool isChange = false;
        bool isTrue;

        GameObject juggle;
        GameObject bigJuggle;
        GameObject backBtn;

        Image mask;

        Transform backTra;
        Transform barTra;
        #endregion

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            LoadGame();

            GameInit();

            GameStart();
        }

        void LoadGame()
        {
            juggle = curTrans.Find("juggle").gameObject;
            juggle.SetActive(false);

            bigJuggle = curTrans.Find("bigJuggle").gameObject;
            bigJuggle.SetActive(false);

            curTrans.Find("arrow").gameObject.SetActive(false);
            curTrans.Find("lastJuggle").gameObject.SetActive(true);

            barTra = bigJuggle.transform.Find("Background/mask/Bar");
            barTra.gameObject.SetActive(false);

            mask = curTrans.Find("Mask").GetImage();
            mask.gameObject.SetActive(true);
            mask.color = Color.white;

            backBtn = curTrans.Find("backBtn").gameObject;
            backBtn.SetActive(false);

            for (int i = 0; i < juggle.transform.childCount; ++i)
            {
                GameObject obj = juggle.transform.GetChild(i).gameObject;
                obj.SetActive(true);
                Util.AddBtnClick(obj, EnlargeImage);
            }

            Util.AddBtnClick(backBtn, ShrinkImage);
        }

        void GameInit()
        {
            Input.multiTouchEnabled = false;
            talkIndex = 1;
            isChange = true;

            bigJuggle.transform.Find("Background").GetRectTransform().sizeDelta = new Vector2(380, 530);
        }

        void GameStart()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                curTrans.Find("lastJuggle").gameObject.SetActive(false);
                curTrans.Find("arrow").gameObject.SetActive(true);
                juggle.SetActive(true);
                mask.gameObject.SetActive(false);
            }

            talkIndex++;
        }

        //放大图片
        void EnlargeImage(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);

            backTra = obj.transform;
            Vector3 curVector3 = backTra.localScale;

            bigJuggle.transform.position = obj.transform.position;
            bigJuggle.transform.localScale = curVector3;

            RawImage raw = bigJuggle.transform.Find("Background/mask/Obj").GetRawImage();
            raw.texture = obj.transform.Find("Obj").GetRawImage().texture;
            raw.SetNativeSize();

            isTrue = isChange && obj.transform.GetSiblingIndex() != 3;

            if (isTrue)
            {
                //正规表达式
                int num = int.Parse(System.Text.RegularExpressions.Regex.Replace(obj.name, @"[^0-9]+", ""));

                Transform partsTra = barTra.Find("Parts");
                Transform stepTra = barTra.Find("Step");
                RawImage objRaw = barTra.Find("Obj").GetRawImage();

                partsTra.GetRawImage().texture = partsTra.GetComponent<BellSprites>().texture[num - 1];
                stepTra.GetRawImage().texture = stepTra.GetComponent<BellSprites>().texture[num - 1];
                objRaw.texture = raw.texture;

                partsTra.GetRawImage().SetNativeSize();
                stepTra.GetRawImage().SetNativeSize();
                objRaw.SetNativeSize();
            }

            bigJuggle.SetActive(true);
            bigJuggle.transform.Find("Background/mask/Obj").gameObject.SetActive(true);
            obj.SetActive(false);

            mask.color = new Color(255, 255, 255, 0);
            mask.gameObject.SetActive(true);
            mask.DOColor(Color.white, 0.5f).SetEase(Ease.OutQuart);

            bigJuggle.transform.GetRectTransform().DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.InOutSine);

            bigJuggle.transform.DOScale(curVector3 * 0.9f, 0.15f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                bigJuggle.transform.DOScale(Vector3.one * 1.5f, 0.35f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    if (isTrue)
                    {
                        ChangEnlargeImage(() =>
                        {
                            backBtn.SetActive(true);
                        });
                    }
                    else backBtn.SetActive(true);
                });
            });
        }

        //缩小图片
        void ShrinkImage(GameObject obj)
        {
            obj.SetActive(false);

            if (isTrue)
            {
                ChangShrinkImage(() =>
                {
                    Shrink();
                });
            }
            else Shrink();
        }

        void Shrink()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);

            bigJuggle.transform.DOMove(backTra.position, 0.5f).SetEase(Ease.OutQuart);

            Vector3 curVector3 = backTra.localScale;

            bigJuggle.transform.DOScale(curVector3 * 0.9f, 0.35f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                bigJuggle.transform.DOScale(curVector3, 0.15f).SetEase(Ease.InOutCubic).OnComplete(() =>
                {
                    backTra.gameObject.SetActive(true);
                    bigJuggle.gameObject.SetActive(false);
                });
            });

            mask.DOColor(new Color(255, 255, 255, 0), 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                mask.gameObject.SetActive(false);
            });
        }

        //放大后切换图片
        void ChangEnlargeImage(Action method = null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

            Transform tra = bigJuggle.transform.Find("Background");

            Vector2 curVec = tra.GetRectTransform().sizeDelta;


            tra.GetRectTransform().DOSizeDelta(new Vector2(0, curVec.y), 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                tra.Find("mask/Obj").gameObject.SetActive(false);
                tra.Find("mask/Bar").gameObject.SetActive(true);

                tra.GetRectTransform().DOSizeDelta(new Vector2(curVec.x * 2f, curVec.y), 1f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    method?.Invoke();
                });
            });
        }

        //缩小前切换图片
        void ChangShrinkImage(Action method = null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

            Transform tra = bigJuggle.transform.Find("Background");

            Vector2 curVec = tra.GetRectTransform().sizeDelta;

            tra.GetRectTransform().DOSizeDelta(new Vector2(0, curVec.y), 1f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                tra.Find("mask/Obj").gameObject.SetActive(true);
                tra.Find("mask/Bar").gameObject.SetActive(false);

                tra.GetRectTransform().DOSizeDelta(new Vector2(curVec.x / 2, curVec.y), 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    method?.Invoke();
                });
            });
        }
    }
}
