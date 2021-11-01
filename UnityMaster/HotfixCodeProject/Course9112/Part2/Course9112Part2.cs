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
    public class Course9112Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private GameObject _ani;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            _ani = curTrans.GetGameObject("Ani");

#if !UNITY_EDITOR
            btnTest.SetActive(false);
#endif
            btnTest.SetActive(false);
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            _ani.Show();
            SpineManager.instance.DoAnimation(_ani, "a", true);
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();

        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 5, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            //“爷爷新发明了一款具有急停功能新型的核反应控制装置，它是怎样工作的，一起来看看”
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
            ()=>
            {
                SpineManager.instance.DoAnimation(_ani, "a2");
                mono.StartCoroutine(WaitCoroutine(
                ()=>
                {
                    //贝尔说不好
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_ani, "a3", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_ani, "a4", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_ani, "a5", false,
                                () =>
                                {
                                    SoundManager.instance.ShowVoiceBtn(true);
                                });
                            });
                        });
                    }, null, 0));
                }
                , 1.8f));
            }, 0));
        }

        //自定义协程
        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }

            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                //接下来请根据核反应控制装置的工作方式，将感受质量按钮的程序，急停按钮的程序以及手动复位的程序的流程图分别写出来吧。
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null, null, 0));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }
    }
}
