using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDG3P1L10Part6
    {

        GameObject curGo, npc;
        int voiceCount, voiceNum;
        MonoBehaviour mono;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            npc = curTrans.Find("npc").gameObject;
            //curGo.AddComponent<ContactPartManager>();
            //ContactPartManager.instance.PlayAnimationVoice(curGo, 0.3f);
            SoundManager.instance.BgSoundPart2(SoundManager.SoundType.BGM);
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(npc, "talk");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(npc, "breath");

            if (method_2 != null)
            {
                method_2();
            }

            mono.StopCoroutine(SpeckerCoroutine(type, clipIndex));
        }

        private void OnDisable()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(null);
            SoundManager.instance.skipBtn.SetActive(false);
            SoundManager.instance.sheildGo.SetActive(false);
        }
    }
}
