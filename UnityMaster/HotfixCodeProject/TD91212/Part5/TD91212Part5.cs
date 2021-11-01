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
    public class TD91212Part5
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
            dbd = curTrans.Find("DBD").gameObject;
            _caidai = curTrans.GetGameObject("caidaiSpine");
            _caidai.Show();
            SpineManager.instance.DoAnimation(_caidai, "kong", false,
                () => SpineManager.instance.DoAnimation(_caidai, "sp", false));
           
            dbd.SetActive(true);
            SoundManager.instance.ShowVoiceBtn(false);
           
            GameStart();
        }

        void GameStart()
        { 
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
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
