using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course931Part1
    {

        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject amy;
        private GameObject eye;
        private GameObject eyeTexts;
        private GameObject amyTexts;

        private GameObject clickBtn;
        private Empty4Raycast clickMask;
        private Empty4Raycast[] clickBtns;
        private string clickBtnName;
        private int flag;
        private int talkIndex;
        private int voiceBtn;

        private GameObject _mask;

        private GameObject _bg0;
        private GameObject _bg1;
        private GameObject _bg2;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopCoroutine("WaitCoroutine");
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            SoundManager.instance.ShowVoiceBtn(false);

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }
        private void GameInit()
        {
            talkIndex = 1;
            flag = 0;
            voiceBtn = 0;

            _mask = curTrans.Find("mask").gameObject;
            _mask.Hide();


            amy = curTrans.Find("spines/amy").gameObject;
            amy.SetActive(false);

            eye = curTrans.Find("spines/eye").gameObject;
            eye.SetActive(false);

            eyeTexts = curTrans.Find("spines/eyeTexts").gameObject;
            eyeTexts.SetActive(false);

            amyTexts = curTrans.Find("spines/amyTexts").gameObject;
            //SpineManager.instance.DoAnimation(amyTexts, "kong", false);
            amyTexts.Hide();

            clickMask = curTrans.Find("clickMask").GetComponent<Empty4Raycast>();
            Util.AddBtnClick(clickMask.gameObject, ClickMaskEvent);
            clickMask.gameObject.SetActive(false);

            clickBtn = curTrans.Find("clickBtn").gameObject;
            clickBtns = clickBtn.GetComponentsInChildren<Empty4Raycast>();
            for (int i = 0; i < clickBtns.Length; i++)
            {
                Util.AddBtnClick(clickBtns[i].gameObject, ClickBtnEvent);
            }
            clickBtn.SetActive(false);


            _bg0 = curTrans.Find("Bg/0").gameObject;
            _bg1 = curTrans.Find("Bg/1").gameObject;
            _bg2 = curTrans.Find("Bg/2").gameObject;
            _bg0.Hide();
            _bg1.Hide();
            _bg2.Hide();
        }
        void GameStart()
        {

            Max.SetActive(true);
            isPlaying = true;
            PlayEyeTextAni("animation4", () =>
            {
                SpineManager.instance.DoAnimation(eyeTexts, "animation3", false);
            });
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                isPlaying = false;
                SoundManager.instance.ShowVoiceBtn(true);
            }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
        }
        /// <summary>
        /// 播放EyeTexts动画(animation,animation2,animation3,animation4,h1,h2,h3)
        /// </summary>
        /// <param name="aniName"></param>
        private void PlayEyeTextAni(string aniName, Action callBack = null)
        {
            eyeTexts.SetActive(true);
            eyeTexts.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.SetTimeScale(eyeTexts, 1);
            SpineManager.instance.DoAnimation(eyeTexts, aniName, false, callBack);
        }
        private void PlayEyeTextAniH(string aniName, Action callBack = null)
        {
            SpineManager.instance.SetTimeScale(amyTexts, 1);
            SpineManager.instance.DoAnimation(amyTexts, aniName, false, callBack);
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
                speaker = Max;
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

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    ClickVoiceBtnFirst();
                    break;
                case 2:
                    ClickVoiceBtnSceond();
                    break;
            }
            talkIndex++;
        }
        /// <summary>
        /// 第一次点击语音键
        /// </summary>
        private void ClickVoiceBtnFirst()
        {
            eyeTexts.SetActive(true);
            eyeTexts.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(eyeTexts, "animation2", false);
            mono.StartCoroutine(WaitCoroutine(
            () =>
            {
                SpineManager.instance.SetTimeScale(eyeTexts, 0.3f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 2f));
                mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 4.5f));
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    SpineManager.instance.SetTimeScale(eyeTexts, 1);                                     

                    clickBtn.SetActive(true);
                }));
            }, 1.0f));
        }
        /// <summary>
        /// 第二次点击语音键
        /// </summary>
        private void ClickVoiceBtnSceond()
        {
            Max.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 8, () =>
             {
                 clickBtn.SetActive(false);
             }));
        }
        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        #region 拖拽
        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }
        #endregion
                
        /// <summary>
        /// clickBtn点击事件
        /// </summary>
        private void ClickBtnEvent(GameObject obj)
        {
            _mask.Show();

            SoundManager.instance.ShowVoiceBtn(false);
            if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
            {
                flag += 1 << obj.transform.GetSiblingIndex();
            }
            if (flag == (Mathf.Pow(2, clickBtn.transform.childCount) - 1))
            {
                voiceBtn = 1;
            }
            clickBtn.SetActive(false);            
            switch (obj.name)
            {
                case "1":
                    clickBtnName = "1";
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);                  
                    mono.StartCoroutine(OnClickBtnText(_bg0, "d1", 2, "a1", "a2", "a3", "u1", "u11", "u12", "u13", 4, 4, 4, 0.7f, 0.7f, 2, 2.5f));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false); }, 6));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); }, 10));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 11));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 14.5f));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 17));
                    


                    break;
                case "2":
                    clickBtnName = "2";
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);                   
                    
                    mono.StartCoroutine(OnClickBtnText(_bg1, "d2", 6, "b1", "b2", "b3", "u2", "u21", "u22", "u23", 3, 3, 3, 0.7f, 0.7f, 1, 2));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false); }, 6));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); }, 13));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 14));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 16));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 18));

                    break;
                case "3":
                    clickBtnName = "3";
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);                   
                    
                    mono.StartCoroutine(OnClickBtnText(_bg2, "d3", 7, "c1", "c2", "c3", "u3", "u31", "u32", "u33", 5, 5, 5, 0.4f, 1, 1.5f, 3.5f));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false); }, 9));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); }, 15));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false); }, 11));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 17));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 20.5f));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 23));

                    break;
            }
        }

        IEnumerator PlaySound(Tuple<int, float>[] tuples)
        {
            foreach (var item in tuples)
            {
                if (item ==null|| item.Item1 <= 0)
                {
                    continue;
                }
                yield return new WaitForSeconds(item.Item2);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, item.Item1, false);
            }
        }
        IEnumerator OnClickBtnText(GameObject go, string AnimName, int index, string aniName1, string aniName2, string aniName3, string aniName4, string aniName5, string aniName6, string aniName7, int index_1, int index_2, int index_3, float time, float time_0, float time_1, float time_2)
        {
            float temTime = 0;
            eyeTexts.Show();            
            temTime = SpineManager.instance.DoAnimation(eyeTexts, AnimName, false);
            yield return new WaitForSeconds(temTime);
            go.Show();
            if (go.transform.childCount>0)
            {
                go.transform.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);

               SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, go.transform.GetChild(0).name + 1, false,()=> 
               {
                   SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, go.transform.GetChild(0).name + 2, false, () =>
                   {
                       SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, go.transform.GetChild(0).name + 3, false);
                   });
               });
            }
                   
            ClickBtnText(go, index, aniName1, aniName2, aniName3, aniName4, aniName5, aniName6, aniName7, index_1, index_2, index_3, time, time_0, time_1, time_2);
        }
        private void ClickBtnText(GameObject go, int index, string aniName1, string aniName2, string aniName3, string aniName4, string aniName5, string aniName6, string aniName7, int index_1, int index_2, int index_3, float time, float time_0, float time_1, float time_2)
        {
            //eyeTexts.SetActive(false);
            SpineManager.instance.DoAnimation(eyeTexts, "kong", false, () =>
            {

            });
            Max.SetActive(false);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index, () =>
            {
                amy.SetActive(true);
                amy.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.SetTimeScale(amy, time);
                SpineManager.instance.DoAnimation(amy, aniName1, false, () =>
                {
                    SpineManager.instance.DoAnimation(amy, aniName2, false, () =>
                    {
                        SpineManager.instance.SetTimeScale(amy, time_0);
                        SpineManager.instance.DoAnimation(amy, aniName3, false, () =>
                        {
                            go.Hide();
                            SpineManager.instance.SetTimeScale(amy, 1);
                            amy.SetActive(false);
                            SpineManager.instance.DoAnimation(amy, "kong", false);
                            PlayAmyTextAni(aniName4, aniName5, aniName6, aniName7, index_1, index_2, index_3, time_1, time_2);
                        });
                    });
                });
            }));
        }
        private void PlayAmyTextAni(string aniName4, string aniName5, string aniName6, string aniName7, int index_1, int index_2, int index_3, float time, float time_1)
        {
            amyTexts.Show();
            amyTexts.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index_1, null, () =>
            {
                clickMask.gameObject.SetActive(true);
            }));
            SpineManager.instance.DoAnimation(amyTexts, "kong", false, () =>
            {
                SpineManager.instance.DoAnimation(amyTexts, aniName4, false, () =>
                 {
                     SpineManager.instance.DoAnimation(amyTexts, aniName5, false, () =>
                     {
                         mono.StartCoroutine(WaitCoroutine(() =>
                         {
                             SpineManager.instance.DoAnimation(amyTexts, aniName6, false, () =>
                             {
                                 mono.StartCoroutine(WaitCoroutine(() =>
                                 {
                                     SpineManager.instance.DoAnimation(amyTexts, aniName7, false);
                                     _mask.Hide();
                                 }, time));
                             });
                         }, time_1));
                     });
                 });
            });
        }
        private void ClickMaskEvent(GameObject obj)
        {
            switch (clickBtnName)
            {
                case "1":
                    ClickMaskBtn("h1");
                    break;
                case "2":
                    ClickMaskBtn("h2");
                    break;
                case "3":
                    ClickMaskBtn("h3");
                    break;
            }
        }
        private void ClickMaskBtn(string aniName)
        {
            //Debug.LogError("-----------ClickMaskBtn");
            SoundManager.instance.ShowVoiceBtn(false);
            clickMask.gameObject.SetActive(false);
            //eyeTexts.SetActive(true);
            //amyTexts.Hide();
            PlayEyeTextAniH(aniName, () =>
            {
                clickBtn.SetActive(true);
                clickMask.gameObject.SetActive(false);

                //eyeTexts.SetActive(true);
                //SpineManager.instance.DoAnimation(eyeTexts, "kong", false, () =>
                //{
                //    SpineManager.instance.DoAnimation(eyeTexts, "animation", false, () => { amyTexts.Hide(); });
                //});
                SpineManager.instance.DoAnimation(eyeTexts, "animation", false, () => { amyTexts.Hide(); });
                switch (voiceBtn)
                {
                    case 1:
                        SoundManager.instance.ShowVoiceBtn(true);
                        break;
                }
            });
        }
    }
}
