using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course227Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
        private GameObject cl;
        private GameObject panel;
        private GameObject fxp;
        private GameObject bsz;
        private GameObject fxpShow;
        private GameObject lxg;
        private GameObject bsl;
        private GameObject bsr;
        private GameObject bsl2;
        private GameObject bsr2;
        private GameObject xg;
        private GameObject sz;

        private GameObject btnFxp;
        private GameObject btnBsz;
        private GameObject fxpText;
        private GameObject bszText;
        private GameObject btnBack;

        private bool isPlay = false;

        private bool isFxp = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            cl = curTrans.Find("cl").gameObject;
            cl.SetActive(true);
            panel = curTrans.Find("panel").gameObject;
            fxp = curTrans.Find("panel/fxp").gameObject;
            bsz = curTrans.Find("panel/bsz").gameObject;
            fxpShow = curTrans.Find("panel/fxpShow").gameObject;
            fxpShow.SetActive(true);
            lxg = curTrans.Find("panel/lxg").gameObject;
            lxg.SetActive(true);
            bsl = curTrans.Find("panel/bsl").gameObject;
            bsl.SetActive(true);
            bsr = curTrans.Find("panel/bsr").gameObject;
            bsr.SetActive(true);

            bsl2 = curTrans.Find("panel/bsl2").gameObject;
            bsl2.SetActive(true);
            bsr2 = curTrans.Find("panel/bsr2").gameObject;
            bsr2.SetActive(true);

            sz = curTrans.Find("panel/sz").gameObject;
            sz.SetActive(true);
            xg = curTrans.Find("panel/xg").gameObject;
            xg.SetActive(true);
            btnFxp = curTrans.Find("panel/btnFxp").gameObject;
            btnFxp.SetActive(true);
            btnBsz = curTrans.Find("panel/btnBsz").gameObject;
            btnBsz.SetActive(true);
            fxpText = curTrans.Find("panel/fxpText").gameObject;
            bszText = curTrans.Find("panel/bszText").gameObject;

            btnBack = curTrans.Find("panel/btnBack").gameObject;
            panel.SetActive(false);
            btnBack.SetActive(false);
            Util.AddBtnClick(btnFxp, onClickFxp);
            Util.AddBtnClick(btnBsz, onClickBsz);
            Util.AddBtnClick(btnBack, onClickBtnBack);
            GameInit();
            GameStart();
        }
        private IEnumerator playSpineShow()
        {
            float playDur = 0;
            for (int i = 1; i < 9; i++)
            {
                yield return new WaitForSeconds(playDur);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                playDur = SpineManager.instance.DoAnimation(fxpShow, "gb" + i, false);
            }
            yield return new WaitForSeconds(playDur);
            SpineManager.instance.DoAnimation(fxp, "tua3", false,
             () =>
             {
                 isPlay = false;
             });
        }
        private void onClickBtnBack(GameObject obj)
        {
            if (isPlay)
                return;
            isPlay = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            if (isFxp)
            {
                SpineManager.instance.DoAnimation(fxp, "tua6", false, () => { reSetPanel(); });
            }
            else
            {
                SpineManager.instance.DoAnimation(bsz, "tub1", false, () => { reSetPanel(); });
            }

        }

        private void reSetPanel()
        {
            SpineManager.instance.DoAnimation(fxp, "tua1", false); 
            SpineManager.instance.DoAnimation(bsz, "tub1", false);
            fxpText.SetActive(true);
            bszText.SetActive(true);
            btnFxp.SetActive(true);
            btnBsz.SetActive(true);
            btnBack.SetActive(false);
            isPlay = false;
        }

        private void onClickFxp(GameObject obj)
        {
            if (isPlay)
                return;
            isPlay = true;
            isFxp = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            SpineManager.instance.DoAnimation(bsz, "jing", false);
            fxpText.SetActive(false);
            bszText.SetActive(false);
            btnFxp.SetActive(false);
            btnBsz.SetActive(false);

            // SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            SpineManager.instance.DoAnimation(fxp, "tua2", false,
                () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, () =>
                    {
                        mono.StartCoroutine(playSpineShow());
                    }, () => { btnBack.SetActive(true); }));
                });

        }
        private void onClickBsz(GameObject obj)
        {
            if (isPlay)
                return;
            isPlay = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            isFxp = false;
            SpineManager.instance.DoAnimation(fxp,"jing",false);   
            fxpText.SetActive(false);
            bszText.SetActive(false);
            btnFxp.SetActive(false);
            btnBsz.SetActive(false);

            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            SpineManager.instance.DoAnimation(bsz, "tub2", false,
               () =>
               {
                   mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4, () =>
                   {
                       SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                       SpineManager.instance.DoAnimation(lxg, "g4", false,
                       () =>
                       {
                           SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                           SpineManager.instance.DoAnimation(bsl, "g1", false,
                       () =>
                       {
                           SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                           SpineManager.instance.DoAnimation(bsr, "g2", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(bsz, "tub3", false,
                      () =>
                      {
                          SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                          SpineManager.instance.DoAnimation(bsl2, "g1", false,
                      () =>
                      {
                          SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                          SpineManager.instance.DoAnimation(bsr2, "g2", false,
                      () =>
                      {
                          SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                          SpineManager.instance.DoAnimation(xg, "g6", false,
                      () =>
                      {
                          SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                          SpineManager.instance.DoAnimation(sz, "g5", false,
                      () =>
                      {
                          isPlay = false;
                      });
                      });
                      });
                      });
                      });
                    });
                       });
                       });
                   }, () => { btnBack.SetActive(true); }));
               });

        }

        void GameInit()
        {
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            isFxp = false;
            fxp.GetComponent<SkeletonGraphic>().Initialize(true);
            fxp.SetActive(true);
            SpineManager.instance.DoAnimation(fxp, "jing", false, () =>
            {
                fxpText.SetActive(true);
                SpineManager.instance.DoAnimation(fxp, "tua1", false);
            });
            bsz.GetComponent<SkeletonGraphic>().Initialize(true);
            bsz.SetActive(true);
            SpineManager.instance.DoAnimation(bsz, "jing", false, () =>
            {
                bszText.SetActive(true); 
                SpineManager.instance.DoAnimation(bsz, "tub1", false);
            });
            SpineManager.instance.DoAnimation(fxpShow, "jing", false);
            SpineManager.instance.DoAnimation(lxg, "jing", false);
            SpineManager.instance.DoAnimation(bsl, "jing", false);
            SpineManager.instance.DoAnimation(bsr, "jing", false);
            SpineManager.instance.DoAnimation(sz, "jing", false);
            SpineManager.instance.DoAnimation(bsl2, "jing", false);
            SpineManager.instance.DoAnimation(bsr2, "jing", false);
            SpineManager.instance.DoAnimation(xg, "jing", false);
        }

        void GameStart()
        {

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            isPlay = true;
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true); SpineManager.instance.DoAnimation(cl, "4"); }, () => { isPlay = false; SoundManager.instance.ShowVoiceBtn(true); }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                isPlay = true;
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, null, () => { isPlay = false; SoundManager.instance.ShowVoiceBtn(true); }));
            }
            if (talkIndex == 2)
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                cl.SetActive(false);
                panel.SetActive(true);
                isPlay = true;
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, () => { isPlay = false; }));
            }
            talkIndex++;
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

    }
}
