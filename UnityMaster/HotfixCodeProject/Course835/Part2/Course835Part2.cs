using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course835Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;

        private GameObject bell;

        private GameObject _target;
        private GameObject _judgeTarget;
        private GameObject _highLight;

        //private StringBuilder _myStringBuilder;

        //private Text _myText;
        private GameObject _myText2;

        private char[] _allText = new[] { '什', '么', '是', '条', '件', '语', '句' };

        private GameObject _carParent;
        private ILDrager _carClear;

        private ILDrager _carDirty;
       
        
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            bell = curTrans.Find("bell").gameObject;

            talkIndex = 1;
            _target = curTrans.Find("Target").gameObject;
            _judgeTarget = curTrans.GetGameObject("Target2");
            InitProperty(curTrans);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        void GameStart()
        {
            
            _carClear.SetDragCallback(null, null, DragClearEnd);
            _carDirty.SetDragCallback(null, null, DragDirtyEnd);
            
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));

            //_myText.DOText("什么是条件语句", 2f).OnComplete(() => SoundManager.instance.ShowVoiceBtn(true));
            // ShowTitle();

        }



        void ResetAndPlayBGM()
        {
            SoundManager.instance.StopAudio();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }



        void InitProperty(Transform curTrans)
        {
            ResetAndPlayBGM();
            _carParent = curTrans.GetGameObject("Cars");
            _carClear = curTrans.Find("Cars/CarClear").GetComponent<ILDrager>();
            _carDirty = curTrans.Find("Cars/CarDirty").GetComponent<ILDrager>();
            //_myText = curTrans.Find("Text").GetComponent<Text>();
            _myText2 = curTrans.GetGameObject("Sentence");
            _highLight = curTrans.GetGameObject("HighLight");
            _target.Hide();
            _judgeTarget.Hide();
            _myText2.Show();
            //_myStringBuilder=new StringBuilder("");
            //_myText.gameObject.Show();
            _carClear.gameObject.Show();
            _carDirty.gameObject.Show();
            _carParent.Hide();
        }

        void DragClearEnd(Vector3 position, int type, int index, bool isMatch)
        {
            
            if (isMatch)
            {
                _carClear.DoReset();
                _carClear.gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                //todo...伴随音效，检查装置高亮，出现扫描动画两秒后，语音：“车辆干净，车辆干净”右方的道闸升起，车辆通行，直至行驶到最下方消失
                SpineManager.instance.DoAnimation(_target, "3", false, () =>
                {
                    //curGo.transform.GetComponent<RectTransform>().offsetMax=new Vector2();
                    SpineManager.instance.DoAnimation(_target, "1", false);
                    if (!_carDirty.gameObject.activeSelf)
                        SoundManager.instance.ShowVoiceBtn(true);
                });
            }
            else
            {
                _carClear.DoReset();
            }
        }

        void DragDirtyEnd(Vector3 position, int type, int index, bool isMatch)
        {
            
            if (isMatch)
            {
                _carDirty.DoReset();
                _carDirty.gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                //todo.... 伴随音效，检查装置高亮，出现扫描动画两秒后，语音：“脏车，脏车”下方的道闸升起，车辆通行，停在洗车房，伴随音效，播放洗车动画，车辆干净后，继续前行，行驶至最下方消失
                SpineManager.instance.DoAnimation(_target, "2", false, () =>
                {
                    if (!_carClear.gameObject.activeSelf)
                        SoundManager.instance.ShowVoiceBtn(true);

                });
            }
            else
            {
                _carDirty.DoReset();
            }
        }

        private float _currentVoiceLenth = 0;

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            _currentVoiceLenth = ind;
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");

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
                    //bell做说话动画，同时语音：“接下来我们通过一个例子来了解什么是条件语句。”
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                        () => SoundManager.instance.ShowVoiceBtn(true)));
                    break;

                case 2:
                    //点击语音键，进入下一个场景

                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () =>
                    {
                        _myText2.gameObject.Hide();
                        _carParent.Show();
                        _target.Show();
                        mono.StartCoroutine(HighLightByTimeIE(1, _currentVoiceLenth));
                    }, () => bell.transform.SetAsFirstSibling()));
                    break;
                case 3:


                    _target.Hide();
                    _judgeTarget.Show();
                    _carParent.Hide(); ;
                    SpineManager.instance.DoAnimation(_judgeTarget, "4", false,
                        () => SpineManager.instance.DoAnimation(_judgeTarget, "5", false));
                    bell.transform.SetAsLastSibling();
                    //伴随音效，将图形简略变形为流程图。
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, null, () =>
                    {
                        // float currentVoiceLenth =
                        //     SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5,
                            () => mono.StartCoroutine(HighLightByTimeIE(2, _currentVoiceLenth))));
                        
                    }));

                    //todo 同时播放语音：条件语句，就是判断给定的条件是否满足，并根据判断的结果，决定执行的语句。 “
                    //如果满足条件（箭头高亮，“是”高亮及放大），那么执行相应的程序（“语句框”高亮）。否则跳出程序（箭头高亮，”否“高亮及放大）。

                    break;

            }
            talkIndex++;
        }

        IEnumerator HighLightByTimeIE(int currentSentence,float voiceLenth)
        {
            
            if (currentSentence == 1)
            {
                yield return new WaitForSeconds(voiceLenth * 8f / 57f);
                _highLight.Show();
                SpineManager.instance.DoAnimation(_highLight, "3", false);
                yield return new WaitForSeconds(voiceLenth * 7 / 57f);
                SpineManager.instance.DoAnimation(_highLight, "2", false,
                    () => SpineManager.instance.DoAnimation(_highLight, "1", false));
                yield return new WaitForSeconds(18 * voiceLenth / 57f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                SpineManager.instance.DoAnimation(_highLight, "4", false);
            }
            else if(currentSentence==2)
            {
                _highLight.Show();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                SpineManager.instance.DoAnimation(_highLight, "8", false,
                    () => SpineManager.instance.DoAnimation(_highLight, "6", false));
                yield return new WaitForSeconds(voiceLenth*6f/21f);
                SpineManager.instance.DoAnimation(_highLight, "5", false);
                yield return new WaitForSeconds(voiceLenth * 9f / 21f);
                SpineManager.instance.DoAnimation(_highLight, "7", false);
                
                
            }
            yield return new WaitForSeconds(1f);
            _highLight.Hide();
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }

        // void ShowTitle()
        // {
        //     mono.StartCoroutine(ShowTitleIE());
        // }
        //
        // IEnumerator ShowTitleIE()
        // {
        //     WaitForSeconds oneSeconds = new WaitForSeconds(0.5f);
        //     for (int i = 0; i < _allText.Length; i++)
        //     {
        //         _myStringBuilder.Append(_allText[i]);
        //         _myText.text = _myStringBuilder.ToString();
        //         yield return oneSeconds;
        //     }
        //
        // }
    }
}
