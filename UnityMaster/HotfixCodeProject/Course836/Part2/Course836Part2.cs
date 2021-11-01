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
    public class Course836Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private Transform StagePanel;
        private GameObject ColorPanel;
        private GameObject HaloPanel;

        private Transform GreenY;
        private Transform GreenStartPos;
        private Transform BlueY;
        private Transform BlueStartPos;
        private Transform RedY;
        private Transform RedStartPos;
        private Transform GreenEndPos;
        private Transform BlueEndPos;
        private Transform RedEndPos;

        private Transform bellStartPos;
        private Transform bellDownPos;

        private MonoScripts ms;

        private Slider GreenSlider;
        private Slider BlueSlider;
        private Slider RedSlider;

        private GameObject GreenLight;
        private GameObject BlueLight;
        private GameObject RedLight;

        private Image GreenLightM;
        private Image BlueLightM;
        private Image RedLightM;


        Color green;
        Color blue;
        Color red;




        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            HaloPanel = curTrans.Find("HaloPanel").gameObject;
            GreenStartPos = curTrans.Find("HaloPanel/GreenStartPos");
            GreenY = curTrans.Find("HaloPanel/GreenY");
            GreenY.position = GreenStartPos.position;
            BlueStartPos = curTrans.Find("HaloPanel/BlueStartPos");
            BlueY = curTrans.Find("HaloPanel/BlueY");
            BlueY.position = BlueStartPos.position;
            RedStartPos = curTrans.Find("HaloPanel/RedStartPos");
            RedY = curTrans.Find("HaloPanel/RedY");
            RedY.position = RedStartPos.position;
            GreenEndPos = curTrans.Find("HaloPanel/GreenEndPos");
            BlueEndPos = curTrans.Find("HaloPanel/BlueEndPos");
            RedEndPos = curTrans.Find("HaloPanel/RedEndPos");


            HaloPanel.SetActive(false);

            StagePanel = curTrans.Find("StagePanel");
            StagePanel.gameObject.SetActive(true);



            ColorPanel = curTrans.Find("ColorPanel").gameObject;
            ms = ColorPanel.GetComponent<MonoScripts>();
            ms.UpdateCallBack += this.Update;
            ms.OnDisableCallBack += this.OnDisable;

            GreenLight = curTrans.Find("ColorPanel/GreenLight").gameObject;
            BlueLight = curTrans.Find("ColorPanel/BlueLight").gameObject;

            RedLight = curTrans.Find("ColorPanel/RedLight").gameObject;

            GreenLightM = GreenLight.GetComponent<Image>();
            BlueLightM = BlueLight.GetComponent<Image>();
            RedLightM = RedLight.GetComponent<Image>();

            GreenSlider = curTrans.Find("ColorPanel/GreenSlider").GetComponent<Slider>();
            BlueSlider = curTrans.Find("ColorPanel/BlueSlider").GetComponent<Slider>();
            RedSlider = curTrans.Find("ColorPanel/RedSlider").GetComponent<Slider>();

            GreenSlider.value = 0;
            BlueSlider.value = 0;
            RedSlider.value = 0;

            red = new Color(RedSlider.value / 255f, RedSlider.value / 255f, RedSlider.value / 255f);

            green = new Color(RedSlider.value / 255f, GreenSlider.value / 255f, RedSlider.value / 255f);
            blue = new Color(RedSlider.value / 255f, RedSlider.value / 255f, BlueSlider.value / 255f);



            ColorPanel.SetActive(false);
            bell = curTrans.Find("bell").gameObject;
            bellStartPos = curTrans.Find("bellStartPos");
            bellDownPos = curTrans.Find("bellDownPos");

            bell.transform.position = bellStartPos.position;
            bell.SetActive(true);
            bellTextures = Bg.GetComponent<BellSprites>();

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        private void OnDisable()
        {
            ms.UpdateCallBack -= this.Update;
            ms.OnDisableCallBack -= this.OnDisable;

        }
        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        void SetColor(Color color, float value)
        {
            color.a = value * 255f / 255f;
            color.r = 1;
            color.g = 1;
            color.b = 1;
        }

        void Update()
        {
            SetColor(green, GreenSlider.value);
            GreenLightM.color = green;
            SetColor(blue, BlueSlider.value);
            BlueLightM.color = blue;
            SetColor(red, RedSlider.value);
            RedLightM.color = red;
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,null,()=> { SoundManager.instance.ShowVoiceBtn(true); }));
            }
            if (talkIndex == 2)
            {
                StagePanel.gameObject.SetActive(false);
                HaloPanel.gameObject.SetActive(true);
                bell.transform.position = bellDownPos.position;
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () =>
                {
                    GreenEndPos.DOMove(GreenEndPos.position, 3f).OnComplete(() =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        GreenY.DOMove(GreenEndPos.position, 1);
                        BlueY.DOMove(BlueEndPos.position, 1);
                        RedY.DOMove(RedEndPos.position, 1).OnComplete(() => { SoundManager.instance.ShowVoiceBtn(true); });
                    });
                }));
            }
            if (talkIndex == 3)
            {
                HaloPanel.gameObject.SetActive(false);
                ColorPanel.SetActive(true);
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[2];
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, null, () => { bell.SetActive(false); }));
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
