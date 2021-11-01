using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course8311Part2
    {

     

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;   
        private GameObject _bell;
        private GameObject _mask;

        private GameObject _mapLevel1;
        private GameObject _mapLevel2;

        private int _curLevelId;

        private MapDrag _levelOneMapDrag;
        private MapDrag _levelTwoMapDrag;

        private GameObject _restartBtn;
        private GameObject _success;
        private GameObject _successSpine;
        private bool _levelOneDraging;
        private bool _levelTwoDraging;
    

        void Start(object o)
        {
            _curGo = (GameObject)o;
            Transform curTrans = _curGo.transform;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            _bell = curTrans.Find("bell").gameObject;
            _mask = curTrans.GetGameObject("mask");
            _mapLevel1 = curTrans.GetGameObject("MapLevel1");
            _mapLevel2 = curTrans.GetGameObject("MapLevel2");

            _levelOneMapDrag = curTrans.Find("MapLevel1/Content/Player").GetComponent<MapDrag>();      
             _levelTwoMapDrag = curTrans.Find("MapLevel2/Content/Player").GetComponent<MapDrag>();    
            
            _restartBtn = curTrans.GetGameObject("RestartBtn");
            _success = curTrans.GetGameObject("Success");
            _successSpine = curTrans.GetGameObject("Success/Spine");

         
            GameInit();

        }
      

        void GameInit()
        {
            _levelOneDraging = false;
            _levelTwoDraging = false;
            _talkIndex = 1;

            _curLevelId = 1;

         

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            
            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            
            _bell.Hide(); _success.Hide();
            
            _levelOneMapDrag.SetMapDragCallBack(null, LevelOneDragEnd);
            _levelTwoMapDrag.SetMapDragCallBack(null, LevelTwoDragEnd);
     
            PointerClickListener.Get(_restartBtn).onClick = null;
            PointerClickListener.Get(_restartBtn).onClick = OnClickRestartBtn;

            GameStart();
        }

       

        void GameStart()
        {
           
            PlayBgm(0);
            ShowLevel(_curLevelId);
            BellSpecker(0, () => { _mask.Show(); }, () => { _mask.Hide();_restartBtn.Show(); });
        }


        void ShowLevel(int levelId)
        {
            switch (levelId)
            {
                case 1:
                  
                    _mapLevel1.Show();
                    _levelOneMapDrag.DoReset();
                    _mapLevel2.Hide();
                    
                    break;
                case 2:
                  
                    _mapLevel1.Hide();
                    _mapLevel2.Show();
                    _levelTwoMapDrag.DoReset();
                    break;
            }
        }

        void ShowSuccess(Action callBack=null)
        {
            PlayVoice(1);
            BellSpecker(1);
            _restartBtn.Hide(); _mask.Show(); _success.Show();


            SpineManager.instance.DoAnimation(_successSpine, "animation", false, () =>
            {
                SpineManager.instance.DoAnimation(_successSpine, "animation2", true);
            });

            Delay(4.0f, () => {
                _restartBtn.Show(); _mask.Hide(); _success.Hide();
                callBack?.Invoke();
            });
        }

        private void OnClickRestartBtn(GameObject go)
        {
            BtnPlaySound();
            switch (_curLevelId)
            {
                case 1:
                    _levelOneMapDrag.DoReset();
                    break;
                case 2:
                    _levelTwoMapDrag.DoReset();
                    break;
            }
        }


        private void LevelOneDraing(Vector3 v3)
        {

            if (!_levelOneDraging)
            {
                PlayVoice(0, true);
                _levelOneDraging = true;
            }
            
        }

        private void LevelOneDragEnd(Vector3 localPos, bool isMatch)
        {          
            if (isMatch)
            {
                _curLevelId = 2;
                ShowSuccess(() => { ShowLevel(_curLevelId); });
            }
        }


        private void LevelTwoDraing(Vector3 v3)
        {
            if (!_levelTwoDraging)
            {
                PlayVoice(0, true);
                _levelTwoDraging = true;
            }
            
        }

        private void LevelTwoDragEnd(Vector3 localPos, bool isMatch)
        {         
            if (isMatch)
            {
                ShowSuccess( ()=> { _restartBtn.Hide(); _mask.Show(); _success.Show(); });
            }
        }


        void BellSpecker(int index,Action specking=null,Action speckend=null,SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index,specking, speckend));
        }

        void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        IEnumerator IEDelay(float delay,Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }


        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(_bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (_talkIndex)
            {
                case 1:

                    break;
            }
            _talkIndex++;
        }


        private void PlayBgm(int index, bool isLoop=true)
        {
            Debug.LogError("111");
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, isLoop);
            Debug.LogError("222");
        }

        private void PlayVoice(int index,bool isLoop=false)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
        }

        private void StopPlayAudio(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
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
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }
    }
}
