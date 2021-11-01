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

    public class TD91211Part3
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;

        private GameObject mask;

        private GameObject btnBack;

        //---------------------------------------
        private GameObject _parent;
        private GameObject c;
        GameObject _aniMask;
        //---------------------------------------
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();


            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(false);


            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);


            //------------------------------------------------------------------
            _parent = curTrans.GetGameObject("parent");
            c = _parent.transform.GetChild(0).gameObject;
            Util.AddBtnClick(c.transform.GetChild(0).gameObject, OnClickShow);
            _aniMask = _parent.transform.GetGameObject("animask");
            //------------------------------------------------------------------

            btnBack = _parent.transform.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();

        }




        private void OnClickBtnBack(GameObject obj)
        {
            _aniMask.Show();
            btnBack.Hide();
            SpineManager.instance.DoAnimation(c, "c2", false, () =>
            {
                _aniMask.Hide();
                SoundManager.instance.ShowVoiceBtn(true);
            });

        }

        private void OnClickShow(GameObject obj)
        {
            _aniMask.Show();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 1, null,
               () =>
               {
                   btnBack.Show();
                   _aniMask.Hide();
               }));
            SpineManager.instance.DoAnimation(c, "c", false, () =>
            {
               
            });
        }



        private void GameInit()
        {
            _aniMask.Hide();
            talkIndex = 1;
            SpineManager.instance.DoAnimation(c, "c3", true);
        }

        void GameStart()
        {
            mask.Show();
            bd.Show();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 0, null,
                () =>
                {
                    mask.SetActive(false);
                    bd.SetActive(false);
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
                mask.Show();
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 2, null, () =>
                 {

                 }));
            }

            talkIndex++;
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