using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course9110Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        //private GameObject Bg;

        private List<ILDrager> _gearList;
        private ILDroper[] _gearDropperArray;
        private Dictionary<string, GameObject> _gearSwitchDic;
        
        private const string EightGear = "Eight";
        private const string TwentyGear = "Twenty";
        private const string TwelveGear1 = "Twelve1";
        private const string TwelveGear2 = "Twelve2";

        private GameObject _mainTarget;
        // private Transform _mainTargetOriginalPosTransform;
        // private Transform _mainTragetUpPosTransform;
        private Transform _frame1BG;
        private Transform _frame2BG;
        private GameObject _gearPanel;
        private GameObject _frame1Text;

        private Vector3 _mainTargetOriginalLocalPos;
        private Vector3 _mainTargetMiddleLocalPos;
        private string _currentTargetName = null;
        
        private Transform _frame1EndPos;
        private Transform _frame2EndPos;

        private Transform _frame1StartPos;
        private Transform _frame2StartPos;
        
        //private AudioSource 

        void Start(object o)
        {
            curGo = (GameObject) o;
            curTrans = curGo.transform;
            ResetFrameProperty(curTrans);
            
            //用于测试课程环节的切换，测试完成注意隐藏
            GameObjectInializeFrame1(_frame1BG);
            //GameObjectInializeFrame2(_frame2BG);
            ReStart();
        }

        void ResetFrameProperty(Transform curTrans)
        {
            _frame1BG = curTrans.GetTransform("Frame1BG");
            _frame2BG = curTrans.GetTransform("Frame2BG");
            _frame1EndPos = curTrans.GetTransform("Frame1EndPos");
            _frame2EndPos = curTrans.GetTransform("Frame2EndPos");
            _frame1StartPos = curTrans.GetTransform("Frame1StartPos");
            _frame2StartPos = curTrans.GetTransform("Frame2StartPos");
            _frame1BG.position = _frame1StartPos.position;
            _frame1BG.localScale=Vector3.one;
            _frame2BG.position = _frame2StartPos.position;
            _frame2BG.localScale=Vector3.one;
            _frame1Text = _frame1BG.GetGameObject("Frame1Text");
            _frame1Text.Hide();
            _frame2BG.gameObject.Hide();
        }

        void GameObjectInializeFrame1(Transform curTrans)
        {
            SoundManager.instance.SetShield(true);
            ResetBGM();
            RecordStartPos();
            _mainTarget = curTrans.GetGameObject("Target");
           
            SpineManager.instance.DoAnimation(_mainTarget, "animation", false);
            Debug.Log(curTrans.GetTransform("MainTargetMiddlePos").name);
            _mainTargetMiddleLocalPos = curTrans.GetTransform("MainTargetMiddlePos").localPosition;
            _mainTargetOriginalLocalPos = curTrans.GetTransform("MainTargetOriginalPos").localPosition;

            _mainTarget.transform.localPosition = _mainTargetMiddleLocalPos;
            _gearList = new List<ILDrager>();
            _gearSwitchDic = new Dictionary<string, GameObject>();
            _gearPanel = curTrans.GetGameObject("GearPanel");
            Transform gearParent = _gearPanel.transform.GetTransform("Gear");
            for (int i = 0; i < gearParent.childCount; i++)
            {
                ILDrager target = gearParent.GetChild(i).GetComponent<ILDrager>();
                _gearList.Add(target);
                target.SetDragCallback(null, null, DragGearEnd);
                target.DoReset();
            }
            _gearPanel.Hide();
            Transform gearSwitchParent = _mainTarget.transform.GetTransform("GearSwitch");
            for (int i = 0; i < gearSwitchParent.childCount; i++)
            {
                GameObject target = gearSwitchParent.GetChild(i).gameObject;

                _gearSwitchDic.Add(target.name, target);
                SpineManager.instance.DoAnimation(target, "k" + (i + 1), false);
                //target.Hide();
                target.Show();
            }

            Transform gearRightPosParent = curTrans.GetTransform("GearRightPos");
            _gearDropperArray = gearRightPosParent.GetComponentsInChildren<ILDroper>();
            for (int i = 0; i < _gearDropperArray.Length; i++)
            {
                _gearDropperArray[i].SetDropCallBack(DropCallBack);
            }
            
            //TurnToTheBothSide();
        }
        
        void ResetBGM()
        {
            SoundManager.instance.StopAudio();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }

        void ReStart()
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            //Bg = curTrans.Find("Bg").gameObject;
            bell.transform.SetAsLastSibling();

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
           
        }

        void GameStart()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () =>
            {
                bell.transform.SetAsFirstSibling();
                MainTargetMovetoTheMidleOrUp(false);
                // _mainTarget.transform.DOLocalMove(_mainTargetOriginalLocalPos, 2f).OnComplete(() =>
                // {
                //     SetAllGearSwitchVisible(true);
                //     _gearPanel.Show();
                // });
            }));
            //TurnToTheBothSide();

        }

        #region DragPart

        /// <summary>
        /// 全部齿轮
        /// </summary>
        /// <param name="index"></param>
        void AllFinishedAndDoTheNext()
        {
            SetAllGearSwitchVisible(false);
            SpineManager.instance.DoAnimation(_mainTarget, "animation3");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
            SoundManager.instance.ShowVoiceBtn(true);
        }

        void GetAndExcuteTargetAnimation(int index)
        {
            //Debug.Log("Index:" + index);
            _gearList[index].gameObject.Hide();
            //Debug.Log(GetTargetGearByIndex(index).name);
            GameObject gearSwith = GetTargetGearSwith(_currentTargetName);
            string animationName = SpineManager.instance.GetCurrentAnimationName(gearSwith);
            string switchAnimationName = animationName.Replace("k", "c");
            SpineManager.instance.DoAnimation(gearSwith, switchAnimationName, false);
            _currentTargetName = null;
        }

        private bool DropCallBack(int dragType, int index, int drogType)
        {
            if (dragType == drogType)
            {
                //Debug.Log("Drop Index:" + index);
                _currentTargetName = FindNameInDropperArray(index);
                return true;
            }

            return false;
        }

        void DragGearEnd(Vector3 postion, int type, int index, bool isMath)
        {
            if (isMath)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                GetAndExcuteTargetAnimation(index);
                if (CheckIfIsAllGearSet())
                {
                    AllFinishedAndDoTheNext();
                }
            }
            else
            {
                _gearList[index].DoReset();
            }
        }

        
        bool CheckIfIsAllGearSet()
        {
            for (int i = 0; i < _gearList.Count; i++)
            {
                if (_gearList[i].gameObject.activeSelf)
                {
                    return false;
                }
            }

            return true;
        }

        void MainTargetMovetoTheMidleOrUp(bool isMiddle)
        {
            if (!isMiddle)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                _mainTarget.transform.DOLocalMove(_mainTargetOriginalLocalPos, 2f).OnComplete(() =>
                {
                    SetAllGearSwitchVisible(true);
                    _gearPanel.Show();
                });
            }
            else
            {
                SetAllGearSwitchVisible(false);
                _mainTarget.transform.DOLocalMove(_mainTargetMiddleLocalPos, 2f);
                _gearPanel.Hide();
            }
      
            
        }

        void SetAllGearSwitchVisible(bool isShow)
        {
            int i = 0;
            foreach (GameObject go in _gearSwitchDic.Values)
            {
                go.SetActive(isShow);
                if (isShow)
                {
                    i++;
                    SpineManager.instance.DoAnimation(go, "k" + i, false);
                }
            }
            
        }

        ILDrager GetTargetGear(string name)
        {
            ILDrager target = null;
            if (_gearList.Count > 0 )
            {
                target = _gearList.Find(p => p.name == name);
            }

            return target;
        }

        ILDrager GetTargetGearByIndex(int index)
        {
            ILDrager target = null;
            if (_gearList.Count > 0 )
            {
                target = _gearList[index];
            }

            return target;
        }

        GameObject GetTargetGearSwith(string name)
        {
            GameObject target = null;
            if (_gearSwitchDic.TryGetValue(name, out target))
                return target;
            else
            {
                Debug.LogError("找不到对应名字的目标动画，请检查！"+name);
                return null;
            }


        }

        string FindNameInDropperArray(int index)
        {
            string targetName = null;
            if (index < _gearDropperArray.Length)
            {
                targetName = _gearDropperArray[index].name;
            }

            return targetName;
        }
        

        #endregion

        #region MoveFramePart

        private Vector3 _frame1OriginalPos ;
        private Vector3 _frame2OriginalPos;

        void InializedGameProperty()
        {
            _frame1BG.gameObject.Show();
            _frame2BG.gameObject.Hide();
        }

        void RecordStartPos()
        {
            _frame1OriginalPos = _frame1BG.localPosition;
            _frame2OriginalPos = _frame2BG.localPosition;
        }

        void BackToMiddle(Transform frameTrans)
        {
            frameTrans.gameObject.Show();
            frameTrans.transform.localScale=Vector3.one;
            if (frameTrans.name == "Frame1BG")
            {
                _frame1BG.localPosition = _frame1OriginalPos;
                _frame2BG.gameObject.Hide();
            }
            else if(frameTrans.name=="Frame2BG")
            {
                _frame2BG.localPosition = _frame2OriginalPos;
                _frame1BG.gameObject.Hide();
            }
        }

        
        void TurnToTheBothSide()
        {
            MoveFrameAndChangeScale(_frame1BG, _frame1EndPos.localPosition, new Vector3(0.6f, 0.6f, 1f));
            MoveFrameAndChangeScale(_frame2BG, _frame2EndPos.localPosition, new Vector3(0.6f, 0.6f, 1f));
        }

        void MoveFrameAndChangeScale(Transform frameTrans,Vector3 endPos,Vector3 endScale)
        {

            frameTrans.gameObject.Show();
            frameTrans.DOLocalMove(endPos, 2f);
            frameTrans.DOScale(endScale, 2f);
        }


        #endregion

        #region BellFunc

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
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
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
            switch (talkIndex)
            {
                case 1:
                    //语音：恭喜你们，完整的地月模型已经组装好了，接下来请计算出该模型中地球和月球自转和公转的速度传动比吧
                   
                    SoundManager.instance.Stop("sound");
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () =>
                    {
                        //BackToMiddle(_frame2BG);
                        
                        MainTargetMovetoTheMidleOrUp(true);
                        bell.transform.SetAsLastSibling();
                    }, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
                       
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
                    
                    
                    break;
                case 2:
                    //模型中地球和月球自转和公转的速度传动比是1:1，你们答对了吗？
                    
                    
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2,
                        () => SoundManager.instance.Stop("sound"),
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
                            SoundManager.instance.ShowVoiceBtn(true);
                        }));
                    break;
                case 3:
                    //语音：实际上，我们地球自转是约是1天；月球的自转和公转是一样的，约是30天，所以地球与月球自转和公转的速度传动比约为1:30；
                   
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () =>
                        {
                            SoundManager.instance.Stop("sound");
                            _frame1Text.Show();
                            TurnToTheBothSide();
                        },
                        () => SoundManager.instance.ShowVoiceBtn(true)));
                    break;
                case 4:
                    //语音：由于器材影响，无法完美模拟出真实的地月1:30的速度比，所以请你们利用积木按照画面中地月模型1:1的传动比搭建出来吧
                   

                    mono.StartCoroutine(
                        SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () =>
                            {
                                _frame1Text.Hide();
                                BackToMiddle(_frame1BG);
                            },
                            () => SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true)));
                    break;
            }

            talkIndex++;

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
        

        #endregion
        
    }
}
