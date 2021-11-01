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
    public class TD6734Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

#region 通用环节
        private GameObject Bg;
        private BellSprites bellTextures;

        //田丁
        private GameObject bd;
        private GameObject dbd;

        //Mask       
        private GameObject mask;
        private Transform anyBtns;
        //成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;

        //田丁对话 用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private float textSpeed;
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;

        private int flag = 0;
        //创作指引完全结束
        bool isEnd = false;
        bool isPlaying = false;
        bool isPressBtn = false;
        #endregion

        //交互环节-非通用部分
        private GameObject dz;
        private GameObject em;
        private GameObject xem;
        private Slider emSlider;

        //非洲人
        private GameObject star;
        private GameObject african;       
        private GameObject dh;
        private GameObject hat;
        private Transform[] hats;
        private GameObject hz;

        private GameObject bu;
        private ILDrager[] bus;
        private GameObject food;
        private ILDrager[] foods;
        private GameObject ear;
        private ILDrager[] ears;
        private List<ILDrager[]> hzList;
        private Dictionary<ILDrager, Vector3> dicHzPos;
        private List<GameObject> hzs;

        private Image[] personHats;
        private List<int> personHatsIndex;
        private List<int> hatIndexList;
        private int personIndex;
        private int hatIndex;
        private int personHatIndex;
        private int rangeDhIndex;
        private int rangeHzIndex;

        private Transform dragerHat;
        private ILDrager[] dragerHats;
        private string spriteName;       
        private int dragerCompleteIndex;
        private bool isShow;
        private GameObject hzMask;
        private GameObject hzMask_1;

        private string hatAniName;
        private string hatFailAniName;
        private string designAniName;

        private GameObject hatMask;
        private GameObject mask_hats;
        private GameObject kp1;
        private GameObject kp2;
        private GameObject kp3;
        private GameObject kp4;

        private IEnumerator _renIE;
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
            //加载游戏按钮
            TDLoadButton();
            //任务对话方法加载
            TDLoadDialogue();
            //加载人物
            TDLoadCharacter();
            //加载成功界面
            TDLoadSuccessPanel();           

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);           
            GameInit();
            HatInit();
            InterInit();
            EmSlider();
        }
        private void GameInit()
        {
            mono.StopAllCoroutines();
            talkIndex = 1;
            //designAniName = new string[3];
            personHatsIndex = new List<int>();
            for (int i = 1; i <4; i++)
            {
                personHatsIndex.Add(i);
            }
            hatIndexList = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                hatIndexList.Add(i);
            }           
            //田丁初始化
            TDGameInit();
        }
        void GameStart()
        {
            //田丁开始游戏
            TDGameStart();

        }
        void TDGameInit()
        {
            isPressBtn = false;
            textSpeed = 0.1f;
            flag = 0;
        }
        
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, true);
            Dialogue();
        }
        /// <summary>
        /// 对话环节
        /// </summary>
        private void Dialogue()
        {
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, () =>
                {
                    ShowDialogue("我讨厌一切，是不会让你们得逞的！", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, () =>
                        {
                            ShowDialogue("小恶魔来了，小朋友们快击退它吧！", bdText);
                        }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    });
                }));
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
        /// 加载对话环节
        /// </summary>
        void TDLoadDialogue()
        {
            buDing = curTrans.Find("mask/buDing").gameObject;
            devil = curTrans.Find("mask/devil").gameObject;

            buDing.SetActive(true);
            devil.SetActive(true);

            bdText = buDing.transform.GetChild(0).GetComponent<Text>();                          
            devilText = devil.transform.GetChild(0).GetComponent<Text>();

            bdText.text = string.Empty;
            devilText.text = string.Empty;

            bdStartPos = curTrans.Find("mask/bdStartPos");
            devilStartPos = curTrans.Find("mask/devilStartPos");           

            bdEndPos = curTrans.Find("mask/bdEndPos");
            devilEndPos = curTrans.Find("mask/devilEndPos");

            buDing.transform.position = bdStartPos.position;
            devil.transform.position = devilStartPos.position;
        }

        /// <summary>
        /// 加载成功环节
        /// </summary>
        void TDLoadSuccessPanel()
        {
            successSpine = curTrans.Find("mask/successSpine").gameObject;            
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;

            successSpine.SetActive(false);
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
            buDing.SetActive(false);
            devil.SetActive(false);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null, () => { mask.SetActive(false); bd.SetActive(false);  InterStart(); }));
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
        #region 延时执行
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
        #endregion
        /// <summary>
        /// 播放BGM（用在只有一个BGM的时候）
        /// </summary>
        private void PlayBGM()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }       
        //按钮音效
        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
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
        /// <summary>
        /// 播放，返回，OK按钮点击
        /// </summary>
        /// <param name="obj"></param>
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
                        anyBtns.gameObject.SetActive(false); isPlaying = false; GameStart(); 
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => 
                    { 
                        anyBtns.gameObject.SetActive(false); mask.SetActive(false); 
                        isPlaying = false;
                        HatInit();
                        GameInit();
                        InterInit();
                        EmSlider();
                        InterStart();
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    { 
                        switchBGM();
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE,3));

                        african.SetActive(false);
                        for (int i = 0; i < hzs.Count; i++)
                        {
                            hzs[i].SetActive(false);
                        }
                    });
                }

            });
        }
        /// <summary>
        /// 田丁对话方法
        /// </summary>
        /// <param name="str"></param>
        /// <param name="text"></param>
        /// <param name="callBack"></param>

        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
                text.text += str[i];
                if (i == 25)
                {
                    text.text = "";
                }
                i++;
            }
            callBack?.Invoke();
            yield break;
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

        //交互环节-非通用部分
        private void InterInit()
        {
            dz = curTrans.Find("dz").gameObject;
            em = curTrans.Find("em").gameObject;
            xem = curTrans.Find("em/xem").gameObject;
            xem.SetActive(true);
            SpineManager.instance.DoAnimation(xem, "xem1", false);

            emSlider = curTrans.Find("em/emSlider").GetComponent<Slider>();

            star = curTrans.Find("star").gameObject;
            african = curTrans.Find("hatManager/ren").gameObject;          

            hat = curTrans.Find("hatManager/hat").gameObject;
            hats = hat.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < hats.Length; i++)
            {
                hats[i].gameObject.SetActive(false);
            }
            dh = curTrans.Find("hatManager/ren/dh").gameObject;
            hz = curTrans.Find("hz").gameObject;
            hzMask = curTrans.Find("hzMask").gameObject;
            hzMask_1 = curTrans.Find("hzMask_1").gameObject;
            hzMask_1.SetActive(false);

            bu = hz.transform.Find("bu").gameObject;
            food= hz.transform.Find("food").gameObject;
            ear= hz.transform.Find("ear").gameObject;
            hzs = new List<GameObject>();
            hzs.Add(bu);
            hzs.Add(food);
            hzs.Add(ear);            

            bus = bu.GetComponentsInChildren<ILDrager>(true);
            foods = food.GetComponentsInChildren<ILDrager>(true);
            ears = ear.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < ears.Length; i++)
            {
                bus[i].index = i;
                foods[i].index = i;
                ears[i].index = i;
            }

            personHats = dh.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < personHats.Length; i++)
            {
                personHats[i].gameObject.SetActive(false);               
            }
            personIndex = -1;
            hatIndex = -1;
            personHatIndex = 0;
            dragerCompleteIndex =0;
            rangeDhIndex = 1;
            isShow = true;

            mask_hats = curTrans.Find("mask_hats").gameObject;
            mask_hats.SetActive(false);



            for (int i = 0; i < dragerHats.Length; i++)
            {                
                dragerHats[i].isActived = true;
                dragerHats[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                dragerHats[i].index = i;
            }                               
            hzList = new List<ILDrager[]>();
            dicHzPos = new Dictionary<ILDrager, Vector3>();          
            hzList.Add(bus);
            hzList.Add(foods);
            hzList.Add(ears);           

            //显示与隐藏初始化
            dz.SetActive(true);
            em.SetActive(true);
            star.SetActive(false);
            african.SetActive(false);
            hat.SetActive(false);
            dh.SetActive(false);
            hz.SetActive(true);
            hzMask.SetActive(false);

            for (int i = 0; i < hzs.Count; i++)
            {
                hzs[i].SetActive(false);
            }            
            for (int i = 0; i < hzList.Count; i++)
            {
                for (int j = 0; j < hzList[i].Length; j++)
                {
                    hzList[i][j].GetComponent<ILDrager>().SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                    hzList[i][j].gameObject.SetActive(false);
                    dicHzPos.Add(hzList[i][j], hzList[i][j].transform.position);
                }
            }            
        }
        private void EmSlider()
        {
            em.SetActive(true);
            emSlider.value = 1.0f;
        }
        //交互部分游戏开始;
        private void InterStart()
        {          
            african.SetActive(true);
            _renIE = ShowRen();
            mono.StartCoroutine(_renIE);
            rangeHzIndex = Random.Range(0, 3);
            hzs[rangeHzIndex].SetActive(true);
            hzMask_1.SetActive(true);
            for (int i = 0; i < hzList[0].Length; i++)
            {              
                hzList[rangeHzIndex][i].gameObject.SetActive(true);
            }
            dh.SetActive(true);
            SpineManager.instance.DoAnimation(dh, "kong", false, () =>
            {
                //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                SpineManager.instance.DoAnimation(dh, "dh", false, () =>
                {
                    hzMask.SetActive(true);
                    hatIndex = Random.Range(0, hatIndexList.Count);
                    if (hatIndexList.Count != 0)
                    {
                        personHatIndex = hatIndexList[hatIndex];
                    }                    
                    personHats[personHatIndex].gameObject.SetActive(true);
                    personHats[personHatIndex].sprite = personHats[personHatIndex].GetComponent<BellSprites>().sprites[0];
                    personHats[personHatIndex].transform.GetComponent<CanvasGroup>().alpha = 0;
                    personHats[personHatIndex].transform.GetComponent<CanvasGroup>().DOFade(1, 1);
                    if (hatIndexList.Count != 0)
                    {
                        hatIndexList.Remove(hatIndexList[hatIndex]);
                    }
                });
            });          
        }
        private IEnumerator ShowRen()
        {
            bool isShow = true;
            while (isShow)
            {
                dz.SetActive(false);
                for (int i = 0; i < hats.Length; i++)
                {
                    if (hats[i].gameObject.activeSelf == true)
                    {
                       
                        
                        //hats[i].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        //african.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(hats[i].gameObject, hatFailAniName + (i==0?"":(i+1).ToString()), false);                       
                    }
                }
                                                                                           
                SpineManager.instance.DoAnimation(african, "ren1", false, () => 
                {                    
                    SpineManager.instance.DoAnimation(african, "ren0", true);
                    for (int i = 0; i < hats.Length; i++)
                    {
                        if (hats[i].gameObject.activeSelf == true)
                        {
                            string s = SpineManager.instance.GetCurrentAnimationName(hats[i].gameObject);
                            string re = s.Replace("1", "0");
                            SpineManager.instance.DoAnimation(hats[i].gameObject, re, true);
                        }
                    }
                }); 
                yield return new WaitForSeconds(5.0f);
            }        
        }
        private void OnBeginDrag(Vector3 pos, int type, int index)
        { 
            SoundManager.instance.PlayClip(9);
            dragerHats[index].transform.parent.SetAsLastSibling();
            if (dragerCompleteIndex >= 1)
            {
                hzList[rangeDhIndex - 1][index].transform.parent.SetAsLastSibling();
            }                  
        }
        private void OnDrag(Vector3 pos, int type, int dragerIndex){}
        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            ResetPos(index);                     
            if (isMatch)
            {
                mono.StopCoroutine(_renIE);
                if (isShow)
                {
                    ShowHat(index);
                }
                else
                {
                    if (personHatIndex == 0 && dragerCompleteIndex <= 4)
                    {
                        ShowPersonHats(dragerCompleteIndex, index, "ren0-a");
                    }
                    else if (personHatIndex == 1 && dragerCompleteIndex <= 4)
                    {
                        ShowPersonHats(dragerCompleteIndex, index, "ren0-b");
                    }
                    else if (personHatIndex == 2 && dragerCompleteIndex <= 4)
                    {
                        ShowPersonHats(dragerCompleteIndex, index, "ren0-c");
                    }
                    else if (personHatIndex == 3 && dragerCompleteIndex <= 4)
                    {
                        ShowPersonHats(dragerCompleteIndex, index, "ren0-d");
                    }
                }               
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            }
        }
        private void ResetPos(int index)
        {
            if (dragerCompleteIndex == 0)
            {
                dragerHats[index].DoReset();              
            }
            else if (dragerCompleteIndex == 1)
            {
                hzList[rangeDhIndex - 1][index].DoReset();
            }
            else if (dragerCompleteIndex == 2)
            {
                hzList[rangeDhIndex - 1][index].DoReset();
            }
            else if (dragerCompleteIndex == 3)
            {
                hzList[rangeDhIndex - 1][index].DoReset();
            }
        }
        /// <summary>
        /// 显示帽子
        /// </summary>
        private void ShowHat(int index)
        {
            spriteName = dragerHats[index].GetComponent<Image>().sprite.name;
            if (spriteName == personHats[personHatIndex].sprite.name)
            {
                isShow = false;
                hat.SetActive(true);
                dragerCompleteIndex++;//dragerCompleteIndex=1
                dragerHats[index].gameObject.SetActive(false);
                if (personHatIndex == 0)
                {
                    hatAniName = "ren0-a";
                    hatFailAniName = "ren1-a";                   
                    PlayHatAni();
                    PlayStarAnimation();                    
                }
                else if (personHatIndex == 1)
                {
                    hatAniName = "ren0-b";
                    hatFailAniName = "ren1-b";                   
                    PlayHatAni();
                    PlayStarAnimation();                    
                }
                else if (personHatIndex == 2)
                {
                    hatAniName = "ren0-c";
                    hatFailAniName = "ren1-c";                   
                    PlayHatAni();
                    PlayStarAnimation();                  
                }
                else if (personHatIndex == 3)
                {
                    hatAniName = "ren0-d";
                    hatFailAniName = "ren1-d";                   
                    PlayHatAni();
                    SpineManager.instance.DoAnimation(hat, "ren0-d", true, () => {});
                    PlayStarAnimation();                   
                }
                mask_hats.SetActive(true);               
                dh.SetActive(false);
                WaitTimeAndExcuteNext(3.0f, () =>
                {
                    mono.StartCoroutine(_renIE);
                    rangeDhIndex = RandomDhIndex();                  
                    hzs[rangeHzIndex].SetActive(false);
                    hzs[rangeDhIndex-1].SetActive(true);
                    for (int i = 0; i < hzList.Count + 1; i++)
                    {
                        hzList[rangeDhIndex - 1][i].gameObject.SetActive(true);
                    }
                    dh.SetActive(true);
                    hzMask.SetActive(false);                   
                    ShowDH(rangeDhIndex);
                    hzMask_1.SetActive(false);
                });               
                UnDrager(false);              
            }
            else
            {
                UnDrager(false);
                PlayEncourageVioce();
                SpineManager.instance.DoAnimation(african, "ren1", false);
                WaitTimeAndExcuteNext(1.5f, () => { UnDrager(true); });
                mono.StartCoroutine(_renIE);
            }
        }
        /// <summary>
        /// 播放人和帽子动画
        /// </summary>
        private void PlayHatAni()
        {
            african.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            hat.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);           
            SpineManager.instance.DoAnimation(african, "ren0", true);
            SpineManager.instance.DoAnimation(hat, hatAniName, true);
        }
        void InitializeAni(string designAniName)
        {                   
            if (dragerCompleteIndex > 1)
            {
                african.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                hat.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(african, "ren0", true);
                SpineManager.instance.DoAnimation(hat, hatAniName, true);
                PlayDecorate(designAniName);
            }
            if (dragerCompleteIndex > 2)
            {
                for (int i = 1; i < 4; i++)
                {
                    if (hats[i].gameObject.activeSelf)
                    {
                        string s = SpineManager.instance.GetCurrentAnimationName(hats[i].gameObject);
                        string re = s.Replace("1", "0");
                        hats[i].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(hats[i].gameObject, re, true);
                    }
                }
                SpineManager.instance.DoAnimation(hat, hatAniName, true);
                PlayDecorate(designAniName);
            }
            if (dragerCompleteIndex > 3)
            {
                for (int i = 1; i < 4; i++)
                {
                    if (hats[i].gameObject.activeSelf)
                    {
                        string s = SpineManager.instance.GetCurrentAnimationName(hats[i].gameObject);
                        string re = s.Replace("1", "0");
                        hats[i].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(hats[i].gameObject, re, true);
                    }
                }
                SpineManager.instance.DoAnimation(hat, hatAniName, true);
                PlayDecorate(designAniName);
            }

        }      
        private void PlayDecorate(string designAniName)
        {
            if (rangeDhIndex == 1)
            {
                SpineManager.instance.DoAnimation(hats[1].gameObject, designAniName, true);//bu               
            }
            if (rangeDhIndex == 2)
            {                                
                SpineManager.instance.DoAnimation(hats[2].gameObject, designAniName, true);//food               
            }
            if (rangeDhIndex == 3)
            {               
                SpineManager.instance.DoAnimation(hats[3].gameObject, designAniName, true);//ear
            }
        }      
        /// <summary>
        /// 播放失败动画
        /// </summary>
        /// <param name="index"></param>
        private void PlayFailAni(int index,string aniName)
        {         
            african.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            hat.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(african, "ren0", true);            
            hats[0].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            hats[1].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            hats[2].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);

            if (index == 1)
            {
                SpineManager.instance.DoAnimation(hats[0].gameObject, aniName, false);
            }
            if (index == 2)
            {
                SpineManager.instance.DoAnimation(hats[1].gameObject, aniName + (2), false);
                SpineManager.instance.DoAnimation(hats[0].gameObject, aniName, false);
            }
            if (index == 3)
            {
                SpineManager.instance.DoAnimation(hats[2].gameObject, aniName + (3), false);
                SpineManager.instance.DoAnimation(hats[1].gameObject, aniName + (2), false);
                SpineManager.instance.DoAnimation(hats[0].gameObject, aniName, false);
            }

            mono.StartCoroutine(_renIE);

        }

        /// <summary>
        /// 显示帽子装饰
        /// </summary>
        private void ShowPersonHats(int index, int dragerIndex, string animationName)
        {           
            string name = hzList[rangeDhIndex-1][dragerIndex].GetComponent<Image>().sprite.name;           
            if (name == personHats[personHatIndex].GetComponent<BellSprites>().sprites[rangeDhIndex].name && index < 4)
            {
                dragerCompleteIndex++;
                hats[rangeDhIndex].gameObject.SetActive(true);              
                designAniName = animationName + (rangeDhIndex+1).ToString();               
                InitializeAni(designAniName);
                hzList[rangeDhIndex - 1][dragerIndex].gameObject.SetActive(false);               
                if (dragerCompleteIndex < 4)
                {
                    WaitTimeAndExcuteNext(2.0f, () =>
                    {
                        mono.StartCoroutine(_renIE);
                        hzs[rangeDhIndex - 1].SetActive(false);
                        if (personHatsIndex.Count != 0)
                        {
                            rangeDhIndex = RandomDhIndex();
                        }
                        hzs[rangeDhIndex - 1].SetActive(true);
                        ShowHz(true);
                        ShowDH(rangeDhIndex);
                    });
                    PlayStarAnimation();
                }
                else
                {
                    //单个帽子装饰完成
                    dh.SetActive(false);
                    hzs[rangeDhIndex - 1].SetActive(false);
                    hzMask.SetActive(true);
                    WaitTimeAndExcuteNext(0.5f, () => { CalcBloodSlider(); });
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 5);                  
                    SpineManager.instance.DoAnimation(xem, "xem2", false, () =>
                    {                       
                        hzMask.SetActive(false);
                        PlayGameAgain();
                    });
                }
            }
            else
            {
                hzMask.SetActive(true);
                PlayEncourageVioce();
                WaitTimeAndExcuteNext(1.5f, () => { hzMask.SetActive(false); });
                mono.StopCoroutine(_renIE);
                PlayFailAni(dragerCompleteIndex, hatFailAniName);                
            }
        } 
        /// <summary>
        /// 显示或隐藏装饰
        /// </summary>
        /// <param name="isShow"></param>
        private void ShowHz(bool isShow)
        {
            for (int i = 0; i < hzList.Count + 1; i++)
            {
                hzList[rangeDhIndex - 1][i].gameObject.SetActive(isShow);
            }
        }
        /// <summary>
        /// 显示对话框里的图
        /// </summary>
        /// <param name="index"></param>
        private void ShowDH(int index)
        {
            if (index < 4)
            {
                personHats[personHatIndex].gameObject.SetActive(false);
                dh.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                SpineManager.instance.DoAnimation(dh, "dh", false, ()=>
                {
                    personHats[personHatIndex].gameObject.SetActive(true);
                    personHats[personHatIndex].sprite = personHats[personHatIndex].GetComponent<BellSprites>().sprites[index];
                    personHats[personHatIndex].transform.GetComponent<CanvasGroup>().alpha = 0;
                    personHats[personHatIndex].transform.GetComponent<CanvasGroup>().DOFade(1, 1);
                });                
            }           
        }
        private void ResetHatsIndex()
        {
            for (int i = 1; i < 4; i++)
            {
                personHatsIndex.Add(i);
            }
        }
        /// <summary>
        /// 随机生成零件
        /// </summary>
        /// <returns></returns>
        private int RandomDhIndex()
        {
            int tempIndex = -1;
            tempIndex = Random.Range(0, personHatsIndex.Count);
            personIndex= personHatsIndex[tempIndex];
            personHatsIndex.Remove(personHatsIndex[tempIndex]);
            return personIndex;
        }
        /// <summary>
        /// 帽子拖拽是否启用
        /// </summary>
        /// <param name="isActived"></param>
        private void UnDrager(bool isActived)
        {
            for (int i = 0; i < dragerHats.Length; i++)
            {
                dragerHats[i].isActived = isActived;
            }
            //for (int i = 0; i < dragerHats.Length; i++)
            //{
            //    dragerHats[i].isActived = isActived;
            //}
        }  
        /// <summary>
        /// 播放星星动画
        /// </summary>
        private void PlayStarAnimation()
        {          
            star.SetActive(true);
            hzMask.SetActive(true);
            SpineManager.instance.DoAnimation(star, "guang", false, () => { star.SetActive(false); hzMask.SetActive(false); });           
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4,10));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0, false);
        }
        /// <summary>
        /// 播放鼓励音效
        /// </summary>
        private void PlayEncourageVioce()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0,4));
        }
        private void PlayGameAgain()
        {           
            if (dragerCompleteIndex == 4 && hatIndexList.Count != 0)
            {                    
                GameAgainInit();
            }
            if (dragerCompleteIndex == 4 && hatIndexList.Count == 0)
            {
                SuccessGame();                
            }
        }
        /// <summary>
        /// 游戏重玩初始化
        /// </summary>
        private void GameAgainInit()
        {           
            WaitTimeAndExcuteNext(1.0f, () =>
            {                
                for (int i = 0; i < hzList.Count; i++)
                {                   
                    for (int j = 0; j < hzList[i].Length; j++)
                    {
                        Vector3 tempPos;
                        dicHzPos.TryGetValue(hzList[i][j], out tempPos);
                        hzList[i][j].gameObject.SetActive(true);
                        hzList[i][j].transform.position = tempPos;
                    }
                }
                isShow = true;
                rangeDhIndex = 1;
                if (personHatsIndex.Count == 0)
                {
                    ResetHatsIndex();
                }                
                InterInit();
                InterStart();
            });
           
        }
        /// <summary>
        /// 计算血量
        /// </summary>
        private void CalcBloodSlider()
        {
            emSlider.value -= 0.25f;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
        }
        /// <summary>
        /// 重置游戏
        /// </summary>
        private void HatInit()
        {
            dragerHat = curTrans.Find("dragerHat");
            dragerHats = dragerHat.GetComponentsInChildren<ILDrager>(true);            

            for (int i = 0; i < dragerHats.Length; i++)
            {
                dragerHats[i].gameObject.SetActive(true);              
            }                     
            hatMask = curTrans.Find("hatMask").gameObject;
            hatMask.SetActive(false);

            kp1 = curTrans.Find("hatSuccess/kp1").gameObject;
            kp1.SetActive(false);
            kp2 = curTrans.Find("hatSuccess/kp2").gameObject;
            kp2.SetActive(false);
            kp3 = curTrans.Find("hatSuccess/kp3").gameObject;
            kp3.SetActive(false);
            kp4 = curTrans.Find("hatSuccess/kp4").gameObject;
            kp4.SetActive(false);
        }
        /// <summary>
        /// 判断游戏是否成功
        /// </summary>
        private void SuccessGame()
        {
            dh.SetActive(false);
            african.SetActive(false);
            hat.SetActive(false);

            hatMask.SetActive(true);
            kp1.SetActive(true);
            kp1.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(kp1, "kp1", false, () =>
            {
                kp2.SetActive(true);
                kp2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(kp2, "kp2", false, () =>
                {
                    kp3.SetActive(true);
                    kp3.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(kp3, "kp3", false, () =>
                    {
                        kp4.SetActive(true);
                        kp4.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(kp4, "kp4", false, () =>
                        {
                            WaitTimeAndExcuteNext(2.0f, () =>
                            {
                                successSpine.SetActive(true);
                                caidaiSpine.SetActive(true);
                                playSuccessSpine();

                                hatMask.SetActive(false);
                                kp1.SetActive(false);
                                kp2.SetActive(false);
                                kp3.SetActive(false);
                                kp4.SetActive(false);
                                //EmSlider();
                                //GameAgainInit();
                            });
                        });
                    });
                });
            });
        }
    }
}
