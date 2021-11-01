using DG.Tweening;
using Spine;
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
    public class CourseSackPart2
    {
        GameObject curGo;
        List<GameObject> childDatas;//图片资源
        int randomNum;              //随机数
        List<Sprite> onclickDatas;  //按钮图片资源
        List<SkeletonAnimation> onclickAnim;
        List<string> orders;        //图片的名字顺序
        int curNum;                 //当前图片最大数量

        WinTarget winTarget;
        GameObject mask;
        GameObject point;           //移动结束的位置
        GameObject dingding;
        Transform UI3D;
        GameObject win;
        GameObject black;
        GameObject soundMask;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            UI3D = curTrans.Find("UI3D");
            childDatas = new List<GameObject>();
            randomNum = 0;
            onclickDatas = new List<Sprite>();
            onclickAnim = new List<SkeletonAnimation>();
            orders = new List<string>();
            mask = UI3D.Find("mask").gameObject;
            point = UI3D.Find("targetPoint").gameObject;
            dingding = curTrans.Find("dingding").gameObject;
            curNum = 0;
            InitData();
            InitOnClick();
            win = curTrans.Find("win").gameObject;
            black = curTrans.Find("black").gameObject;
            winTarget = new WinTarget(win.transform);
            SoundManager.instance.BgSoundPart1();
            soundMask = UI3D.Find("soundMask").gameObject;
            UI3D.Find("mask").gameObject.SetActive(true);
        }
        public void InitOnClick()
        {
            for(int i = 0;i < onclickAnim.Count;i++)
            {
                Shader curShader = onclickAnim[i].GetComponent<MeshRenderer>().material.shader;
                SpineManager.instance.CreateRegionAttachmentByTexture(onclickAnim[i], "UI_barock", onclickDatas[i], curShader);
            }
        }
        public void InitData()
        {
            Transform trans = UI3D.Find("data");
            for(int i = 0;i < trans.childCount;i++)
            {
                childDatas.Add(trans.GetChild(i).gameObject);
            }
            trans = curGo.transform.Find("onclick_data");
            for(int i = 0;i < trans.childCount;i++)
            {
                onclickDatas.Add(trans.GetChild(i).GetComponent<SpriteRenderer>().sprite);
            }
            trans = UI3D.Find("animOnclick");
            for(int i = 0;i < trans.childCount;i++)
            {
                onclickAnim.Add(trans.GetChild(i).GetComponent<SkeletonAnimation>());
                Debug.Log(onclickAnim[i].GetComponent<ILObject3DAction>().index);
                onclickAnim[i].GetComponent<ILObject3DAction>().OnMouseDownLua = OnMouseDown;
            }
            RandomChilds();
            Speaking();
        }

        private void OnMouseDown(int index)
        {
            mask.SetActive(true);
            if (orders[curNum] == onclickAnim[index].name)
            {
                SpineManager.instance.DoAnimation(onclickAnim[index].gameObject, "animation", false,()=> 
                {
                    soundMask.SetActive(true);
                    float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, Random.Range(1, 4));
                    curGo.GetComponent<MonoBehaviour>().StartCoroutine(wait(time));
                    MoveChild();
                });
            }
            else
            {
                childDatas[curNum].transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 4.0f), 0.1f).OnComplete(() =>
                   {
                       childDatas[curNum].transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0f), 0.1f).OnComplete(() =>
                       {
                           childDatas[curNum].transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -4.0f), 0.1f).OnComplete(() =>
                           {
                               childDatas[curNum].transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0f), 0.1f);
                           });
                       });
                   });
                SpineManager.instance.DoAnimation(onclickAnim[index].gameObject, "click_error", false, () =>
                {
                    mask.SetActive(false);
                    soundMask.SetActive(true);
                    float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                    curGo.GetComponent<MonoBehaviour>().StartCoroutine(wait(time));
                });
            }
        }

        IEnumerator wait(float time)
        {
            yield return new WaitForSeconds(time);
            soundMask.SetActive(false);
        }
        public void Speaking()
        {
            SoundManager.instance.Speaking(dingding,"talk",SoundManager.SoundType.SOUND,0,null,()=>mask.SetActive(false));
        }
        public void RandomChilds()
        {
            for (int i = 0; i < childDatas.Count; i++)
            {
                int count = childDatas.Count;
                randomNum = Random.Range(i, count);
                GameObject curObj = childDatas[i];
                childDatas[i] = childDatas[randomNum];
                childDatas[randomNum] = curObj;
                float z = 38 + 0.1f * i;
                childDatas[i].transform.localPosition = new Vector3(0, 0, z);
                orders.Add(childDatas[i].name.Split('_')[1]);
            }
            Debug.Log(childDatas[curNum].name);
        }
        public void MoveChild()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            childDatas[curNum].transform.DOLocalMoveY(point.transform.localPosition.y, 0.5f).OnComplete(()=> 
            {
                PlayFinish();
            });
        }
       
        public void PlayFinish()
        {
            mask.SetActive(true);
            curNum++;
            if (curNum == orders.Count)
            {
                mask.SetActive(true);
                win.transform.localScale = Vector3.one;
                black.SetActive(true);
                Debug.Log("eeeee1");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                Debug.Log("eeeee2");
                winTarget.PlayFinish();
            }
            else
            {
                mask.SetActive(false);
            }
            Debug.Log(childDatas[curNum].name);
        }
    }
    public struct AnimationTarget
    {
        public GameObject obj;
        public List<string> name_animation;
        public string name_idle;
    }
    public class WinTarget
    {
        AnimationTarget bg;
        AnimationTarget lizi;
        AnimationTarget star;
        AnimationTarget win;
        Transform parent;
        List<string> names;
        public WinTarget(Transform _parent)
        {
            parent = _parent;
            ExposedList<Animation> animations = _parent.GetChild(0).GetComponent<SkeletonGraphic>().Skeleton.Data.Animations;
            names = new List<string>();
            for (int i = 0;i < animations.Count;i++)
            {
                names.Add(animations.Items[i].Name);
            }
            SetAnimation(ref bg, 0);
            SetAnimation(ref lizi, 1);
            SetAnimation(ref star, 2);
            SetAnimation(ref win, 3);
        }
        public void SetAnimation(ref AnimationTarget target, int index)
        {
            target = new AnimationTarget();
            target.obj = parent.GetChild(index).gameObject;
            target.obj.transform.localScale = Vector3.zero;
            target.name_animation = new List<string>();
            for(int i = 0;i < names.Count;i++)
            {
                if (names[i].Split('_')[1] == "animation" && names[i].Split('_')[0] == target.obj.name)
                {
                    target.name_animation.Add(names[i]);
                }
                else if(names[i].Split('_')[1] == "idle" && names[i].Split('_')[0] == target.obj.name)
                {
                    target.name_idle = names[i];
                }
            }
        }
        public void PlayFinish()
        {
            SpineManager.instance.PlayAnimationState(bg.obj.GetComponent<SkeletonGraphic>(), bg.name_idle);
            SpineManager.instance.PlayAnimationState(star.obj.GetComponent<SkeletonGraphic>(), star.name_animation[0]);
            SpineManager.instance.PlayAnimationState(lizi.obj.GetComponent<SkeletonGraphic>(), lizi.name_animation[0]);
            win.obj.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
            SpineManager.instance.DoAnimation(win.obj, win.name_animation[0], false, () =>
               {
                   bg.obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                   lizi.obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                   SpineManager.instance.DoAnimation(bg.obj, bg.name_idle);
                   SpineManager.instance.DoAnimation(lizi.obj, lizi.name_animation[0], false);
                   SpineManager.instance.DoAnimation(win.obj, win.name_animation[1], false, () =>
                        {
                            SpineManager.instance.DoAnimation(win.obj, win.name_idle);
                            star.obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                            SpineManager.instance.DoAnimation(star.obj, star.name_animation[0], false, () =>
                              {
                                  SpineManager.instance.DoAnimation(star.obj, star.name_idle);
                              });
                        });
               }
            );
        }
    }

}
