using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
namespace ILFramework.HotClass
{
    public class Course217Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;

        private Transform _transhBgs;  
        private Transform _spinesTra;  
        private Transform _transhsTra;
        private Transform _levelsTra;
        private Transform _spinesMaskTra;
        private Transform _mask;       
        private Transform _progressTran;
        private Transform _maskProgressTra;
        private Transform _starProgressTra;
        private Transform _endPosTra;

        private Transform _succeedSpines;
        private float _progressMax;
        private bool _isDroping = false;
       

        /// <summary>
        /// 拖拽数组
        /// </summary>
        private  List<ILDrager> _iLDragers;

        /// <summary>
        /// 释放数组
        /// </summary>
        private ILDroper[] _iLDropers;

        private int _levelId;
        private int _succesNum ;
        private bool _isPass;


        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
         
            bell = curTrans.Find("bell").gameObject;
            mono = curGo.GetComponent<MonoBehaviour>();

            _transhBgs = curTrans.Find("Bg/TranshBgs");

            _spinesTra = curTrans.Find("Spines");
            _transhsTra = _spinesTra.Find("Transhs");
            _levelsTra = _spinesTra.Find("Levels");        
            _endPosTra = _spinesTra.Find("EndPos");
            _spinesMaskTra = _spinesTra.Find("SpinesMask");

            _mask = curTrans.Find("mask");

