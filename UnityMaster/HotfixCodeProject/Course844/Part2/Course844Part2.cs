using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course844Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private Transform up;
        private Transform down;
        private Transform upImg;
        private Transform downImg;
        private Transform spineGo;
        private Transform life;
        private float SportNum;
        private Transform mask;
        private Transform robotman;

        private bool _canUp;
        private bool _canDown;

        bool isPlaying = false;


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
            up = curTrans.Find("upBtn");
            down = curTrans.Find("downBtn");
            upImg = curTrans.Find("up");
            downImg = curTrans.Find("down");
            spineGo = curTrans.Find("robot");
            life = curTrans.Find("life");
            mask = curTrans.Find("mask");
            SportNum = 0;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            _canDown = true;
            _canUp = false;
            PlaySpine(life.gameObject,"0",null,false);
            PlaySpine(spineGo.gameObject, "fwc5", null, false);
            SportNum = 0;
            mask.gameObject.SetActive(false);
            Util.AddBtnClick(up.gameObject,OnClickUp);
            Util.AddBtnClick(down.gameObject, OnClickDown);
            robotman = curTrans.Find("Bell");
            robotman.gameObject.SetActive(true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            
            
            mono.StartCoroutine(SpeckerCoroutine(robotman.gameObject, SoundManager.SoundType.SOUND, 0, null, () =>
            {
                up.GetComponent<Empty4Raycast>().raycastTarget = true; down.GetComponent<Empty4Raycast>().raycastTarget = true;
                robotman.gameObject.SetActive(false);
                isPlaying = true;
            }));
            // mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () => { Max.SetActive(false); isPlaying = false; }));
            

            ConnectAndroid.Instance.SendConnectData();
            FunctionOf3Dof.Current3DofCallBack = TriggerCallBack;

            
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

            SpineManager.instance.DoAnimation(speaker, "daiji1");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji1");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
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


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        private void OnClickUp(GameObject obj) //机器人俯卧撑上按钮
        {
           if(!isPlaying)
               return;
           
           if (_canUp==true)
           {
                BtnPlaySound();
                _canUp = false;
                upImg.GetComponent<RawImage>().texture = upImg.GetComponent<BellSprites>().texture[1];
                Delay(0.2f,()=> { upImg.GetComponent<RawImage>().texture = upImg.GetComponent<BellSprites>().texture[0]; });
                PlaySpine(spineGo.gameObject, "fwc4", () =>
                {
                    PlaySpine(spineGo.gameObject, "fwc5", ()=> 
                    {
                        _canDown = true;
                        SportNum =SportNum+0.5f;
                        Debug.Log(SportNum);
                        CheckSportNum(); 
                    }, false);
                }, false);
             
           }
        }

        private void TriggerCallBack()
        {
            if(!isPlaying)
                return;
            if (FunctionOf3Dof.Instance.EulerAngleY > 15f)
            {
                if (_canUp == true)
                {
                    BtnPlaySound();
                    _canUp = false;
                    PlaySpine(spineGo.gameObject, "fwc4", () =>
                    {
                        PlaySpine(spineGo.gameObject, "fwc5", () =>
                        {
                            _canDown = true;
                            SportNum = SportNum + 0.5f;
                            Debug.Log(SportNum);
                            CheckSportNum();
                        }, false);
                    }, false);

                }
            }
            else if (FunctionOf3Dof.Instance.EulerAngleY < -15f)
            {
                if(!isPlaying)
                    return;
                if (_canDown == true)
                {
                    BtnPlaySound();
                    _canDown = false;
                    PlaySpine(spineGo.gameObject, "fwc3", () =>
                    {
                        PlaySpine(spineGo.gameObject, "fwc6", () => { _canUp = true; SportNum = SportNum + 0.5f; }, false);
                    }, false);
                }
            }
        }


        private void OnClickDown(GameObject obj)  //机器人俯卧撑下按钮
        {
           if(!isPlaying)
               return;
            if (_canDown==true)
            {
                BtnPlaySound();
                _canDown = false;
                downImg.GetComponent<RawImage>().texture = downImg.GetComponent<BellSprites>().texture[1];
                Delay(0.2f, () => { downImg.GetComponent<RawImage>().texture = downImg.GetComponent<BellSprites>().texture[0]; });
                PlaySpine(spineGo.gameObject, "fwc3", () =>
                {
                    PlaySpine(spineGo.gameObject, "fwc6", ()=> { _canUp = true; SportNum = SportNum + 0.5f; }, false);
                }, false);
            }
        }
        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }
        private void Delay(float delay, Action callBack)
        {
            mono.StartCoroutine(IEDelay(delay, callBack));
        }
        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }
        private void CheckSportNum() 
        {
            if (SportNum > 10) 
            {
                return;
            }
            switch (SportNum) 
            {
                case 1:
             
                    PlaySpine(life.gameObject,SportNum.ToString(),null,false);
                    break;
                case 2:
       
                    PlaySpine(life.gameObject, SportNum.ToString(), null, false);
                    break;
                case 3:
       
                    PlaySpine(life.gameObject, SportNum.ToString(), null, false);
                    break;
                case 4:
          
                    PlaySpine(life.gameObject, SportNum.ToString(), null, false);
                    break;
                case 5:
               
                    PlaySpine(life.gameObject, SportNum.ToString(), null, false);
                    break;
                case 6:
             
                    PlaySpine(life.gameObject, SportNum.ToString(), null, false);
                    break;
                case 7:
              
                    PlaySpine(life.gameObject, SportNum.ToString(), null, false);
                    break;
                case 8:
                  
                    PlaySpine(life.gameObject, SportNum.ToString(), null, false);
                    break;
                case 9:
                    
                    PlaySpine(life.gameObject, SportNum.ToString(), null, false);
                    break;
                case 10:
                    isPlaying = false;
                    PlaySpine(life.gameObject, SportNum.ToString(), ()=>
                    {
                        FunctionOf3Dof.Instance.IsOpened = false;
                        mask.gameObject.SetActive(true);
                        
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0);
                        PlaySpine(mask.GetChild(0).gameObject,"animation",()=> 
                        {

                            PlaySpine(mask.GetChild(0).gameObject, "animation2",null,true); 
                        },false
                            );
                    }, false);
                    break;
            }
        }
    }
}
