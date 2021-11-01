using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course70002Part2
    {
        private GameObject nv;
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            nv = curTrans.Find("nv").gameObject;

            mono = curGo.GetComponent<MonoBehaviour>();
            GameStart();
        }

        void GameInit()
        {
            talkIndex = 1;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
        }

        void GameStart()
        {
            GameInit();
            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 1, () => {
                SpineManager.instance.DoAnimation(nv, "animation2", true);
            }, () => {
                SpineManager.instance.DoAnimation(nv, "animation", true);
            });
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }
            talkIndex++;
        }
    }
}
