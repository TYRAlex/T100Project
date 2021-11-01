using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ILFramework.HotClass
{
    public class Course734Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject _spine0;
        private GameObject _spine1;
        private GameObject _spine2;

        private Transform _clickBtn;
        private PolygonCollider2D[] _clickBtnPo;
        private int clickIndex;

        private GameObject Video;
        private VideoPlayer _videoPlayer;
        private RawImage _rtImg;

        private GameObject _question;
        private GameObject _bg_1;

        private GameObject _bg_2;
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

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
          

            GameInit();
            GameStart();
        }
        private void GameInit()
        {
            talkIndex = 1;
            clickIndex = 1;

            _spine0 = curTrans.Find("spinePanel/0").gameObject;
            _spine1 = curTrans.Find("spinePanel/1").gameObject;
            _spine2 = curTrans.Find("spinePanel/2").gameObject;

            _spine0.Show();
            _spine1.Hide();
            _spine2.Hide();

            SpineManager.instance.DoAnimation(_spine0, "kong", false);

            _clickBtn = curTrans.Find("clickBtn");
            _clickBtnPo = _clickBtn.GetComponentsInChildren<PolygonCollider2D>(true);
            for (int i = 0; i < _clickBtnPo.Length; i++)
            {
                Util.AddBtnClick(_clickBtnPo[i].gameObject, ClickBtnPoEvent);
                _clickBtnPo[i].gameObject.Hide();
            }

            Video = curTrans.Find("videoMask/Video Player").gameObject;
            _videoPlayer = Video.GetComponent<VideoPlayer>();
            _rtImg = Video.GetComponent<RawImage>();
            Video.transform.parent.SetAsFirstSibling();
            _videoPlayer.targetTexture.Release();
            mono.StartCoroutine(PlayMp4("2"));
            
            //Video.Hide();
            _bg_1 = curTrans.Find("bg").gameObject;
            _bg_1.Show();
            _question = curTrans.Find("question").gameObject;
            _question.Show();

            _bg_2 = curTrans.Find("bg_2").gameObject;
            _bg_2.Hide();

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

        IEnumerator PlayMp4(string number)
        {           
            _videoPlayer.url = GetVideoPath(number + ".mp4");         
            _videoPlayer.Prepare();
            while (true)
            {
                if (!_videoPlayer.isPrepared)   //监听是否准备完毕。没有完成一直等待，完成后跳出循环，进行img赋值，让后播放                             
                    yield return null;
                else
                    break;
            }
            _rtImg.texture = _videoPlayer.texture;
         
            _videoPlayer.targetTexture.DiscardContents();
            //_videoPlayer.targetTexture.Release();
            _videoPlayer.playOnAwake = false;
            _videoPlayer.skipOnDrop = false;
            _videoPlayer.Play();
            _videoPlayer.Pause();

        }

        private string GetVideoPath(string videoPath)
        {
            var path = LogicManager.instance.GetVideoPath(videoPath);
            return path;
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
        IEnumerator WraiteCoroutine(Action method_2 = null, float len = 0)
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
                _question.Hide();
                _bg_1.Hide();
                SpineManager.instance.DoAnimation(_spine0, "tu2", false,()=> 
                {
                    mono.StartCoroutine(WraiteCoroutine(() => 
                    {
                        SpineManager.instance.DoAnimation(_spine0, "tu5", false);
                    }, 7.0f));
                });
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 2)
            {               
                //mono.StartCoroutine(PlayMp4("2"));               
                _spine0.Hide();               
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, false);
                Video.transform.parent.SetAsLastSibling();
                //Video.Show();
                _videoPlayer.Play();
                _spine1.Show();
                _spine1.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);               
                SpineManager.instance.DoAnimation(_spine1, "qiuzhuan-75", false, () =>
                {                    
                    mono.StartCoroutine(WraiteCoroutine(() =>
                    {
                        SpineManager.instance.SetTimeScale(_spine1, 0.2f);
                        SpineManager.instance.DoAnimation(_spine1, "qiuzhuan-750", false, () =>
                        {
                            SpineManager.instance.SetTimeScale(_spine1, 0.3f);
                            SpineManager.instance.DoAnimation(_spine1, "qiuzhuan075", false, () =>
                            {
                                SpineManager.instance.SetTimeScale(_spine1, 1);
                            });
                        });
                    }, 0.5f));
                });                             
            }
            else if (talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            }
            else if (talkIndex == 4)
            {
                 Video.transform.parent.SetAsFirstSibling();
                //Video.Hide();
                _bg_1.Show();
                _spine0.Hide();
                _spine1.Hide();
                _spine2.Show();
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, null));
                _spine2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_spine2, "5", false, () =>
                {
                    mono.StartCoroutine(WraiteCoroutine(() =>
                    {
                        //SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 4, false);
                        SpineManager.instance.DoAnimation(_spine2, "1", false, () =>
                        {
                            mono.StartCoroutine(WraiteCoroutine(() =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2, false);
                                mono.StartCoroutine(WraiteCoroutine(() => 
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2, false);
                                }, 1.2f));
                                mono.StartCoroutine(WraiteCoroutine(() =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2, false);
                                }, 2.3f));
                                SpineManager.instance.DoAnimation(_spine2, "2", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(_spine2, "3", false, () =>
                                    {
                                        SoundManager.instance.ShowVoiceBtn(true);
                                    });
                                });
                            },2.0f));
                        });
                    }, 4.0f));
                });
            }
            else if (talkIndex == 5)
            {
                _bg_1.Hide();
                _bg_2.Show();
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5, null, ()=> { ShowClick(true); }));              
                SpineManager.instance.DoAnimation(_spine2, "jxs10", false, () =>
                {
                    //ShowClick(true);
                });
            }

            talkIndex++;
        }
        private void ShowClick(bool isShow)
        {
            for (int i = 0; i < _clickBtnPo.Length; i++)
            {
                _clickBtnPo[i].gameObject.SetActive(isShow);
            }
        }
        private void ClickBtnPoEvent(GameObject obj)
        {
            SoundManager.instance.ShowVoiceBtn(false);
            ShowClick(false);
            if (clickIndex == 1)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 4, false);
                PlayAni(SoundManager.SoundType.VOICE, 6, "jxs", "jxs2",null, () => { ShowClick(true); });                
            }
            else if (clickIndex == 2)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 5, false);
                PlayAni(SoundManager.SoundType.VOICE, 7, "jxs3", "jxs4",null, () => { ShowClick(true); });                
            }
            else if (clickIndex == 3)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 5, false);
                PlayAni(SoundManager.SoundType.VOICE, 8, "jxs5", "jxs6",null, () => { ShowClick(true); });
            }
            else if (clickIndex == 4)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 4, false);
                PlayAni(SoundManager.SoundType.VOICE, 9, "jxs7", "jxs8", () =>
                {

                    SpineManager.instance.DoAnimation(_spine2, "jxs9", false, () =>
                    {
                        ShowClick(false);
                        //mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 10, null, null));
                    });
                },()=> 
                {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 10, null, null));
                });
            }
            clickIndex++;

        }
        private void PlayAni(SoundManager.SoundType type, int voiceIndex, string aniName, string aniName1, Action callBack = null,Action callBack2=null)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, type, voiceIndex, null, callBack2));
            SpineManager.instance.DoAnimation(_spine2, aniName, false, () =>
            {
                SpineManager.instance.DoAnimation(_spine2, aniName1, false, callBack);
            });
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
