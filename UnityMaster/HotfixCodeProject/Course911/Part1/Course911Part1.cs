using DG.Tweening;
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
    public class Course911Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject max;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        GameObject _lia;


        GameObject _unit;
        GameObject _mask;

        private Transform _parents;

        void Start(object o)
        {
            curGo = (GameObject) o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;

            Util.AddBtnClick(btnTest, ReStart);
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            _mask = curTrans.GetGameObject("mask");
            _parents = curTrans.Find("Parents");
            _unit = _parents.GetGameObject("unit");
            _lia = _parents.GetGameObject("lia");
            max = _parents.Find("max").gameObject;
            PointerClickListener.Get(_mask).onClick = null;

            for (int i = 0; i < _parents.childCount; i++)
            {
                if (i >= 4)
                    break;

                var child = _parents.GetChild(i).gameObject;
                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = OnClickP;
            }

            btnTest.SetActive(false);
            ReStart(btnTest);
        }


        //点击图片事件
        private void OnClickP(GameObject go)
        {
            BtnPlaySound();
            _mask.Show();
            var name = go.name;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);

            switch (name)
            {
                case "p1":
                    PlaySpineAni("a1", () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        PlaySpineAni("a2", () => { BackSpineAni("a3"); });
                    });
                    break;
                case "p2":
                    PlaySpineAni("b1", () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        PlaySpineAni("b2", () => { BackSpineAni("b3"); });
                    });
                    break;
                case "p3":
                    PlaySpineAni("c1", () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        PlaySpineAni("c2", () => { BackSpineAni("c3"); });
                    });
                    break;
                case "p4":
                    PlaySpineAni("d1", () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        PlaySpineAni("d2", () => { BackSpineAni("d3"); });
                    });
                    break;
            }
        }

        void PlaySpineAni(string aniName, Action callBack = null)
        {
            SpineManager.instance.DoAnimation(_unit, aniName, false, callBack);
        }

        void BackSpineAni(string name)
        {
            PointerClickListener.Get(_mask).onClick = go =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                PointerClickListener.Get(_mask).onClick = null;
                PlaySpineAni(name, () => { _mask.Hide(); });
            };
        }

        void ReStart(GameObject obj)
        {
            _lia.Show();
            max.Show();
            _mask.Hide();
            _unit.Hide();
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            talkIndex = 1;
            max.transform.GetRectTransform().anchoredPosition = new Vector2(367, 140);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.SetActive(false);
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
                () => 
                {
                    _mask.Show();  
                    Debug.Log("平时我们用洗手液时，必须用手按压，手上的细菌很有可能残留在按压处");
                    SpineManager.instance.DoAnimation(max, "daiji2", false,
                        () => { SpineManager.instance.DoAnimation(max, "daijishuohua", false); });
                    SpineManager.instance.DoAnimation(_lia, "animation", false,
                        () => { SpineManager.instance.DoAnimation(_lia, "animation2", false); }); },
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                    _mask.Hide();
                }));
          
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
            SpineManager.instance.DoAnimation(max, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(max, "daiji");
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
                Sence();
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

        void Sence()
        {
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
            _lia.Hide();
            _unit.Show();
            SpineManager.instance.DoAnimation(_unit,"jing", false);
            max.transform.GetRectTransform().anchoredPosition = new Vector3(-300, -40);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                () =>
                {
                    Debug.Log("这里有四款免接触按压洗手液的装置，它们能解决细菌残留问题吗？仔细观察，并说一说你们的想法吧~");
                    _mask.Show();
                }, () => { _mask.Hide(); }));
        }
    }
}