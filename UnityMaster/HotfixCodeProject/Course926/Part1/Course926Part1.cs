using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course926Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject bell;

        private GameObject _block;
        private Transform _zone;
        private Transform _bellHead;
        private GameObject _startBtn;
        private GameObject _endBtn;
        private GameObject _overTime;
        private Transform _endCount;
        private GameObject _countText;

        private GameObject _bg2;
        private GameObject _ani2;
        private GameObject _ani3;

        private bool _canClickStart;
        private bool _canClickEnd;
        private float _startTime;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;

            _block = curTrans.GetGameObject("Block");
            _zone = curTrans.Find("Zone");
            _bellHead = curTrans.Find("bellHead");
            _startBtn = curTrans.GetGameObject("StartBtn");
            _endBtn = curTrans.GetGameObject("EndBtn");
            _overTime = curTrans.GetGameObject("OverTime");
            _endCount = curTrans.Find("EndCount");
            _countText = _endCount.GetGameObject("Count");

            _bg2 = curTrans.GetGameObject("Bg2");
            _ani2 = curTrans.GetGameObject("ani2");
            _ani3 = curTrans.GetGameObject("ani3");

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameInit();
            GameStart();
        }


        private void GameInit()
        {
            talkIndex = 1;
            _canClickStart = false;
            _canClickEnd = false;
            
            _block.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_block, "daiji", true);
            SpineManager.instance.DoAnimation(_bellHead.gameObject, "kong", false);
            Util.AddBtnClick(_startBtn, StartClick);
            Util.AddBtnClick(_endBtn, EndClick);

            _endCount.gameObject.Hide();
            _overTime.Hide();
            _bg2.Hide();
            _ani2.Hide();
            _ani3.Hide();
        }

        void GameStart()
        {
            bell.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, 
            () => 
            {
                SpineManager.instance.DoAnimation(_block, "jing", false);
                bell.SetActive(false);
                _canClickStart = true;
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
                speaker = bell;
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

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                bell.SetActive(true);
                _bg2.Show();
                _ani2.Show();
                _ani2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, 
                () =>
                {
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); }, 0.2f));
                    SpineManager.instance.DoAnimation(_ani2, "animation", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_ani2, "animation2", false);
                    });
                }, 
                ()=> 
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2,
                () =>
                {
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); }, 0.5f));
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); }, 1.0f));
                    SpineManager.instance.DoAnimation(_ani2, "animation3", false, () => { SpineManager.instance.DoAnimation(_ani2, "animation4", false); });
                },
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            if (talkIndex == 3)
            {
                _ani2.Hide();
                _ani3.Show();
                _ani3.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3,
                () =>
                {
                    SpineManager.instance.DoAnimation(_ani3, "animation", false,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                        SpineManager.instance.DoAnimation(_ani3, "animation2", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                            SpineManager.instance.DoAnimation(_ani3, "animation3", false,
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                                SpineManager.instance.DoAnimation(_ani3, "animation4", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(_ani3, "animation", false);
                                });
                            });
                        });
                    });
                }, null));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void StartClick(GameObject obj)
        {
            if(_canClickStart)
            {
                _canClickStart = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                SoundManager.instance.ShowVoiceBtn(false);
                SpineManager.instance.DoAnimation(_block, "ks", false);
                SpineManager.instance.DoAnimation(_bellHead.gameObject, "kong", false);
                if (_endCount.gameObject.activeSelf)
                    _endCount.gameObject.Hide();

                int MaxX = (int)(_zone.localPosition.x + _zone.GetRectTransform().rect.width / 2 - _bellHead.GetRectTransform().rect.width * 3 / 4);
                int MinX = (int)(_zone.localPosition.x - _zone.GetRectTransform().rect.width / 2 + _bellHead.GetRectTransform().rect.width * 3 / 4);
                int MaxY = (int)(_zone.localPosition.y + _zone.GetRectTransform().rect.height / 2 - _bellHead.GetRectTransform().rect.height * 3 / 4);
                int MinY = (int)(_zone.localPosition.y - _zone.GetRectTransform().rect.height / 2 + _bellHead.GetRectTransform().rect.height * 3 / 4);
                int ranX = Random.Range(MinX, MaxX);
                int ranY = Random.Range(MinY, MaxY);

                int ranAni = Random.Range(1, 7);
                float ranTime = Random.Range(15, 25) * 0.1f;

                _bellHead.localPosition = new Vector2(ranX, ranY);
                mono.StartCoroutine(WaitCoroutine(() => 
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    _bellHead.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(_bellHead.gameObject, "bell" + ranAni.ToString(), true);
                    _startTime = Time.realtimeSinceStartup; 
                    _canClickEnd = true; 
                }, ranTime));

                mono.StartCoroutine(WaitCoroutine(() =>
                {

                    float len = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    mono.StartCoroutine(WaitCoroutine(() =>
                    {
                        SpineManager.instance.DoAnimation(_bellHead.gameObject, "kong", false);
                        _overTime.Show();
                        _canClickEnd = false;
                        _canClickStart = false;
                        mono.StartCoroutine(WaitCoroutine(() =>
                        {
                            _overTime.Hide();
                            _canClickStart = true;
                        }, 3.0f));
                    }, len - 0.5f));
                }, 30f));
            }
        }

        private void EndClick(GameObject obj)
        {
            if(_canClickEnd)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                mono.StopAllCoroutines();
                _canClickEnd = false;
                float timeLen = Time.realtimeSinceStartup - _startTime;

                SpineManager.instance.DoAnimation(_block, "js", false);
                _endCount.gameObject.Show();
                _countText.Show();
                _countText.GetComponent<Text>().text = timeLen.ToString("f3");
                _canClickStart = true;
                SoundManager.instance.ShowVoiceBtn(true);
                SpineManager.instance.DoAnimation(_bellHead.gameObject, "kong", false);
            }
        }
    }
}
