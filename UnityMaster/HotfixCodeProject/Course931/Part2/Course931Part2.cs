using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
namespace ILFramework.HotClass
{
    public class Course931Part2
    {

        public enum BtnLName
        {
            gaunbi,
            close,
            three,
            four,
            five,
            Null
        }
        public enum BtnRName
        {
            one,
            two,
            three,
            four,
            five
        }
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject book;
        private Vector3 bellPos;

        private GameObject clickBtn;
        private Empty4Raycast[] clickBtns;
        private BtnLName clickLName = BtnLName.close;
        private BtnRName clickRName = BtnRName.one;

        private GameObject eyeWord;
        private Transform[] eyeWords;
        private string word;

        private GameObject eyeStru_1;
        private GameObject eyeStru_2;
        private GameObject eyeStru_3;
        private GameObject eyeStru_4;
        private GameObject eyeStru_5;
        private GameObject eyeStru_6;
       

        private GameObject eye_1;
        private GameObject eye_2;
        private GameObject eye_3;
        private GameObject eye_4;
        private GameObject eye_5;
        private GameObject eye_6;
        //private List<GameObject> eyeWords;


        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopCoroutine("WariteCoroutine");
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

            book = curTrans.Find("spines/book").gameObject;
            book.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(book, "fengmian", false);

            bellPos = curTrans.Find("bellPos").position;

            clickBtn = curTrans.Find("clickBtns").gameObject;
            clickBtns = clickBtn.GetComponentsInChildren<Empty4Raycast>(true);
            Util.AddBtnClick(clickBtns[0].gameObject, ClickBtnEvent);
            Util.AddBtnClick(clickBtns[1].gameObject, ClickBtnLEvent);
            Util.AddBtnClick(clickBtns[2].gameObject, ClickBtnREvent);
            clickBtn.SetActive(false);

            word = "wz";            
            eyeWord = curTrans.Find("spines/eyeWord").gameObject;
            eyeWords = eyeWord.GetComponentsInChildren<Transform>(true);

            eye_1 = curTrans.Find("spines/eyeWord/1").gameObject;
            eye_2 = curTrans.Find("spines/eyeWord/2").gameObject;
            eye_3 = curTrans.Find("spines/eyeWord/3").gameObject;
            eye_4 = curTrans.Find("spines/eyeWord/4").gameObject;
            eye_5 = curTrans.Find("spines/eyeWord/5").gameObject;
            eye_6 = curTrans.Find("spines/eyeWord/6").gameObject;
            
            PlayKongAni();

