using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;

namespace ILFramework.HotClass
{
    public class Course721Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;


        private Vector3 lastPos;//鼠标上次位置
        private Vector3 currentPos;//鼠标当前位置
         private float offset;//两次位置的偏移值

        private Transform DragObj;
        private Transform car;
        private Transform chilun;
        private Transform shijian;
        private bool _canDrag;
        private mILDrager _iLDragers;
        private Transform _Mask;
        private Transform _mask;
        private Transform shou;
        private bool _alreadyDrag;
        private bool _spinePlaying;
        private Transform roate1;
        private Transform roate2;
        private float time;
        private float MouseOffsetX;
       float OldPos;
        float NewPos;
        float chiValue;
        private int ceshizhi;
        private bool isPlayingAudio;
        private Transform _videos;
        private VideoPlayer _videoPlayer;
        private Transform DragRect;

        bool isPlaying = false;
        float b;
        float c;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            DragObj = curTrans.Find("drag");
            car = curTrans.Find("drag/car");
            chilun = curTrans.Find("spine/chilun");
            shijian = curTrans.Find("spine/shijian");
            _Mask = curTrans.Find("_Mask");
            _mask = curTrans.Find("_mask");
            shou = curTrans.Find("spine/shou");
            _spinePlaying = true;
           
            roate1 = curTrans.Find("shijian/roate1");
            roate2 = curTrans.Find("chilun");
            DragRect = curTrans.Find("DragRect");
            isPlayingAudio = false;
            ceshizhi = 0;
            _alreadyDrag = true;
            SoundManager.instance.ShowVoiceBtn(false);
            _canDrag = true;
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            lastPos = new Vector3(0,0,0);
            currentPos = new Vector3(0, 0, 0);
            offset = 0;
            OldPos =0;
            NewPos = 0;
            chiValue = 0;
            time = 0;

            Vector2 newXX = DragObj.transform.position;
            float newTimer = 0;
            bool canUpdate = false;
            bool notMove = false;

            bool a = true;
            

            GameInit();
         
            GameStart();
            InitILDragers();
            AddDragersEvent();

