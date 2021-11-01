using ILFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CourseLeapOfSalmanPart1
{
    class SalmonPart1_Scene4
    {
        GameObject bg4Spine, salmon4Spine0, salmon4Spine1, points4, bubbleSpine, npc;
        MonoBehaviour mono;

        public SalmonPart1_Scene4(GameObject curGo)
        {
            Transform curTrans = curGo.transform;

            salmon4Spine0 = curTrans.Find("gameScene/Level_4/salmonSpine").gameObject;
            salmon4Spine1 = curTrans.Find("gameScene/Level_4/salmon1Spine").gameObject;
            bg4Spine = curTrans.Find("gameScene/Level_4/background").gameObject;
            points4 = curTrans.Find("gameScene/Level_4/points").gameObject;
            bubbleSpine = curTrans.Find("gameScene/Level_4/bubbleSpine").gameObject;
            npc = curTrans.Find("npc").gameObject;
            mono = curGo.GetComponent<MonoBehaviour>();

            Init();            
        }

        void Init()
        {
            salmon4Spine0.SetActive(true );
            salmon4Spine1.SetActive(false);

            SceneInit();
        }

        void SceneInit()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);
            SoundManager.instance.soundSource.volume = 1f;
            SpineManager.instance.DoAnimation(bg4Spine, "bg", true);
            //SpineManager.instance.SetTimeScale(bubbleSpine, 5f);
            SpineManager.instance.DoAnimation(bubbleSpine, "animation2", true);
            SpineManager.instance.PlayAnimationDuring(salmon4Spine0, "animation", "0|9");
            mono.StartCoroutine(WaitTime(9f));
            //mono.StartCoroutine(Salmon4Move());
        }

        IEnumerator WaitTime(float time)
        {
            yield return new WaitForSeconds(time);
            salmon4Spine0.SetActive(false);
            salmon4Spine1.SetActive(true);
            SpineManager.instance.DoAnimation(salmon4Spine1, "swim", true);
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 5);

            mono.StopCoroutine(WaitTime(time));
        }
    }
}
