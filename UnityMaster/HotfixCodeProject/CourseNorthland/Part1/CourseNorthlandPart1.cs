using CourseNorthlandPart;
using LuaInterface;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ILFramework.HotClass
{
    public enum AnimationState
    {
        STATE,
        PLAY,
        GAMEOVER
    }

    public class CourseNorthlandPart1
    {
        GameObject curGo;
        public static int speakIndex;
        public static GameObject mask;
        List<GameObject> btns;
        List<GameObject> randomBtns;
        GameObject btnObjs;
        Dictionary<int, Sprite> datas;
        Dictionary<int, string[]> orderDatas;
        public static int index;
        public  bool operation;
        public float max;
        public float curTime;
        public bool isOver;
        public SkeletonAnimation skeleton;
        // public GameObject btnStart;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
           
            // btnStart = curGo.transform.Find("startBtn").gameObject;
            // btnStart.SetActive(true);
            // Util.AddBtnClick(btnStart, BtnStart);
            //Thread.Sleep(10000);
            mask = curTrans.Find("Camera/mask").gameObject;
            btnObjs = curTrans.Find("Camera/btn").gameObject;
            mask.SetActive(false);
            OnInit();
            curGo.AddComponent<MesManager>();
            curGo.AddComponent<ClassCopyManager>();
            AddListener();
            
            skeleton = curTrans.transform.Find("Camera/lightAnimation").GetComponent<SkeletonAnimation>();
            Util.AddBtnClick(curTrans.transform.Find("btn").gameObject, BtnClick);
            SoundManager.instance.BgSoundPart2();

            BtnStart();

        }
        public void BtnStart()
        {
            // Util.AddBtnClick(btnStart, o=> { });
            // btnStart.SetActive(false);
            curGo.transform.Find("Camera").localScale = Vector3.one;
            curGo.transform.Find("tiantian").gameObject.SetActive(true);
            StartGame();
        }
        public void BtnClick(GameObject btn)
        {
            skeleton.AnimationName = null;
        }
        public void Update()
        {
            if (isOver) return;
            curTime  += Time.deltaTime;
            if(curTime >= max)
            {
                curTime = 0f;
                if(!operation)
                {
                    RandomBtn();
                }
                else
                {
                    operation = true;
                }
            }
            if (operation)
            {
                curTime = 0;
                operation = false;
            }
        }
        public void RandomBtn()
        {
            if(randomBtns.Count == 0)
            {
                GameObject[] games = new GameObject[btns.Count];
                btns.CopyTo(games);
                randomBtns = games.ToList();
            }
            int num = Util.Random(0, randomBtns.Count);
            GameObject go = randomBtns[num];
            randomBtns.RemoveAt(num);
            PlayAnimationDuring(go, "erro", 0.5f);
        }
        public void PlayAnimationDuring(GameObject gameobject, string animationName, float scale)
        {
            SkeletonAnimation ske = gameobject.GetComponent<SkeletonAnimation>();
            ske.AnimationState.SetAnimation(0, animationName, false);
            var track = ske.AnimationState.GetCurrent(0);
            track.TimeScale = scale;
        }
        public void OnInit()
        {
            //if (btns.Count > 0 && datas.Count > 0 && orderDatas.Count > 0)
            //{
            //    btns.Clear();
            //    //randomBtns.Clear();
            //    datas.Clear();
            //    orderDatas.Clear();
            //}
            curGo.transform.Find("Camera").localScale = Vector3.zero;
            curGo.transform.Find("tiantian").gameObject.SetActive(false);

            speakIndex = 0;
            index = 0;
            operation = false;
            max = 5.0f;
            curTime = 0.0f;
            isOver = false;
            orderDatas = new Dictionary<int, string[]>()
            {
                { 0 ,new string[]{ "yj","2","1"} },
                { 1 ,new string[]{ "t","1" ,"1"} },
                { 2 ,new string[]{ "nh","3","0"} },
                { 3 ,new string[]{ "cb","5","5"} },
                { 4 ,new string[]{ "nt","7","3"} },
                { 5 ,new string[]{ "jz","8" ,"4"} },
                { 6 ,new string[]{ "st","4" ,"2"} }
            };
            Transform tran = curGo.transform.Find("data");
            datas = new Dictionary<int, Sprite>();
            for (int i = 0;i < tran.childCount;i++)
            {
                datas.Add(i, tran.GetChild(i).GetComponent<SpriteRenderer>().sprite);
            }
            BtnState(btnObjs);
        }
        public void AddListener()
        {
            MesManager.instance.Register("CourseNorthlandPart1",(int)AnimationState.STATE,Playing);
            MesManager.instance.Register("CourseNorthlandPart1",(int)AnimationState.PLAY,Playing);
            MesManager.instance.Register("CourseNorthlandPart1",(int)AnimationState.GAMEOVER, GameOver);
        }
        private void Playing(object[] param)
        {
            speakIndex = int.Parse(orderDatas[index][1]);
            AddAnimation("Speak,Breath");
        }
        public void GameOver(object[] param)
        {
            isOver = true;
            speakIndex = 9;
            AnimationClass animation = new AnimationClass("","" ,"GameOver,Breath");
            object[] obj = new object[] { "","", "GameOver,Breath" };
            animation.AddTask_Run(obj, animation);
            index++;            
        }
        public void StartGame()
        {
            speakIndex = 0;
            AddAnimation("StateSpeak");
        }
        public void AddAnimation(string functions)
        {
            string lightName = orderDatas[index][0];
            string nextLightName = "";
            if(index < orderDatas.Count - 1)
            {
                nextLightName = orderDatas[index+1][0];
            }
            AnimationClass animation = new AnimationClass(lightName, nextLightName, functions);
            object[] obj = new object[] { lightName, nextLightName, functions };
            animation.AddTask_Run(obj, animation);
        }
        public void BtnState(GameObject btn)
        {
            btns = new List<GameObject>();
            int count = btn.transform.childCount;
            for(int i = 0;i<count;i++)
            {
                GameObject child = btn.transform.GetChild(i).gameObject;
                btns.Add(child);
                child.GetComponent<ILObject3DAction>().index = i;
                child.GetComponent<ILObject3DAction>().OnMouseDownLua = OnMouseDown;
                PushTexture(child, datas[i]);
            }
            GameObject[] games = new GameObject[btns.Count];
            btns.CopyTo(games);
            randomBtns = games.ToList();
        }

        private void OnMouseDown(int _index)
        {
            operation = true;
            if (index < 7)
            {
                if (int.Parse(orderDatas[index][2]) == _index)
                {
                    //��ȷ
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                    SpineManager.instance.DoAnimation(btns[_index], "right", false);
                    speakIndex = Util.Random(10, 13);
                    AddAnimation("Speak,Breath");
                    index++;
                }
                else
                {
                    //����
                    speakIndex = Util.Random(13, 16);
                    AddAnimation("Speak,Breath");
                    SpineManager.instance.DoAnimation(btns[_index], "erro", false);
                }
            }
            
        }

        public void PushTexture(GameObject btn,Sprite sprite)
        {
            SkeletonAnimation animation = btn.GetComponent<SkeletonAnimation>();
            Shader shader = animation.gameObject.GetComponent<MeshRenderer>().material.shader;
            SpineManager.instance.CreateRegionAttachmentByTexture(animation,"yx",sprite,shader);
            SpineManager.instance.PlayAnimationState(animation, "erro");
        }

    }
}
