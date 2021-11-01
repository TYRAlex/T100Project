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
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD8944Part5
    {

        public enum AniEnum
        {
            che,
            nv,
            lang,
            NULL
        }
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

        //成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;
        //创作指引完全结束
        bool isEnd = false;
        bool isPlaying = false;
        bool isPressBtn = false;
        private int flag = 0;

        private Transform bg_Spine;
        private GameObject[] bg_Spines;
        private GameObject succeedAnime;

        private GameObject mask_1;
        private GameObject maskDrager;
        private Transform dropr;
        private ILDroper[] _dropers;
        private Transform drager;
        private ILDrager[] _dragers;

        private GameObject Btn;
        private Empty4Raycast deteBtn;
        private GameObject btnMask;
        private GameObject dragerSolt;
        private GameObject yes;

        private int dropIndex;
        private int dragerIndex;

        private AniEnum aniEnum_Che = AniEnum.NULL;
        private AniEnum aniEnum_Nv = AniEnum.NULL;
        private AniEnum aniEnum_Lang = AniEnum.NULL;
        private AniEnum aniEnum_Hou = AniEnum.NULL;

        private Transform _dragerPosInit;
        private Vector3[] _dragersPos;
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
            //任务对话方法加载
            //TDLoadDialogue();
            //加载人物
            TDLoadCharacter();
            //加载成功界面
            TDLoadSuccessPanel();
            //加载游戏按钮
            TDLoadButton();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            FindInit();
            GameInit();
            //GameStart();
        }
        private void GameInit()
        {
            talkIndex = 1;
            isPressBtn = false;
            //textSpeed = 0.1f;
            isPlaying = false;
            flag = 0;
            dropIndex = -1;
            dragerIndex = 0;
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
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
        }
        void GameStart()
        {
            //对话环节开始
            //DialogueGameStart();
            bd.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
            //buDing.SetActive(false);
            //devil.SetActive(false);
            //bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () => 
            { mask.SetActive(false); bd.SetActive(false);isPlaying = false; }));
        }
        /// <summary>
        /// 播放Spine动画
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="name">目标名字</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="callback">完成之后回调</param>
        private void PlaySpineAni(GameObject target, string name, bool isLoop = false, Action callback = null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }
        /// <summary>
        /// Bell说话
        /// </summary>
        /// <param name="index">Voice语音下标</param>
        /// <param name="goingEvent">同步执行的方法</param>
        /// <param name="finishEvent">完成回调</param>
        private void Talk(GameObject target, int index, Action goingEvent = null, Action finishEvent = null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target, SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
        }
        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        /// <param name="callback">播放完成回调</param>
        private void PlayVoice(int targetIndex, Action callback = null)
        {
            float voiceTimer = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
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
        void WaitTimeAndExcuteNext(float timer, Action callback)
        {
            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }
        IEnumerator WaitTimeAndExcuteNextIE(float timer, Action callBack)
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
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => 
                    { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); GameReset(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2)); });
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
        private void FindInit()
        {
            bg_Spine = curTrans.Find("Bg_Spine");
            bg_Spines = new GameObject[bg_Spine.childCount];
            for (int i = 0; i < bg_Spines.Length; i++)
            {
                bg_Spines[i] = bg_Spine.GetChild(i).gameObject;
                bg_Spines[i].Show();
            }

            succeedAnime = curTrans.Find("succeed/anime").gameObject;
            SpineManager.instance.DoAnimation(succeedAnime, "kong", false);

            mask_1 = curTrans.Find("mask_1").gameObject;
            mask_1.Show();
            maskDrager = curTrans.Find("maskDrager").gameObject;
            maskDrager.Hide();

            dropr = curTrans.Find("dropr");
            dropr.gameObject.Show();
            _dropers = dropr.GetComponentsInChildren<ILDroper>(true);
            for (int i = 0; i < _dropers.Length; i++)
            {
                _dropers[i].index = i;
                _dropers[i].isActived = true;
                _dropers[i].SetDropCallBack(OnAfter);
                _dropers[i].transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_dropers[i].transform.GetChild(0).gameObject, "kong", false);
            }

            _dragerPosInit = curTrans.Find("dragerPos");
            _dragersPos = new Vector3[_dragerPosInit.childCount];
            for (int i = 0; i < _dragersPos.Length; i++)
            {
                _dragersPos[i] = _dragerPosInit.GetChild(i).transform.position;                
            }

            drager = curTrans.Find("drager");
            drager.gameObject.Show();
            _dragers = drager.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < _dragers.Length; i++)
            {                
                _dragers[i].gameObject.Show();
                _dragers[i].index = i;
                _dragers[i].transform.position = _dragers[i].transform.parent.GetChild(1).transform.position;
                _dragers[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);                
            }
            DragerIsActive(true);

            Btn = curTrans.Find("Btn").gameObject;
            Btn.Show();
            btnMask = curTrans.Find("Btn/btnMask").gameObject;
            btnMask.Show();
            deteBtn = curTrans.Find("Btn/deteBtn").GetComponent<Empty4Raycast>();
            deteBtn.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(deteBtn.transform.GetChild(0).gameObject, "anniu3", false);
            Util.AddBtnClick(deteBtn.gameObject, DeteBtnEvent);

            dragerSolt = curTrans.Find("dragerSolt").gameObject;
            dragerSolt.Show();
            yes = curTrans.Find("Btn/Yes").gameObject;           
            SpineManager.instance.DoAnimation(yes, "kong", false);

            SetAniEnum();
         }
        private void SetAniEnum()
        {
            aniEnum_Che = AniEnum.NULL;
            aniEnum_Nv = AniEnum.NULL;
            aniEnum_Lang = AniEnum.NULL;
            aniEnum_Hou = AniEnum.NULL;
        }
        private void GameReset()
        {
            ShowBGSpine();
            SpineManager.instance.DoAnimation(succeedAnime, "kong", false);
            mask_1.Show();
            maskDrager.Hide();
            dropr.gameObject.Show();
            for (int i = 0; i < _dropers.Length; i++)
            {
                _dropers[i].transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_dropers[i].transform.GetChild(0).gameObject, "kong", false);
            }
            DroperIsActive(true);
            dragerSolt.Show();
            drager.gameObject.Show();
            for (int i = 0; i < _dragers.Length; i++)
            {
                _dragers[i].gameObject.Show();
            }
            DragerIsActive(true);         
            deteBtn.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(deteBtn.transform.GetChild(0).gameObject, "anniu3", false);

            Btn.Show();
            btnMask.Show();
            SpineManager.instance.DoAnimation(yes, "kong", false);

            SetAniEnum();
        }
        private void DroperIsActive(bool isActive)
        {
            for (int i = 0; i < _dropers.Length; i++)
            {
                _dropers[i].isActived = isActive;
            }
        }
        private void DragerIsActive(bool isActive)
        {
            for (int i = 0; i < _dragers.Length; i++)
            {                
                _dragers[i].isActived = isActive;               
            }
        }     
        private void ShowBGSpine()
        {
            for (int i = 0; i < bg_Spines.Length; i++)
            {
                bg_Spines[i].Show();
            }
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {
            dropIndex = index;            
            return true;
        }
        private Vector3 _dragerPos;
        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
           // _dragerPos = _dragers[index].transform.parent.GetChild(1).transform.position;
            _dragers[index].transform.parent.SetAsLastSibling();            
        }

        private void OnDrag(Vector3 pos, int type, int index){}

        private string _name;
        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            _dragers[index].DoReset(); 
            //_dragers[index].transform.position = _dragerPos;
            _name = _dragers[index].transform.GetChild(0).GetComponent<Image>().sprite.name;
            DragerIsActive(false);           
            if (isMatch)
            {
                if (_name == _dropers[dropIndex].gameObject.name)
                {
                    dragerIndex++;
                    _dropers[dropIndex].isActived = false;
                    _dragers[index].gameObject.Hide();
                    if(_name== "che")
                    {
                        aniEnum_Che = AniEnum.che;
                        bg_Spines[0].Hide();
                        SpineManager.instance.DoAnimation(_dropers[dropIndex].transform.GetChild(0).gameObject, "che", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                        PlayVioce(4, 10, () => { DragerIsActive(true); IFSucceed(); });
                    }
                    else if(_name == "huo")
                    {
                        bg_Spines[1].Hide();
                        aniEnum_Hou = AniEnum.NULL;
                        SpineManager.instance.DoAnimation(_dropers[dropIndex].transform.GetChild(0).gameObject, "hou", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                        PlayVioce(4, 10, () => { DragerIsActive(true); IFSucceed(); });
                    }
                    else if (_name == "nv")
                    {
                        aniEnum_Nv = AniEnum.nv;
                        bg_Spines[2].Hide();
                        SpineManager.instance.DoAnimation(_dropers[dropIndex].transform.GetChild(0).gameObject, "nv", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                        PlayVioce(4, 10, () => { DragerIsActive(true); IFSucceed(); });
                    }
                    else if (_name == "lang")
                    {
                        aniEnum_Lang = AniEnum.lang;
                        bg_Spines[3].Hide();
                        SpineManager.instance.DoAnimation(_dropers[dropIndex].transform.GetChild(0).gameObject, "lang", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                        PlayVioce(4, 10, () => 
                        {
                            DragerIsActive(true);
                            IFSucceed();
                        });
                    }                                                                            
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    PlayVioce(0, 4, () => { DragerIsActive(true); });
                }
            }
            else if(!isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), null, ()=> { DragerIsActive(true); }));
            }
        }
        private void IFSucceed()
        {
            if (dragerIndex == 3)
            {
                dragerIndex = 0;
                //maskDrager.Show();
                btnMask.Hide();
                DragerIsActive(false);
            }
        }       
        private void PlayVioce(int index,int index_1,Action callBack = null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);            
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(index, index_1), null, callBack));
        }
        private void DeteBtnEvent(GameObject obj)
        {           
            btnMask.Show();
            SpineManager.instance.DoAnimation(deteBtn.transform.GetChild(0).gameObject, "anniu", false);
            if (aniEnum_Lang== AniEnum.lang && aniEnum_Nv == AniEnum.nv && aniEnum_Che == AniEnum.che)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                yes.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(yes, "yes", false, () => 
                {
                    dragerSolt.Hide();
                    drager.gameObject.Hide();
                    Btn.Hide();
                    dropr.gameObject.Hide();
                    mask_1.Hide();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(succeedAnime, "anime", false, () => 
                    {                        
                        WaitTimeAndExcuteNext(1.0f, () => { playSuccessSpine(); });
                    });
                });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                SpineManager.instance.DoAnimation(deteBtn.transform.GetChild(0).gameObject, "anniu2", false,()=> 
                {                    
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), null, () => 
                    {
                        SpineManager.instance.DoAnimation(deteBtn.transform.GetChild(0).gameObject, "anniu3", false);
                        SetAniEnum();
                        DragerIsActive(true);
                        for (int i = 0; i < _dropers.Length; i++)
                        {
                            SpineManager.instance.DoAnimation(_dropers[i].transform.GetChild(0).gameObject, "kong", false);
                        }
                        for (int i = 0; i < _dragers.Length; i++)
                        {
                            _dragers[i].gameObject.Show();
                        }
                        ShowBGSpine();
                        DroperIsActive(true);
                        maskDrager.Hide();
                    }));                   
                });
            }
        }
    }
}
