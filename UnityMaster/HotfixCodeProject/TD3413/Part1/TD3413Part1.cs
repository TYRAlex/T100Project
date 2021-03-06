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
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }

    public enum E_ClickAnimalType
    {
        None,
        Right,
        Wrong
    }

    public class TD3413Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject di;
        private GameObject dd;
        private GameObject tt;
        private GameObject bd;
        private GameObject dbd;

        private Dictionary<string, GameObject> _allAnimalDic;

        private GameObject caidaiSpine;
        private GameObject anyBtns;
       

        private GameObject successSpine;
        private GameObject mask;

        private Transform _animalsClickParent;

        private string[] _behindAnimalName;
        private string[] _frontAnimalName;

        private string[] _rightAnimalName;
        private string[] _wrongAnimalName;
        private List<string> _restOfTheRightAnimalList;

        private bool _isPlaying = false;
        private bool _isDoingTheAni = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            di = curTrans.Find("di").gameObject;
            dd = curTrans.Find("DD").gameObject;
            tt = curTrans.GetGameObject("TT");
            bd = curTrans.GetGameObject("BD");
            dbd = curTrans.GetGameObject("DBD");
            dbd.Hide();
            mask = curTrans.GetGameObject("mask");
            anyBtns = curTrans.Find("mask/Btn").gameObject;
            for (int i = 0; i < anyBtns.transform.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.transform.GetChild(i).gameObject, OnClickAnyBtn);
            }
            
            
           
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            caidaiSpine = curTrans.GetGameObject("mask/caidai ");
            successSpine.Hide();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            ShowTheBFButton();
            _isPlaying = false;
            _isDoingTheAni = false;
            mask.Show();
            InializedGameProperty();
           
            
        }

        void ShowTheBFButton()
        {
            anyBtns.Show();
            anyBtns.transform.GetChild(1).gameObject.Hide();
            anyBtns.transform.GetChild(0).name = getBtnName(BtnEnum.bf,0);
            
        }

        void InializedGameProperty()
        {
            
            
            _isDoingTheAni = false;
            LoadAnimalInfoList();
            LoadAllAnimals();
            dd.SetActive(false);
            di.SetActive(false);
            dbd.Hide();
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            
        }

        void LoadAnimalInfoList()
        {
            _behindAnimalName = new[] {"mogu","gou","baozi" };
            _frontAnimalName = new[] {"ma", "hudie", "chong", "tu", "wa", "wugui"};
            _rightAnimalName = new[] {"mogu", "baozi", "hudie", "gou", "wa"};
            _wrongAnimalName = new[] {"tu", "ma", "chong", "wugui"};
            _restOfTheRightAnimalList=new List<string>();
            for (int i = 0; i < _rightAnimalName.Length; i++)
            {
                _restOfTheRightAnimalList.Add(_rightAnimalName[i]);
            }
        }

        E_ClickAnimalType JudgeClickType(string targetName)
        {
            E_ClickAnimalType targetClickType = E_ClickAnimalType.None;
            if (FindTargetName(targetName, _wrongAnimalName))
            {
                targetClickType = E_ClickAnimalType.Wrong;
            }
            else if (FindTargetName(targetName, _rightAnimalName))
            {
                targetClickType = E_ClickAnimalType.Right;
            }

            return targetClickType;
        }

        bool  FindTargetName(string name,string[] target)
        {
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i] == name)
                {
                    return true;
                }
            }

            return false;
        }

        void LoadAllAnimals()
        {
            Transform animalsParent = curTrans.GetTransform("Animals");
            _allAnimalDic=new Dictionary<string, GameObject>();
            for (int i = 0; i < animalsParent.childCount; i++)
            {
                GameObject targetAnimal = animalsParent.GetChild(i).gameObject;
                if (targetAnimal.name!="MiddleBG")
                {
                   
                    _allAnimalDic.Add(targetAnimal.name, targetAnimal);
                    PlaySpineAni(targetAnimal, targetAnimal.name);
                   
                }
            }
            for (int i = 0; i < _behindAnimalName.Length; i++)
            {
                _allAnimalDic[_behindAnimalName[i]].transform.SetAsFirstSibling();
            }

            for (int i = 0; i < _frontAnimalName.Length; i++)
            {
                _allAnimalDic[_frontAnimalName[i]].transform.SetAsLastSibling();
            }

            _animalsClickParent = curTrans.GetTransform("AnimalsClick");
            for (int i = 0; i < _animalsClickParent.childCount; i++)
            {
                GameObject target = _animalsClickParent.GetChild(i).gameObject;
                target.Show();
                PointerClickListener.Get(target).onClick = OnClickEvent;
               
            }
        }

        void SetTargetClickFalse(string name)
        {
            for (int i = 0; i < _animalsClickParent.childCount; i++)
            {
                GameObject target = _animalsClickParent.GetChild(i).gameObject;
                if (target.name.Equals(name))
                {
                    target.Hide();
                }
            }
        }

        void OnClickEvent(GameObject o)
        {
            if(!_isPlaying||_isDoingTheAni)
                return;
            _isDoingTheAni = true;
            string targetName = o.name;
            

            GameObject targetGameObject = null;
            if (_allAnimalDic.TryGetValue(targetName, out targetGameObject))
            {
                if (FindTargetName(targetName, _behindAnimalName))
                {
                    targetGameObject.transform.SetAsLastSibling();
                }

                bool isRightAnimal = true;
                if (FindTargetName(targetName, _rightAnimalName))
                {
                    SetTargetClickFalse(targetName);
                    isRightAnimal = true;
                }
                else if (FindTargetName(targetName, _wrongAnimalName))
                {
                    isRightAnimal = false;
                }

                JudgeTargetAndPlayAni(targetName,targetGameObject,isRightAnimal);
            }
            else
            {
                Debug.LogError("?????????????????????????????????????????????" + targetName);
            }
        }

        void JudgeTargetAndPlayAni(string targetName, GameObject targetGameObject,bool isRightAnimal)
        {
            if (isRightAnimal)
            {
                BtnPlaySoundSuccess(() =>
                {
                    _isDoingTheAni = false;
                    JudgeNameAndDoNext(targetName,isRightAnimal);
                });
            }
            else
            {
                BtnPlaySoundFail(() =>
                {
                    _isDoingTheAni = false;
                    JudgeNameAndDoNext(targetName,isRightAnimal);
                });
            }
            PlaySpineAni(targetGameObject, targetName + "2",false, () =>
            {
               
            });
           
        }


        void JudgeNameAndDoNext(string targetName,bool isRightAnimal)
        {
            // E_ClickAnimalType target = JudgeClickType(targetName);
            // if (target == E_ClickAnimalType.Right)
            // {
            //     
            //     JudgeIfFinishGame(targetName);
            // }
            if (isRightAnimal)
            {
                JudgeIfFinishGame(targetName);
            }
        }

        void JudgeIfFinishGame(string name)
        {
            if (_restOfTheRightAnimalList.Contains(name))
                _restOfTheRightAnimalList.Remove(name);
            if (_restOfTheRightAnimalList.Count <= 0)
            {
                WinTheGame();
            }
        }

        void WinTheGame()
        {
            mask.Show();
            anyBtns.Hide();
            successSpine.Show();
            _isPlaying = false;
            playSuccessSpine();
            
        }
        
        /// <summary>
        /// ??????????????????
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, "3-5-z", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, "3-5-z2", false,
                        () =>
                        {
                            anyBtns.gameObject.SetActive(true);
                            GameObject fh = anyBtns.transform.GetChild(0).gameObject;
                            GameObject ok = anyBtns.transform.GetChild(1).gameObject;
                    
                            fh.Show();
                            ok.Show();
                            fh.name = getBtnName(BtnEnum.fh,0);
                            ok.name = getBtnName(BtnEnum.ok, 1);
                    
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }
        
        

        /// <summary>
        /// ????????????mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum,int index)
        {
            string result = string.Empty;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.transform.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if(_isPlaying)
                return;
            _isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () => {
                if (obj.name=="bf")
                {
                    Talk(tt, 0, null, () => SoundManager.instance.ShowVoiceBtn(true));
                    //mask.gameObject.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                   
                }
                else if (obj.name == "fh")
                {
                    mask.Hide();
                    // anyBtns.Show();
                    // anyBtns.name = getBtnName(BtnEnum.bf);
                    PlaySpineAni(anyBtns, "bf2");
                    InializedGameProperty();
                    _isPlaying = true;
                }
                else if(obj.name=="ok")
                {
                    anyBtns.Hide();
                    dbd.Show();
                    SoundManager.instance.Stop("bgm");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
                    mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2, null,() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 3, null,
                            () => _isPlaying = false));
                        // caidaiSpine.Show();
                        // SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
                    }));
                }
            
            });
        }

        private void GameInit()
        {
           
        }

        void GameStart()
        {
            _isPlaying = true;
            //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0));

        }
        //bell????????????
        IEnumerator SpeckerCoroutine(GameObject target, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            
            if (target == tt||target==dd)
            {
                SpineManager.instance.DoAnimation(target, "breath");
            }
            else if(target==bd||target==dbd)
            {
                SpineManager.instance.DoAnimation(target, "bd-daiji");
            }

            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            if (target == tt||target==dd)
            {
                SpineManager.instance.DoAnimation(target, "talk");
            }
            else if(target==bd||target==dbd)
            {
                SpineManager.instance.DoAnimation(target, "bd-speak");
            }
            

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
           if (target == tt||target==dd)
            {
                SpineManager.instance.DoAnimation(target, "breath");
            }
            else if(target==bd||target==dbd)
            {
                SpineManager.instance.DoAnimation(target, "bd-daiji");
            }
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    tt.Hide();
                    Talk(bd, 1, null, () =>
                    {
                        mask.Hide();
                        bd.Hide();
                        GameStart();
                    });
                    
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
            talkIndex++;
        }
        

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //??????????????????
        private void BtnPlaySoundFail(Action callback=null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
            mono.StartCoroutine(WaitTimeAndExcuteNext(timer, callback));
        }
        //??????????????????
        private void BtnPlaySoundSuccess(Action callback=null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
            mono.StartCoroutine(WaitTimeAndExcuteNext(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNext(float timer,Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }

        /// <summary>
        /// ??????Spine??????
        /// </summary>
        /// <param name="target">????????????</param>
        /// <param name="name">????????????</param>
        /// <param name="isLoop">????????????</param>
        /// <param name="callback">??????????????????</param>
        private void PlaySpineAni(GameObject target,string name,bool isLoop=false,Action callback=null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }

        /// <summary>
        /// Bell??????
        /// </summary>
        /// <param name="index">Voice????????????</param>
        /// <param name="goingEvent">?????????????????????</param>
        /// <param name="finishEvent">????????????</param>
        private void Talk(GameObject target,int index,Action goingEvent=null,Action finishEvent=null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target, SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
        }

        /// <summary>
        /// ??????BGM?????????????????????BGM????????????
        /// </summary>
        private void PlayBGM()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }

        /// <summary>
        /// ???????????????Sound??????
        /// </summary>
        /// <param name="targetIndex">????????????</param>
        private void PlaySound(int targetIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, targetIndex);
        }


    }
}
