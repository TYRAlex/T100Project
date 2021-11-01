using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace ILFramework.HotClass
{
    public class Course718Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
         
        private GameObject _spine21;
        private GameObject _spine22;
        private GameObject _mask;
        private Transform _onClicks;
        private GameObject _imgHei;
    
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
       
            bell = curTrans.Find("bell").gameObject;                              
            _spine21 = curTrans.Find("Spines/21").gameObject;
            _spine22 = curTrans.Find("Spines/22").gameObject;
            _mask = curTrans.Find("mask").gameObject;
            _onClicks = curTrans.Find("OnClicks");
            _imgHei = curTrans.GetGameObject("Bgs/1/img");
            GameStart();
        }

        void GameStart()
        {
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            _imgHei.Show();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
          
            bell.Show(); _spine21.Hide(); _spine22.Hide(); _onClicks.gameObject.Hide();
            AddOnClickEvents();
           
          


            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1,true);
          
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 0, 
            () => { _mask.Show(); },
            () => { SoundManager.instance.ShowVoiceBtn(true);}));
        }

        #region 点击事件

        private void AddOnClickEvents()
        {
            for (int i = 0; i < _onClicks.childCount; i++)
                Util.AddBtnClick(_onClicks.GetChild(i).gameObject, OnClick);
        }

        private void OnClick(GameObject go)
        {

            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            _mask.SetActive(true);
            SpineManager.instance.DoAnimation(_spine22, go.name, false, () => {
                _spine22.SetActive(false);
                _spine21.SetActive(true);
                _imgHei.Hide();
                switch (go.name)
                {
                    case "1":
                        OnClickOneFixedPulley();                       
                        break;
                    case "2":
                        OnClickOnePulleyBlock();                      
                        break;
                    case "3":
                        OnClickNinePulleyBlock();                     
                        break;
                    case "4":
                        OnClickTwentyPulleyBlock();                   
                        break;
                }
            });
        }


        /// <summary>
        /// 点击一个定滑轮卡片
        /// </summary>
        private void OnClickOneFixedPulley()
        {
            SpineManager.instance.DoAnimation(_spine21, "cx1", false, One);

            void One()
            {
                SpineManager.instance.DoAnimation(_spine21, "daiji", false, Two);
            }

            void Two()
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                SpineManager.instance.DoAnimation(_spine21, "la1", false, Three);                                          
            }

            void Three()
            {
                SpineManager.instance.DoAnimation(_spine21, "las1", true);
                var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                mono.StartCoroutine(Delay(time, Four));           
            }

            void Four()
            {
              
                SpineManager.instance.DoAnimation(_spine21, "xs1", false, ()=> { Reset("5"); });
            }
         
        }

        /// <summary>
        /// 点击一组滑轮组
        /// </summary>
        private void OnClickOnePulleyBlock()
        {
            SpineManager.instance.DoAnimation(_spine21, "cx2", false, One);

            void One() { SpineManager.instance.DoAnimation(_spine21, "daiji2", false, Two); }
          
            void Two()
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                SpineManager.instance.DoAnimation(_spine21, "la2", false, Three);                                          
            }

            void Three()
            {
                SpineManager.instance.DoAnimation(_spine21, "las2", true);
                var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                mono.StartCoroutine(Delay(time,() => { Four(); }));
            }

            void Four()
            {
               
                SpineManager.instance.DoAnimation(_spine21, "xs2", false, () => { Reset("6"); });
            }
        }

        /// <summary>
        /// 点击九组滑轮组
        /// </summary>
        private void OnClickNinePulleyBlock()
        {
            SpineManager.instance.DoAnimation(_spine21, "cx3", false,One);

            void One() { SpineManager.instance.DoAnimation(_spine21, "daiji3", false, Two); }
           
            void Two()
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                SpineManager.instance.DoAnimation(_spine21, "la3", false, Three);
                                       
            }
            void Three()
            {
                SpineManager.instance.DoAnimation(_spine21, "las3", true);
                var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                mono.StartCoroutine(Delay(time,() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                    SpineManager.instance.DoAnimation(_spine21, "las3z", false, Four);
                }));
            }

            void Four()
            {
               
                SpineManager.instance.DoAnimation(_spine21, "xs3", false, () => {
                   
                    Reset("7"); });
            }

        }

        /// <summary>
        /// 点击二十组滑轮组
        /// </summary>
        private void OnClickTwentyPulleyBlock()
        {
            SpineManager.instance.DoAnimation(_spine21, "cx4", false, One);

            void One() { SpineManager.instance.DoAnimation(_spine21, "daiji4", false, Two); }
     
            void Two()
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                SpineManager.instance.DoAnimation(_spine21, "la4", false, Three);                                
            }

            void Three()
            {
                SpineManager.instance.DoAnimation(_spine21, "las4", true);
                var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);

                mono.StartCoroutine(Delay(time, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6);
                    SpineManager.instance.DoAnimation(_spine21, "las4z", false, Four);
                }));
            }

            void Four()
            {
               
                SpineManager.instance.DoAnimation(_spine21, "xs4", false, () => {
                   
                    Reset("8"); });
            }
        }
        #endregion



        void Reset(string name)
        {
            _imgHei.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
            SpineManager.instance.DoAnimation(_spine22, name, false);
            _spine21.Hide();        
            _spine22.Show();
            _mask.Hide();
        }


        IEnumerator Delay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }


        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");

            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);

            switch(talkIndex)
            {
                case 1:
                    bell.Hide(); _spine22.Show(); _onClicks.gameObject.Show();_mask.Hide();
                    SpineManager.instance.DoAnimation(_spine22, "d", false);
                    break;

                case 2:
                    break;
            }

          
            talkIndex++;
        }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}
