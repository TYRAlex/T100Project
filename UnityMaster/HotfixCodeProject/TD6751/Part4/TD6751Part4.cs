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
    public class TD6751Part4
    {
        GameObject dbd;
        GameObject curGo;
        GameObject caidaiSpine;

        Transform curTrans;

        MonoBehaviour mono;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;

            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();

            SoundManager.instance.StopAudio();
            SoundManager.instance.ShowVoiceBtn(false);

            Input.multiTouchEnabled = false;


            //加载彩带
            caidaiSpine = curTrans.Find("caidaiSpine").gameObject;

            //加载人物         
            dbd = curTrans.Find("DBD").gameObject;
            dbd.SetActive(true);

            GameStart();
        }

        void GameStart()
        {
            //BGM
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);

            mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 29, () =>
            {
                SpineManager.instance.DoAnimation(caidaiSpine, "kong", false, () =>
                {
                    SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
                });
            }));
        }

        //协程:播放说话语音
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker) speaker = dbd;
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
    }
}
