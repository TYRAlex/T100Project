using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course9311Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private GameObject Bg2;
        private BellSprites bellTextures;

        private GameObject max;

        private GameObject _ani;       
        private Transform _click;
        private Empty4Raycast[] _clickRay;
        private GameObject _back;

        private bool _canClick;
        private int _flag;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            Bg2 = curTrans.Find("Bg2").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            max = curTrans.Find("Panel/Max").gameObject;

            _ani = curTrans.GetGameObject("Panel/ani");           
            _click = curTrans.Find("Panel/Click");
            _clickRay = _click.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _clickRay.Length; i++)
            {
                Util.AddBtnClick(_clickRay[i].gameObject, ClickEvent);
            }
            _back = curTrans.GetGameObject("btnBack");
            Util.AddBtnClick(_back, BackEvent);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            _flag = 0;
            _canClick = false;

            _back.Hide();
            Bg2.Hide();

            _ani.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_ani, "dianji", false);

        }

        void GameStart()
        {
            max.SetActive(true);           
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 0, null, () => { max.SetActive(false); _canClick = true; }));
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
                speaker = max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        IEnumerator WariteCoroutine( Action method_2 = null, float len = 0)
        {
           
            yield return new WaitForSeconds(len);
            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                _canClick = false;
                max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 4, null, () => { max.SetActive(false); }));
            }

            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void BackEvent(GameObject obj)
        {
            obj.Hide();            
            mono.StartCoroutine(WariteCoroutine(() => { Bg2.Hide(); }, 0.2f));           
            SpineManager.instance.DoAnimation(_ani, "d4", false,
            () =>
            {
                _ani.Show();
                SpineManager.instance.DoAnimation(_ani, "dianji", false, () =>
                 {                   
                     _canClick = true;
                     if (_flag == (Mathf.Pow(2, _click.childCount) - 1))
                         SoundManager.instance.ShowVoiceBtn(true);
                 });
            });
        }

        private void ClickEvent(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                SoundManager.instance.ShowVoiceBtn(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                SpineManager.instance.DoAnimation(_ani, "d" + obj.name, false, 
                () => 
                {
                    Bg2.Show();
                    mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, int.Parse(obj.name), null, () => { _back.Show(); }, 0));
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, int.Parse(obj.name), false);
                    SpineManager.instance.DoAnimation(_ani, "kong", false);
                    SpineManager.instance.DoAnimation(_ani, obj.name + "d", false, 
                    () => 
                    {
                        SpineManager.instance.DoAnimation(_ani, "saomiao" + obj.name, false, 
                        () => 
                        {
                            if((_flag & (1 << (int.Parse(obj.name) - 1))) == 0)
                            {
                                _flag += 1 << (int.Parse(obj.name) - 1);
                            }
                        });
                    });
                });
            }
        }
    }
}
