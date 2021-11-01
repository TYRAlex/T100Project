using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course932Part3
    {
        public enum ClickBtnOne
        {
            one,
            two,
            three,
            four,
            NULL
        }
        public enum ClickBtnZero
        {
            one,
            two,
            three,
            four,
            NULL
        }
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private Transform clickBtn;
        private Transform[] _clickBtn;
        private Empty4Raycast[] clickBtnA;        
        private Empty4Raycast[] clickBtnB;
        private ClickBtnOne clickBtnOne;
        private ClickBtnZero clickBtnZero;


        private Transform spineA;
        private Transform spineB;
        private Transform spineC;       
        private GameObject[] _spineA;
        private GameObject[] _spineB;
        private GameObject[] _spineC;
        private GameObject book;

        private GameObject spine_A;
        private GameObject spine_B;
        private GameObject spine_C;
        private GameObject spine_D;
        private float playAniTime;


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
            playAniTime = 0;

            ClickBtnInit();

            book = curTrans.Find("spine/book").gameObject;
            book.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(book, "fengmian", false);

            spineA = curTrans.Find("spine/a");
            spineB = curTrans.Find("spine/b");
            spineC = curTrans.Find("spine/c");
            _spineA = new GameObject[spineA.childCount];
            for (int i = 0; i < _spineA.Length; i++)
            {
                _spineA[i] = spineA.GetChild(i).gameObject;                           
            }
            _spineA[0].Show();
            

            _spineB = new GameObject[spineB.childCount];
            for (int i = 0; i < _spineB.Length; i++)
            {
                _spineB[i] = spineC.GetChild(i).gameObject;                            
            }
            _spineB[0].Show();
           

            _spineC = new GameObject[spineC.childCount];
            for (int i = 0; i < _spineB.Length; i++)
            {
                _spineC[i] = spineC.GetChild(i).gameObject;                            
            }
            _spineC[0].Show();
           

            spine_A = curTrans.Find("spine/A").gameObject;
            spine_A.Show();
            

            spine_B = curTrans.Find("spine/B").gameObject;
            spine_B.Show();            

            spine_C = curTrans.Find("spine/C").gameObject;
            spine_C.Show();
            

            spine_D = curTrans.Find("spine/D").gameObject;
            spine_D.Show();
           

            PlayKongAni();
        }
        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            {
                isPlaying = false;
                _clickBtn[0].gameObject.Show();
            }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
        }
        private void ClickBtnInit()
        {
            clickBtn = curTrans.Find("clickBtn");
            _clickBtn = new Transform[clickBtn.childCount];
            for (int i = 0; i < _clickBtn.Length; i++)
            {
                _clickBtn[i] = clickBtn.GetChild(i);
            }
            clickBtnA = new Empty4Raycast[_clickBtn[0].childCount];
            for (int i = 0; i < clickBtnA.Length; i++)
            {
                clickBtnA[i] = _clickBtn[0].GetChild(i).GetComponent<Empty4Raycast>();
                Util.AddBtnClick(clickBtnA[i].gameObject, ClickBtnAEvent);
            }
            clickBtnB = new Empty4Raycast[_clickBtn[1].childCount];
            for (int i = 0; i < clickBtnB.Length; i++)
            {
                clickBtnB[i] = _clickBtn[1].GetChild(i).GetComponent<Empty4Raycast>();
                Util.AddBtnClick(clickBtnB[i].gameObject, ClickBtnBEvent);
            }
            _clickBtn[1].GetChild(1).gameObject.Show();
            HideBtn();
        }
        /// <summary>
        /// 隐藏按钮
        /// </summary>
        private void HideBtn()
        {
            for (int i = 0; i < _clickBtn.Length; i++)
            {
                _clickBtn[i].gameObject.Hide();
            }
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

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        IEnumerator WaritCoroutine( Action method_2 = null, float len = 0)
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

            talkIndex++;
        }     
        private void ClickBtnAEvent(GameObject obj)
        {
            Max.SetActive(false);
            _clickBtn[0].gameObject.Hide();
            //心愿储蓄法
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            SpineManager.instance.DoAnimation(book, "dakai", false,()=> 
            {                
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, () =>
                {
                    mono.StartCoroutine(WaritCoroutine(() =>
                    {
                        SpineManager.instance.DoAnimation(_spineA[0], "a1", false, () =>
                        {
                            mono.StartCoroutine(WaritCoroutine(() => 
                            {
                                SpineManager.instance.DoAnimation(_spineA[0], "a2", false, () =>
                                {
                                    mono.StartCoroutine(WaritCoroutine(() => 
                                    { SpineManager.instance.DoAnimation(_spineA[0], "a3", false); }, 3.9f));                                   
                                });
                            }, 1.5f)); 
                        });
                    }, 7f));                    
                }, ()=> 
                {
                    clickBtnOne = ClickBtnOne.one;
                    clickBtnZero = ClickBtnZero.one;
                    _clickBtn[1].gameObject.Show();
                }));                             
            });            
        }
        private void ClickBtnBEvent(GameObject obj)
        {
            switch (obj.name)
            {
                case "0":
                    ClickBtnZreoEvent();
                    break;
                case "1":
                    ClickBtnOneEvent();
                    break;
            }
        }
        private void ClickBtnOneEvent()
        {
            HideBtn();
            _clickBtn[0].gameObject.Hide();
            _clickBtn[1].GetChild(1).gameObject.Show();
            PlayKongAni();
            switch (clickBtnOne)
            {
                case ClickBtnOne.one:
                    //52周  
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(spine_A, "4", false);
                    SpineManager.instance.DoAnimation(spine_B, "1", false);
                    playAniTime = SpineManager.instance.DoAnimation(spine_C, "a1", false);
                    mono.StartCoroutine(WaritCoroutine(() =>
                    {
                        SpineManager.instance.DoAnimation(spine_C, "3", false);
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, () =>
                        {
                            SpineManager.instance.SetTimeScale(_spineB[0], 0.2f);
                            SpineManager.instance.DoAnimation(_spineB[0], "b1", false, () =>
                            {
                                SpineManager.instance.SetTimeScale(_spineB[0], 0.5f);
                                SpineManager.instance.DoAnimation(_spineB[0], "b2", false, () =>
                                {
                                    SpineManager.instance.SetTimeScale(_spineB[0], 1);
                                    mono.StartCoroutine(WaritCoroutine(() =>
                                    {
                                        SpineManager.instance.DoAnimation(_spineB[0], "b3", false);
                                    }, 0.5f));
                                });
                            });
                        }, () =>
                        {
                            clickBtnOne = ClickBtnOne.two;
                            clickBtnZero = ClickBtnZero.two;
                            _clickBtn[1].gameObject.Show();
                        }));
                    }, playAniTime));
                    break;
                case ClickBtnOne.two:
                    //365天 
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(spine_A, "6", false);
                    SpineManager.instance.DoAnimation(spine_B, "3", false);
                    playAniTime = SpineManager.instance.DoAnimation(spine_C, "a2", false);
                    mono.StartCoroutine(WaritCoroutine(() => 
                    {
                        SpineManager.instance.DoAnimation(spine_C, "5", false);
                        _spineC[0].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, () =>
                        {
                            SpineManager.instance.DoAnimation(_spineC[0], "c1", false, () =>
                            {
                                mono.StartCoroutine(WaritCoroutine(() =>
                                {
                                    SpineManager.instance.DoAnimation(_spineC[0], "c2", false, () =>
                                    {
                                        mono.StartCoroutine(WaritCoroutine(() =>
                                        {
                                            SpineManager.instance.DoAnimation(_spineC[0], "c3", false);
                                        }, 1));
                                    });
                                }, 10));
                            });
                        }, () =>
                        {
                            clickBtnOne = ClickBtnOne.three;
                            clickBtnZero = ClickBtnZero.three;
                            _clickBtn[1].gameObject.Show();
                        }));
                    }, playAniTime));
                    break;
                case ClickBtnOne.three:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(spine_A, "8", false);
                    SpineManager.instance.DoAnimation(spine_B, "5", false);
                    SpineManager.instance.DoAnimation(spine_C, "a3", false, () =>
                    {
                        SpineManager.instance.DoAnimation(spine_C, "7", false, () => 
                        {
                            Max.SetActive(true);
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, () =>
                            {

                            }, () =>
                            {
                                clickBtnOne = ClickBtnOne.NULL;
                                clickBtnZero = ClickBtnZero.four;
                                _clickBtn[1].gameObject.Show();
                                _clickBtn[1].GetChild(1).gameObject.Hide();
                            }));
                        });
                    });                                   
                    break;
                case ClickBtnOne.NULL:
                    Debug.LogError("----ClickBtnOne.NULL----");
                    break;
            }
        }
        private void ClickBtnZreoEvent()
        {
            HideBtn();
            PlayKongAni();
            _clickBtn[0].gameObject.Hide();
            _clickBtn[1].GetChild(1).gameObject.Show();
            switch (clickBtnZero)
            {
                case ClickBtnZero.one: 
                    //关书
                    SpineManager.instance.DoAnimation(_spineA[0], "kong", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    SpineManager.instance.DoAnimation(book, "guanbi", false, () => 
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { _clickBtn[0].gameObject.Show(); }));
                    });                   
                    break;
                case ClickBtnZero.two:
                    //心愿
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, () =>
                    {
                        SpineManager.instance.DoAnimation(spine_A, "b1", false, () =>
                        {
                            mono.StartCoroutine(WaritCoroutine(() =>
                            {
                                SpineManager.instance.DoAnimation(_spineA[0], "a1", false, () =>
                                {
                                    mono.StartCoroutine(WaritCoroutine(() =>
                                    {
                                        SpineManager.instance.DoAnimation(_spineA[0], "a2", false, () =>
                                        {
                                            mono.StartCoroutine(WaritCoroutine(() =>
                                            { SpineManager.instance.DoAnimation(_spineA[0], "a3", false); }, 3));
                                        });
                                    },1.5f));
                                });
                            }, 6.5f));
                        });
                    }, ()=> 
                    {
                        clickBtnOne = ClickBtnOne.one;
                        clickBtnZero = ClickBtnZero.one;
                        _clickBtn[1].gameObject.Show();
                    }));                    
                    break;
                case ClickBtnZero.three:
                    //52
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(spine_A, "6", false);
                    SpineManager.instance.DoAnimation(spine_B, "3", false);
                    playAniTime = SpineManager.instance.DoAnimation(spine_C, "b2", false);
                    mono.StartCoroutine(WaritCoroutine(() => 
                    {
                        SpineManager.instance.DoAnimation(spine_C, "4", false);
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, () =>
                        {
                            SpineManager.instance.SetTimeScale(_spineB[0], 0.2f);
                            SpineManager.instance.DoAnimation(_spineB[0], "b1", false, () =>
                            {
                                SpineManager.instance.SetTimeScale(_spineB[0], 0.5f);
                                SpineManager.instance.DoAnimation(_spineB[0], "b2", false, () =>
                                {

                                    mono.StartCoroutine(WaritCoroutine(() =>
                                    {
                                        SpineManager.instance.SetTimeScale(_spineB[0], 1);
                                        SpineManager.instance.DoAnimation(_spineB[0], "b3", false);
                                    }, 0.5f));
                                });
                            });
                        }, () =>
                        {
                            clickBtnOne = ClickBtnOne.two;
                            clickBtnZero = ClickBtnZero.two;
                            _clickBtn[1].gameObject.Show();
                        }));
                    }, playAniTime));
                    break;
                case ClickBtnZero.four:
                    //365
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    Max.SetActive(false);
                    SpineManager.instance.DoAnimation(spine_A, "8", false);
                    SpineManager.instance.DoAnimation(spine_B, "5", false);
                    playAniTime = SpineManager.instance.DoAnimation(spine_C, "b3", false);
                    mono.StartCoroutine(WaritCoroutine(() =>
                    {
                        SpineManager.instance.DoAnimation(spine_C, "6", false);
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, () =>
                        {
                            SpineManager.instance.DoAnimation(_spineC[0], "c1", false, () =>
                            {
                                mono.StartCoroutine(WaritCoroutine(() =>
                                {
                                    SpineManager.instance.DoAnimation(_spineC[0], "c2", false, () =>
                                    {
                                        mono.StartCoroutine(WaritCoroutine(() =>
                                        {
                                            SpineManager.instance.DoAnimation(_spineC[0], "c3", false);
                                        }, 1));
                                    });
                                }, 10));
                            });
                        }, () =>
                        {
                            clickBtnOne = ClickBtnOne.three;
                            clickBtnZero = ClickBtnZero.three;
                            _clickBtn[1].gameObject.Show();
                        }));
                    }, playAniTime));
                    break;
            }
        }
        private void SpeakeAndPlayAni(SoundManager.SoundType type,int index,GameObject go,string aniName,Action callBack = null)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, type, index, () =>
            {
                SpineManager.instance.DoAnimation(go, aniName, false);
            }, callBack));
        }
        private void PlayAniABC(SoundManager.SoundType type,int index,GameObject go,string aniName1,string aniName2,Action callBack = null)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, type, index, () =>
            {
                SpineManager.instance.DoAnimation(go, aniName1, false, () =>
                {
                    SpineManager.instance.DoAnimation(go, aniName2, false, callBack);
                });
            }, null));       
        }
        private void PlayKongAni()
        {
            SpineManager.instance.DoAnimation(_spineA[0], "kong", false);
            SpineManager.instance.DoAnimation(_spineB[0], "kong", false);
            SpineManager.instance.DoAnimation(_spineC[0], "kong", false);
            SpineManager.instance.DoAnimation(spine_A, "kong", false);
            SpineManager.instance.DoAnimation(spine_B, "kong", false);
            SpineManager.instance.DoAnimation(spine_C, "kong", false);
        }
        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        #region
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
        #endregion
    }
}
