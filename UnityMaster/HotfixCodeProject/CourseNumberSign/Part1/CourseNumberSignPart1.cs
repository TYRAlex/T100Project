using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseNumberSignPart1
    {
        enum ANI_SPINE
        {
            NumberVIII, Number0, Number1, Number2, Number3, Number4, Number5, Number6, Number7, Number8, Number9,
            NumberI, NumberII, NumberIII, NumberIV, NumberV, NumberVI, NumberVII, NumberIX, NumberX
        }
        string[] aniSpine;

        enum UI_SPINE
        {
            UIVIII, UI0, UI1, UI2, UI3, UI4, UI5, UI6, UI7, UI8, UI9,
            UII, UIII, UIIII, UIIV, UIV, UIVI, UIVII, UIIX, UIX
        }
        string[] uiSpine;

        enum FINISH_SPINE
        {
            Text1 = 0, Text2 = 1, Text3 = 2, Light = 3, Star = 4
        }
        string[] finishSpine;

        GameObject curGo;
        GameObject npc, mask, aniNumSpine, uiNumSpine, tempUINum, tempAniNum, finishScene;

        GameObject[] aniNum, uiNum, lightNum, winSpine;

        int countNum;
        string tempStr;
        bool isMatch;
        MonoBehaviour mono;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            mask = curTrans.Find("Mask").gameObject;
            npc = curTrans.Find("NPC").gameObject;
            aniNumSpine = curTrans.Find("StartGame/AniSpine").gameObject;
            uiNumSpine = curTrans.Find("UISpine").gameObject;
            finishScene = curTrans.Find("finshScene").gameObject;

            aniNum = GetChildren(curTrans.Find("StartGame/ImgNum").gameObject);
            lightNum = GetChildren(curTrans.Find("StartGame/lightNum").gameObject);
            uiNum = GetChildren(curTrans.Find("UIFrame").gameObject);
            winSpine = GetChildren(finishScene);

            tempStr = "";
            tempUINum = null;
            isMatch = false;
            mono = curGo.GetComponent<MonoBehaviour>();
            finishSpine = new string[] { "text_1", "text_2", "text_3", "light", "star" };
            aniSpine = new string[] { "VIII", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                        "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"};
            uiSpine = new string[] { "ui_VIII", "ui_0", "ui_1", "ui_2", "ui_3", "ui_4", "ui_5", "ui_6", "ui_7", "ui_8", "ui_9",
                                        "ui_I", "ui_II", "ui_III", "ui_IV", "ui_V", "ui_VI", "ui_VII", "ui_VIII", "ui_IX", "ui_X"};

            // before game
            for (int i = 0; i < winSpine.Length; i++)
            {
                SpineManager.instance.PlayAnimationDuring(winSpine[i], finishSpine[(int)FINISH_SPINE.Light], "0|0");
            }
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                npc.SetActive(true);
                SpineManager.instance.DoAnimation(npc, "breath", true);
                Util.AddBtnClick(mask, StartGame);
            });
        }

        void StartGame(GameObject btn)
        {
            npc.SetActive(false);
            btn.SetActive(false);
            uiNumSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.bgmSource.volume = 0.3f;

            for (int i = 0; i < uiNum.Length; i++)
            {
                Util.AddBtnClick(uiNum[i], ClickUINumber);
            }
            for (int i = 0; i < aniNum.Length; i++)
            {
                Util.AddBtnClick(aniNum[i], ClickAniNumber);
            }
        }

        void ClickUINumber(GameObject num)
        {
            tempStr = num.name;
            string aniStr = "ui_" + (tempStr.Split('_'))[0];
            SpineManager.instance.PlayAnimationDuring(aniNumSpine, (tempStr.Split('_'))[0], "0|0");
            if (isMatch)
            {
                string arr = ((tempUINum.name).Split('_'))[1];
                int index = Convert.ToInt32(arr);
                tempUINum.SetActive(false);
                tempAniNum.SetActive(true);
                lightNum[index].SetActive(true);
                mono.StartCoroutine(FadeImage(lightNum[index], 0));
                countNum++;              
            }
            else
            {
                if (tempUINum != null && tempUINum != num)
                {
                    tempUINum.SetActive(true);
                }
            }
            
            
            num.SetActive(false);
            uiNumSpine.transform.position = new Vector3(num.transform.position.x, num.transform.position.y - 100f, 0);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            SpineManager.instance.DoAnimation(uiNumSpine, aniStr, true);
            tempUINum = num;
            isMatch = false;

        }

        void ClickAniNumber(GameObject image)
        {
            string imgStr = image.name;
            string arr =imgStr.Split('_')[0];
            SpineManager.instance.PlayAnimationDuring(aniNumSpine, arr, "0|0");
            Debug.Log(imgStr + "___" + tempStr);
            SoundManager.instance.sheildGo.SetActive(true);
            if (imgStr == tempStr)
            {
                image.SetActive(false);
                tempAniNum = image;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                int voiceIndex = UnityEngine.Random.Range(1, 4);
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, voiceIndex, null, () =>
                {
                    SoundManager.instance.sheildGo.SetActive(false);
                });
                SpineManager.instance.DoAnimation(aniNumSpine, arr, true);
                isMatch = true;

                Debug.Log("countNum:" + countNum);
                if (countNum == 19)
                {
                    string str = imgStr.Split('_')[1];
                    int index = Convert.ToInt32(str);

                    SpineManager.instance.PlayAnimationDuring(aniNumSpine, arr, "0|0");
                    mono.StartCoroutine(WaitTime(2f, index));
                }
            }
            else
            {
                if (isMatch)
                {
                    tempAniNum.SetActive(true);
                }
                isMatch = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 4, null, () =>
                 {
                     SoundManager.instance.sheildGo.SetActive(false);
                 });
            }
        }

        void FinishScene()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);            

            SpineManager.instance.DoAnimation(winSpine[1], finishSpine[(int)FINISH_SPINE.Text1], false, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                SpineManager.instance.DoAnimation(winSpine[0], finishSpine[(int)FINISH_SPINE.Light], false);
                SpineManager.instance.DoAnimation(winSpine[2], finishSpine[(int)FINISH_SPINE.Star], true);
            });

        }

        IEnumerator WaitTime(float time, int index)
        {
            yield return new WaitForSeconds(time);
            uiNumSpine.SetActive(false);
            lightNum[index].SetActive(true);
            mono.StartCoroutine(FadeImage(lightNum[index], 0));
            FinishScene();
            mono.StopCoroutine(WaitTime(time, index));
        }

        IEnumerator FadeImage(GameObject go, int alpha)
        {
            while (alpha < 255)
            {
                alpha = alpha + 2;
                go.GetComponent<Image>().color = new Color(1, 1, 1, alpha / 255f);
                yield return null;
            }
            go.GetComponent<Image>().color = new Color(1, 1, 1, 1);

            mono.StopCoroutine(FadeImage(go, alpha));
        }

        GameObject[] GetChildren(GameObject father)
        {
            GameObject[] children = new GameObject[father.transform.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = father.transform.GetChild(i).gameObject;
            }
            return children;
        }
    }
}
