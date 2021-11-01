using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course227Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;

        private GameObject door;
        private GameObject Panel;
        private GameObject bell;


        private GameObject btnMask;

        private int flag = 0;
        private bool isPlay = false;
        private bool isEnd = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();


            btnMask = curTrans.Find("btnMask").gameObject;
            btnMask.SetActive(false);
            door = curTrans.Find("door").gameObject;
            door.SetActive(true);
            Panel = curTrans.Find("Panel").gameObject;
            Panel.SetActive(false);
            for (int i = 0; i < Panel.transform.childCount; i++)
            {
                GameObject go = Panel.transform.GetChild(i).gameObject;
                Util.AddBtnClick(go, onClickBtn);
            }
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);


            GameInit();
            GameStart();
        }

        private void onClickBtn(GameObject obj)
        {
            if (isPlay)
                return;
            isPlay = true;
            SoundManager.instance.ShowVoiceBtn(false);
            if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
            {
                flag += 1 << obj.transform.GetSiblingIndex();
            }
            btnMask.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, int.Parse(obj.name), () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, int.Parse(obj.name), false); SpineManager.instance.DoAnimation(obj, obj.name, false);
            }, () =>
            {
                btnMask.SetActive(false);
                isPlay = false;
                if (flag == (Mathf.Pow(2, Panel.transform.childCount) - 1) && !isEnd)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            }));

        }

        void GameInit()
        {
            flag = 0;
            isEnd = false;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            isPlay = true;
            btnMask.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false); SpineManager.instance.DoAnimation(door, "men", false); }, () =>
            {
                isPlay = false; SoundManager.instance.ShowVoiceBtn(true);
                btnMask.SetActive(false);
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
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bell;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                door.SetActive(false);
                Panel.SetActive(true);
            }
            else
            {
                isEnd = true;
                isPlay = true;
                btnMask.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 5, null, () => { btnMask.SetActive(false); isPlay = false; }));
            }
            talkIndex++;
        }
    }
}
