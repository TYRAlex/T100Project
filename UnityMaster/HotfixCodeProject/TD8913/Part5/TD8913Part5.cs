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
    
    public class TD8913Part5
    {

       
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
      
        private GameObject dbd;


        private GameObject _caidai;
       

       

       

       

       
      
       
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            _caidai = curTrans.GetGameObject("caidai");
            dbd = curTrans.Find("DBD").gameObject;
            dbd.SetActive(true);
            

            GameInit();
            GameStart();
        }


      

        
       
       


       

        

        private void GameInit()
        {
            // talkIndex = 1;
            // curPageIndex = 0;
            // isPressBtn = false;
            // textSpeed =0.1f;
            // SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            // for (int i = 0; i < SpinePage.childCount; i++)
            // {
            //     SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            // }

           
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0));
            SpineManager.instance.DoAnimation(_caidai, "sp", false);
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
