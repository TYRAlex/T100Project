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
    public class TD5612Part1
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
        private GameObject spineShow;
        private Transform cats;
        private GameObject mask;

        private int curIndex = 0;
        bool isPlaying = false;

        //胜利动画名字
        private string tz;
        private string sz;
        private List<int> SelectCats;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();


            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            spineShow = curTrans.Find("spineShow").gameObject;
            spineShow.SetActive(true);

            cats = curTrans.Find("cats");

            for (int i = 0; i < cats.childCount; i++)
            {
                Util.AddBtnClick(cats.GetChild(i).GetChild(0).gameObject, OnClickSelect);
            }

            mask = curTrans.Find("mask").gameObject;


            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            mask.SetActive(true);

            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);

            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }


        private void OnClickSelect(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            string tem = "";
            bool isSucceed = obj.transform.parent.GetSiblingIndex() == (curIndex - 1);
            if (isSucceed)
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                BtnPlaySoundSuccess();
                tem = obj.name+"2";
               
            }
            else
            {
                BtnPlaySoundFail();
                tem = obj.name;
            }
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, tem, false, () =>
            {
                if (isSucceed)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
                    SpineManager.instance.DoAnimation(spineShow, "h", false, () =>
                    {
                        SpineManager.instance.DoAnimation(spineShow, "kong", false, () =>
                        {
                            RandomSpineShow(true);
                            if (SelectCats.Count <= 0)
                            {
                                playSuccessSpine();
                            }
                        });
                    });
                }
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
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false);
                        GameInit();
                    });                                     
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => {
                        switchBGN();
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd,SoundManager.SoundType.VOICE, 2));
                    });                  
                }

                isPlaying = false;
            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            isPlaying = true;
            if (SelectCats == null || SelectCats.Count < 4)
            {
                SelectCats = new List<int>() { 1, 2, 3, 4 };
            }

            for (int i = 0; i < cats.childCount; i++)
            {
                SpineManager.instance.DoAnimation(cats.GetChild(i).gameObject, cats.GetChild(i).name,false);
            }
            RandomSpineShow();
        }

        void RandomSpineShow(bool isSelect = false)
        {
            if (isSelect)
            {
                SelectCats.RemoveAt(SelectCats.IndexOf(curIndex));
            }
            if (SelectCats.Count > 0)
            {
                int tem = Random.Range(0, SelectCats.Count);
                curIndex = SelectCats[tem];
            }
            SpineManager.instance.DoAnimation(spineShow, "m" + curIndex, true);
            isPlaying = false;

        }
        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));

        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(GameObject speak,SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speak, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speak, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speak, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE, 1, null, () => {
                    bd.SetActive(false);
                    mask.SetActive(false);
                }));

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
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                });
                });
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void switchBGN()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM,4,true);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4),null,()=> { isPlaying = false; }));
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () => { isPlaying = false; }));
        }
    }

}
