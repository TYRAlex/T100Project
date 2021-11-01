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
    public class TD3442Part5
    {

        public enum DragerEnum
        {
            dragerCollor,
            dragerExpression,
            NULL
        }
        public enum DragerNum
        {
            first,
            second,
            third,
            Null
        }
        public enum EmpyNum
        {
            first,
            second,
            third,
            Null
        }
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private GameObject Bg;
        private BellSprites bellTextures;

       
        private GameObject bd;
        private GameObject dbd;
        
         
        private Transform anyBtns;
        private GameObject mask;

      
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;

        bool isPressBtn = false;
        private int flag = 0;
        //创作指引完全结束
        bool isEnd = false;
        bool isPlaying = false;

        private GameObject xem_1;
        private GameObject haystack_0;
        private GameObject haystack_1;

        private Transform leftPanel;
        private GameObject[] _leftPanel;

        private Transform droper;
        private Transform[] _droper;
        private ILDroper[] _droper_0;
        private ILDroper[] _droper_1;
        private ILDroper[] _droper_2;
        private ILDroper[] _droper_3;


        private Transform dragerColor;
        private Transform[] _dragerColor;
        private ILDrager[] _dragerColor_0;
        private ILDrager[] _dragerColor_1;
        private ILDrager[] _dragerColor_2;

        private Transform eag;
        private GameObject[] _eag;
        private Image[] _eag_0;
        private Image[] _eag_1;
        private Image[] _eag_2;

        private Transform succeedEag;
        private GameObject[] _succeedEag;

        private Transform starEag;
        private GameObject[] _starEag;
        private Transform lightEag;
        private GameObject[] _lightEag;

        private Transform dragerExpression;
        private GameObject[] _dragerExpression;
        private Empty4Raycast[] _e4r_0;
        private Empty4Raycast[] _e4r_1;
        private Empty4Raycast[] _e4r_2;


        private Transform dragerExp;
        private ILDrager[] _dragerExp;

        private Transform chicken;
        private GameObject[] _chicken;

        private DragerEnum dragerEnum;
        private DragerNum dragerNum;
        private EmpyNum empyNum;

        private GameObject dragerMask;
        private int droperIndex;

        private Transform dragerExpPos;
        private Vector3[] _dragerExpPos;

        private Transform dragerChildPos;
        private Vector3[] _dragerChildPos;

        private Transform dragerColorPos_0;
        private Transform dragerColorPos_1;
        private Transform dragerColorPos_2;
        private Vector3[] _dragerColorPos_0;
        private Vector3[] _dragerColorPos_1;
        private Vector3[] _dragerColorPos_2;

        private Vector3 _dragerColorVectorPos;
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

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            FindInit();            
        }
        private void GameInit()
        {
            talkIndex = 1;
            isPressBtn = false;
            flag = 0;          
        }
        void GameStart()
        {
            isPlaying = false;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, true);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => 
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
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
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () => 
            { 
                mask.SetActive(false); bd.SetActive(false);
                InterGameStart();
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
          /// 田丁加载所有物体
          /// </summary>
          void TDLoadGameProperty()
          {
              mask = curTrans.Find("mask").gameObject;
              mask.SetActive(true);
              //任务对话方法加载
              //TDLoadDialogue();
              //加载人物
              TDLoadCharacter();
              //加载成功界面
              TDLoadSuccessPanel();
              //加载游戏按钮
              TDLoadButton();             ;

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
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); isPlaying = false; GameReset(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 3)); });
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
        private void GameReset()
        {            
            xem_1.Show();
            SpineManager.instance.DoAnimation(xem_1, "xem1", true);

            haystack_0.Show();
            haystack_1.Hide();

            _leftPanel[1].Hide();
            _leftPanel[0].Show();
            succeedEag.gameObject.Show();
            //拖拽颜色
            DragerInit(_dragerColor_0,_dragerColorPos_0);
            DragerInit(_dragerColor_1, _dragerColorPos_1);
            DragerInit(_dragerColor_2, _dragerColorPos_2);
            HideDragerColor();
            _dragerColor[0].gameObject.Show();

            for (int i = 0; i < _droper.Length; i++)
            {                
                _droper[i].gameObject.Show();
            }
            SetDroperSprite(_droper_0);
            SetDroperSprite(_droper_1);
            SetDroperSprite(_droper_2);            
            UnDroper(_droper_0, true);
            UnDroper(_droper_1, false);
            UnDroper(_droper_2, false);
            UnDroper(_droper_3, false);

            FindAndShowEag(_eag_0, true);
            FindAndShowEag(_eag_1, true);
            FindAndShowEag(_eag_2, true);

            for (int i = 0; i < _succeedEag.Length; i++)
            {               
                _succeedEag[i].Hide();
            }
          

            for (int i = 0; i < _starEag.Length; i++)
            {               
                _starEag[i].Hide();
                _lightEag[i].Hide();
            }
            for (int i = 0; i < _dragerExpression.Length; i++)
            {               
                _dragerExpression[i].Hide();
            }           

            for (int i = 0; i < _dragerExp.Length; i++)
            {
                _dragerExp[i].transform.parent.position = _dragerExpPos[i];
                _dragerExp[i].transform.parent.gameObject.Hide();
                _dragerExp[i].transform.localPosition= _dragerChildPos[i]; 
            }
            _dragerExp[0].transform.parent.gameObject.Show();
            for (int i = 0; i < _chicken.Length; i++)
            {               
                _chicken[i].Hide();
            }
            dragerEnum = DragerEnum.dragerCollor;
            dragerNum = DragerNum.first;
            empyNum = EmpyNum.first;

            //dragerMask.Hide();
            droperIndex = -1;
            InterGameStart();
        }
        private void FindInit()
        {
            xem_1 = curTrans.Find("xem/xem_1").gameObject;
            xem_1.Show();
            SpineManager.instance.DoAnimation(xem_1, "xem1", true);

            haystack_0 = curTrans.Find("haystack/0").gameObject;
            haystack_1 = curTrans.Find("haystack/1").gameObject;
            haystack_0.Show();
            haystack_1.Hide();

            leftPanel = curTrans.Find("left");
            _leftPanel = new GameObject[leftPanel.childCount];
            for (int i = 0; i < _leftPanel.Length; i++)
            {
                _leftPanel[i] = leftPanel.GetChild(i).gameObject;
                _leftPanel[i].Hide();
            }
            _leftPanel[0].Show();

            DroperAndDrager();

            eag = curTrans.Find("eag");
            _eag = new GameObject[eag.childCount];            
            for (int i = 0; i < _eag.Length; i++)
            {
                _eag[i] = eag.GetChild(i).gameObject;                              
            }
            _eag_0 = _eag[0].GetComponentsInChildren<Image>(true);
            _eag_1 = _eag[1].GetComponentsInChildren<Image>(true);
            _eag_2 = _eag[2].GetComponentsInChildren<Image>(true);
            FindAndShowEag(_eag_0, true);
            FindAndShowEag(_eag_1, true);
            FindAndShowEag(_eag_2, true);

            succeedEag = curTrans.Find("succeedEag");
            succeedEag.gameObject.Show();
            _succeedEag = new GameObject[succeedEag.childCount];
            for (int i = 0; i < _succeedEag.Length; i++)
            {
                _succeedEag[i] = succeedEag.GetChild(i).gameObject;
                _succeedEag[i].Hide();
            }

            starEag = curTrans.Find("starEag");
            _starEag = new GameObject[starEag.childCount];
            for (int i = 0; i < _starEag.Length; i++)
            {
                _starEag[i] = starEag.GetChild(i).gameObject;
                _starEag[i].Hide();
            }
            lightEag = curTrans.Find("lightEag");
            _lightEag = new GameObject[lightEag.childCount];
            for (int i = 0; i < _lightEag.Length; i++)
            {
                _lightEag[i] = lightEag.GetChild(i).gameObject;
                _lightEag[i].Hide();
            }

            dragerExpression = curTrans.Find("dragerExpression");
            _dragerExpression = new GameObject[dragerExpression.childCount];
            for (int i = 0; i < _dragerExpression.Length; i++)
            {
                _dragerExpression[i] = dragerExpression.GetChild(i).gameObject;
                _dragerExpression[i].Hide();
            }
            _e4r_0 = _dragerExpression[0].GetComponentsInChildren<Empty4Raycast>(true);
            _e4r_1 = _dragerExpression[1].GetComponentsInChildren<Empty4Raycast>(true);
            _e4r_2 = _dragerExpression[2].GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _e4r_0.Length; i++)
            {
                Util.AddBtnClick(_e4r_0[i].gameObject, EmptyClickEvent);
                Util.AddBtnClick(_e4r_1[i].gameObject, EmptyClickEvent);
                Util.AddBtnClick(_e4r_2[i].gameObject, EmptyClickEvent);
            }

            dragerExpPos = curTrans.Find("dragerExpPos");
            _dragerExpPos = new Vector3[dragerExpPos.childCount];
            for (int i = 0; i < _dragerExpPos.Length; i++)
            {
                _dragerExpPos[i] = dragerExpPos.GetChild(i).transform.localPosition;
            }

            dragerChildPos = curTrans.Find("dragerChildPos");
            _dragerChildPos = new Vector3[3];
            for (int i = 0; i < _dragerChildPos.Length; i++)
            {
                _dragerChildPos[i] = dragerChildPos.GetChild(i).GetChild(0).localPosition;
            }

            dragerExp = curTrans.Find("dragerExp");
            _dragerExp = dragerExp.GetComponentsInChildren<ILDrager>(true);           
            for (int i = 0; i < _dragerExp.Length; i++)
            {
                _dragerExp[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                _dragerExp[i].transform.parent.localPosition = _dragerExpPos[i];
                _dragerExp[i].transform.localPosition = _dragerChildPos[i];
                _dragerExp[i].transform.parent.gameObject.Hide();               
            }
            _dragerExp[0].transform.parent.gameObject.Show();

            chicken = curTrans.Find("chicken");
            _chicken = new GameObject[chicken.childCount];
            for (int i = 0; i < _chicken.Length; i++)
            {
                _chicken[i] = chicken.GetChild(i).gameObject;
                _chicken[i].Hide();
            }
            dragerEnum = DragerEnum.dragerCollor;
            dragerNum = DragerNum.first;

            dragerMask = curTrans.Find("dragerMask").gameObject;
            dragerMask.Show();

            droperIndex = -1;
        }
        /// <summary>
        /// 显示或隐藏鸡蛋线
        /// </summary>
        /// <param name="eag"></param>
        /// <param name="isTrue"></param>
        private void FindAndShowEag(Image[]eag,bool isTrue)
        {                       
            for (int i = 0; i < eag.Length; i++)
            {
                eag[i].gameObject.SetActive(isTrue);
            }
        }
        private void DroperAndDrager()
        {
            droper = curTrans.Find("droper");
            _droper = new Transform[droper.childCount];
            for (int i = 0; i < _droper.Length; i++)
            {
                _droper[i] = droper.GetChild(i);

                _droper[i].gameObject.Show();
            }
            _droper_0 = _droper[0].GetComponentsInChildren<ILDroper>(true);
            _droper_1 = _droper[1].GetComponentsInChildren<ILDroper>(true);
            _droper_2 = _droper[2].GetComponentsInChildren<ILDroper>(true);
            _droper_3 = _droper[3].GetComponentsInChildren<ILDroper>(true);
            DroperInit(_droper_0);
            DroperInit(_droper_1);
            DroperInit(_droper_2);
            DroperInit(_droper_3);

            SetDroperSprite(_droper_0);
            SetDroperSprite(_droper_1);
            SetDroperSprite(_droper_2);

            UnDroper(_droper_0, true);
           
            dragerColorPos_0 = curTrans.Find("dragerColorPos/drager_0");
            dragerColorPos_1 = curTrans.Find("dragerColorPos/drager_1");
            dragerColorPos_2 = curTrans.Find("dragerColorPos/drager_2");

            _dragerColorPos_0 = new Vector3[dragerColorPos_0.childCount];
            GetDragerColorPos(_dragerColorPos_0, dragerColorPos_0);

            _dragerColorPos_1 = new Vector3[dragerColorPos_1.childCount];
            GetDragerColorPos(_dragerColorPos_1, dragerColorPos_1);

            _dragerColorPos_2 = new Vector3[dragerColorPos_2.childCount];
            GetDragerColorPos(_dragerColorPos_2, dragerColorPos_2);

            dragerColor = curTrans.Find("dragerColor");
            _dragerColor = new Transform[dragerColor.childCount];
            for (int i = 0; i < _dragerColor.Length; i++)
            {
                _dragerColor[i] = dragerColor.GetChild(i);
            }
            _dragerColor_0 = _dragerColor[0].GetComponentsInChildren<ILDrager>(true);
            _dragerColor_1 = _dragerColor[1].GetComponentsInChildren<ILDrager>(true);
            _dragerColor_2 = _dragerColor[2].GetComponentsInChildren<ILDrager>(true);
            DragerInit(_dragerColor_0, _dragerColorPos_0);
            DragerInit(_dragerColor_1, _dragerColorPos_1);
            DragerInit(_dragerColor_2, _dragerColorPos_2);

            HideDragerColor();
            _dragerColor[0].gameObject.Show();
            
        }
        private void GetDragerColorPos(Vector3[] _dragerColorPos,Transform dragerColorPos)
        {
            for (int i = 0; i < _dragerColorPos.Length; i++)
            {
                _dragerColorPos[i] = dragerColorPos.GetChild(i).localPosition;
            }
        }
        private void DragerInit(ILDrager[] drager_Sum,Vector3[]_dragerColorPos)
        {
            for (int i = 0; i < drager_Sum.Length; i++)
            {
                drager_Sum[i].gameObject.Show();
                drager_Sum[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                drager_Sum[i].index = i;
                drager_Sum[i].transform.localPosition = _dragerColorPos[i];
            }
        }      
        private void DroperInit(ILDroper[] droper_Sum)
        {
            for (int i = 0; i < droper_Sum.Length; i++)
            {
                droper_Sum[i].SetDropCallBack(OnAfter);
                droper_Sum[i].index = i;
                droper_Sum[i].isActived = false;               
            }
        }
        private void SetDroperSprite(ILDroper[] droper_Sprite)
        {
            for (int i = 0; i < droper_Sprite.Length; i++)
            {
                droper_Sprite[i].transform.GetChild(0).GetComponent<Image>().sprite = droper_Sprite[i].transform.GetChild(0).GetComponent<BellSprites>().sprites[1];
            }
        }
        private void UnDroper(ILDroper[] droperActived,bool IsActived)
        {
            for (int i = 0; i < droperActived.Length; i++)
            {
                droperActived[i].isActived = IsActived;
            }
        }       
        private void HideDragerColor()
        {
            for (int i = 0; i < _dragerColor.Length; i++)
            {                
                _dragerColor[i].gameObject.Hide();
            }
        }
        private void InterGameStart()
        {
            _starEag[0].Show();
            _starEag[0].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_starEag[0], "g", false, () =>
            {
                WaitTimeAndExcuteNext(1.0f, () =>
                {
                    dragerMask.Hide();
                    _starEag[0].Hide();
                });
            });
        }      
        private bool OnAfter(int dragType, int index, int dropType)
        {          
            droperIndex = index;          
            return true;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {           
            dragerMask.Show();          
            if (dragerEnum == DragerEnum.dragerCollor)
            {
                if (dragerNum == DragerNum.first)
                {
                    _dragerColor_0[index].transform.SetAsLastSibling();
                    _dragerColorVectorPos = _dragerColor_0[index].transform.position;
                }
                else if (dragerNum == DragerNum.second)
                {
                    _dragerColor_1[index].transform.SetAsLastSibling();
                    _dragerColorVectorPos = _dragerColor_0[index].transform.position;
                }
                else if (dragerNum == DragerNum.third)
                {
                    _dragerColor_2[index].transform.SetAsLastSibling();
                    _dragerColorVectorPos = _dragerColor_0[index].transform.position;
                }                
            }
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }
        private Image droperImage;
        private int dragerSum;
        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {           
            if (dragerEnum == DragerEnum.dragerCollor)
            {
                if (dragerNum == DragerNum.first)
                {
                    //_dragerColor_0[index].DoReset();
                    _dragerColor_0[index].transform.position = _dragerColorVectorPos;
                }
                else if (dragerNum == DragerNum.second)
                {
                    //_dragerColor_1[index].DoReset();
                    _dragerColor_1[index].transform.position = _dragerColorVectorPos;
                }
                else if (dragerNum == DragerNum.third)
                {
                    //_dragerColor_2[index].DoReset();
                    _dragerColor_2[index].transform.position = _dragerColorVectorPos;
                }
            }
            else if(dragerEnum==DragerEnum.dragerExpression)
            {
                if (empyNum == EmpyNum.first)
                {
                    _dragerExp[0].DoReset();
                }
                else if (empyNum == EmpyNum.second)
                {
                    _dragerExp[1].DoReset();
                }
                else if (empyNum == EmpyNum.third)
                {
                    _dragerExp[2].DoReset();
                }
            }
            if (isMatch)
            {
                if (dragerEnum == DragerEnum.dragerCollor)
                {
                    if (dragerNum == DragerNum.first)
                    {
                        if (_dragerColor_0[index].gameObject.name == _droper_0[droperIndex].gameObject.name)
                        {
                            if (_dragerColor_0[index].gameObject.name == "0")
                            {
                                _eag_0[1].gameObject.Hide();
                            }
                            if (_dragerColor_0[index].gameObject.name == "1" || _dragerColor_0[index].gameObject.name == "2")
                            {
                                _eag_0[0].gameObject.Hide();
                            }
                            dragerSum++;
                            _dragerColor_0[index].gameObject.Hide();
                            droperImage = _droper_0[droperIndex].transform.GetChild(0).GetComponent<Image>();
                            droperImage.sprite = _droper_0[droperIndex].transform.GetChild(0).GetComponent<BellSprites>().sprites[0];
                            _starEag[0].Show();
                            _starEag[0].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            SpineManager.instance.DoAnimation(_starEag[0], "star", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () =>
                              {
                                  dragerMask.Hide();
                                  if (dragerSum == 3)
                                  {
                                      dragerMask.Show();
                                      dragerSum = 0;
                                      _succeedEag[0].Show();
                                      UnDroper(_droper_0, false);
                                      _starEag[0].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                      SpineManager.instance.DoAnimation(_starEag[0], "g", false,()=> 
                                      {                                        
                                          WaitTimeAndExcuteNext(1.0f, () =>
                                          {
                                              _starEag[0].Hide();                                             
                                              _dragerColor[0].gameObject.Hide();
                                              _dragerColor[1].gameObject.Show();
                                              _droper[0].gameObject.Hide();
                                              _droper[1].gameObject.Show();
                                              UnDroper(_droper_1, true);
                                              dragerNum = DragerNum.second;
                                              _starEag[1].Show();
                                              _starEag[1].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                              SpineManager.instance.DoAnimation(_starEag[1], "g", false, () =>
                                              {
                                                  WaitTimeAndExcuteNext(1.0f, () =>
                                                  {
                                                      dragerMask.Hide();
                                                      _starEag[1].Hide();
                                                  });
                                              });
                                          });
                                      });                                                                           
                                  }
                              }));
                        }
                        else 
                        {
                            PlayFailVoice();
                        }
                    }
                    else if (dragerNum == DragerNum.second)
                    {
                        if (_dragerColor_1[index].gameObject.name == _droper_1[droperIndex].gameObject.name)
                        {
                            if (_dragerColor_1[index].gameObject.name == "0")
                            {
                                _eag_1[1].gameObject.Hide();
                            }
                            if (_dragerColor_1[index].gameObject.name == "1" || _dragerColor_1[index].gameObject.name == "2")
                            {
                                _eag_1[0].gameObject.Hide();
                            }
                            dragerSum++;
                            _dragerColor_1[index].gameObject.Hide();
                            droperImage = _droper_1[droperIndex].transform.GetChild(0).GetComponent<Image>();
                            droperImage.sprite = _droper_1[droperIndex].transform.GetChild(0).GetComponent<BellSprites>().sprites[0];
                            _starEag[1].Show();
                            _starEag[1].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            SpineManager.instance.DoAnimation(_starEag[1], "star", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () =>
                            {
                                dragerMask.Hide();
                                if (dragerSum == 3)
                                {
                                    dragerMask.Show();
                                    dragerSum = 0;
                                    _succeedEag[1].Show();
                                    UnDroper(_droper_1, false);
                                    _starEag[1].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                    SpineManager.instance.DoAnimation(_starEag[1], "g", false, () =>
                                    {
                                        WaitTimeAndExcuteNext(1.0f, () =>
                                        {                                            
                                            _starEag[1].Hide();                                            
                                            _dragerColor[1].gameObject.Hide();
                                            _dragerColor[2].gameObject.Show();
                                            _droper[1].gameObject.Hide();
                                            _droper[2].gameObject.Show();
                                            UnDroper(_droper_2, true);
                                            dragerNum = DragerNum.third;
                                            _starEag[2].Show();
                                            _starEag[2].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                            SpineManager.instance.DoAnimation(_starEag[2], "g", false, () =>
                                            {
                                                WaitTimeAndExcuteNext(1.0f, () =>
                                                {
                                                    dragerMask.Hide();
                                                    _starEag[2].Hide();
                                                });
                                            });
                                        });
                                    });
                                }
                            }));
                        }
                        else
                        {
                            PlayFailVoice();
                        }
                    }
                    else if (dragerNum == DragerNum.third)
                    {
                        if (_dragerColor_2[index].gameObject.name == _droper_2[droperIndex].gameObject.name)
                        {
                            if (_dragerColor_2[index].gameObject.name == "0")
                            {
                                _eag_2[1].gameObject.Hide();
                            }
                            if (_dragerColor_2[index].gameObject.name == "1" || _dragerColor_2[index].gameObject.name == "2")
                            {
                                _eag_2[0].gameObject.Hide();
                            }
                            dragerSum++;
                            _dragerColor_2[index].gameObject.Hide();
                            droperImage = _droper_2[droperIndex].transform.GetChild(0).GetComponent<Image>();
                            droperImage.sprite = _droper_2[droperIndex].transform.GetChild(0).GetComponent<BellSprites>().sprites[0];
                            _starEag[2].Show();
                            _starEag[2].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            SpineManager.instance.DoAnimation(_starEag[2], "star", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () =>
                            {
                                dragerMask.Hide();
                                if (dragerSum == 3)
                                {
                                    dragerMask.Show();
                                    dragerSum = 0;
                                    _succeedEag[2].Show();
                                    UnDroper(_droper_0, false);
                                    _starEag[2].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                    SpineManager.instance.DoAnimation(_starEag[2], "g", false, () =>
                                    {
                                        WaitTimeAndExcuteNext(1.0f, () =>
                                        {                                           
                                            SpineManager.instance.DoAnimation(xem_1, "xem3", true);
                                            SpineManager.instance.DoAnimation(_starEag[2], "kong", false,()=>
                                            {
                                                _starEag[2].Hide();
                                                _starEag[1].Show();
                                                haystack_0.Hide();
                                                _leftPanel[0].Hide();
                                                _droper[2].gameObject.Hide();
                                                succeedEag.gameObject.Hide();
                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                                                _starEag[1].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                                SpineManager.instance.DoAnimation(_starEag[1], "c", false,()=> 
                                                {                                                  
                                                    _chicken[1].Show();
                                                    SpineManager.instance.DoAnimation(_chicken[1], "n", true);                                                   
                                                    SpineManager.instance.DoAnimation(_starEag[1], "d", false, () =>
                                                    {
                                                        SpineManager.instance.DoAnimation(xem_1, "xem1", true);
                                                        _starEag[1].Hide();                                                                                                             
                                                        _droper[3].gameObject.Show();
                                                        UnDroper(_droper_3, true);                                                       
                                                        _leftPanel[1].Show();
                                                        _dragerExpression[0].Show();
                                                        dragerNum = DragerNum.Null;
                                                        dragerEnum = DragerEnum.dragerExpression;
                                                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2,null,()=> 
                                                        {
                                                            dragerMask.Hide();                                                           
                                                        }));                                                         
                                                    });
                                                });                                                                                                
                                            });                                                                                                                                   
                                        });
                                    });
                                }
                            }));
                        }
                        else
                        {
                            PlayFailVoice();
                        }
                    }
                }
                else if(dragerEnum == DragerEnum.dragerExpression)
                {                   
                    if(empyNum == EmpyNum.first)
                    {
                        PlaySspAni("sspwjia2",0,1, 2, "n-y", 0, 1,EmpyNum.second);
                        PlaySspAni("sspwjib2", 0, 1, 2, "n-y2", 0, 1, EmpyNum.second);
                        PlaySspAni("sspwjic2", 0, 1, 2, "n-y3", 0, 1, EmpyNum.second);
                        PlaySspAni("sspwjid2", 0, 1, 2, "n-y4", 0, 1, EmpyNum.second);
                        PlaySspAni("sspwjie2", 0, 1, 2, "n-y5", 0, 1, EmpyNum.second);
                    }
                    else if(empyNum == EmpyNum.second)
                    {                       
                        PlaySspAni("sspjiha2", 1, 2, 0, "n-c5", 1, 2, EmpyNum.third);
                        PlaySspAni("sspjihb2", 1, 2, 0, "n-c", 1, 2, EmpyNum.third);
                        PlaySspAni("sspjihc2", 1, 2, 0, "n-c2", 1, 2, EmpyNum.third);
                        PlaySspAni("sspjihd2", 1, 2, 0, "n-c3", 1, 2, EmpyNum.third);
                        PlaySspAni("sspjihe2", 1, 2, 0, "n-c4", 1, 2, EmpyNum.third);
                    }
                    else if (empyNum == EmpyNum.third)
                    {                      
                        SucceedFinal("ssptjia2", 3, 2, "n-t", 2, EmpyNum.Null);
                        SucceedFinal("ssptjib2", 3, 2, "n-t2", 2, EmpyNum.Null);
                        SucceedFinal("ssptjic2", 3, 2, "n-t3", 2, EmpyNum.Null);
                        SucceedFinal("ssptjid2", 3, 2, "n-t4", 2, EmpyNum.Null);
                        SucceedFinal("ssptjie2", 3, 2, "n-t5", 2, EmpyNum.Null);
                    }
                }
            }
            else
            {
                PlayFailVoice();
                if(dragerEnum == DragerEnum.dragerExpression)
                {                                      
                    if (empyNum == EmpyNum.first)
                    {
                        _dragerExp[0].transform.parent.position = _dragerExpPos[0];
                    }
                    else if (empyNum == EmpyNum.second)
                    {
                        _dragerExp[1].transform.parent.position = _dragerExpPos[1];
                    }
                    else if (empyNum == EmpyNum.third)
                    {
                        _dragerExp[2].transform.parent.position = _dragerExpPos[2];
                    }
                }                
            }           
        } 
        private void PlayFailVoice()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), null, () =>
            {
                dragerMask.Hide();
            }));
        }
        private void PlaySspAni(string name,int indexExp, int indexExp_1, int indexCh,string aniName,int indexE_1,int indexE_2,EmpyNum empyNum_1)
        {
            dragerMask.Show();
            if (dragerContentParentName == name)
            {                             
                _dragerExp[indexExp].transform.GetChild(0).gameObject.Hide();
                _chicken[indexCh].Show();
                _chicken[indexCh].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                //SpineManager.instance.DoAnimation(_chicken[indexCh], "kong", false);
                SpineManager.instance.DoAnimation(_chicken[indexCh], aniName, true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);                
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () =>
                {
                    _dragerExp[indexExp].transform.GetChild(0).gameObject.Show();
                    _dragerExp[indexExp].transform.parent.gameObject.Hide();
                    _dragerExpression[indexE_1].Hide();
                    _dragerExpression[indexE_2].Show();                   
                    _dragerExp[indexExp_1].transform.parent.gameObject.Show();
                    dragerMask.Hide();
                    empyNum = empyNum_1;
                }));
            }           
        }
        private void SucceedFinal(string name,int indexCh,int indexExp,string aniName,int indexE_1,EmpyNum empyNum_1)
        {
            dragerMask.Show();
            if(dragerContentParentName == name)
            {
                _chicken[indexCh].Show();
                _dragerExp[indexExp].transform.parent.gameObject.Hide();
                SpineManager.instance.DoAnimation(_chicken[indexCh], aniName, true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
                SpineManager.instance.DoAnimation(xem_1, "xem3", true);
                _lightEag[1].Show();
                _leftPanel[1].Hide();
                _dragerExpression[indexE_1].Hide();
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null));
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SpineManager.instance.DoAnimation(_lightEag[1], "light", false, () => 
                {                    
                    _lightEag[1].Hide();                   
                    empyNum = empyNum_1;
                    playSuccessSpine();
                });               
            }
        }
        private string dragerContentParentName;        
        private Vector3 dragerContentParentPos;       
        private void EmptyClickEvent(GameObject obj)
        {
            dragerMask.Show();
            dragerContentParentName = obj.transform.parent.GetComponent<Image>().sprite.name;            
            dragerContentParentPos = obj.transform.parent.position;          
            if (empyNum == EmpyNum.first)
            {
                _dragerExp[0].transform.parent.position = dragerContentParentPos;                
                SetSprite("sspwjia2", 0,0);
                SetSprite("sspwjib2", 1, 0);
                SetSprite("sspwjic2", 2, 0);
                SetSprite("sspwjid2", 3, 0);
                SetSprite("sspwjie2", 4, 0);               
            }
            if (empyNum == EmpyNum.second)
            {
                _dragerExp[1].transform.parent.position = dragerContentParentPos;
                SetSprite("sspjiha2", 0,1);
                SetSprite("sspjihb2", 1, 1);
                SetSprite("sspjihc2", 2, 1);
                SetSprite("sspjihd2", 3, 1);
                SetSprite("sspjihe2", 4, 1);
            }
            if (empyNum == EmpyNum.third)
            {
                _dragerExp[2].transform.parent.position = dragerContentParentPos;
                SetSprite("ssptjia2", 0, 2);
                SetSprite("ssptjib2", 1, 2);
                SetSprite("ssptjic2", 2, 2);
                SetSprite("ssptjid2", 3, 2);
                SetSprite("ssptjie2", 4, 2);
            }
        }
        private void SetSprite(string name, int index,int indexDrager)
        {
            if (dragerContentParentName == name)
            {
                _dragerExp[indexDrager].transform.parent.GetComponent<Image>().sprite = _dragerExp[indexDrager].transform.parent.GetComponent<BellSprites>().sprites[index];
                _dragerExp[indexDrager].transform.GetChild(0).GetComponent<Image>().sprite = _dragerExp[indexDrager].transform.GetChild(0).GetComponent<BellSprites>().sprites[index];
                _dragerExp[indexDrager].transform.parent.GetComponent<Image>().SetNativeSize();
                _dragerExp[indexDrager].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
            }
        }
    }
}
