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
    class SalmonPart1_Scene0
    {
        //level 0
        
        GameObject salmon0Spine, npcSalmonSpine0, bubbleSpine0, npc;

        public bool isEnd;
        public AudioSource bgmSource;
        Vector3 npcSalmonSpine0Pos, salmon0SpinePos;
        MonoBehaviour mono;

        public SalmonPart1_Scene0(GameObject curGo)
        {
            Transform curTrans = curGo.transform;

            //level 0            
            npcSalmonSpine0 = curTrans.Find("gameScene/Level_0/npcSalmonSpine").gameObject;
            salmon0Spine = curTrans.Find("gameScene/Level_0/salmonSpine").gameObject;
            bubbleSpine0 = curTrans.Find("gameScene/Level_0/background/bubble").gameObject;
            bgmSource = curTrans.Find("bgmSource").GetComponent<AudioSource>();
            npc = curTrans.Find("npc").gameObject;
            mono = curGo.GetComponent<MonoBehaviour>();
            isEnd = false;
            Init();            
        }

        void Init()
        {
            SoundManager.instance.voiceBtn.SetActive(false);
            npcSalmonSpine0.transform.localPosition = new Vector3(-23f, -546f, 0);
            salmon0Spine.transform.localPosition = new Vector3(-23f, -546f, 0);
            SceneInit();
        }

        void SceneInit()
        {
            SpineManager.instance.DoAnimation(bubbleSpine0, "animation", true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, true);
            SoundManager.instance.bgmSource.volume = 0.2f;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, true);
            SoundManager.instance.soundSource.volume = 0.7f;
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 0, () => {
                SoundManager.instance.skipBtn.SetActive(false); 
            }, () =>
            {
                npc.SetActive(true);
            });
            SpineManager.instance.DoAnimation(npcSalmonSpine0, "swim_2", false, () =>
            {
                npc.SetActive(true);
            });
            mono.StartCoroutine(WaitTime(10f));
        }

        IEnumerator WaitTime(float time)
        {
            yield return new WaitForSeconds(time);
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 1);
            SpineManager.instance.DoAnimation(salmon0Spine, "swim_1", false, () =>
            {
                isEnd = true;
            });                 

            mono.StopCoroutine(WaitTime(time));
        }
    }
}
