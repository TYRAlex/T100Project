using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course9210Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private int[,] number;
        private GameObject Box;
        private List<int> _randomList;
        private List<(int, int)> _randomList2;
        private RawImage[,] _rawimages;
        private (int, int) tempindex;
        private (int, int) bellpos;
        private int temp;
        private GameObject controlBox;
        private bool _canClick;
        private GameObject plus;
        private Text text;
        private int _number;
        private GameObject _replay;
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
            new List<(int, int)>();
            _randomList = new List<int>();
            _randomList2 = new List<(int, int)>();
            _rawimages = new RawImage[4, 4];
            Box = curTrans.Find("Box").gameObject;
            controlBox = curTrans.Find("controlBox").gameObject;
            _replay = curTrans.Find("replay").gameObject;
            plus = curTrans.Find("plus").gameObject;
            text = curTrans.Find("Text").GetComponent<Text>();
            number = new int[4, 4];
            Util.AddBtnClick(_replay, Replay);


            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _rawimages[i, j] = Box.transform.Find(i.ToString() + j.ToString()).GetComponent<RawImage>();
                }
            }

            for (int i = 0; i < 4; i++)
            {
                Util.AddBtnClick(controlBox.transform.GetChild(i).GetChild(0).gameObject, Move);
            }

            GameInit();
            GameStart();
        }







        private void GameInit()
        {

            controlBox.transform.GetChild(0).GetComponent<RawImage>().texture = bellTextures.texture[8];
            controlBox.transform.GetChild(1).GetComponent<RawImage>().texture = bellTextures.texture[10];
            controlBox.transform.GetChild(2).GetComponent<RawImage>().texture = bellTextures.texture[12];
            controlBox.transform.GetChild(3).GetComponent<RawImage>().texture = bellTextures.texture[14];

            for (int i = 0; i < 5; i++)
            {
                _randomList.Add(i);
            }
            curTrans.Find("mask").gameObject.SetActive(false);
            curTrans.Find("cg").gameObject.SetActive(false);
            talkIndex = 1;
            plus.SetActive(false);
            text.text = "0";
            _number = 0;
            _replay.SetActive(false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                Max.SetActive(false); isPlaying = false;
                Next(); _canClick = true;
            }));

            ConnectAndroid.Instance.SendConnectData();


            FunctionOf3Dof.ClickAnyButtonDown = TriggerCallBack;

        }

       private void Replay(GameObject obj)
        {
            FunctionOf3Dof.ClickAnyButtonDown = TriggerCallBack;
            BtnPlaySound();
            _replay.SetActive(false);
            GameInit();
            _canClick = true;
            Next();
        }

        //自动生成关卡
        private void Next()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    number[i, j] = 0;
                }
            }
            temp = _randomList[Random.Range(0, _randomList.Count)];
            _randomList.Remove(temp);
            _randomList2.Clear();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _randomList2.Add((i, j));
                }
            }
            for (int i = 0; i < 3; i++)
            {
                tempindex = _randomList2[Random.Range(0, _randomList2.Count)];
                number[tempindex.Item1, tempindex.Item2] = 1;
                _randomList2.Remove(tempindex);
            }
            tempindex = _randomList2[Random.Range(0, _randomList2.Count)];
            number[tempindex.Item1, tempindex.Item2] = 2;
            _randomList2.Remove(tempindex);
            show();
            mono.StartCoroutine(Wait(0.3f,
                () =>
                { _canClick = true; }
                ));
            mono.StartCoroutine(Wait(5f,
                () =>
                { ShanShuo(); }
                ));
            mono.StartCoroutine(Wait(8.5f,
                () =>
                { _canClick = false; }
                ));
            mono.StartCoroutine(Wait(9.2f,
                () =>
                {
                    if (_randomList.Count != 0)
                    {
                        mono.StartCoroutine(Wait(0.8f,
                            () =>
                            {
                                _canClick = true;
                                Next();
                            }
                            ));
                    }
                    else
                    {
                        Win();
                        return;
                    }
                }
                ));
        }

        private void show()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (number[i, j] == 0)
                    {
                        _rawimages[i, j].texture = null;
                        _rawimages[i, j].color = new Vector4(255, 255, 255, 0);
                    }
                    if (number[i, j] == 1)
                    {
                        _rawimages[i, j].texture = bellTextures.texture[temp];
                        _rawimages[i, j].color = new Vector4(255, 255, 255, 255);
                        _rawimages[i, j].SetNativeSize();
                    }
                    if (number[i, j] == 2)
                    {
                        _rawimages[i, j].texture = bellTextures.texture[5];
                        _rawimages[i, j].color = new Vector4(255, 255, 255, 255);
                        _rawimages[i, j].SetNativeSize();
                    }
                    if (number[i, j] == 3)
                    {
                        _rawimages[i, j].texture = bellTextures.texture[6];
                        _rawimages[i, j].color = new Vector4(255, 255, 255, 255);
                        _rawimages[i, j].SetNativeSize();
                    }
                }
            }
        }

        private void TriggerCallBack()
        {
            if (FunctionOf3Dof.Instance.PadBtnLeft || FunctionOf3Dof.Instance.PadBtnRight ||
                FunctionOf3Dof.Instance.PadBtnBotton || FunctionOf3Dof.Instance.PadBtnTop)
            {
                if (_canClick)
                {
                    _canClick = false;
                    BtnPlaySound();
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (number[i, j] == 2)
                            {
                                bellpos = (i, j);
                            }
                        }
                    }

                    if (FunctionOf3Dof.Instance.PadBtnTop)
                    {
                        //Debug.Log("按了上键");
                        if (bellpos.Item1 == 0)
                        {
                            _canClick = true;
                            return;
                        }

                        if (number[bellpos.Item1 - 1, bellpos.Item2] == 0)
                        {
                            number[bellpos.Item1 - 1, bellpos.Item2] = 2;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            show();
                            _canClick = true;
                        }
                        else
                        {
                            number[bellpos.Item1 - 1, bellpos.Item2] = 3;
                            _canClick = false;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            plus.SetActive(true);
                            plus.transform.position = _rawimages[bellpos.Item1 - 1, bellpos.Item2].gameObject.transform
                                .position;
                            mono.StartCoroutine(MovePlus(plus));
                            show();
                            number[bellpos.Item1 - 1, bellpos.Item2] = 2;
                            mono.StartCoroutine(Wait(1.2f,
                                () =>
                                {
                                    show();
                                    _canClick = true;
                                }
                            ));
                        }
                    }
                    else if (FunctionOf3Dof.Instance.PadBtnBotton)
                    {
                        //Debug.Log("按了下键");
                        if (bellpos.Item1 == 3)
                        {
                            _canClick = true;
                            return;
                        }

                        if (number[bellpos.Item1 + 1, bellpos.Item2] == 0)
                        {
                            number[bellpos.Item1 + 1, bellpos.Item2] = 2;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            show();
                            _canClick = true;
                        }
                        else
                        {
                            number[bellpos.Item1 + 1, bellpos.Item2] = 3;
                            _canClick = false;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            plus.SetActive(true);
                            plus.transform.position = _rawimages[bellpos.Item1 + 1, bellpos.Item2].gameObject.transform
                                .position;
                            mono.StartCoroutine(MovePlus(plus));
                            show();
                            number[bellpos.Item1 + 1, bellpos.Item2] = 2;
                            mono.StartCoroutine(Wait(1.2f,
                                () =>
                                {
                                    show();
                                    _canClick = true;
                                }
                            ));
                        }
                    }
                    else if (FunctionOf3Dof.Instance.PadBtnLeft)
                    {
                        //Debug.Log("按了左键");
                        if (bellpos.Item2 == 0)
                        {
                            _canClick = true;
                            return;
                        }

                        if (number[bellpos.Item1, bellpos.Item2 - 1] == 0)
                        {
                            number[bellpos.Item1, bellpos.Item2 - 1] = 2;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            show();
                            _canClick = true;
                        }
                        else
                        {
                            number[bellpos.Item1, bellpos.Item2 - 1] = 3;
                            _canClick = false;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            plus.SetActive(true);
                            plus.transform.position = _rawimages[bellpos.Item1, bellpos.Item2 - 1].gameObject.transform
                                .position;
                            mono.StartCoroutine(MovePlus(plus));
                            show();
                            number[bellpos.Item1, bellpos.Item2 - 1] = 2;
                            mono.StartCoroutine(Wait(1.2f,
                                () =>
                                {
                                    show();
                                    _canClick = true;
                                }
                            ));
                        }
                    }
                    else if (FunctionOf3Dof.Instance.PadBtnRight)
                    {
                        //Debug.Log("按了右键");
                        if (bellpos.Item2 == 3)
                        {
                            _canClick = true;
                            return;
                        }

                        if (number[bellpos.Item1, bellpos.Item2 + 1] == 0)
                        {
                            number[bellpos.Item1, bellpos.Item2 + 1] = 2;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            show();
                            _canClick = true;
                        }
                        else
                        {
                            number[bellpos.Item1, bellpos.Item2 + 1] = 3;
                            _canClick = false;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            plus.SetActive(true);
                            plus.transform.position = _rawimages[bellpos.Item1, bellpos.Item2 + 1].gameObject.transform
                                .position;
                            mono.StartCoroutine(MovePlus(plus));
                            show();
                            number[bellpos.Item1, bellpos.Item2 + 1] = 2;
                            mono.StartCoroutine(Wait(1.2f,
                                () =>
                                {
                                    show();
                                    _canClick = true;
                                }
                            ));
                        }
                    }
                }
            }
        }

        private void Move(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                BtnPlaySound();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (number[i, j] == 2)
                        { bellpos = (i, j); }
                    }
                }
                switch (obj.name)
                {
                    case "up":
                        obj.transform.parent.GetComponent<RawImage>().texture = bellTextures.texture[7];
                        mono.StartCoroutine(Wait(0.2f,
                            () => { obj.transform.parent.GetComponent<RawImage>().texture = bellTextures.texture[8]; }
                            ));
                        if (bellpos.Item1 == 0)
                        { _canClick = true; return; }
                        if (number[bellpos.Item1 - 1, bellpos.Item2] == 0)
                        {
                            number[bellpos.Item1 - 1, bellpos.Item2] = 2;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            show();
                            _canClick = true;
                        }
                        else
                        {
                            number[bellpos.Item1 - 1, bellpos.Item2] = 3;
                            _canClick = false;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            plus.SetActive(true);
                            plus.transform.localPosition = _rawimages[bellpos.Item1 - 1, bellpos.Item2].gameObject.transform.localPosition;
                            mono.StartCoroutine(MovePlus(plus));
                            show();
                            number[bellpos.Item1 - 1, bellpos.Item2] = 2;
                            mono.StartCoroutine(Wait(1.2f,
                                () =>
                                { 
                                    show();
                                    _canClick = true;
                                }
                                ));
                        }
                        break;
                    case "down":
                        obj.transform.parent.GetComponent<RawImage>().texture = bellTextures.texture[9];
                        mono.StartCoroutine(Wait(0.2f,
                            () => { obj.transform.parent.GetComponent<RawImage>().texture = bellTextures.texture[10]; }
                            ));
                        if (bellpos.Item1 == 3)
                        { _canClick = true; return; }
                        if (number[bellpos.Item1 + 1, bellpos.Item2] == 0)
                        {
                            number[bellpos.Item1 + 1, bellpos.Item2] = 2;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            show();
                            _canClick = true;
                        }
                        else
                        {
                            number[bellpos.Item1 + 1, bellpos.Item2] = 3;
                            _canClick = false;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            plus.SetActive(true);
                            plus.transform.localPosition = _rawimages[bellpos.Item1 + 1, bellpos.Item2].gameObject.transform.localPosition;
                            mono.StartCoroutine(MovePlus(plus));
                            show();
                            number[bellpos.Item1 + 1, bellpos.Item2] = 2;
                            mono.StartCoroutine(Wait(1.2f,
                                () =>
                                {
                                    show();
                                    _canClick = true;
                                }
                                ));
                        }
                        break;
                    case "left":
                        obj.transform.parent.GetComponent<RawImage>().texture = bellTextures.texture[11];
                        mono.StartCoroutine(Wait(0.2f,
                            () => { obj.transform.parent.GetComponent<RawImage>().texture = bellTextures.texture[12]; }
                            ));
                        if (bellpos.Item2 == 0)
                        { _canClick = true; return; }
                        if (number[bellpos.Item1, bellpos.Item2 - 1] == 0)
                        {
                            number[bellpos.Item1, bellpos.Item2 - 1] = 2;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            show();
                            _canClick = true;
                        }
                        else
                        {
                            number[bellpos.Item1, bellpos.Item2 - 1] = 3;
                            _canClick = false;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            plus.SetActive(true);
                            plus.transform.localPosition = _rawimages[bellpos.Item1, bellpos.Item2 - 1].gameObject.transform.localPosition;
                            mono.StartCoroutine(MovePlus(plus));
                            show();
                            number[bellpos.Item1, bellpos.Item2 - 1] = 2;
                            mono.StartCoroutine(Wait(1.2f,
                                () =>
                                {
                                    show();
                                    _canClick = true;
                                }
                                ));
                        }
                        break;
                    case "right":
                        obj.transform.parent.GetComponent<RawImage>().texture = bellTextures.texture[13];
                        mono.StartCoroutine(Wait(0.2f,
                            () => { obj.transform.parent.GetComponent<RawImage>().texture = bellTextures.texture[14]; }
                            ));
                        if (bellpos.Item2 == 3)
                        { _canClick = true; return; }
                        if (number[bellpos.Item1, bellpos.Item2 + 1] == 0)
                        {
                            number[bellpos.Item1, bellpos.Item2 + 1] = 2;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            show();
                            _canClick = true;
                        }
                        else
                        {
                            number[bellpos.Item1, bellpos.Item2 + 1] = 3;
                            _canClick = false;
                            number[bellpos.Item1, bellpos.Item2] = 0;
                            plus.SetActive(true);
                            plus.transform.localPosition = _rawimages[bellpos.Item1, bellpos.Item2 + 1].gameObject.transform.localPosition;
                            mono.StartCoroutine(MovePlus(plus));
                            show();
                            number[bellpos.Item1, bellpos.Item2 + 1] = 2;
                            mono.StartCoroutine(Wait(1.2f,
                                () =>
                                { 
                                    show();
                                    _canClick = true;
                                }
                                ));
                        }
                        break;
                }
            }
        }

        private void ShanShuo()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (number[i, j] == 1)
                    {
                        mono.StartCoroutine(Shan(2.7f, _rawimages[i, j].gameObject, i, j));
                    }
                }
            }
        }

        private void Win()
        {
            curTrans.Find("mask").gameObject.SetActive(true);
            curTrans.Find("cg").gameObject.SetActive(true);
            curTrans.Find("cg").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(curTrans.Find("cg").gameObject, "animation", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("cg").gameObject, "animation2", true);
                    _replay.SetActive(true);
                }
                );
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            FunctionOf3Dof.ClickAnyButtonDown = null;
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

        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        IEnumerator Shan(float time, GameObject obj, int i, int j)
        {
            float a = 0;
            while (true)
            {
                if (a < time)
                {
                    obj.SetActive(true);
                    a += 0.3f;
                    yield return new WaitForSeconds(0.3f);
                    a += 0.3f;
                    if (number[i, j] != 1)
                    { break; }
                    obj.SetActive(false);
                    yield return new WaitForSeconds(0.3f);
                }
                else
                {
                    obj.SetActive(true);
                    break;
                }
            }
            yield break;
        }

        IEnumerator MovePlus(GameObject obj)
        {
            float temp = 0;
            float time = 1f;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            while (true)
            {
                temp += 0.01f;
                if (temp > time)
                { break; }
                obj.transform.localPosition = new Vector2(obj.transform.localPosition.x + (255 - obj.transform.localPosition.x) * (temp / time), obj.transform.localPosition.y + ((454) - obj.transform.localPosition.y) * (temp / time));
                yield return new WaitForSeconds(0.01f);
            }
            
            _number += 10;
            text.text = _number.ToString();
            obj.SetActive(false);
            yield break;
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                GameInit();
                _canClick = true;
                Next();
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
