using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course937Part1
    {        
        int talkIndex;
        int clickIndex;

        bool isRotated;
        bool isClicked;
        bool isRemainRotate;//是否在旋转中原地不动
        bool isDraging;

        float _angle;
        float _angleBef;

        GameObject curGo;
        GameObject Bg;
        GameObject Max;
        GameObject btnBack;
        GameObject rotateButton;
        GameObject clickButton;
        GameObject fan;
        GameObject unClickMask;
        GameObject click;
        GameObject animation;

        Transform button;
        Transform curTrans;

        BellSprites bellTextures;
        BellSprites bellBg;
        RawImage number;

        mILDrager drag;

        MonoBehaviour mono;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellBg = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            LoadGame();

            GameInit();

            GameStart();
        }

        void LoadGame()
        {
            Bg.GetComponent<RawImage>().texture = bellBg.texture[0];

            unClickMask = curTrans.Find("UnClickMask").gameObject;
            unClickMask.SetActive(false);

            btnBack = curTrans.Find("btnBack").gameObject;
            btnBack.SetActive(false);

            rotateButton = curTrans.Find("RotateButton").gameObject;
            rotateButton.SetActive(false);

            button = curTrans.Find("RotateButton/mask/Button");
            drag = button.GetComponent<mILDrager>();
            drag.SetDragCallback(RotateDragBegin, null, RotateDragEnd);

            clickButton = curTrans.Find("ClickButton").gameObject;
            clickButton.SetActive(false);

            bellTextures = clickButton.transform.Find("Time/Number").GetComponent<BellSprites>();
            number = bellTextures.GetComponent<RawImage>();
            number.texture = bellTextures.texture[0];

            click = clickButton.transform.Find("Button").gameObject;

            fan = curTrans.Find("Fan").gameObject;            
            fan.SetActive(true);

            animation = curTrans.Find("Animation").gameObject;
            animation.SetActive(false);

            Util.AddBtnClick(curTrans.Find("Fan/Fan01").gameObject, ChangeButton);
            Util.AddBtnClick(curTrans.Find("Fan/Fan02").gameObject, ChangeButton);
            Util.AddBtnClick(animation.transform.Find("Button1").gameObject, DoAnimation);
            Util.AddBtnClick(animation.transform.Find("Button2").gameObject, DoAnimation);
            Util.AddBtnClick(btnBack, ChangeButton);
            Util.AddBtnClick(click, ClickButton);
        }

        void GameInit()
        {
            talkIndex = 1;
            clickIndex = 0;

            isRotated = false;
            isClicked = false;
            isRemainRotate = false;
            isDraging = false;

            button.transform.rotation = Quaternion.Euler(0, 0, 0);

            animation.GetComponent<SkeletonGraphic>().Initialize(true);
            clickButton.GetComponent<SkeletonGraphic>().Initialize(true);
            Max.GetComponent<SkeletonGraphic>().Initialize(true);            
            fan.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(animation, "ka", false);
            SpineManager.instance.DoAnimation(fan, "jing", false);
            SpineManager.instance.DoAnimation(clickButton, "ds2", false);
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);           
            SpineManager.instance.DoAnimation(fan, "animation", true);

            Max.SetActive(true);
            unClickMask.SetActive(true);

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                Max.SetActive(false);
                unClickMask.SetActive(false);
            }));

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, true);
        }

        //切换风扇和旋转按钮的状态
        void ChangeButton(GameObject obj)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);

            switch (obj.name)
            {
                case "Fan01":
                    rotateButton.SetActive(true);
                    fan.SetActive(false);
                    btnBack.SetActive(true);
                    SoundManager.instance.ShowVoiceBtn(false);
                    Bg.GetComponent<RawImage>().texture = bellBg.texture[1];
                    isDraging = false;
                    isRemainRotate = false;
                    break;

                case "Fan02":
                    clickButton.SetActive(true);
                    clickIndex = 0;
                    fan.SetActive(false);
                    btnBack.SetActive(true);                    
                    SoundManager.instance.ShowVoiceBtn(false);
                    Bg.GetComponent<RawImage>().texture = bellBg.texture[1];
                    break;

                default:
                    clickButton.GetComponent<SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(clickButton, "ds2", false);

                    rotateButton.SetActive(false);
                    clickButton.SetActive(false);
                    fan.SetActive(true);
                    btnBack.SetActive(false);
                    Bg.GetComponent<RawImage>().texture = bellBg.texture[0];
                    number.texture = bellTextures.texture[0];
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, true);

                    if (isRotated && isClicked) SoundManager.instance.ShowVoiceBtn(true);
                    break;
            }
        }

        //点击按钮增加时长
        void ClickButton(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);

            isClicked = true;
            SpineManager.instance.DoAnimation(clickButton, "ds2z", false);
            if (++clickIndex == 7) clickIndex = 0;
            number.texture = bellTextures.texture[clickIndex];
        }

        void TalkClick()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            unClickMask.SetActive(true);

            switch (talkIndex)
            {
                case 1:
                    Max.SetActive(true);
                    fan.SetActive(false);
                    animation.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () =>
                    {
                        isRotated = false;
                        isClicked = false;
                        unClickMask.SetActive(false);
                    }));
                    break;

                case 2:
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1));
                    break;
            }

            talkIndex++;
        }

        void DoAnimation(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            SoundManager.instance.ShowVoiceBtn(false);
            unClickMask.SetActive(true);

            switch (obj.name)
            {
                case "Button1":
                    isRotated = true;

                    SpineManager.instance.DoAnimation(animation, "ds1cx", false, ()=>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

                        SpineManager.instance.DoAnimation(animation, "ds1z", false, ()=>
                        {
                            SpineManager.instance.DoAnimation(animation, "ds1z2", false);
                        });

                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, () =>
                        {
                            mono.StartCoroutine(WaitCor(1.5f, ()=>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);

                                SpineManager.instance.DoAnimation(animation, "ds1xs", false, ()=>
                                {                                    
                                    if (isRotated && isClicked) SoundManager.instance.ShowVoiceBtn(true);
                                    unClickMask.SetActive(false);
                                });
                            }));
                        }));                        
                    });
                    break;

                case "Button2":
                    isClicked = true;

                    SpineManager.instance.DoAnimation(animation, "ds2cx", false, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

                        SpineManager.instance.DoAnimation(animation, "ds2donghua", false, ()=>
                        {
                            SpineManager.instance.DoAnimation(animation, "ds2donghua2", false);
                        });

                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () =>
                        {
                            mono.StartCoroutine(WaitCor(1.5f, () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);

                                SpineManager.instance.DoAnimation(animation, "ds2xs", false, () =>
                                {
                                    if (isRotated && isClicked) SoundManager.instance.ShowVoiceBtn(true);
                                    unClickMask.SetActive(false);
                                });
                            }));
                        }));
                    });
                    break;
            }
        } 

        #region 拖拽
        void RotateDragBegin(Vector3 pos, int type, int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, true);

            isDraging = true;

            mono.StartCoroutine(FixUpdate(()=>
            {
                Vector3 _mousePos = Input.mousePosition;// 获取鼠标坐标
                _mousePos.z = 0;// 将鼠标坐标z轴和和屏幕相等

                Vector3 dic2 = _mousePos - button.position; // 鼠标到物体的方向向量

                //计算与x轴的夹角cos值
                Vector3 v3 = Vector3.Cross(Vector2.up, dic2);

                //计算与x轴的夹角的角度

                if (v3.z > 0) _angle = Vector3.Angle(Vector2.up, dic2);
                else _angle = 360 - Vector3.Angle(Vector2.up, dic2);

                //如果和上次帧的角度差不多相同则停止音乐
                if (!isRemainRotate && (System.Math.Abs(_angle - _angleBef) < 0.01f))
                {                    
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    isRemainRotate = true;
                }
                else if (isRemainRotate && (System.Math.Abs(_angle - _angleBef) >= 0.01f))
                {
                    isRemainRotate = false;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, true); 
                }

                _angleBef = _angle;//记录这一帧的angle

                button.transform.rotation = Quaternion.Euler(0, 0, _angle);
            }));
        }

        void RotateDragEnd(Vector3 pos, int type, int index, bool isMatch)
        {
            isDraging = false;
            isRemainRotate = false;
            isRotated = true;
        }
        #endregion

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker) speaker = Max;

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

        IEnumerator WaitCor(float _time,  Action method)
        {
            yield return new WaitForSeconds(_time);
            method?.Invoke();
        }

        IEnumerator FixUpdate(Action callBack)
        {
            WaitForSeconds wait = new WaitForSeconds(0.03f);

            while (isDraging)
            {
                callBack?.Invoke();
                yield return wait;
            }

            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
        }
    }
}
