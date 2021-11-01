using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course8310Part2
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        GameObject _pictures;
        GameObject _ani;
        GameObject _mask;

        List<int> _voice;

        GameObject _bag0;
        GameObject _bag1;
        GameObject _bag2;
        GameObject _bag3;
        GameObject _playvoice;
        GameObject _delete;
        GameObject _parent;
        private GameObject _leftPage;
        private GameObject _rightPage;

        void Start(object o)
        {
            curGo = (GameObject) o;
            curTrans = curGo.transform;

            _parent = curTrans.GetGameObject("Parent");
            _voice = new List<int>();
            _pictures = _parent.transform.GetGameObject("pictures");
            if (_pictures != null)
            {
                for (int i = 0; i < _pictures.transform.childCount; i++)
                {
                    var child = _pictures.transform.GetChild(i).gameObject;
                    Util.AddBtnClick(child, ShowImg);
                }
            }

            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = _parent.transform.GetGameObject("btnTest");
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(true);
            _voice = new List<int>();
            _ani = _parent.transform.GetGameObject("ani");
            _bag0 = _parent.transform.GetGameObject("bag0");
            _bag1 = _parent.transform.GetGameObject("bag1");
            _bag2 = _parent.transform.GetGameObject("bag2");
            _bag3 = _parent.transform.GetGameObject("bag3");

            _leftPage = _parent.transform.GetGameObject("leftPage");
            _rightPage = _parent.transform.GetGameObject("rightPage");
            _mask = curTrans.GetGameObject("mask");
            _playvoice = _parent.transform.GetGameObject("playvoice");
            _delete = _parent.transform.GetGameObject("delete");
            index = 0;
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.StopAudio();
            bell = _parent.transform.GetGameObject("bell");
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            _bag2.Hide();
            _bag3.Hide();
            bell.Show();
            _voice.Clear();
            ClearImage();
            _ani.Show();
            _pictures.Show();
            _mask.Hide();
            talkIndex = 1;
            UIEventListener.Get(_delete).onDown = PointDwon;
            UIEventListener.Get(_delete).onUp = PointUp;
            Util.AddBtnClick(_delete, Delete);
            Util.AddBtnClick(_playvoice, PlayVoice);

            Util.AddBtnClick(_leftPage, ChangePage);
            Util.AddBtnClick(_rightPage, ChangePage);


            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.Hide();
        }

        private void ChangePage(GameObject obj)
        {
            if (obj == _leftPage)
            {
                _bag0.Show();
                _bag1.Show();
                _bag2.Hide();
                _bag3.Hide();
            }
            else
            {
                _bag0.Hide();
                _bag1.Hide();
                _bag2.Show();
                _bag3.Show();
            }
        }


        float time1 = 0;
        float time = 0;

        private void PointDwon(PointerEventData eventData)
        {
            time1 = Time.time;
        }

        private void PointUp(PointerEventData eventData)
        {
            _mask.Show();
            time = Time.time - time1;
            if (time > 1.2f)
            {
                if (_voice.Count == 0)
                    return;
                SpineManager.instance.DoAnimation(_ani, "animation2", false);
                ClearImage();
                _voice.Clear();
            }

            _mask.Hide();
        }


        void ClearImage()
        {
            for (int i = 0; i < _bag0.transform.childCount; i++)
            {
                var child = _bag0.transform.GetChild(i).gameObject;
                child.Hide();
                child.GetComponentInChildren<Image>().sprite = null;
            }

            for (int i = 0; i < _bag1.transform.childCount; i++)
            {
                var child = _bag1.transform.GetChild(i).gameObject;
                child.Hide();
                child.GetComponentInChildren<Image>().sprite = null;
            }

            for (int i = 0; i < _bag2.transform.childCount; i++)
            {
                var child = _bag2.transform.GetChild(i).gameObject;
                child.Hide();
                child.GetComponentInChildren<Image>().sprite = null;
            }

            for (int i = 0; i < _bag3.transform.childCount; i++)
            {
                var child = _bag3.transform.GetChild(i).gameObject;
                child.Hide();
                child.GetComponentInChildren<Image>().sprite = null;
            }
            _bag3.Hide();
            _bag2.Hide();
            _bag1.Show();
            _bag0.Show();
        }

        /// <summary>
        /// 删除图片列表
        /// </summary>
        /// <param name="obj"></param>
        private void Delete(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            if (_voice.Count == 0)
                return;
            SpineManager.instance.DoAnimation(_ani, "animation2", false);

            if (_voice.Count < 31)
            {
                var bagchildName = "p" + _voice.Count;
                var child = _bag0.transform.GetGameObject(bagchildName);
                child.transform.GetComponentInChildren<Image>().sprite = null;
                child.Hide();
            }

            if (_voice.Count > 30 && _voice.Count < 61)
            {
                var bagchildName = "p" + (_voice.Count - 30);
                var child = _bag1.transform.GetGameObject(bagchildName);
                child.transform.GetComponentInChildren<Image>().sprite = null;
                child.Hide();
                _bag3.Hide();
                _bag2.Hide();
                _bag1.Show();
                _bag0.Show();
            }

            if (_voice.Count > 60 && _voice.Count < 91)
            {
                var bagchildName = "p" + (_voice.Count - 60);
                var child = _bag2.transform.GetGameObject(bagchildName);
                child.transform.GetComponentInChildren<Image>().sprite = null;
                child.Hide();
               
            }

            if (_voice.Count > 90 && _voice.Count <= 120)
            {
                var bagchildName = "p" + (_voice.Count - 90);
                var child = _bag3.transform.GetGameObject(bagchildName);
                child.transform.GetComponentInChildren<Image>().sprite = null;
                child.Hide();
                
            }
            _voice.RemoveAt(_voice.Count - 1);
            Debug.LogError("VoiceCount:         " + _voice.Count);
            _mask.Hide();
        }


        private IEnumerator Play()
        {
            float ind;
            foreach (var tem in _voice)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, tem, null, null));
                ind = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, tem);
                yield return new WaitForSeconds(ind);
            }
        }

        void PlayVoice(GameObject obj)
        {
            if (_voice.Count == 0)
            {
                return;
            }

            _bag3.Hide();
            _bag2.Hide();
            _bag1.Show();
            _bag0.Show();
            SpineManager.instance.DoAnimation(_ani, "animation3", false);
            _mask.Show();
            PlayVoices();
        }


        /// <summary>
        /// 播放声音
        /// </summary>
        int index;

        void PlayVoices()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, GetVoiceIndex(index),
                () => { },
                () =>
                {
                    index++;
                    //说话结束
                    Debug.LogError("说话结束");
                    if (index <= 59)
                    {
                        _bag3.Hide();
                        _bag2.Hide();
                        _bag1.Show();
                        _bag0.Show();
                    }

                    if (index > 60)
                    {
                        _bag0.Hide();
                        _bag1.Hide();
                        _bag2.Show();
                        _bag3.Show();
                    }

                    var isEnd = index == _voice.Count;

                    if (!isEnd)
                    {
                        PlayVoices();
                    }
                    else
                    {
                        index = 0;
                        _mask.Hide();
                        return;
                    }
                }));
        }


        int GetVoiceIndex(int index)
        {
            var temp = _voice[index];
            return temp;
        }


        void GameStart()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 36, () => { _mask.Show(); }, () =>
            {
                _mask.Hide();
                bell.Hide();
            }));
        }

        /// <summary>
        /// 点击展示图片
        /// </summary>
        /// <param name="obj"></param>
        private void ShowImg(GameObject obj)
        {
            if (_voice == null)
            {
                Debug.LogError("_voice is Null");
                return;
            }

            Debug.LogError("voice:      " + _voice.Count);

            _mask.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);


            if (_voice.Count < 120)
            {
                if (_voice.Count < 60)
                {
                    _bag3.Hide();
                    _bag2.Hide();
                    _bag1.Show();
                    _bag0.Show();
                    if (_voice.Count < 30)
                    {
                        foreach (Transform temp in _bag0.transform)
                        {
                            var image = temp.GetGameObject("Image");
                            if (image.GetComponent<Image>().sprite == null)
                            {
                                image.GetComponent<Image>().sprite =
                                    obj.transform.GetChild(0).GetComponent<BellSprites>().sprites[0];
                                image.GetComponent<Image>().SetNativeSize();
                                temp.gameObject.Show();

                                break;
                            }


                            Debug.Log("Bag0           ");
                        }
                    }

                    if (_voice.Count > 29 && _voice.Count < 60)
                    {
                        foreach (Transform temp in _bag1.transform)
                        {
                            var image = temp.GetGameObject("Image");
                            if (image.GetComponent<Image>().sprite == null)
                            {
                                image.GetComponent<Image>().sprite =
                                    obj.transform.GetChild(0).GetComponent<BellSprites>().sprites[0];
                                image.GetComponent<Image>().SetNativeSize();
                                temp.gameObject.Show();

                                break;
                            }

                            Debug.Log("Bag1           ");
                        }
                    }
                }


                if (_voice.Count > 59 && _voice.Count < 120)
                {
                    _bag0.Hide();
                    _bag1.Hide();
                    _bag2.Show();
                    _bag3.Show();
                    if (_voice.Count < 90)
                    {
                        foreach (Transform temp in _bag2.transform)
                        {
                            var image = temp.GetGameObject("Image");
                            if (image.GetComponent<Image>().sprite == null)
                            {
                                image.GetComponent<Image>().sprite =
                                    obj.transform.GetChild(0).GetComponent<BellSprites>().sprites[0];
                                image.GetComponent<Image>().SetNativeSize();
                                temp.gameObject.Show();

                                break;
                            }

                            Debug.Log("Bag2           ");
                        }
                    }

                    if (_voice.Count > 89 && _voice.Count < 120)
                    {
                        foreach (Transform temp in _bag3.transform)
                        {
                            var image = temp.GetGameObject("Image");
                            if (image.GetComponent<Image>().sprite == null)
                            {
                                image.GetComponent<Image>().sprite =
                                    obj.transform.GetChild(0).GetComponent<BellSprites>().sprites[0];
                                image.GetComponent<Image>().SetNativeSize();
                                temp.gameObject.Show();
                                break;
                            }

                            Debug.Log("Bag3           ");
                        }
                    }
                }

                int voiceIndex = int.Parse(obj.transform.GetChild(1).gameObject.name);
                _voice.Add(voiceIndex);
            }
            else
            {
                Debug.Log("chaochu"+_voice.Count);
            }
            Debug.LogError("i:       " + _voice.Count);
            _mask.Hide();
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }

            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "daiji");
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
            if (talkIndex == 1)
            {
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
    }
}