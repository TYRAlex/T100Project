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
    public class TD5614Part1
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

        private Transform colorPanel;
        private Transform colorMixs;

        private Transform btns;
        private Transform colorShow;

        private Transform drawPanel;
        private Transform drawSpines;

        bool isPlaying = false;

        int selectColor = 0;
        int selectColor2 = 0;
        int voiceIndex = 0;

        int flag = 0;
        List<int> colorFlags = null;
        private Transform error;
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

            colorPanel = curTrans.Find("colorPanel");

            colorMixs = curTrans.Find("colorPanel/colorMixs");
            for (int i = 0; i < colorMixs.childCount; i++)
            {
                for (int j = 0; j < colorMixs.GetChild(i).childCount; j++)
                {
                    Util.AddBtnClick(colorMixs.GetChild(i).GetChild(j).gameObject, OnClickColorShow);
                }
            }
            btns = curTrans.Find("colorPanel/btns");
            for (int i = 0; i < btns.childCount; i++)
            {
                Util.AddBtnClick(btns.GetChild(i).gameObject, OnClickSetColorShow);
            }
            colorShow = curTrans.Find("colorPanel/colorShow");
            drawPanel = curTrans.Find("drawPanel");
            drawSpines = curTrans.Find("drawPanel/drawSpines");
            for (int i = 0; i < drawSpines.childCount; i++)
            {
                Util.AddBtnClick(drawSpines.GetChild(i).GetChild(0).GetChild(0).gameObject, OnClickDrawShow);
            }

            error = curTrans.Find("errorText");
            error.gameObject.SetActive(false);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            colorFlags = new List<int>();
            flag = 0;
            GameInit();
        }

        private void OnClickSetColorShow(GameObject obj)
        {
            selectColor = obj.transform.GetSiblingIndex();
            bool isColorMix = false;
          
            string str = Convert.ToString(flag, 2);
            if (str.Length < 3)
            {
                str = "0" + Convert.ToString(flag, 2);
            }
            int len = str.Length;
            for (int i = 0; i < len; i++)
            {
                if (len < 3)
                    break;
                if (i != selectColor)
                {
                    if (int.Parse(str[i].ToString()) <= 0)
                    {
                        isColorMix = false;
                        break;
                    }
                    isColorMix = true;
                }
            }


            if (isColorMix)
            {
                PlayErrorText("已经配过了哦~");
                return;
            }
            if (isPlaying)
                return;
            isPlaying = true;
            colorShow.GetChild(0).name = setColorShowName(selectColor);
            for (int i = 0; i < colorMixs.childCount; i++)
            {
                if (i != selectColor)
                {
                    colorMixs.GetChild(i).gameObject.SetActive(false);
                }
            }
            btns.gameObject.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            SpineManager.instance.DoAnimation(colorMixs.GetChild(selectColor).gameObject, colorMixs.GetChild(selectColor).GetChild(selectColor).name, false, () => { isPlaying = false; });
        }

        private void OnClickDrawShow(GameObject obj)
        {
            if (isPlaying)
                return;
            if ((flag & (1 << int.Parse(obj.transform.parent.parent.name))) > 0)
            {
                //PlayErrorText("已经画过了哦~ 换个颜色吧");
                return;
            }
            BtnPlaySound();
            error.gameObject.SetActive(false);
            isPlaying = true;
            obj.transform.parent.parent.SetAsLastSibling();
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            SpineManager.instance.DoAnimation(drawSpines.GetChild(drawSpines.childCount - 1).gameObject, obj.name, false, () =>
              {
                  SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "kong", false, () =>
                  {
                      isPlaying = false;
                      if ((flag & (1 << int.Parse(obj.transform.parent.parent.name))) == 0)
                      {
                          flag += 1 << int.Parse(obj.transform.parent.parent.name);
                      }
                      if (flag == (Mathf.Pow(2, drawSpines.childCount) - 1))
                      {
                          SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 9);
                          playSuccessSpine();
                      }
                  });

              });
        }

        private void OnClickColorShow(GameObject obj)
        {
            selectColor2 = obj.transform.GetSiblingIndex();
            if (isPlaying)
                return;
            if (colorFlags.Contains(selectColor + selectColor2))
            {
                PlayErrorText("已经配过了哦~");
                return;
            }
            else
            {
                colorFlags.Add(selectColor + selectColor2);
            }
            if ((flag & (1 << (selectColor + selectColor2 - 1))) == 0)
            {
                flag += 1 << (selectColor + selectColor2 - 1);
            }
            isPlaying = true;
            colorShow.GetChild(2).name = setColorShowName(selectColor2);
            colorShow.GetChild(4).name = setColorShowName(selectColor + selectColor2 + 2);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () =>
            {
                mono.StartCoroutine(colorGongShi());
            });
        }

        private string setColorShowName(int index)
        {
            string name = "";
            switch (index)
            {
                case 0:
                    name = "color3";
                    break;
                case 1:
                    name = "color2";
                    break;
                case 2:
                    name = "color";
                    break;
                case 3:
                    name = "color6";
                    if (selectColor == 0)
                    {
                        voiceIndex = 7;
                    }
                    else
                    {
                        voiceIndex = 4;
                    }

                    break;
                case 4:
                    name = "color5";
                    if (selectColor == 2)
                    {
                        voiceIndex = 5;
                    }
                    else
                    {
                        voiceIndex = 2;
                    }

                    break;
                case 5:
                    name = "color4";
                    if (selectColor == 1)
                    {
                        voiceIndex = 6;
                    }
                    else
                    {
                        voiceIndex = 3;
                    }
                    break;
                default:
                    break;
            }
            return name;
        }

        IEnumerator colorGongShi()
        {
            float temNum = 0;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, voiceIndex);
            for (int i = 0; i < colorShow.childCount; i++)
            {
                if (i != 1 && i != 3)
                {
                    SoundManager.instance.PlayClip(27);
                }
                temNum = SpineManager.instance.DoAnimation(colorShow.GetChild(i).gameObject, colorShow.GetChild(i).name, false);
                yield return new WaitForSeconds(temNum);
            }
            yield return new WaitForSeconds(3f);

            if (flag == (Mathf.Pow(2, colorMixs.childCount) - 1))
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                colorPanel.gameObject.SetActive(false);
                drawPanel.gameObject.SetActive(true);
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                flag = 0;
                mask.SetActive(true);
                dbd.SetActive(true);
                SpineManager.instance.DoAnimation(drawPanel.GetChild(0).gameObject, drawPanel.GetChild(0).name, false);
                SpineManager.instance.DoAnimation(drawPanel.GetChild(2).gameObject, drawPanel.GetChild(2).name, false, () => { isPlaying = false; });
                for (int i = 0; i < drawSpines.childCount; i++)
                {
                    SpineManager.instance.DoAnimation(drawSpines.GetChild(i).gameObject, "kong", false);
                    SpineManager.instance.DoAnimation(drawSpines.GetChild(i).GetChild(0).gameObject, drawSpines.GetChild(i).GetChild(0).name, false);
                }
                mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 8, null, () =>
                {
                    mask.SetActive(false);
                    dbd.SetActive(false);

                }));
            }
            else
            {
                GameInit();
            }
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
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); colorFlags.Clear(); flag = 0; GameInit(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 9)); });
                }
                isPlaying = false;
            });
        }

        private void GameInit()
        {
            colorPanel.gameObject.SetActive(true);
            btns.gameObject.SetActive(true);
            drawPanel.gameObject.SetActive(false);
            talkIndex = 1;

            selectColor = 0;
            selectColor2 = 0;

            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];

            for (int i = 0; i < colorPanel.childCount; i++)
            {
                if (i != 2)
                {
                    SpineManager.instance.DoAnimation(colorPanel.GetChild(i).gameObject, colorPanel.GetChild(i).name, false);
                }
            }
            for (int i = 0; i < colorMixs.childCount; i++)
            {
                colorMixs.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < colorShow.childCount; i++)
            {
                SpineManager.instance.DoAnimation(colorShow.GetChild(i).gameObject, "kong", false);
            }

            SpineManager.instance.DoAnimation(colorMixs.GetChild(0).gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(colorMixs.GetChild(0).gameObject, colorMixs.GetChild(0).name, false); });
            SpineManager.instance.DoAnimation(colorMixs.GetChild(1).gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(colorMixs.GetChild(1).gameObject, colorMixs.GetChild(1).name, false); });
            SpineManager.instance.DoAnimation(colorMixs.GetChild(2).gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(colorMixs.GetChild(2).gameObject, colorMixs.GetChild(2).name, false, () => { isPlaying = false; }); });


        }

        void GameStart()
        {
            bd.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
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
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () => { mask.SetActive(false); bd.SetActive(false); }));
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
            //caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            //SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, "2-caih2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, "2-caih", false,
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

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
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
        bool isPlayError = false;
        private void PlayErrorText(string str)
        {
            if (isPlayError)
                return;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            isPlayError = true;
            error.localPosition = Vector3.zero;
            error.GetText().text = str;
            error.GetText().color = Color.red;
            error.gameObject.SetActive(true);
            error.DOLocalMoveY(200, 1f).OnComplete(() => { error.GetText().DOFade(0, 0.5f).OnComplete(() => { error.gameObject.SetActive(false); isPlayError = false; }); });
        }

    }
}