            eyeStru_1 = curTrans.Find("spines/1").gameObject;
            eyeStru_2 = curTrans.Find("spines/2").gameObject;
            eyeStru_3 = curTrans.Find("spines/3").gameObject;
            eyeStru_4 = curTrans.Find("spines/4").gameObject;
            eyeStru_5 = curTrans.Find("spines/5").gameObject;
            eyeStru_6 = curTrans.Find("spines/6").gameObject;
            EyeStru();
        }
        /// <summary>
        ///播空
        /// </summary>
        private void EyeStru()
        {
            eyeStru_1.Show();
            eyeStru_2.Show();
            eyeStru_3.Show();
            eyeStru_4.Show();
            eyeStru_5.Show();
            eyeStru_6.Show();
            SpineManager.instance.DoAnimation(eyeStru_1, "kong", false);
            SpineManager.instance.DoAnimation(eyeStru_2, "kong", false);
            SpineManager.instance.DoAnimation(eyeStru_3, "kong", false);
            SpineManager.instance.DoAnimation(eyeStru_4, "kong", false);
            SpineManager.instance.DoAnimation(eyeStru_5, "kong", false);
            SpineManager.instance.DoAnimation(eyeStru_6, "kong", false);
        }
        /// <summary>
        /// 文字动画拨空
        /// </summary>
        private void PlayKongAni()
        {           
            eye_1.Show();
            eye_2.Show();
            eye_3.Show();
            eye_4.Show();
            eye_5.Show();
            eye_6.Show();
            SpineManager.instance.DoAnimation(eye_1, "kong", false);
            SpineManager.instance.DoAnimation(eye_2, "kong", false);
            SpineManager.instance.DoAnimation(eye_3, "kong", false);
            SpineManager.instance.DoAnimation(eye_4, "kong", false);
            SpineManager.instance.DoAnimation(eye_5, "kong", false);
            SpineManager.instance.DoAnimation(eye_6, "kong", false);
        }
        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            Max.transform.DOMoveY(bellPos.y, 0.5f).OnComplete(() => 
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
                {
                    isPlaying = false;
                    clickBtn.SetActive(true);
                    clickBtns[0].gameObject.SetActive(true);
                    for (int i = 1; i < clickBtns.Length; i++)
                    {
                        clickBtns[i].gameObject.SetActive(false);
                    }                   
                }));
            });
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,4, true);
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
        IEnumerator WariteCoroutine( Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }

            talkIndex++;
        }
        private void ShowOrHideBtn(bool isShow)
        {
            for (int i = 1; i < clickBtns.Length; i++)
            {
                clickBtns[i].gameObject.SetActive(isShow);
            }
        } 
        /// <summary>
        /// 1按钮点击事件（中间按钮）
        /// </summary>
        /// <param name="obj"></param>
        private void ClickBtnEvent(GameObject obj)
        {
            ShowOrHideBtn(false);
            clickBtns[0].gameObject.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            SpineManager.instance.DoAnimation(book, "dakai", false, () =>
            {
                mono.StartCoroutine(WariteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 4));
                PlayWordAni();
            });           
        }
        /// <summary>
        /// 2按钮（L按钮）
        /// </summary>
        /// <param name="obj"></param>
        private void ClickBtnLEvent(GameObject obj)
        {
            ShowOrHideBtn(false);
            clickBtns[0].gameObject.SetActive(false);
            switch (clickLName)
            {
                case BtnLName.gaunbi:
                    //回到封面
                    PlayKongAni();
                    EyeStru();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(book, "guanbi", false, () =>
                    {
                        //mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, ()=> { clickBtns[0].gameObject.SetActive(true); })); 
                        clickBtns[0].gameObject.SetActive(true);
                    });
                                   
                    break;
                case BtnLName.close:
                    //回到眼球结构图  
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    mono.StartCoroutine(WariteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); }, 5));
                    BackVision("4", "b1", "kong", () =>
                    {
                        WordAni(() =>
                        {                             
                            clickLName = BtnLName.gaunbi;
                            clickRName = BtnRName.one;
                            ShowOrHideBtn(true);
                        });                                                         
                    });
                    break;
                case BtnLName.three:
                    //回到视觉形成(完成）2
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    CreateVision("6", "3", "b2","4", "donghua2",2,1,0, () =>
                    {
                        clickLName = BtnLName.close;
                        clickRName = BtnRName.two;
                        ShowOrHideBtn(true);
                        SpineManager.instance.SetTimeScale(eyeStru_5, 1);
                    });
                    break;
                case BtnLName.four:
                    //回到正常眼睛调节图  EyeVision("8", "12", "b3","6", "5", "donghua
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    EyeVision("8", "12", "b3","6", "kong", "donghua3",4, () => 
                    {
                        SpineManager.instance.DoAnimation(eyeStru_5, "12", false, () =>
                        {
                            clickLName = BtnLName.three;
                            clickRName = BtnRName.three;
                            ShowOrHideBtn(true);
                        });
                    });                                    
                    break;
                case BtnLName.five:
                    //回到近视图  VisionEye("8", "11", "b4", "7", "donghua4",
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    VisionEye("8", "11", "b4", "kong", "donghua4",3, () =>
                    {
                        SpineManager.instance.DoAnimation(eyeStru_5, "11", false);

                    },()=> {
                        clickLName = BtnLName.four;
                        clickRName = BtnRName.four;
                        ShowOrHideBtn(true);
                    });                  
                    break;
            }           
        }
        /// <summary>
        /// 3按钮（R按钮）
        /// </summary>
        /// <param name="obj"></param>
        private void ClickBtnREvent(GameObject obj)
        {
            PlayKongAni();
            ShowOrHideBtn(false);
            switch (clickRName)
            {
                case BtnRName.one:
                    //视觉形成（完成）
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    Vision("4","a1" , "donghua2",2, () => 
                    {
                        SpineManager.instance.SetTimeScale(eyeStru_3, 1);
                        clickLName = BtnLName.close;
                        clickRName = BtnRName.two;
                        ShowOrHideBtn(true);
                    });
                    break;
                case BtnRName.two:
                    //正常眼睛  //VisionEye("3", "6", "a2", "5","donghua3", () 
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);

                    EyeStru();
                    SpineManager.instance.DoAnimation(eyeStru_1, "3", false);
                    SpineManager.instance.DoAnimation(eyeStru_2, "6", false);
                    SpineManager.instance.DoAnimation(eyeStru_3, "a2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(eyeStru_6, "donghua3", false, ()=> 
                        {
                            SpineManager.instance.DoAnimation(eyeStru_5, "12", false);
                        });
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null,()=> 
                        {
                            clickLName = BtnLName.three;
                            clickRName = BtnRName.three;
                            ShowOrHideBtn(true);
                        }));
                    });                                                      
                    break;
                case BtnRName.three:
                    //近视眼 VisionEye("12", "8", "a3", "7", "donghua4", ()
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    VisionEye("12", "8", "a3", "kong", "donghua4",3, () =>
                    {
                        SpineManager.instance.DoAnimation(eyeStru_5, "11", false);                      
                    },()=> {
                        clickLName = BtnLName.four;
                        clickRName = BtnRName.four;
                        ShowOrHideBtn(true);
                    });                   
                    break;
                case BtnRName.four:
                    //假性近视
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    CreateVision("11", "10", "a4","9", "donghua5",6,0.5f,2, () =>
                    {
                        clickLName = BtnLName.five;
                        clickRName = BtnRName.five;
                        ShowOrHideBtn(true);
                        SpineManager.instance.SetTimeScale(eyeStru_5, 1);
                    });
                    break;
                case BtnRName.five:
                    //封面                   
                    EyeStru();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(book, "guanbi2", false,()=> 
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5,null,()=> 
                        {
                            clickLName = BtnLName.gaunbi;
                            clickRName = BtnRName.one;
                            clickBtns[0].gameObject.SetActive(true);
                        }));                       
                    });                   
                    break;
            }           
        }
        private void Vision(string aniName1,string aniName2,string aniName3,int index,Action callBack = null)
        {
            EyeStru();//不闪
            SpineManager.instance.DoAnimation(eyeStru_1, aniName1, false);
            aniTime = SpineManager.instance.DoAnimation(eyeStru_2, aniName2, false, () =>
            {
                SpineManager.instance.DoAnimation(eyeStru_4, "kong", false);
                mono.StartCoroutine(WariteCoroutine(() => 
                {
                    SpineManager.instance.DoAnimation(eyeStru_3, "kong", false, () =>
                    {
                        SpineManager.instance.SetTimeScale(eyeStru_3, 0.5f);
                        SpineManager.instance.DoAnimation(eyeStru_3, aniName3, false);
                    });
                }, aniTime));
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index, () =>
                {
                   
                }, callBack));
            });
        }
        private void BackVision(string aniName1, string aniName2, string aniName3, Action callBack = null)
        {
            EyeStru();//不闪
            SpineManager.instance.DoAnimation(eyeStru_1, aniName1, false);
            SpineManager.instance.DoAnimation(eyeStru_2, aniName2, false, () =>
            {
                SpineManager.instance.DoAnimation(eyeStru_4, "kong", false);
                SpineManager.instance.DoAnimation(eyeStru_3, "kong", false, () =>
                {
                    SpineManager.instance.DoAnimation(eyeStru_3, aniName3, false, callBack);
                });
            });
        }
        private void VisionEye(string name1,string name2,string name3,string name4,string name5,int index,Action callBack_1 = null,Action callBack_2=null)
        {           
            EyeStru();
            SpineManager.instance.DoAnimation(eyeStru_1, name1, false);
            SpineManager.instance.DoAnimation(eyeStru_2, name2, false);
            SpineManager.instance.DoAnimation(eyeStru_3, name3, false, ()=> 
            {
                SpineManager.instance.DoAnimation(eyeStru_6, name5, false, callBack_1);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index, () =>
                {
                    //SpineManager.instance.DoAnimation(eyeStru_6, name5, false, callBack_1);
                },
               callBack_2));
            });                      
        }
        private void EyeVision(string name1, string name2, string name3, string name4, string name5,string name6,int index, Action callBack = null)
        {
            EyeStru();
            eyeStru_3.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            PlayAni(name1, name2, name3, () =>
            {                                    
                SpineManager.instance.DoAnimation(eyeStru_4, name5, false);
                SpineManager.instance.DoAnimation(eyeStru_1, name4, false);
                SpineManager.instance.DoAnimation(eyeStru_6, name6, false, callBack);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index, () =>
                {
                    //eyeStru_6.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    //SpineManager.instance.DoAnimation(eyeStru_6, name6, false, callBack);
                },
               null));
            });           
        }
        private float aniTime;
        private void CreateVision(string aniName1, string aniName2, string aniName3,string aniName4,string aniName5,int index,float time,float waiteTime, Action callBack = null)
        {
            EyeStru();//不闪           
            PlayAni(aniName1, aniName2, aniName3, () => 
            {              
                SpineManager.instance.DoAnimation(eyeStru_3, "kong", false);
                aniTime = SpineManager.instance.DoAnimation(eyeStru_4, aniName4, false, () =>
                {
                    mono.StartCoroutine(WariteCoroutine(()=> 
                    {
                        mono.StartCoroutine(WariteCoroutine(() => {
                            SpineManager.instance.SetTimeScale(eyeStru_5, time);
                            SpineManager.instance.DoAnimation(eyeStru_5, aniName5, false);
                        }, waiteTime));
                    }, aniTime));
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index, () =>
                    {
                       
                    },
               callBack));
                });
            });           
        }
        private void PlayAni(string name1,string name2,string name3,Action callBack = null)
        {
            SpineManager.instance.DoAnimation(eyeStru_1, name1, false);
            SpineManager.instance.DoAnimation(eyeStru_2, name2, false);
            SpineManager.instance.DoAnimation(eyeStru_3, name3, false, callBack);

        }
        private void PlayWordAni()
        {           
            SpineManager.instance.DoAnimation(eyeStru_1, "1", false);
            SpineManager.instance.DoAnimation(eyeStru_3, "kong", false);
            SpineManager.instance.DoAnimation(eyeStru_2, "2", false, () =>
            {               
                WordAni(() => { SetPlayEyeTime(); ShowOrHideBtn(true); });
            });           
        }
        private void WordAni(Action callBack = null)
        {
             mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, null));
             mono.StartCoroutine(WariteCoroutine(() => 
             {
                SpineManager.instance.DoAnimation(eyeWords[1].gameObject, word + 1, false,()=> 
                {
                    SpineManager.instance.SetTimeScale(eyeWords[2].gameObject, 0.5f);
                    SpineManager.instance.DoAnimation(eyeWords[2].gameObject, word + 2, false,()=> 
                    {
                        SpineManager.instance.SetTimeScale(eyeWords[3].gameObject, 0.5f);
                        SpineManager.instance.DoAnimation(eyeWords[3].gameObject, word + 3, false, () =>
                        {
                            SpineManager.instance.SetTimeScale(eyeWords[4].gameObject, 0.5f);
                            SpineManager.instance.DoAnimation(eyeWords[4].gameObject, word + 4, false, () =>
                            {                               
                                SpineManager.instance.DoAnimation(eyeWords[5].gameObject, word + 5, false, () =>
                                {                                    
                                    SpineManager.instance.DoAnimation(eyeWords[6].gameObject, word + 6, false, callBack);
                                });
                            });
                        });
                    });
                });                 
             }, 3.5f));           
        }
        private void SetPlayEyeTime()
        {
            for (int i = 1; i < eyeWords.Length; i++)
            {
                SpineManager.instance.SetTimeScale(eyeWords[i].gameObject, 1);
            }
        }
        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
    }
}
