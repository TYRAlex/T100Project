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
    public class TD8914Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject dbd;
        private GameObject caidaiSpine;


        private GameObject mask;

 
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);

            GameInit();
            GameStart();
        }


     
        private void GameInit()
        {
            talkIndex = 1;
        }

        void GameStart()
        {
            SoundManager.instance.StopAllCoroutines();
            mono.StopAllCoroutines();
            caidaiSpine.SetActive(true);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            mask.Show();
            dbd.Show();
           
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0));
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

            SpineManager.instance.DoAnimation(dbd, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(dbd, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(dbd, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

       
      
    }
}
