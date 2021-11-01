using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course932Part1
    {
        public enum ClickState
        {
            one,
            two,
            three,
            NULL
        }
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;
        bool isPressBtn = false;
        bool isEnd = false;
        int flag = 0;
        private int voiceBtn;

        private GameObject btnBack;
        private GameObject clickBtn;
        private Empty4Raycast[] e4r;

        private GameObject clickEnterBtn;
        private Empty4Raycast[] e4rEnter;
        private ClickState clickState = ClickState.NULL;


        private GameObject jing;
        private GameObject stru_1;
        private GameObject stru_2;
        private GameObject stru_3;


        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            SoundManager.instance.ShowVoiceBtn(false);

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }
        private void GameInit()
        {
            talkIndex = 1;
            flag = 0;
            voiceBtn = 0;

            btnBack = curTrans.Find("btnBack").gameObject;           

            clickBtn = curTrans.Find("clickBtn").gameObject;
            e4r = clickBtn.GetComponentsInChildren<Empty4Raycast>();
            for (int i = 0; i < e4r.Length; i++)
            {
                Util.AddBtnClick(e4r[i].gameObject, OnClickShow);
            }
            clickBtn.SetActive(false);

            clickEnterBtn = curTrans.Find("clickCenterBtn").gameObject;
            e4rEnter = clickEnterBtn.GetComponentsInChildren<Empty4Raycast>();
            for (int i = 0; i < e4rEnter.Length; i++)
            {
                Util.AddBtnClick(e4rEnter[i].gameObject, ClickEnterEvent);
            }
            clickEnterBtn.Hide();

            jing = curTrans.Find("spines/jing").gameObject;
            jing.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(jing, "jing", false);

            stru_1 = curTrans.Find("spines/1").gameObject;
            stru_2 = curTrans.Find("spines/2").gameObject;
            stru_3 = curTrans.Find("spines/3").gameObject;
            PlayKongAni();
        }
        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            {
                Max.SetActive(false); 
                isPlaying = false;
                clickBtn.SetActive(true);
            }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
        }
        /// <summary>
        /// 播空
        /// </summary>
        private void PlayKongAni()
        {
            SpineManager.instance.DoAnimation(stru_1, "kong", false);
            SpineManager.instance.DoAnimation(stru_2, "kong", false);
            SpineManager.instance.DoAnimation(stru_3, "kong", false);
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

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        IEnumerator WaiteCoroutine(Action method_2 = null, float len = 0)
        {            
            yield return new WaitForSeconds(len);
            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    clickBtn.SetActive(false);
                    Max.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () => 
                    { 
                        clickBtn.SetActive(false); 
                    }));
                    break;
            }           
            //talkIndex++;
        }
        /// <summary>
        /// 点击放大
        /// </summary>
        private void OnClickShow(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            Max.SetActive(false);
            if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
            {
                flag += 1 << obj.transform.GetSiblingIndex();
            }
            if (flag == (Mathf.Pow(2, clickBtn.transform.childCount) - 1))
            {
                //SoundManager.instance.ShowVoiceBtn(true);     
                voiceBtn = 1;
            }
            isPlaying = false;
            clickBtn.SetActive(false);
            switch (obj.name)
            {
                case "1":
                    
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);                   
                    SpineManager.instance.DoAnimation(jing, "dian1", false, () =>
                    {
                        mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); }, 1.5f));
                        mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false); }, 3f));
                        PlayAni(SoundManager.SoundType.VOICE, 3, jing, "a", () => 
                        {
                            mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); }, 1.5f));
                            mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false); }, 3f));
                            SpineManager.instance.DoAnimation(jing, "a2", false, ()=> 
                            {
                                mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); }, 1.5f));
                                mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false); }, 3f));
                                SpineManager.instance.DoAnimation(jing, "a3", false, () =>
                                {
                                    clickEnterBtn.Show();
                                    clickState = ClickState.one;
                                });
                            });                           
                        });                      
                    });
                    break;
                case "2":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    //jing.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(jing, "dian2", false, () =>
                    {
                        mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false); }, 1.5f));
                        mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false); }, 5.5f));
                        mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false); }, 9f));
                        mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false); },12.5f));
                        mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false); }, 16.5f));
                        PlayAni(SoundManager.SoundType.VOICE, 2, jing, "b", () =>
                        {
                            clickEnterBtn.Show();
                            clickState = ClickState.two;
                        });
                    });
                    break;
                case "3":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    //jing.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(jing, "dian3", false, () =>
                    {
                        mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false); }, 1.5f));
                        mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false); }, 5.5f));
                        mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false); }, 9f));
                        PlayAni(SoundManager.SoundType.VOICE, 4, jing, "c", () =>
                        {
                            clickEnterBtn.Show();
                            clickState = ClickState.three;
                        });
                    });
                    break;
            }
        } 
        private void PlayAni(SoundManager.SoundType type,int index,GameObject aniObj,string aniName,Action callBack)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, type, index, () =>
            {
                SpineManager.instance.DoAnimation(aniObj, aniName, false, callBack);
            }));            
        }
       
        /// <summary>
        /// 点击回到原大小
        /// </summary>
        /// <param name="obj"></param>
        private void ClickEnterEvent(GameObject obj)
        {
            SoundManager.instance.ShowVoiceBtn(false);
            clickEnterBtn.Hide();
            switch (clickState)
            {
                case ClickState.one:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(jing, "t1", false, () => 
                    {
                        clickBtn.SetActive(true);
                        clickEnterBtn.Hide();
                        ShowViceBtn();
                    });
                    break;
                case ClickState.two:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(jing, "t2", false, () =>
                    {
                        clickBtn.SetActive(true);
                        clickEnterBtn.Hide();
                        ShowViceBtn();
                    });
                    break;
                case ClickState.three:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(jing, "t3", false, () =>
                    {
                        clickBtn.SetActive(true);
                        clickEnterBtn.Hide();
                        ShowViceBtn();
                    });
                    break;
            }
        }
        /// <summary>
        /// 显示语音键
        /// </summary>
        private void ShowViceBtn()
        {
            if (voiceBtn == 1)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }
        #region
        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

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
    }
}
