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
    public class CourseShadowPart1
    {
        GameObject[] lightBtn, lightBtnSpine, lightSpinePlaster,lampSpine, startGame, ballImage;
        GameObject mask, sheild, npc, light_1;

        enum LIGHT_SPINE
        {
            bright = 0, dark = 1, grey = 2, line = 3, shadow = 4
        }
        string[] L_1_ball;
        string[] L_1_cuboid;
        string[] L_2_ball;
        string[] L_2_cuboid;
        string[] L_3_ball;
        string[] L_3_cuboid;
       
        enum BUTTON_SPINE
        {
            Light1 = 0, Light2 = 1, Light3 =2
        }
        string[] lightSpine;

        int btnIndex;
        int clickIndex;
        string[] str;
        List<int> clickCount;
        float waitTime;
        MonoBehaviour mono;
        Dictionary<string, string[]> lightObjDic;
       
        GameObject curGo;      
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            
            lightBtn = GetChildren(curTrans.Find("LightButton").gameObject);
            startGame = GetChildren(curTrans.Find("StartGame").gameObject);
            lightBtnSpine = GetChildren(curTrans.Find("LightBtnSpine").gameObject);
            lightSpinePlaster = GetChildren(curTrans.Find("LightSpine").gameObject);
            lampSpine = GetChildren(curTrans.Find("LampSpine").gameObject);
            ballImage = GetChildren(curTrans.Find("ball").gameObject);
            
            light_1 = curTrans.Find("StartGame/L_1/L_1_bg/light_1").gameObject;
            mask = curTrans.Find("Mask").gameObject;
            sheild = curTrans.Find("sheild").gameObject;
            npc = curTrans.Find("NPC").gameObject;
            mono = curTrans.GetComponent<MonoBehaviour>();
            lightObjDic = new Dictionary<string, string[]>();
            waitTime = 0.5f;
            clickCount = new List<int>();

            L_1_ball = new string[] { "L_1_ball_bright", "L_1_ball_dark", "L_1_ball_grey", "L1_b_sl", "L1_b_s_0", "idle"};
            L_1_cuboid = new string[] { "L1_c_b", "L1_c_d", "L1_c_g", "L1_cc_sl", "L1_c_s_0" };
            L_2_ball = new string[] { "L_2_ball_bright", "L_2_ball_dark", "L_2_ball_grey", "L2_ball_sl", "L2_b_s_0", "idle" };
            L_2_cuboid = new string[] { "L2_c_b", "L2_c_d", "L2_c_g", "L2_cc_sl", "L2_c_s_0" };
            L_3_ball = new string[] { "L_3_ball_bright", "L_3_ball_dark", "L_3_ball_grey", "L3_ball_sl", "L3_b_s_0", "idle" };
            L_3_cuboid = new string[] { "L3_c_b", "L3_c_d", "L3_c_g", "L3_cc_sl", "L3_c_s_0" };
            lightSpine = new string[] { "light_1", "light_2", "light_3" };

            mask.SetActive(true);
            //AtlasAsset atlas = lightSpinePlaster[0].transform.GetComponent<SkeletonGraphic>().skeletonDataAsset.atlasAssets[0];
            //Shader curlightShader = atlas.materials[0].shader;
            //SpineManager.instance.CreateRegionAttachmentByTexture(atlas, "L1_c_b", curGo, lightSpinePlaster[0].gameObject, curlightShader);
            Util.AddBtnClick(mask, Init);
            
        }       
       
        void Init(GameObject btn)
        {
            npc.SetActive(true);
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 12, null, () =>
            {
                npc.SetActive(false);
            });
            lightObjDic.Add("L_1_ball", L_1_ball);
            lightObjDic.Add("L_1_cuboid", L_1_cuboid);
            lightObjDic.Add("L_2_ball", L_2_ball);
            lightObjDic.Add("L_2_cuboid", L_2_cuboid);
            lightObjDic.Add("L_3_ball", L_3_ball);
            lightObjDic.Add("L_3_cuboid", L_3_cuboid);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            btn.SetActive(false);
            ButtonAnimation(lightBtn[0]);

            
            //AddClickEvent();
        }

        void AddClickEvent()
        {
            for (int i = 0; i < lightBtn.Length; i++)
            {
                Util.AddBtnClick(lightBtn[i], ButtonAnimation);
            }
        }

        void ButtonAnimation(GameObject btn)
        {
            Debug.Log(btn.name);
            
            if (clickIndex < 1)
            {
                sheild.SetActive(true);
            }
            string[] lightBtnName = btn.name.Split('_');
            int index = Convert.ToInt32(lightBtnName[1]) - 1;

            btnIndex = 0;
            clickIndex++;
            clickCount.Clear();
            Debug.Log(222);
            //Debug.Log(SpineManager.instance.GetAnimationName(lightSpinePlaster[index]));
            for (int i = 0; i < ballImage.Length; i++)
            {                
                if (i != index)
                {
                    ballImage[i].SetActive(false);
                }
                else
                {
                    ballImage[i].SetActive(true);
                }
            }
            SpineManager.instance.DoAnimation(lightSpinePlaster[0], "idle", false);
            SpineManager.instance.DoAnimation(lightSpinePlaster[1], "idle", false);
            SpineManager.instance.DoAnimation(lightSpinePlaster[2], "idle", false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

            Debug.Log(111);
            if (clickIndex != 1)
            {
                light_1.SetActive(false);
                lightBtn[index].SetActive(false);
                lightBtnSpine[index].SetActive(true);

                for (int i = 0; i < lampSpine.Length; i++)
                {
                    if (i == index)
                    {
                        lampSpine[i].SetActive(true);
                    }
                    else
                    {
                        lampSpine[i].SetActive(false);
                    }
                }
                
                SpineManager.instance.DoAnimation(lampSpine[index], "animation", false);
                SpineManager.instance.DoAnimation(lightBtnSpine[index], lightSpine[index], false, () =>
                {
                    lightBtnSpine[index].SetActive(false);
                    lightBtn[index].SetActive(true);
                    mono.StartCoroutine(WaitTime(waitTime));
                });
            }           

            for (int j = 0; j < startGame.Length; j++)
            {
                if (index != j)
                {
                    startGame[j].SetActive(false);
                    lightSpinePlaster[j].SetActive(false);
                }
                else
                {                    
                    startGame[j].SetActive(true);
                    lightSpinePlaster[j].SetActive(true);

                    ShadowAnimation(startGame[j].transform.GetChild(1).gameObject);
                    ShadowAnimation(startGame[j].transform.GetChild(2).gameObject);                                      
                }
            }
        }

        void ShadowAnimation(GameObject father)
        {
            for (int i = 0; i < father.transform.childCount; i++)
            {
                if (father.transform.GetChild(i).childCount == 0)
                {
                    father.transform.GetChild(i).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
                }
                
                Debug.Log(father.transform.GetChild(i));
                Util.AddBtnClick(father.transform.GetChild(i).gameObject, PlayShadowAnimation);

                if (father.transform.GetChild(i).name == "L_3_cuboid_shadow")
                {
                    Util.AddBtnClick(father.transform.GetChild(i).GetChild(0).gameObject, PlayShadowAnimation);
                }
            }
        }
       
        void PlayShadowAnimation(GameObject btn)
        {
            string[] lightBtnName = btn.name.Split('_');
            int index = Convert.ToInt32(lightBtnName[1]) - 1;

            if (btn.name == "L_3_cuboid_shadow_1")
            {
                str = lightObjDic[btn.transform.parent.parent.name];
            }
            else
            {
                str = lightObjDic[btn.transform.parent.name];
            }

            int type =(int)((LIGHT_SPINE)Enum.Parse(typeof(LIGHT_SPINE), lightBtnName[3]));
            string[] lightBtnPrentName = btn.transform.parent.name.Split('_');
            float waitTime = 0f;

            bool isAdd = false;

            if (lightBtnPrentName[2] == "ball")
            {
                type += 5;
            }
            if (clickCount.Count > 0)
            {
                for (int i = 0; i < clickCount.Count; i++)
                {
                    if (type == clickCount[i])
                    {
                        isAdd = true;
                    }
                }
            }

            if (!isAdd)
            {
                btnIndex++;
            }

            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            Debug.Log(type + "1");
            if (clickIndex == 1)
            {
                waitTime = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, type, false);
                clickCount.Add(type);         
            }
            else
            {
                clickCount.Add(type);
            }

            
            if (lightBtnPrentName[2] == "ball")
            {
                type -= 5;
            }
            ballImage[index].SetActive(false);
            SpineManager.instance.DoAnimation(lightSpinePlaster[index], str[type], true);
            //SpineManager.instance.DoAnimation(lightSpinePlaster[index], str[type], true);
            Debug.Log(btn + " " + lightSpinePlaster[index]+ " "+ str[type]);
            Debug.Log(btnIndex + " ," + clickIndex);
            //mono.StartCoroutine(WaitTime(waitTime));

            if (btnIndex == 10 && clickIndex == 1)
            {                
                sheild.SetActive(false);
                SoundManager.instance.ShowVoiceBtn(true);
                SoundManager.instance.SetVoiceBtnEvent(TalkMessage);
                               
            }
            //else if (btnIndex == 10 && clickIndex == 2)
            //{
            //    sheild.SetActive(false);
            //    SoundManager.instance.ShowVoiceBtn(true);
            //    SoundManager.instance.SetVoiceBtnEvent(TalkMessage);                
            //}
            //else if (btnIndex == 10 && clickIndex == 3)
            //{
            //    sheild.SetActive(false);                
            //    Util.AddBtnClick(lightBtn[0], ButtonAnimation);
            //}
            //else if(clickIndex > 3)
            //{
            //    sheild.SetActive(false);
            //}
        }

        void TalkMessage()
        {

            ballImage[clickIndex - 1].SetActive(true);
            //SpineManager.instance.DoAnimation(lightSpinePlaster[1], L_2_ball[5], false);
            //SpineManager.instance.DoAnimation(lightSpinePlaster[2], L_3_ball[5], false);
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            if (clickIndex == 1)
            {
                SpineManager.instance.DoAnimation(lightSpinePlaster[0], "idle", false);
                SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 10, null, AddClickEvent);                
            }
            //else if (clickIndex == 2)
            //{
            //    SpineManager.instance.DoAnimation(lightSpinePlaster[1], "idle", false);
            //    SoundManager.instance.Speaking(npc, "talk2", SoundManager.SoundType.VOICE, 11, null, () =>
            //    {
            //        Util.AddBtnClick(lightBtn[2], ButtonAnimation);
            //    });
            //}            
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

        IEnumerator WaitTime(float time,Action action = null)
        {
            SoundManager.instance.sheildGo.SetActive(true);
            yield return new WaitForSeconds(time);
            //action();
            SoundManager.instance.sheildGo.SetActive(false);
            mono.StopCoroutine(WaitTime(waitTime));
        }
    }
}
