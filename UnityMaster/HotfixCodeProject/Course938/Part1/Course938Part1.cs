using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course938Part1
    {

        public enum ClickEnum
        {
            Donghua,
            Donghua2,
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
        private int flag;

        private Transform _spinePanel;
        private GameObject[] _spinePanels;
        private Transform _clickPanel;
        private Empty4Raycast[] _e4rs;
        private ClickEnum clickEnum ;

        private Transform _enterClick;
        private Empty4Raycast[] _enterE4rs;

        private Transform _dragerPanel;
        private ILDrager[] _ilDragers;
        private ILDroper _ilDropers;

        private Transform _dragerPanelPos;
        private Transform[] _ilDragerStartPos;

        private bool isShowVoiceBtn;
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
            GameStart();
        }
        private void GameInit()
        {
            talkIndex = 1;
            FindInit();
        }
        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, true);

            _spinePanels[0].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spinePanels[0], "animation", false);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            { 
                Max.SetActive(false); isPlaying = false;
                _e4rs[0].gameObject.Show();
                _e4rs[1].gameObject.Show();
            }));

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
            _clickPanel.gameObject.Show();
            _e4rs = _clickPanel.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _e4rs.Length; i++)
            {
                Util.AddBtnClick(_e4rs[i].gameObject, ClickEvent);
                _e4rs[i].gameObject.Hide();
            }

            _enterClick = curTrans.Find("centerClick");
            _enterE4rs = _enterClick.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _enterE4rs.Length; i++)
            {
                Util.AddBtnClick(_enterE4rs[i].gameObject, CenterClickEvent);
                _enterE4rs[i].gameObject.Hide();
            }

            _dragerPanel = curTrans.Find("dragerPanel");
            _ilDragers = _dragerPanel.GetComponentsInChildren<ILDrager>(true);
           

            for (int i = 0; i < _ilDragers.Length; i++)
            {
                _ilDragers[i].index = i;
                _ilDragers[i].SetDragCallback(OnBeginDrag,OnDrag,OnEndDrag);
                _ilDragers[i].isActived = false;
            }
            _dragerPanelPos = curTrans.Find("dragerPanelPos");
            _ilDragerStartPos = _dragerPanelPos.GetComponentsInChildren<Transform>(true);
            for (int i = 1; i < _ilDragerStartPos.Length; i++)
            {
                _ilDragers[i-1].transform.position = _ilDragerStartPos[i].position;
            }

            _spinePanels[0].Show();
            _spinePanels[1].Hide();
            clickEnum = ClickEnum.Null;

            _enterClick.gameObject.Show();
            _dragerPanel.gameObject.Hide();
            flag = 0;
            isShowVoiceBtn = false;
        }       
        private void ClickEvent(GameObject go)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
            if (go.name == "0")
            {               
                PlaySpineAni(go,"d1", SoundManager.SoundType.VOICE, 1, "donghua", () => 
                {
                    _enterE4rs[0].gameObject.Show();
                    clickEnum = ClickEnum.Donghua;
                },()=> 
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    mono.StartCoroutine(WaiteCoroutine(() => 
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        mono.StartCoroutine(WaiteCoroutine(() => 
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                            mono.StartCoroutine(WaiteCoroutine(() => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false); },1.2f));
                        },0.8f));
                    }, 0.27f));
                },6.0f);
            }
            else if (go.name == "1")
            {
                PlaySpineAni(go,"d2", SoundManager.SoundType.VOICE, 2, "donghua2", () =>
                {
                    _enterE4rs[0].gameObject.Show();
                    clickEnum = ClickEnum.Donghua2;
                },()=> 
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    mono.StartCoroutine(WaiteCoroutine(() => 
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);                        
                    }, 3.0f));
                },2);

            }
            SoundManager.instance.ShowVoiceBtn(false);
            if ((flag & (1 << go.transform.GetSiblingIndex())) == 0)
            {
                flag += 1 << go.transform.GetSiblingIndex();
            }
            if (flag == (Mathf.Pow(2, _clickPanel.childCount) - 1))
            {
                isShowVoiceBtn = true;
            }
        }
        private void PlayAniClickTwo(string aniName,Action callBack=null)
        {
            _enterE4rs[0].gameObject.Hide();           
            SpineManager.instance.DoAnimation(_spinePanels[0], aniName, false, callBack);
        }
        private void CenterClickEvent(GameObject go)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
            if (clickEnum == ClickEnum.Donghua)
            {
                PlayAniClickTwo("t1", () =>
                {

                    _e4rs[0].gameObject.Show();
                    _e4rs[1].gameObject.Show();
                    if (isShowVoiceBtn)
                    {
                        isShowVoiceBtn = false;
                        SoundManager.instance.ShowVoiceBtn(true);
                    }                   
                });
            }
            else if (clickEnum == ClickEnum.Donghua2)
            {
                PlayAniClickTwo("t2", () =>
                {
                    _e4rs[0].gameObject.Show();
                    _e4rs[1].gameObject.Show();
                    if (isShowVoiceBtn)
                    {
                        isShowVoiceBtn = false;
                        SoundManager.instance.ShowVoiceBtn(true);
                    }                   
                });
            }            
        }       
        private void PlaySpineAni(GameObject go, string aniName, SoundManager.SoundType soundType,int soundIndex,string aniName_0,Action callBack=null,Action callBack_1=null,float delayTime =0)
        {
            _spinePanels[0].Show();
            _e4rs[0].gameObject.Hide();
            _e4rs[1].gameObject.Hide();
            SpineManager.instance.DoAnimation(_spinePanels[0], aniName, false, () =>
            {                            
                mono.StartCoroutine(SpeckerCoroutine(Max, soundType, soundIndex, null, callBack));
                SpineManager.instance.DoAnimation(_spinePanels[0], aniName_0,false);
                mono.StartCoroutine(WaiteCoroutine(callBack_1, delayTime));
            });
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
                SwithPartThree();
            }

            talkIndex++;
        }
        private void SwithPartThree()
        {
            _dragerPanel.gameObject.Show();
            _spinePanel.gameObject.Hide();
            _clickPanel.gameObject.Hide();
            _enterClick.gameObject.Hide();           

            Max.Show();
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, () => 
            {
                Max.Hide();
                for (int i = 0; i < _ilDragers.Length; i++)
                {
                    _ilDragers[i].isActived = true;
                }
            }));
        }
        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            _ilDragers[index].transform.SetAsLastSibling();
            _ilDragerStartPos[index + 1].SetAsLastSibling();
        }

        private void OnDrag(Vector3 pos, int type, int index){}

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            }
            else
            {
                _ilDragers[index].DoReset();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
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

        
    }
}
