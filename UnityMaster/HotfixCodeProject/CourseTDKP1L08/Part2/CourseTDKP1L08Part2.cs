using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDKP1L08Part2
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
            Npc = curTrans.Find("tiantian").gameObject;
            Shiled = curTrans.Find("Shield").gameObject;
            isPlaying = false;
            if (curGo != null)
            {
                Debug.Log(111111);
            }

            curGo.AddComponent<PowerpointManager>();
            PowerpointManager.instance.CreatePower(curGo, new string[] { "1", "2", "3", "4", "5" });
            lastIndex = -1;
        }

        void Update()
        {
            // Debug.Log("lastIndex: " + lastIndex);
            // Debug.Log("curIndex: " + PowerpointManager.instance.curIndex);
            if (!isPlaying)
            {
                if (PowerpointManager.instance.curIndex < 5 && lastIndex != PowerpointManager.instance.curIndex)
                {

                    SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, PowerpointManager.instance.curIndex, () =>
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
