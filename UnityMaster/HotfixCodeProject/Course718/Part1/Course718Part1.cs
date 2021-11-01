using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course718Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;

        private GameObject _mask;
        private Button _maskBtn;

        private GameObject _pulleySpine;    //滑轮点击发亮Spine
        private Transform _pulleyOnClicks;

        private GameObject _11Spine;  //动滑轮介绍
        private GameObject _12Spine;  //定滑轮介绍
        private GameObject _13Spine;  //滑轮组介绍

        private GameObject _bgSpine;
        private GameObject _heiImg;
        private RectTransform _bellRect;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;       
            mono = curGo.GetComponent<MonoBehaviour>();

            bell = curTrans.Find("bell").gameObject;
            _bellRect = bell.transform.GetRectTransform();
            talkIndex = 1;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _pulleySpine = curTrans.Find("Spines/PulleySpine").gameObject;
            _pulleyOnClicks = curTrans.Find("OnClicks/PulleyOnClicks");
            _mask = curTrans.Find("mask").gameObject;
            _maskBtn = _mask.GetComponent<Button>();

            _heiImg = curTrans.GetGameObject("Bg/Image/Image");
            _bgSpine = curTrans.GetGameObject("Bg/BgSpine");
            _11Spine = curTrans.Find("Spines/11").gameObject;
            _12Spine = curTrans.Find("Spines/12").gameObject;
            _13Spine = curTrans.Find("Spines/13").gameObject;
            BellMoveAni(-54f, 0);
            GameStart();
        }

        void GameStart()
        {
            _maskBtn.onClick.RemoveAllListeners();
           mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            _bgSpine.Show();
            _heiImg.Hide();
            _11Spine.GetComponent<SkeletonGraphic>().color = new Color(0, 0, 0, 0);
            _12Spine.GetComponent<SkeletonGraphic>().color = new Color(0, 0, 0, 0);
            _13Spine.GetComponent<SkeletonGraphic>().color = new Color(0, 0, 0, 0);

            SpineManager.instance.DoAnimation(_11Spine,"3",false,()=> {
                _11Spine.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            });
            SpineManager.instance.DoAnimation(_12Spine, "3", false,()=> {
                _12Spine.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            });
            SpineManager.instance.DoAnimation(_13Spine, "3", false,()=> {
                _13Spine.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            });
        

            _maskBtn.onClick.RemoveAllListeners();


          

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            AddBtnEvents();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 0, ()=> {
                _mask.SetActive(true);
            }, () =>
            {
                _mask.SetActive(false);
                BellMoveAni(-454f, 1.0f);
            
            }));
        }

        private void BellMoveAni(float yEnd,float delay)
        {
            _bellRect.DOAnchorPosY(yEnd, delay);
        }

        #region 点击事件

        private void AddBtnEvents()
        {
            for (int i = 0; i < _pulleyOnClicks.childCount; i++)
                Util.AddBtnClick(_pulleyOnClicks.GetChild(i).gameObject, PulleyOnClick);
        }


        /// <summary>
        /// 滑轮点击
        /// </summary>
        /// <param name="go"></param>
        private void PulleyOnClick(GameObject go)
        {
            BtnPlaySound();
            _mask.SetActive(true);
          
            SpineManager.instance.DoAnimation(_pulleySpine, go.name, false,()=> {
                _heiImg.Show();
                _bgSpine.Hide();
                switch (go.name)
                {
                    case "animation6":
                    case "animation5":               //定滑轮                   
                        CommonOnClick(_12Spine, 1);
                        break;
                    case "animation4":               //滑轮组
                        CommonOnClick(_13Spine, 3);
                        break;
                    case "animation2":
                    case "animation3":
                        CommonOnClick(_11Spine, 2);//动滑轮               
                        break;
                }

            });
          
        }

        private void CommonOnClick(GameObject go, int index)
        {
            SpineManager.instance.DoAnimation(go, "2", false,()=> {
                
            });
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, index, null, End));

            void End()
            {
                SpineManager.instance.DoAnimation(go, "1", true,
                   () =>
                   {
                       _maskBtn.onClick.AddListener(OnClickMask);
                   });
            }

            void OnClickMask()
            {
                _heiImg.Hide();
                _bgSpine.Show();
                SpineManager.instance.DoAnimation(go, "3", false, () =>
                {
                  
                    _mask.SetActive(false);
                    _maskBtn.onClick.RemoveListener(OnClickMask);
                   
                });
            }
        }


        

        #endregion




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
            if (talkIndex == 1)
            {

            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}
