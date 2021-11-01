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
    public class TD6711Part2
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


        //private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        //private GameObject buDing;
        //private Text bdText;
        //private GameObject devil;
        //private Text devilText;

        //private Transform bdStartPos;
        //private Transform bdEndPos;
        //private Transform devilStartPos;
        //private Transform devilEndPos;
      
        //胜利动画名字
        private string tz;
        private string sz;

        private bool isPlaying;
        private Transform _level1;
        private GameObject _ani;
        private bool[] _clickAll;
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

            //buDing = curTrans.Find("mask/buDing").gameObject;
            //bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            //bdStartPos = curTrans.Find("mask/bdStartPos");
            //buDing.transform.position = bdStartPos.position;
            //bdEndPos = curTrans.Find("mask/bdEndPos");

            //devil = curTrans.Find("mask/devil").gameObject;
            //devilText = devil.transform.GetChild(0).GetComponent<Text>();
            //devilStartPos = curTrans.Find("mask/devilStartPos");
            //devil.transform.position = devilStartPos.position;
            //devilEndPos = curTrans.Find("mask/devilEndPos");

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

            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _level1 = curTrans.Find("Level1");
            _ani = _level1.GetGameObject("Ani");
            for (int i = 0; i < _level1.childCount - 1; i++)
            {
                Util.AddBtnClick(_level1.GetChild(i).gameObject, ClickLevel1);
            }
            _clickAll = new bool[12];
            for (int i = 0; i < _clickAll.Length; i++)
            {
                _clickAll[i] = false;
            }
            GameInit();
            GameStart();
        }

        private void ClickLevel1(GameObject obj)
        {
            if(!isPlaying)
            {
                isPlaying = true;
                _clickAll[Convert.ToInt32(obj.name) - 1] = true;
                SpineManager.instance.DoAnimation(_ani, obj.name, false);
                int index = Convert.ToInt32(obj.name);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, index, null, () => { isPlaying = false; CheckAll(); }));
            }
        }

        void CheckAll()
        {
            bool isClickAll = true;
            for (int i = 0; i < _clickAll.Length; i++)
            {
                if(_clickAll[i] == false)
                {
                    isClickAll = false;
                    return;
                }
            }

            if (isClickAll)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
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
                else if(obj.name == "fh")
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
            isPlaying = true;
        }

        void GameStart()
        {
            //buDing.transform.DOMove(bdEndPos.position,1f).OnComplete(()=> {/*正义的一方对话结束 devil开始动画*/

            //    devil.transform.DOMove(devilEndPos.position, 1f).OnComplete(() => {/*对话*/ });
            //});

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, 
            () => 
            {
                bd.SetActive(false);
                mask.SetActive(false);
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
                mask.SetActive(true);
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 13, null, null, 0));
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
                {  /* anyBtn.name = getBtnName(BtnEnum.fh);*/
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
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

        //void ShowDialogue(string str, Text text, Action callBack = null)
        //{
        //    mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        //}

        //IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        //{
        //    int i = 0;
        //    str = str.Replace(" ", "\u00A0");  //空格非换行
        //    while (i <= str.Length - 1)
        //    {
        //        yield return new WaitForSeconds(textSpeed);
        //        text.text += str[i];
        //        i++;
        //    }
        //    callBack?.Invoke();
        //    yield break;
        //}

    }
}
