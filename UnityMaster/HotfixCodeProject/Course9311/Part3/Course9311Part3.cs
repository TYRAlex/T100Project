using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course9311Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private GameObject _enterGame;
        private Transform _final;
        private Transform _level1;
        private Transform _level2;
        private Transform _level3;

        private mILDrager[] _drager1;
        private mILDrager[] _drager2;
        private mILDrager[] _drager3;

        private GameObject[] _dragerObj1;
        private GameObject[] _dragerObj2;
        private GameObject[] _dragerObj3;

        private Dictionary<int, float> _price1;
        private Dictionary<int, float> _price2;
        private Dictionary<int, float> _price3;

        private GameObject _sureClick;
        private GameObject _mask;
        private GameObject _success;
        private GameObject _tanqi;

        private int _level;
        private bool _canClick;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("Max").gameObject;
            _enterGame = curTrans.Find("EnterGame").gameObject;
            Util.AddBtnClick(_enterGame, EnterGame);

            _final = curTrans.Find("Final");

            _level1 = _final.Find("1");
            _level2 = _final.Find("2");
            _level3 = _final.Find("3");

            _drager1 = new mILDrager[_level1.Find("Obj").childCount + _level1.Find("EndPos").childCount];
            _dragerObj1 = new GameObject[_level1.Find("Obj").childCount + _level1.Find("EndPos").childCount];
            for (int i = 0; i < _level1.Find("Obj").childCount; i++)
            {
                int index = int.Parse(_level1.Find("Obj").GetChild(i).gameObject.name);
                _dragerObj1[index] = _level1.Find("Obj").GetGameObject(index.ToString());
                _drager1[index] = _dragerObj1[index].GetComponent<mILDrager>();
            }
            for (int i = 0; i < _level1.Find("EndPos").childCount; i++)
            {
                int index = int.Parse(_level1.Find("EndPos").GetChild(i).gameObject.name);
                _dragerObj1[index] = _level1.Find("EndPos").GetGameObject(index.ToString());
                _drager1[index] = _dragerObj1[index].GetComponent<mILDrager>();
            }

            _drager2 = new mILDrager[_level2.Find("Obj").childCount + _level2.Find("EndPos").childCount];
            _dragerObj2 = new GameObject[_level2.Find("Obj").childCount + _level2.Find("EndPos").childCount];
            for (int i = 0; i < _level2.Find("Obj").childCount; i++)
            {
                int index = int.Parse(_level2.Find("Obj").GetChild(i).gameObject.name);
                _dragerObj2[index] = _level2.Find("Obj").GetGameObject(index.ToString());
                _drager2[index] = _dragerObj2[index].GetComponent<mILDrager>();
            }
            for (int i = 0; i < _level2.Find("EndPos").childCount; i++)
            {
                int index = int.Parse(_level2.Find("EndPos").GetChild(i).gameObject.name);
                _dragerObj2[index] = _level2.Find("EndPos").GetGameObject(index.ToString());
                _drager2[index] = _dragerObj2[index].GetComponent<mILDrager>();
            }

            _drager3 = new mILDrager[_level3.Find("Obj").childCount + _level3.Find("EndPos").childCount];
            _dragerObj3 = new GameObject[_level3.Find("Obj").childCount + _level3.Find("EndPos").childCount];
            for (int i = 0; i < _level3.Find("Obj").childCount; i++)
            {
                int index = int.Parse(_level3.Find("Obj").GetChild(i).gameObject.name);
                _dragerObj3[index] = _level3.Find("Obj").GetGameObject(index.ToString());
                _drager3[index] = _dragerObj3[index].GetComponent<mILDrager>();
            }
            for (int i = 0; i < _level3.Find("EndPos").childCount; i++)
            {
                int index = int.Parse(_level3.Find("EndPos").GetChild(i).gameObject.name);
                _dragerObj3[index] = _level3.Find("EndPos").GetGameObject(index.ToString());
                _drager3[index] = _dragerObj3[index].GetComponent<mILDrager>();
            }

            for (int i = 0; i < _dragerObj1.Length; i++)
            {
                _dragerObj1[i].GetComponent<Image>().SetNativeSize();
                _dragerObj1[i].GetComponent<Image>().eventAlphaThreshold = 0.5f;
            }
            for (int i = 0; i < _dragerObj2.Length; i++)
            {
                _dragerObj2[i].GetComponent<Image>().SetNativeSize();
                _dragerObj2[i].GetComponent<Image>().eventAlphaThreshold = 0.5f;
            }
            for (int i = 0; i < _dragerObj3.Length; i++)
            {
                _dragerObj3[i].GetComponent<Image>().SetNativeSize();
                _dragerObj3[i].GetComponent<Image>().eventAlphaThreshold = 0.5f;
            }

            _sureClick = _final.GetGameObject("sure");
            Util.AddBtnClick(_sureClick, SureClick);
            _mask = curTrans.GetGameObject("mask");
            _success = curTrans.GetGameObject("mask/success");
            _tanqi = curTrans.GetGameObject("mask/tanqi");

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            InitPrice();
            GameInit();
            GameStart();
        }

        void InitPrice()
        {
            _price1 = new Dictionary<int, float>();
            _price1.Add(0, 2.5f);
            _price1.Add(1, 2.5f);
            _price1.Add(2, 2.5f);
            _price1.Add(3, 7.5f);
            _price1.Add(4, 5.0f);
            _price1.Add(5, 2.5f);
            _price1.Add(6, 2.5f);
            _price1.Add(7, 2.5f);

            _price2 = new Dictionary<int, float>();
            _price2.Add(0, 3.5f);
            _price2.Add(1, 3.5f);
            _price2.Add(2, 3.5f);
            _price2.Add(3, 5.5f);
            _price2.Add(4, 5.5f);
            _price2.Add(5, 5.5f);
            _price2.Add(6, 6.0f);
            _price2.Add(7, 6.0f);
            _price2.Add(8, 6.0f);

            _price3 = new Dictionary<int, float>();
            _price3.Add(0, 10.7f);
            _price3.Add(1, 10.15f);
            _price3.Add(2, 2.7f);
            _price3.Add(3, 7.3f);
            _price3.Add(4, 20.85f);
            _price3.Add(5, 6.3f);
        }

        private void GameInit()
        {
            talkIndex = 1;
            _level = 0;
            _canClick = true;

            _mask.Hide();
            _enterGame.Hide();
            _final.gameObject.Hide();

            InitPos();
            InitDrag();
        }

        void GameStart()
        {
            Max.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { _enterGame.Show(); }));
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

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }

        void Wait(Action method_1 = null, float len = 0)
        {
            mono.StartCoroutine(WaitCoroutine(method_1, len));
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }
            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        void InitPos()
        {
            for (int i = 0; i < _dragerObj1.Length; i++)
            {
                _dragerObj1[i].transform.parent = _level1.Find("Obj");
                _dragerObj1[i].transform.position = _level1.Find("Pos").Find(_dragerObj1[i].name).position;
            }

            for (int i = 0; i < _dragerObj2.Length; i++)
            {
                _dragerObj2[i].transform.parent = _level2.Find("Obj");
                _dragerObj2[i].transform.position = _level2.Find("Pos").Find(_dragerObj2[i].name).position;
            }

            for (int i = 0; i < _dragerObj3.Length; i++)
            {
                _dragerObj3[i].transform.parent = _level3.Find("Obj");
                _dragerObj3[i].transform.position = _level3.Find("Pos").Find(_dragerObj3[i].name).position;
            }
        }

        private void EnterGame(GameObject obj)
        {
            obj.Hide();
            _final.gameObject.Show();

            UpdateLevel();
        }

        private void SureClick(GameObject obj)
        {
            if(_canClick)
            {
                DontDrag();
                _canClick = false;
                float trueTotal = _level == 1 ? 20 : (_level == 2 ? 30 : 40);
                float total = 0;
                if(_level == 1)
                {
                    for (int i = 0; i < _level1.Find("EndPos").childCount; i++)
                    {
                        total += _price1[int.Parse(_level1.Find("EndPos").GetChild(i).name)];
                    }
                }
                if (_level == 2)
                {
                    for (int i = 0; i < _level2.Find("EndPos").childCount; i++)
                    {
                        total += _price2[int.Parse(_level2.Find("EndPos").GetChild(i).name)];
                    }
                }
                if (_level == 3)
                {
                    for (int i = 0; i < _level3.Find("EndPos").childCount; i++)
                    {
                        total += _price3[int.Parse(_level3.Find("EndPos").GetChild(i).name)];
                    }
                }

                Debug.Log("正确的金额：" + trueTotal);
                Debug.Log("现在的金额：" + total);

                if (total == trueTotal)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, null));

                    _mask.Show();
                    _tanqi.Hide();
                    _success.Show();
                    _success.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(_success, "animation", false, 
                    ()=> 
                    {
                        SpineManager.instance.DoAnimation(_success, "animation2", true);
                    });
                    Wait(
                    () => 
                    { 
                        if(_level != 3)
                        {
                            _mask.Hide();
                            UpdateLevel();
                        }
                    }, 3.0f);
                }
                else
                {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, 
                    ()=> 
                    {
                        _mask.Hide();
                        _canClick = true;
                        CanDrag();
                    }));

                    _mask.Show();
                    _tanqi.Show();
                    _success.Hide();
                    _tanqi.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(_tanqi, "animation", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                }
            }
        }

        private void UpdateLevel()
        {
            _level++;

            _level1.gameObject.Hide();
            _level2.gameObject.Hide();
            _level3.gameObject.Hide();

            if (_level == 1)
                _level1.gameObject.Show();
            if (_level == 2)
                _level2.gameObject.Show();
            if (_level == 3)
                _level3.gameObject.Show();

            _canClick = true;
            CanDrag();
        }

        #region 拖拽

        void InitDrag()
        {
            for (int i = 0; i < _drager1.Length; i++)
            {
                _drager1[i].SetDragCallback(StartDrag, Draging, EndDrag, null);
                _drager1[i].canMove = true;
            }
            for (int i = 0; i < _drager2.Length; i++)
            {
                _drager2[i].SetDragCallback(StartDrag, Draging, EndDrag, null);
                _drager2[i].canMove = true;
            }
            for (int i = 0; i < _drager3.Length; i++)
            {
                _drager3[i].SetDragCallback(StartDrag, Draging, EndDrag, null);
                _drager3[i].canMove = true;
            }
        }

        void CanDrag()
        {
            for (int i = 0; i < _drager1.Length; i++)
            {
                _drager1[i].transform.gameObject.GetComponent<Image>().raycastTarget = true;
            }
            for (int i = 0; i < _drager2.Length; i++)
            {
                _drager2[i].transform.gameObject.GetComponent<Image>().raycastTarget = true;
            }
            for (int i = 0; i < _drager3.Length; i++)
            {
                _drager3[i].transform.gameObject.GetComponent<Image>().raycastTarget = true;
            }
        }

        void DontDrag()
        {
            for (int i = 0; i < _drager1.Length; i++)
            {
                _drager1[i].transform.gameObject.GetComponent<Image>().raycastTarget = false;
            }
            for (int i = 0; i < _drager2.Length; i++)
            {
                _drager2[i].transform.gameObject.GetComponent<Image>().raycastTarget = false;
            }
            for (int i = 0; i < _drager3.Length; i++)
            {
                _drager3[i].transform.gameObject.GetComponent<Image>().raycastTarget = false;
            }
        }

        private void StartDrag(Vector3 dragPos, int dragType, int dragIndex)
        {
            if (_level == 1)
            {
                _dragerObj1[dragIndex].transform.parent = _level1.Find("Obj");
                _dragerObj1[dragIndex].GetComponent<Image>().SetNativeSize();
                _dragerObj1[dragIndex].transform.SetAsLastSibling();
            }
            if (_level == 2)
            {
                _dragerObj2[dragIndex].transform.parent = _level2.Find("Obj");
                _dragerObj2[dragIndex].GetComponent<Image>().SetNativeSize();
                _dragerObj2[dragIndex].transform.SetAsLastSibling();
            }
            if (_level == 3)
            {
                _dragerObj3[dragIndex].transform.parent = _level3.Find("Obj");
                _dragerObj3[dragIndex].GetComponent<Image>().SetNativeSize();
                _dragerObj3[dragIndex].transform.SetAsLastSibling();
            }
        }

        private void Draging(Vector3 dragPos, int dragType, int dragIndex)
        {
            
        }

        private void EndDrag(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            if (dragBool)
            {
                if(_level == 1)
                {
                    _dragerObj1[dragIndex].transform.parent = _level1.Find("EndPos");
                }
                if (_level == 2)
                {
                    _dragerObj2[dragIndex].transform.parent = _level2.Find("EndPos");
                }
                if (_level == 3)
                {
                    _dragerObj3[dragIndex].transform.parent = _level3.Find("EndPos");
                }
            }
            else
            {
                if (_level == 1)
                {
                    _dragerObj1[dragIndex].transform.position = _level1.Find("Pos").Find(_dragerObj1[dragIndex].gameObject.name).position;
                }
                if (_level == 2)
                {
                    _dragerObj2[dragIndex].transform.position = _level2.Find("Pos").Find(_dragerObj2[dragIndex].gameObject.name).position;
                }
                if (_level == 3)
                {
                    _dragerObj3[dragIndex].transform.position = _level3.Find("Pos").Find(_dragerObj3[dragIndex].gameObject.name).position;
                }
            }
        }
        #endregion
    }
}
