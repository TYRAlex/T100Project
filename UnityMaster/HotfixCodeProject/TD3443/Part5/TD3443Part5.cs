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

    public class TD3443Part5
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



        private GameObject _dDD;



        private GameObject _sDD;




        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private Transform MoveBg;
        private Transform bg1;
        private Transform bg2;
        private Transform bg3;
        private Transform bg4;
        private Vector3 startPos;
        private Vector3 xunhuan;
        private Vector3 startPos2;
        private Vector3 xunhuan2;
        private float speed; private float speed2;
        private Transform swap;
        private Transform grabeCount;
        private Transform fishCount;

        private List<Transform> gb;
        private List<Transform> fish;
        private List<Vector3> fishEndPosList;
        private List<Vector3> gbEndPosList;
        private List<Vector3> StartPosList;
        private List<Vector3> FishStartPosList;
        private int OnClickindex;
        private Transform shoujiqi;

        private List<Vector3> randomV3List;
        private List<Vector3> randomgbV3List;
        private Transform lifeBar;
        private Transform lifeBar2;
        private float FillAmount;
        private float FillAmount2;

        private Transform BG;
        private Transform qipao1;
        private Transform qipao2;
        private Transform qipao3;
        private Transform paopao;
        private Transform qst;
      
        private Transform pingzi;
        private Transform effect;

        private bool _isPlaying;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");

            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");


            _dDD = curTrans.GetGameObject("dDD");



            _sDD = curTrans.GetGameObject("sDD");

            MoveBg = curTrans.Find("MoveBg");
            bg1 = curTrans.Find("MoveBg/bg1");
            bg2 = curTrans.Find("MoveBg/bg2");
            bg3 = curTrans.Find("MoveBg/bg3");
            bg4 = curTrans.Find("MoveBg/bg4");

            startPos = new Vector3(0, 540, 0);
            xunhuan = new Vector3(-1920, 540, 0);
            startPos2 = new Vector3(-60, 266.2f);
            xunhuan2 = new Vector3(-1920, 266.2f);
            // startPos = bg1.transform.position;//(960,540,0)
            // xunhuan = bg2.transform.position;//(-960,540,0)
           // startPos2 = bg3.transform.position;//(963.4f,266.2f)
           // xunhuan2 = bg4.transform.position;//(-924.7,266.2f)
           
         
            speed = Screen.width / 700f;
            speed2 = Screen.width / 1500f;
            grabeCount = curTrans.Find("gb");
            fishCount = curTrans.Find("fish");


            lifeBar = curTrans.Find("pingzi/lifeBar");
            lifeBar2 = curTrans.Find("pingzi/lifeBar2");
            randomV3List = new List<Vector3>(); randomgbV3List = new List<Vector3>();
            FillAmount = 0;
            FillAmount2 = 0.05f;
            OnClickindex = 0;
            shoujiqi = curTrans.Find("1");
            BG = curTrans.Find("BG/bg");
            qipao1 = curTrans.Find("qipao1");
            qipao2 = curTrans.Find("qipao2");
            qipao3= curTrans.Find("qipao3");
            paopao = curTrans.Find("paopao");
            qst = curTrans.Find("qst");

            pingzi = curTrans.Find("pingzi");
            effect = curTrans.Find("effect");
          
            GameInit();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = true;
            gb = new List<Transform>();
            for (int i = 0; i < grabeCount.childCount; i++)
            {
                gb.Add(grabeCount.GetChild(i).transform);
            }
            fish = new List<Transform>();
            for (int i = 0; i < fishCount.childCount; i++)
            {
                fish.Add(fishCount.GetChild(i).transform);
            }
            StartPosList = new List<Vector3>();
            //for (int i = 0; i < grabeCount.childCount; i++)
            //{
            //    StartPosList.Add(grabeCount.GetChild(i).transform.localPosition);
            //}
            FishStartPosList = new List<Vector3>();
            AddFishListPos();
            AddGrabeListPos();
            //for (int i = 0; i < fishCount.childCount; i++)
            //{
            //    FishStartPosList.Add(fishCount.GetChild(i).transform.localPosition);
            //}
            FillAmount = 0;
            FillAmount2 = 0.05f;
            bg1.transform.localPosition = new Vector2(0,0);
            bg2.transform.localPosition = new Vector2(-1920, 0);
            bg3.transform.localPosition = new Vector2(3.4f, -286);
            bg4.transform.localPosition = new Vector2(-1884.65f, -286);
            pingzi.GetRectTransform().anchoredPosition = new Vector2(838,195.87f);
            paopao.GetRectTransform().anchoredPosition = new Vector2(-32,-68);
            paopao.transform.localScale = new Vector3(0.2f,0.2f,1);
            qst.GetRectTransform().anchoredPosition = new Vector2(0,812);
            lifeBar.GetComponent<Image>().fillAmount = 0; lifeBar2.GetComponent<Image>().fillAmount = 0;
            paopao.transform.rotation = Quaternion.Euler(0, 0, 0);
            qst.transform.rotation = Quaternion.Euler(0,0,0);
            pingzi.gameObject.SetActive(true);
            pingzi.transform.rotation = Quaternion.Euler(0, 0, 0);
            shoujiqi.gameObject.SetActive(true);
            speed = Screen.width / 700f;
            speed2 = Screen.width / 1500f;
            OnClickindex = 0;
            for (int i=0;i<grabeCount.childCount;i++) 
            {
                grabeCount.GetChild(i).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(grabeCount.GetChild(i).GetChild(0).gameObject, grabeCount.GetChild(i).GetChild(0).name+"1", true);
                grabeCount.GetChild(i).localPosition = StartPosList[i];
              
            }
            for (int i = 0; i < fishCount.childCount; i++)
            {
                fishCount.GetChild(i).gameObject.SetActive(true);
                fishCount.GetChild(i).localPosition = FishStartPosList[i];
         
            }
            SpineManager.instance.DoAnimation(qipao1.gameObject,"kong",false);
            SpineManager.instance.DoAnimation(qipao2.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(qipao3.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(effect.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(qst.GetChild(0).gameObject,"qst",true);

            
            AddEndPosition();
            AddBtnOnClick();












            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

        }

        void GameInit()
        {
            DOTween.KillAll();
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();



            _dDD.Hide();



            _sDD.Hide();







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
                        _sDD.SetActive(true);
                        BellSpeck(_sDD, 0,null,ShowVoiceBtn,RoleType.Child);
            

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
                    BellSpeck(_sDD,1,null,()=> 
                    {
                        StartGame();
                        _sDD.SetActive(false);
                    },RoleType.Child);
                
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑

















        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            BgMove();
            Bg34Move();
  
            UpDate(true, 8f, () =>
              {
                  SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,1);
                  ThrowGb();
                  ThrowFish();
              });

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
                        _dDD.Show(); BellSpeck(_dDD, 2, null, null, RoleType.Child);
                        //ToDo...
                        //显示Middle角色并且说话  _dBD.Show(); BellSpeck(_dBD,0);						

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
            _successSpine.Show();
            PlayCommonSound(3);
            PlaySpine(_successSpine, "3-5-z", () => { PlaySpine(_successSpine, "3-5-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }

        private void BgMove()
        {
            UpDate(true, 0.01f, () =>
              {
             
                  bg1.transform.Translate(Vector3.right * speed, Space.World);
                  bg2.transform.Translate(Vector3.right * speed, Space.World);
                  if (bg2.transform.GetRectTransform().anchoredPosition.x > startPos.x)
                  {

                      bg1.transform.GetRectTransform().anchoredPosition = xunhuan;
                      swap = bg1;
                      bg1 = bg2;
                      bg2 = swap;
                  }
                //if (bg1.transform.position.x > startPos.x)
                //{

                //    bg2.transform.position = xunhuan;
                //    swap = bg2;
                //    bg2 = bg1;
                //    bg1 = swap;
                //}
                //if (bg2.transform.position == new Vector3(0, 0, 0))
                //{
                //    bg2.transform.position = new Vector3(-1920, 0, 0);
                //}
            });
        }
        private void Bg34Move()
        {
            UpDate(true, 0.01f, () =>
            {
                bg3.transform.Translate(Vector3.right * speed2, Space.World);
                bg4.transform.Translate(Vector3.right * speed2, Space.World);
                if (bg4.transform.GetRectTransform().anchoredPosition.x > startPos2.x)
                {

                    bg3.transform.GetRectTransform().anchoredPosition = xunhuan2;
                    swap = bg3;
                    bg3 = bg4;
                    bg4 = swap;
                }
                //if (bg1.transform.position.x > startPos.x)
                //{

                //    bg2.transform.position = xunhuan;
                //    swap = bg2;
                //    bg2 = bg1;
                //    bg1 = swap;
                //}
                //if (bg2.transform.position == new Vector3(0, 0, 0))
                //{
                //    bg2.transform.position = new Vector3(-1920, 0, 0);
                //}
            });
        }

        private void ThrowGb()
        {
            randomgbV3List = new List<Vector3>();
            for (int a = 0; a < Random.Range(2, 4); a++)
            {

                if (gb.Count == 0)
                {
                    return;
                }

                int i = Random.Range(0, gb.Count);

                gb[i].GetRectTransform().anchoredPosition = new Vector3(-47, -212, 0);
                gb[i].GetComponent<Empty4Raycast>().raycastTarget = true;
                string name1 = gb[i].name;


                Vector3 j = gbEndPosList[Random.Range(0, gbEndPosList.Count)];
                if (!randomgbV3List.Contains(j))
                {

                    randomgbV3List.Add(j);

                }
                else
                { 
                    j = gbEndPosList[Random.Range(0, gbEndPosList.Count)];
                }

                gb[i].GetRectTransform().DOAnchorPos(j, 4f).OnComplete(() =>
                {
                    //回数组

                    // Debug.Log("回到数组");
                    // gb[i].gameObject.SetActive(false);
                    if (gb[i].name == name1)
                    {
                        gb[i].GetComponent<Empty4Raycast>().raycastTarget = false;
                        gb[i].localPosition = StartPosList[i];
                    }



                });



                //  = output.GetRectTransform().anchoredPosition;
            }


        }
        private void ThrowFish()
        {
            randomV3List = new List<Vector3>();
            for (int a = 0; a < Random.Range(3, 4); a++)
            {
                if (fish.Count == 0)
                {

                    return;

                }
                int i = Random.Range(0, fish.Count);
                fish[i].GetRectTransform().anchoredPosition = new Vector3(-47, -212, 0);
                Vector3 j = fishEndPosList[Random.Range(0, fishEndPosList.Count)];
                if (!randomV3List.Contains(j))
                {

                    randomV3List.Add(j);

                }
                else
                {


                    j = fishEndPosList[Random.Range(0, fishEndPosList.Count)];
                }



                fish[i].GetRectTransform().DOAnchorPos(j, 5f).OnComplete(() =>
                {


                });
                fish.Remove(fish[i]);
            }
        }

        private void AddEndPosition()
        {
            fishEndPosList = new List<Vector3>();
            fishEndPosList.Add(new Vector3(-1090, -469));
            fishEndPosList.Add(new Vector3(1200, -534));
            fishEndPosList.Add(new Vector3(1250, -314));
            fishEndPosList.Add(new Vector3(-1069, 2));

            gbEndPosList = new List<Vector3>();

            gbEndPosList.Add(new Vector3(407, -831));
            gbEndPosList.Add(new Vector3(-9, -831));
            gbEndPosList.Add(new Vector3(-492, -831));
            gbEndPosList.Add(new Vector3(-248, -831));
            gbEndPosList.Add(new Vector3(200, -831));
            gbEndPosList.Add(new Vector3(50, -831));
        }
        private void AddBtnOnClick()
        {
            for (int i = 0; i < grabeCount.childCount; i++)
            {
                Util.AddBtnClick(grabeCount.GetChild(i).gameObject, OnClickEvent);
            }
        }
        private void OnClickEvent(GameObject obj)
        {
            SpineManager.instance.DoAnimation(shoujiqi.gameObject, "lenl1", true, () =>
            {

            });
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0);
            //868 517
            gb.Remove(obj.transform);
            FillAmount += 0.085f;
            FillAmount2 += 0.085f;
          
            //DOTween.KillAll();

            obj.transform.DOPause();
            obj.GetComponent<Empty4Raycast>().raycastTarget = false;

            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.transform.GetChild(0).name + "2", false, () =>
                {
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "guang", false, () =>
                    {
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "guang", true);
                        obj.transform.GetRectTransform().DOAnchorPos(new Vector2(868, -485), 2f).OnComplete(() =>
                        {
                            OnClickindex++;
                            obj.SetActive(false);
                            AddFillAmount(FillAmount);
                            AddFillAmount2(FillAmount2);
                            if (OnClickindex == 12)
                            {
                                //成功
                                //1.收集器去中间  旋转 发光 净化水
                                //2.背景变色
                                //3.泡泡出现 泡泡包裹潜水艇 潜水艇出
                                //成功动画
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,3);
                                RoatingPZ();
                                shoujiqi.gameObject.SetActive(false);
                                pingzi.GetRectTransform().DOAnchorPos(new Vector2(-19, 478), 1.5f).OnComplete(() =>
                                {
                                    SpineManager.instance.DoAnimation(effect.gameObject, "guang2", true);
                                });
                                Delay(7f, () =>
                                {
                                    SpineManager.instance.DoAnimation(effect.gameObject, "kong", false);
                                    pingzi.gameObject.SetActive(false);
                                    SpineManager.instance.DoAnimation(qst.GetChild(0).gameObject,"qst2",false,()=> 
                                    {
                                        SpineManager.instance.DoAnimation(qst.GetChild(0).gameObject,"qst3",false);
                                    });
                                    BG.GetComponent<RawImage>().texture = BG.GetComponent<BellSprites>().texture[0];
                                    speed = 0;
                                    speed2 = 0;
                                    SpineManager.instance.DoAnimation(qipao1.gameObject, "qipao");
                                    SpineManager.instance.DoAnimation(qipao2.gameObject, "qipao");
                                    SpineManager.instance.DoAnimation(qipao3.gameObject, "qipao");
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,2);
                                    paopao.GetRectTransform().DOAnchorPos(new Vector2(-75, 731), 7f).OnComplete(() =>
                                    {
                                      //  qst.SetParent(paopao);
                                        RoatingQST();
                                    });
                                    paopao.GetRectTransform().DOScale(1, 7f);
                                });




                            }
                            SpineManager.instance.DoAnimation(shoujiqi.gameObject, "lenl4", false);
                        });
                    });

                });
            
            
        }
        private void AddFillAmount(float amount) 
        {
            UpDate(true,0.05f,amount,()=> 
            {
                lifeBar.GetComponent<Image>().fillAmount += 0.01f;
              
            });
           

        }
        private void AddFillAmount2(float amount)
        {
            UpDate2(true, 0.05f, amount, () =>
            {
                lifeBar2.GetComponent<Image>().fillAmount += 0.01f;

            });
        }
        private void RoatingQST()
        {
           
            UpDate3(true, 0.01f, () => 
            {
               float RoateAngle = 2f;
               paopao.transform.Rotate(0, 0, RoateAngle);
                qst.transform.Rotate(0, 0, RoateAngle);

               // paopao.transform.rotation = Quaternion.Euler(new Vector3(0,0,RoateAngle));
                //qst.transform.rotation = Quaternion.Euler(new Vector3(0, 0, RoateAngle));
            });
            paopao.GetRectTransform().DOAnchorPos(new Vector2(800,1500),4f).OnComplete(()=> { GameSuccess(); });
            qst.GetRectTransform().DOAnchorPos(new Vector2(800, 1500), 4f).OnComplete(() => { GameSuccess(); });
        }
        private void RoatingPZ()
        {
            UpDate3(true, 0.01f, () =>
            {
              float RoateAngle = 2f;
                pingzi.transform.Rotate(0, 0, RoateAngle);
            });
        }
        private void AddFishListPos() 
        {
            FishStartPosList.Add(new Vector3(294,-86.9f));
            FishStartPosList.Add(new Vector3(323, -111));
            FishStartPosList.Add(new Vector3(285, -124));
            FishStartPosList.Add(new Vector3(307.7f, -86.9f));
            FishStartPosList.Add(new Vector3(271.85f, -62.9f));
            FishStartPosList.Add(new Vector3(280.7f, -100.45f));
            FishStartPosList.Add(new Vector3(280.7f, -97.38f));
            FishStartPosList.Add(new Vector3(235, -50));
            FishStartPosList.Add(new Vector3(248.85f, -86.9f));
            FishStartPosList.Add(new Vector3(294, -76.79f));
            FishStartPosList.Add(new Vector3(235, -97));
        }
        private void AddGrabeListPos() 
        {
            StartPosList.Add(new Vector3(282.95f, -75.84f));
            StartPosList.Add(new Vector3(207.7f, -111));
            StartPosList.Add(new Vector3(285f, -76.79f));
            StartPosList.Add(new Vector3(280.7f, -125.4f));
            StartPosList.Add(new Vector3(300f, -127.8f));
            StartPosList.Add(new Vector3(310.35f, -95f));
            StartPosList.Add(new Vector3(236.3f, -157.2f));
            StartPosList.Add(new Vector3(236.35f, -157.2f));
            StartPosList.Add(new Vector3(247.45f, -55.2f));
            StartPosList.Add(new Vector3(278.3f, -97f));
            StartPosList.Add(new Vector3(247.45f, -55.2f));
            StartPosList.Add(new Vector3(289.15f, -97f));
            StartPosList.Add(new Vector3(321.5f, -103.8f));
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

        private void UpDate(bool isStart, float delay,float amount, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isStart, delay,amount, callBack));
        }
        private void UpDate2(bool isStart, float delay, float amount, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate2(isStart, delay, amount, callBack));
        }
        private void UpDate(bool isStart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
        }
        private void UpDate3(bool isStart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate3(isStart, delay, callBack));
        }
        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        IEnumerator IEUpdate2(bool isStart, float delay, float amount, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
                if (amount - lifeBar2.GetComponent<Image>().fillAmount <= 0.01f)
                {
                    break;
                }
            }
        }
        IEnumerator IEUpdate(bool isStart, float delay, float amount, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
                if (amount - lifeBar.GetComponent<Image>().fillAmount <= 0.01f)
                {
                    break;
                }
            }
        }
        IEnumerator IEUpdate(bool isStart, float delay, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
                if (gb.Count == 0)
                {
                    break;
                }
            }
            if (gb.Count==0) 
            {
              
                yield break;
            }
        }
        IEnumerator IEUpdate3(bool isStart, float delay, Action callBack)
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
