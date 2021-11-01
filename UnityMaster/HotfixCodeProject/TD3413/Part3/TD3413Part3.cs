using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
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

    public enum E_LadybugColor
    {
        B=1,Y,R,P
    }

    public enum E_BagColorSpineType
    {
        Start,
        Shake,
        Animation,
        None
    }

    public enum E_LadybugStatu
    {
        
    }


    public class TD3413Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject di;
        private GameObject dd;
        private GameObject dbd;
        private GameObject xem;
        private GameObject Bg;
       

        private GameObject anyBtns;
        

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private Transform mask;

        private Dictionary<string, GameObject> _allBagDic;
        private List<E_LadybugColor> _gameLeftBagList;
        private List<int> _allBugColorTypeNumber;

        private GameObject _ladyBug;

        private GameObject _ladyBugEat;

        private E_LadybugColor _currentLadyColor = E_LadybugColor.B;

        private Transform _ladyBugEndPos;

        private bool _isGamePlaying = false;

        private bool _isDragTheLadyBug = false;

        private bool _isLadyBugDispear = false;

        private int _currentRandomNumber = -1;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            
            InializedAllProperty();
            ShowTheBFButton();
           
        }
        void ShowTheBFButton()
        {
            anyBtns.Show();
            anyBtns.transform.GetChild(0).gameObject.Show();
            anyBtns.transform.GetChild(1).gameObject.Hide();
            anyBtns.transform.GetChild(0).name = getBtnName(BtnEnum.bf,0);
            
            
        }

        void InializedAllProperty()
        {
            LoadAllObject();
            
            ReSetAllGameProperty();
           
            
        }

        void ReSetAllGameProperty()
        {
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            di.SetActive(false);
            dd.SetActive(false);
            xem.Hide();
            dbd.Hide();
            Bg = curTrans.Find("Bg").gameObject;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            float randomPosX = Random.Range(Screen.width * -500f / 1920f, Screen.width * 500f / 1920f);
            _ladyBug.transform.localPosition = new Vector3(randomPosX, Screen.height/2f, 0);
            _ladyBug.Hide();
            
            _ladyBugEat.Hide();
            _allBagDic=new Dictionary<string, GameObject>();
            ResetAllBugColorTypeNumber();
            //ChangeLadybugColorAndShow();
            _isDragTheLadyBug = false;
            ResetGameLeftBag();
            ResetAllBagObject();
            _isGamePlaying = false;
            _isLadyBugDispear = false;
            
        }

        void LoadAllObject()
        {
            di = curTrans.Find("di").gameObject;
            dd = curTrans.Find("DD").gameObject;
            dbd = curTrans.GetGameObject("DBD");
            xem = curTrans.GetGameObject("XEM");
            mask = curTrans.Find("mask");
            mask.gameObject.Show();
            caidaiSpine = mask.GetGameObject("caidai");
            AddButtonEvent();
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            _ladyBug = curTrans.GetGameObject("LadyBug");
            _ladyBugEndPos = curTrans.GetTransform("LadyBugEndPos");
            _ladyBugEat = curTrans.GetGameObject("LadyBugEat");
            _ladyBug.GetComponent<ILDrager>().SetDragCallback(DragLadybugStart,null,DragLadyBugEnd);
            LoadAllDropObject();
           
        }

        void AddButtonEvent()
        {
            anyBtns = curTrans.Find("mask/Btn").gameObject;
            for (int i = 0; i < anyBtns.transform.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.transform.GetChild(i).gameObject, OnClickAnyBtn);
            }
        }

        void ResetGameLeftBag()
        {
            _gameLeftBagList=new List<E_LadybugColor>();
            foreach (E_LadybugColor target in Enum.GetValues(typeof(E_LadybugColor)))
            {
                _gameLeftBagList.Add(target);
            }
        }

        void ResetAllBugColorTypeNumber()
        {
            _allBugColorTypeNumber=new List<int>();
            for (int i = 1; i < 5; i++)
            {
                _allBugColorTypeNumber.Add(i);
            }
        }

        void LoadAllDropObject()
        {
            Transform allDropParent = curTrans.GetTransform("BoxPuArea");
            for (int i = 0; i < allDropParent.childCount; i++)
            {
                Transform target= allDropParent.GetChild(i);
                target.GetComponent<ILDroper>().SetDropCallBack(DropCallBack);
            }
        }

        void ResetAllBagObject()
        {
            Transform allBag = curTrans.GetTransform("BoxPanel");
            for (int i = 0; i < allBag.childCount; i++)
            {
                GameObject target = allBag.GetChild(i).gameObject;
                _allBagDic.Add(target.name, target);
                PlaySpineAni(target, GetBagColorSpineName(target.name, E_BagColorSpineType.Start));

            }
        }

        void FindTargetBagAndPlaySpine(E_LadybugColor bagName,E_BagColorSpineType spineType)
        {
            GameObject target = _allBagDic["daizi-" + bagName.ToString().ToLower()];
            PlayBagCurretnState(target, spineType);
        }

        void PlayBagCurretnState(GameObject target,E_BagColorSpineType spineType)
        {
            string targetName = SpineManager.instance.GetCurrentAnimationName(target);
            Debug.Log(targetName);
            if (targetName.Equals(target.name)|| targetName.Equals(target.name+"5"))
                PlaySpineAni(target, GetBagColorSpineName(target.name, spineType, true));
            else
                PlaySpineAni(target, GetBagColorSpineName(target.name, spineType));
        }

        string GetBagColorSpineName(string bagName,E_BagColorSpineType spineType,bool isGetTheBug=false)
        {
            StringBuilder targetName = new StringBuilder();
            targetName.Append("daizi-" + bagName.Split('-')[1]);
            if (bagName != null)
            {
                switch (spineType)
                {
                    case E_BagColorSpineType.None:
                        targetName.Append("2");
                        break;
                    case E_BagColorSpineType.Start:
                        targetName.Append("3");
                        break;
                    case E_BagColorSpineType.Shake:
                        if (isGetTheBug)
                            targetName.Append("5");
                        else
                            targetName.Append("4");
                        break;
                }
            }

            return targetName.ToString();
        }


        


        void ChangeLadybugColorAndShow()
        {
            _ladyBug.Show();
            _currentRandomNumber = GetRandomNumber();
            //Debug.Log(_currentRandomNumber);
            //E_LadybugColor color = (E_LadybugColor)Enum.ToObject(typeof(E_LadybugColor), 2);
            E_LadybugColor color = (E_LadybugColor) _currentRandomNumber;
            Debug.Log("Color:" + color);
            _currentLadyColor = color;
            PlayCurrentLadyBugStatu(true);
        }

        int GetRandomNumber()
        {
            int randomNumber = -1;
            if (_allBugColorTypeNumber.Count > 0)
            {
                int targetNumber = Random.Range(0, _allBugColorTypeNumber.Count);
                randomNumber = _allBugColorTypeNumber[targetNumber];
                
            }

            return randomNumber;
        }

        void ReMoveBugColorTypeNumber(int number)
        {
            _allBugColorTypeNumber.Remove(number);
        }

        void PlayCurrentLadyBugStatu(bool isShow)
        {
            string ladybugFrontName = "c-" + _currentLadyColor.ToString().ToLower();
            if (isShow)
            {
                PlaySpineAni(_ladyBug, ladybugFrontName+"2", true);
            }
            else
            {
                //PlaySpineAni(_ladyBug, ladybugFrontName, false, () => _ladyBug.Hide());
                _ladyBug.Hide();
            }
        }


        void ShowLadyBug()
        {
            ChangeLadybugColorAndShow();
            float randomPosX = Random.Range(Screen.width * -500f / 1920f, Screen.width * 500f / 1920f);
            _ladyBug.transform.localPosition = new Vector3(randomPosX, Screen.height/2f, 0);
            Debug.Log("瓢虫开始的位置：" + _ladyBug.transform.localPosition);
            //mono.StartCoroutine("LadyBugMoveToTheEnd");
            mono.StartCoroutine(LadyBugMoveToTheEnd());
        }

        void LadybugChangToEatFood()
        {
            _ladyBugEat.Show();
            Debug.LogError("55555");
            PlaySpineAni(_ladyBugEat, "w-" + _currentLadyColor.ToString().ToLower(), false,
                () =>
                {
                    Debug.Log("0000000");
                    WaitTimeAndExcuteNext(2f, () =>
                    {
                        _ladyBugEat.Hide();
                        Debug.Log("22");
                        ShowLadyBug();
                    });
                });
        }
        

        IEnumerator LadyBugMoveToTheEnd()
        {
            Debug.Log("开启移动携程");
            while (true)
            {
                if (!_isDragTheLadyBug)
                {
                    _ladyBug.transform.localPosition = Vector3.LerpUnclamped(_ladyBug.transform.localPosition,
                        _ladyBugEndPos.localPosition, Time.fixedDeltaTime*0.1f);
                    Vector2 direction = _ladyBugEndPos.localPosition - _ladyBug.transform.localPosition;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    _ladyBug.transform.rotation = Quaternion.AngleAxis(angle+90f, Vector3.forward);
                    //Debug.Log(Vector3.Distance(_ladyBug.transform.localPosition, _ladyBugEndPos.localPosition));
                    // Vector3 _nomalized = (_ladyBugEndPos.localPosition - _ladyBug.transform.localPosition).normalized;
                    // _ladyBug.transform.forward = _nomalized;
                    if (_isLadyBugDispear)
                    {
                        _isLadyBugDispear = false;
                        yield break;
                    }

                    //Debug.Log(Vector3.Distance(_ladyBug.transform.localPosition, _ladyBugEndPos.localPosition));
                }

                yield return new WaitForFixedUpdate();
                
                if (Vector3.Distance(_ladyBug.transform.localPosition, _ladyBugEndPos.localPosition) < 50f)
                {
                    break;
                }
            }
            Debug.Log("启动下一个");
            yield return new WaitForSeconds(0.2f);
            if (_isGamePlaying&&!_isDragTheLadyBug)
            {
                ShowLadyBug();
            }
        }

        private bool DropCallBack(int dragType, int index, int drogType)
        {

            // Debug.Log("DragType:" + dragType + " index:" + index + " DropType" + drogType);
            // if (dragType == drogType)
            // {
            //     
            // }
            JudgeIndexAndExcuteNext(index);

            return true;
        }

        void JudgeIndexAndExcuteNext(int index)
        {
            Debug.LogError("3333");
            _isDragTheLadyBug = false;
            _isLadyBugDispear = true;
            E_LadybugColor bagColor = (E_LadybugColor) index;
            if (_gameLeftBagList.Contains(bagColor)&&_currentLadyColor == bagColor)
            {
                FindTargetBagAndPlaySpine(bagColor, E_BagColorSpineType.Animation);
                ReMoveBugColorTypeNumber(_currentRandomNumber);
                PlayCurrentLadyBugStatu(false);
                
                BtnPlaySoundSuccess(()=>JudgeColorTypeAndRemove(bagColor));
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            }
            else
            {
                FindTargetBagAndPlaySpine(bagColor, E_BagColorSpineType.Shake);
                LadybugChangToEatFood();
                BtnPlaySoundFail();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                //WaitTimeAndExcuteNext(2f, () => ShowLadyBug());
            }
        }

        void JudgeColorTypeAndRemove(E_LadybugColor colorType)
        {
            if (_gameLeftBagList.Contains(colorType))
            {
                _gameLeftBagList.Remove(colorType);
                
            }

            if (_gameLeftBagList.Count <=0)
            {
                WinTheGame();
            }
            else
            {
                Debug.Log("执行一次");
                WaitTimeAndExcuteNext(1f,()=>ShowLadyBug());
            }
        }

        void WinTheGame()
        {
            _isGamePlaying = false;
            mask.gameObject.Show();
            xem.Show();
            Talk(xem,2,null, () =>
            {
                xem.Hide();
                playSuccessSpine();
            });
           

        }

        private void DragLadybugStart(Vector3 position,int type,int index)
        {
            //Debug.Log("position" + position + " type:" + type + " index:" + "isMath:" );
            //mono.StopCoroutine("LadyBugMoveToTheEnd");
           
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            _isDragTheLadyBug = true;
        }

        private void DragLadyBugEnd(Vector3 position,int type,int index,bool isMath)
        {
            
            if (isMath)
            {
                _ladyBug.Hide();
            }
            else
            {
                _isDragTheLadyBug = false;
            }
            // else
            // {
            //     mono.StartCoroutine("LadyBugMoveToTheEnd");
            // }
        }


        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum,int index)
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
            SpineManager.instance.DoAnimation(anyBtns.transform.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if(_isGamePlaying)
                return;
            _isGamePlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () => {
                if (obj.name=="bf")
                {
                    //mask.gameObject.SetActive(false);
                    //Debug.Log("ssss");
                    anyBtns.Hide();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                    Talk(dd,0,null, () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                        _isGamePlaying = false;
                    });
                   
                }
                else if (obj.name == "ok")
                {
                    //mask.gameObject.Hide();
                    //小恶魔出现播放语音：“啊啊啊，你们给我等着！” 布丁从下方出现同时播放语音：小朋友们非常感谢你们帮主瓢虫家族守护住它们到的家园
                    anyBtns.Hide();
                    SoundManager.instance.Stop("bgm");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4);
                    Talk(dbd, 3, null, () => _isGamePlaying = false);

                }
                else if(obj.name=="fh")
                {
                   
                    mask.gameObject.Hide();
                    anyBtns.Hide();
                    GameInit();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                }
            
            });
        }

        private void GameInit()
        {
           ReSetAllGameProperty();
           _isGamePlaying = true;
          
           ShowLadyBug();
        }

        void GameStart()
        {
            
            
            //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0));
            ShowLadyBug();

        }
       
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    Talk(dd,1,null, () =>
                    {
                        dd.Hide();
                        _isGamePlaying = true;
                        mask.gameObject.Hide();
                        GameStart();
                        
                    });
                    
                    break;
                case 2:
                    
                    break;
                case 3:
                    break;
            }
            talkIndex++;
        }
        
        

        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.gameObject.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, "3-5-z", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, "3-5-z2", false,
                        () =>
                        {
                            anyBtns.gameObject.SetActive(true);
                            GameObject fh = anyBtns.transform.GetChild(0).gameObject;
                            GameObject ok = anyBtns.transform.GetChild(1).gameObject;
                    
                            fh.Show();
                            ok.Show();
                            fh.name = getBtnName(BtnEnum.fh,0);
                            ok.name = getBtnName(BtnEnum.ok, 1);
                    
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }
        

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess(Action callback = null)
        {
            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
            WaitTimeAndExcuteNext(timer, callback);
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
        
        //bell说话协程
        IEnumerator SpeckerCoroutine(GameObject target, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            string daijiName = null;
            string speakName = null;
            if (target.name.Equals("XEM"))
            {
                daijiName = "daiji";
                speakName = "speak";
            }
            else
            {
                daijiName = "bd-daiji";
                speakName = "bd-speak";
            }

            SpineManager.instance.DoAnimation(target, daijiName);
            
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(target, speakName);

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(target, daijiName);
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        /// <summary>
        /// 播放BGM（用在只有一个BGM的时候）
        /// </summary>
        private void PlayBGM()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }

        /// <summary>
        /// 播放相应的Sound语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        private void PlaySound(int targetIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, targetIndex);
        }

        private void PlayVoice(int targetIndex,Action callback=null)
        {
            float voiceTimer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
            if (callback != null)
                WaitTimeAndExcuteNext(voiceTimer, callback);
        }

        void WaitTimeAndExcuteNext(float timer,Action callback)
        {
            
            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNextIE(float timer,Action callBack)
        {
            yield return new WaitForSeconds(timer);
            callBack?.Invoke();
            
        }

    }
}
