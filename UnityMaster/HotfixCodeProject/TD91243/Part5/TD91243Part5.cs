using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
	
	public enum RoleType
	{
	   Bd,
       Xem,
       Child,
       Adult,		
	}
	
    public class TD91243Part5
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;


        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;

        private Transform drag1;
        private Transform drag2;
        private Transform drag3;
        private Transform drop1;
        private Transform drop2;
        private Transform drop3;
        private GameObject _dDD;
        private Transform content;
        private int index1;
		
        private GameObject _sDD;
						
		            
      
	   
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private List<mILDrager> _MILDragers;
        private List<mILDrager> _MILDragers2;
        private List<mILDrager> _MILDragers3;
        private Transform uiPanel;
        private Transform spineGo;

        private Transform drag;
        
        
        
        
		
		   
        
        
		
       

		
       
      
        private bool _isPlaying;

        void Start(object o)
        {
            
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");			
									
            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");

			
			_dDD = curTrans.GetGameObject("dDD");
			           			
			
			
         	_sDD = curTrans.GetGameObject("sDD");
            drag = curTrans.Find("Drag");
            drag1 = curTrans.Find("Drag/drag1");
            drag2 = curTrans.Find("Drag/drag2");
            drag3 = curTrans.Find("Drag/drag3");
            drop1 = curTrans.Find("drop1");
            drop2 = curTrans.Find("drop2");
            drop3 = curTrans.Find("drop3");
            spineGo = curTrans.Find("BG/content/SpineGo");
            uiPanel = curTrans.Find("UIPanel");
            index1 = 0;
            content = curTrans.Find("BG/content");








           




            GameInit();
            GameStart();
            AddDragEvent();
        }

        void InitData()
        {
            Input.multiTouchEnabled = false;
            _isPlaying = true;
            index1 = 0;
            content.transform.localScale = new Vector3(1, 1, 1);

            uiPanel.GetRectTransform().anchoredPosition = new Vector2(0, 0);

            uiPanel.GetChild(0).gameObject.SetActive(true);
            uiPanel.GetChild(1).gameObject.SetActive(false);
            uiPanel.GetChild(2).gameObject.SetActive(false);

            drag1.gameObject.SetActive(true); drag2.gameObject.SetActive(false); drag3.gameObject.SetActive(false);

            for (int i = 0; i<drop1.childCount; i++) 
            {
                drop1.GetChild(i).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 0);
            }
            for (int i = 0; i < drop2.childCount; i++)
            {
                drop2.GetChild(i).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 0);
            }
            for (int i = 0; i < drop3.childCount; i++)
            {
                drop3.GetChild(i).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 0);
            }

            for (int i = 0; i < drag1.childCount; i++)
            {
                drag1.GetChild(i).gameObject.SetActive(true);
            }

            for (int i = 0; i < drag1.childCount; i++)
            {
                drag2.GetChild(i).gameObject.SetActive(true);
            }

            for (int i = 0; i < drag1.childCount; i++)
            {
                drag3.GetChild(i).gameObject.SetActive(true);
            }

            for (int i = 0; i < spineGo.childCount; i++)
            {
                spineGo.GetChild(i).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(spineGo.GetChild(i).gameObject, "kong", false);
            }

            drag1.Find("0").GetRectTransform().anchoredPosition = new Vector2(-575, 109);
            drag1.Find("1").GetRectTransform().anchoredPosition = new Vector2(-285, 109);
            drag1.Find("2").GetRectTransform().anchoredPosition = new Vector2(4, 109);
            drag1.Find("3").GetRectTransform().anchoredPosition = new Vector2(301, 109);
            drag1.Find("4").GetRectTransform().anchoredPosition = new Vector2(564, 109);

            drag2.Find("0").GetRectTransform().anchoredPosition = new Vector2(-556, 109);
            drag2.Find("1").GetRectTransform().anchoredPosition = new Vector2(-283, 109);
            drag2.Find("2").GetRectTransform().anchoredPosition = new Vector2(9, 109);
            drag2.Find("3").GetRectTransform().anchoredPosition = new Vector2(299, 109);
            drag2.Find("4").GetRectTransform().anchoredPosition = new Vector2(567, 109);

            drag3.Find("0").GetRectTransform().anchoredPosition = new Vector2(-560, 109);
            drag3.Find("1").GetRectTransform().anchoredPosition = new Vector2(-287, 109);
            drag3.Find("2").GetRectTransform().anchoredPosition = new Vector2(11, 109);
            drag3.Find("3").GetRectTransform().anchoredPosition = new Vector2(295, 109);
            drag3.Find("4").GetRectTransform().anchoredPosition = new Vector2(566, 109);


            for (int i = 0; i < drag1.childCount; i++) 
            {
                drag1.GetChild(i).gameObject.GetComponent<RawImage>().texture = drag1.GetChild(i).gameObject.GetComponent<BellSprites>().texture[1];
            }
            for (int i = 0; i < drag2.childCount; i++)
            {
                drag2.GetChild(i).gameObject.GetComponent<RawImage>().texture = drag2.GetChild(i).gameObject.GetComponent<BellSprites>().texture[1];
            }
            for (int i = 0; i < drag3.childCount; i++)
            {
                drag3.GetChild(i).gameObject.GetComponent<RawImage>().texture = drag3.GetChild(i).gameObject.GetComponent<BellSprites>().texture[1];
            }




            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
			           
        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); 
			StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); 
			
			          
			
		     _dDD.Hide(); 
			
			
			
			 _sDD.Hide(); 
					
			
			
			
			
			

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
						         
        }



        void GameStart()
        {
            _mask.Show(); _startSpine.Show();
			
			
			

            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        PlayBgm(0);//ToDo...改BmgIndex
                        _startSpine.Hide();
                        _sDD.Show();
                        //ToDo...
                        BellSpeck(_sDD, 0, null, ShowVoiceBtn,RoleType.Adult);
                     
                        
                    });
                });
            });
        }

        private void AddDragEvent()
        {
            CWLayers();
            _MILDragers = new List<mILDrager>();
            for (int i=0;i<drag1.childCount;i++) 
            {
                _MILDragers.Add(drag1.GetChild(i).GetComponent<mILDrager>());
            }
            _MILDragers2 = new List<mILDrager>();
            for (int i = 0; i < drag2.childCount; i++)
            {
                _MILDragers2.Add(drag2.GetChild(i).GetComponent<mILDrager>());
            }
            _MILDragers3 = new List<mILDrager>();
            for (int i = 0; i < drag3.childCount; i++)
            {
                _MILDragers3.Add(drag3.GetChild(i).GetComponent<mILDrager>());
            }
            foreach (var a in _MILDragers) 
            {
                a.SetDragCallback(DragStart,null,DragEnd);
            }
            foreach (var b in _MILDragers2)
            {
                b.SetDragCallback(DragStart2, null, DragEnd2);
            }
            foreach (var c in _MILDragers3)
            {
                c.SetDragCallback(DragStart3, null, DragEnd3);
            }
        }
        private void DragStart(Vector3 position, int type, int index) 
        {
            _MILDragers[index].transform.SetAsLastSibling();
            _MILDragers[index].transform.position = Input.mousePosition;
            drag1.Find(index.ToString()).GetComponent<RawImage>().texture = drag1.Find(index.ToString()).GetComponent<BellSprites>().texture[0];
            drop1.GetChild(index).gameObject.GetComponent<CustomImage>().color = new Color(1,1,1,1);
           
           
        }
        private void DragStart2(Vector3 position, int type, int index)
        {
            _MILDragers2[index-5].transform.SetAsLastSibling();
            _MILDragers2[index-5].transform.position = Input.mousePosition;
            drag2.Find((index-5).ToString()).GetComponent<RawImage>().texture = drag2.Find((index-5).ToString()).GetComponent<BellSprites>().texture[0];
            drop2.GetChild(index-5).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 1);


        }
        private void DragStart3(Vector3 position, int type, int index)
        {
            _MILDragers3[index-10].transform.SetAsLastSibling();
            _MILDragers3[index - 10].transform.position = Input.mousePosition;
            drag3.Find((index - 10).ToString()).GetComponent<RawImage>().texture = drag3.Find((index - 10).ToString()).GetComponent<BellSprites>().texture[0];
            drop3.GetChild(index - 10).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 1);


        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            
            if (isMatch)
            {
                index1++;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0);
               // Debug.Log(index.ToString());
                drag1.Find(index.ToString()).gameObject.SetActive(false);
                drop1.GetChild(index).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 0);
                SpineManager.instance.DoAnimation(spineGo.Find(index.ToString()).gameObject, (index + 1).ToString(), false);
                if (index1 == 5)
                {
                   
                    uiPanel.GetChild(0).gameObject.SetActive(false);
                    uiPanel.GetChild(1).gameObject.SetActive(true);
                    drag2.gameObject.SetActive(true);
                    drag2.gameObject.SetActive(true);
                    index1 = 0;
                }
            }
            else 
            {
                _MILDragers[index].DoReset();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                drop1.GetChild(index).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 0);
                drag1.Find(index.ToString()).GetComponent<RawImage>().texture = drag1.Find(index.ToString()).GetComponent<BellSprites>().texture[1];
               
            }
           
        }
        private void DragEnd2(Vector3 position, int type, int index, bool isMatch)
        {
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                index1++;
                // Debug.Log(index.ToString());
                drag2.Find((index-5).ToString()).gameObject.SetActive(false);
                drop2.GetChild(index-5).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 0);
                SpineManager.instance.DoAnimation(spineGo.Find(index.ToString()).gameObject, (index + 2).ToString(), false); 
                if (index1 == 5)
                {
                    index1 = 0;
                    uiPanel.GetChild(1).gameObject.SetActive(false);
                    uiPanel.GetChild(2).gameObject.SetActive(true);
                    drag3.gameObject.SetActive(true);
                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                _MILDragers2[index-5].DoReset();
                drop2.GetChild(index-5).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 0);
                drag2.Find((index-5).ToString()).GetComponent<RawImage>().texture = drag2.Find((index-5).ToString()).GetComponent<BellSprites>().texture[1];

            }
          
        }
        private void DragEnd3(Vector3 position, int type, int index, bool isMatch)
        {
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                index1++;
                // Debug.Log(index.ToString());
                drag3.Find((index - 10).ToString()).gameObject.SetActive(false);
                drop3.GetChild(index - 10).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 0);
                SpineManager.instance.DoAnimation(spineGo.Find(index.ToString()).gameObject, (index + 2).ToString(), false);
                if (index1 == 5)
                {
                    index = 0;
                    //画布放大
                    successeffect();
                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                _MILDragers3[index - 10].DoReset();
                drop3.GetChild(index - 10).gameObject.GetComponent<CustomImage>().color = new Color(1, 1, 1, 0);
                drag3.Find((index - 10).ToString()).GetComponent<RawImage>().texture = drag3.Find((index - 10).ToString()).GetComponent<BellSprites>().texture[1];

            }
          
        }
        void TalkClick()
        {
            HideVoiceBtn();
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sDD, 1, null, () =>
                    {
                        _sDD.SetActive(false);
                        StartGame();
                    },RoleType.Adult);
                    break;                                
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        private void CWLayers() 
        {
            drag1.Find("0").SetSiblingIndex(0);
            drag1.Find("1").SetSiblingIndex(1);
            drag1.Find("2").SetSiblingIndex(2);
            drag1.Find("3").SetSiblingIndex(3);
            drag1.Find("4").SetSiblingIndex(4);

            drag2.Find("0").SetSiblingIndex(0);
            drag2.Find("1").SetSiblingIndex(1);
            drag2.Find("2").SetSiblingIndex(2);
            drag2.Find("3").SetSiblingIndex(3);
            drag2.Find("4").SetSiblingIndex(4);

            drag3.Find("0").SetSiblingIndex(0);
            drag3.Find("1").SetSiblingIndex(1);
            drag3.Find("2").SetSiblingIndex(2);
            drag3.Find("3").SetSiblingIndex(3);
            drag3.Find("4").SetSiblingIndex(4);
        }
		



		

		
		

		

			
	    
     
		
        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();       

			//测试代码记得删
			//Delay(4,GameSuccess);
        }

		
		

		
		
        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            _mask.Show();
            _replaySpine.Show();
			_okSpine.Show();
            _successSpine.Hide();
            PlaySpine(_replaySpine, "fh2", () => {
                AddEvent(_replaySpine, (go) => {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine); 
					RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () => {
                        _okSpine.Hide();
                        PlayBgm(0); //ToDo...改BmgIndex
                        GameInit();       
                        //ToDo...	
                        
                        StartGame();
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () => {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound();
					PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _replaySpine.Hide();        

                        //ToDo...
                        //显示Middle角色并且说话  					
                        _dDD.Show(); BellSpeck(_dDD, 2,null,null,RoleType.Adult);

                    });
                });
            });

        }

        /// <summary>
        /// 游戏成功界面
        /// </summary>
        private void GameSuccess()
        {
            _mask.Show();
            _successSpine.Show();
            PlayCommonSound(3);			
			 PlaySpine(_successSpine, "6-12-z", () => { PlaySpine(_successSpine, "6-12-z2"); });         
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }

        private void successeffect() 
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,2);
            uiPanel.GetRectTransform().DOAnchorPosY(-300,1.5f);
            content.GetRectTransform().DOScale(1.2f,1.5f).OnComplete(()=> 
            {
                Delay(3.5f,()=> 
                {
                    GameSuccess();
                });
               
            });
           
        }

		

        #endregion

        #region 常用函数

        #region 语音按钮

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

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void HideChilds(Transform parent,int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }
        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
        }

        #endregion

        #region 拖拽相关

        /// <summary>
        /// 设置Drager回调
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="dragStart"></param>
        /// <param name="draging"></param>
        /// <param name="dragEnd"></param>
        /// <param name="onClick"></param>
        /// <returns></returns>
        private List<mILDrager> SetDragerCallBack(Transform parent, Action<Vector3, int, int> dragStart = null, Action<Vector3, int, int> draging = null, Action<Vector3, int, int, bool> dragEnd = null, Action<int> onClick = null)
        {
            var temp = new List<mILDrager>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var drager = parent.GetChild(i).GetComponent<mILDrager>();
                temp.Add(drager);
                drager.SetDragCallback(dragStart, draging, dragEnd, onClick);
            }

            return temp;
        }

        /// <summary>
        /// 设置Droper回调(失败)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="failCallBack"></param>
        /// <returns></returns>
        private List<mILDroper> SetDroperCallBack(Transform parent, Action<int> failCallBack = null)
        {
            var temp = new List<mILDroper>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var droper = parent.GetChild(i).GetComponent<mILDroper>();
                temp.Add(droper);
                droper.SetDropCallBack(null, null, failCallBack);
            }
            return temp;
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

        private GameObject FindGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        #endregion

        #region 音频相关

        private float PlayFailSound()
        {
            PlayCommonSound(5);

            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
            PlayCommonSound(4);
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Bd, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Bd, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

			switch(roleType)
			{
				case RoleType.Bd:
				     daiJi = "bd-daiji"; speak = "bd-speak";
				break;
				case RoleType.Xem:
				     daiJi = "daiji"; speak = "speak";
				break;
				case RoleType.Child:
				     daiJi = "animation"; speak = "animation2";
				 break;
				case RoleType.Adult:
				     daiJi = "daiji"; speak = "speak";
				break;
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

        #endregion

    }
}