            _progressTran = curTrans.Find("Progress");
            _maskProgressTra = _progressTran.Find("Mask/Progress");
            _starProgressTra = _progressTran.Find("Star/Progress");
            bell.Hide();
            GameStart();
        }
      
        void InitILDragers()
        {
            _iLDragers = new List<ILDrager>();

            for (int i = 0; i < _levelsTra.childCount; i++)
            {
                var child = _levelsTra.GetChild(i).transform;
                for (int j = 0; j < child.childCount; j++)
                {
                    var iLDrager = child.GetChild(j).GetComponent<ILDrager>();
                    _iLDragers.Add(iLDrager);
                }
            }
        }


        void GameStart()
        {
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            _levelId = 1;
            _succesNum = 0;
            _isPass = false;
            _mask.gameObject.Hide();
            _transhBgs.gameObject.Show();
          //  bell.Show();
          //  bell.transform.GetRectTransform().anchoredPosition = new Vector2(269, 3);


            for (int i = 0; i < _levelsTra.childCount; i++)
            {
                var isOneLevel = i == 0;
                var child = _levelsTra.GetChild(i);

                if (isOneLevel)
                    child.gameObject.Show();
                else
                    child.gameObject.Hide();

                for (int j = 0; j < child.childCount; j++)
                    child.GetChild(j).gameObject.Show();
            }


            _progressMax = _progressTran.Find("Mask").GetComponent<RectTransform>().rect.height;
            _succeedSpines = curGo.transform.Find("SucceedSpines");
            _succeedSpines.gameObject.Hide();

            _iLDropers = _endPosTra.GetComponentsInChildren<ILDroper>();


            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            InitILDragers();
            ResetProgress();
            GameInit();            
            AddDragersEvent();
            AddDropersEvent();
        }

        void GameInit()
        {
            talkIndex = 1;

            SpineManager.instance.DoAnimation( _starProgressTra.Find("Star/StarAni").gameObject,"xingxing",true);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,29,()=> { _spinesMaskTra.gameObject.Show(); },()=>{
              //  bell.SetActive(false);
                _spinesMaskTra.gameObject.SetActive(false);
            }));
        }


        /// <summary>
        /// 获取当前对应ILDroper
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ILDroper GetCurILDroper(int index)
        {
            int type = _iLDragers[index].dragType;
            ILDroper iLDroper = null;
            for (int i = 0; i < _iLDropers.Length; i++)
                if (_iLDropers[i].dropType == type)
                    iLDroper = _iLDropers[i];
            return iLDroper;
        }

      
        #region 垃圾Icon释放 

        private void AddDropersEvent()
        {
            foreach (var droper in _iLDropers)            
                droper.SetDropCallBack(DropDoAfter, null, DropFail);           
        }

        /// <summary>
        /// 释放成功
        /// </summary>
        /// <param name="type"></param>
        /// <param name="droperIndex"></param>
        /// <param name="unknown"></param>
        /// <returns></returns>
        private bool DropDoAfter(int type,int droperIndex,int unknown)
        {                    
            return true;
        }

        /// <summary>
        /// 释放失败
        /// </summary>
        /// <param name="type">垃圾桶类型</param>
        private void DropFail(int type)
        {
            PlayErrorTranshAni(type);          
        }
       
      
        #endregion

        #region 播放动画

        /// <summary>
        /// 播放正确的垃圾桶动画
        /// </summary>
        private void PlayCorrectTranshAni(int index,Action openSound,Action closeSound)
        {
            var iLDroper = GetCurILDroper(index);
            var iLDroperIndex = iLDroper.index;
            var curTransh =_transhsTra.GetChild(iLDroperIndex).gameObject;
            openSound?.Invoke();
            switch (iLDroperIndex)
            {
                case 0:

                    SpineManager.instance.DoAnimation(curTransh, "cy3", false,()=> {
                        _iLDragers[index].gameObject.SetActive(false);                     
                       closeSound?.Invoke();
                        SpineManager.instance.DoAnimation(curTransh, "cy5", false);
                    });
                    break;
                case 1:
                    SpineManager.instance.DoAnimation(curTransh, "khs3", false, () => {
                        _iLDragers[index].gameObject.SetActive(false);                     
                        closeSound?.Invoke();
                        SpineManager.instance.DoAnimation(curTransh, "khs5", false);
                    });
                    break;
                case 2:
                    SpineManager.instance.DoAnimation(curTransh, "yh3", false, () => {
                        _iLDragers[index].gameObject.SetActive(false);                      
                        closeSound?.Invoke();
                        SpineManager.instance.DoAnimation(curTransh, "yh5", false);
                    });
                    break;
                case 3:
                    SpineManager.instance.DoAnimation(curTransh, "qt3", false, () => {
                        _iLDragers[index].gameObject.SetActive(false);                 
                        closeSound?.Invoke();
                        SpineManager.instance.DoAnimation(curTransh, "qt5", false);
                    });
                    break;
            }
        }

       
        /// <summary>
        /// 播放错误的垃圾桶动画
        /// </summary>
        private void PlayErrorTranshAni(int type)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 38, false);
            switch (type)
            {
                case 1:
                    Debug.Log("错误的垃圾桶:厨房垃圾");
                    SpineManager.instance.DoAnimation(_transhsTra.Find("1").gameObject, "cy2",false);
                    break;
                case 2:
                    Debug.Log("错误的垃圾桶:可回收垃圾");
                    SpineManager.instance.DoAnimation(_transhsTra.Find("2").gameObject, "khs2", false);
                    break;
                case 3:
                    Debug.Log("错误的垃圾桶:有害垃圾");
                    SpineManager.instance.DoAnimation(_transhsTra.Find("3").gameObject, "yh2", false);
                    break;
                case 4:
                    Debug.Log("错误的垃圾桶:其他垃圾");
                    SpineManager.instance.DoAnimation(_transhsTra.Find("4").gameObject, "qt2", false);
                    break;            
            }
        }




        #endregion

        #region 播放语音

       
        /// <summary>
        /// 播放正确的语音
        /// </summary>
        /// <param name="index">当前拖到的物体在集合中的索引</param>
        /// <param name="speckeringCallBack">正在讲话时的回调</param>
        /// <param name="speckendCallBack">讲话结束的回调</param>
        private void PlayCorrectVoice(int index,Action speckeringCallBack, Action speckendCallBack)
        {
            switch (index)
            {
                case 0:  
                    PlayBellVoice(SoundManager.SoundType.VOICE, 4, speckeringCallBack, speckendCallBack);
                    break;
                case 1:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 5, speckeringCallBack, speckendCallBack);
                    break;
                case 2:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 6, speckeringCallBack, speckendCallBack);
                    break;
                case 3:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 7, speckeringCallBack, speckendCallBack);
                    break;
                case 4:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 12, speckeringCallBack, speckendCallBack);
                    break;
                case 5:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 13, speckeringCallBack, speckendCallBack);
                    break;
                case 6:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 14, speckeringCallBack, speckendCallBack);
                    break;
                case 7:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 15, speckeringCallBack, speckendCallBack);
                    break;
                case 8:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 30, speckeringCallBack, speckendCallBack);
                    break;
                case 9:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 31, speckeringCallBack, speckendCallBack);
                    break;
                case 10:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 32, speckeringCallBack, speckendCallBack);
                    break;
                case 11:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 33, speckeringCallBack, speckendCallBack);
                    break;
                case 12:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 24, speckeringCallBack, speckendCallBack);
                    break;
                case 13:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 25, speckeringCallBack, speckendCallBack);
                    break;
                case 14:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 26, speckeringCallBack, speckendCallBack);
                    break;
                case 15:
                    PlayBellVoice(SoundManager.SoundType.VOICE, 27, speckeringCallBack, speckendCallBack);
                    break;
            }
        }

   
        /// <summary>
        /// 播放Bell语音
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        private void PlayBellVoice(SoundManager.SoundType type,int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            mono.StartCoroutine(SpeckerCoroutine(type, clipIndex, method_1,method_2,len));
        }




        #endregion

        #region 垃圾Icon拖拽

        /// <summary>
        /// 添加拖拽事件
        /// </summary>
        private void AddDragersEvent()
        {         
            foreach (var drager in _iLDragers)           
                drager.SetDragCallback(DragStart,Draging,DragEnd,OnClickIocn);           
        }

        /// <summary>
        /// 拖拽开始
        /// </summary>
        private void DragStart(Vector3 position, int type,int index)
        {                     
        }
        /// <summary>
        /// 拖拽中
        /// </summary>      
        private void Draging(Vector3 position, int type, int index)
        {
            if (!_isDroping)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 40, false);
                DragingAni(index, true);
                _isDroping = true;
            }
        }

        private void DragingAni(int index,bool isLoop)
        {
            var gameObject = _iLDragers[index].transform.GetChild(0).gameObject;

            switch (index)
            {
                case 0:
                    SpineManager.instance.DoAnimation(gameObject, "h2", isLoop);
                    break;
                case 1:
                    SpineManager.instance.DoAnimation(gameObject, "d2", isLoop);
                    break;
                case 2:
                    SpineManager.instance.DoAnimation(gameObject, "l2", isLoop);
                    break;
                case 3:
                    SpineManager.instance.DoAnimation(gameObject, "k2", isLoop);
                    break;
                case 4:
                    SpineManager.instance.DoAnimation(gameObject, "a2", isLoop);
                    break;
                case 5:
                    SpineManager.instance.DoAnimation(gameObject, "p1", isLoop);
                    break;
                case 6:
                    SpineManager.instance.DoAnimation(gameObject, "i2", isLoop);
                    break;
                case 7:
                    SpineManager.instance.DoAnimation(gameObject, "o2", isLoop);
                    break;
                case 8:
                    SpineManager.instance.DoAnimation(gameObject, "g2", isLoop);
                    break;
                case 9:
                    SpineManager.instance.DoAnimation(gameObject, "n2", isLoop);
                    break;
                case 10:
                    SpineManager.instance.DoAnimation(gameObject, "m2", isLoop);
                    break;
                case 11:
                    SpineManager.instance.DoAnimation(gameObject, "b2", isLoop);
                    break;
                case 12:
                    SpineManager.instance.DoAnimation(gameObject, "f2", isLoop);
                    break;
                case 13:
                    SpineManager.instance.DoAnimation(gameObject, "c2", isLoop);
                    break;
                case 14:
                    SpineManager.instance.DoAnimation(gameObject, "e2", isLoop);
                    break;
                case 15:
                    SpineManager.instance.DoAnimation(gameObject, "j2", isLoop);
                    break;
            }
        }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        private void DragEnd(Vector3 position, int type, int index,bool isMatch)
        {
            _isDroping = false;
            DragingAni(index, false);

            if (isMatch)
            {             
                SetProgress(_progressMax / 4);
                PlayCorrectVoice(index,()=> { _spinesMaskTra.gameObject.SetActive(true); },()=> {
                    _spinesMaskTra.gameObject.SetActive(false);
                    CallBack();
                });
                PlayCorrectTranshAni(index,
                ()=> { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 36, false); },
                ()=> { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 37, false);
                    _iLDragers[index].DoReset();
                    _iLDragers[index].gameObject.Hide();
                });                                                   
            }
            else
            {
                _iLDragers[index].DoReset();                    
            }
        }


        #region 进度条

        /// <summary>
        /// 重置进度条
        /// </summary>
        private void ResetProgress()
        {

            var maskProgressRect = _maskProgressTra.GetComponent<RectTransform>();
            maskProgressRect.sizeDelta = new Vector2(maskProgressRect.rect.width, 0);

            var starProgressRect = _starProgressTra.GetComponent<RectTransform>();
            starProgressRect.sizeDelta = new Vector2(starProgressRect.rect.width, 0);
        }



        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="addProgress"></param>
        /// <param name="callBack"></param>
        private void SetProgress(float addProgress)
        {
            var maskProgressRect = _maskProgressTra.GetComponent<RectTransform>();
            var starProgressRect = _starProgressTra.GetComponent<RectTransform>();

            _succesNum++;

            if (_succesNum == 4)           
                _isPass = true;            
            else           
                _isPass = false; ;

          //  Debug.LogError(string.Format("_succesNum:{0}---_isPass:{1}", _succesNum,_isPass)) ;
          

            if (_isPass)
            {
                var tweener1 = maskProgressRect.DOSizeDelta(new Vector2(maskProgressRect.rect.width, maskProgressRect.rect.height + addProgress), 0.2f);
                var tweener2 = starProgressRect.DOSizeDelta(new Vector2(starProgressRect.rect.width, starProgressRect.rect.height + addProgress), 0.2f);

                DOTween.Sequence().Append(tweener1)
                                  .Join(tweener2);

                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 35, false);
            }     
        }

        private void CallBack()
        {
            if (_isPass)
            {
                _levelId++;
               // Debug.LogError("_levelId："+ _levelId);
            
                if (_levelId > 4)
                {
                    _transhBgs.gameObject.SetActive(false);
                    _succeedSpines.gameObject.SetActive(true);
                    var go = _succeedSpines.Find("Spine").gameObject;
                    SpineManager.instance.DoAnimation(go, "animation", false,()=> {
                        SpineManager.instance.DoAnimation(go,"animation2",true);
                    });
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 41, false);
                    _mask.gameObject.SetActive(true);
                 //   bell.transform.position = new Vector3(960, 300, 0);
                //    bell.gameObject.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 34));
                    return;
                }

                var lastlevelName = "Level" + (_levelId - 1);
                var curlevelName = "Level" + _levelId ;

                _levelsTra.Find(lastlevelName).gameObject.Hide();
                _levelsTra.Find(curlevelName).gameObject.Show();
   
                _succesNum = 0;
            }
        }


        #endregion

        /// <summary>
        /// 垃圾Icon点击
        /// </summary>
        /// <param name="index"></param>
        private void OnClickIocn(int index)
        {
            GameObject go = _iLDragers[index].gameObject;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 39, false);
            switch (index)
            {
                case 0:
                    OnClickGlassBottle(go);
                    break;
                case 1:
                    OnClickEggshell(go);
                    break;
                case 2:
                    OnClickBananaSkin(go);
                    break;
                case 3:
                    OnClickMercurialThermometer(go);
                    break;
                case 4:
                    OnClickPencil(go);
                    break;
                case 5:
                    OnClickFishbone(go);
                    break;
                case 6:
                    OnClickOldPhones(go);
                    break;
                case 7:
                    OnClickPopcan(go);
                    break;
                case 8:
                    OnClickAppleCore(go);
                    break;
                case 9:
                    OnClickToothbrush(go);
                    break;
                case 10:
                    OnClickCigaretteEnd(go);
                    break;
                case 11:
                    OnClickIncandescentTube(go);
                    break;
                case 12:
                    OnClickMilkCarton(go);
                    break;
                case 13:
                    OnClickOldCell(go);
                    break;
                case 14:
                    OnClickOldTowel(go);
                    break;
                case 15:
                    OnClickPlasticBottles(go);
                    break;
            }
        }

        #endregion

        #region 垃圾Icon点击事件

        /// <summary>
        /// 点击图标
        /// </summary>
        /// <param name="go"></param>
        /// <param name="spineName"></param>
        void OnClickItme(GameObject go, int index, string spineName)
        {
            
            _spinesMaskTra.gameObject.SetActive(true);
            var parent = go.transform.Find(go.name).gameObject;
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, index, () => {
                SpineManager.instance.DoAnimation(parent, spineName, false);
            }, () => {              
                _spinesMaskTra.gameObject.SetActive(false);
            }));
        }
 
        #region 第一关        

        /// <summary>
        /// 点击玻璃瓶
        /// </summary>
        void OnClickGlassBottle(GameObject go)
        {
            OnClickItme(go, 0, "h2");
        }

        /// <summary>
        /// 点击鸡蛋壳
        /// </summary>
        void OnClickEggshell(GameObject go)
        {
            OnClickItme(go, 1, "d2");
        }

        /// <summary>
        /// 点击香蕉皮
        /// </summary>
        void OnClickBananaSkin(GameObject go)
        {
            OnClickItme(go, 2, "l2");
        }

        /// <summary>
        /// 点击水银温度计
        /// </summary>
        void OnClickMercurialThermometer(GameObject go)
        {
            OnClickItme(go, 3, "k2");
        }
        #endregion

        #region 第二关

        /// <summary>
        /// 点击铅笔
        /// </summary>
        /// <param name="go"></param>
        void OnClickPencil(GameObject go)
        {
            OnClickItme(go, 8, "a2");
        }

        /// <summary>
        /// 点击鱼骨头
        /// </summary>
        /// <param name="go"></param>
        void OnClickFishbone(GameObject go)
        {
            OnClickItme(go, 9, "p2");
        }

        /// <summary>
        /// 点击旧手机
        /// </summary>
        /// <param name="go"></param>
        void OnClickOldPhones(GameObject go)
        {
            OnClickItme(go, 10, "i2");
        }

        /// <summary>
        /// 点击易拉罐
        /// </summary>
        /// <param name="go"></param>
        void OnClickPopcan(GameObject go)
        {
            OnClickItme(go, 11, "o2");
        }

        #endregion

        #region 第三关

        /// <summary>
        /// 点击苹果核
        /// </summary>
        /// <param name="go"></param>
        void OnClickAppleCore(GameObject go)
        {
            OnClickItme(go, 16, "g2");
        }

        /// <summary>
        /// 点击牙刷
        /// </summary>
        /// <param name="go"></param>
        void OnClickToothbrush(GameObject go)
        {
            OnClickItme(go, 17, "n2");
        }

        /// <summary>
        /// 点击烟头
        /// </summary>
        /// <param name="go"></param>
        void OnClickCigaretteEnd(GameObject go)
        {
            OnClickItme(go, 18, "m2");
        }

        /// <summary>
        /// 点击白炽灯管
        /// </summary>
        /// <param name="go"></param>
        void OnClickIncandescentTube(GameObject go)
        {
            OnClickItme(go, 19, "b2");
        }

        #endregion

        #region 第四关

        /// <summary>
        /// 点击牛奶纸盒
        /// </summary>
        /// <param name="go"></param>
        void OnClickMilkCarton(GameObject go)
        {
            OnClickItme(go, 20, "f2");
        }

        /// <summary>
        /// 点击旧电池
        /// </summary>
        /// <param name="go"></param>
        void OnClickOldCell(GameObject go)
        {
            OnClickItme(go, 21, "c2");
        }

        /// <summary>
        /// 点击旧毛巾
        /// </summary>
        /// <param name="go"></param>
        void OnClickOldTowel(GameObject go)
        {
            OnClickItme(go, 22, "e2");
        }

        /// <summary>
        /// 点击塑料瓶
        /// </summary>
        /// <param name="go"></param>
        void OnClickPlasticBottles(GameObject go)
        {
            OnClickItme(go, 23, "j2");
        }

        #endregion

        #endregion
      
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");

            if (method_2 != null)
            {
                method_2();
            }
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex == 1)
            {

            }
            talkIndex++;
        }


     
    }
}
