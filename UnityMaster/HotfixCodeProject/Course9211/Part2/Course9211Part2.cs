using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course9211Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;
        private Transform OnClickBtn;
        private Transform max;
        private Transform ball;
        private int index1;
        private int index2;
        private Transform ui1;
        private Transform ui2;
        private Transform Panel;
        private Transform effect;
        private bool _canClick;
        private int num1=0;
        private int num2=0;
        private int allnum = 0;
        private Transform textArt;
        bool isPlaying = false;
        private bool _canBlack;
        private bool _canBlack2;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            OnClickBtn = curTrans.Find("OnClickBtn");
            Max = curTrans.Find("bell").gameObject;
            max = curTrans.Find("max");
            ball = curTrans.Find("Bg/ball");
            ui1 = curTrans.Find("ui1");
            ui2 = curTrans.Find("ui2");
            Panel = curTrans.Find("Bg/Panel");
            effect = curTrans.Find("effect");
            textArt = curTrans.Find("Bg/Panel/text");
            _canBlack = true;
            _canBlack2 = true;
            index1 = 0;
            index2 = 1;
            _canClick =false;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            num1 = 0;
            num2 = 0;
            _canBlack = true;
            _canBlack2 = true;
            SpineManager.instance.DoAnimation(ui1.gameObject, "ui1", false);
            SpineManager.instance.DoAnimation(ui2.gameObject, "ui2", false);
            SpineManager.instance.DoAnimation(Bg.transform.GetChild(0).gameObject, "beijing2", true);
            textArt.GetComponent<Text>().text = 0.ToString();
            OnClickBtn.GetChild(1).gameObject.GetComponent<Empty4Raycast>().raycastTarget = true;
            OnClickBtn.GetChild(0).gameObject.GetComponent<Empty4Raycast>().raycastTarget = true;
            gameInit();
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            //同学们 我们需要改变变量的值  第一句话
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); _canClick = true; }));
            UpDate(0.01f, () =>
             {

             if ((textArt.GetComponent<Text>().text == 18.ToString()|| textArt.GetComponent<Text>().text == 19.ToString() || textArt.GetComponent<Text>().text == 20.ToString())&&_canBlack==true) 
                 {

                     //三分黑
                     SpineManager.instance.DoAnimation(ui2.gameObject,"ui2j",false);
                     OnClickBtn.GetChild(1).gameObject.GetComponent<Empty4Raycast>().raycastTarget = false;
                     _canBlack = false;
                 }
                 if ((textArt.GetComponent<Text>().text == 19.ToString() || textArt.GetComponent<Text>().text == 20.ToString())&&_canBlack2==true) 
                 {
                     SpineManager.instance.DoAnimation(ui1.gameObject, "ui1j", false);
                     OnClickBtn.GetChild(0).gameObject.GetComponent<Empty4Raycast>().raycastTarget = false;
                     _canBlack2 = false;
                     //二分黑
                 }
             });
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



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    Max.SetActive(true);
                   // SpineManager.instance.DoAnimation(ui1.gameObject, "ui1", false);
                   // SpineManager.instance.DoAnimation(ui2.gameObject, "ui2", false);
                    mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,null));
                    textArt.GetComponent<Text>().text = 0.ToString();
                    PlayBallSpine();
                  
                    break;
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void OnClickEvent1(GameObject obj) 
        {
            index2 = 0;
      
            if (_canClick)
            {
                num1++;
                SoundManager.instance.ShowVoiceBtn(false);
                _canClick = false;
                SpineManager.instance.DoAnimation(ui1.gameObject,"ui12",false,()=> 
                {
                    SpineManager.instance.DoAnimation(ui1.gameObject, "ui1", false);
                });
                if (index1 == 0)
                {
                    SpineManager.instance.DoAnimation(max.gameObject, "z", false, () =>
                    {
                        SpineManager.instance.DoAnimation(ball.gameObject, "sz2", false);
                        SpineManager.instance.DoAnimation(Bg.transform.GetChild(0).gameObject, "beijing3", true);
                        WaitTime(0.5f,()=> { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false); });
                      
                        SpineManager.instance.DoAnimation(max.gameObject, "2f", false, () =>
                        {
                            //     SpineManager.instance.DoAnimation(max.gameObject,"3fd",true);  
                            SoundManager.instance.ShowVoiceBtn(true);
                            index1++;
                            _canClick = true;

                            //  Panel.GetChild(3).gameObject.SetActive(false);
                            // Panel.GetChild(2).gameObject.SetActive(true);
                            //  PanlUpdate();
                            allnum = allnum + 2;
                            textArt.GetComponent<Text>().text = allnum.ToString();


                            SpineManager.instance.DoAnimation(effect.gameObject,"zi",false,()=> 
                            { 
                                SpineManager.instance.DoAnimation(effect.gameObject,"kong",false);
                            });
                        });
                    });
                }
                if (index1 != 0)
                {
                    SpineManager.instance.DoAnimation(ball.gameObject, "sz2", false);

                    WaitTime(0.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false); });
                    SpineManager.instance.DoAnimation(max.gameObject, "2f", false, () =>
                    {
                        //     SpineManager.instance.DoAnimation(max.gameObject,"3fd",true);  
                        SoundManager.instance.ShowVoiceBtn(true);
                        _canClick = true;
                        // PanlUpdate();
                        allnum = allnum + 2;
                        textArt.GetComponent<Text>().text = allnum.ToString();
                        SpineManager.instance.DoAnimation(effect.gameObject, "zi", false, () =>
                        {
                            SpineManager.instance.DoAnimation(effect.gameObject, "kong", false);
                        });
                    });
                }
              
            }
        
           
        }

        private void OnClickEvent2(GameObject obj)
        {
            index1 = 0;
           
            if (_canClick)
            {
                num2++;
                SoundManager.instance.ShowVoiceBtn(false);
                SpineManager.instance.DoAnimation(ui2.gameObject, "ui22", false,()=> 
                {
                   SpineManager.instance.DoAnimation(ui2.gameObject, "ui2", false);
                });
                _canClick = false;
                if (index2 == 0)
                {
                    SpineManager.instance.DoAnimation(max.gameObject, "z2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Bg.transform.GetChild(0).gameObject, "beijing3", true);
                        SpineManager.instance.DoAnimation(ball.gameObject, "sz3", false);
                        WaitTime(0.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false); });
                        SpineManager.instance.DoAnimation(max.gameObject, "3f", false, () =>
                        {
                            //     SpineManager.instance.DoAnimation(max.gameObject,"3fd",true);
                            SoundManager.instance.ShowVoiceBtn(true);
                            _canClick = true;
                            index2++;
                            // PanlUpdate();
                            allnum = allnum + 3;
                            textArt.GetComponent<Text>().text = allnum.ToString();
                            // Panel.GetChild(3).gameObject.SetActive(true);

                            SpineManager.instance.DoAnimation(effect.gameObject, "zi2", false, () =>
                            {
                                SpineManager.instance.DoAnimation(effect.gameObject, "kong", false);
                            });
                        });
                    });
                }
                else if (index2 != 0)
                {
                    SpineManager.instance.DoAnimation(Bg.transform.GetChild(0).gameObject, "beijing3", true);
                    SpineManager.instance.DoAnimation(ball.gameObject, "sz3", false);
                    WaitTime(0.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false); });
                    SpineManager.instance.DoAnimation(max.gameObject, "3f", false, () =>
                    {
                        //     SpineManager.instance.DoAnimation(max.gameObject,"3fd",true);  
                        SoundManager.instance.ShowVoiceBtn(true);
                        _canClick = true;
                        //PanlUpdate();
                        allnum = allnum + 3;
                        textArt.GetComponent<Text>().text = allnum.ToString();
                        SpineManager.instance.DoAnimation(effect.gameObject, "zi2", false, () =>
                        {
                            SpineManager.instance.DoAnimation(effect.gameObject, "kong", false);
                        });
                    });
                }
               
            }
            
        }
        private void PlayBallSpine() 
        {
            _canClick = false;
           
            if (index1 == 0)
            {
                BallSpine();
            }
            else 
            {
                BallSpine2();
            }
        }
        private void BallSpine() 
        {
         
            SpineManager.instance.DoAnimation(max.gameObject, "z", false, () =>
            {
               // SpineManager.instance.DoAnimation(Bg.transform.GetChild(0).gameObject, "beijing3", true);
                SpineManager.instance.DoAnimation(ball.gameObject, "sz2", false);
                WaitTime(0.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false); });
                SpineManager.instance.DoAnimation(max.gameObject, "2f", false, () =>
                {
                    //     SpineManager.instance.DoAnimation(max.gameObject,"3fd",true);  

                    textArt.GetComponent<Text>().text = 2.ToString();
                    SpineManager.instance.DoAnimation(effect.gameObject, "zi", false, () =>
                    {
                        SpineManager.instance.DoAnimation(effect.gameObject, "kong", false);
                    });


                    SpineManager.instance.DoAnimation(max.gameObject, "z2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(ball.gameObject, "sz3", false);
                        WaitTime(0.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false); });
                        SpineManager.instance.DoAnimation(max.gameObject, "3f", false, () =>
                        {
                            //     SpineManager.instance.DoAnimation(max.gameObject,"3fd",true);


                            textArt.GetComponent<Text>().text = 5.ToString();
                            SpineManager.instance.DoAnimation(effect.gameObject, "zi2", false, () =>
                            {
                                SpineManager.instance.DoAnimation(effect.gameObject, "kong", false);
                            });
                        });
                    });
                });
            });
        }
        private void BallSpine2() 
        {
           // SpineManager.instance.DoAnimation(Bg.transform.GetChild(0).gameObject, "beijing3", true);
            SpineManager.instance.DoAnimation(ball.gameObject, "sz2", false);
            WaitTime(0.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false); });
            SpineManager.instance.DoAnimation(max.gameObject, "2f", false, () =>
            {
                //     SpineManager.instance.DoAnimation(max.gameObject,"3fd",true);  

                textArt.GetComponent<Text>().text = 2.ToString();
                SpineManager.instance.DoAnimation(effect.gameObject, "zi", false, () =>
                {
                    SpineManager.instance.DoAnimation(effect.gameObject, "kong", false);
                });


                SpineManager.instance.DoAnimation(max.gameObject, "z2", false, () =>
                {
                    SpineManager.instance.DoAnimation(ball.gameObject, "sz3", false);
                    WaitTime(0.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false); });
                    SpineManager.instance.DoAnimation(max.gameObject, "3f", false, () =>
                    {
                        //     SpineManager.instance.DoAnimation(max.gameObject,"3fd",true);


                        textArt.GetComponent<Text>().text = 5.ToString();
                        SpineManager.instance.DoAnimation(effect.gameObject, "zi2", false, () =>
                        {
                            SpineManager.instance.DoAnimation(effect.gameObject, "kong", false);
                        });
                    });
                });
            });
        }

        private void gameInit() 
        {
            effect.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            max.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Util.AddBtnClick(OnClickBtn.GetChild(0).gameObject, OnClickEvent1);
            Util.AddBtnClick(OnClickBtn.GetChild(1).gameObject, OnClickEvent2);
            SpineManager.instance.DoAnimation(ball.gameObject, "kong", false);
          
            SpineManager.instance.DoAnimation(max.gameObject, "3fd", false);
           
     
            _canClick = false;
        }
        private void PanlFalse() 
        {
            Panel.GetChild(1).gameObject.SetActive(false); 
            Panel.GetChild(2).gameObject.SetActive(false); 
            Panel.GetChild(3).gameObject.SetActive(false);
            Panel.GetChild(4).gameObject.SetActive(false); 
            Panel.GetChild(5).gameObject.SetActive(false);
            Panel.GetChild(6).gameObject.SetActive(false);
            Panel.GetChild(7).gameObject.SetActive(false);
            Panel.GetChild(8).gameObject.SetActive(false);
            Panel.GetChild(9).gameObject.SetActive(false);
        }
        private void PanlUpdate() 
        {
            PanlFalse();
            allnum = num1 * 2 + num2 * 3;
            Panel.GetChild(allnum).gameObject.SetActive(true);
        }
        IEnumerator IEUpdate(float delay, Action callBack)
        {
            while (true) 
            {
                yield return new WaitForSeconds(delay);

                callBack?.Invoke();
                if (OnClickBtn.GetChild(1).gameObject.GetComponent<Empty4Raycast>().raycastTarget == false && OnClickBtn.GetChild(0).gameObject.GetComponent<Empty4Raycast>().raycastTarget == false) 
                {
                    break;
                }
            }
             
            
        }
        IEnumerator IEDelay(float time, Action callBack=null)
        {
            yield return new WaitForSeconds(time);
            callBack?.Invoke();
        }
        private void WaitTime(float time, Action callBack=null)
        {
            mono.StartCoroutine(IEDelay(time, callBack));
        }
        private void UpDate(float delay, Action callBack)
        {
            mono.StartCoroutine(IEUpdate(delay, callBack));
        }
    }
}
