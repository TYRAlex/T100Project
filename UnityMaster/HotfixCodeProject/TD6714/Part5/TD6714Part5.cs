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
    

    public class TD6714Part5
    {
        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

     
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private Vector2 _roleStartPos;
        private Vector2 _roleEndPos;

        private Vector2 _enemyStartPos;
        private Vector2 _enemyEndPos;

        private List<string> _dialogues;

        private GameObject _dd;
        private GameObject _dTT;
        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _oKSpine;
        private GameObject _nextLevelSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;
        private GameObject _dialogue;
        private GameObject _uiMask;

        private RectTransform _devil;
        private RectTransform _dingDing;
        private RectTransform _tianTian;

        private Text _dingDingTxt;
        private Text _tianTianTxt;
        private Text _devilTxt;

        private Transform _cards;
        private Transform _levels;
        private Transform _level1;
        private Transform _btn1;
        private Transform _level2;
        private Transform _btn2;
        private Transform _level3;
        private Transform _btn3;
        private Transform _level4;
        private Transform _btn4;

        private List<GameObject> _cardSpines;

        private Dictionary<string, List<string>> _levelDatas;
        private List<string> _recordDatas;
        private int _finishNums;

        private bool _isPlaying ;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
            _dTT = curTrans.GetGameObject("DTT");
            _mask = curTrans.GetGameObject("mask");
            _uiMask = curTrans.GetGameObject("UIMask");
            _replaySpine = curTrans.GetGameObject("replayBtn");
            _startSpine = curTrans.GetGameObject("startBtn");
            _oKSpine = curTrans.GetGameObject("oKBtn");
            _nextLevelSpine = curTrans.GetGameObject("nextLevelBtn");
            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/spSpine");
            _dialogue = curTrans.GetGameObject("dialogue");
            _dd = curTrans.GetGameObject("dd");
            _devil = curTrans.GetRectTransform("dialogue/devil");
            _dingDing = curTrans.GetRectTransform("dialogue/dingDing");
            _tianTian = curTrans.GetRectTransform("dialogue/tianTian");

            _dingDingTxt = curTrans.GetText("dialogue/dingDing/Text");
            _tianTianTxt = curTrans.GetText("dialogue/tianTian/Text");
            _devilTxt = curTrans.GetText("dialogue/devil/Text");

            _cards = curTrans.Find("Cards");
            _levels = curTrans.Find("Levels");
            _level1 = curTrans.Find("Levels/Level1");
            _btn1 = curTrans.Find("Levels/Btns1");
            _level2 = curTrans.Find("Levels/Level2");
            _btn2 = curTrans.Find("Levels/Btns2");
            _level3 = curTrans.Find("Levels/Level3");
            _btn3 = curTrans.Find("Levels/Btns3");
            _level4 = curTrans.Find("Levels/Level4");
            _btn4 = curTrans.Find("Levels/Btns4");


            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            InitDate();
            _isPlaying = false;
            _dingDingTxt.text = string.Empty;
            _devilTxt.text = string.Empty;
            _tianTianTxt.text = string.Empty;

            _mask.Show(); _startSpine.Show(); _dialogue.Show(); _uiMask.Show(); _cards.gameObject.Show();
            _replaySpine.Hide();_oKSpine.Hide();_nextLevelSpine.Hide();_dTT.Hide(); _dd.Hide();

            RemoveEvent(_startSpine); RemoveEvent(_replaySpine); RemoveEvent(_oKSpine); RemoveEvent(_nextLevelSpine);

            SetPos(_devil, _enemyStartPos); SetPos(_dingDing,_roleStartPos); SetPos(_tianTian, _roleStartPos);

            AddBtnsEvent(_cards, OnClickCardEvent);

            for (int i = 0; i < _cards.childCount; i++)           
                _cards.GetChild(i).GetEmpty4Raycast().raycastTarget = true;
            
            foreach (var go in _cardSpines)            
                PlaySpine(go, "kong",()=> { PlaySpine(go, go.name+"4"); });

            for (int i = 0; i < _levels.childCount; i++)
                _levels.GetChild(i).gameObject.Hide();

        
            AddBtnsEvent(_btn1, OnLevelOne); AddBtnsEvent(_btn2, OnLevelTwo);
            AddBtnsEvent(_btn3, OnLevelThree); AddBtnsEvent(_btn4, OnLevelFour);
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllCoroutines();
            StopAllAudio();
            SetRaycastTarget(_btn1);
            SetRaycastTarget(_btn2);
            SetRaycastTarget(_btn3);
            SetRaycastTarget(_btn4);

        }

   

        private void InitDate()
        {
            _succeedSoundIds = new List<int> { 4, 5, 7, 8, 9 }; _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _roleStartPos = new Vector2(-2170,539); _roleEndPos = new Vector2(-960, 539);
            _enemyStartPos = new Vector2(200, 540); _enemyEndPos = new Vector2(-994, 540);
            _dialogues = new List<string> {
                "狗狗什么的最烦了，我要让它们全部消失！哈哈哈~",
                "小朋友们，小恶魔又来了，你们愿意帮助我们一起阻止他吗？",
                "随机选中卡牌，在场景里面找到相对应的狗狗哦" };
            _dingDingTxt.text = ""; _tianTianTxt.text = ""; _devilTxt.text ="";

            _cardSpines = new List<GameObject>();
            for (int i = 0; i < _cards.childCount; i++)
            {           
                var spineGo = _cards.GetChild(i).GetChild(0).gameObject;             
                _cardSpines.Add(spineGo);
            }

            _levelDatas = new Dictionary<string, List<string>>();
            _levelDatas.Add("1", new List<string> { "g-a", "g-c" });
            _levelDatas.Add("2", new List<string> {"g-b","g-d" });
            _levelDatas.Add("3", new List<string> { "g-e"});
            _levelDatas.Add("4", new List<string> {"g-f" });

            _recordDatas = new List<string>();
            _finishNums = 0;
        }


        private void GameStart()
        {
            PlaySpine(_startSpine, "kong", () => { PlaySpine(_startSpine, "bf2", AddOnClickStartBtnEvent); });
        }


        #region 游戏逻辑

        /// <summary>
        /// 添加点击开始按钮事件
        /// </summary>
        private void AddOnClickStartBtnEvent()
        {
            AddEvent(_startSpine, (go) => {
                RemoveEvent(_startSpine);
                PlayOnClickSound();
                PlaySpine(_startSpine, "bf",()=> {
                    _startSpine.Hide();
                    //播放游戏Bgm
                    PlayBgm(0);
                    StartDialogue();
                });
            });
        }

        /// <summary>
        /// 开始情景对话
        /// </summary>
        private void StartDialogue()
        {
            //小恶魔
         
            SetMove(_devil, _enemyEndPos, 1.0f, () => {
                
                ShowDialogue(_dialogues[0], _devilTxt);
                RoleSpeck(0, TianDing);
            });

            //田丁
            void TianDing()
            {
                SetMove(_tianTian, _roleEndPos, 1.0f, () => {
                   
                    ShowDialogue(_dialogues[1], _tianTianTxt);
                    RoleSpeck(1, DingDing);
                });
            }

            //丁丁
            void DingDing()
            {
                _dialogue.Hide();
                _dd.Show();
                BellSpeck(_dd,2,null,()=> {
                    _dd.Hide();
                    StartDialogueOver(); });
               
                //SetMove(_tianTian, _roleStartPos, 1.0f);
                //SetMove(_dingDing, _roleEndPos, 1.0f, () => {

                //    ShowDialogue(_dialogues[2], _dingDingTxt);
                //    RoleSpeck(2, StartDialogueOver);
                //});
            }            
        }

        /// <summary>
        /// 情景对话结束
        /// </summary>
        private void StartDialogueOver()
        {
          
            _mask.Hide();         
            Delay(1, () => { _uiMask.Hide(); });
        }

        /// <summary>
        /// 点击卡牌事件
        /// </summary>
        /// <param name="go"></param>
        private void OnClickCardEvent(GameObject go)
        {
            _uiMask.Show(); PlayVoice(0);
      
            var index =int.Parse(go.name);
            var spineGo = _cardSpines[index];
            PlaySpine(spineGo, spineGo.name + "2",()=> {
                Delay(2.0f, () => { _cards.gameObject.Hide();
                    ShowLevel(index);
                });
            });
        }


        private void SetRaycastTarget(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).GetEmpty4Raycast().raycastTarget = true;            
        }

        private void ShowLevel(int index)
        {
            switch (index)
            {
                case 0:
                    _level1.gameObject.Show();
                    _btn1.gameObject.Show();
                    _level2.gameObject.Hide();
                    _btn2.gameObject.Hide();
                    _level3.gameObject.Hide();
                    _btn3.gameObject.Hide();
                    _level4.gameObject.Hide();
                    _btn4.gameObject.Hide();

                
                  

                    InitSpines(_level1,true,null,true);
                    Delay(1.0f, () => { _uiMask.Hide(); });
                    break;
                case 1:
                    _level1.gameObject.Hide();
                    _btn1.gameObject.Hide();
                    _level2.gameObject.Show();
                    _btn2.gameObject.Show();
                    _level3.gameObject.Hide();
                    _btn3.gameObject.Hide();
                    _level4.gameObject.Hide();
                    _btn4.gameObject.Hide();
                   
                    InitSpines(_level2, true, null, true);
                    Delay(1.0f, () => { _uiMask.Hide(); });
                    break;
                case 2:
                    _level1.gameObject.Hide();
                    _btn1.gameObject.Hide();
                    _level2.gameObject.Hide();
                    _btn2.gameObject.Hide();
                    _level3.gameObject.Show();
                    _btn3.gameObject.Show();
                    _level4.gameObject.Hide();
                    _btn4.gameObject.Hide();
                   
                    InitSpines(_level3, true, null, true);
                    Delay(1.0f, () => { _uiMask.Hide(); });
                    break;
                case 3:
                    _level1.gameObject.Hide();
                    _btn1.gameObject.Hide();
                    _level2.gameObject.Hide();
                    _btn2.gameObject.Hide();
                    _level3.gameObject.Hide();
                    _btn3.gameObject.Hide();
                    _level4.gameObject.Show();
                    _btn4.gameObject.Show();
                    
                    InitSpines(_level4, true, null, true);
                    Delay(1.0f, () => { _uiMask.Hide(); });
                    break;
            }
        }

        /// <summary>
        /// 关卡1
        /// </summary>
        private void OnLevelOne(GameObject go)
        {
            OnLevel(go, "1", _level1, 2, 0, _level1.gameObject, _btn1.gameObject);
        }

        /// <summary>
        /// 关卡2
        /// </summary>
        private void OnLevelTwo(GameObject go)
        {
            OnLevel(go, "2", _level2, 2, 1, _level2.gameObject, _btn2.gameObject);
        }

        /// <summary>
        /// 关卡3
        /// </summary>
        private void OnLevelThree(GameObject go)
        {
            OnLevel(go, "3", _level3, 1, 2, _level3.gameObject, _btn3.gameObject);
        }

        /// <summary>
        /// 关卡4
        /// </summary>
        private void OnLevelFour(GameObject go)
        {
            OnLevel(go, "4", _level4, 1, 3, _level4.gameObject, _btn4.gameObject);
        }

       
    
        private void OnLevel(GameObject go,string key,Transform levelTra,int nums,int cardIndex,GameObject levelGo,GameObject btnGo)
        {

            if (_isPlaying)            
                return;

            _isPlaying = true;

            PlayOnClickSound();
            _uiMask.Show();
           
            var name = go.name;            //当前点击的名字
            var data = _levelDatas[key];  //当前关卡的数据
            bool isContain = data.Contains(name); //是否包含此数据
            var spineGo = levelTra.Find(name).gameObject; //找到当前SpineGo

            if (isContain)   //正确
            {
                go.transform.GetEmpty4Raycast().raycastTarget = false;
                var time =   PlaySuccessSound();

                Delay(time, () => { _isPlaying = false; });
                var isRecord = _recordDatas.Contains(name);
                if (!isRecord)               
                    _recordDatas.Add(name); //添加记录数据
                              
                PlaySpine(spineGo, name + "2", NextStep); //播放发光Spine

                //发光完后的下一步
                void NextStep()
                {
                    _uiMask.Hide();
                    IsFinish();
                  //  PlaySpine(spineGo, name,null, true);
                }

                //是否选完
                void IsFinish()
                {
                    var isFinish = _recordDatas.Count == nums;
                    if (isFinish)
                    {
                        _recordDatas.Clear();
                        _cards.GetChild(cardIndex).GetEmpty4Raycast().raycastTarget = false;
                        _finishNums++;
                        IsSuccess();
                    }
                }

                //游戏是否成功
                void IsSuccess()
                {
                    var isSuccess = _finishNums == 4;
                    if (isSuccess)
                        ShowSuccess();
                    else
                        ShowNextLevelBtn();
                }

                //显示成功
                void ShowSuccess()
                {
                    _uiMask.Show();
                    _cards.gameObject.Show();
                    levelGo.Hide(); btnGo.Hide();
                    var cardSpine = _cards.GetChild(cardIndex).GetChild(0).gameObject;
                    PlaySpine(cardSpine, cardSpine.name + "3");
                    Delay(1.5f, () =>
                    {
                        foreach (var item in _cardSpines)
                            PlaySpine(item, item.name + "2");
                        Delay(3.0f, () => { ShowSuccessSpine(); });                       
                    });
                }
                
                //显示成功Spine
                void ShowSuccessSpine()
                {
                    _mask.Show(); SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3);
                    _successSpine.Show();
                    PlaySpine(_successSpine, "6-12-z",()=> { PlaySpine(_successSpine, "6-12-z2",null,true); });
                    PlaySpine(_spSpine, "kong",()=> { PlaySpine(_spSpine, "sp"); });
                    Delay(4.0f,()=>{
                        _successSpine.Hide();
                        ShowReplayAndOk();                                           
                    });

                }

               

                //显示下一关
                void ShowNextLevelBtn()
                {
                    _mask.Show();
                    _nextLevelSpine.Show();                   
                    PlaySpine(_nextLevelSpine, "kong", () => { PlaySpine(_nextLevelSpine, "next2",AddNextLevelOnClickEvent); });
                }
          
                //添加下一关按钮事件
                void AddNextLevelOnClickEvent()
                {
                    AddEvent(_nextLevelSpine,g=> {
                        PlayOnClickSound();
                        RemoveEvent(g);
                        PlaySpine(g, "next", ShowCard);
                    });
                }

                //显示卡牌选择界面
                void ShowCard()
                {
                    _cards.gameObject.Show();
                    var cardSpine = _cards.GetChild(cardIndex).GetChild(0).gameObject;

                    PlaySpine(cardSpine, cardSpine.name + "3");
                    _nextLevelSpine.Hide(); _mask.Hide();
                    levelGo.Hide();btnGo.Hide();
                }
            }
            else             //错误
            {
                var time= PlayFailSound();
                Delay(time, () => { _isPlaying = false; });
                PlaySpine(spineGo, name + "4", () => { PlaySpine(spineGo, name, null, true); _uiMask.Hide(); });
            }
        }

        /// <summary>
        /// 显示重玩和Ok
        /// </summary>
        private void ShowReplayAndOk()
        {
            _replaySpine.Show(); _oKSpine.Show();
            PlaySpine(_replaySpine, "kong",()=> { PlaySpine(_replaySpine, "fh2"); });
            PlaySpine(_oKSpine, "kong", () => { PlaySpine(_oKSpine, "ok2"); });
            AddEvent(_replaySpine, OnClickReplay);
            AddEvent(_oKSpine, OnClickOk);
        }

        private void OnClickReplay(GameObject go)
        {
            PlayOnClickSound();
            RemoveEvent(_replaySpine);RemoveEvent(_oKSpine);
            PlaySpine(go, "fh",()=> {
                _oKSpine.Hide();
                GameInit();
                _mask.Hide(); PlayBgm(0); _uiMask.Hide();_startSpine.Hide();
            });
        }

        private void OnClickOk(GameObject go)
        {
            PlayOnClickSound();
            RemoveEvent(_replaySpine); RemoveEvent(_oKSpine);
           // PlaySpine(_replaySpine, "fh");
                PlaySpine(go, "ok", () => {
                  _replaySpine.Hide();
                _dTT.Show();
                BellSpeck(_dTT,3);
                    PlayBgm(4, true, SoundManager.SoundType.COMMONBGM);
            });
        }

        #endregion



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
            }
            _talkIndex++;
        }

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
        private float PlayFailSound()
        {

            PlayVoice(2);
            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time= SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        /// <summary>
        /// 播放成功声音
        /// </summary>
        private float PlaySuccessSound()
        {
            PlayVoice(1);
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            var time=  SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        #endregion

        #region 播放Spine

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

 
        private void InitSpines(Transform parent, bool isKong = true, Action callBack = null,bool isLoop=false)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;

                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;

                if (isKong)
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name,null,isLoop); });
                else
                    PlaySpine(child, child.name,null,isLoop);
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
        private void BellSpeck( GameObject go, int index, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(go,type, index, specking, speckend));
        }
     
        private void RoleSpeck(int index,Action callBack=null,bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
           var time= PlaySound(index, isLoop, type);
            Delay(time, callBack);
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

        private void SetPos(RectTransform rect, Vector2 pos)
        {
            rect.anchoredPosition = pos;
        }

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
    }
}
