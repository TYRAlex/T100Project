using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class TD6714Part4
    {

        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _bD;
        private GameObject _sP;

        void Start(object o)
        {

            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;


            _bD = curTrans.GetGameObject("BD");
            _sP = curTrans.GetGameObject("SP");

            GameInit();
        }



        private void GameInit()
        {
            _sP.Hide();
            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            GameStart();
        }

        private void GameStart()
        {
            PlayBgm(6, true, SoundManager.SoundType.COMMONBGM);
            BellSpeck(0);//, () => { _sP.Show(); PlaySpine(_sP, "kong", () => { PlaySpine(_sP, "sp"); }); });
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(_bD, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(_bD, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_bD, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        private void BellSpeck(int index, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, specking, speckend));
        }

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
        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

    }
}
