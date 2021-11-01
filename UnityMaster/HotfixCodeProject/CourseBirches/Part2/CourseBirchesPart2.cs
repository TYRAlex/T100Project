using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseBirchesPart2
    {
        GameObject curGo, npc;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            curGo.AddComponent<PowerpointManager>();
            npc = curGo.transform.Find("npc").gameObject;

            PowerpointManager.instance.CreatePower(curGo, new string[] { "tamp_one", "tamp_one (1)", "tamp_one (2)", "tamp_three", "tamp_two", "tamp_two (1)" }, Speak);
            SoundManager.instance.BgSoundPart1();
        }

        public void Speak()
        {
            int index = PowerpointManager.instance.curIndex;
            if (index < 3)
            {
                SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, index,
                () => PowerpointManager.instance.drag.GetComponent<BoxCollider>().enabled = false,
                () => PowerpointManager.instance.drag.GetComponent<BoxCollider>().enabled = true);
            }
            
        }

        //void Update()
        //{
        //    if (isActive)
        //    {
        //        isActive = false;
        //        for (int i = 0; i < ppts.Length; i++)
        //        {
        //            if (ppts[i].activeSelf == true)
        //            {
        //                Debug.Log("ppts[i]:" + i);
        //                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
        //                SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, i);
        //            }
        //        }
        //    }            
        //}
    }
}
