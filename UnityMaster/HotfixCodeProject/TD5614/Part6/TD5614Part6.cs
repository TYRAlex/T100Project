using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using RandomNew = System.Random;
namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD5614Part6
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform paoPaoSpines;
        private Transform scoreCard;

        private Transform anyBtns;
        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;


        bool isPlaying = false;
        string[] faceLevels = null;
        private List<int> selectLevels = null;
        private int curIndex = 0;

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

            faceLevels = new string[] { "boom", "noa", "nob", "noc" };

            paoPaoSpines = curTrans.Find("paoPaoSpines");
            scoreCard = curTrans.Find("scoreCard");


            for (int i = 0; i < paoPaoSpines.childCount - 1; i++)
            {
                Util.AddBtnClick(paoPaoSpines.GetChild(i).gameObject, OnClickSelectPaoPao);
            }

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }

        private void OnClickSelectPaoPao(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            bool isSucceed = obj.name == curIndex + "-" + "boom";
            if (isSucceed)
            {                
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.COMMONSOUND, 4,()=> { SoundManager.instance.skipBtn.SetActive(false);  }, ()=> { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0); });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            }


            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (isSucceed)
                {
                    BtnPlaySoundSuccess();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    SpineManager.instance.DoAnimation(scoreCard.GetChild(selectLevels.Count - 1).GetChild(0).gameObject, "z", false, 
                        () => {
                            scoreCard.GetChild(selectLevels.Count-1).GetImage().sprite = bellTextures.sprites[1];

                            scoreCard.DOMove(scoreCard.position, 1).OnComplete(()=> {
                                RandomLevel(true);
                                if (selectLevels.Count <= 0)
                                {
                                    playSuccessSpine();
                                }
                            });
                          
                        });                   
                }
                else
                {
                    BtnPlaySoundFail();
                    SpineManager.instance.DoAnimation(obj, obj.name + "2", true);
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
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2)); });
                }

            });
        }

        private void RandomArr(string[] arr)
        {
            RandomNew r = new RandomNew();//创建随机类对象，定义引用变量为r
            for (int i = 0; i < arr.Length; i++)
            {
                int index = r.Next(arr.Length);//随机获得0（包括0）到arr.Length（不包括arr.Length）的索引
                string temp = arr[i];  //当前元素和随机元素交换位置
                arr[i] = arr[index];
                arr[index] = temp;
            }
        }

        void RandomLevel(bool isSelect = false)
        {
            if (isSelect)
            {
                selectLevels.RemoveAt(selectLevels.IndexOf(curIndex));
            }
            if (selectLevels.Count > 0)
            {
                int tem = Random.Range(0, selectLevels.Count);
                curIndex = selectLevels[tem];
            }

            RandomArr(faceLevels);

            for (int i = 0; i < paoPaoSpines.childCount; i++)
            {
                if (i != (paoPaoSpines.childCount - 1))
                {
                    SpineManager.instance.DoAnimation(paoPaoSpines.GetChild(i).gameObject, curIndex + "-" + faceLevels[i] + "2", true);
                    paoPaoSpines.GetChild(i).name = curIndex + "-" + faceLevels[i];
                }
                else
                {
                    SpineManager.instance.DoAnimation(paoPaoSpines.GetChild(i).gameObject, curIndex + "-shua", true);
                }

            }

            isPlaying = false;
        }

        private void GameInit()
        {
            talkIndex = 1;

            if (selectLevels == null || selectLevels.Count < 5)
            {
                selectLevels = new List<int>() { 1, 2, 3, 4, 5 };
            }

            for (int i = 0; i < scoreCard.childCount; i++)
            {
                scoreCard.GetChild(i).GetImage().sprite = bellTextures.sprites[0];
                SpineManager.instance.DoAnimation(scoreCard.GetChild(i).GetChild(0).gameObject, "kong", false);
            }
            RandomLevel();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, "jiangli-", false,
                () =>
                {
                    mono.StartCoroutine(CtrlAnimTimeLen(ac));
                });
        }

        IEnumerator CtrlAnimTimeLen(Action ac) {
            float temNum = 0;
            temNum = SpineManager.instance.DoAnimation(successSpine, "jiangli", false);
            yield return new WaitForSeconds(temNum - 2f);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(1).gameObject.SetActive(true);
            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
        }
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
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), null, () => { isPlaying = false; }));
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }


    }
}
