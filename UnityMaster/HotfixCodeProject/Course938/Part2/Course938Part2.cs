using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course938Part2
    {   
        private enum ClickOneEnum
        {
            One,
            Second,
            Three,
            Four,
            Null
        }
        private enum ClickTwoEnum
        {
            One,
            Second,
            Three,
            Four,
            Null
        }
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private Transform _spinePanel;
        private GameObject[] _spinePanels;

        private Transform _clickPanel;
        private Empty4Raycast[] _e4rs;

        private ClickOneEnum clickOneEnum;
        private ClickTwoEnum clickTwoEnum;

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

            GameInit();
            FindInit();
            GameStart();
        }
        private void GameInit()
        {
            talkIndex = 1;
            clickOneEnum = ClickOneEnum.Null;
            clickTwoEnum = ClickTwoEnum.Null;
        }
        private void FindInit()
        {
            _spinePanel = curTrans.Find("spinePanel");
            _spinePanel.gameObject.Show();

            _spinePanels = new GameObject[_spinePanel.childCount];
            for (int i = 0; i < _spinePanels.Length; i++)
            {
                _spinePanels[i] = _spinePanel.GetChild(i).gameObject;
                _spinePanels[i].Hide();
            }

            _clickPanel = curTrans.Find("clickPanel");
            _e4rs = _clickPanel.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _e4rs.Length; i++)
            {
                Util.AddBtnClick(_e4rs[i].gameObject, ClickEvent);
                _e4rs[i].gameObject.Hide();
            }
        }
        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);

            _spinePanels[0].Show();
            _spinePanels[0].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spinePanels[0], "kong", false);
            SpineManager.instance.DoAnimation(_spinePanels[0], "fengmian", false);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            { 
                isPlaying = false;
                _e4rs[0].gameObject.Show();
            }));

        }
        private void ClickEvent(GameObject go)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            if (go.name == "0")
            {
                _e4rs[0].gameObject.Hide();
                mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false); }, 0.3f));
                SpineManager.instance.DoAnimation(_spinePanels[0], "dakai", false, () => 
                {
                    Max.SetActive(false);                   
                    PlayAni();
                });
            }
            else if (go.name == "1")
            {
                Max.SetActive(false);
                if (clickOneEnum == ClickOneEnum.One)
                {
                    _e4rs[1].gameObject.Hide();
                    _e4rs[2].gameObject.Hide();
                    _spinePanels[1].Hide();
                    _spinePanels[2].Hide();
                    SpineManager.instance.DoAnimation(_spinePanels[0], "guanbi", false,()=> { _e4rs[0].gameObject.Show(); });
                    //mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
                    //{                        
                    //    _e4rs[0].gameObject.Show();
                    //}));
                }
                else if (clickOneEnum == ClickOneEnum.Second)
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                    _e4rs[1].gameObject.Hide();
                    _spinePanels[3].Hide();
                    _spinePanels[4].Hide();
                    _spinePanels[5].Show();
                    _spinePanels[1].Show();
                    _spinePanels[2].Show();
                    _spinePanels[1].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    _spinePanels[2].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(_spinePanels[1], "1", false);
                    SpineManager.instance.DoAnimation(_spinePanels[2], "2", false);
                    SpineManager.instance.DoAnimation(_spinePanels[5], "4++", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(_spinePanels[6], "b1", false, () => 
                    {
                        _spinePanels[5].Hide();
                        _spinePanels[6].Hide();                                               
                        PlayAni();
                    });
                }
            }
            else if (go.name == "2")
            {
                SoundManager.instance.ShowVoiceBtn(false);
                _e4rs[1].gameObject.Hide();
                _e4rs[2].gameObject.Hide();
                if (clickTwoEnum == ClickTwoEnum.One)
                {                   
                    _spinePanels[3].Show();
                    _spinePanels[4].Show();
                    _spinePanels[3].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    _spinePanels[4].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(_spinePanels[3], "4", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(_spinePanels[4], "a2", false,()=> 
                    {
                        mono.StartCoroutine(WaiteCoroutine(() =>
                        {
                            _spinePanels[6].Show();
                            _spinePanels[6].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            SpineManager.instance.DoAnimation(_spinePanels[6], "4+", false);
                        }, 1.0f));
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () => 
                        {
                            SpineManager.instance.DoAnimation(_spinePanels[6], "4++", false);
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5, null, () =>
                            {
                                clickOneEnum = ClickOneEnum.Second;
                                _e4rs[1].gameObject.Show();                                
                                SoundManager.instance.ShowVoiceBtn(true);
                            }));
                        }));
                    });
                }               
            }
        }
        private void PlayAni()
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
            {
                _spinePanels[1].Show();
                _spinePanels[2].Show();
                _spinePanels[1].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                _spinePanels[2].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_spinePanels[1], "1", false);
                SpineManager.instance.DoAnimation(_spinePanels[2], "2", false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, ()=> { }));
                mono.StartCoroutine(WaiteCoroutine(() => 
                {
                    SpineManager.instance.DoAnimation(_spinePanels[1], "1", false);
                    SpineManager.instance.DoAnimation(_spinePanels[2], "2+", true);
                    mono.StartCoroutine(WaiteCoroutine(() => 
                    {
                        SpineManager.instance.DoAnimation(_spinePanels[1], "1+", false);
                        SpineManager.instance.DoAnimation(_spinePanels[2], "kong", false, () =>
                        {
                            SpineManager.instance.DoAnimation(_spinePanels[2], "2++", false, () =>
                            {
                                SpineManager.instance.DoAnimation(_spinePanels[2], "2+2", false); 
                            });
                        });
                        mono.StartCoroutine(WaiteCoroutine(() => 
                        {
                            SpineManager.instance.DoAnimation(_spinePanels[2], "2++2",false);
                        }, 5.3f));
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, () =>
                        {
                            _e4rs[1].gameObject.Show();
                            _e4rs[2].gameObject.Show();
                            clickOneEnum = ClickOneEnum.One;
                            clickTwoEnum = ClickTwoEnum.One;
                        }));
                    }, 8.0f));
                    //
                }, 4.3f));
                
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
               
            }
            _e4rs[1].gameObject.Hide();
            _e4rs[2].gameObject.Hide();
            Max.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6, null, () =>
            {
                _e4rs[1].gameObject.Show();
                clickOneEnum = ClickOneEnum.Second;
            }));

            talkIndex++;
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
