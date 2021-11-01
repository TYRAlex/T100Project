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
    public class TD5633Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        //bg
        private GameObject Bg;
        private BellSprites bellTextures;

        //scorePage部分
        private GameObject scorePage;
        private Image devilImage;
        private Image em;
        private BellSprites devilSprite;
        private BellSprites emSprite;
        private int emIndex;
        private bool isFinish = false;

        //dingDing(人物）
        private GameObject bd;
        private GameObject dbd;

        private GameObject mask;
        private Transform anyBtns;      
        //Spine部分
        private Transform treesParent;
        private Transform[] treeSpines;
        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject light;
        private GameObject star;
        //house部分
        private List<int> indexImage;
        private List<GameObject> houses;
        private Transform drager;
        private ILDrager[] dragers;
        private ILDroper[] dropers;
        private Image[] houseImage;
        private Image[] dragerImage;
        private Transform pink;
        private Transform blue;
        private Transform yellow;
        private Transform purple;
        private Vector3 dragerPos;

        private GameObject td;
        private GameObject td_G;
        //胜利动画名字
        private string tz;
        private string sz;



        #region 情景对话 用于情景对话，需要的自行复制在 Dialogues路径下找对应spine

        private GameObject buDing;
        private GameObject devil;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;

        private float textSpeed;
        private Text buDingText;
        private Text devilText;
        #endregion                                                         
        //创作指引完全结束     
        bool isPlaying = false;




        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            SoundManager.instance.ShowVoiceBtn(false);

            talkIndex = 1;
            textSpeed = 0.1f; //打字效果速度           
            emIndex = 0;

            FindInit();
            VoiceBtnInit();
            FindSpine();
            ShowOrHide();
            LoadSuccessPanel();
            //DevilSlide();          
            HouseInit(houses[0], 0);
        }
        /// <summary>
        /// 查找初始化
        /// </summary>
        private void FindInit()
        {
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            devilImage = curTrans.Find("scorePage/devilImage").GetComponent<Image>();
            em = curTrans.Find("scorePage/em").GetComponent<Image>();           
            devilSprite = devilImage.GetComponent<BellSprites>();
            emSprite = em.GetComponent<BellSprites>();
            em.sprite = emSprite.sprites[0];
            scorePage = curTrans.Find("scorePage").gameObject;
            scorePage.SetActive(true);
            devilImage.sprite = devilSprite.sprites[0];

            bd = curTrans.Find("mask/BD").gameObject;
            dbd = curTrans.Find("mask/DBD").gameObject;
            mask = curTrans.Find("mask").gameObject;           
            anyBtns = curTrans.Find("mask/Btns");           

            //对话部分
            bdStartPos = curTrans.Find("mask/bdStartPos");
            bdEndPos = curTrans.Find("mask/bdEndPos");
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devilEndPos = curTrans.Find("mask/devilEndPos");

            buDing = curTrans.Find("mask/buDing").gameObject;
            devil = curTrans.Find("mask/devil").gameObject;

            buDingText = buDing.transform.Find("buDingText").GetComponent<Text>();
            devilText = devil.transform.Find("devilText").GetComponent<Text>();
            buDingText.text = string.Empty;
            devilText.text = string.Empty;

            buDing.transform.position = bdStartPos.position;
            devil.transform.position = devilStartPos.position;

            drager = curTrans.Find("house/drager");
            pink = curTrans.Find("house/pink");
            blue = curTrans.Find("house/blue");
            yellow = curTrans.Find("house/yellow");
            purple = curTrans.Find("house/purple");

            houses = new List<GameObject>();
            houses.Add(pink.gameObject);
            houses.Add(blue.gameObject);
            houses.Add(yellow.gameObject);
            houses.Add(purple.gameObject);
            indexImage = new List<int>();

            td = curTrans.Find("TD/TD_1").gameObject;
            td_G = curTrans.Find("TD/TD_G").gameObject;
            SpineManager.instance.DoAnimation(td, "TS0", false, () => { SpineManager.instance.DoAnimation(td, "TS1"); });

            dragers = drager.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < dragers.Length; i++)
            {
                dragers[i].index = i;
            }

        }
        /// <summary>
        /// 查找Spine
        /// </summary>
        private void FindSpine()
        {
            treesParent = curTrans.Find("treeSpine");
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            light = curTrans.Find("light").gameObject;
            star = curTrans.Find("star").gameObject;

            treeSpines = treesParent.GetComponentsInChildren<Transform>(true);
            TreeSprinesInit();

        }
        /// <summary>
        /// 树的初始化
        /// </summary>
        private void TreeSprinesInit()
        {
            for (int i = 0; i < treeSpines.Length; i++)
            {
                treeSpines[i].gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 显示与隐藏
        /// </summary>
        private void ShowOrHide()
        {
            bd.SetActive(false);
            dbd.SetActive(false);

            buDing.SetActive(true);
            devil.SetActive(true);

            successSpine.SetActive(false);
            caidaiSpine.SetActive(false);
            light.SetActive(false);
            star.SetActive(false);
            td_G.SetActive(false);

            treesParent.gameObject.SetActive(true);
            mask.SetActive(true);                   

            anyBtns.gameObject.SetActive(false);
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);

            ShowHouseInit();

        }
        /// <summary>
        /// 房子显示初始化
        /// </summary>
        private void ShowHouseInit()
        {
            houses[0].gameObject.SetActive(true);
            for (int i = 1; i < houses.Count; i++)
            {
                houses[i].gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 语音键的初始化
        /// </summary>
        private void VoiceBtnInit()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
        }
        /// <summary>
        /// 播放游戏BGM
        /// </summary>
        private void PlayGameBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 3, true);
        }
        /// <summary>
        /// 恶魔滑入
        /// </summary>
        private void DevilSlide()
        {
            devil.transform.DOMove(devilEndPos.position, 1).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, () => { ShowDialogue("哈哈哈哈，我来捣蛋咯", devilText); }, () =>
                 {
                     buDing.transform.DOMove(bdEndPos.position, 1).OnComplete(() =>
                     {
                         mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, () => { ShowDialogue("不要怕，我们一起赶跑它吧", buDingText); }, () =>
                          {
                              SoundManager.instance.ShowVoiceBtn(true);
                          }));
                     });
                 }));
            });
        }
        /// <summary>
        /// 加载成功环节
        /// </summary>
        void LoadSuccessPanel()
        {
            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
        }
        /// <summary>
        /// 定义按钮mode
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
        #region 说话语音
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

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

        #region 语音键对应方法       
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
            buDing.SetActive(false);
            devil.SetActive(false);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null, () =>
            {
                mask.SetActive(false); bd.SetActive(false); td_G.SetActive(true);
            }));
        }
        /// <summary>
        /// 播放点击语音键按钮的音效
        /// </summary>
        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        #endregion        
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
        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
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
                        anyBtns.gameObject.SetActive(false);                       
                        isPlaying = false;
                        DevilSlide();
                        PlayGameBGM();
                        HouseInit(houses[0], 0);
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); isPlaying = false; GameReset(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 3)); });
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
        /// <summary>
        /// 显示对话内容（实现打字效果）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="text"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
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
                            caidaiSpine.SetActive(false); 
                            successSpine.SetActive(false); ac?.Invoke();
                        });
                });
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

        List<GameObject> temList;
        /// <summary>
        /// 房子初始化
        /// </summary>
        private void HouseInit(GameObject trans, int colorIndex)
        {

            groupIndex = colorIndex;
            houseImage = trans.GetComponentsInChildren<Image>();
            dragerImage = drager.GetComponentsInChildren<Image>();
            dropers = trans.GetComponentsInChildren<ILDroper>(true);
            UnDrager(false);
            SpineManager.instance.DoAnimation(td_G, "kong", false, () =>
            {              
                SpineManager.instance.DoAnimation(td_G, "TS-G", false,()=> { UnDrager(true); });
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            });          
            if (temList == null)
            {
                temList = new List<GameObject>();
            }
            else
            {
                temList.Clear();
            }

            for (int i = 0; i < houseImage.Length; i++)
            {
                //设置droper的index
                dropers[i].index = i;
                indexImage.Add(i);
                dropers[i].SetDropCallBack(null);
                temList.Add(dropers[i].gameObject);
            }
            for (int i = 0; i < houseImage.Length; i++)
            {
                int index_1 = Random.Range(0, indexImage.Count);                              
                houseImage[i].sprite = houseImage[i].GetComponent<BellSprites>().sprites[indexImage[index_1]];

                dragerImage[i].sprite = dragerImage[i].GetComponent<BellSprites>().sprites[colorIndex * 5 + indexImage[index_1]];

                dragers[i].dragType = colorIndex * 5 + indexImage[index_1];
                dropers[i].dropType = indexImage[index_1];
                dragers[i].drops = null;
                dragers[i].AddDrops(temList.ToArray());
                dropers[i].SetDropCallBack(OnAfter);
                indexImage.Remove(indexImage[index_1]);

            }           
            DragerInit();
        }
        /// <summary>
        /// 拖拽初始化
        /// </summary>
        private void DragerInit()
        {
            for (int i = 0; i < dragers.Length; i++)
            {
                
                dragers[i].gameObject.SetActive(true);
                dragers[i].GetComponent<ILDrager>().isActived = true;
                dragers[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
            }
        }
        /// <summary>
        /// 拖拽隐藏
        /// </summary>
        private void DragerHide(int index)
        {
            for (int i = 0; i < dragers.Length; i++)
            {
                dragers[i].gameObject.SetActive(false);
            }
            dragers[index].gameObject.SetActive(true);
        }
        /// <summary>
        /// 拖拽显示
        /// </summary>
        private void DragerShow()
        {
            for (int i = 0; i < dragers.Length; i++)
            {
                dragers[i].gameObject.SetActive(true);
            }
        }
        //记录拖拽下标的临时变量
        int temp = 0;
        int groupIndex = 0;
        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            temp = 0;
            temp = index;
            DragerHide(index);

            dragerPos = dragers[index].transform.position;
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
            //dragers[index].transform.position = Input.mousePosition;
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            dragers[index].DoReset();
            //dragers[index].transform.position = dragerPos;
            DragerShow();
            if (isMatch)
            {                                      
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
            }
        }
        private bool OnAfter(int dragType, int index, int dropType)
        {           
            dropers[index].GetComponent<Image>().sprite = dropers[index].GetComponent<BellSprites>().sprites[dragType % 5];
            dropers[temp].GetComponent<Image>().sprite = dropers[temp].GetComponent<BellSprites>().sprites[dropType];


            dragers[index].GetComponent<Image>().sprite = dragers[index].GetComponent<BellSprites>().sprites[dragType];
            dragers[temp].GetComponent<Image>().sprite = dragers[temp].GetComponent<BellSprites>().sprites[dropType + groupIndex * 5];

            dropers[index].dropType = dragType % 5;
            dragers[index].dragType = dragType;
            dropers[temp].dropType = dropType;
            dragers[temp].dragType = dropType + groupIndex * 5;

            DropersFinish();
            return true;
        }
        /// <summary>
        /// 判断是否拖拽完成
        /// </summary>
        private void DropersFinish()
        {
            if (dropers[0].GetComponent<ILDroper>().index == dropers[0].GetComponent<ILDroper>().dropType && dropers[1].GetComponent<ILDroper>().index == dropers[1].GetComponent<ILDroper>().dropType && dropers[2].GetComponent<ILDroper>().index == dropers[2].GetComponent<ILDroper>().dropType && dropers[3].GetComponent<ILDroper>().index == dropers[3].GetComponent<ILDroper>().dropType && dropers[4].GetComponent<ILDroper>().index == dropers[4].GetComponent<ILDroper>().dropType)
            {
                isFinish = true;
                UnDrager(false);
                ShowStar();                
                WaitTimeAndExcuteNext(1.867f, () => 
                {
                    CalcScore();
                });              
            }
        }
        private void UnDrager(bool isDrager)
        {
            for (int i = 0; i < dragers.Length; i++)
            {
                dragers[i].GetComponent<ILDrager>().isActived = isDrager;
            }
        }
        /// <summary>
        /// 显示星星
        /// </summary>
        private void ShowStar()
        {
            star.SetActive(true);
            SpineManager.instance.DoAnimation(star, "kong", false, () => 
            {
                SpineManager.instance.DoAnimation(star, "guang", false);
            });
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10));
        }
        /// <summary>
        /// 计算分数
        /// </summary>
        private void CalcScore()
        {
            if (isFinish && emIndex < houses.Count)
            {
                emIndex++;
                em.sprite = emSprite.sprites[emIndex];
                if (emIndex == 4)
                {
                    devilImage.sprite = devilSprite.sprites[1];
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,5, false);
                }               
                treesParent.gameObject.SetActive(true);
                treeSpines[emIndex].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                treeSpines[emIndex].gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(treeSpines[emIndex].gameObject, "kong", false, () =>
                {
                    SpineManager.instance.DoAnimation(treeSpines[emIndex].gameObject, "SHU" + emIndex, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                });
               
                mono.StartCoroutine(WaitCoroutine(2));
            }
        }
        IEnumerator WaitCoroutine(int time)
        {           
            yield return new WaitForSeconds(time);
            if (emIndex < houses.Count)
            {
                houses[emIndex - 1].gameObject.SetActive(false);
                houses[emIndex].gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(td, "TS0",false,()=> { SpineManager.instance.DoAnimation(td, "TS" + (emIndex + 1)); });
                HouseInit(houses[emIndex], emIndex);
                UnDrager(false);
                SpineManager.instance.DoAnimation(td_G, "kong", false, () =>
                {                    
                    SpineManager.instance.DoAnimation(td_G, "TS-G", false,()=> { UnDrager(true); });
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                });
            }
            if (emIndex == 4)
            {               
                WaitTimeAndExcuteNext(1.0f, () => 
                {
                    light.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    scorePage.SetActive(false);
                    td_G.SetActive(false);
                    mono.StartCoroutine(PlaySuccess(1.5f));
                });                
            }
        }       
        IEnumerator PlaySuccess(float time)
        {
            yield return new WaitForSeconds(time);
            light.SetActive(false);
            playSuccessSpine();            
        }
        /// <summary>
        /// 游戏的重置
        /// </summary>
        private void GameReset()
        {           
            scorePage.SetActive(true);
            devilImage.sprite = devilSprite.sprites[0];
            em.sprite = emSprite.sprites[0];                       


            emIndex = 0;
            SpineManager.instance.DoAnimation(td, "TS0", false, () => { SpineManager.instance.DoAnimation(td, "TS" + (emIndex + 1)); });
            td_G.SetActive(true);
            SpineManager.instance.DoAnimation(td_G, "kong", false, () =>
            {
                SpineManager.instance.DoAnimation(td_G, "TS-G", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            });

            ShowHouseInit();
            HouseInit(houses[0], 0);

            anyBtns.gameObject.SetActive(false);
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }           
            TreeSprinesInit();           
        }
    }
}
