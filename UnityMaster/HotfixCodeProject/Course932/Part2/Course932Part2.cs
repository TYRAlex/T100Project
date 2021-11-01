using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course932Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private Transform question;
        private GameObject[] _question;
        private Transform anwser;
        private GameObject[] _anwser;
        private GameObject[] _anwserA;
        private GameObject[] _anwserB;
        private Transform clickBtn;
        private Empty4Raycast[] _clickBtn;
        private GameObject btnBack;

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

            question = curTrans.Find("question");
            question.gameObject.Show();
            _question = new GameObject[question.childCount];
            for (int i = 0; i < _question.Length; i++)
            {
                _question[i] = question.GetChild(i).gameObject;
            }

            anwser = curTrans.Find("answer");
            _anwser = new GameObject[anwser.childCount];           
            for (int i = 0; i < _anwser.Length; i++)
            {
                _anwser[i] = anwser.GetChild(i).gameObject;                            
            }
            _anwserA = new GameObject[_anwser[0].transform.childCount];
            _anwserB = new GameObject[_anwser[1].transform.childCount];
            for (int i = 0; i < _anwserA.Length; i++)
            {
                _anwserA[i] = _anwser[0].transform.GetChild(i).gameObject;
                _anwserA[i].Hide();
            }
            for (int i = 0; i < _anwserB.Length; i++)
            {
                _anwserB[i] = _anwser[1].transform.GetChild(i).gameObject;
                _anwserB[i].Hide();
            }

            clickBtn = curTrans.Find("clickBtn");
            _clickBtn = new Empty4Raycast[clickBtn.childCount];
            for (int i = 0; i < _clickBtn.Length; i++)
            {
                _clickBtn[i] = clickBtn.GetChild(i).GetComponent<Empty4Raycast>();
                Util.AddBtnClick(_clickBtn[i].gameObject, ClickBtnEvent);
            }
            clickBtn.gameObject.Hide();

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, ClickBtnBack);
            btnBack.Hide();
        }
        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE,1, null, () => 
            { 
                isPlaying = false;
                clickBtn.gameObject.Show();
            }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
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
        IEnumerator WaritCoroutine(Action method_2 = null, float len = 0)
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
        private void ClickBtnEvent(GameObject obj)
        {
            question.gameObject.Hide();
            clickBtn.gameObject.Hide();
            btnBack.Hide();
            AnswerHide(_anwserA.Length, _anwserA);
            AnswerHide(_anwserB.Length, _anwserB);
            switch (obj.name)
            {
                case "1":                    
                    MaxSpeack(SoundManager.SoundType.VOICE, 0, () => 
                    { 
                        _anwserA[0].Show(); ChangeApla(_anwserA[0]);
                        mono.StartCoroutine(WaritCoroutine(() =>
                        {
                            _anwserA[1].Show(); ChangeApla(_anwserA[1]);                            
                        }, 6));
                       
                    },()=> { btnBack.Show(); });                    
                    break;
                case "2":
                    MaxSpeack(SoundManager.SoundType.VOICE, 2, () => 
                    { 
                        _anwserB[0].Show(); ChangeApla(_anwserB[0]);
                        mono.StartCoroutine(WaritCoroutine(() => 
                        {
                            _anwserB[1].Show(); ChangeApla(_anwserB[1]);                            
                            mono.StartCoroutine(WaritCoroutine(() =>
                            {
                               
                                _anwserB[2].Show(); ChangeApla(_anwserB[2]);                               
                                mono.StartCoroutine(WaritCoroutine(() =>
                                {
                                    
                                    _anwserB[3].Show(); ChangeApla(_anwserB[3]);
                                }, 4));
                            }, 2.5f));
                        }, 3.5f));
                    }, () => 
                    {
                        btnBack.Show();
                    });
                    break;
            }
        }
        private void ClickBtnBack(GameObject obj)
        {
            btnBack.Hide();
            AnswerHide(_anwserA.Length, _anwserA);
            AnswerHide(_anwserB.Length, _anwserB);
            question.gameObject.Show();
            for (int i = 0; i < _question.Length; i++)
            {
                ChangeApla(_question[i]);
            }
            clickBtn.gameObject.Show();
            //mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () => 
            //{
                
            //}));
        }
        private void MaxSpeack(SoundManager.SoundType type, int index, Action callBack = null, Action callBack2 = null)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, type, index, callBack,callBack2));
        }
        /// <summary>
        /// 改变不透明度
        /// </summary>
        private void ChangeApla(GameObject obj)
        {
            obj.GetComponent<CanvasGroup>().alpha = 0;
            obj.GetComponent<CanvasGroup>().DOFade(1, 1.0f);
        }
        /// <summary>
        /// 隐藏问题
        /// </summary>
        /// <param name="length"></param>
        /// <param name="goArray"></param>
        private void AnswerHide(int length, GameObject[]goArray)
        {
            for (int i = 0; i < length; i++)
            {
                goArray[i].Hide();
            }
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
