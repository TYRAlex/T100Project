using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course737Part5
    {

        #region 通用变量
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;
        #endregion

        #region 游戏变量
        int gameIndex = 0;

        GameObject car;
        GameObject carAnimation;
        GameObject mask;

        Image bgImage;
        RawImage carRaw;
        #endregion

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

            LoadGame();
            GameInit();
            GameStart();
        }

        void LoadGame()
        {
            car = curTrans.Find("click/car").gameObject;
            carAnimation = curTrans.Find("Bg/background/bigCar").gameObject;
            mask = curTrans.Find("mask").gameObject;

            bgImage = curTrans.Find("Bg/background").GetImage();
            carRaw = curTrans.Find("Bg/background/carImage").GetRawImage();

            Util.AddBtnClick(car.transform.parent.gameObject, ClickButton);
        }

        private void GameInit()
        {
            talkIndex = 1;
            gameIndex = 0;

            car.GetComponent<SkeletonGraphic>().Initialize(true);
            carAnimation.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(carAnimation, "1");

            carAnimation.gameObject.SetActive(true);
            carRaw.gameObject.SetActive(false);

            SpineManager.instance.DoAnimation(car, "w-run");

            mask.SetActive(true);

            bgImage.sprite = bgImage.GetComponent<BellSprites>().sprites[0];
        }

        void GameStart()
        {
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => {isPlaying = false; }));

        }

        void ClickButton(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

            SpineManager.instance.DoAnimation(car, "w", false, () =>
            {
                SpineManager.instance.DoAnimation(car, "w-run");

                switch(gameIndex)
                {
                    case 0:
                        mask.SetActive(false);

                        mono.StartCoroutine(WaitFor(4f, ()=>
                        {
                            bgImage.sprite = bgImage.GetComponent<BellSprites>().sprites[2];

                            mono.StartCoroutine(CarAnimation()); 
                        }));

                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                        {
                            ++gameIndex;
                            isPlaying = false;
                        }));
                        break;

                    case 1:
                        SpineManager.instance.DoAnimation(carAnimation, "2", false);
                        bgImage.sprite = bgImage.GetComponent<BellSprites>().sprites[1];

                        mono.StartCoroutine(WaitFor(5f, ()=>
                        {
                            carAnimation.SetActive(false);
                            carRaw.gameObject.SetActive(true);

                            carRaw.texture = carRaw.GetComponent<BellSprites>().texture[0];
                            mono.StartCoroutine(WaitFor(5f, () =>
                            {
                                carRaw.texture = carRaw.GetComponent<BellSprites>().texture[1];

                                mono.StartCoroutine(WaitFor(5f, () =>
                                {
                                    carRaw.texture = carRaw.GetComponent<BellSprites>().texture[2];
                                }));
                            }));
                        }));

                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, ()=>
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                        }));

                        break;
                }

            });
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3));
            }

            talkIndex++;
        }

        IEnumerator CarAnimation()
        {
            int num = 4;

            while(--num >= 0)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                float _time = SpineManager.instance.DoAnimation(carAnimation, "animation", false);

                yield return new WaitForSeconds(_time);

                SpineManager.instance.DoAnimation(carAnimation, "animation2", false);

                yield return new WaitForSeconds(1f);
            }

            
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

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

    }
}
