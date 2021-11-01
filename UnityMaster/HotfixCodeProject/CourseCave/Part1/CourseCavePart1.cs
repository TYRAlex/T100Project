using DG.Tweening;
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
    public class CourseCavePart1
    {
        GameObject curGo;
        GameObject bgBack;
        SkeletonAnimation winSpine;
        SkeletonAnimation winSpine1;
        SkeletonAnimation winSpine2;
        SkeletonAnimation spineState;
        Dictionary<int, ChooseTransform> chooseDicts;
        Dictionary<int, List<GameObject>> movePosDicts;
        GameObject mask;
        ChooseTransform choose;
        ChooseTransform chooseClone;
        GameObject chooseParent;
        int indexMaxNum;
        int succesNum;
        MonoBehaviour mono;
        GameObject tiantian;
        int numSound;
        bool isOnSound;
        Shader curShader;
        string animation_name;
        string animation_idle;
        string slot;
        string checkname;
        GameObject spineAnimation;
        GameObject GridPos;
        int curindex;
        GameObject right;
        GameObject left;
        void Start(object o)
        {
            Debug.Log("ff1");
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Transform tran = curGo.transform.Find("3DUI");
            bgBack = tran.Find("bgBack").gameObject;
            spineAnimation = tran.Find("spineAnimation").gameObject;
            winSpine = tran.Find("spineAnimation/winSpine").GetComponent<SkeletonAnimation>();
            winSpine1 = tran.Find("spineAnimation/winSpine_1").GetComponent<SkeletonAnimation>();
            winSpine2 = tran.Find("spineAnimation/winSpine_2").GetComponent<SkeletonAnimation>();

            spineState = tran.Find("spineAnimation/spineState").GetComponent<SkeletonAnimation>();
            mask = tran.Find("mask").gameObject;
            chooseParent = tran.Find("chooseParent").gameObject;
            GridPos = tran.Find("GridPos").gameObject;
            mono = curGo.transform.GetComponent<MonoBehaviour>();
            tiantian = curGo.transform.Find("tiantian").gameObject;
            SoundManager.instance.Speaking(tiantian, "talk",SoundManager.SoundType.SOUND, 3, delegate () { MaskState(true); }, delegate () { MaskState(false); });
            OnInit();
            InitClass();
            checkname = "";
            curindex = 0;
            SoundManager.instance.BgSoundPart1();
            right = GridPos.transform.Find("right").gameObject;
            left = GridPos.transform.Find("left").gameObject;
        }
        void OnInit()
        {
            slot = "images_1";
            animation_name = "animation";
            animation_idle = "idle";
            indexMaxNum = 8;
            succesNum = 0;
            numSound = 0;
            isOnSound = false;
            chooseDicts = new Dictionary<int, ChooseTransform>();
            movePosDicts = new Dictionary<int, List<GameObject>>();
            choose = null;
            chooseClone = null;
            for (int i = 0; i < GridPos.transform.childCount - 2; i++)
            {
                List<GameObject> curPos = new List<GameObject>();
                curPos.Add(GridPos.transform.GetChild(i).Find("inlay").gameObject);
                curPos.Add(GridPos.transform.GetChild(i).Find("inlay_pair").gameObject);
                movePosDicts.Add(i, curPos);
            }
            mono = curGo.GetComponent<MonoBehaviour>();
            curShader = spineState.transform.GetComponent<MeshRenderer>().material.shader;
            SpineManager.instance.DoAnimation(spineState.gameObject, animation_idle, false);
            AtlasAsset asset = spineState.SkeletonDataAsset.atlasAssets[0];
            SpineManager.instance.CreateRegionAttachmentByTexture(asset, slot, curGo, spineState.gameObject, curShader);
        }
        void InitClass()
        {
            for (int i = 0; i < chooseParent.transform.childCount; i++)
            {
                ChooseTransform curChoose = new ChooseTransform(chooseParent.transform.GetChild(i).gameObject, CheckChoose, MaskState, OverGame);
                curChoose.OnInit();
                chooseDicts.Add(i, curChoose);
            }
        }
        void OverGame()
        {
            if (isOnSound == false)
            {
                mono.StartCoroutine(waitSound());
            }
            else
            {
                OverWaitGame();
            }

        }
        void OverWaitGame()
        {
            succesNum++;
            if (succesNum >= indexMaxNum)
            {
                winSpine.transform.localScale = Vector3.one;
                winSpine1.transform.localScale = Vector3.one;
                winSpine2.transform.localScale = Vector3.one;
                SpineManager.instance.DoAnimation(winSpine.gameObject, "light", false);
                float time = SpineManager.instance.DoAnimation(winSpine1.gameObject, "star", false);
                SpineManager.instance.DoAnimation(winSpine2.gameObject, "text_1", false,delegate()
                {
                    SpineManager.instance.DoAnimation(winSpine2.gameObject, "text_2", false, delegate ()
                    {
                        SpineManager.instance.DoAnimation(winSpine2.gameObject, "text_3", false);
                    });
                });
                float time1 = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                mono.StartCoroutine(wait(time1, delegate ()
                {
                    SoundManager.instance.Speaking(tiantian, "talk", SoundManager.SoundType.SOUND, 4, null, () =>
                    {
                        tiantian.SetActive(true);
                        SpineManager.instance.DoAnimation(tiantian, "breath", true);
                    });
                }));
                float timeIndex = time / 0.02f;
                mono.StartCoroutine(ChangeTwo((int)timeIndex));
            }
            else
            {
                MaskState(false);
            }
        }
        IEnumerator ChangeTwo(int leng)
        {
            for (int i = 1; i <= leng; i++)
            {
                bgBack.transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 - ((float)i / (float)leng));
                yield return new WaitForSeconds(0.02f);
            }
            bgBack.transform.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
        }
        IEnumerator waitSound()
        {
            while(isOnSound == false)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            OverWaitGame();
        }
        void MaskState(bool b)
        {
           if(!b)
            {
                spineState.transform.SetParent(spineAnimation.transform);
                spineState.transform.localPosition = new Vector3(30, 0, 0);
                spineState.transform.localScale = Vector3.one;
            }
            mask.SetActive(b);
        }
        void pushAnimation(Sprite sprite)
        {
            SpineManager.instance.CreateRegionAttachmentByTexture(spineState, slot, sprite, curShader);
        }
        void CheckChoose(int index)
        {
            spineState.transform.SetParent(chooseDicts[index].obj.transform);
            spineState.transform.localPosition = new Vector3(0, -15.32f, 0);
            spineState.transform.localScale = Vector3.one;
            Debug.Log("CheckChoose---------"+chooseDicts[index].name);
            if (choose == null)
            {
                choose = chooseDicts[index];
                chooseDicts[index].SetMaster(true);
                pushAnimation(chooseDicts[index].sprite);
                checkname = "right";
                chooseDicts[index].PlayAnimation(spineState, animation_name, checkname,false);
            }
            else
            {
                if(choose.name == chooseDicts[index].name)
                {
                    Debug.Log("2222222222222222222");
                    //初始化状态
                    checkname = "right";
                    pushAnimation(chooseDicts[index].sprite);
                    chooseDicts[index].PlayAnimation(spineState, animation_name, checkname, false, () =>
                    {
                        chooseDicts[index].right.SetActive(false);
                        if(chooseClone != null)
                        {
                            chooseClone.error.SetActive(false);
                            chooseClone = null;
                        }
                        choose = null;
                    });
                    return;
                }
                int indexLastTime = 0;
                for (int i = 0; i < chooseDicts.Count; i++)
                {
                    chooseDicts[i].OnInit();
                    if (chooseDicts[i] == choose)
                    {
                        indexLastTime = i;
                    }
                }
                int max = Mathf.Max(indexLastTime, index);
                int min = Mathf.Min(indexLastTime, index);
                Debug.Log("max:" + max + "  min:" + min);
                if (max - min == 1 && (min == 0 || min % 2 == 0))
                {
                    checkname = "right";
                    pushAnimation(chooseDicts[index].sprite);
                    chooseDicts[index].PlayAnimation(spineState, animation_name, checkname,true);
                    Vector3 pos = movePosDicts[succesNum][0].transform.localPosition;
                    Vector2 v2 = new Vector2(pos.x, pos.y);
                    Vector3 pos_pair = movePosDicts[succesNum][1].transform.localPosition;
                    Vector2 v2_pair = new Vector2(pos_pair.x, pos_pair.y);
                    float posx = 0;
                    if (succesNum <= 3)
                    {
                        right.transform.SetParent(movePosDicts[succesNum][0].transform.parent);
                        posx = right.transform.localPosition.x;
                    }
                    else
                    {
                        left.transform.SetParent(movePosDicts[succesNum][0].transform.parent);
                        posx = left.transform.localPosition.x;
                    }
                    Debug.Log("posx:" + posx);
                    if (chooseDicts[index].timeMove> chooseDicts[indexLastTime].timeMove)
                    {
                        chooseDicts[indexLastTime].actEnd = ()=>{ };
                    }
                    else
                    {
                        chooseDicts[index].actEnd = () => { };
                    }

                    float time1 = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, numSound);
                    
                    mono.StartCoroutine(wait1(time1));
                    numSound++;
                    if (numSound == 2)
                    {
                        numSound = 0;
                    }
                    if (indexLastTime == 0 || indexLastTime % 2 == 0)
                    {
                        mono.StartCoroutine(wait(chooseDicts[index].timeAnimation,delegate ()
                        {
                            chooseDicts[indexLastTime].MoveChooseGrid(v2, posx);
                            chooseDicts[index].MoveChooseGrid(v2_pair, posx);
                            //lizi.transform.localScale = new Vector3(2,2,2);
                            //SpineManager.instance.DoAnimation(lizi, "lizi", false, delegate ()
                            //{
                            //    lizi.transform.localScale = Vector3.zero;
                            //    
                            //});
                        }));
                    }
                    else
                    {
                        mono.StartCoroutine(wait(chooseDicts[index].timeAnimation, delegate ()
                        {
                            chooseDicts[indexLastTime].MoveChooseGrid(v2_pair, posx);
                            chooseDicts[index].MoveChooseGrid(v2, posx);
                            //lizi.transform.localScale = new Vector3(2, 2, 2);
                            //SpineManager.instance.DoAnimation(lizi, "lizi", false, delegate ()
                            //{
                            //    lizi.transform.localScale = Vector3.zero;

                            //});
                        }));

                    }
                    choose = null;
                    chooseClone = null;
                }
                else
                {
                    Debug.Log("11111111111111111111111");
                    checkname = "error";
                    pushAnimation(chooseDicts[index].sprite);
                    if(chooseClone != null)
                    {
                        //初始化状态
                        chooseDicts[index].PlayAnimation(spineState, animation_name, checkname, false,() =>
                            {
                                chooseDicts[index].error.SetActive(false);
                                chooseClone = null;
                            });
                    }
                    else
                    {
                        chooseDicts[index].PlayAnimation(spineState, animation_name, checkname, false);
                        chooseClone = chooseDicts[index];
                    }
                }
                
            }
        }
        IEnumerator wait(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            act();
        }
        IEnumerator wait1(float time)
        {
            yield return new WaitForSeconds(time);
            isOnSound = true;
        }
    }
    public class ChooseTransform
    {
        public string name;
        public GameObject error;
        public GameObject right;
        bool isMater;
        public ILObject3DAction obj;
        public Sprite sprite;
        float MoveSize;
        public int index;
        Action<int> act;
        public int type;
        float speed;
        float speedNext;
        Action<bool> actMask;
        public Action actEnd;
        public float timeAnimation;
        public float timeMove;
        public float timeNextMove;
        public ChooseTransform(GameObject curGo, Action<int> _act, Action<bool> _actMask,Action _actEnd)
        {
            name = curGo.name;
            error = curGo.transform.Find("error").gameObject;
            right = curGo.transform.Find("right").gameObject;

            obj = curGo.transform.GetComponent<ILObject3DAction>();
            obj.OnMouseDownLua = OnMouseDown;
            index = obj.index;
            if (index == 0 || index % 2 == 0)
            {
                type = 0;
            }
            else
            {
                type = 1;
            }
            sprite = curGo.transform.GetComponent<SpriteRenderer>().sprite;
            MoveSize = 0.5f;
            speed = 20.0f;
            speedNext = 25.0f;
            act = _act;
            actMask = _actMask;
            actEnd = _actEnd;
            isMater = false;
            timeAnimation = 0;
            timeMove = 0;
            timeNextMove = 0;
           //Debug.LogErrorFormat("y: {0}", Formula(new Vector2(2, 9), new Vector2(5, 15), 2.5f));
        }
        public void OnInit()
        {
            if (isMater) return;
            error.SetActive(false);
            right.SetActive(false);
        }
        public void OnMouseDown(int index)
        {
            Debug.Log("OnMouseDown -------------" + name);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
            //if (isMater)
            //{
            //    return;
            //}
            act(index);
        }
        public void SetState(string name)
        {
            error.SetActive(false);
            right.SetActive(false);
            if (name == "right")
            {
                right.SetActive(true);
            }
            else
            {
                error.SetActive(true);
            }
        }
        public void PlayAnimation(SkeletonAnimation ske, string name,string checkname,bool isSuccess,Action act = null)
        {
            actMask(true);
            obj.transform.GetComponent<SpriteRenderer>().enabled = false;
            timeAnimation = SpineManager.instance.DoAnimation(ske.gameObject, name, false, delegate ()
            {
                if(!isSuccess)
                {
                    actMask(false);
                }
                obj.transform.GetComponent<SpriteRenderer>().enabled = true;
                
                SetState(checkname);
                if (act != null)
                {
                    act();
                }
            });
        }
        public float Formula(Vector2 v1,Vector2 v2,float _x = 0,float _y = 0)
        {
            float k, b;
            k = (v2.y - v1.y) / (v2.x - v1.x);
            b = (v2.x * v1.y - v1.x * v2.y) / (v2.x - v1.x);
            float num = 0;
            if(_x != 0)
            {
                num = k * _x + b;
            }
            else if(_y != 0)
            {
                num = (_y - b) / k;
            }
            return num;
        }
        public void MoveChooseGrid(Vector2 v2,float posx)
        {
            Vector2 v2self = new Vector2(obj.transform.localPosition.x, obj.transform.localPosition.y);
            Vector2 curpos = Vector2.zero;
            curpos.x = posx;
            curpos.y = Formula(v2, v2self, curpos.x);
            float dis = Vector2.Distance(curpos, v2self);
            //float dis = (v2 - v2self).sqrMagnitude;
            timeMove = dis / speed;
            dis = Vector2.Distance(curpos, v2);
            timeNextMove = dis / speedNext;
            actMask(true);

            //Debug.LogErrorFormat("target:{0},{1}", v2.x, v2.y);
            //Debug.LogErrorFormat("self:{0},{1}", v2self.x, v2self.y);
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 20);

            obj.transform.DOLocalMove(curpos, timeMove).OnComplete(()=>
            {
                obj.transform.DOLocalMove(v2, timeNextMove).OnComplete(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                });
            });
            obj.transform.DOScale(new Vector3(MoveSize, MoveSize, 1), timeMove + timeNextMove).OnComplete(delegate ()
             {
                 MoveEnd();
             });
        }
        public void MoveEnd()
        {
            error.transform.localScale = Vector3.zero;
            right.transform.localScale = Vector3.zero;
            actEnd();
        }
        public void SetMaster(bool b)
        {
            isMater = b;
        }
    }
}
