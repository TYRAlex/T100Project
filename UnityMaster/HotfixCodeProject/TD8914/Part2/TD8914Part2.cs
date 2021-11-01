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

    public class TD8914Part2
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

        private GameObject _aniMask;
        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;


        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

        //-----------------------------------------------------------------------------------------
        private GameObject _parent;
        private GameObject _ani;

        private List<string> _nameList;
         private List<GameObject> _cilckList;
        //-----------------------------------------------------------------------------------------

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
            mask.SetActive(false);

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
            _aniMask = curTrans.GetGameObject("animask");
            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            //--------------------------------------------------------------------
            _parent = curTrans.GetGameObject("parent");
            _ani = _parent.transform.GetGameObject("0");
            _cilckList = new List<GameObject>();
            _nameList=new List<string>();
            for (int i = 0; i < _parent.transform.childCount; i++)
            {
                var child = _parent.transform.GetChild(i).gameObject;
                if (i > 0)
                {
                    _nameList.Add(child.name);
                    Util.AddBtnClick(child, OnClickShow);
                    _cilckList.Add(child);
                }

            }
            //--------------------------------------------------------------------

            GameInit();
            GameStart();
            //IsStart();
        }

        void IsStart()
        {
            mask.Show();
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
        }




        void RemoveName(GameObject gameObject)
        {
            for (int i = 0; i < _nameList.Count; i++)
            {
                if (gameObject.name == _nameList[i])
                {
                    _nameList.RemoveAt(i);
                }
                
            }
        }
        

        private void OnClickShow(GameObject obj)
        {
            //Debug.Log("DoAni");
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            //播放对应语音
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND,1);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, bd, GetVoiceIndex(obj), () => {_aniMask.Show(); },
                () => { _aniMask.Hide(); }));
            SpineManager.instance.DoAnimation(_ani, obj.name, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_ani, _ani.name, false,
                        () =>
                        {
                            RemoveName(obj);
                            if (_nameList.Count == 0)
                            {
                                SoundManager.instance.ShowVoiceBtn(true);   
                            }
                        });
                });
        }

        int GetVoiceIndex(GameObject go)
        {
            int index = -1;
            for (int i = 0; i < _cilckList.Count; i++)
            {
                var temp = _cilckList[i];
                if (go == temp)
                {
                    index = i;
                }
            }

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
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, dbd, 0));
                    });
                }
            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            _aniMask.Hide();
            SoundManager.instance.StopAllCoroutines();
            mono.StopAllCoroutines();
            SpineManager.instance.DoAnimation(_ani, _ani.name, false,
                       () => { });
            isPressBtn = false;
        }

        void GameStart()
        {
            mask.Show();
            anyBtns.gameObject.Hide();
            bd.Show();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd,0 , null, () =>
            {
                 SoundManager.instance.ShowVoiceBtn(true);
            }));
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
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, GameObject bd, int clipIndex, Action method_1 = null,
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
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 1, null, () =>
                 {
                     mask.Hide();
                     bd.Hide();
                 }));
            }

            if (talkIndex == 2)
            {
                mask.Show();
                bd.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 2, null, () =>
                {
                   
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
    }
}