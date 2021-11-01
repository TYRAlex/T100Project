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
  
    public class TD8912Part5
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
              
        private GameObject _successSpine;
        private GameObject _spSpine;
        private GameObject _mask;
        private GameObject _bD;
        private GameObject _bigBD;
        private GameObject _startSpine;
        private GameObject _restartSpine;
        private GameObject _okSpine;
        private GameObject _uiMask;
        private GameObject _yunSpine;
       
        private GameObject _tSpine;

        private Transform _enemy1Tra;
        private Transform _levels;
        private Transform _enemyIconsTra;
        private Transform _socresTra;
       

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private int _curDragIndex;
        private int _curLevelId;
        private List<mILDrager> _mILDragers;

        private bool _isOnClickReplay;

        private List<Vector2> _initPos;
        private List<int> _posTempRange;
        private Dictionary<int, Vector2> _posDics;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/spSpine");
            _mask = curTrans.GetGameObject("mask");
            _bD = curTrans.GetGameObject("BD");
            _bigBD = curTrans.GetGameObject("DBD");
            _startSpine = curTrans.GetGameObject("startSpine");
            _restartSpine = curTrans.GetGameObject("restartSpine");
            _okSpine = curTrans.GetGameObject("okSpine");
            _uiMask = curTrans.GetGameObject("uiMask");
            _yunSpine = curTrans.GetGameObject("Bg/yunSpine");
        
            _tSpine = curTrans.GetGameObject("Spines/Spine1/t");

            _enemy1Tra = curTrans.Find("Spines/enemy1");
            _levels = curTrans.Find("Spines/Levels");
            _enemyIconsTra = curTrans.Find("ScoreParent/EnemyIcons");
            _socresTra = curTrans.Find("ScoreParent/Scores");

            _isOnClickReplay = false;

            _mask.Show(); _startSpine.Show();
            _successSpine.Hide(); _bD.Hide(); _bigBD.Hide(); _restartSpine.Hide(); _okSpine.Hide();

            PlaySpine(_startSpine, "kong", () => { PlaySpine(_startSpine, "bf2"); });

            GameInit();
            
        }
  

        private void GameInit()
        {
            InitData();

            _talkIndex = 1; 
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

                     
            InitBtnEvent(); 
            PlayYunSpine();
            ShowEnemyIcon(0);
            InitEnemySpineState();
            ShowCurScoreIcno(_curLevelId);
            ShowCurLevel(_curLevelId);
                   
        }

        void InitData()
        {
            _curLevelId = 1; _curDragIndex = -1;
            _succeedSoundIds = new List<int> { 4, 5, 7, 8,9 }; _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _posDics = new Dictionary<int, Vector2>();
            _posDics.Add(1, new Vector2(-474f,-415));
            _posDics.Add(2, new Vector2(-239f, -415));
            _posDics.Add(3, new Vector2(-4f, -415));
            _posDics.Add(4, new Vector2(231f, -415));
            _posDics.Add(5, new Vector2(466f, -415));
           
        }

      

        #region 游戏逻辑

        /// <summary>
        /// 播放云的Spine
        /// </summary>
        private void PlayYunSpine()
        {
            PlaySpine(_yunSpine,"yun",null,true);
        }

        /// <summary>
        /// 显示敌人Icon
        /// </summary>
        /// <param name="index">传0和1,0正常，1投降</param>
        private void ShowEnemyIcon(int index)
        {          
            _enemyIconsTra.GetChild(index).gameObject.Show();
            switch (index)
            {
                case 0:
                    _enemyIconsTra.GetChild(1).gameObject.Hide();
                    break;
                case 1:
                    _enemyIconsTra.GetChild(0).gameObject.Hide();
                    break;
            }
        }

        /// <summary>
        /// 隐藏All分数Icon
        /// </summary>
        private void HideScoreIcons()
        {
            for (int i = 0; i < _socresTra.childCount; i++)            
                 _socresTra.GetChild(i).gameObject.Hide();            
        }

        /// <summary>
        /// 显示当前分数Icon
        /// </summary>
        /// <param name="curLevelId">当前关卡Id</param>
        private void ShowCurScoreIcno(int curLevelId)
        {
            HideScoreIcons();
            for (int i = 0; i < _socresTra.childCount; i++)
            {
                var go = _socresTra.GetChild(i).gameObject;
                var name = go.name;
                var isCurSocre = name == curLevelId.ToString();
                if (isCurSocre)
                    go.Show();
                else
                    go.Hide();
            }

        }

        /// <summary>
        /// 隐藏AllLevel
        /// </summary>
        private void HideLevels()
        {
            for (int i = 0; i < _levels.childCount; i++)           
                 _levels.GetChild(i).gameObject.Hide();                       
        }

        /// <summary>
        /// 显示当前关卡
        /// </summary>
        /// <param name="curLevelId"></param>
        private void ShowCurLevel(int curLevelId)
        {       
            if (curLevelId>=6)
            {
              
                GameOver();                               
                return;
            }

           
            ShowLevelSpine(curLevelId);
            HideLevels();
            var name = "Level" + curLevelId;
            var curLevel = _levels.Find(name);
            curLevel.gameObject.Show();
            _mILDragers = new List<mILDrager>();
            _posTempRange = new List<int>();
            _initPos = new List<Vector2>();
            while (true)
            {

                var key = Random.Range(1, 6);
                var isExist = _posTempRange.Contains(key);

                if (!isExist)
                    _posTempRange.Add(key);

                if (_posTempRange.Count == 5)
                    break;
            }


          

            for (int i = 0; i < curLevel.childCount; i++)
            {
                var child = curLevel.GetChild(i);
                var isMILDrager = child.GetComponent<mILDrager>()!=null;
                var isMIDroper = child.GetComponent<mILDroper>()!=null;

                if (isMILDrager)
                {
                   var mILDrager= child.GetComponent<mILDrager>();
                    _mILDragers.Add(mILDrager);                   
                    mILDrager.gameObject.transform.localPosition = _posDics[_posTempRange[i-1]];
                    _initPos.Add(mILDrager.gameObject.transform.localPosition);
                    mILDrager.gameObject.Show();
                    mILDrager.SetDragCallback(DragStart, null, DragEnd);
                }

                if (isMIDroper)
                {
                    var mIDroper = child.GetComponent<mILDroper>();
                    mIDroper.SetDropCallBack(null, null, DragFail);
                }
            }

           
        }

        /// <summary>
        /// 拖拽失败
        /// </summary>
        /// <param name="type"></param>
        private void DragFail(int type)
        {
            ShowEnemyTauntSpine();
            PlayFailSound(); PlaySound(2);
            DoResetCurDargPos();
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <param name="index"></param>
        private void DragStart(Vector3 pos, int type, int index)
        {
            _curDragIndex = index;        
        }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="isMatch"></param>
        private void DragEnd(Vector3 pos,int type,int index,bool isMatch)
        {
            if (isMatch)
            {
                DoResetCurDargPos(true);
            

                _uiMask.Show();
                PlaySuccessSound();
                PlaySpine(_tSpine, "t" + _curLevelId);
                PlaySound(3);
                Delay(1.3f, () => {              
                    ShowEnemyAssaultedSpine(curenemy => { Delay(1.0f, () => { curenemy.Hide(); }); });
                });
                          
                Delay(2.9f, () => {
                    _curLevelId++;
                 
                    ShowCurScoreIcno(_curLevelId);
                    ShowCurLevel(_curLevelId);
                });
                            
             

            }
            else
            {
                DoResetCurDargPos();
            }
        }

      

        /// <summary>
        /// 重置当前拖拽的位置
        /// </summary>
        private void DoResetCurDargPos(bool isHide=false)
        {
            for (int i = 0; i < _mILDragers.Count; i++)
            {
                var isFind = _mILDragers[i].index == _curDragIndex;
                if (isFind)
                {
                    if (isHide)
                        _mILDragers[i].gameObject.Hide();

                    _mILDragers[i].transform.localPosition = _initPos[i] ;
                }
            }
   
        }

        /// <summary>
        /// 初始化敌人Spine状态
        /// </summary>
        private void InitEnemySpineState()
        {       
            for (int i = 0; i < _enemy1Tra.childCount; i++)
            {
                var enemyGo = _enemy1Tra.GetChild(i).gameObject;
                enemyGo.Show();
                PlaySpine(enemyGo, "kong", () => { PlaySpine(enemyGo, "xem", null, true); });            
            }
        }

        /// <summary>
        /// 显示敌人嘲讽Spine
        /// </summary>
        private void ShowEnemyTauntSpine()
        {
            
            GameObject enemyGo = RandomEnemyGo();
            PlaySpine(enemyGo, "xem2", () =>
            {
                PlaySpine(enemyGo, "xem", null, true);
            });       
        }

        /// <summary>
        /// 显示敌人被攻击Spine
        /// </summary>
        private void ShowEnemyAssaultedSpine(Action<GameObject> callBack=null)
        {       
            GameObject enemyGo = RandomEnemyGo();       
            PlaySpine(enemyGo, "xem3", () => { callBack?.Invoke(enemyGo); });         
        }



        /// <summary>
        /// 随机一个敌人
        /// </summary>
        /// <returns></returns>
        private GameObject RandomEnemyGo()
        {
            var rangeIndex = -1;
            GameObject enemyGo = null;
            while (true)
            {
                rangeIndex = Random.Range(0, 5);            
                enemyGo = _enemy1Tra.GetChild(rangeIndex).gameObject;
                var isActiveSelf = enemyGo.activeSelf == true;
                if (isActiveSelf)
                    break;
            }
            
            return enemyGo;
        }


        /// <summary>
        /// 显示对应关卡Spine
        /// </summary>
        /// <param name="curLevelId"></param>
        private void ShowLevelSpine(int curLevelId)
        {
            var faGuang = "td" + curLevelId;
            var jingZhi = "tx" + curLevelId;

            PlaySpine(_tSpine, "kong", PlayFaGuang);

            void PlayFaGuang()
            {
                PlaySpine(_tSpine, faGuang, PlayJingZhi);
            }

            void PlayJingZhi()
            {
                PlaySpine(_tSpine, jingZhi,()=> { _uiMask.Hide(); });
            }                         
        }

   
        /// <summary>
        /// 初始化开始，重玩，Ok按钮事件
        /// </summary>
        private void InitBtnEvent()
        {
            RemoveEvent(_restartSpine); RemoveEvent(_startSpine); RemoveEvent(_okSpine);
            AddEvent(_restartSpine, OnRestartBtn);
            AddEvent(_startSpine, OnStartBtn);
            AddEvent(_okSpine, OnOkBtn);
        }
     
        /// <summary>
        /// 点击开始Btn
        /// </summary>
        /// <param name="go"></param>
        private void OnStartBtn(GameObject go)
        {           
            PlayOnClickSound();
            RemoveEvent(go); 
            PlaySpine(go, "bf",()=> { go.Hide();
                StarPlayGame();
            });
        }

        /// <summary>
        /// 点击重玩Btn
        /// </summary>
        /// <param name="go"></param>
        private void OnRestartBtn(GameObject go)
        {       
            PlayOnClickSound();
            RemoveEvent(_restartSpine); RemoveEvent(_okSpine);
         //   PlaySpine(_okSpine, "ok", () => { _okSpine.Hide(); });
            PlaySpine(_restartSpine, "fh", () => { _okSpine.Hide(); _restartSpine.Hide(); GameInit(); _mask.Hide();  PlayBgm(0); });                 
        }

        /// <summary>
        /// 点击OkBtn
        /// </summary>
        /// <param name="go"></param>
        private void OnOkBtn(GameObject go)
        {
            PlayOnClickSound();
            RemoveEvent(_okSpine); RemoveEvent(_restartSpine);
          //  PlaySpine(_restartSpine, "fh", () => { _restartSpine.Hide(); });
            PlaySpine(_okSpine, "ok",()=> {
                _restartSpine.Hide();
                _okSpine.Hide(); _bigBD.Show();
                BellSpeck(4, _bigBD);
                PlayBgm(4, true, SoundManager.SoundType.COMMONBGM);
            });
                         
        }

        /// <summary>
        /// 显示胜利界面
        /// </summary>
        private void PlaySuccessSpine()
        {
            _mask.Show();
            _successSpine.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3);
            PlaySpine(_spSpine,"kong",()=> { PlaySpine(_spSpine, "sp"); });
            PlaySpine(_successSpine, "6-12-z",()=> { PlaySpine(_successSpine, "6-12-z2",null,true); });
        }


        /// <summary>
        /// 开始玩游戏
        /// </summary>
        private void StarPlayGame()
        {
            PlayBgm(0);

            _bD.Show();
            BellSpeck(0, _bD, null, () => {
                SoundManager.instance.ShowVoiceBtn(true);
            });         
        }



        private void GameOver()
        {
            ShowEnemyIcon(1);
            PlaySuccessSpine();
          
            Delay(4.0f, () => {
                _successSpine.Hide();
                _okSpine.SetActive(true);
                _restartSpine.Show();                             
              
                PlaySpine(_restartSpine, "kong",()=> { PlaySpine(_restartSpine, "fh2",()=> { _uiMask.Hide(); }); });
                PlaySpine(_okSpine, "kong", () => { PlaySpine(_okSpine, "ok2"); });
               
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
        IEnumerator SpeckerCoroutine(GameObject go,SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            PlayOnClickSound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (_talkIndex == 1)
            {
                BellSpeck(1, _bD, null, () => {
                    _mask.Hide(); _uiMask.Hide(); _bD.Hide();
                });
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
        private void PlayFailSound()
        {
            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
        }

        /// <summary>
        /// 播放成功声音
        /// </summary>
        private void PlaySuccessSound()
        {
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
        }

        #endregion

        #region 播放Spine

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }


        private void InitSpines(Transform parent, bool isKong = true, Action callBack = null, bool isLoop = false)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;

                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;

                if (isKong)
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name, null, isLoop); });
                else
                    PlaySpine(child, child.name, null, isLoop);
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
        private void BellSpeck(int index,GameObject go, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(go,type, index, specking, speckend));
        }

        private void RoleSpeck(int index, Action callBack = null, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            var time = PlaySound(index, isLoop, type);
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
