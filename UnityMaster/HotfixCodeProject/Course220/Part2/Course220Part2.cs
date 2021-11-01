using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course220Part2
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject displayObj;
        private GameObject zouBuJi;
        private GameObject btnLeft;
        private GameObject btnRight;
        private GameObject bell;

        private GameObject btnMask;
        private List<GameObject> ShowCards = null;

        private List<GameObject> Selects = null;

        private int len = 0;

        int flag = 0;
        private int pressIndex;
        public int PressIndex
        {
            get => pressIndex; set
            {
                if (value >= 0)

                {
                    if (value > 3)
                    {
                        pressIndex = 0;
                    }
                    else
                    {
                        pressIndex = value;
                    }
                }
                else
                {
                    pressIndex = 3;
                }
            }
        }


        GameObject oldObj = null;
        GameObject oldGo = null;
        GameObject go = null;

        GameObject btnTest;


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

        private void ReStart(GameObject btnTest)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            len = 4;
            flag = 0;
            oldObj = null;
            oldGo = null;
            go = null;
            ShowCards = new List<GameObject>();
            Selects = new List<GameObject>();
            for (int i = 0; i < len; i++)
            {
                ShowCards.Add(curTrans.Find(i.ToString()).gameObject);
                Selects.Add(curTrans.Find(i + "/select").gameObject);
            }
            displayObj = curTrans.Find("displayObj").gameObject;
            displayObj.SetActive(true);
            zouBuJi = curTrans.Find("zouBuJi").gameObject;
            zouBuJi.SetActive(true);
            btnLeft = curTrans.Find("btnLeft").gameObject;
            btnLeft.SetActive(true);
            btnRight = curTrans.Find("btnRight").gameObject;
            btnRight.SetActive(true);
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            btnMask = curTrans.Find("btnMask").gameObject;
            btnMask.SetActive(true);
            GameInit();
            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, null, () => { bell.SetActive(false); btnMask.SetActive(false); }, 2));
            UpdataPanel();
        }


        private void GameInit()
        {
            displayObj.SetActive(false);
            SpineManager.instance.DoAnimation(zouBuJi, "qbhk3", false);
            for (int i = 0; i < len; i++)
            {
                Selects[i].SetActive(true);
                switch (i)
                {
                    case 0:
                        {
                            SpineManager.instance.DoAnimation(ShowCards[i], "qbyg2", false);
                        }
                        break;
                    case 2:
                        {
                            SpineManager.instance.DoAnimation(ShowCards[i], "sqb2", false);
                        }
                        break;
                    case 3:
                        {
                            SpineManager.instance.DoAnimation(ShowCards[i], "syg2", false);
                        }
                        break;
                    case 1:
                        {
                            SpineManager.instance.DoAnimation(ShowCards[i], "qbhk2", false);
                        }
                        break;
                    default:
                        break;
                }
            }


            for (int i = 0; i < len; i++)
            {
                Util.AddBtnClick(Selects[i], BtnClick);

                SpineManager.instance.DoAnimation(Selects[i], "guang3", false);

            }

            Util.AddBtnClick(btnLeft, BtnLeftClick);
            Util.AddBtnClick(btnRight, BtnRightClick);

            Util.AddBtnClick(zouBuJi, PlayZBJ);
            Util.AddBtnClick(displayObj, PlayObj);

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

        }

        private void PlayObj(GameObject obj)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, PressIndex);
            switch (PressIndex)
            {
                case 1:
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        SpineManager.instance.DoAnimation(obj, "qbyg2", false);
                    }
                    break;
                case 2:
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        SpineManager.instance.DoAnimation(obj, "sqb2", false);
                    }
                    break;
                case 3:
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                        SpineManager.instance.DoAnimation(obj, "syg2", false);
                    }
                    break;
                default:
                    break;
            }
        }

        private void PlayZBJ(GameObject obj)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            SpineManager.instance.DoAnimation(obj, "qbhk2", false);
        }

        private void BtnLeftClick(GameObject obj)
        {
            playBtnSound();
            PressIndex--;
            SwitchPlay(PressIndex);
            StopPlay();
        }

        private void BtnRightClick(GameObject obj)
        {
            playBtnSound();
            PressIndex++;
            SwitchPlay(PressIndex);
            StopPlay();
        }


        private void StopPlay()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            if (go)
            {
                switch (go.name)
                {
                    case "0":
                        {
                            SpineManager.instance.DoAnimation(go, "qbyg2", false);
                        }
                        break;
                    case "2":
                        {
                            SpineManager.instance.DoAnimation(go, "sqb2", false);
                        }
                        break;
                    case "3":
                        {
                            SpineManager.instance.DoAnimation(go, "syg2", false);
                        }
                        break;
                    case "1":
                        {
                            SpineManager.instance.DoAnimation(go, "qbhk2", false);
                        }
                        break;
                    default:
                        break;
                }
            }

        }

        private void SwitchPlay(int playIndex)
        {
            if (playIndex == 0)
            {
                zouBuJi.SetActive(true);
                displayObj.SetActive(false);
            }
            else
            {
                zouBuJi.SetActive(false);
                displayObj.SetActive(true);
            }
            switch (playIndex)
            {
                case 0:
                    {
                        SpineManager.instance.DoAnimation(zouBuJi, "qbhk3", false);
                    }
                    break;
                case 1:
                    {
                        SpineManager.instance.DoAnimation(displayObj, "qbyg3", false);
                    }
                    break;
                case 2:
                    {
                        SpineManager.instance.DoAnimation(displayObj, "sqb3", false);
                    }
                    break;
                case 3:
                    {
                        SpineManager.instance.DoAnimation(displayObj, "syg3", false);
                    }
                    break;
                default:
                    break;
            }
            UpdataPanel();
        }




        private void BtnClick(GameObject obj)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            if (oldObj)
            {
                oldGo = oldObj.transform.parent.gameObject;

                switch (oldGo.name)
                {
                    case "0":
                        {
                            SpineManager.instance.DoAnimation(oldGo, "qbyg2", false);
                        }
                        break;
                    case "2":
                        {
                            SpineManager.instance.DoAnimation(oldGo, "sqb2", false);
                        }
                        break;
                    case "3":
                        {
                            SpineManager.instance.DoAnimation(oldGo, "syg2", false);
                        }
                        break;
                    case "1":
                        {
                            SpineManager.instance.DoAnimation(oldGo, "qbhk2", false);
                        }
                        break;
                    default:
                        break;
                }

            }
            oldObj = obj;
            go = obj.transform.parent.gameObject;

            if (int.Parse(go.name) != PressIndex)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, Random.Range(8, 11));
                SpineManager.instance.DoAnimation(oldObj, "guang", false, () => { SpineManager.instance.DoAnimation(oldObj, "guang3", false); });
            }
            else
            {
                if ((flag & (1 << int.Parse(go.name))) == 0)
                {
                    flag += 1 << int.Parse(go.name);
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, PressIndex + 4);
            }

            switch (go.name)
            {
                case "0":
                    {
                        SpineManager.instance.DoAnimation(go, "qbyg1", int.Parse(go.name) == PressIndex);
                    }
                    break;
                case "2":
                    {
                        SpineManager.instance.DoAnimation(go, "sqb1", int.Parse(go.name) == PressIndex);
                    }
                    break;
                case "3":
                    {
                        SpineManager.instance.DoAnimation(go, "syg1", int.Parse(go.name) == PressIndex);
                    }
                    break;
                case "1":
                    {
                        SpineManager.instance.DoAnimation(go, "qbhk1", int.Parse(go.name) == PressIndex);
                    }
                    break;
                default:
                    break;
            }
            UpdataPanel();
        }


        void UpdataPanel()
        {
            for (int i = 0; i < len; i++)
            {
                if (PressIndex == i)
                {
                    if ((flag & (1 << i)) > 0)
                    {
                        SpineManager.instance.DoAnimation(Selects[i], "guang2", false);

                    }
                }
                else
                {
                    SpineManager.instance.DoAnimation(Selects[i], "guang3", false);
                }
            }
        }


        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                SpineManager.instance.DoAnimation(bell, "DAIJI");
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

            if (method_2 != null)
            {
                method_2();
            }
        }

        void playBtnSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        void TalkClick()
        {
            playBtnSound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }
            talkIndex++;
        }


    }
}
