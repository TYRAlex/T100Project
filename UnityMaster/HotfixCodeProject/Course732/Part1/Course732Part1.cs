using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course732Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject _car;

        private int _clickFrontWheelIndex;
        private int _clickRearWheelIndex;
        private Transform _clickBtn;
        private PolygonCollider2D[] _clickBtnPo;


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

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, true);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            _clickFrontWheelIndex = 0;
            _clickRearWheelIndex = 0;

            _car = curTrans.Find("spineManager/car").gameObject;
            _car.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_car, "jing1", false);

            _clickBtn = curTrans.Find("clickBtn");
            _clickBtnPo = _clickBtn.GetComponentsInChildren<PolygonCollider2D>(true);

            for (int i = 0; i < _clickBtnPo.Length; i++)
            {
                if (i < 2)
                {
                    Util.AddBtnClick(_clickBtnPo[i].gameObject, FrontWheelClickEvent);
                }
                else
                {
                    Util.AddBtnClick(_clickBtnPo[i].gameObject, RearWheelClickEvent);
                }
                _clickBtnPo[i].gameObject.Hide();
            }
        }
        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                isPlaying = false;
                SoundManager.instance.ShowVoiceBtn(true);
            }));

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
        IEnumerator WaiteCoroutine(Action method_2 = null, float len = 0)
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
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    ShowClickBtn(_clickBtnPo, true);                    
                }));
                SpineManager.instance.DoAnimation(_car, "touming", false, () =>
                {
                    SpineManager.instance.DoAnimation(_car, "jing2", false, () => { /*ShowClickBtn(_clickBtnPo, true);*/ });
                });
            }
            else if (talkIndex == 2)
            {
                ShowClickBtn(_clickBtnPo, false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, ()=> 
                {                    
                }));
            }

            talkIndex++;
        }
        /// <summary>
        /// 前轮点击
        /// </summary>
        /// <param name="obj"></param>
        private void FrontWheelClickEvent(GameObject obj)
        {
            SoundManager.instance.ShowVoiceBtn(false);
            ShowClickBtn(_clickBtnPo, false);        
            SpineManager.instance.SetTimeScale(_car, 0.75f);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2, false);
            SpineManager.instance.DoAnimation(_car, "ql1", false, () =>
            {
                SpineManager.instance.DoAnimation(_car, "ql2", false, () =>
                {
                    SpineManager.instance.DoAnimation(_car, "ql3", false, () =>
                    {
                        SpineManager.instance.DoAnimation(_car, "ql4", true);
                        mono.StartCoroutine(WaiteCoroutine(() => 
                        {
                            SpineManager.instance.SetTimeScale(_car, 1);
                            SpineManager.instance.DoAnimation(_car, "ql5", false);
                        }, 5.5f));
                    });
                });
            });                 
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, ()=> 
            {
                SpineManager.instance.DoAnimation(_car, "jing2", false);
                if (_clickRearWheelIndex >= 1 && _clickFrontWheelIndex >= 1)
                {
                    //_clickFrontWheelIndex = 1;
                    SoundManager.instance.ShowVoiceBtn(true);
                }
                ShowClickBtn(_clickBtnPo, true);
            }));
            _clickFrontWheelIndex++;

        }
        /// <summary>
        /// 后轮点击
        /// </summary>
        /// <param name="obj"></param>
        private void RearWheelClickEvent(GameObject obj)
        {
            SoundManager.instance.ShowVoiceBtn(false);
            ShowClickBtn(_clickBtnPo, false);           
            SpineManager.instance.SetTimeScale(_car, 0.75f);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
            SpineManager.instance.DoAnimation(_car, "hl1", false, () =>
            {
                SpineManager.instance.SetTimeScale(_car, 0.8f);
                SpineManager.instance.DoAnimation(_car, "hl2", false, () =>
                {
                    SpineManager.instance.SetTimeScale(_car, 1);
                    SpineManager.instance.DoAnimation(_car, "hl3", false, () =>
                    {
                        SpineManager.instance.DoAnimation(_car, "hl4", true);
                    });
                });
            });

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, ()=> 
            {
                SpineManager.instance.DoAnimation(_car, "jing2", false);
                if (_clickRearWheelIndex >= 1 && _clickFrontWheelIndex >= 1)
                {
                    //_clickRearWheelIndex = 1;
                    SoundManager.instance.ShowVoiceBtn(true);
                }
                ShowClickBtn(_clickBtnPo, true);
            }));
            _clickRearWheelIndex++;
        }
        private void ShowClickBtn(PolygonCollider2D[] _clickBtnPo, bool isShow)
        {
            for (int i = 0; i < _clickBtnPo.Length; i++)
            {
                _clickBtnPo[i].gameObject.SetActive(isShow);
            }
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }
    }
}
