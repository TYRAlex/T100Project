using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Data;

namespace ILFramework.HotClass
{
    public class Course848Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;
        private Animator a;
        private Animator b;
        private Animator c;
        private Animator d;
        private Animator e;
        private Transform a_gameObject;
        private Transform b_gameObject;
        private Transform c_gameObject;
        private Transform d_gameObject;
        private Transform e_gameObject;
        private Transform js_gameObject;
        private Transform ui1; private Transform ui12; private Transform ui13;
        private Transform ui2;
        private Transform OnClick;
        private Transform btn;
        private Transform _mask;
        private Transform Mask;
        private Text jstext;
        private string sum1=" ";
        private string sum2=" ";
        private double result=0;
        private string sum = " ";
        private string allnumandOpera=" ";
        private string bb = " ";
        private DataTable dt;
        private bool _canClickOperator = false;
        private bool _canClickZero = false;
        bool isPlaying = false;
        private int LevelNum = 0;
        private int OperatorNum = 0;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DataTable dt = new DataTable();
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            a = curTrans.Find("02/02").GetComponent<Animator>();
            b = curTrans.Find("03/03").GetComponent<Animator>();
            c = curTrans.Find("04/04").GetComponent<Animator>();
            d = curTrans.Find("05/05").GetComponent<Animator>();
            e = curTrans.Find("06/06").GetComponent<Animator>();
            ui1 = curTrans.Find("ui1"); ui12 = curTrans.Find("ui12"); ui13 = curTrans.Find("ui13");
            ui2 = curTrans.Find("ui2");
            a_gameObject = curTrans.Find("02");
            b_gameObject = curTrans.Find("03");
            c_gameObject = curTrans.Find("04");
            d_gameObject = curTrans.Find("05");
            e_gameObject = curTrans.Find("06");
            js_gameObject = curTrans.Find("js");
            _mask = curTrans.Find("_mask");
            allnumandOpera = " ";
            btn = curTrans.Find("btn");
            OnClick = curTrans.Find("OnClick");
            jstext = curTrans.Find("js/Text").GetComponent<Text>();
            Mask = curTrans.Find("mask");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Mask.gameObject.SetActive(false);
            jstext.GetComponent<Text>().text = "";
            OnClick.gameObject.SetActive(false);
            a.SetBool("isplay", false);
            a_gameObject.GetRectTransform().DOAnchorPos(new Vector3(0, 0,200),0.1f);
            _mask.gameObject.SetActive(false);
            OnClick.GetChild(16).gameObject.SetActive(true);
            btn.gameObject.SetActive(false);
            ui1.gameObject.SetActive(false);
            ui12.gameObject.SetActive(false);
            ui13.gameObject.SetActive(false);
            ui2.gameObject.SetActive(false);
            js_gameObject.GetRectTransform().DOAnchorPos(new Vector2(1360,1520),0.01f);
            jstext.text = " ";LevelNum = 0;
            sum1 = " ";
            sum = " ";
            a_gameObject.gameObject.SetActive(true);
            b_gameObject.gameObject.SetActive(false); c_gameObject.gameObject.SetActive(false); 
            d_gameObject.gameObject.SetActive(false); e_gameObject.gameObject.SetActive(false);
            Util.AddBtnClick(OnClick.GetChild(0).gameObject, OnClickEvent0);
            Util.AddBtnClick(OnClick.GetChild(1).gameObject, OnClickEvent1);
            Util.AddBtnClick(OnClick.GetChild(2).gameObject, OnClickEvent2);
            Util.AddBtnClick(OnClick.GetChild(3).gameObject, OnClickEvent3);
            Util.AddBtnClick(OnClick.GetChild(4).gameObject, OnClickEvent4);
            Util.AddBtnClick(OnClick.GetChild(5).gameObject, OnClickEvent5);
            Util.AddBtnClick(OnClick.GetChild(6).gameObject, OnClickEvent6);
            Util.AddBtnClick(OnClick.GetChild(7).gameObject, OnClickEvent7);
            Util.AddBtnClick(OnClick.GetChild(8).gameObject, OnClickEvent8);
            Util.AddBtnClick(OnClick.GetChild(9).gameObject, OnClickEvent9);

