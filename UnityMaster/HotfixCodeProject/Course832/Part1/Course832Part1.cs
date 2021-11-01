using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course832Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
        private GameObject bellYingZi;
       
        private RawImage _xuBg;
        private GameObject _spine1;   //三张卡牌Spine
        private GameObject _spine2; 

        private Transform _onClick1;  //对于三张卡牌点击的Parent
        private GameObject _mask;


        private List<string> _isOvers;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            bell = curTrans.Find("bells/bell").gameObject;
         
            _xuBg = curTrans.Find("Bg/xuBg").GetComponent<RawImage>();
            _spine1 = curTrans.Find("Spines/Spine1").gameObject;
            _spine2 = curTrans.Find("Spines/Spine2").gameObject;
            _onClick1 = curTrans.Find("OnClicks/OnClick1");
            bellYingZi = curTrans.GetGameObject("bells/bellYingZi");
            _mask = curTrans.Find("Mask").gameObject;

            GameInit();
            GameStart();
        }



        void GameInit()
        {
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            _xuBg.gameObject.Show();
            _xuBg.transform.GetRawImage().color = new Color(1, 1, 1, 1);
       
            _spine1.Hide();
            _spine2.Hide();
            _onClick1.gameObject.Hide();
            _mask.Hide();
            bell.Show();
            bellYingZi.Show();
            SetBellPos(new Vector2(960, 150));
            _isOvers = new List<string>();
        }

        void GameStart()
        {
            AddOnClickEvents();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 0, null, () => {
              
                BgAni();
                bell.Hide();
                bellYingZi.Hide();
                _spine1.Show();
                SpineManager.instance.DoAnimation(_spine1, "animation", false);
                _onClick1.gameObject.Show();
            }));
            
        }

      
        /// <summary>
        /// 背景模糊动画
        /// </summary>
        private void BgAni()
        {
            var tweener1 = _xuBg.DOColor(new Color(1, 1, 1, 0), 0.5f);

            DOTween.Sequence().Append(tweener1).                            
                              AppendCallback(() =>
                              {
                                  _xuBg.gameObject.Hide();
                              });
        }


        private void SetBellPos(Vector2 pos)
        {
            bell.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y);
            bellYingZi.transform.GetRectTransform().anchoredPosition = new Vector2(pos.x, pos.y);
        }


        #region 添加Add点击事件

        private void AddOnClickEvents()
        {
            for (int i = 0; i < _onClick1.childCount; i++)
            {
                var child = _onClick1.GetChild(i).gameObject;
                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = OnClick1Item;
            }
        }

        private void OnClick1Item(GameObject go)
        {
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
            var name = go.name;
            _mask.Show();

             bool isContains= _isOvers.Contains(name);
            if (!isContains)
                _isOvers.Add(name);

            SpineManager.instance.DoAnimation(_spine1, name, false,                
             () => 
             {
               switch (name)
               {
                   case "1":
                         SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);

                         mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,1,
                             ()=> {
                                 SpineManager.instance.DoAnimation(_spine1, "a", false, () => {
                                     SpineManager.instance.DoAnimation(_spine1,"a2",false);
                                 });
                             },
                             ()=> {
                                 SpineManager.instance.DoAnimation(_spine1, "animation", false);
                                 _mask.Hide();

                                 if (_isOvers.Count == _onClick1.childCount)
                                 {  SoundManager.instance.ShowVoiceBtn(true); }                                                           
                             }));
                
                         mono.StartCoroutine(Delay(2.0f,()=>{
                             SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                         }));
                       break;
                   case "2":

                         SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, true);
                         var time1 = SpineManager.instance.DoAnimation(_spine1, "b", false);
                         var time2 = SpineManager.instance.DoAnimation(_spine1, "b2", false);

                         var time = time1 + time2;
                         mono.StartCoroutine(Delay(time,()=> {
                             SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                         }));
                         
                   

                         mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,2,
                             ()=> {
                                 SpineManager.instance.DoAnimation(_spine1, "b", false, () => {
                                     SpineManager.instance.DoAnimation(_spine1, "b2", false);
                                 });
                             },
                             ()=> {
                                 SpineManager.instance.DoAnimation(_spine1, "animation", false);
                                 _mask.Hide();
                                 if (_isOvers.Count == _onClick1.childCount)
                                 { SoundManager.instance.ShowVoiceBtn(true); }
                             }));
                         break;
                   case "3":
                         var timec = SpineManager.instance.DoAnimation(_spine1, "c", false);
                         SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, true);

                         mono.StartCoroutine(Delay(timec,()=> {
                             SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                         }));

                         mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,3,
                         ()=> { SpineManager.instance.DoAnimation(_spine1, "c", false,()=> {
                             mono.StartCoroutine(Delay(0.5f,()=> {

                                 SpineManager.instance.DoAnimation(_spine1, "animation", false);
                                 _mask.Hide();
                                 if (_isOvers.Count == _onClick1.childCount)
                                 {  SoundManager.instance.ShowVoiceBtn(true); }
                             }));
                            
                         }); }, null));
                         break;

               }
             });                  
        }
         
        #endregion


       
       


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
            switch (talkIndex)
            {
                case 1:
                 //   SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, true);
                    SetBellPos(new Vector2(214,-54));
                    bell.Show();
                    bellYingZi.Show();
                    _spine1.Hide();
                    _onClick1.gameObject.Hide();
                    _spine2.Show();
                    SpineManager.instance.DoAnimation(_spine2, "animation", false,()=> {
                       // SpineManager.instance.DoAnimation(_spine2, "animation2", true);
                    });
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 4, null, 
                       null// () => { SoundManager.instance.ShowVoiceBtn(true); }
                        ));
                    break;
                case 2:
                   // mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 0));
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
