using DG.Tweening;
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
    public class Course735Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private Transform SpineGo;
        private Transform SpineGo2;
        private Transform SpineGo3;

        private Transform SpineGo22a;
        private Transform SpineGo22b;
        private int index;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            SpineGo = curTrans.Find("spineGo");
            SpineGo2 = curTrans.Find("spineGo2");
            SpineGo3 = curTrans.Find("spineGo3");

            SpineGo22a = curTrans.Find("spineGo22a");
            SpineGo22b = curTrans.Find("spineGo22b");
            index = 0;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            CW();
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            Wait(0.6f,()=> 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
            });
          
            //交警通过摆动肩关节、肘关节、腕关节与转动颈部关节，完成指挥手势。
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { isPlaying = false; }));
            Wait(1.5f,()=> 
            {
                SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "a2", false, () =>
                {
                    SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "a3", false, () =>
                    {
                        SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "a4", false, () =>
                        {
                            SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "a5", true);
                            //交警手势信号共有8种，我们以“直行信号”为例，思考机器人交警可以使用哪些关节球实现手势信号动作。
                            Wait(1.5f,()=>
                            {
                               
                                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 1, null, () => { SoundManager.instance.ShowVoiceBtn(true); isPlaying = false; }));
                            });
                           
                        });
                    });
                });
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
            if (talkIndex == 1)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                //直行信号，准许右方直行的车辆通行。接下来，通过点击交警，依次熟悉直行信号的分解动作吧。
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 2, null, () => {
                    SpineGo.GetComponent<Image>().raycastTarget = true;
                    Util.AddBtnClick(SpineGo.gameObject, MoveDW);
                }));
               
                SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "b1", false, () =>
                   {
                       Wait(1.5f, () =>
                        {
                            SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "a5", false);
                      
                        });

                   });
            }
            else if (talkIndex == 2) 
            {

                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND,8, null, () => { Max.SetActive(false); }));
                SpineGo.gameObject.SetActive(false);
                SpineGo2.gameObject.SetActive(false);
                SpineGo3.gameObject.SetActive(true);
                SpineGo22a.gameObject.SetActive(false);
                SpineGo22b.gameObject.SetActive(false);
            }

            talkIndex++;
        }
        private void MoveDW(GameObject obj)
        {
            index++;
            BtnPlaySound();
            obj.GetComponent<Image>().raycastTarget = false;
            if (index == 1)
            {
                obj.GetComponent<Image>().raycastTarget = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                SpineGo.GetRectTransform().DOAnchorPos(new Vector2(-269, 549), 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    SpineGo2.gameObject.SetActive(true); SpineGo22a.gameObject.SetActive(true); SpineGo22b.gameObject.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 3, null, () => { obj.GetComponent<Image>().raycastTarget = true; }));
              
                    SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "b1", false);
                    SpineManager.instance.DoAnimation(SpineGo2.gameObject, "animation", false);
                    SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua2", false, () =>
                     {
                      
                     });
                    SpineManager.instance.DoAnimation(SpineGo22b.gameObject, "qiub2", false, () =>
                    {

                    });
                });
                SpineGo.GetRectTransform().DOScale(new Vector2(0.8f, 0.8f), 1f);
            }
            else if (index == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 4, null, () => { obj.GetComponent<Image>().raycastTarget = true; }));
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "b3", false);
                //头回正左转 右手不动
                SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua3", false,()=> 
                {
                    SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua4", false);
                });
            }
            else if (index == 3)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,2, false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 5, null, () => { obj.GetComponent<Image>().raycastTarget = true; }));
                SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "b7", false,()=> 
                {
                    SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "b8", false);
                });
                //头左转
                SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua5", false, () =>
                {
                    SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua2", false);
                });
            }
            else if (index == 4)
            {
          
             //   SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,3, false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 6, null, () => { obj.GetComponent<Image>().raycastTarget = true; }));
               
                    SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "b6", false);
              
              
                //无
               // SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua2", false);
                
                //SpineManager.instance.DoAnimation(SpineGo22b.gameObject,"qiub3",false);
            }
            else if (index == 5)
            {
                //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 7, null, () => { obj.GetComponent<Image>().raycastTarget = true; SoundManager.instance.ShowVoiceBtn(true); }));
                //重复完  头回正 手回正
              
                SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "b7", false, () =>
                {
                    SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "b8", false);

                    //SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua3", false, () =>
                    //{
                    //    SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua4", false, () =>
                    //    {
                    //        SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua5", false);
                    //    });
                    //});
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
                    Wait(1.5f, () =>
                     {
                       
                         SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua3", false, () =>
                         {
                             SpineManager.instance.DoAnimation(SpineGo22a.gameObject, "qiua1", false, () =>
                             {
                                 
                             });
                         });
                         SpineManager.instance.DoAnimation(SpineGo22b.gameObject, "qiub3", false);

                         SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject, "a5", false, () =>
                         {
                            
                         });
                      
                     });
                   
                 });
            }

        }
        private void CW()
        {
            SpineGo.transform.localScale = new Vector3(1,1,1);
            SpineGo.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
          SpineManager.instance.DoAnimation(SpineGo.GetChild(0).gameObject,"a5",true);
            SpineGo.GetRectTransform().anchoredPosition = new Vector2(38, 545);
            SpineGo.gameObject.SetActive(true);
            SpineGo2.gameObject.SetActive(false);
            SpineGo3.gameObject.SetActive(false);
            SpineGo.GetComponent<Image>().raycastTarget = false;
            SpineGo22a.gameObject.SetActive(false); SpineGo22b.gameObject.SetActive(false);
            SpineGo22a.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineGo22b.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            index = 0;
        }

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

        IEnumerator wait(float time,Action method=null) 
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }
        private void Wait(float time, Action method = null) 
        {
            mono.StartCoroutine(wait(time,method));
        }
    }
}
