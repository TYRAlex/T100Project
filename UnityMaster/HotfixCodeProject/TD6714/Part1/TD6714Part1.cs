using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class TD6714Part1
    {
        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _mask;
        private GameObject _startBtn;
        private GameObject _restartBtn;
        private GameObject _okBtn;
        private GameObject _successSpine;
        private GameObject _spSpine;
       
        private GameObject _tT;
        private GameObject _dD;
        private GameObject _dTT;

      
        private GameObject _spines;
        private Transform _spine1;
        private Transform _spine2;


        private Transform _levelBg;
        private int _cutLevelId;
        private Transform _bigShuas;
        private GameObject _uiMask;

        private List<string> _levelNums;
        private List<string> _level2Nums;
        private List<string> _level3Nums;
        private List<string> _level4Nums;

        private List<int> _okSoundIds;
        private List<int> _errorSoundIds;

        private GameObject _f6Spine;
        private GameObject _l6Spine;
        private GameObject _r6Spine;
        private GameObject _c6Spine;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _startBtn = curTrans.GetGameObject("StartBtn");
            _restartBtn = curTrans.GetGameObject("RestartBtn");
            _okBtn = curTrans.GetGameObject("OkBtn");
            _successSpine = curTrans.GetGameObject("SuccessSpine");
            _spSpine = curTrans.GetGameObject("SuccessSpine/SpSpine");

            _tT = curTrans.GetGameObject("TT");
            _dD = curTrans.GetGameObject("DD");
            _dTT = curTrans.GetGameObject("DTT");
           

            _spines = curTrans.GetGameObject("Spines");
            _spine1 = curTrans.Find("Spines/Spine1");
            _spine2 = curTrans.Find("Spines/Spine2");

            _levelBg = curTrans.Find("UIs/LevelBg");
            _bigShuas = curTrans.Find("UIs/BigShua");

            _uiMask = curTrans.GetGameObject("UIMask");

            _f6Spine = curTrans.GetGameObject("Spines/Spine1/f6");
            _l6Spine = curTrans.GetGameObject("Spines/Spine2/l6");
            _r6Spine = curTrans.GetGameObject("Spines/Spine2/r6");
            _c6Spine = curTrans.GetGameObject("Spines/Spine2/c6");



            GameInit();

            GameStart();
        }





        private void GameInit()
        {
            InitDate();
            DOTween.KillAll();
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllCoroutines();
            StopAllAudio();

            ShowXingXingSpine(null, true);
            HideImg(_spine1.GetChild(0)); HideImg(_spine2.GetChild(0)); HideImg(_spine2.GetChild(1)); HideImg(_spine2.GetChild(2)); HideImg(_bigShuas);
            _successSpine.Hide(); _startBtn.Hide(); _spines.Hide(); _levelBg.gameObject.Hide(); _tT.Hide(); _dTT.Hide();_restartBtn.Hide();_okBtn.Hide();
            RemoveEvent(_startBtn); RemoveEvent(_restartBtn); RemoveEvent(_okBtn);
        
        }


        private void ShowXingXingSpine(GameObject go, bool isHideAll)
        {
            if (isHideAll)
            {
                _f6Spine.Hide();_l6Spine.Hide();
                _r6Spine.Hide();_c6Spine.Hide();
            }
            else
            {
                go.Show();
                PlaySpine(go, "kong",()=> {
                    PlaySpine(go, go.name,()=> {
                        Delay(0.5f, () => { go.Hide(); });
                       
                    });
                });
            }
        }

        private void ShowLevel()
        {

            Transform shuasTra = null;
           
            for (int i = 0; i < _levelBg.childCount; i++)
            {
                var name = "Level" + _cutLevelId;
                var child = _levelBg.GetChild(i);
                var tongs = child.Find("tongs");
                var tongSpines = child.Find("tongSpines");
             
              
            
                var spineName = tongSpines.GetChild(0).gameObject.name;
                if (name == child.name)
                {               
                    child.gameObject.Show();
                    var shuas = child.Find("shuas");
                    shuasTra = shuas;
                    PlaySpine(tongSpines.gameObject, spineName);
                    for (int j = 0; j < tongs.childCount; j++)
                    {
                        var tongchild = tongs.GetChild(j).gameObject;
                        RemoveEvent(tongchild);
                        AddEvent(tongchild, OnClickTong);
                    }
                }
                else
                {
                    child.gameObject.Hide();
                }
            }

            for (int k = 0; k < shuasTra.childCount; k++)
            {             
                shuasTra.GetChild(k).gameObject.Hide();
            }
               
        }


        /// <summary>
        /// 显示引导Spine
        /// </summary>
        private void ShowGuideSpine()
        {
            _uiMask.Show();
            Delay(1.0f, () => {
                PlayVoice(0);
                switch (_cutLevelId)
                {
                    case 1:   //房子
                        var go1 = _spine1.GetChild(0).gameObject;
                        PlaySpine(go1, "f", () => { PlaySpine(go1, "f2", () => { _uiMask.Hide(); }); });
                        break;
                    case 2:   //路灯
                        var go2 = _spine2.GetChild(0).gameObject;
                        PlaySpine(go2, "l", () => { PlaySpine(go2, "l2", () => { _uiMask.Hide(); }); });
                        break;
                    case 3:   //汽车
                        var go3 = _spine2.GetChild(2).gameObject;
                        PlaySpine(go3, "c", () => { PlaySpine(go3, "c2", () => { _uiMask.Hide(); }); });
                        break;
                    case 4:  //马路
                        var go4 = _spine2.GetChild(1).gameObject;
                        PlaySpine(go4, "r", () => { PlaySpine(go4, "r2", () => { _uiMask.Hide(); }); });
                        break;
                }
            });
        }

        private void HideImg(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {

                var child = parent.GetChild(i);
                child.gameObject.Hide();
                var img = child.GetImage();
                img.DOFade(0, 0);
            }
        }


        /// <summary>
        /// 显示刷子
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        private void ShowShua(GameObject spineGo, Transform parent, string name, Action<string,GameObject,string> findImgNameCallBack)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var shua = parent.GetChild(i);
                if (shua.name == name)
                { shua.gameObject.Show();
                    var imgName = shua.GetImage().sprite.name;

                    findImgNameCallBack.Invoke(imgName,spineGo, name);
                }

                else
                    shua.gameObject.Hide();
            }
        }

        /// <summary>
        /// 点击任务栏中的桶
        /// </summary>
        /// <param name="go"></param>
        private void OnClickTong(GameObject go)
        {
            _uiMask.Show();
            PlayOnClickSound();
            var name = go.name;
            var tongs = go.transform.parent;
            var curShuas = tongs.parent.Find("shuas");
            var spineGo = tongs.parent.Find("tongSpines").gameObject;
            ShowShua(spineGo,curShuas, name, ShowBigShua);

        }

        /// <summary>
        /// 显示大刷子
        /// </summary>
        /// <param name="imgName"></param>
        private void ShowBigShua(string imgName,GameObject spineGo,string onClickName)
        {
            PlaySpine(spineGo, onClickName);
            var bigName = "S" + imgName[1];
         
            switch (_cutLevelId)
            {
                case 1:
                    switch (bigName)
                    {
                        case "S8":
                        case "S7":
                            PlayFailSound();
                            Delay(1.0f,()=>{
                                _uiMask.Hide();
                            });
                           
                            //FangZiShuaAni(bigName, null, null, null, null, () => { _uiMask.Hide(); }, false);
                            break;
                        case "S9":
                            PlayVoice(1);
                            FangZiShuaAni(bigName, new List<string> { "F8", "F7" }, null, new List<string> { "F15", "F16" }, null, () => { FangZiOver(bigName); });
                            break;
                        case "S5":
                            PlayVoice(1);
                            FangZiShuaAni(bigName, new List<string> { "F10" }, new List<string> { "F9", "F3" }, new List<string> { "F18" }, new List<string> { "F17", "F11" }, () => { FangZiOver(bigName); });
                            break;
                        case "S4":
                            PlayVoice(1);
                            FangZiShuaAni(bigName, new List<string> { "F4" }, new List<string> { "F5", "F6" }, new List<string> { "F12" }, new List<string> { "F13", "F14" }, () => { FangZiOver(bigName); });
                            break;
                    }
                    break;
                case 2:
                    switch (bigName)
                    {
                        case "S2":
                            PlayFailSound();
                            Delay(1.0f, () => {
                                _uiMask.Hide();
                            });
                            //LuDengShuaAni(bigName, null, null, () => { _uiMask.Hide(); }, false);
                            break;
                        case "S9":
                            PlayVoice(1);
                            LuDengShuaAni(bigName, null, new List<string> { "L8", "L9", "L10", "L11" }, () => { LuDengOver(bigName); }, true);
                            break;
                        case "S5":
                            PlayVoice(1);
                            LuDengShuaAni(bigName, new List<string> { "L3" }, new List<string> { "L17", "L16", "L18", "L19", "L12", "L14", "L15" }, () => { LuDengOver(bigName); }, true);
                            break;
                        case "S4":
                            PlayVoice(1);
                            LuDengShuaAni(bigName, new List<string> { "L7" }, null, () => { LuDengOver(bigName); }, true);
                            break;
                        case "S6":
                            PlayVoice(1);
                            LuDengShuaAni(bigName, new List<string> { "L4", "L5", "L6" }, new List<string> { "L13" }, () => { LuDengOver(bigName); }, true);
                            break;
                    }
                    break;
                case 3:
                    PlayVoice(1);
                    switch (bigName)
                    {
                        case "S4":                          
                            CarShuaAni(bigName, new List<string>{ "C13" }, null, new List<string>{ "C11" }, () => { CarOver(bigName); });
                            break;
                        case "S9":
                            CarShuaAni(bigName, null, new List<string>{ "C16", "C15" }, null, () => { CarOver(bigName); });
                            break;
                        case "S6":
                            CarShuaAni(bigName, new List<string>{ "C6", "C4" }, null, new List<string>{ "C10", "C8" }, () => { CarOver(bigName); });
                            break;
                        case "S5":
                            CarShuaAni(bigName, new List<string>{ "C5" }, null, new List<string>{ "C9" }, () => { CarOver(bigName); });
                            break;
                        case "S1":
                            CarShuaAni(bigName, new List<string>{ "C7","C14" }, null, new List<string>{ "C3","C12", "C17" }, () => { CarOver(bigName); });
                            break;
                    }
                    break; 
                case 4:
                    switch (bigName)
                    {
                        case "S2":                          
                        case "S3":
                            PlayFailSound();
                            Delay(1.0f, () => {
                                _uiMask.Hide();
                            });
                            //   MaLuShuaAni(bigName, null, null, null, null,null,null, () => { _uiMask.Hide(); }, false);
                            break;
                        case "S9":
                            PlayVoice(1);
                            MaLuShuaAni(bigName, new List<string> {"R11"}, new List<string> { "R10"}, new List<string> { "R9"}, new List<string> { "R8"}, new List<string> { "R7","R6"}, new List<string> { "R5","R3","R4"}, () => { MaLuOver(bigName); });
                            break;
                        case "S5":
                            PlayVoice(1);
                            MaLuShuaAni(bigName, null, null, null, null, null, new List<string> { "R13"}, () => { MaLuOver(bigName); });
                            break;
                        case "S6":
                            PlayVoice(1);
                            MaLuShuaAni(bigName, null, null, null, null, null, new List<string> { "R12" }, () => { MaLuOver(bigName); });
                            break;
                    }
                    break;
            }

        }

        private void ShowSuccess()
        {
            _mask.Show(); SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3);
            _successSpine.Show();
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine,"sp"); });
            PlaySpine(_successSpine, "6-12-z", () => { PlaySpine(_successSpine, "6-12-z2",null,true); });

            ShowRestartAndOk();
        }

        /// <summary>
        /// 显示重玩和Ok按钮
        /// </summary>
        private void ShowRestartAndOk()
        {
            Delay(4, () => {
                _successSpine.Hide();
                _restartBtn.Show(); _okBtn.Show();
                PlaySpine(_restartBtn, "kong",()=> { PlaySpine(_restartBtn, "fh2",()=> { AddEvent(_restartBtn,OnClickRestartBtn); }); });
                PlaySpine(_okBtn, "kong", () => { PlaySpine(_okBtn, "ok2",()=> { AddEvent(_okBtn, OnClickOkBtn); }); });
            });
        }

        private void OnClickOkBtn(GameObject go)
        {
            PlayOnClickSound();
            RemoveEvent(_okBtn); RemoveEvent(_restartBtn);
            PlaySpine(_okBtn, "ok",()=> {
                _restartBtn.Hide(); _okBtn.Hide();
                PlayBgm(4, true, SoundManager.SoundType.COMMONBGM);
                _dTT.Show();
                BellSpeck(_dTT, 2);
            });
        }

        private void OnClickRestartBtn(GameObject go)
        {
            PlayOnClickSound();
            RemoveEvent(_okBtn); RemoveEvent(_restartBtn);
            PlaySpine(_restartBtn, "fh", () => {
                _restartBtn.Hide(); _okBtn.Hide();
                PlayBgm(2, true, SoundManager.SoundType.COMMONBGM);
                GameInit();
                _tT.Hide();
                _mask.Hide(); _spines.Show(); InitSpines(_spine1); InitSpines(_spine2); _levelBg.gameObject.Show();
                ShowLevel(); ShowGuideSpine();
            });        
        }



        /// <summary>
        /// 马路刷结束
        /// </summary>
        private void MaLuOver(string name)
        {
            _uiMask.Hide();
        
            var isCon = _level4Nums.Contains(name);
            if (!isCon)
                _level4Nums.Add(name);

            if (_level4Nums.Count == 3)
            {
                ShowXingXingSpine(_r6Spine, false);
                _uiMask.Show();
                Delay(3, ShowSuccess);
            }
        }

        /// <summary>
        /// 汽车刷结束
        /// </summary>
        private void CarOver(string name)
        {
            _uiMask.Hide();
          

            var isCon = _level3Nums.Contains(name);
            if (!isCon)
                _level3Nums.Add(name);

            if (_level3Nums.Count == 5)
            {
                ShowXingXingSpine(_c6Spine, false);
                _cutLevelId++;
                ShowLevel();
                ShowGuideSpine();
            }
        }

        /// <summary>
        /// 路灯刷结束
        /// </summary>
        private void LuDengOver(string name)
        {
            _uiMask.Hide();
           

            var isCon = _level2Nums.Contains(name);
            if (!isCon)
                _level2Nums.Add(name);

            if (_level2Nums.Count == 4)
            {
                ShowXingXingSpine(_l6Spine, false);
                _cutLevelId++;
                ShowLevel();
                ShowGuideSpine();
            }
        }

        /// <summary>
        /// 房子刷结束
        /// </summary>
        private void FangZiOver(string name)
        {
           
            _uiMask.Hide();
            var isCon = _levelNums.Contains(name);
            if (!isCon)
                _levelNums.Add(name);

            if (_levelNums.Count == 3)
            {
                ShowXingXingSpine(_f6Spine, false);
                _cutLevelId++;
                ShowLevel();
                ShowGuideSpine();
            }

        }


        /// <summary>
        /// 房子刷子Ani
        /// </summary>
        private void FangZiShuaAni(string bigName, List<string> str1 = null, List<string> str2 = null, List<string> str3 = null, List<string> str4 = null, Action callBack = null, bool isOk = true)
        {
            var curShua = _bigShuas.Find(bigName);
            var curRect = curShua.GetRectTransform();
            curRect.anchoredPosition = new Vector2(229, 364);
            curShua.gameObject.Show();
            curShua.transform.GetImage().DOFade(1, 0);

            if (bigName=="S5")
            {
                Delay(0.5f, () => { ShowCurImg(_spine1.GetChild(0), str1,0.5f); });
                Delay(1.5f, () => { ShowCurImg(_spine1.GetChild(0), str2, 0.5f); });
                Delay(2.5f, () => { ShowCurImg(_spine1.GetChild(0), str3, 0.5f); });
                Delay(3.5f, () => { ShowCurImg(_spine1.GetChild(0), str4, 0.5f); });
            }

            var tw1 = curRect.DOAnchorPos(new Vector2(229, 708), 1)
              .OnComplete(() => {
                  if (bigName!="S5")                                  
                  ShowCurImg(_spine1.GetChild(0), str1);

                  curRect.anchoredPosition = new Vector2(70, 718); });

            var tw2 = curRect.DOAnchorPos(new Vector2(70, 364), 1)
              .OnComplete(() => {
                  if (bigName != "S5")
                      ShowCurImg(_spine1.GetChild(0), str2);
                  curRect.anchoredPosition = new Vector2(-135, 364); });

            var tw3 = curRect.DOAnchorPos(new Vector2(-135, 615), 1)
                  .OnComplete(() => {
                      if (bigName != "S5")
                          ShowCurImg(_spine1.GetChild(0), str3);
                      curRect.anchoredPosition = new Vector2(-302, 615);
                      Delay(0.3f, () => { StopAudio(SoundManager.SoundType.VOICE); });
                  });

            var tw4 = curRect.DOAnchorPos(new Vector2(-302, 374), 1)
            .OnComplete(() => {
                if (bigName != "S5")
                    ShowCurImg(_spine1.GetChild(0), str4);
                curRect.anchoredPosition = new Vector2(-302, 615);  });

            var tw = DOTween.Sequence().Append(tw1)
                                       .Append(tw2)
                                       .Append(tw3)
                                       .Append(tw4)
                                       .OnComplete(() => {
                                          
                                           curShua.gameObject.Hide();
                                           curShua.transform.GetImage().DOFade(0, 0);
                                           if (isOk)
                                               PlaySuccessSound();
                                           else
                                               PlayFailSound();

                                           callBack?.Invoke();
                                       });
        }

        /// <summary>
        /// 路灯刷子Ani
        /// </summary>   
        private void LuDengShuaAni(string bigName, List<string> str1 = null, List<string> str2 = null, Action callBack = null, bool isOk = true)
        {
            var curShua = _bigShuas.Find(bigName);
            var curRect = curShua.GetRectTransform();
            curRect.anchoredPosition = new Vector2(554, 291);
            curShua.gameObject.Show();
            curShua.transform.GetImage().DOFade(1, 0);
            var tw1 = curRect.DOAnchorPos(new Vector2(554, 654), 1)
            .OnComplete(() => { ShowCurImg(_spine2.GetChild(0), str1); curRect.anchoredPosition = new Vector2(726, 435);
                Delay(0.5f, () => { StopAudio(SoundManager.SoundType.VOICE); });
            });

            var tw2 = curRect.DOAnchorPos(new Vector2(726, 200), 1)
           .OnComplete(() => { ShowCurImg(_spine2.GetChild(0), str2);  });

            var tw = DOTween.Sequence().Append(tw1)
                                    .Append(tw2)
                                    .OnComplete(() => {
                                     
                                        curShua.gameObject.Hide();
                                        curShua.transform.GetImage().DOFade(0, 0);
                                        if (isOk)
                                            PlaySuccessSound();
                                        else
                                            PlayFailSound();

                                        callBack?.Invoke();
                                    });
        }

        /// <summary>
        /// 车刷子Ani
        /// </summary>      
        private void CarShuaAni(string bigName, List<string> str1 = null, List<string> str2 = null, List<string> str3 = null, Action callBack = null, bool isOk = true)
        {

            var curShua = _bigShuas.Find(bigName);
            var curRect = curShua.GetRectTransform();
            curRect.anchoredPosition = new Vector2(-574, 10);
            curShua.gameObject.Show();
            curShua.transform.GetImage().DOFade(1, 0);

            if (bigName== "S1")
            {
                var newStr3 = new List<string> { str3[str3.Count-1] };
                Delay(0.5f, () => { ShowCurImg(_spine2.GetChild(2), newStr3,2.5f); });
            }

            var tw1 = curRect.DOAnchorPos(new Vector2(-574, 223), 1)
          .OnComplete(() => { ShowCurImg(_spine2.GetChild(2), str1); curRect.anchoredPosition = new Vector2(-368, 277); });

            var tw2 = curRect.DOAnchorPos(new Vector2(-368, 54), 1)
          .OnComplete(() => { ShowCurImg(_spine2.GetChild(2), str2); curRect.anchoredPosition = new Vector2(-183, 21);
              Delay(0.5f, () => { StopAudio(SoundManager.SoundType.VOICE); });
          });

            var tw3 = curRect.DOAnchorPos(new Vector2(-183, 260), 1)
          .OnComplete(() => {

              if (bigName!= "S1")             
                 ShowCurImg(_spine2.GetChild(2), str3);
              else
               {
                  str3.Remove("C17");
                  ShowCurImg(_spine2.GetChild(2), str3);
              }

              StopAudio(SoundManager.SoundType.VOICE); });

            var tw = DOTween.Sequence().Append(tw1)
                                   .Append(tw2)
                                    .Append(tw3)
                                   .OnComplete(() => {                                     
                                       curShua.gameObject.Hide();
                                       curShua.transform.GetImage().DOFade(0, 0);
                                       if (isOk)
                                           PlaySuccessSound();
                                       else
                                           PlayFailSound();

                                       callBack?.Invoke();
                                   });
        }

        /// <summary>
        /// 马路刷子Ani
        /// </summary>     
        private void MaLuShuaAni(string bigName,List<string> str1=null,List<string> str2 =null,List<string> str3 =null,List<string> str4=null,List<string> str5=null,List<string> str6=null,Action callBack=null,bool isOk=true)
        {
           
            var curShua = _bigShuas.Find(bigName);
            var curRect = curShua.GetRectTransform();
            curRect.anchoredPosition = new Vector2(-823, -117);
            curShua.gameObject.Show();
            curShua.transform.GetImage().DOFade(1, 0);
            var tw1 = curRect.DOAnchorPos(new Vector2(-823, 129), 1)
           .OnComplete(() => { ShowCurImg(_spine2.GetChild(1), str1);
               curRect.anchoredPosition = new Vector2(-480, 44);

               if (bigName == "S6"||bigName=="S5")               
                   ShowCurImg(_spine2.GetChild(1), str6, 7.5F);                           
           });

            var tw2 = curRect.DOAnchorPos(new Vector2(-480, -138), 1)
           .OnComplete(() => { ShowCurImg(_spine2.GetChild(1), str2); curRect.anchoredPosition = new Vector2(-205, -138); });

            var tw3 = curRect.DOAnchorPos(new Vector2(-205, 133), 1)
           .OnComplete(() => { ShowCurImg(_spine2.GetChild(1), str3); curRect.anchoredPosition = new Vector2(45, 133); });

            var tw4 = curRect.DOAnchorPos(new Vector2(45, -100), 1)
            .OnComplete(() => { ShowCurImg(_spine2.GetChild(1), str4); curRect.anchoredPosition = new Vector2(342, -24); });

            var tw5 = curRect.DOAnchorPos(new Vector2(342, 323), 1)
           .OnComplete(() => { ShowCurImg(_spine2.GetChild(1), str5); curRect.anchoredPosition = new Vector2(655, 163);

           });

            var tw6 = curRect.DOAnchorPos(new Vector2(655, -79), 1)
           .OnComplete(() => {

               if (bigName != "S6" || bigName != "S5")
                   ShowCurImg(_spine2.GetChild(1), str6);                                              
           });


            var tw = DOTween.Sequence().Append(tw1)
                                  .Append(tw2)
                                  .Append(tw3)
                                  .Append(tw4)
                                  .Append(tw5)
                                  .Append(tw6)
                                  .OnComplete(() => {
                                      StopAudio(SoundManager.SoundType.VOICE);
                                      curShua.gameObject.Hide();
                                      curShua.transform.GetImage().DOFade(0, 0);
                                      if (isOk)                                     
                                          PlaySuccessSound();                                                                               
                                     else                                      
                                          PlayFailSound();
                                      callBack?.Invoke();
                                  });
        }

        private void ShowCurImg(Transform parent, List<string> names = null, float time = 0)
        {
            if (names==null)            
                return;

            for (int i = 0; i < names.Count; i++)
            {
                var goTra = parent.Find(names[i]);
                goTra.gameObject.Show();
                var img = goTra.GetImage();
                img.DOFade(1, time);
            }   
        }

        private void InitDate()
        {           
            _cutLevelId = 1;
            _levelNums = new List<string>(); _level2Nums = new List<string>();
            _level3Nums = new List<string>(); _level4Nums = new List<string>();

            _okSoundIds = new List<int> { 4, 5, 7, 8, 9 };  _errorSoundIds = new List<int> { 0, 1, 2, 3 };
        }

        private void GameStart()
        {
            _mask.Show();
            _startBtn.Show();
            PlaySpine(_startBtn, "kong", () => {
                PlaySpine(_startBtn, "bf2", () => {
                    AddEvent(_startBtn, (go) => {
                        PlayOnClickSound();
                        PlayBgm(2, true, SoundManager.SoundType.COMMONBGM);
                        RemoveEvent(_startBtn);
                        PlaySpine(_startBtn, "bf", () => {
                            _startBtn.Hide(); _tT.Show();
                            BellSpeck(_tT,0, null, () => {
                                SoundManager.instance.ShowVoiceBtn(true);
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
        IEnumerator BDSpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(_dTT, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(_dTT, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_dTT, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
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
        IEnumerator SpeckerCoroutine(GameObject go, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            PlayOnClickSound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (_talkIndex == 1)
            {
                _tT.Hide(); _dD.Show();
                BellSpeck(_dD,1, null, () => {
                    _dD.Hide();
                    _mask.Hide(); _spines.Show(); InitSpines(_spine1); InitSpines(_spine2); _levelBg.gameObject.Show();
                    ShowLevel(); ShowGuideSpine();
                  
                });
            }
            _talkIndex++;
        }




        #region 常用函数


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

        #region 播放Audio

        private float PlayBgm(int index, bool isLoop = true, SoundManager.SoundType type = SoundManager.SoundType.BGM)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);          
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);           
            return time;
        }

        private float PlaySound(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);           
            return time;
        }

        /// <summary>
        /// 播放点击声音
        /// </summary>
        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        /// <summary>
        /// 播放失败声音
        /// </summary>
        private void PlayFailSound()
        {
            PlayVoice(3);
            var index = Random.Range(0, _errorSoundIds.Count);
            var id = _errorSoundIds[index];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);

           
        }

        /// <summary>
        /// 播放成功声音
        /// </summary>
        private void PlaySuccessSound()
        {
            PlayVoice(2);
            var index = Random.Range(0, _okSoundIds.Count);
            var id = _okSoundIds[index];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
        }

        #endregion

        #region 播放Spine

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);          
            return time;
        }

        private void InitSpines(Transform parent, bool isKong = true, Action callBack = null)
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
            callBack?.Invoke();
        }

        #endregion

        #region 停止Audio
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

        #region 延时
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
        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {        
            _mono.StartCoroutine(SpeckerCoroutine(go,type, index, specking, speckend));
        }


        private void SetPos(RectTransform rect, Vector2 pos)
        {           
            rect.anchoredPosition = pos;
        }

        #endregion

        #region 添加Btn监听
        private void AddBtnsEvent(Transform parent, PointerClickListener.VoidDelegate callBack)
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
            PointerClickListener.Get(go).onClick = g => {
                callBack?.Invoke(g);
            };
        }

        private void RemoveEvent(GameObject go)
        {           
            PointerClickListener.Get(go).onClick = null;
        }

        #endregion

        #region 修改Rect
        private void SetPos(RectTransform rect, Vector3 v3)
        {
            rect.anchoredPosition = v3;
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }

        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null)
        {
            rect.DOAnchorPos(v2, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        #endregion

        #endregion
    }
}
