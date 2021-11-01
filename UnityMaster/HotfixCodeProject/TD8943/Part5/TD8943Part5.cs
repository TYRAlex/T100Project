using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public enum ClickEnum
    {
        first,
        second,
        third
    }
    public class TD8943Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private GameObject Bg;
        private BellSprites bellTextures;

        //田丁
        private GameObject bd;
        private GameObject dbd;
        
        //Mask
        private Transform anyBtns;
        private GameObject mask;
        private GameObject mask_1;
        //成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;        

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;

        //创作指引完全结束
        bool isEnd = false;      
        bool isPlaying = false;
        bool isPressBtn = false;
        private int flag = 0;
        private bool isMove = true;
        private int _index = 0;
    

        //Solt
        private Transform soltPanel;
        private List<GameObject> _soltPanelList;
        private Dictionary<GameObject, ILDroper[]> soltDic;
        private ILDroper[] soltDroper;
        private List<GameObject> soltSpineList;

        //Devil        
        private List<int> saveDevilIndexList;       
        //Spine
        private Transform spinePanel;
        private ILDrager[] spineDrager;
        private List<int> spineIndexList;
        private List<int> saveSpineIndexList;       
        private ClickEnum clickEnum;
        private Vector3 localScaleDevil;
        private Vector3 localScaleSpine;
        private int dragerIndex;

        private Transform star_0;
        private Transform star_1;
        private Transform star_2;
        private GameObject[] stars_0;
        private GameObject[] stars_1;
        private GameObject[] stars_2;
        private Dictionary<string, GameObject> dicStar_0;
        private Dictionary<string, GameObject> dicStar_1;
        private Dictionary<string, GameObject> dicStar_2;

        private Transform posPanel;
        private Transform[] _posPanels;

        private Vector3 spineStartPos;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            SoundManager.instance.ShowVoiceBtn(false);

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            mono.StopCoroutine(WaiteCoroutine());
            //加载人物
            TDLoadCharacter();
            //加载成功界面
            TDLoadSuccessPanel();
            //加载游戏按钮
            TDLoadButton();

            FindSoltInit();
            FindStarInit();
            DevilAndSpineInit();
            ShowDevilAndSpine();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            talkIndex = 1;
            //GameInit();                      
        }
        private void GameInit()
        {                                   
            isPressBtn = false;           
            flag = 0;
            isMove = true;
            clickEnum = ClickEnum.first;
            _index = 0;
            isPlaying = false;
            dragerIndex = -1;
            localScaleDevil = new Vector3(1, 1, 1);
            localScaleSpine = new Vector3(0.3f, 0.3f, 0.3f);                       
            CanActived(true, 0);            
            SetSoltDic();
            star_0.gameObject.Show();
            mask_1.Show();            
            ShowDevilAndSpine();
            DevilStart();
            WaitTimeAndExcuteNext(0.5f, () => { DevilStart(); WaitTimeAndExcuteNext(0.5f, () => 
            { 
                DevilStart(); });
                WaitTimeAndExcuteNext(0.5f, () => 
                {                  
                    SpineStart("1-a", "1-b", "2-a", "2-b", "3-a", "3-b", "2-a2", "2-b2", 3);
                });
            });          
        }       
        /// <summary>
        /// 加载人物
        /// </summary>
        void TDLoadCharacter()
        {
            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
        }
       
        /// <summary>
        /// 加载成功环节
        /// </summary>
        void TDLoadSuccessPanel()
        {
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);
            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
        }
        /// <summary>
        /// 加载按钮
        /// </summary>
        void TDLoadButton()
        {
            anyBtns = curTrans.Find("mask/Btns");
            anyBtns.gameObject.SetActive(false);
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
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
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        IEnumerator WaiteCoroutine(float len = 0,Action method_2 = null)
        {           
            yield return new WaitForSeconds(len);            
            method_2?.Invoke();
        }
        /// <summary>
        /// 语音键对应方法   
        /// </summary>
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    //田丁游戏开始方法
                    TDGameStartFunc();
                    
                    break;
            }
            
            talkIndex++;
        }        
        void TDGameStartFunc()
        {
            //点击标志位
            flag = 0;           
            //bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () => 
            {
                mask.SetActive(false); bd.SetActive(false);
                mask_1.Show();
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, ()=> { GameInit(); }, ()=> { mask_1.Hide(); }));
            }));
        }
        /// <summary>
        /// 播放Spine动画
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="name">目标名字</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="callback">完成之后回调</param>
        private void PlaySpineAni(GameObject target,string name,bool isLoop=false,Action callback=null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }
        /// <summary>
        /// Bell说话
        /// </summary>
        /// <param name="index">Voice语音下标</param>
        /// <param name="goingEvent">同步执行的方法</param>
        /// <param name="finishEvent">完成回调</param>
        private void Talk(GameObject target, int index,Action goingEvent=null,Action finishEvent=null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target,SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
        }
        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        /// <param name="callback">播放完成回调</param>
        private void PlayVoice(int targetIndex,Action callback=null)
        {
            float voiceTimer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
            if (callback != null)
                WaitTimeAndExcuteNext(voiceTimer, callback);
        }
        /// <summary>
        /// 播放相应的Sound语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        private void PlaySound(int targetIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, targetIndex);
        }
        /// <summary>
        /// 延时执行
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="callback"></param>
        void WaitTimeAndExcuteNext(float timer,Action callback)
        {      
            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }
        IEnumerator WaitTimeAndExcuteNextIE(float timer,Action callBack)
        {
            yield return new WaitForSeconds(timer);
            callBack?.Invoke();
            
        }
        /// <summary>
        /// 播放BGM（用在只有一个BGM的时候）
        /// </summary>
        private void PlayBGM()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }
        
        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }                      
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

        #endregion       
        
        /// <summary>
        /// 定义按钮mode 切换游戏按键方法
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum, int index)
        {
            string result = string.Empty;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }
        private void OnClickAnyBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => {

                        anyBtns.GetChild(0).gameObject.SetActive(false);
                        bd.SetActive(true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => 
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                        }));                       
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => 
                    {
                        anyBtns.gameObject.SetActive(false); mask.SetActive(false);
                        ClearList();
                        for (int i = 0; i < spineDrager.Length; i++)
                        {
                            spineDrager[i].gameObject.Hide();
                        }
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, () => { GameInit(); }, () => { mask_1.Hide(); }));
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 5)); });
                }

            });
        }
        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
                        () =>
                        {
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        } 
        private void FindStar(GameObject[] stars,Transform p)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = p.GetChild(i).gameObject;
                stars[i].Hide();
            }            
        }
        private void StarDic(string name,GameObject[] stars,Dictionary<string,GameObject> dicStar)
        {
            dicStar.Add(name, stars[0].gameObject);
            for (int i = 1; i < stars.Length; i++)
            {
                dicStar.Add(name + (i + 1), stars[i].gameObject);
            }
        }
        private void FindStarInit()
        {
            star_0 = curTrans.Find("starManager/0");           
            star_1 = curTrans.Find("starManager/1");           
            star_2 = curTrans.Find("starManager/2");            
            stars_0 = new GameObject[star_0.childCount];           
            stars_1 = new GameObject[star_1.childCount];
            stars_2 = new GameObject[star_2.childCount];
            FindStar(stars_0, star_0);
            FindStar(stars_1, star_1);
            FindStar(stars_2, star_2);
            dicStar_0 = new Dictionary<string, GameObject>();
            dicStar_1 = new Dictionary<string, GameObject>();
            dicStar_2 = new Dictionary<string, GameObject>();
            StarDic("1-b", stars_0, dicStar_0);
            StarDic("2-b", stars_1, dicStar_1);
            StarDic("3-b", stars_2, dicStar_2);
            star_0.gameObject.Show();
            star_1.gameObject.Hide();
            star_2.gameObject.Hide();           
        }      
        private void FindSoltInit()
        {                       
            mask_1 = curTrans.Find("mask_1").gameObject;
            mask_1.Show();  
            
            soltPanel = curTrans.Find("slotPanel");
            _soltPanelList = new List<GameObject>();
            soltDic = new Dictionary<GameObject, ILDroper[]>();
            for (int i = 0; i < soltPanel.childCount; i++)
            {
                _soltPanelList.Add(soltPanel.GetChild(i).gameObject);
                soltDroper = _soltPanelList[i].GetComponentsInChildren<ILDroper>(true);               
                soltDic.Add(_soltPanelList[i], soltDroper);
            }
            soltSpineList = new List<GameObject>();
            for (int i = 0; i < soltDic.Count; i++)
            {
                for (int j = 0; j < soltDic[_soltPanelList[i]].Length; j++)
                {
                    soltDic[_soltPanelList[i]][j].index = j;
                    soltDic[_soltPanelList[i]][j].SetDropCallBack(OnAfter);
                    soltDic[_soltPanelList[i]][j].isActived = false;
                    soltDic[_soltPanelList[i]][j].transform.GetChild(0).gameObject.Hide();
                    //SpineManager.instance.DoAnimation(soltDic[_soltPanelList[i]][j].transform.GetChild(0).gameObject, "kong", false);
                }
                for (int k = 0; k < soltDic[_soltPanelList[0]].Length; k++)
                {
                    soltDic[_soltPanelList[0]][k].isActived = true;
                }
                _soltPanelList[i].Hide();
            }
            _soltPanelList[0].Show();

        } 
        private void SetSoltDic()
        {
            for (int i = 0; i < soltDic.Count; i++)
            {
                for (int j = 0; j < soltDic[_soltPanelList[i]].Length; j++)
                {                    ;
                    soltDic[_soltPanelList[i]][j].isActived = false;
                    soltDic[_soltPanelList[i]][j].transform.GetChild(0).gameObject.Hide();
                    //SpineManager.instance.DoAnimation(soltDic[_soltPanelList[i]][j].transform.GetChild(0).gameObject, "kong", false);
                }
                for (int k = 0; k < soltDic[_soltPanelList[0]].Length; k++)
                {
                    soltDic[_soltPanelList[0]][k].isActived = true;
                }
                _soltPanelList[i].Hide();
            }
            _soltPanelList[0].Show();
        }
        private void DevilAndSpineInit()
        {
            posPanel = curTrans.Find("posPanel");
            _posPanels = new Transform[posPanel.childCount];
            for (int i = 0; i < _posPanels.Length; i++)
            {
                _posPanels[i] = posPanel.GetChild(i);
            }

            spinePanel = curTrans.Find("spinePanel");
            spineDrager = spinePanel.GetComponentsInChildren<ILDrager>(true);           
            saveSpineIndexList = new List<int>();
            saveDevilIndexList = new List<int>();
            spineIndexList = new List<int>();           
            for (int i = 0; i < spineDrager.Length; i++)
            {
                spineDrager[i].index = i;
                spineDrager[i].transform.localPosition = _posPanels[i].localPosition;
                spineDrager[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);                
                spineDrager[i].transform.Rotate(Vector3.forward, Random.Range(-10, 10));
                spineIndexList.Add(i);
            }            
        }
        /// <summary>
        /// 显示小恶魔和精灵
        /// </summary>
        private void ShowDevilAndSpine()
        {           
            for (int i = 0; i < spineDrager.Length; i++)
            {                
                spineDrager[i].gameObject.Show();               
                spineDrager[i].transform.GetChild(0).gameObject.Show();
                spineDrager[i].transform.GetChild(1).gameObject.Show();                
                SpineManager.instance.DoAnimation(spineDrager[i].transform.GetChild(0).gameObject, "kong", false);
                spineDrager[i].transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(spineDrager[i].transform.GetChild(1).gameObject, "kong", false);
            }                       
        }
        private GameObject tempDevil = null;
        private void DevilStart()
        {
            tempDevil = spineDrager[RandomDevilFist()].gameObject;
            tempDevil.Show();
            tempDevil.transform.GetChild(0).gameObject.Show();
            tempDevil.transform.GetChild(0).transform.localScale = localScaleDevil;            
            tempDevil.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(tempDevil.transform.GetChild(0).gameObject, "xem1", true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
        }
        /// <summary>
        /// 第一关：随机出小恶魔
        /// </summary>
        private int RandomDevilFist()
        {           
            int temp = Random.Range(0, spineIndexList.Count);
            saveDevilIndexList.Add(spineIndexList[temp]);
            int target = spineIndexList[temp];
            spineIndexList.Remove(target);
            return target;

        }      
        private void SpineStart(string name0,string name1,string name2,string name3,string name4,string name5,string name6,string name7,int sum)
        {            
            HideSpine();
            PlaySpineAni(name0, name1);
            WaitTimeAndExcuteNext(0.5f, () => { PlaySpineAni(name0 + 2, name1 + 2); WaitTimeAndExcuteNext(0.5f, () => { PlaySpineAni(name0 + 3, name1 + 3); WaitTimeAndExcuteNext(0.5f, () => {
                PlaySpineAni(name2, name3); WaitTimeAndExcuteNext(0.5f, () =>
                {
                    PlaySpineAni(name4, name5);
                });
                WaitTimeAndExcuteNext(0.5f, () => { PlaySpineAni(name6, name7); });
            });
            }); });                           
        }
        private void SpineStartSecond(string name0, string name1, string name2, string name3, string name4, string name5, string name6, string name7, int sum)
        {          
            HideSpine();
            PlaySpineAni(name0, name1);
            WaitTimeAndExcuteNext(0.5f, () => {
                PlaySpineAni(name0 + 2, name1 + 2); WaitTimeAndExcuteNext(0.5f, () => 
                {
                    PlaySpineAni(name0 + 3, name1 + 3); WaitTimeAndExcuteNext(0.5f, () => { PlaySpineAni(name0 + 4, name1 + 4); WaitTimeAndExcuteNext(0.5f, () => { PlaySpineAni(name2, name3); WaitTimeAndExcuteNext(0.5f, () => { PlaySpineAni(name4, name5); WaitTimeAndExcuteNext(0.5f, () => { PlaySpineAni(name6, name7);}); }); }); });
                });
            }); 
            //mask_1.Hide();
        }
        private void SpineStartThird(string name0, string name1, string name2, string name3, string name4, string name5, string name6, string name7, int sum)
        {           
            HideSpine();
            PlaySpineAni(name0, name1);
            WaitTimeAndExcuteNext(0.5f, () => {
                PlaySpineAni(name0 + 2, name1 + 2); WaitTimeAndExcuteNext(0.5f, () =>
                {
                    PlaySpineAni(name0 + 3, name1 + 3); WaitTimeAndExcuteNext(0.5f, () => 
                    { 
                        PlaySpineAni(name0 + 4, name1 + 4);
                        WaitTimeAndExcuteNext(0.5f, () => { 
                            PlaySpineAni(name0 + 5, name1 + 5);
                            WaitTimeAndExcuteNext(0.5f, () => { PlaySpineAni(name2, name3); WaitTimeAndExcuteNext(0.5f, () => { PlaySpineAni(name4, name5); WaitTimeAndExcuteNext(0.5f, () => { PlaySpineAni(name6, name7); }); }); });
                        });
                    });
                });
            });                        
            //mask_1.Hide();
        }
        private void HideSpine()
        {           
            for (int i = 0; i < saveDevilIndexList.Count; i++)
            {                               
                spineIndexList.Remove(saveDevilIndexList[i]);               
            }           
        }
        /// <summary>
        /// 第一关：随机出精灵
        /// </summary>
        private int RandomSpineFist()
        {
            int temp = Random.Range(0, spineIndexList.Count);          
            saveSpineIndexList.Add(spineIndexList[temp]);
            int target = spineIndexList[temp];
            spineIndexList.Remove(spineIndexList[temp]);
            return target;
        }       
        private void PlaySpineAni(String aniName, string aniName1)
        {
            GameObject tempSpine = null;
            tempSpine = spineDrager[RandomSpineFist()].gameObject;
            tempSpine.Show();
            tempSpine.transform.GetChild(0).gameObject.Show();
            tempSpine.transform.GetChild(0).transform.localScale = localScaleSpine;
            tempSpine.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,7, false);
            SpineManager.instance.DoAnimation(tempSpine.transform.GetChild(0).gameObject, aniName, false, () =>
            {
                SpineManager.instance.DoAnimation(tempSpine.transform.GetChild(0).gameObject, aniName1, true);                
            });          
        }                                                                                                     
        /// <summary>
        /// 数据还原
        /// </summary>
        private void ClearList()
        {           
            for (int i = 0; i < saveDevilIndexList.Count; i++)
            {                
                spineIndexList.Add(saveDevilIndexList[i]);
            }
            saveDevilIndexList.Clear();
            for (int i = 0; i < saveSpineIndexList.Count; i++)
            {
                spineIndexList.Add(saveSpineIndexList[i]);
            }
            saveSpineIndexList.Clear();          
        }            
        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            spineDrager[index].transform.SetAsLastSibling();
            spineStartPos = spineDrager[index].transform.localPosition;
        }
        private void OnDrag(Vector3 pos, int type, int index) {}
        private string aniName;
        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            spineDrager[index].transform.localPosition = spineStartPos;
            //spineDrager[index].DoReset();
            aniName = SpineManager.instance.GetCurrentAnimationName(spineDrager[index].transform.GetChild(0).gameObject);
            mask_1.Show();
            if (isMatch)
            {
                if (clickEnum == ClickEnum.first)
                {
                    if (aniName == soltDic[_soltPanelList[0]][dropIndex].gameObject.name)
                    {
                        _index += 1;
                        //mask_1.Show();
                        spineDrager[index].gameObject.SetActive(false);
                        soltDic[_soltPanelList[0]][dropIndex].isActived = false;
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), () =>
                        {
                            dicStar_0[aniName].Show();
                            soltDic[_soltPanelList[0]][dropIndex].transform.GetChild(0).gameObject.Show();
                            SpineManager.instance.DoAnimation(soltDic[_soltPanelList[0]][dropIndex].transform.GetChild(0).gameObject, aniName, true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                            SpineManager.instance.DoAnimation(dicStar_0[aniName], "star", false, () =>
                            {
                                SpineManager.instance.DoAnimation(dicStar_0[aniName], "kong", false, () =>
                                {
                                    mask_1.Hide();
                                    if (_index == 3)
                                    {
                                        mask_1.Show();
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                        WaitTimeAndExcuteNext(1.0f, () => { SpineManager.instance.DoAnimation(spineDrager[saveDevilIndexList[0]].transform.GetChild(0).gameObject, "kong", false); });
                                        SpineManager.instance.DoAnimation(spineDrager[saveDevilIndexList[0]].transform.GetChild(1).gameObject, "xem-flash", false, () =>
                                        {

                                            mono.StartCoroutine(WaiteCoroutine(1.2f, () => 
                                            {
                                                star_0.gameObject.Hide();
                                                star_1.gameObject.Show();
                                                clickEnum = ClickEnum.second;
                                                Succeed(0, 1, 2);
                                                DevilStart();
                                                WaitTimeAndExcuteNext(0.5f, () =>
                                                {
                                                    DevilStart();
                                                    SpineStartSecond("2-a", "2-b", "1-a3", "1-b3", "3-a", "3-b", "3-a4", "3-b4", 4);
                                                });
                                                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 3, null, () => { mask_1.Hide(); }));
                                            }));
                                        });
                                    }
                                });
                            });
                        }));
                    }
                    else
                    {
                        PlayFailVioce();
                    }
                }
                else if (clickEnum == ClickEnum.second)
                {
                    if (aniName == soltDic[_soltPanelList[1]][dropIndex].gameObject.name)
                    {
                        _index += 1;
                        mask_1.Show();
                        spineDrager[index].gameObject.SetActive(false);
                        soltDic[_soltPanelList[1]][dropIndex].isActived = false;
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), () =>
                        {
                            dicStar_1[aniName].Show();
                            soltDic[_soltPanelList[1]][dropIndex].transform.GetChild(0).gameObject.Show();
                            SpineManager.instance.DoAnimation(soltDic[_soltPanelList[1]][dropIndex].transform.GetChild(0).gameObject, aniName, true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                            SpineManager.instance.DoAnimation(dicStar_1[aniName], "star", false, () =>
                            {
                                SpineManager.instance.DoAnimation(dicStar_1[aniName], "kong", false, () =>
                                {
                                    mask_1.Hide();
                                    if (_index == 4)
                                    {
                                        mask_1.Show();
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                        WaitTimeAndExcuteNext(1.0f, () => { SpineManager.instance.DoAnimation(spineDrager[saveDevilIndexList[0]].transform.GetChild(0).gameObject, "kong", false); });
                                        SpineManager.instance.DoAnimation(spineDrager[saveDevilIndexList[0]].transform.GetChild(1).gameObject, "xem-flash", false, () =>
                                        {
                                            mono.StartCoroutine(WaiteCoroutine(1.2f, () => 
                                            {
                                                star_1.gameObject.Hide();
                                                star_2.gameObject.Show();
                                                clickEnum = ClickEnum.third;
                                                Succeed(1, 2, 1);
                                                DevilStart();
                                                WaitTimeAndExcuteNext(0.5f, () =>
                                                {
                                                    SpineStartThird("3-a", "3-b", "1-a3", "1-b3", "2-a", "2-b", "1-a2", "1-b2", 5);
                                                });
                                                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 4, null, () => { mask_1.Hide(); }));
                                            }));                                           
                                        });
                                    }
                                });
                            });
                        }));
                    }
                    else
                    {
                        PlayFailVioce();
                    }
                }
                else if (clickEnum == ClickEnum.third)
                {
                    if (aniName == soltDic[_soltPanelList[2]][dropIndex].gameObject.name)
                    {
                        _index += 1;
                        mask_1.Show();
                        spineDrager[index].gameObject.Hide();
                        soltDic[_soltPanelList[2]][dropIndex].isActived = false;
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), () =>
                        {
                            dicStar_2[aniName].Show();
                            soltDic[_soltPanelList[2]][dropIndex].transform.GetChild(0).gameObject.Show();
                            soltDic[_soltPanelList[2]][dropIndex].transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            SpineManager.instance.DoAnimation(soltDic[_soltPanelList[2]][dropIndex].transform.GetChild(0).gameObject, aniName, true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                            SpineManager.instance.DoAnimation(dicStar_2[aniName], "star", false, () =>
                            {
                                SpineManager.instance.DoAnimation(dicStar_2[aniName], "kong", false, () =>
                                {
                                    mask_1.Hide();
                                    if (_index == 5)
                                    {
                                        mask_1.Show();
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                        WaitTimeAndExcuteNext(1.0f, () => { SpineManager.instance.DoAnimation(spineDrager[saveDevilIndexList[0]].transform.GetChild(0).gameObject, "kong", false); });
                                        SpineManager.instance.DoAnimation(spineDrager[saveDevilIndexList[0]].transform.GetChild(1).gameObject, "xem-flash", false, () =>
                                        {
                                            WaitTimeAndExcuteNext(1.2f, () =>
                                            {
                                                clickEnum = ClickEnum.first;
                                                isPlaying = false;
                                                playSuccessSpine();
                                                WaitTimeAndExcuteNext(1.0f, () => 
                                                {
                                                    _soltPanelList[2].Hide();
                                                });
                                            });
                                        });
                                    }
                                });
                            });
                        }));
                    }
                    else
                    {
                        PlayFailVioce();
                    }
                }
            }
            else
            {
                PlayFailVioce();
            }
                                      
        }  
        private void PlayFailVioce()
        {
            mask_1.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), null, () =>
            {
                mask_1.Hide();
            }));
        }
        private int dropIndex;
        private bool OnAfter(int dragType, int index, int dropType)
        {
            dropIndex = index;                            
            return true;
        }
        private void Succeed(int index_0,int index_1,int index_3)
        {
            _index = 0;
            ClearList();
            //ShowOrHide(soltDic, _soltPanelList, index_0);
            _soltPanelList[index_1].Show();
            //Hide(index_0);
            _soltPanelList[index_0].Hide();            
            CanActived(true, index_1);
            ShowDevilAndSpine();
        }
        private void Hide(int index_0)
        {
            for (int i = 0; i < soltDic[_soltPanelList[index_0]].Length; i++)
            {
                SpineManager.instance.DoAnimation(soltDic[_soltPanelList[index_0]][i].transform.GetChild(0).gameObject, "kong", false);
            }
        }
        private void CanActived(bool isCan,int index_1)
        {
            for (int i = 0; i < soltDic[_soltPanelList[index_1]].Length; i++)
            {

                soltDic[_soltPanelList[index_1]][i].isActived = isCan;
            }
        }
        private void ShowOrHide(Dictionary<GameObject,ILDroper[]>dic, List<GameObject>goList,int index_0)
        {
            for (int i = 0; i < dic[goList[index_0]].Length; i++)
            {
                //SpineManager.instance.DoAnimation(dic[goList[index_0]][i].transform.GetChild(0).gameObject, "kong", false);                
            }
        }
    }    
}
