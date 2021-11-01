using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace ILFramework.HotClass
{
    public class Course722Part2
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _bell;

        private SkeletonGraphic _bellSG;
        private RawImage _xuBg;
        private Transform _curTrans;
        private Transform _onClicks;
        private Transform _onClicks1;
        private Transform _spines;
        private Transform _spines1;
        private Transform _spines2;
        private GameObject _mask;
        private bool _isPlaying;

    


        private RectTransform _bg1Rect;

        private mILDrager _yuanDrag;
        private RectTransform _yuanRect;
        private RectTransform _zhenRect;
        private float _max;
        private RectTransform _4Rect;
        private RectTransform _upRect;
        private RectTransform _downRect;
        private Vector3 _yuanRectStartPos;
        
        private bool _isDraging;
        private Transform _4Tra;
        private bool _isUp;
        private float _angle;
        private float _crossZ;
        private Vector3 _directionHyp;
        private float _upDownMax;
        private Vector2 _upMaxPos;
        private Vector2 _downMaxPos;


        //左右滑动
        private mILDrager _leftRightDrag;
        private Vector2 _leftRightStartPos;
        private float _leftRightMax;
        private RectTransform _leftRightRect;
        private bool _leftRightDraging;
        private bool _isLeftMove;
        private bool _isRightMove;
        private float _leftRightLastPosX;
        private float _leftRightCurPosX;
       
        private Vector2 _leftMaxPos;
        private Vector2 _rightMaxPos;


        void Start(object o)
        {
            _curGo = (GameObject)o;
             _curTrans = _curGo.transform;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            _bell = _curTrans.GetGameObject("bell");              
            _bellSG = _curTrans.Find("bell").GetComponent<SkeletonGraphic>();      
            _xuBg = _curTrans.GetRawImage("BgGroup/3");
            _onClicks = _curTrans.Find("BgGroup/2/OnClick");
            _onClicks1 = _curTrans.Find("BgGroup/2/OnClick1");

            _spines = _curTrans.Find("BgGroup/2/Spines");
            _spines1 = _curTrans.Find("BgGroup/2/Spines1");

            _spines2 = _curTrans.Find("BgGroup/2/Spines2");
            _mask = _curTrans.GetGameObject("mask");
            _4Tra = _curTrans.Find("4");
            _bg1Rect = _curTrans.GetRectTransform("BgGroup/1");

            
            

            _yuanRect = _curTrans.GetRectTransform("4/1/e1Image/Yuan");
            _yuanDrag = _curTrans.Find("4/1/e1Image/Yuan").GetComponent<mILDrager>();
            _zhenRect = _curTrans.GetRectTransform("4/1/e1Image/Zhen");
            _4Rect = _curTrans.GetRectTransform("4");
            _upRect = _curTrans.GetRectTransform("4/1/e1Image/Up");
            _downRect = _curTrans.GetRectTransform("4/1/e1Image/Down");

            _leftRightDrag = _curTrans.Find("4/0/Point").GetComponent<mILDrager>();
            _leftRightRect = _curTrans.GetRectTransform("4/0/Point");

            GameInit();
            GameStart();


         
        }

   

        void GameInit()
        {
            _talkIndex = 1;
            HideVoiceBtn();          
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllCoroutines();
            StopAllAudio();

            _isPlaying = false;
            RemoveEvent(_mask);RemoveEvent(_onClicks.gameObject);
            _mask.Show(); _bell.Show();  _xuBg.gameObject.Show();
            _bellSG.color = new Color(1, 1, 1, 1);
            _xuBg.color = new Color(1, 1, 1, 1);
            InitSpines(_spines, false); InitSpines(_spines1, false);
            AddEvents(_onClicks, OnClicks);
            AddEvents(_onClicks1, OnClicks1);
            for (int i = 0; i < _spines2.childCount; i++)
            {
                var child = _spines2.GetChild(i).gameObject;
                PlaySpine(child, child.name[0]+"3");
            }
            
            HideAllChilds(_4Tra);

             _yuanDrag.DragRect = _4Rect;
            _yuanRectStartPos = new Vector2(-220, 0);
            _yuanRect.localPosition = _yuanRectStartPos;
             _max = Vector2.Distance(_yuanRect.localPosition, _zhenRect.localPosition);
            _yuanDrag.SetDragCallback(StartDrag, Draging, DragEnd);

            _zhenRect.rotation = Quaternion.Euler(0, 0, 0);
            _isDraging = false;

            _upDownMax = (_bg1Rect.rect.height / 2) - (_curTrans.GetRectTransform().rect.height / 2) ;

            //左右滑动
            _leftRightMax = ((_bg1Rect.rect.width / 2) - (_curTrans.GetRectTransform().rect.width / 2))-200f;
            Debug.LogError("_leftRightMax:"+ _leftRightMax);
            _leftRightDraging = false;
            _leftRightDrag.SetDragCallback(LeftRightStartDrag, LeftRightStartDraging, LeftRightDragEnd);
            _leftRightStartPos = new Vector2(277f, -213f);

            _isLeftMove = false;
            _isRightMove = false;

        }

       

        void GameStart()
        {
            PlayBgm(0);
            BellSpeck(_bell, 0, null, () => {

                _bell.Hide();
                _xuBg.DOColor(new Color(1, 1, 1, 0), 1f).OnComplete(() => { _mask.Hide(); _xuBg.gameObject.Hide(); });
            });
            StartUpdate();
        }

        void TalkClick()
        {
            PlayOnClickSound();
            HideVoiceBtn();
        
            if(_talkIndex == 1)
            {

            }
            _talkIndex++;
        }

        #region 游戏逻辑


        private void StartUpdate()
        {
            UpDate(true, 0.02f, () => {

                UpDownMove();
                LeftRightMove();
            });
        }



        private void LeftRightStartDrag(Vector3 pos, int arg2, int arg3)
        {           
            _leftRightLastPosX = pos.x;
            _leftMaxPos = new Vector2(-_leftRightMax, _bg1Rect.localPosition.y);
            _rightMaxPos = new Vector2(_leftRightMax, _bg1Rect.localPosition.y);
          
        }
       

        private void LeftRightStartDraging(Vector3 pos, int arg2, int arg3)
        {
            
            _leftRightDraging = true;
            _leftRightCurPosX = pos.x;

            
        }

        private void LeftRightDragEnd(Vector3 pos, int arg2, int arg3, bool arg4)
        {
            _leftRightDraging = false;
            _isLeftMove = false;
            _isRightMove = false;

        }


        private void LeftRightMove()
        {
            if (_leftRightDraging)
            {            
                if (_leftRightCurPosX!= _leftRightLastPosX)
                {
                

                    if (_leftRightCurPosX< _leftRightLastPosX)
                    {
                        _isLeftMove = true;
                        _isRightMove = false;
                    }
                    else if(_leftRightCurPosX> _leftRightLastPosX)
                    {
                        _isLeftMove = false;
                        _isRightMove = true;
                    }

                    var speed = _leftRightStartPos.x - _leftRightCurPosX;
                    if (speed < 0) { speed = -speed; }
                    
                    if (_isLeftMove)
                    {
                        if (_bg1Rect.localPosition.x >-_leftRightMax)
                            _bg1Rect.Translate(Vector3.left * speed);
                        else
                            _bg1Rect.localPosition = _leftMaxPos;
                    }

                    if (_isRightMove)
                    {
                        if (_bg1Rect.localPosition.x <_leftRightMax)
                            _bg1Rect.Translate(Vector3.right * speed);
                        else
                            _bg1Rect.localPosition = _rightMaxPos;
                    }



                    _leftRightLastPosX = _leftRightCurPosX;

                }
                else
                {
                    
                    _isLeftMove = false;
                    _isRightMove = false;
                }

            }
        }

        private void StartDrag(Vector3 pos, int arg2, int arg3)
        {
           
            _upMaxPos = new Vector2(_bg1Rect.localPosition.x, -_upDownMax);
            _downMaxPos = new Vector2(_bg1Rect.localPosition.x, _upDownMax);
        }
        private void Draging(Vector3 pos, int arg2, int arg3)
        {
            _isDraging = true;

            var dic = Vector2.Distance(_yuanRect.localPosition, _zhenRect.localPosition);
            
            if (dic < _max || dic> _max)
            {
                _directionHyp = _yuanRect.localPosition - Vector3.zero;           
                _yuanRect.localPosition = _directionHyp.normalized * _max;   //设置位置
            }
          
            Vector2 nor1 = _yuanRect.anchoredPosition.normalized;
            Vector2 nor2 = _yuanRectStartPos.normalized;

            var angle = Vector2.Angle(nor2, nor1);

            Vector3 cross = Vector3.Cross(nor2, nor1);
            _zhenRect.rotation = Quaternion.Euler(0, 0, cross.z > 0 ? angle : -angle);
            bool isGtZero = cross.z > 0;


            if (_yuanDrag.DragRect==_4Rect)
            {
                if (isGtZero)
                { _yuanDrag.DragRect = _downRect; _isUp = false; }
                else
                { _yuanDrag.DragRect = _upRect; _isUp = true; }            
            }
            else if(_yuanDrag.DragRect ==_downRect && cross.z == 0 && angle == 0)
            {
                 _yuanDrag.DragRect = _upRect; _isUp = true;
            }
            else if(_yuanDrag.DragRect == _upRect && cross.z == 0 && angle == 0)
            {
                _yuanDrag.DragRect = _downRect; _isUp = false;
            }
            _angle = angle; _crossZ = cross.z;
         
        }


        
        private void DragEnd(Vector3 pos, int arg2, int arg3, bool arg4)
        {
            _isDraging = false;
        }

      

        private void UpDownMove()
        {
            if (_isDraging)   //拖拽中
            {
                if (_isUp)
                {
               
                    if (_bg1Rect.localPosition.y > -_upDownMax)
                        _bg1Rect.Translate(Vector3.down * _angle*0.1f);
                    else
                        _bg1Rect.localPosition = _upMaxPos;
                }
                else
                {                  
                    if (_bg1Rect.anchoredPosition.y < _upDownMax)
                        _bg1Rect.Translate(Vector3.up * _angle*0.1f);
                    else
                        _bg1Rect.localPosition = _downMaxPos;
                }
            }
            else              //拖拽结束
            {
              
                //角度回归到0度，同时升降速度随着角度减少而减少
                if (_yuanRect.localPosition!= _yuanRectStartPos )
                {
                   
                    if (_angle==180)
                    {                      
                        if (_isUp)                        
                            _yuanRect.Translate(Vector3.up * _angle * 0.1f);                        
                        else                       
                            _yuanRect.Translate(Vector3.down * _angle * 0.1f);                       
                    }
                    else
                    {
                        if (_angle<=20)
                        {                          
                            _yuanRect.Translate(Vector3.left * 50 * 0.1f);
                        }
                        else
                        {
                            _yuanRect.Translate(Vector3.left * _angle * 0.1f);
                        }                                             
                    }
                   


                    var dic = Vector2.Distance(_yuanRect.localPosition, _zhenRect.localPosition);

                    if (dic < _max || dic > _max)
                    {                     
                        _directionHyp = _yuanRect.localPosition - Vector3.zero;
                        _yuanRect.localPosition = _directionHyp.normalized * _max;   //设置位置
                    }

                    Vector2 nor1 = _yuanRect.anchoredPosition.normalized;
                    Vector2 nor2 = _yuanRectStartPos.normalized;

                    var angle = Vector2.Angle(nor2, nor1);
                    _angle = angle;
                 
                    Vector3 cross = Vector3.Cross(nor2, nor1);
                    _zhenRect.rotation = Quaternion.Euler(0, 0, cross.z > 0 ? angle : -angle);
                    _crossZ = cross.z;
                    if (_isUp)
                    {                     
                        if (_bg1Rect.localPosition.y > -_upDownMax)
                            _bg1Rect.Translate(Vector3.down * _angle * 0.1f);
                        else
                            _bg1Rect.localPosition = _upMaxPos;
                    }
                    else
                    {                      
                        if (_bg1Rect.anchoredPosition.y < _upDownMax)
                            _bg1Rect.Translate(Vector3.up * _angle * 0.1f);
                        else
                            _bg1Rect.localPosition = _downMaxPos;
                    }

                   
                }
            }
        }



        private void OnClicks1(GameObject go)
        {
            if (_isPlaying)
                return;
            _isPlaying = true;

            PlayCommonSound(6);

            var name = go.name;
            var guangGo = FindSpineGo(_spines1, name);
            OnClick2(1, guangGo);
        }

        private void OnClicks(GameObject go)
        {
            if (_isPlaying)           
                return;
            _isPlaying = true;

            PlayCommonSound(6);

            var name = go.name;
            var guangGo = FindSpineGo(_spines, name);
            GameObject fangDaGo = null;
            switch (name)
            {
                case "1":  //空速表
                    Delay(0.5f, () => { PlayCommonSound(1); });
                    fangDaGo = FindSpineGo(_spines2, "c1");
                    OnClick1(4, guangGo, fangDaGo, "c3");                   
                    break;
                case "2":  //姿态指示器
                    Delay(0.5f, () => { PlayCommonSound(1); });
                    fangDaGo = FindSpineGo(_spines2, "f1");
                    OnClick1(5, guangGo, fangDaGo, "f3");                 
                    break;
                case "3":  //气压高度表
                    Delay(0.5f, () => { PlayCommonSound(1); });
                    fangDaGo = FindSpineGo(_spines2, "d1");
                    OnClick1(6, guangGo, fangDaGo, "d3");
                    break;
                case "4": //侧滑仪
                    Delay(0.5f, () => { PlayCommonSound(1); });
                    _leftRightRect.localPosition = _leftRightStartPos;  //重置拖拽的初始位置
                    fangDaGo = FindSpineGo(_spines2, "a1");                
                    OnClick3(7, guangGo, fangDaGo, "a2", "a3",0);
                    break;
                case "5":  //航向指示器
                    Delay(0.5f, () => { PlayCommonSound(1); });
                    fangDaGo = FindSpineGo(_spines2, "b1");
                    OnClick1(8, guangGo, fangDaGo, "b3");
                    break;
                case "6":   //升降速度表
                    Delay(0.5f, () => { PlayCommonSound(1); });
                    _yuanRect.localPosition = _yuanRectStartPos; _yuanDrag.DragRect = _4Rect;  //重置位置   //重置拖拽范围
                    _zhenRect.rotation = Quaternion.Euler(0, 0, 0);       //重置针的角度
                    fangDaGo = FindSpineGo(_spines2, "e1");                  
                    OnClick3(9, guangGo, fangDaGo, "e2", "e3", 1,()=> { _yuanRect.localPosition = _yuanRectStartPos; _yuanDrag.DragRect = _4Rect; _zhenRect.rotation = Quaternion.Euler(0, 0, 0); });
                    break;
                case "7":   //驾驶杆
                    OnClick2(2, guangGo);
                    break;
                case "8":     //无线电接收器
                    OnClick2(3, guangGo);
                    break;             
            }

        }


        private void OnClick1(int index,GameObject guangGo,GameObject fangDaGo,string smallName)
        {           
            PlaySpine(guangGo, guangGo.name, () => { PlaySpine(fangDaGo, fangDaGo.name); });
            BellSpeck(_bell, index, null, () => { _mask.Show();
                AddEvent(_mask, m => { RemoveEvent(_mask);
                    PlayCommonSound(2);
                    PlaySpine(fangDaGo, smallName, () => { _mask.Hide(); _isPlaying = false; });
                });
            });
        }

        private void OnClick2(int index, GameObject guangGo)
        {
            PlaySpine(guangGo, guangGo.name);
            BellSpeck(_bell, index, null, () => { _isPlaying = false; });
        }

        private void OnClick3(int index,GameObject guangGo, GameObject fangDaGo,string fangDaJingName, string smallName,int showIndex,Action callBack=null)
        {
            HideAllChilds(_4Tra);
            PlaySpine(guangGo, guangGo.name, () => { PlaySpine(fangDaGo, fangDaGo.name); });
            BellSpeck(_bell, index, null, () => {
                PlaySpine(fangDaGo, fangDaJingName);
                ShowChilds(_4Tra,showIndex);
                _mask.Show();
                AddEvent(_mask, m => {
                    PlayCommonSound(2);
                    RemoveEvent(_mask); callBack?.Invoke();
                    HideChilds(_4Tra, showIndex);
                 PlaySpine(fangDaGo, smallName, () => { _mask.Hide(); _isPlaying = false; });
                });
            });
        }
     
        #endregion

        #region 常用函数

        #region 语音键显示隐藏
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

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
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

        private GameObject FindSpineGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        private void PlaySequenceSpine(GameObject go, List<string> spineNames)
        {
            _mono.StartCoroutine(IEPlaySequenceSpine(go, spineNames));
        }

        #endregion

        #region 音频相关

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

        IEnumerator IEPlaySequenceSpine(GameObject go, List<string> spineNames)
        {
            for (int i = 0; i < spineNames.Count; i++)
            {
                var name = spineNames[i];
                var delay = PlaySpine(go, name);
                yield return new WaitForSeconds(delay);
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, bool isBell = true, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, isBell));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, bool isBell = true, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            if (isBell)
            {
                daiJi = "DAIJI"; speak = "DAIJIshuohua";
            }
            else
            {
                Debug.LogError("Role Spine Name...");
                daiJi = "daiji"; speak = "speak";
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

        #endregion
    }
}
