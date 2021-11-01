using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
	
	public enum RoleType
	{
	   Bd,
       Xem,
       Child,
       Adult,		
	}

    public class TD5652Part5
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;


        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;




        private GameObject _dTT;



        private GameObject _sTT;



        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private Transform leftonClick1;
        private Transform rightonClick1;
        private Transform leftpanel1;
        private Transform rightpanel1;

        private Transform leftonClick2;
        private Transform rightonClick2;
        private Transform leftpanel2;
        private Transform rightpanel2;

        private Transform leftonClick3;
        private Transform rightonClick3;
        private Transform leftpanel3;
        private Transform rightpanel3;

        private List<Transform> LeftleveloneonClickList;
        private List<Transform> RightleveloneonClickList;
        private List<Transform> LeftleveltwoonClickList;
        private List<Transform> RightleveltwoonClickList;
        private List<Transform> LeftlevelthreeonClickList;
        private List<Transform> RightlevelthreeonClickList;
        private Transform _mask1;
        private Transform _mask2;
        private int rightindex;

        private GameObject gx1;
        private GameObject gx2;
        private GameObject gx3;
        private GameObject gx4;
        private GameObject hong;
        private GameObject effect;
        private GameObject effect2;
        private GameObject zha1;
        private GameObject zha2;
        private GameObject zha3;
        private Transform bg3;
        private Transform bg4;
        private Transform bg5;
        private Transform meng1;
        private Transform meng2;
        private Transform meng3;
        private Transform meng4;
        private Transform meng5;
        private Transform meng6;
        private bool _canClick;
        private Transform leftPanelPos;
        private Transform rightPanelPos;
        private Transform leftPanelPos2;
        private Transform rightPanelPos2;
        private int LevelIndex;
        private int FaultCount;
 


        private bool _isPlaying;

        void Start(object o)
        {
            Input.multiTouchEnabled = false;
            //   DOTween.Clear();
            DOTween.KillAll();
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");

            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");



            _dTT = curTrans.GetGameObject("mask/dTT");



            _sTT = curTrans.GetGameObject("mask/sTT");


            leftonClick1 = curTrans.Find("LeftL1");
            rightonClick1 = curTrans.Find("RightL1");
            leftpanel1 = curTrans.Find("Leftpanel1");
            rightpanel1 = curTrans.Find("Rightpanel1");


            leftonClick2 = curTrans.Find("LeftL2");
            rightonClick2= curTrans.Find("RightL2");
            leftpanel2 = curTrans.Find("Leftpanel2");
            rightpanel2 = curTrans.Find("Rightpanel2");

            leftonClick3 = curTrans.Find("LeftL3");
            rightonClick3 = curTrans.Find("RightL3");
            leftpanel3 = curTrans.Find("Leftpanel3");
            rightpanel3 = curTrans.Find("Rightpanel3");

            _mask1 = curTrans.Find("mask1");
            _mask2 = curTrans.Find("mask2");
            rightindex = 0;

            gx1 = curTrans.GetGameObject("gx1");
            gx2 = curTrans.GetGameObject("gx2");
            gx3 = curTrans.GetGameObject("gx3");
            gx4 = curTrans.GetGameObject("gx4");
            hong = curTrans.GetGameObject("hong");
            effect = curTrans.GetGameObject("effect");
            effect2 = curTrans.GetGameObject("effect2");
            zha1 = curTrans.GetGameObject("zha1");
            zha2 = curTrans.GetGameObject("zha2");
            zha3 = curTrans.GetGameObject("zha3");
            bg3 = curTrans.Find("bg3");
            bg4 = curTrans.Find("bg4");
            bg5 = curTrans.Find("bg5");
            meng1 = curTrans.Find("meng1");
            meng2 = curTrans.Find("meng2");
            meng3 = curTrans.Find("meng3");
            meng4 = curTrans.Find("meng4");
            meng5 = curTrans.Find("meng5");
            meng6 = curTrans.Find("meng6");
            leftPanelPos = curTrans.Find("LeftpanelPos");
            rightPanelPos = curTrans.Find("RightpanelPos");
            leftPanelPos2 = curTrans.Find("LeftpanelPos2");
            rightPanelPos2 = curTrans.Find("RightpanelPos2");
            _canClick = true;
            LevelIndex = 0;
            FaultCount = 0;


            GameInit();
            GameStart();
            //AddList();
            AddBtnClick();
        }

        void InitData()
        {
            _isPlaying = true;
















            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

        }

        void GameInit()
        {
            InitData();
            Replay();
            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();




            _dTT.Hide();



            _sTT.Hide();






            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

        }



        void GameStart()
        {
            _mask.Show(); _startSpine.Show();




            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        PlayBgm(0);//ToDo...改BmgIndex
                        _startSpine.Hide();

                        //ToDo...
                        _sTT.SetActive(true);
                        BellSpeck(_sTT, 0, null, ShowVoiceBtn, RoleType.Child);
                    //    StartGame();

                    });
                });
            });
        }


        void TalkClick()
        {
            HideVoiceBtn();
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sTT, 1, null,()=> { _mask.Hide(); }, RoleType.Child);
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑
        private void AddBtnClick() 
        {
            Util.AddBtnClick(leftonClick1.Find("a").gameObject, OnFaultEvent);
            Util.AddBtnClick(leftonClick1.Find("aa").gameObject,OnRightEvent);
            Util.AddBtnClick(leftonClick1.Find("ab").gameObject, OnRightEvent);
            Util.AddBtnClick(leftonClick1.Find("ac").gameObject, OnRightEvent);

            Util.AddBtnClick(rightonClick1.Find("b").gameObject, OnFaultEvent);
           Util.AddBtnClick(rightonClick1.Find("ba").gameObject, OnRightEvent);
           Util.AddBtnClick(rightonClick1.Find("bb").gameObject, OnRightEvent);
            Util.AddBtnClick(rightonClick1.Find("bc").gameObject, OnRightEvent);

            Util.AddBtnClick(leftonClick2.Find("a").gameObject, OnFaultEvent2);
            Util.AddBtnClick(leftonClick2.Find("aa").gameObject, OnRightEvent2);
            Util.AddBtnClick(leftonClick2.Find("ab").gameObject, OnRightEvent2);
            Util.AddBtnClick(leftonClick2.Find("ac").gameObject, OnRightEvent2);

            Util.AddBtnClick(rightonClick2.Find("b").gameObject, OnFaultEvent2);
            Util.AddBtnClick(rightonClick2.Find("ba").gameObject, OnRightEvent2);
            Util.AddBtnClick(rightonClick2.Find("bb").gameObject, OnRightEvent2);
            Util.AddBtnClick(rightonClick2.Find("bc").gameObject, OnRightEvent2);

            Util.AddBtnClick(leftonClick3.Find("a").gameObject, OnFaultEvent3);
            Util.AddBtnClick(leftonClick3.Find("aa").gameObject, OnRightEvent3);
            Util.AddBtnClick(leftonClick3.Find("ab").gameObject, OnRightEvent3);
            Util.AddBtnClick(leftonClick3.Find("ac").gameObject, OnRightEvent3);

            Util.AddBtnClick(rightonClick3.Find("b").gameObject, OnFaultEvent3);
            Util.AddBtnClick(rightonClick3.Find("ba").gameObject, OnRightEvent3);
            Util.AddBtnClick(rightonClick3.Find("bb").gameObject, OnRightEvent3);
            Util.AddBtnClick(rightonClick3.Find("bc").gameObject, OnRightEvent3);
        }
        private void OnRightEvent(GameObject obj)
        {
            if (_canClick&& (LeftleveloneonClickList.Contains(obj.transform)||RightleveloneonClickList.Contains(obj.transform)))
            {
                _canClick = false;
                FaultCount = 0;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,1,false);

                rightindex++;
                int ObjLayerIndex = obj.transform.GetSiblingIndex();
                if (obj.transform.parent.name == "LeftL1")
                {
                    SpineManager.instance.DoAnimation(gx1, "guang2", false,()=> 
                    {
                        _canClick = true;
                        if (rightindex == 3)
                        {
                            FaultCount = 0;
                            _canClick = false;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                            OkEffect();


                        }
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(gx3, "guang2", false,()=> 
                    {
                        _canClick = true;
                        if (rightindex == 3)
                        {
                            FaultCount = 0;
                            _canClick = false;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,5, false);
                            OkEffect();


                        }
                    });
                }

               // leftonClick1.GetChild(ObjLayerIndex).GetComponent<CustomImage>().raycastTarget = false;
              //  rightonClick1.GetChild(ObjLayerIndex).GetComponent<CustomImage>().raycastTarget = false;
                SpineManager.instance.DoAnimation(leftonClick1.GetChild(ObjLayerIndex).GetChild(0).gameObject, "61" + leftonClick1.GetChild(ObjLayerIndex).name + "2", false);
                SpineManager.instance.DoAnimation(rightonClick1.GetChild(ObjLayerIndex).GetChild(0).gameObject, "61" + rightonClick1.GetChild(ObjLayerIndex).name + "2", false);
                // SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "61" + obj.name + "2", false);
                LeftleveloneonClickList.Remove(leftonClick1.GetChild(ObjLayerIndex));
                RightleveloneonClickList.Remove(rightonClick1.GetChild(ObjLayerIndex));
              
            }
           
        }
        private void OnRightEvent2(GameObject obj)
        {
            if (_canClick&&(LeftleveltwoonClickList.Contains(obj.transform) || RightleveltwoonClickList.Contains(obj.transform)))
            {
                FaultCount = 0;
                _canClick = false; SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                rightindex++;
                int ObjLayerIndex = obj.transform.GetSiblingIndex();
                if (obj.transform.parent.name == "LeftL2")
                {
                    SpineManager.instance.DoAnimation(gx1, "guang2", false,()=> 
                    {
                        _canClick = true;
                        if (rightindex == 3)
                        {
                            FaultCount = 0;
                            _canClick = false;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                            OkEffect();
                            //   NexTEffect(leftonClick2, rightonClick2, leftpanel2, rightpanel2, _mask2);

                        }
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(gx3, "guang2", false,()=> 
                    { 
                        _canClick = true;
                        if (rightindex == 3)
                        {
                            FaultCount = 0;
                            _canClick = false;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                            OkEffect();
                            //   NexTEffect(leftonClick2, rightonClick2, leftpanel2, rightpanel2, _mask2);

                        }
                    });
                }

               // leftonClick2.GetChild(ObjLayerIndex).GetComponent<CustomImage>().raycastTarget = false;
               // rightonClick2.GetChild(ObjLayerIndex).GetComponent<CustomImage>().raycastTarget = false;
                SpineManager.instance.DoAnimation(leftonClick2.GetChild(ObjLayerIndex).GetChild(0).gameObject, "62" + leftonClick1.GetChild(ObjLayerIndex).name + "2", false);
                SpineManager.instance.DoAnimation(rightonClick2.GetChild(ObjLayerIndex).GetChild(0).gameObject, "62" + rightonClick1.GetChild(ObjLayerIndex).name + "2", false);
                LeftleveltwoonClickList.Remove(leftonClick2.GetChild(ObjLayerIndex));
                RightleveltwoonClickList.Remove(rightonClick2.GetChild(ObjLayerIndex));
             
            }
           
        }
        private void OnRightEvent3(GameObject obj)
        {
            if (_canClick && (LeftlevelthreeonClickList.Contains(obj.transform) || RightlevelthreeonClickList.Contains(obj.transform)))
            {
                FaultCount = 0;
                _canClick = false; SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                rightindex++;
                int ObjLayerIndex = obj.transform.GetSiblingIndex();
                if (obj.transform.parent.name == "LeftL3")
                {
                    SpineManager.instance.DoAnimation(gx1, "guang2", false,()=> 
                    { 
                        _canClick = true;
                        if (rightindex == 3)
                        {
                            FaultCount = 0;
                            _canClick = false;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                            OkEffect();
                            //NexTEffect(leftonClick3, rightonClick3, leftpanel3, rightpanel3, null);

                        }
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(gx3, "guang2", false,()=> 
                    { 
                        _canClick = true;
                        if (rightindex == 3)
                        {
                            FaultCount = 0;
                            _canClick = false;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                         //   SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                            OkEffect();
                            //NexTEffect(leftonClick3, rightonClick3, leftpanel3, rightpanel3, null);

                        }
                    });
                }

             //   leftonClick3.GetChild(ObjLayerIndex).GetComponent<CustomImage>().raycastTarget = false;
              //  rightonClick3.GetChild(ObjLayerIndex).GetComponent<CustomImage>().raycastTarget = false;
                SpineManager.instance.DoAnimation(leftonClick3.GetChild(ObjLayerIndex).GetChild(0).gameObject, "63" + leftonClick1.GetChild(ObjLayerIndex).name + "2", false);
                SpineManager.instance.DoAnimation(rightonClick3.GetChild(ObjLayerIndex).GetChild(0).gameObject, "63" + rightonClick1.GetChild(ObjLayerIndex).name + "2", false);
                LeftlevelthreeonClickList.Remove(leftonClick3.GetChild(ObjLayerIndex));
                RightlevelthreeonClickList.Remove(rightonClick3.GetChild(ObjLayerIndex));
             
            }
           
        }
        private void AddList() 
        {
           
            LeftleveloneonClickList = new List<Transform>();
            LeftleveloneonClickList.Add(leftonClick1.Find("aa"));
            LeftleveloneonClickList.Add(leftonClick1.Find("ab"));
            LeftleveloneonClickList.Add(leftonClick1.Find("ac"));
            RightleveloneonClickList = new List<Transform>();
            RightleveloneonClickList.Add(rightonClick1.Find("ba"));
            RightleveloneonClickList.Add(rightonClick1.Find("bb"));
            RightleveloneonClickList.Add(rightonClick1.Find("bc"));

            LeftleveltwoonClickList = new List<Transform>();
            LeftleveltwoonClickList.Add(leftonClick2.Find("aa"));
            LeftleveltwoonClickList.Add(leftonClick2.Find("ab"));
            LeftleveltwoonClickList.Add(leftonClick2.Find("ac"));
            RightleveltwoonClickList = new List<Transform>();
            RightleveltwoonClickList.Add(rightonClick2.Find("ba"));
            RightleveltwoonClickList.Add(rightonClick2.Find("bb"));
            RightleveltwoonClickList.Add(rightonClick2.Find("bc"));

            LeftlevelthreeonClickList = new List<Transform>();
            LeftlevelthreeonClickList.Add(leftonClick3.Find("aa"));
            LeftlevelthreeonClickList.Add(leftonClick3.Find("ab"));
            LeftlevelthreeonClickList.Add(leftonClick3.Find("ac"));
            RightlevelthreeonClickList = new List<Transform>();
            RightlevelthreeonClickList.Add(rightonClick3.Find("ba"));
            RightlevelthreeonClickList.Add(rightonClick3.Find("bb"));
            RightlevelthreeonClickList.Add(rightonClick3.Find("bc"));
        }
        private void OnFaultEvent(GameObject obj)
        {
            if (_canClick) 
            {
                FaultCount++;
                _canClick = false; SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                if (obj.transform.parent.name == "LeftL1")
                {
                    if (FaultCount == 3) 
                    {
                        FaultCount = 0;
                        RandomPlaySpine(LeftleveloneonClickList);
                    }
                  
                    SpineManager.instance.DoAnimation(gx2, "guang1", false,()=> 
                    {
                        _canClick = true;
                  
                    });
                }
                else
                {
                    if (FaultCount == 3)
                    {
                        FaultCount = 0;
                        RandomPlaySpine(RightleveloneonClickList);
                    }
                  
                    SpineManager.instance.DoAnimation(gx4, "guang1", false,()=> 
                    {
                        _canClick = true;
                       
                    });
                }

                
            }
           
        }
        private void OnFaultEvent2(GameObject obj)
        {
            if (_canClick) 
            {
                FaultCount++;
                _canClick = false; 
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                if (obj.transform.parent.name == "LeftL2")
                {
                    if (FaultCount == 3) 
                    {
                        FaultCount = 0;
                        RandomPlaySpine(LeftleveltwoonClickList);
                    }
                  
                    SpineManager.instance.DoAnimation(gx2, "guang1", false,()=> { _canClick = true; });
                }
                else
                {
                    if (FaultCount == 3)
                    {
                        FaultCount = 0;
                        RandomPlaySpine(RightleveltwoonClickList);
                    }
              
                    SpineManager.instance.DoAnimation(gx4, "guang1", false,()=> { _canClick = true; });
                }

              
            }
           
        }
        private void OnFaultEvent3(GameObject obj)
        {
            if (_canClick)
            {
                FaultCount++;
                _canClick = false; SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                if (obj.transform.parent.name == "LeftL3")
                {
                    if (FaultCount == 3)
                    {
                        FaultCount = 0;
                        RandomPlaySpine(LeftlevelthreeonClickList);
                    }
                   
                    SpineManager.instance.DoAnimation(gx2, "guang1", false,()=> { _canClick = true; });
                }
                else
                {
                    if (FaultCount == 3)
                    {
                        FaultCount = 0;
                        RandomPlaySpine(RightlevelthreeonClickList);
                    }
                    SpineManager.instance.DoAnimation(gx4, "guang1", false,()=> { _canClick = true; });
                }

                
            }
          
        }
        private void RandomPlaySpine(List<Transform>list) 
        {
            if (list.Count == 0) 
            {
                return;
            }
            int random = Random.Range(0, list.Count);
            SpineManager.instance.DoAnimation(list[random].GetChild(0).GetChild(0).gameObject, list[random].GetChild(0).GetChild(0).gameObject.name, false);
        }
        private void NexTEffect(Transform left, Transform right, Transform leftPanel, Transform rightPanel, Transform meng1Spine, Transform meng2Spine,Transform mask=null,Action method=null) 
        {
            rightindex = 0;

            meng1Spine.GetRectTransform().DOAnchorPosX(-1425,2.5f);
            meng2Spine.GetRectTransform().DOAnchorPosX(1362f, 2.5f);
            left.GetRectTransform().DOAnchorPosX(-271,2.5f);
            right.GetRectTransform().DOAnchorPosX(973, 2.5f);
            leftPanel.GetRectTransform().DOAnchorPosX(leftPanelPos2.GetRectTransform().anchoredPosition.x,2.5f);
            rightPanel.GetRectTransform().DOAnchorPosX(rightPanelPos2.GetRectTransform().anchoredPosition.x, 2.5f).OnComplete(()=> 
            {
                
                _canClick = true;
                method?.Invoke();
            });
            if (mask != null) 
            {
                _mono.StartCoroutine(ColorMove(mask));
            }
          
        }
        IEnumerator ColorMove(Transform mask) 
        {
            float temp = 0;
            while (true) 
            {
                temp += 2;
                mask.GetComponent<Image>().color = new Color(0,0,0,0.78f+(-0.78f)*temp/100);
                yield return new WaitForSeconds(0.02f);
                if (temp == 100) 
                {
                   // mask.gameObject.SetActive(false);
                    mask.GetComponent<Image>().raycastTarget = false;
                    
                    break;
                }
            }
            yield break;
        }
        private void OkEffect() 
        {
            _canClick = false;
            LevelIndex++;
            Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false), () =>
            {

            });
            SpineManager.instance.DoAnimation(hong,"guangxiao3",false,()=> 
            {
                SpineManager.instance.DoAnimation(effect,"gjxg1",false,()=> 
                {
                    if (LevelIndex == 1)
                    {
                      
                        SpineManager.instance.DoAnimation(zha1, "gjxg2", false,()=> 
                        {
                            //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0,false);
                            //bg4.Find("1/4").gameObject.SetActive(false);
                            //NexTEffect(leftonClick1, rightonClick1, leftpanel1, rightpanel1, meng1, meng2, _mask1);
                            Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false),()=> 
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
                                NexTEffect(leftonClick1, rightonClick1, leftpanel1, rightpanel1, meng1, meng2, _mask1);
                            });
                           
                            bg4.Find("1/4").gameObject.SetActive(false);
                          
                        });
                        
                    }
                    else if (LevelIndex == 2)
                    {
                     
                        SpineManager.instance.DoAnimation(zha2, "gjxg2", false,()=>
                        {
                            Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false),()=>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
                                NexTEffect(leftonClick2, rightonClick2, leftpanel2, rightpanel2, meng3, meng4, _mask2);
                            });
                           
                            bg4.Find("1/3").gameObject.SetActive(false);
                           
                        });
                     
                    }
                    else if (LevelIndex == 3)
                    {
                    
                        SpineManager.instance.DoAnimation(zha3, "gjxg2", false,()=> 
                        {
                            Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false),()=>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
                                NexTEffect(leftonClick3, rightonClick3, leftpanel3, rightpanel3, meng5, meng6, null, () => { Bg3Disapper(); hong.gameObject.SetActive(false); });
                                SpineManager.instance.DoAnimation(effect2, "guangxiao1", false);
                                EndingSpine();
                            });
                 
                            bg4.Find("1/2").gameObject.SetActive(false);
                          
                        });
                      
                        Delay(7f, () =>
                        {
                            GameSuccess();
                        });
                    }
                });
            });
        }
        private void Bg3Disapper() 
        {
            bg3.GetRectTransform().DOScale(2,0.5f);
        }
        private void Replay() 
        {
            LevelIndex = 0;
            FaultCount = 0;
            bg3.localScale = new Vector3(1,1,1);
            _mask1.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
            _mask2.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
            _mask1.GetComponent<Image>().raycastTarget = false;
            _mask2.GetComponent<Image>().raycastTarget = false;
            hong.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            effect.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            effect2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            zha1.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            zha2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            zha3.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            leftonClick1.Find("a").GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            leftonClick1.Find("aa").GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            leftonClick1.Find("ab").GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            leftonClick1.Find("ac").GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            rightonClick1.Find("b").GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            rightonClick1.Find("ba").GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            rightonClick1.Find("bb").GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            rightonClick1.Find("bc").GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            bg5.gameObject.SetActive(false);
            bg4.gameObject.SetActive(true); bg3.gameObject.SetActive(true);hong.SetActive(true);
            bg4.Find("1/2").gameObject.SetActive(true);
            bg4.Find("1/3").gameObject.SetActive(true);
            bg4.Find("1/4").gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(zha1.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(zha2.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(zha3.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(effect, "kong", false); SpineManager.instance.DoAnimation(effect2, "kong", false);
            SpineManager.instance.DoAnimation(gx1, "kong", false);
            SpineManager.instance.DoAnimation(gx2, "kong", false);
            SpineManager.instance.DoAnimation(gx3, "kong", false);
            SpineManager.instance.DoAnimation(gx4, "kong", false);
            SpineManager.instance.DoAnimation(hong, "kong", false);
            AddList();
            meng1.GetRectTransform().anchoredPosition = new Vector2(-378,475f);
            meng2.GetRectTransform().anchoredPosition = new Vector2(395.8f, 475f);
            meng3.GetRectTransform().anchoredPosition = new Vector2(-378, 475);
            meng4.GetRectTransform().anchoredPosition = new Vector2(393.8f, 475f);
            meng5.GetRectTransform().anchoredPosition = new Vector2(-378, 475);
            meng6.GetRectTransform().anchoredPosition = new Vector2(393.8f, 475f);
            leftonClick1.GetRectTransform().anchoredPosition = new Vector2(776,539);
            leftpanel1.GetRectTransform().anchoredPosition = leftPanelPos.GetRectTransform().anchoredPosition;
           // leftpanel1.position = new Vector2(141.5f, 131);
            SpineManager.instance.DoAnimation(leftonClick1.Find("aa/aa").gameObject,"61aa1",false);
            SpineManager.instance.DoAnimation(leftonClick1.Find("ab/ab").gameObject, "61ab1", false);
            SpineManager.instance.DoAnimation(leftonClick1.Find("ac/ac").gameObject, "61ac1", false);
            rightonClick1.GetRectTransform().anchoredPosition = new Vector2(9,539);
           rightpanel1.GetRectTransform().anchoredPosition = rightPanelPos.GetRectTransform().anchoredPosition;
            // rightpanel1.position = new Vector2(957.5f, 146.5f);
            SpineManager.instance.DoAnimation(rightonClick1.Find("ba/ba").gameObject, "61ba1", false);
            SpineManager.instance.DoAnimation(rightonClick1.Find("bb/bb").gameObject, "61bb1", false);
            SpineManager.instance.DoAnimation(rightonClick1.Find("bc/bc").gameObject, "61bc1", false);

            leftonClick2.GetRectTransform().anchoredPosition = new Vector2(776, 539);
            leftpanel2.GetRectTransform().anchoredPosition = leftPanelPos.GetRectTransform().anchoredPosition;
            SpineManager.instance.DoAnimation(leftonClick2.Find("aa/aa").gameObject, "62aa1", false);
            SpineManager.instance.DoAnimation(leftonClick2.Find("ab/ab").gameObject, "62ab1", false);
            SpineManager.instance.DoAnimation(leftonClick2.Find("ac/ac").gameObject, "62ac1", false);
            rightonClick2.GetRectTransform().anchoredPosition = new Vector2(9, 539);
            rightpanel2.GetRectTransform().anchoredPosition = rightPanelPos.GetRectTransform().anchoredPosition;
            SpineManager.instance.DoAnimation(rightonClick2.Find("ba/ba").gameObject, "62ba1", false);
            SpineManager.instance.DoAnimation(rightonClick2.Find("bb/bb").gameObject, "62bb1", false);
            SpineManager.instance.DoAnimation(rightonClick2.Find("bc/bc").gameObject, "62bc1", false);

            leftonClick3.GetRectTransform().anchoredPosition = new Vector2(776, 539);
            leftpanel3.GetRectTransform().anchoredPosition = leftPanelPos.GetRectTransform().anchoredPosition; ;
            SpineManager.instance.DoAnimation(leftonClick3.Find("aa/aa").gameObject, "63aa1", false);
            SpineManager.instance.DoAnimation(leftonClick3.Find("ab/ab").gameObject, "63ab1", false);
            SpineManager.instance.DoAnimation(leftonClick3.Find("ac/ac").gameObject, "63ac1", false);
            rightonClick3.GetRectTransform().anchoredPosition = new Vector2(9, 539);
            rightpanel3.GetRectTransform().anchoredPosition = rightPanelPos.GetRectTransform().anchoredPosition;
            SpineManager.instance.DoAnimation(rightonClick3.Find("ba/ba").gameObject, "63ba1", false);
            SpineManager.instance.DoAnimation(rightonClick3.Find("bb/bb").gameObject, "63bb1", false);
            SpineManager.instance.DoAnimation(rightonClick3.Find("bc/bc").gameObject, "63bc1", false);

            leftonClick1.Find("aa").GetComponent<CustomImage>().raycastTarget = true;
            leftonClick1.Find("ab").GetComponent<CustomImage>().raycastTarget = true;
            leftonClick1.Find("ac").GetComponent<CustomImage>().raycastTarget = true;

            rightonClick1.Find("ba").GetComponent<CustomImage>().raycastTarget = true;
            rightonClick1.Find("bb").GetComponent<CustomImage>().raycastTarget = true;
            rightonClick1.Find("bc").GetComponent<CustomImage>().raycastTarget = true;

            leftonClick2.Find("aa").GetComponent<CustomImage>().raycastTarget = true;
            leftonClick2.Find("ab").GetComponent<CustomImage>().raycastTarget = true;
            leftonClick2.Find("ac").GetComponent<CustomImage>().raycastTarget = true;

            rightonClick2.Find("ba").GetComponent<CustomImage>().raycastTarget = true;
            rightonClick2.Find("bb").GetComponent<CustomImage>().raycastTarget = true;
            rightonClick2.Find("bc").GetComponent<CustomImage>().raycastTarget = true;

            leftonClick3.Find("aa").GetComponent<CustomImage>().raycastTarget = true;
            leftonClick3.Find("ab").GetComponent<CustomImage>().raycastTarget = true;
            leftonClick3.Find("ac").GetComponent<CustomImage>().raycastTarget = true;

            rightonClick3.Find("ba").GetComponent<CustomImage>().raycastTarget = true;
            rightonClick3.Find("bb").GetComponent<CustomImage>().raycastTarget = true;
            rightonClick3.Find("bc").GetComponent<CustomImage>().raycastTarget = true;

          
        }


        private void EndingSpine() 
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,4);
            bg5.gameObject.SetActive(true);
            bg4.gameObject.SetActive(false);
          

        }
       






        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {


            _mask.Hide();       

			//测试代码记得删
			//Delay(4,GameSuccess);
        }

		
		

		
		
        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            _mask.Show();
            _replaySpine.Show();
			_okSpine.Show();
            _successSpine.Hide();
            PlaySpine(_replaySpine, "fh2", () => {
                AddEvent(_replaySpine, (go) => {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine); 
					RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () => {
                        _okSpine.Hide();
                        PlayBgm(0); //ToDo...改BmgIndex
                        GameInit();       
                        //ToDo...						
                        StartGame();
                        Replay();
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () => {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound();
					PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _replaySpine.Hide();

                        //ToDo...
                        //显示Middle角色并且说话  _dBD.Show(); BellSpeck(_dBD,0);						
                        _dTT.Show(); BellSpeck(_dTT, 2,null,null,RoleType.Child);
                    });
                });
            });

        }

        /// <summary>
        /// 游戏成功界面
        /// </summary>
        private void GameSuccess()
        {
            _mask.Show();
            _sTT.SetActive(false);
            _successSpine.Show();
            PlayCommonSound(3);			
			 PlaySpine(_successSpine, "3-5-z", () => { PlaySpine(_successSpine, "3-5-z2"); });         
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }


		

        #endregion

        #region 常用函数

        #region 语音按钮

        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }

        #endregion

        #region 隐藏和显示

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void HideChilds(Transform parent,int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }
        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
        }

        #endregion

        #region 拖拽相关

        /// <summary>
        /// 设置Drager回调
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="dragStart"></param>
        /// <param name="draging"></param>
        /// <param name="dragEnd"></param>
        /// <param name="onClick"></param>
        /// <returns></returns>
        private List<mILDrager> SetDragerCallBack(Transform parent, Action<Vector3, int, int> dragStart = null, Action<Vector3, int, int> draging = null, Action<Vector3, int, int, bool> dragEnd = null, Action<int> onClick = null)
        {
            var temp = new List<mILDrager>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var drager = parent.GetChild(i).GetComponent<mILDrager>();
                temp.Add(drager);
                drager.SetDragCallback(dragStart, draging, dragEnd, onClick);
            }

            return temp;
        }

        /// <summary>
        /// 设置Droper回调(失败)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="failCallBack"></param>
        /// <returns></returns>
        private List<mILDroper> SetDroperCallBack(Transform parent, Action<int> failCallBack = null)
        {
            var temp = new List<mILDroper>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var droper = parent.GetChild(i).GetComponent<mILDroper>();
                temp.Add(droper);
                droper.SetDropCallBack(null, null, failCallBack);
            }
            return temp;
        }


        #endregion
                 
        #region Spine相关

        private void InitSpines(Transform parent, bool isKong = true, Action initCallBack = null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;
                if (isKong)
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name); });
                else
                    PlaySpine(child, child.name);
            }
            initCallBack?.Invoke();
        }

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

        private GameObject FindGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        #endregion

        #region 音频相关

        private float PlayFailSound()
        {
            PlayCommonSound(5);

            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
            PlayCommonSound(4);
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private float PlayBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, isLoop);
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, isLoop);
            return time;
        }

        private float PlayCommonBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, index, isLoop);
            return time;
        }

        private float PlayCommonVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, index, isLoop);
            return time;
        }

        private float PlayCommonSound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, index, isLoop);
            return time;
        }

        private void StopAllAudio()
        {
            SoundManager.instance.StopAudio();
        }

        private void StopAudio(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
        }

        private void StopAudio(string audioName)
        {
            SoundManager.instance.Stop(audioName);
        }

        #endregion

        #region 延时相关

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        private void UpDate(bool isStart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
        }

        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        IEnumerator IEUpdate(bool isStart, float delay, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
            }
        }

        #endregion

        #region 停止协程

        private void StopAllCoroutines()
        {
            _mono.StopAllCoroutines();
        }

        private void StopCoroutines(string methodName)
        {
            _mono.StopCoroutine(methodName);
        }

        private void StopCoroutines(IEnumerator routine)
        {
            _mono.StopCoroutine(routine);
        }

        private void StopCoroutines(Coroutine routine)
        {
            _mono.StopCoroutine(routine);
        }

        #endregion

        #region Bell讲话

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Bd, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Bd, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

			switch(roleType)
			{
				case RoleType.Bd:
				     daiJi = "bd-daiji"; speak = "bd-speak";
				break;
				case RoleType.Xem:
				     daiJi = "daiji"; speak = "speak";
				break;
				case RoleType.Child:
				     daiJi = "animation"; speak = "animation2";
				 break;
				case RoleType.Adult:
				     daiJi = "daiji"; speak = "speak";
				break;
			}				
						 
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, daiJi);
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, speak);

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, daiJi);
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

        #region 监听相关

        private void AddEvents(Transform parent, PointerClickListener.VoidDelegate callBack)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                RemoveEvent(child);
                AddEvent(child, callBack);
            }
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }
        #endregion

        #region 修改Rect相关

        private void SetPos(RectTransform rect, Vector2 pos)
        {
            rect.anchoredPosition = pos;
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }

        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null)
        {
            rect.DOAnchorPos(v2, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            callBack1?.Invoke();
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack2?.Invoke(); });
        }


        #endregion

        #region 打字机
        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            _mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行        
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(0.1f);
                text.text += str[i];
                i++;
            }
            callBack?.Invoke();
            yield break;
        }
        #endregion

        #endregion

    }
}
