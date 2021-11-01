using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class TD6754Part4
    {
	
	           		
        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _dBD;		
		
      				
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;			
			           
            _dBD = curTrans.GetGameObject("dBD");
           
            GameInit();
            GameStart();
        }
    
        void GameInit()
        {                     
	                 			
            SoundManager.instance.ShowVoiceBtn(false);			
			
          			
            SoundManager.instance.StopAudio();
			_mono.StopAllCoroutines();
            _dBD.Show();
			           		   	    
        }

        void GameStart()
        {           
		    PlayCommonBgm(6);          		
			BellSpeck(_dBD, 0);					           
        }

		  
          
        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
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
	
        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null,SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null,float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

			daiJi = "bd-daiji"; speak = "bd-speak";
			        
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
    }
}