            Util.AddBtnClick(OnClick.GetChild(10).gameObject, OnClickAdd);//+
            Util.AddBtnClick(OnClick.GetChild(11).gameObject, OnClickjian);//-
            Util.AddBtnClick(OnClick.GetChild(12).gameObject, OnClickcheng);//*
            Util.AddBtnClick(OnClick.GetChild(13).gameObject, OnClickchu);// 除
            Util.AddBtnClick(OnClick.GetChild(14).gameObject, OnClickDengyu);//=
            Util.AddBtnClick(OnClick.GetChild(15).gameObject, OnClickchongzhi);//c
            Util.AddBtnClick(OnClick.GetChild(16).gameObject, JulLevel);//c
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            first();

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
        IEnumerator SpeckerCoroutineMax(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                //第一题回答正确
                a_gameObject.gameObject.SetActive(false);
                b_gameObject.gameObject.SetActive(true); _canClickOperator = false;
                jstext.text = " ";
                sum = " ";
                sum1 = " ";
                bb = " ";
                allnumandOpera = " ";
                LevelNum++;
                OnClick.GetChild(16).gameObject.SetActive(true);
            }
            if (talkIndex == 2) 
            {
                //第二题回答正确
                b_gameObject.gameObject.SetActive(false); _canClickOperator = false;
                c_gameObject.gameObject.SetActive(true);
                jstext.text = " ";
                sum = " ";
                sum1 = " ";
                bb = " ";
                allnumandOpera = " ";
                LevelNum++;
                OnClick.GetChild(16).gameObject.SetActive(true);
            }
            if (talkIndex == 3)
            {
                //第三题回答正确
                c_gameObject.gameObject.SetActive(false); _canClickOperator = false;
                d_gameObject.gameObject.SetActive(true);
                _mask.gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,3);
                WaitTime(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3),()=> { _mask.gameObject.SetActive(false); });
                jstext.text = " ";
                sum = "0";
                sum1 = " ";
                bb = " ";
                allnumandOpera = " ";
                LevelNum++;
                ui2.gameObject.SetActive(false); ui12.gameObject.SetActive(true);
                OnClick.GetChild(16).gameObject.SetActive(true);
            }
            if (talkIndex ==4)
            {
                //第四题回答正确
                d_gameObject.gameObject.SetActive(false); _canClickOperator = false;
                e_gameObject.gameObject.SetActive(true);
                jstext.text = " ";
                sum = " ";
                sum1 = " ";
                bb = "";
                allnumandOpera = " ";
                LevelNum++; 
                ui12.gameObject.SetActive(false); ui13.gameObject.SetActive(true);
                OnClick.GetChild(16).gameObject.SetActive(true);
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        private void WaitTime(float time, Action method_1 = null) 
        {
            mono.StartCoroutine(iewait(time, method_1)) ;
        }
        IEnumerator iewait(float time,Action method_1=null) 
        {
            yield return new WaitForSeconds(time);
            method_1?.Invoke();
        }
        private void first() 
        {
            mono.StartCoroutine(SpeckerCoroutineMax(Max, SoundManager.SoundType.SOUND, 0,
                () =>
                {
                    a.SetBool("isplay", true);
                    a.SetBool("isContine", true);
                }
                , () =>
                {
                    Max.SetActive(false);
                   
                  
                    isPlaying = false;
                }));

            WaitTime(8f,()=> { a.SetBool("isend", true); a.SetBool("isplay", false); a.SetBool("isContine", false); });

            WaitTime(11f,()=> 
            {
                ui1.gameObject.SetActive(true); ui2.gameObject.SetActive(true);
            });
            WaitTime(18f, () =>
            {
                // a_gameObject.position = new Vector3(-443,2,-331);
                a_gameObject.GetRectTransform().DOAnchorPos(new Vector2(-405, 0), 1f);
                js_gameObject.GetRectTransform().DOAnchorPos(new Vector2(1360, 638), 1f).OnComplete(() => { OnClick.gameObject.SetActive(true);btn.gameObject.SetActive(true); });
            });
        }
        private void OnClickEvent0(GameObject obj) 
        {
            if (_canClickZero) 
            {
                InputNum(obj);
            }
          
        }
        private void OnClickEvent1(GameObject obj)
        {
            InputNum(obj); _canClickOperator = true;
        }
        private void OnClickEvent2(GameObject obj)
        {
            InputNum(obj); _canClickOperator = true;
        }
        private void OnClickEvent3(GameObject obj)
        {
            InputNum(obj); _canClickOperator = true;
        }
        private void OnClickEvent4(GameObject obj)
        {
            InputNum(obj); _canClickOperator = true;
        }
        private void OnClickEvent5(GameObject obj)
        {
            InputNum(obj); _canClickOperator = true;
        }
        private void OnClickEvent6(GameObject obj)
        {
            InputNum(obj); _canClickOperator = true;
        }
        private void OnClickEvent7(GameObject obj)
        {
            InputNum(obj); _canClickOperator = true;
        }
        private void OnClickEvent8(GameObject obj)
        {
            InputNum(obj); _canClickOperator = true;
        }
        private void OnClickEvent9(GameObject obj)
        {
            InputNum(obj);
            _canClickOperator = true;
        }
        private void InputNum(GameObject obj) 
        {
            //if (bb.Length > 8) 
            //{
            //    return;
            //}
            BtnPlaySound();
            
            _canClickZero = true;
            bb = bb + obj.name.ToString();
           
            if (allnumandOpera.Length > 9) 
            {
                return;
            }
            
            allnumandOpera = allnumandOpera + obj.name.ToString();
           jstext.text = allnumandOpera;
            //if (obj.name != "0") 
            //{
                
            //}
        }
        private void OnClickAdd(GameObject obj) 
        {
            if (_canClickOperator) 
            {
                OperatorNum++;
                BtnPlaySound();
                _canClickOperator = false;
               // bb = " ";
               // jstext.text = " ";  
              
                //if (OperatorNum > 1) 
                //{

               // jstext.text = sum1;
               // }
               // allnumandOpera = allnumandOpera + "+";
                InputNum(obj);
            }
           

        }
        private void OnClickjian(GameObject obj)
        {
            if (_canClickOperator) 
            {
                BtnPlaySound();
                _canClickOperator = false;
                // bb = " ";
                //  jstext.text = " ";

                // allnumandOpera = allnumandOpera + "-";
                InputNum(obj);
            }
         
        }
        private void OnClickcheng(GameObject obj)
        {
            if (_canClickOperator) 
            {
                BtnPlaySound();
                _canClickOperator = false;
                //  bb = " ";
                // jstext.text = " ";

                //allnumandOpera = allnumandOpera + "*";
                InputNum(obj);
            }
         
        }
        private void OnClickchu(GameObject obj)
        {

            if (_canClickOperator) 
            {
                BtnPlaySound();
                _canClickOperator = false;
                // bb = " ";
                //jstext.text = " ";


                // allnumandOpera = allnumandOpera + "/";
                InputNum(obj);
            }
           
        }
        private void OnClickchongzhi(GameObject obj)
        {
            BtnPlaySound();
           
            _canClickOperator = false;
            _canClickZero = false;
            jstext.text = " ";
            bb = "";
            allnumandOpera = "";
            sum = "0";
        }
        private void OnClickDengyu(GameObject obj) 
        {
            
            if (_canClickOperator) 
            {
                BtnPlaySound();

                // _canClickOperator = false;
                sum = new DataTable().Compute(allnumandOpera, null).ToString();
                double result = double.Parse(sum);
                sum1 = Math.Round(result, 5).ToString();//四舍五入
                allnumandOpera = sum1;
                bb = " ";
                jstext.text = " ";
              //  allnumandOpera = " ";
              
              

                if (sum1.Length > 9)
                {
                    sum1 = sum1.Substring(0, 9);//限制字符
                }
                jstext.text = sum1;
               // JulLevel();


            }

        }
        private void JulLevel(GameObject obj) 
        {
            if (LevelNum == 0)
            {

                if (sum1 == "1.66667")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    OnClick.GetChild(16).gameObject.SetActive(false);
               
                  
                    WaitTime(0.8f,()=> {
                        a.SetBool("isplay", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);

                    });
                   

                    
       
                    WaitTime(3f, () => 
                    { 
                        a.SetBool("isplay", false);
                      
                        SoundManager.instance.ShowVoiceBtn(true);
                      
                    });
                }
                else
                {

                    OnClick.GetChild(16).gameObject.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    WaitTime(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2), () => { OnClick.GetChild(16).gameObject.SetActive(true); });
                }
            }
            else if (LevelNum == 1) 
            {

                if (sum1 == "0.6")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                  
                    OnClick.GetChild(16).gameObject.SetActive(false);
                    WaitTime(0.8f,()=> {

                        b.SetBool("isplay", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                    });
          
                    
                   
                    WaitTime(3f, () =>
                    {
                        b.SetBool("isplay", false);
                    
                        SoundManager.instance.ShowVoiceBtn(true);
                      
                    });
                }
                else
                {

                    OnClick.GetChild(16).gameObject.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    WaitTime(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2), () => { OnClick.GetChild(16).gameObject.SetActive(true); });
                }
            }
            else if (LevelNum == 2)
            {

                if (sum1 == "1")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                   
                    OnClick.GetChild(16).gameObject.SetActive(false);
                    WaitTime(0.8f,()=> 
                    {

                        c.SetBool("isplay", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                    });
               
             
                    
                    WaitTime(3f, () =>
                    {
                        c.SetBool("isplay", false);
                
                        SoundManager.instance.ShowVoiceBtn(true);
                      
                    });
                }
                else
                {

                    OnClick.GetChild(16).gameObject.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,1, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    WaitTime(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2), () => { OnClick.GetChild(16).gameObject.SetActive(true); });
                }
            }
            else if (LevelNum == 3)
            {

                if (sum1 == "0.3")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                
                    OnClick.GetChild(16).gameObject.SetActive(false);
                    d.SetBool("isplay", true);
                    WaitTime(0.8f,()=> 
                    {
                       

                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                    });
                  
                   
                    WaitTime(3f, () =>
                    {
                        d.SetBool("isplay", false);
            
                        SoundManager.instance.ShowVoiceBtn(true);
                      
                    });
                }
                else
                {
                    OnClick.GetChild(16).gameObject.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    WaitTime(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2), () => { OnClick.GetChild(16).gameObject.SetActive(true); });
                }
            }
            else if (LevelNum == 4)
            {

                if (sum1 == "0.06667")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
               
                    OnClick.GetChild(16).gameObject.SetActive(false);
                    e.SetBool("isplay", true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                    WaitTime(0.8f,()=> {
                    

                      
                    });
       
          
                    
                    WaitTime(3f, () =>
                    {
                        e.SetBool("isplay", false);
                
                        SoundManager.instance.ShowVoiceBtn(false);
                        WaitTime(1.5f, () => { });  
                      
                    });
                }
                else
                {
                    OnClick.GetChild(16).gameObject.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    WaitTime(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2),()=> { OnClick.GetChild(16).gameObject.SetActive(true); });
                }
            }
        }
    }
}