            UpDate(true, 0.1f, () =>
            {
                if (ceshizhi == 1) 
                {
                    if (a)
                    {
                        b = Input.mousePosition.x;
                        a = false;
                    }
                    else
                    {
                        a = true;
                        c = Input.mousePosition.x;
                    }
                    if (b == c)
                    {
                      car.GetComponent<SkeletonGraphic>().freeze = true;
                        StopAudio1(SoundManager.SoundType.SOUND);
                        isPlayingAudio = false;
                    }
                    else
                    {
                        if (isPlayingAudio==false) 
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                            isPlayingAudio = true;
                        }
                        car.GetComponent<SkeletonGraphic>().freeze = false;
                    }               
                }
               
                //if(Input.GetMouseButtonDown(0))
                //{
                //    canUpdate = true;
                //    newTimer = Time.realtimeSinceStartup;
                //    newXX = DragObj.transform.position;
                //}
                //if (Input.GetMouseButtonUp(0))
                //{
                //    canUpdate = false;
                //}

                //if (canUpdate)
                //{

                //    if (Time.realtimeSinceStartup - newTimer < 0.5f && Vector2.Distance(DragObj.transform.position, newXX) < 10)
                //    {
                //        Debug.LogError("111");
                //        newTimer = Time.realtimeSinceStartup;
                //    }
                //    else
                //    {
                //        Debug.LogError("222");
                //        newTimer = Time.realtimeSinceStartup;
                //        newXX = DragObj.transform.position;
                //    }
                //}
                //Debug.Log("执行");
                //if (ceshizhi == 1)
                //{
                //    if (Input.GetMouseButtonDown(0))
                //    {
                //        //记录当前位置
                //        NewPos = Input.mousePosition.x;
                //        if (OldPos == 0)
                //        {
                //            //空等一帧
                //        }
                //        else
                //        {
                //            //差值等于当前帧数 减去 上一帧数
                //            chiValue = NewPos - OldPos;
                //        }
                //        //把上一帧存储的鼠标位置赋值给定义的上一帧鼠标位置变量
                //        OldPos = NewPos;
                //    }
                //    if (chiValue < 1)
                //    {
                //        Debug.Log("123");
                //    }
                //}

            });
        }







        private void GameInit()
        {
            talkIndex = 1;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
           // if (_videos.childCount != 0)
              //  UnityEngine.Object.DestroyImmediate(_videos.GetChild(0).gameObject);
            _mask.gameObject.SetActive(false);
            _Mask.gameObject.SetActive(true);
            _alreadyDrag = true;

            StopAllCoroutines();
            roate1.rotation = new Quaternion(0, 0, 0,0);
            roate2.rotation = new Quaternion(0, 0, 0, 0);
            DragObj.position = curTrans.Find("dragposition").position;

            shou.gameObject.SetActive(false);
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            //mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () => 
            //{ 
            //  //  Max.SetActive(false); isPlaying = false; 

            //}));
            SoundManager.instance.ShowVoiceBtn(true);
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
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
           

            if (talkIndex == 1)
            {
                
                Debug.Log("小手");
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                    {
                        Max.SetActive(false);
                        _Mask.gameObject.SetActive(false);
                        shou.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(shou.gameObject, "shou", true);
                        mono.StartCoroutine(IEDelay(3f, () =>
                        {

                            SpineManager.instance.DoAnimation(shou.gameObject, "kong", true);
                            //开始循环
                            mono.StartCoroutine(IEUpdateHand(_alreadyDrag, 10f, () =>
                                {
                                    SpineManager.instance.DoAnimation(shou.gameObject, "shou", true);
                                    mono.StartCoroutine(IEDelay(3f, () =>
                                    {
                                        SpineManager.instance.DoAnimation(shou.gameObject, "kong", true);
                                        
                                    }));
                                  
                                }));
                        }));
                    }));
                
            }
            if (talkIndex == 2)
            {
                _Mask.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, ()=> 
                {
                    Max.SetActive(true);
                }, () =>
                    {
                        Max.SetActive(false);
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
            }
            //if (talkIndex == 3) 
            //{
            //    OnPlay();
            //    _Mask.gameObject.SetActive(false);
            //}

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void OnPlay() 
        {
            HideVoiceBtn();
            _mask.gameObject.SetActive(true);
            StopAudio1(SoundManager.SoundType.BGM);
        
            PlayOnClickSound();

            var soundIndex = 0;
            string url = string.Empty;
            url = "1.mp4";
            ResourcesVideoPrefab();

            Delay(0.5f, () => {

                // Delay(PlaySound(soundIndex), OnClickMask);
               // Delay(PlaySound(soundIndex), OnClickMask);
               // SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
              //  _videoPlayer.url = GetVideoPath(url);
             //   _videoPlayer.Play();

            });
            void OnClickMask()
            {
                Debug.LogError("OK");
                AddEvent(_mask.gameObject, g => {
                    RemoveEvent(g);
                    HideAllChilds(_videos);
                    _mask.gameObject.Hide();
                   // _Mask.gameObject.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
                //    _isPlaying = false;
                  //  ShowVoiceBtn();
                    UnityEngine.Object.DestroyImmediate(_videoPlayer.gameObject);
                    DragObj.position = curTrans.Find("dragposition").position;
                });
            }
        }


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
            mono.StartCoroutine(IEPlaySequenceSpine(go, spineNames));
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
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
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
            mono.StartCoroutine(IEDelay(delay, callBack));
        }

        private void UpDate(bool isStart, float delay, Action callBack)
        {
            mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
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
        IEnumerator IEUpdateHand(bool isStart, float delay, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                if (ceshizhi == 1) 
                {
                    yield break;
                }
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
            mono.StopAllCoroutines();
        }

        private void StopCoroutines(string methodName)
        {
            mono.StopCoroutine(methodName);
        }

        private void StopCoroutines(IEnumerator routine)
        {
            mono.StopCoroutine(routine);
        }

        private void StopCoroutines(Coroutine routine)
        {
            mono.StopCoroutine(routine);
        }

        #endregion

        #region Bell讲话

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, bool isBell = true, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, isBell));
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

        #endregion

    
        void InitILDragers()
        {
            _iLDragers = new mILDrager();
            _iLDragers = DragObj.GetComponent<mILDrager>();
            
        }
       private void AddDragersEvent()
        {
                _iLDragers.SetDragCallback(DragStart, Draging, DragEnd);
          
        }

        float lastX;
        float newX;

       
        private void DragStart(Vector3 position, int type, int index)
        {
            _alreadyDrag = false;
            lastPos.x= position.x;
            lastX = position.x;
            // StopAllCoroutines();
            shou.gameObject.SetActive(false);
            ceshizhi = 1;
            SpineManager.instance.DoAnimation(shou.gameObject,"kong",false);
            chilun.GetComponent<SkeletonGraphic>().freeze = false;
            shijian.GetComponent<SkeletonGraphic>().freeze = false;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);

            SpineManager.instance.DoAnimation(car.gameObject, "animation2", true);
            car.GetComponent<SkeletonGraphic>().freeze = false;

            
        }
        private void Draging(Vector3 position, int type, int index)
        {
            //mono.StartCoroutine(IEUpdate(true, 0.01f, () =>
            //{

            //}));
            //if (car.GetComponent<SkeletonGraphic>().freeze == false)
            //{
            //    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
            //}
            //else 
            //{
            //    StopAudio1(SoundManager.SoundType.SOUND);
            //}
            newX = position.x;
            if (lastX == newX)
            {
              //  SpineManager.instance.DoAnimation(car.gameObject, "animation", false);
            }
            else
            {
                bool isDY = newX > lastX ? true : false;
                #region 方法1
                //if (isDY&&_canDrag)
                //{
                //    _canDrag = false;
                //    SoundManager.instance.ShowVoiceBtn(false);
                //    //    DragObj.GetComponent<mILDrager>().isActived = false;

                //    SpineManager.instance.DoAnimation(car.gameObject, "animation2", true);
                //    SpineManager.instance.DoAnimation(chilun.gameObject, "a1", false,()=> 
                //    {
                //        SpineManager.instance.DoAnimation(car.gameObject, "animation", false);
                ////        DragObj.GetComponent<mILDrager>().isActived = true;
                //        _canDrag = true;
                //        SoundManager.instance.ShowVoiceBtn(true);
                //    });
                //    SpineManager.instance.DoAnimation(shijian.gameObject, "b1", false,()=> 
                //    {
                //        SpineManager.instance.DoAnimation(car.gameObject, "animation", false);
                //  //      DragObj.GetComponent<mILDrager>().isActived = true;
                //        _canDrag = true;
                //        SoundManager.instance.ShowVoiceBtn(true);


                //    });


                //    Debug.LogError("向右");
                //    lastX = newX;
                //}
                #endregion
                #region 方法1
                //else if (isDY==false&&_canDrag)
                //{

                //    _canDrag = false;
                //    SoundManager.instance.ShowVoiceBtn(false);
                //    //   DragObj.GetComponent<mILDrager>().isActived = false;
                //    SpineManager.instance.DoAnimation(car.gameObject, "animation2", true);
                //    SpineManager.instance.DoAnimation(chilun.gameObject, "a3",false,()=> 
                //    {
                //        SpineManager.instance.DoAnimation(car.gameObject, "animation", false);
                //   //     DragObj.GetComponent<mILDrager>().isActived = true;
                //        _canDrag = true;
                //        SoundManager.instance.ShowVoiceBtn(true);
                //    });
                //    SpineManager.instance.DoAnimation(shijian.gameObject, "b3", false,()=> 
                //    {
                //        SpineManager.instance.DoAnimation(car.gameObject, "animation", false);
                //   //     DragObj.GetComponent<mILDrager>().isActived = true;
                //        _canDrag = true;
                //        SoundManager.instance.ShowVoiceBtn(true);

                //    });

                //    lastX = newX;
                //    Debug.LogError("向左");
                //}
                #endregion
                #region  方法2
                //if (isDY && _canDrag&&_spinePlaying) 
                //{
                //    _spinePlaying = false;
              
                //    Debug.LogError("向右");
                //    SpineManager.instance.DoAnimation(car.gameObject, "animation2",true);
                //    SpineManager.instance.DoAnimation(chilun.gameObject,"a1",false);
                //    SpineManager.instance.DoAnimation(shijian.gameObject, "b1", false,()=> 
                //    {
                //        _spinePlaying = true;
                //    });
                //    lastX = newX;
                //}
                #endregion
                #region 方法2
                //else if (isDY == false && _canDrag&& _spinePlaying)
                //{
                //    _spinePlaying = false;
                //    Debug.LogError("向左");
                //    SpineManager.instance.DoAnimation(car.gameObject, "animation2", true);
                //    SpineManager.instance.DoAnimation(chilun.gameObject, "a3", false);
                //    SpineManager.instance.DoAnimation(shijian.gameObject, "b3", false, () =>
                //    {
                //        _spinePlaying = true;
                //    });
                //    lastX = newX;
                //}
                #endregion
                if (isDY && _canDrag)
                {
                   
                    //if (newX - lastX > 0.00001f)
                    //{
                    //    car.GetComponent<SkeletonGraphic>().freeze = false;
                    //}
                    //else if(newX - lastX <= 0.00001f)
                    //{

                    //}
                    //if (newX == lastX) 
                    //{
                    //    car.GetComponent<SkeletonGraphic>().freeze = true;
                    //}
                    MouseOffsetX= Input.GetAxisRaw("Mouse X");
                    float speed = 20;
                    Vector3 eular1 = Vector3.forward * MouseOffsetX * speed;
                    roate1.Rotate(eular1);
                    roate2.Rotate(eular1 * 7.5f);
                    lastX = newX;
                    
                }
                else if (isDY == false && _canDrag) 
                {
                    MouseOffsetX = Input.GetAxisRaw("Mouse X");
                    
                    float speed =20;
                    Vector3 eular1 = Vector3.forward * MouseOffsetX * speed;
                    roate1.Rotate(eular1);
                    roate2.Rotate(eular1 * 7.5f);
                    lastX = newX;
                   
                }
            }

            //if (position.x > lastPos.x)
            //{
            //    Debug.Log("向右");
            //    lastPos.x = position.x;
            //}
            //else 
            //{
            //    Debug.Log("向左");
            //}

            //mono.StartCoroutine(IEUpdate());
        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            ceshizhi = 0;
            Debug.Log(" DragEnd执行成功");
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            _spinePlaying = true;
            chilun.GetComponent<SkeletonGraphic>().freeze = true;
            shijian.GetComponent<SkeletonGraphic>().freeze = true;
            car.GetComponent<SkeletonGraphic>().freeze = true;
            talkIndex = 3;
           // SoundManager.instance.ShowVoiceBtn(true);
            //SpineManager.instance.DoAnimation(car.gameObject, "animation", true);
            //SpineManager.instance.DoAnimation(chilun.gameObject, "a1", true);
            //SpineManager.instance.DoAnimation(shijian.gameObject, "b1", true);
        }

        #region 动态加载资源


        private void ResourcesVideoPrefab()
        {
            var go = ResourceManager.instance.LoadResourceAB<GameObject>(Util.GetHotfixPackage("Course721Part1"), "VideoPlayer");
            var video = GameObject.Instantiate(go, _videos);

            video.transform.SetParent(_videos);
            _videoPlayer = video.GetComponent<VideoPlayer>();
            _videoPlayer.targetTexture.Release();
        }

        private string GetVideoPath(string videoPath)
        {
            var path = LogicManager.instance.GetVideoPath(videoPath);
            return path;
        }
        private void StopAudio1(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
        }
        #endregion


    }
}
