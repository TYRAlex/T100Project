using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDG4P1L11Part2
    {
        GameObject curGo;
        GameObject Npc;
        GameObject Shiled;

        public bool isPlaying;
        int lastIndex = 0;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Npc = curTrans.Find("tamp_Parent/tiantian").gameObject;
            Shiled = curTrans.Find("tamp_Parent/Shield").gameObject;
            isPlaying = false;
            curGo.AddComponent<PowerpointManager>();
            PowerpointManager.instance.CreatePower(curGo, new string[] { "1", "2", "3" });
            lastIndex = -1;
        }


        void Update()
        {
            // Debug.Log("lastIndex: " + lastIndex);
            // Debug.Log("curIndex: " + PowerpointManager.instance.curIndex);
            if (!isPlaying)
            {
                if (PowerpointManager.instance.curIndex < 3 && lastIndex != PowerpointManager.instance.curIndex)
                {

                    // SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, PowerpointManager.instance.curIndex, () =>
                    // {
                    //     isPlaying = true;
                    //     Shiled.SetActive(true);
                    //     curGo.transform.Find("tamp_Parent/Drag").localScale = Vector3.zero;

                    // }, () =>
                    // {
                    //     isPlaying = false;

                    //     Shiled.SetActive(false);
                    //     curGo.transform.Find("tamp_Parent/Drag").localScale = Vector3.one;
                    // });
                    SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, PowerpointManager.instance.curIndex, () =>
                    {
                        isPlaying = true;
                        Shiled.SetActive(true);
                        curGo.transform.Find("tamp_Parent/Drag").localScale = Vector3.zero;
                    }, () =>
                    {
                        isPlaying = false;

                        Shiled.SetActive(false);
                        curGo.transform.Find("tamp_Parent/Drag").localScale = Vector3.one;
                    });

                }
                lastIndex = PowerpointManager.instance.curIndex;

            }
        }

    }

}
