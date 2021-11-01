using DG.Tweening;
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
    public class Course9113Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        PointerEventData evenData;

        private GameObject di;
        private GameObject max;

        private GameObject mask;
        private GameObject reSetBtn;

        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;

        private GameObject QPanel;
        private GameObject ball;
        private Transform ballStartPos;
        private Rigidbody2D ballRgb;
        private GameObject Car;
        private Transform CarStartPos;
        private Transform firePos;
        private Rigidbody2D carRgb;
        private GameObject Dong;
        private GameObject leftBtn;
        private GameObject rightBtn;
        private GameObject battingBtn;

        private GameObject successPanelBg;
        private GameObject successPanel;


        private MonoScripts ms;

        private EventDispatcher eventDispatcherDong;
        private EventDispatcher eventDispatcherBg;
        private float speed;

        bool isPressLeft;
        bool isPressRight;
        bool isPressBatting;

        bool stateLeft;
        bool stateRight;
        bool stateForward;

        double offset;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            evenData = new PointerEventData(EventSystem.current);
            di = curTrans.Find("di").gameObject;
            di.SetActive(true);
            max = curTrans.Find("di/Max").gameObject;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            ms = Bg.GetComponent<MonoScripts>();
            eventDispatcherBg = Bg.GetComponent<EventDispatcher>();
            ms.UpdateCallBack += this.Update;
            ms.OnDisableCallBack += this.OnDisable;
            eventDispatcherBg.CollisionEnter2D += this.OnCollisionEnter2DBg;

            mask = curTrans.Find("mask").gameObject;
            successPanelBg = curTrans.Find("mask/successPanelBg").gameObject;
            successPanel = curTrans.Find("mask/successPanelBg/successPanel").gameObject;
            successPanelBg.SetActive(true);
            reSetBtn = curTrans.Find("mask/reSetBtn").gameObject;
            reSetBtn.SetActive(false);
            Util.AddBtnClick(reSetBtn, OnClickReSetBtn);
            mask.SetActive(false);

            QPanel = curTrans.Find("QPanel").gameObject;
            QPanel.SetActive(false);
            ball = curTrans.Find("QPanel/ball").gameObject;
            ballStartPos = curTrans.Find("QPanel/ballStartPos");
            ballRgb = ball.GetComponent<Rigidbody2D>();

            Car = curTrans.Find("QPanel/Car").gameObject;
            CarStartPos = curTrans.Find("QPanel/CarStartPos");
            firePos = Car.transform.Find("FirePos");
            carRgb = Car.GetComponent<Rigidbody2D>();
            Car.transform.position = CarStartPos.position;
            Dong = curTrans.Find("QPanel/Dong").gameObject;
            eventDispatcherDong = Dong.GetComponent<EventDispatcher>();
            eventDispatcherDong.TriggerEnter2D += this.DongTriggerEnter2DDong;

            leftBtn = curTrans.Find("QPanel/leftBtn").gameObject;
            rightBtn = curTrans.Find("QPanel/rightBtn").gameObject;
            battingBtn = curTrans.Find("QPanel/battingBtn").gameObject;
            speed = 20;
            isPressLeft = false;
            isPressRight = false;
            isPressBatting = false;

            stateLeft = false;
            stateRight = false;
            stateForward = false;
            offset = Math.Round(Screen.height / 1080f, 2);

            UIEventListener.Get(battingBtn).onDown = this.OnClickBattingBtnDown;
            UIEventListener.Get(battingBtn).onUp = this.OnClickBattingBtnUp;
            UIEventListener.Get(leftBtn).onDown = this.OnClickLeftBtnDown;
            UIEventListener.Get(leftBtn).onUp = this.OnClickLeftBtnUp;
            UIEventListener.Get(rightBtn).onDown = this.OnClickRightBtnDown;
            UIEventListener.Get(rightBtn).onUp = this.OnClickRightBtnUp;


            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            initObj();
            GameStart();
        }

        private void DongTriggerEnter2DDong(Collider2D other, int time)
        {
            if (other.name == "ball")
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 4);
               
                    other.gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(Dong, "qiu5", false, () =>
                {
                    mask.SetActive(true);
                    successPanelBg.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                    SpineManager.instance.DoAnimation(successPanelBg, "animation", false, () => { SpineManager.instance.DoAnimation(successPanelBg, "animation2", false); });
                    SpineManager.instance.DoAnimation(successPanel, "animation3", false, () => { SpineManager.instance.DoAnimation(successPanelBg, "animation4", false, () => { successPanelBg.SetActive(false); reSetBtn.SetActive(true); SpineManager.instance.DoAnimation(reSetBtn, "ck", false); }); });
                });
            }
        }
        int timeNum = 0;
        private void OnCollisionEnter2DBg(Collision2D c, int time)
        {
            if (c.gameObject.name == "ball")
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, timeNum);
                timeNum++;
                if (timeNum >= 2)
                {
                    timeNum = 0;
                }
                c.gameObject.SetActive(false);
                c.transform.DOMove(ballStartPos.position, 1).OnComplete(() => { c.gameObject.SetActive(true); SpineManager.instance.DoAnimation(ball, "qiu4", false); });
            }
        }

        private void OnClickBattingBtnUp(PointerEventData eventData)
        {
            isPressBatting = false;
            SpineManager.instance.DoAnimation(battingBtn, "kg6", false);
        }

        private void OnClickBattingBtnDown(PointerEventData eventData)
        {
            isPressBatting = true;
            SpineManager.instance.DoAnimation(battingBtn, "kg7", false);
            SpineManager.instance.DoAnimation(Car, "che3", false, () =>
            {
                float temX = 0;
                float temY = 0;
                if (firePos.localPosition.x > ball.transform.localPosition.x)
                {
                    temX = firePos.localPosition.x - ball.transform.localPosition.x;
                }
                else
                {
                    temX = ball.transform.localPosition.x - firePos.localPosition.x;
                }

                if (firePos.localPosition.y > ball.transform.localPosition.y)
                {
                    temY = firePos.localPosition.y - ball.transform.localPosition.y;
                }
                else
                {
                    temY = ball.transform.localPosition.y - firePos.localPosition.y;
                }

                //Debug.Log("@------loacl打到小球" + Vector3.Distance(firePos.localPosition, ball.transform.localPosition));
                //Debug.Log("@------打到小球" + Vector3.Distance(firePos.position, ball.transform.position));
                //Debug.Log("@temX--------------:" + temX);

                //Debug.Log("@ temY--------------:" + temY);


                //Debug.Log(offset + "@ 51* offset--------------:" + 51 * offset);
                if (Vector3.Distance(firePos.position, ball.transform.position) <= 51 * offset + 1)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    Debug.Log("打到小球" + Car.transform.up);
                    SpineManager.instance.DoAnimation(ball, "qiu2", true);
                    ballRgb.AddForce(Car.transform.up * 500, ForceMode2D.Impulse);
                }
            });

        }
        void initObj()
        {

            ball.transform.position = ballStartPos.position;
            ball.SetActive(true);
            SpineManager.instance.DoAnimation(ball, "qiu", false);
            Car.transform.position = CarStartPos.position;
            Car.transform.rotation = Quaternion.identity;
            timeNum = 0;
        }
        private void OnClickReSetBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, "ck2", false, () =>
            {
                SpineManager.instance.DoAnimation(obj, "ck", false, () =>
                {
                    initObj();
                    obj.SetActive(false);
                    mask.SetActive(false);
                });
            });
        }


        private void OnClickLeftBtnDown(PointerEventData eventData)
        {
            isPressLeft = true;
            SpineManager.instance.DoAnimation(leftBtn, "kg5", false);
            if (!stateLeft)
            {
                stateLeft = true;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, true);
                SpineManager.instance.DoAnimation(Car, "che2", true);
            }
            Car.transform.Rotate(Vector3.forward * Time.deltaTime * speed);
        }
        private void OnClickLeftBtnUp(PointerEventData eventData)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            isPressLeft = false;
            stateLeft = false;
            if (!stateRight && !stateLeft)
            {
                SpineManager.instance.DoAnimation(Car, "che", false);
            }
            SpineManager.instance.DoAnimation(leftBtn, "kg4", false);
        }

        private void OnClickRightBtnDown(PointerEventData eventData)
        {
            isPressRight = true;
            SpineManager.instance.DoAnimation(rightBtn, "kg3", false);
            if (!stateRight)
            {
                stateRight = true;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, true);
                SpineManager.instance.DoAnimation(Car, "che2", true);
            }
            Car.transform.Rotate(Vector3.back * Time.deltaTime * speed);
        }
        private void OnClickRightBtnUp(PointerEventData eventData)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            isPressRight = false;
            stateRight = false;
            if (!stateLeft && !stateRight)
            {
                SpineManager.instance.DoAnimation(Car, "che", false);
            }

            SpineManager.instance.DoAnimation(rightBtn, "kg2", false);

        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SpineManager.instance.DoAnimation(max, "qiu", false, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                SpineManager.instance.DoAnimation(max, "daqiu", false, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, null, () =>
                    {
                        //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null, () =>
                        //{
                            di.SetActive(false);
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                            QPanel.SetActive(true);
                        //}));
                    }));
                });
            });
        }


        void Update()
        {

            if (Input.GetKey(KeyCode.A))
            {
                OnClickLeftBtnDown(evenData);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                OnClickLeftBtnUp(evenData);
            }
            if (Input.GetKey(KeyCode.D))
            {
                OnClickRightBtnDown(evenData);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                OnClickRightBtnUp(evenData);
            }

            if (isPressLeft)
            {
                OnClickLeftBtnDown(evenData);
            }
            else if (!isPressBatting && stateLeft)
            {
                OnClickLeftBtnUp(evenData);
            }
            if (isPressRight)
            {
                OnClickRightBtnDown(evenData);
            }
            else if (!isPressBatting && stateRight)
            {
                OnClickRightBtnUp(evenData);
            }


            if (isPressLeft && isPressRight)
            {
                if (!stateForward)
                {
                    stateForward = true;
                    SpineManager.instance.DoAnimation(Car, "che2", true);
                }
                Car.transform.Translate(Vector2.up * Time.deltaTime * 100, Space.Self);
            }
            else
            {
                stateForward = false;
            }

        }

        void OnDisable()
        {
            ms.UpdateCallBack -= this.Update;
            ms.OnDisableCallBack -= this.OnDisable;
            eventDispatcherBg.CollisionEnter2D -= this.OnCollisionEnter2DBg;
            eventDispatcherDong.TriggerEnter2D -= this.DongTriggerEnter2DDong;
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
            SpineManager.instance.DoAnimation(max, "daijishuohua2");
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
