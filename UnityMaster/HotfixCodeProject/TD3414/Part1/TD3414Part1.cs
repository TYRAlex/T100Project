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
  
    public class TD3414Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _bd;
        private GameObject _bD;
        private GameObject _tT;
        private GameObject _mask;
        private Transform _dragRoundTra;
        private Transform _contentTra;
        private Transform _dragEndPos;
        private Dictionary<string, Vector3> _dragPos;
        private GameObject _itemsMask;
        private GameObject _xing;

        private List<int> _successSoundId;
        private List<int> _failSoundIds;

        private int _isFinish;
        private RectTransform _contentRect;
    
        private GameObject _successSpine;
        private GameObject _spSpine;

        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            Transform curTrans = _curGo.transform;
            _bd = curTrans.GetGameObject("bd");
            _bD = curTrans.GetGameObject("BD");
            _tT = curTrans.GetGameObject("TT");
            _mask = curTrans.GetGameObject("mask");
            _dragRoundTra = curTrans.Find("DragRound");
            _contentTra = curTrans.Find("gojul/Items/Content");
            _dragEndPos = curTrans.Find("DragEndPos");
            _itemsMask = curTrans.GetGameObject("gojul/ItemsMask");
             _dragPos = new Dictionary<string, Vector3>();
            _contentRect = curTrans.GetRectTransform("gojul/Items/Content");
            _xing = curTrans.GetGameObject("xing");
            _successSpine = curTrans.GetGameObject("SuccessSpine");
            _spSpine = curTrans.GetGameObject("SuccessSpine/SpSpine");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");
       
            GameInit();

            GameStart();
        }

      
        private void GameInit()
        {
            _successSpine.Hide();
            _xing.Hide();
            _successSoundId =new List<int> { 4, 5, 7, 8, 9 }; _failSoundIds = new List<int> { 0, 1, 2, 3 };
            StopAllCoroutines();
            StopAllAudio();        
            _talkIndex = 1;
            _itemsMask.Hide();
            _isFinish = 0;
            SetPos(_contentRect, new Vector2(0,-85f));
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            RemoveEvent(_replaySpine); RemoveEvent(_startSpine); RemoveEvent(_okSpine);
            AddEvent(_replaySpine, OnClickReplay);
            AddEvent(_startSpine, OnClickStart);
            AddEvent(_okSpine, OnClickOk);
            _bD.Hide(); _bd.Hide(); _okSpine.Hide(); _replaySpine.Hide(); _tT.Hide(); _startSpine.Hide(); _mask.Hide();

            for (int i = 0; i < _dragEndPos.childCount; i++)
            {
            
                if (_dragEndPos.GetChild(i).name!= "GameObject")
                {
                    _dragEndPos.GetChild(i).GetImage().enabled = false;
                }
               
            }
     
            AddBtnsEvent();
          
        }

        void GameStart()
        {
            _startSpine.Show(); _mask.Show();
            PlaySpine(_startSpine, "bf2");
        }

        /// <summary>
        /// 点击重玩
        /// </summary>
        /// <param name="go"></param>
        private void OnClickReplay(GameObject go)
        {
            PlayOnClickSound();
            RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
            PlaySpine(_replaySpine, "fh",
                ()=> {
                    _okSpine.Hide(); _replaySpine.Hide();
                GameInit(); PlayBgm(0);
            });
         
        }

        /// <summary>
        /// 点击Ok
        /// </summary>
        /// <param name="go"></param>
        private void OnClickOk(GameObject go)
        {
            PlayOnClickSound();
            RemoveEvent(_replaySpine);RemoveEvent(_okSpine);
      
            var time =  PlaySpine(_okSpine, "ok");
           
            Delay(time, () => {
                _replaySpine.Hide(); _okSpine.Hide();
                PlayBgm(4, true, SoundManager.SoundType.COMMONBGM);
                _bD.Show(); BellSpeck(2, _bD);
            });
            
        }
        private void OnClickStart(GameObject go)
        {
            PlayOnClickSound(); PlayBgm(0);
            RemoveEvent(_startSpine);
            PlaySpine(_startSpine, "bf",()=> {
                _startSpine.Hide();
                _tT.Show();
                BellSpeck(0, _tT, () => { _mask.Show(); }, () => {
                  
                    SoundManager.instance.ShowVoiceBtn(true);
                });
            });
        }


        private void AddBtnsEvent()
        {
            for (int i = 0; i < _contentTra.childCount; i++)
            {
                var child = _contentTra.GetChild(i).GetChild(0).gameObject;
                child.Show();
                var item = _dragRoundTra.Find(child.name).gameObject;
                var mILDrager = item.GetComponent<mILDrager>();
             
                mILDrager.SetDragCallback(null, null, EndDrag, null);
                item.Hide();
                RemoveEvent(child);
                AddEvent(child, OnClickItems);
            }
        }


    
        private void EndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            _itemsMask.Hide();
             var tra = GetCurDragTra(type);
            var name = tra.name;
           
            if (!isMatch)
            {
                PlayFailSound();
                var endPos = _dragPos[name];
                tra.DOScale(1, 0.1f);
                tra.DOMove(endPos, 0.1f).OnComplete(()=> {
                    tra.gameObject.Hide();
                    _contentTra.Find(name.Split('-')[0]).GetChild(0).gameObject.Show(); ;
                }); ;
            }
            else
            {
                PlaySuccessSound();//PlayVoice(0);
                tra.gameObject.Hide();
                _dragEndPos.Find(tra.name).GetImage().enabled = true;
                _isFinish++;
                if (_isFinish==7)
                {
                    Delay(2, () => {

                        _xing.Show(); PlayVoice(3);
                        PlaySpine(_xing, "xing", () => {

                            PlaySpine(_xing, "xing-c", () => {
                                _xing.Hide();
                                Delay(1.0f, () => {

                                    _mask.Show(); PlaySound(3, false, SoundManager.SoundType.COMMONSOUND);
                                    _successSpine.Show();
                                    PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, "sp"); });
                                    PlaySpine(_successSpine, "3-5-z", () => { PlaySpine(_successSpine, "3-5-z2", null, true); });
                                    Delay(4.0f, () => {
                                        _successSpine.Hide();
                                        _okSpine.Show(); _replaySpine.Show();
                                        PlaySpine(_okSpine, "ok2");
                                        PlaySpine(_replaySpine, "fh2");
                                    });
                                });
                            });
                        });


                    });

                  
                }
            }

        }

        private Transform GetCurDragTra(int type)
        {
            Transform tra = null;
            for (int i = 0; i < _dragRoundTra.childCount; i++)
            {
                var child = _dragRoundTra.GetChild(i);
                var drag = child.GetComponent<mILDrager>();
                if (drag.dragType== type)
                {
                    tra = child;
                    break;
                }
            }
            return tra;
        }

        private void OnClickItems(GameObject go)
        {
            PlayOnClickSound(); go.Hide(); _itemsMask.Show();
             var name = go.name;                 
            var item = _dragRoundTra.Find(name).gameObject;
            item.transform.position = go.transform.position;
            item.Show();
            item.transform.SetAsLastSibling();

            var rect = item.transform.GetRectTransform();
            item.transform.position = new Vector2(item.transform.position.x, item.transform.position.y + (60*Screen.height/1080f));
          //  SetScale(rect, new Vector3(1.5f, 1.5f, 0));
            bool isKey = _dragPos.ContainsKey(item.name);
            if (isKey)
                _dragPos[name] = go.transform.position;
            else
                _dragPos.Add(name, go.transform.position);


        }


        //bell说话协程
        IEnumerator SpeckerCoroutine(GameObject go, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {       
            var str1 = String.Empty;
            var str2 = String.Empty;

            switch (go.name)
            {

                case "BD":
                case "bd":
                    str1 = "bd-daiji"; str2 = "bd-speak";
                    break;
                case "TT":
                    str1 = "animation"; str2 = "animation2";
                    break;
            }

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, str1);
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, str2);

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, str1);
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            PlayOnClickSound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (_talkIndex)
            {
                case 1:
                    _tT.Hide(); _bd.Show();
                    BellSpeck(1, _bd, null, () => { _mask.Hide(); _bd.Hide(); });
                    break;
            }
            _talkIndex++;
        }


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
            PlayVoice(2);
            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
        }

        /// <summary>
        /// 播放成功声音
        /// </summary>
        private void PlaySuccessSound()
        {
            PlayVoice(1);
            var index = Random.Range(0, _successSoundId.Count);
            var id = _successSoundId[index];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
        }

        #endregion

        #region 播放Spine

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
           // string log = string.Format("SpineGoName:{0}---SpineAniName:{1}---time:{2}---isLoop:{3}", go.name, name, time, isLoop);
           // Debug.Log(log);
            return time;
        }

        #endregion

        #region 停止Audio
        private void StopAllAudio()
        {
           // string log = "StopAllAudio";
           // Debug.Log(log);
            SoundManager.instance.StopAudio();
        }

        private void StopAudio(SoundManager.SoundType type)
        {
           // string log = string.Format("StopAudio Type:{0}", type);
           // Debug.Log(log);
            SoundManager.instance.StopAudio(type);
        }

        private void StopAudio(string audioName)
        {
            //string log = string.Format("StopAudio Name:{0}", audioName);
            //Debug.Log(log);
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
        private void BellSpeck(int index,  GameObject go, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
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
           // string log = string.Format("parentName:{0}---parentChildCount:{1}", parent.name, parent.childCount);
           // Debug.Log(log);
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                RemoveEvent(child);
                AddEvent(child, callBack);
            }
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
          //  string log = string.Format("AddEvent GoName:{0}", go.name);
          //  Debug.Log(log);
            PointerClickListener.Get(go).onClick = g => {
             //   Debug.Log("OnClick GoName:" + g.name);
                callBack?.Invoke(g);
            };
        }

        private void RemoveEvent(GameObject go)
        {
           // string log = string.Format("RemoveEvent GoName:{0}", go.name);
           // Debug.Log(log);
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
    }
}
