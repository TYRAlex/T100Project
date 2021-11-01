using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course835Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
        
        private GameObject _pureBG;
        
        //private Transform _bgImage;

        private Vector3 _CenterPos;
        private Vector3 _nextPos;

        private GameObject _target;
        private GameObject _target2;
        
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            
            bell = curTrans.Find("bell").gameObject;
            
            bell.transform.SetAsLastSibling();
            
            talkIndex = 1;
            LoadAllProperty(curTrans);
            InitProperty();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        void GameStart()
        {
            
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
                () => SoundManager.instance.ShowVoiceBtn(true)));
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }

        void LoadAllProperty(Transform parent)
        {
            _target = parent.Find("Target").gameObject;
            _target2 = parent.Find("Target2").gameObject;
            _pureBG = parent.Find("PureBG").gameObject;
            bell.Show();
            bell.transform.GetGameObject("Apple").Show();
            _pureBG.transform.SetAsLastSibling();
            bell.transform.SetAsLastSibling();
            _CenterPos = parent.Find("BellCenterPos").localPosition;
            _nextPos = parent.Find("BellNextPos").localPosition;
            
        }

        void InitProperty()
        {
            SoundManager.instance.StopAudio();
             _target.Show();
             _target2.Show();
             _target.transform.SetAsFirstSibling();
             _target2.transform.SetAsFirstSibling();
             SpineManager.instance.DoAnimation(_target, "a0", false);
             SpineManager.instance.DoAnimation(_target2, "a1", false, () => _target2.gameObject.Hide());
            bell.transform.localPosition = _CenterPos;
            _pureBG.Hide();
        }

        private float _currentAudioTime = 0f;
        
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            _currentAudioTime = ind;
            SpineManager.instance.DoAnimation(bell, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "daiji");

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
                    //todo..步骤一： 画面切换至纯色背景，画面中央出现颜色传感器，伴随语音做凸显动画，Max位于画面左下角，播放Max说话动画
                    
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                        () =>
                        {
                            _pureBG.Show();
                            _target.Show();
                            _target.transform.SetAsLastSibling();
                            SpineManager.instance.DoAnimation(_target, "0", false);
                            bell.transform.localPosition = _nextPos;
                            bell.transform.GetGameObject("Apple").Hide();
                            //bell.transform.DOLocalMove(_nextPos, 1f);
                            
                        }, () =>
                        {
                            //完了之后隐藏人物MAX，播放第一版动画加语音
                            bell.gameObject.SetActive(false);
                            
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () =>
                            {
                                _target2.Show();
                                _target2.transform.SetAsLastSibling();
                                mono.StartCoroutine(WaitTimeAndExcuteNextAniIE());
                                // SpineManager.instance.DoAnimation(_target, "a1", false,
                                //     () =>
                                //     {
                                //         _target2.Show();
                                //         SpineManager.instance.DoAnimation(_target2, "b1", false);
                                //     });
                            }));
                            //todo...语音对应动画
                        }));

                    
                   

                    //todo。。。。 同时播放语音：机器人也是可以识别颜色的，它是通过颜色颜色传感器来识别的。我们一起来看下颜色传感是怎样识别颜色的吧！”
                    //todo... 语音播完之后Max消失，语音对应动画
                    //todo。。。。动画完了之后打开右侧系统UI面板，点击下一步，跳转至编程环节
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            talkIndex++;
        }

        IEnumerator WaitTimeAndExcuteNextAniIE()
        {
            
            SpineManager.instance.DoAnimation(_target, "a0", false);
            SpineManager.instance.DoAnimation(_target2, "b0", false);
            yield return new WaitForSeconds(_currentAudioTime * 14f / 111f);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SpineManager.instance.DoAnimation(_target, "a2", false);
            SpineManager.instance.DoAnimation(_target2, "b2", false);
            yield return new WaitForSeconds(_currentAudioTime * 33f / 111f);
            // SpineManager.instance.DoAnimation(_target, "a2", false);
            // SpineManager.instance.DoAnimation(_target2, "b2", false);
            yield return new WaitForSeconds(_currentAudioTime * 21f / 111f);
            SpineManager.instance.DoAnimation(_target, "a3", false);
            SpineManager.instance.DoAnimation(_target2, "b3", false);
            // yield return new WaitForSeconds(0.2f);
            // SpineManager.instance.DoAnimation(_target, "aj", false);
            // SpineManager.instance.DoAnimation(_target2, "bj", false);
            
        }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}
